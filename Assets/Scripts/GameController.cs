using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pacman
{
    public class GameController : MonoBehaviour
    {
        public GUIText ScoreText;
        public GameObject Life;

        private int score;
        private int NumberOfDots = 242;

        private void Start()
        {            
            score = GameObject.FindWithTag("Score").GetComponent<Score>().score;            
            ScoreText.text = "Score: " + score;
            
            Lifecounter();
            DisplayBonuses();
        }

        public void SetGameText(string Text)
        {
            this.GetComponent<GUIText>().text = Text;
        }

        public void UpdateScoreText(int Value)
        {
            score += Value;
            ScoreText.text = "Score: " + score;
        } 
        
        public void Lifecounter()
        {
            // remove exisint live counters
            var CurrentLivesCounter = GameObject.FindGameObjectsWithTag("LifeCounter");
            foreach(var Object in CurrentLivesCounter) { Destroy(Object.gameObject); }

            // instaniate new lives counters
            int NumberofLives = GameObject.FindWithTag("Score").GetComponent<Score>().Lives;
            for (int life=0; life < NumberofLives; life++)
            {
                float Xposition = -3.7f + (1.1f * life);
                Instantiate(Life, new Vector3(Xposition,-8.6f,0.0f), Quaternion.identity);
            }
        }

        public void DisplayBonuses()
        {
            string[] BonusNames = { "Cherry", "Strawberry","Orange","Apple","Melon","Galaxian","Bell","Key"};

            for (int Bonus = 0; Bonus < 8; Bonus++)
            {
                GameObject.FindWithTag(BonusNames[Bonus]).GetComponent<SpriteRenderer>().enabled = false;
            }

            if (GameObject.FindWithTag("Score").GetComponent<Score>().BonusesAquired != null)
            {
                var BonusList = GameObject.FindWithTag("Score").GetComponent<Score>().BonusesAquired;
                foreach(var Bonus in BonusList){
                    GameObject.FindWithTag(BonusNames[Bonus]).GetComponent<SpriteRenderer>().enabled = true;
                }                    
            }            
        }

        public void ReduceDots()
        {
            NumberOfDots--;

            if (NumberOfDots == 172 || NumberOfDots == 72)
            {
                GameObject.FindWithTag("Bonus").GetComponent<BonusFruit>().ActivateFruit();
            }
            
            if (NumberOfDots == 0)
            {
                GameObject.FindWithTag("Score").GetComponent<Score>().Level++;
                SceneManager.LoadScene("Pacman");                    
                }
        }
    }
}