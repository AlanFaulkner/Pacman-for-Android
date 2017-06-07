using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class Score : MonoBehaviour
    {
        private static Score Instance;

        public int score { get; set; }
        public int Lives { get; set; }
        public int Level { get; set; }
        public List<int> BonusesAquired { get; set; }
        
        private void Start()
        {
            score = 0;
            Lives = 3;
            Level = 0;
            BonusesAquired = new List<int> { };
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (Instance) { DestroyImmediate(gameObject); }
            else
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
        }
    }
}