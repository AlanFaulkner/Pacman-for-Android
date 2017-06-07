using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class BounsFruitEaten : MonoBehaviour
    {
        private readonly int[] FruitScores = { 100, 300, 500, 700, 1000, 2000, 3000, 5000 };

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name == "Pacman")
            {
                GameObject.FindWithTag("PacDotAudio").GetComponent<DotAudio>().PlayAudio();
                if (!GameObject.FindWithTag("Score").GetComponent<Score>().BonusesAquired.Contains(GameObject.FindWithTag("Score").GetComponent<Score>().Level)) { GameObject.FindWithTag("Score").GetComponent<Score>().BonusesAquired.Add(GameObject.FindWithTag("Score").GetComponent<Score>().Level); }                
                GameObject.FindWithTag("GameController").GetComponent<GameController>().DisplayBonuses();
                GameObject.FindWithTag("GameController").GetComponent<GameController>().UpdateScoreText(FruitScores[GameObject.FindWithTag("Score").GetComponent<Score>().Level]);
                Destroy(gameObject);
            }
        }
    }
}
