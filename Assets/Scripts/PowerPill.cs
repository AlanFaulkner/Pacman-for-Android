using UnityEngine;

namespace Pacman
{
    public class PowerPill : MonoBehaviour
    {
        private readonly int Score = 50;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name == "Pacman")
            {                
                GameObject.FindWithTag("GameController").GetComponent<GameController>().ReduceDots();
                GameObject.FindWithTag("GameController").GetComponent<GameController>().UpdateScoreText(Score);
                GameObject.FindWithTag("PacDotAudio").GetComponent<DotAudio>().PlayAudio();

                // Make Ghost Scared
                string[] GhostList = { "Blinky", "Pinky", "Inky", "Clyde" };

                foreach (var Ghost in GhostList)
                {
                    if (GameObject.FindWithTag(Ghost)) { GameObject.FindWithTag(Ghost).GetComponent<GhostAI>().Frightened(); }
                    else { Debug.Log(Ghost + ": is not active."); }
                }

                Destroy(gameObject);
            }
        }
    }
}