using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Pacman
{
    public class PacmanMovement : MonoBehaviour
    {
        public float Speed;
        public string DirectionOfTravel { get; set; }
        private int DirectionHorizantial = 0;
        private int DirectionVertical = 0;

        [Flags]
        private enum SwipeDirections
        {
            None = 0,
            Up = 1,
            Down = 2,
            Right = 3,
            Left = 4
        }

        private Vector3 TouchPosition;
        private SwipeDirections Direction { get; set; }

        private void Update()
        {
            Direction = SwipeDirections.None;

            if (Input.GetMouseButtonDown(0))
            {
                TouchPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 deltaswipe = TouchPosition - Input.mousePosition;
                if (Mathf.Abs(deltaswipe.x) > 0.1 && Mathf.Abs(deltaswipe.x)> Mathf.Abs(deltaswipe.y))
                {
                    Direction |= (deltaswipe.x > 0) ? SwipeDirections.Left : SwipeDirections.Right;                    
                }
                else if (Mathf.Abs(deltaswipe.y) > 0.1)
                {
                    Direction |= (deltaswipe.y > 0) ? SwipeDirections.Down : SwipeDirections.Up;
                }
                Debug.Log(Direction.ToString());
            }

            if (Direction != SwipeDirections.None) {
                string[] GhostList = { "Blinky", "Pinky", "Inky", "Clyde" };

                foreach (var Ghost in GhostList)
                {
                    if (GameObject.FindWithTag(Ghost)) { GameObject.FindWithTag(Ghost).GetComponent<GhostAI>().ActivateGhost = true; }
                    else { Debug.Log(Ghost + ": is not active."); }
                }

                GameObject.FindWithTag("ReadyText").GetComponent<GUIText>().enabled = false;
            }
        }
        
        private void FixedUpdate()
        {
            var rb = GetComponent<Rigidbody2D>();

            if (Direction == SwipeDirections.Up) { DirectionHorizantial = 0; DirectionVertical = 1; DirectionOfTravel = "Up"; }
            if (Direction == SwipeDirections.Right) { DirectionHorizantial = 1; DirectionVertical = 0; DirectionOfTravel = "Right"; }
            if (Direction == SwipeDirections.Down) { DirectionHorizantial = 0; DirectionVertical = -1; DirectionOfTravel = "Down"; }
            if (Direction == SwipeDirections.Left) { DirectionHorizantial = -1; DirectionVertical = 0; DirectionOfTravel = "Left"; }

            // Determin if pacman using tunnel
            if (transform.position.x >= 6.5 || transform.position.x <= -6.5)
            {
                rb.position = new Vector2(-transform.position.x, transform.position.y);
            }

            rb.velocity = new Vector2(DirectionHorizantial * transform.localScale.x * Speed, DirectionVertical * transform.localScale.y * Speed);

            // Animation Parameters
            GetComponent<Animator>().SetFloat("DirX", DirectionHorizantial);
            GetComponent<Animator>().SetFloat("DirY", DirectionVertical);
        }

        private void ResetPacman()
        {
            DirectionHorizantial = 0;
            DirectionVertical = 0;
            DirectionOfTravel = "";
            this.transform.position = new Vector2(0.02f, -3.95f);            
        }

        public void PacmanDeath()
        {
            Debug.Log("loose life");
            ResetPacman();
            GameObject.FindWithTag("Score").GetComponent<Score>().Lives--;
            GameObject.FindWithTag("GameController").GetComponent<GameController>().Lifecounter();
            GameObject.FindWithTag("ReadyText").GetComponent<GUIText>().enabled = true;
            string[] GhostList = { "Blinky", "Pinky", "Inky", "Clyde" };

            foreach (var Ghost in GhostList)
            {
                if (GameObject.FindWithTag(Ghost)) { GameObject.FindWithTag(Ghost).GetComponent<GhostAI>().ResetGhost(); }
                else { Debug.Log(Ghost + ": is not active."); }
            }

            if (GameObject.FindWithTag("Score").GetComponent<Score>().Lives == 0)
            {
                // Gameover
                SceneManager.LoadScene("Pacman");
                GameObject.FindWithTag("Score").GetComponent<Score>().score = 0;
                GameObject.FindWithTag("Score").GetComponent<Score>().Lives = 3;
                GameObject.FindWithTag("Score").GetComponent<Score>().Level = 0;
            }
        }
    }
}