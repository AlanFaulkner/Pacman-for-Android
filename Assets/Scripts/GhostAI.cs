using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pacman
{
    public class GhostAI : MonoBehaviour
    {
        /*
         * Describes the AI of a 'Ghost' in Pacman.
         * this includes movement around the maze
         * change of mode when power pill eaten
         */

        /*
        ##############
        ## Settings ##
        ##############
        */

        // ActivateGhost.
        public bool ActivateGhost { get; set; } 

        // Ghost Name
        public enum Name { Blinky, Pinky, Inky, Clyde };

        public Name GhostName;

        // Starting States of Ghost
        public enum Mode { Dead, Initalise, Chase, Frightened };

        private Mode CurrentMode = Mode.Dead;

        // Timers
        private readonly float[] SpawnWaitTimes = { 1, 10, 20, 35 }; // How long ghost spends in house

        private float FrightenedCountdownTimer;             // How long the ghost remains frightened/scared/ediable after pacman has eat power pill
        private float SpawnTimer;

        // Ghost Movement
        private enum Direction { Up, Right, Down, Left };

        private Direction CurrentDirection = Direction.Left;
        public float Speed;                 // How fast Ghost Travels
        private Vector2 Target;             // Where the ghost is headding
        private float MovementHorizantal;   //How far left or right ghost moves in a given time step
        private float MovementVertical;     // How far up or down ghost moves in a given time step
        private float DistanceTraveled;     // How far Ghost has traveled - makes sure ghost follows path -- needed for changing direction
        private Rigidbody2D Ghost;          // What we are moving
        private Vector2 LastPosition;       // used ot calculate distance trveled by ghost

        // Ghost Targets

        private readonly Vector2[] FrightenedTargets = { new Vector2(6.5f, 7.5f), new Vector2(-6.5f, 7.5f), new Vector2(6.5f, -7.5f), new Vector2(-6.5f, -7.5f) }; // Frightened Locations

        // Ghost Score level
        private int CurrentScoreLevel = 0;
        private readonly int[] ScoreLevel = { 200, 400, 800, 1600 };

        /*
         * #############
         * ## AI Code ##
         * #############
         */

        private void Start()
        {
            // Initalise components
            Ghost = GetComponent<Rigidbody2D>();
            LastPosition = transform.position;
            SpawnTimer = SpawnWaitTimes[Convert.ToInt32(GhostName)];
            MovementHorizantal = -0.1f;
            MovementVertical = 0f;
            StartCoroutine(MoveGhost());
            ActivateGhost = false;
        }

        private IEnumerator MoveGhost()
        {
            while (true)
            {
                GetDistanceTraveled();

                if (DistanceTraveled >= 0.49) // sprite size is 0.5 by 0.5 square grid
                {
                    if (transform.position == new Vector3(0.01f, 1.04f) && CurrentMode == Mode.Initalise)
                    {
                        Ghost.transform.position = new Vector3(0.01f, 2.04f, 0);
                        CurrentMode = Mode.Chase;
                    }

                    ChooseTarget();
                    ChooseDirectionToMove();
                    UpdateMovementVectors();
                    DistanceTraveled = 0;
                    LastPosition = transform.position;
                }

                Move();
                UpdateAnimation();

                yield return new WaitForSeconds(Speed / 100); // Smaller the delay the fast the ghost moves
            }
        }

        private void Update()
        {
            if (ActivateGhost)
            {
                FrightenedCountdownTimer -= Time.deltaTime;
                SpawnTimer -= Time.deltaTime;
                if (SpawnTimer < 0 && CurrentMode == Mode.Dead) { CurrentMode = Mode.Initalise; }
                if (FrightenedCountdownTimer < 0 && CurrentMode == Mode.Frightened)
                {
                    this.GetComponent<Animator>().SetBool("Frightened", false);
                    CurrentMode = Mode.Chase;
                    CurrentScoreLevel = 0;
                }
            }
        }

        private void ChooseTarget()
        {
            switch (CurrentMode)
            {
                case (Mode.Dead):
                    Target = new Vector2(UnityEngine.Random.Range(-7, 7), UnityEngine.Random.Range(-8, 8));
                    break;

                case (Mode.Initalise):
                    Target = new Vector2(0.01f, 2f);
                    break;

                case (Mode.Frightened):
                    Target = FrightenedTargets[Convert.ToInt32(GhostName)];
                    break;

                case (Mode.Chase):
                    GetChaseTarget();
                    break;
            }
        }

        private void GetChaseTarget()
        {
            switch (GhostName)
            {
                case (Name.Blinky):
                    Target = GameObject.Find("Pacman").transform.position;
                    break;

                case (Name.Pinky):
                    Target = TargetSurroundingPacman(2f);
                    break;

                case (Name.Inky):
                    Target = GameObject.Find("Pacman").transform.position + TargetSurroundingPacman(1f) + TargetSurroundingPacman(1f);
                    break;

                case (Name.Clyde):
                    if (Vector2.Distance(this.transform.position, GameObject.Find("Pacman").transform.position) < 2) { Target = FrightenedTargets[3]; }
                    else { Target = GameObject.Find("Pacman").transform.position; }
                    break;
            }
        }

        private Vector3 TargetSurroundingPacman(float Distance)
        {
            // return vector x distance in front of pacman.
            Vector3 target = GameObject.Find("Pacman").transform.position;
            var PacmanDirection = GameObject.Find("Pacman").GetComponent<PacmanMovement>().DirectionOfTravel;
            if (PacmanDirection == "Up" || PacmanDirection == "Down") { target.y += Distance; }
            else { target.x += Distance; }
            return target;
        }

        private double GetDistanceToTarget(float X, float Y, float Target_X, float Target_Y)
        {
            return Mathf.Pow(Target_X - X, 2) + Mathf.Pow(Target_Y - Y, 2);
        }

        private void ChooseDirectionToMove()
        {
            List<double> Distances = new List<double> { };
            List<Direction> PossibleDirections = GetPossibleDirections();

            foreach (var direction in PossibleDirections)
            {
                switch (direction)
                {
                    case (Direction.Up):
                        Distances.Add(GetDistanceToTarget(Ghost.transform.position.x, Ghost.transform.position.y + 0.01f, Target.x, Target.y));
                        break;

                    case (Direction.Right):
                        Distances.Add(GetDistanceToTarget(Ghost.transform.position.x + 0.01f, Ghost.transform.position.y, Target.x, Target.y));
                        break;

                    case (Direction.Down):
                        Distances.Add(GetDistanceToTarget(Ghost.transform.position.x, Ghost.transform.position.y - 0.01f, Target.x, Target.y));
                        break;

                    case (Direction.Left):
                        Distances.Add(GetDistanceToTarget(Ghost.transform.position.x - 0.01f, Ghost.transform.position.y, Target.x, Target.y));
                        break;
                }
            }

            CurrentDirection = PossibleDirections[Distances.IndexOf(Distances.Min())];
        }

        private List<Direction> GetPossibleDirections()
        {
            // Get all possible directions the ghost can travel in at a given time.
            // The ghost can not reverse direction. it must continue forward or turn 90 degrees clockwise or anit closkwise from its current heading

            List<Direction> ValidDirections = new List<Direction> { };
            List<Direction> DirectionList = new List<Direction> { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
            List<Vector2> DirectionVectors = new List<Vector2> { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };
            RaycastHit2D Hit;

            // Checks directions perpdicular to the current direct of travel to determine if ghost can move

            for (int Direction = 0; Direction < 4; Direction++)
            {
                if ((Convert.ToDouble(CurrentDirection) % 2 == 0 && Direction % 2 != 0) || (Convert.ToDouble(CurrentDirection) % 2 == 1 && Direction % 2 == 0))
                {
                    Hit = Physics2D.Raycast(Ghost.transform.position, DirectionVectors[Direction], 0.5f);

                    if (Hit.collider == null)
                    {
                        ValidDirections.Add(DirectionList[Direction]);
                    }
                }
            }

            // Checks ghost can contiune in same direction

            Hit = Physics2D.Raycast(Ghost.transform.position, DirectionVectors[Convert.ToInt32(CurrentDirection)], 0.5f);

            if (Hit.collider == null)
            {
                ValidDirections.Add(DirectionList[Convert.ToInt32(CurrentDirection)]);
            }

            return ValidDirections;
        }

        private void UpdateMovementVectors()
        {
            // Sets Movement vectors
            switch (CurrentDirection)
            {
                case (Direction.Up):
                    MovementHorizantal = 0;
                    MovementVertical = 0.1f;
                    break;

                case (Direction.Right):
                    MovementHorizantal = 0.1f;
                    MovementVertical = 0;
                    break;

                case (Direction.Down):
                    MovementHorizantal = 0;
                    MovementVertical = -0.1f;
                    break;

                case (Direction.Left):
                    MovementHorizantal = -0.1f;
                    MovementVertical = 0;
                    break;
            }
        }

        private void Move()
        {
            // Determin if Ghost is using tunnel
            if (transform.position.x >= 6.5 || transform.position.x <= -6.5)
            {
                Ghost.position = new Vector2(-transform.position.x, transform.position.y);
            }

            // Move Ghost
            Ghost.MovePosition(new Vector2(transform.position.x + MovementHorizantal, transform.position.y + MovementVertical));
        }

        private void ReverseDirection()
        {
            // Reverses the direction of travel of ghost.
            switch (CurrentDirection)
            {
                case (Direction.Up):
                    CurrentDirection = Direction.Down;
                    break;

                case (Direction.Right):
                    CurrentDirection = Direction.Left;
                    break;

                case (Direction.Down):
                    CurrentDirection = Direction.Up;
                    break;

                case (Direction.Left):
                    CurrentDirection = Direction.Right;
                    break;
            }
        }

        private void GetDistanceTraveled()
        {
            if (Convert.ToDouble(CurrentDirection) % 2 != 0) { DistanceTraveled = Math.Abs(LastPosition.x - transform.position.x); }
            else { DistanceTraveled = Math.Abs(LastPosition.y - transform.position.y); }
        }

        private void UpdateAnimation()
        {
            // Animation Parameters
            GetComponent<Animator>().SetFloat("Horizantal", MovementHorizantal);
            GetComponent<Animator>().SetFloat("Vertical", MovementVertical);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "Pacman" && CurrentMode != Mode.Frightened)
            {
                GameObject.FindWithTag("Pacman").GetComponent<PacmanMovement>().PacmanDeath();
            }
            else if (collision.gameObject.name == "Pacman" && CurrentMode == Mode.Frightened)
            {
                GhostDeath();
            }
        }

        public void Frightened()
        {
            if (CurrentMode == Mode.Chase)
            {
                CurrentMode = Mode.Frightened;
                this.GetComponent<Animator>().SetBool("Frightened", true);
                FrightenedCountdownTimer = 10 - GameObject.FindWithTag("Score").GetComponent<Score>().Level;
                if (FrightenedCountdownTimer < 0) { FrightenedCountdownTimer = 0; }
            }
        }

        public void ResetGhost()
        {
            CurrentMode = Mode.Dead;
            this.GetComponent<Animator>().SetBool("Frightened", false);
            transform.position = new Vector2(0.01f, 0.54f);
            SpawnTimer = SpawnWaitTimes[Convert.ToInt32(GhostName)] + FrightenedCountdownTimer;
            ActivateGhost = false;
        }

        private void GhostDeath()
        {
            GameObject.FindWithTag("GameController").GetComponent<GameController>().UpdateScoreText(ScoreLevel[CurrentScoreLevel]);
            CurrentScoreLevel++;
            if (CurrentScoreLevel == 4) { GameObject.FindGameObjectWithTag("Score").GetComponent<Score>().score += 3000; }
            GameObject.FindWithTag("GameController").GetComponent<GameController>().UpdateScoreText(ScoreLevel[CurrentScoreLevel]);
            ResetGhost();
        }
    }
}