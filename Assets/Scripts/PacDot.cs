using UnityEngine;

namespace Pacman
{
    public class PacDot : MonoBehaviour
    {
        private readonly int Score = 10;
        private GameController gameController;
        private DotAudio dotAudio;

        private void Start()
        {
            GameObject gameControllObject = GameObject.FindWithTag("GameController");
            if (gameControllObject != null)
            {
                gameController = gameControllObject.GetComponent<GameController>();
            }
            else
            {
                Debug.Log("GameController script not found!");
            }

            GameObject controllObject = GameObject.FindWithTag("PacDotAudio");
            dotAudio = controllObject.GetComponent<DotAudio>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name == "Pacman")
            {                
                
                gameController.ReduceDots();
                gameController.UpdateScoreText(Score);
                dotAudio.PlayAudio();
                Destroy(gameObject);
            }
        }
    }
}