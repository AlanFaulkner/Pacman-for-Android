using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman {

    public class BonusFruit : MonoBehaviour {

        public GameObject[] TypeOfFruit;               
        private bool Active = false;
        private float BonusTimer = 5f;
        private int CurrentLevel;

        private void Start()
        {
            CurrentLevel = GameObject.FindWithTag("Score").GetComponent<Score>().Level;
            if (CurrentLevel > 7) { CurrentLevel = 7; }
        }

        private void Update()
        {
            if (Active)
            {
                BonusTimer -= Time.deltaTime;
                if (BonusTimer < 0)
                {
                    Active = false;

                    var CurrentLivesCounter = GameObject.FindGameObjectsWithTag("BonusFruit");
                    foreach (var Object in CurrentLivesCounter) { Destroy(Object.gameObject); }
                }
            }
        }

        public void ActivateFruit()
        {
            BonusTimer = 7f;
            Active = true;            
            Instantiate(TypeOfFruit[CurrentLevel], new Vector3(0, -1, 0),Quaternion.identity);
        }        
    }
}