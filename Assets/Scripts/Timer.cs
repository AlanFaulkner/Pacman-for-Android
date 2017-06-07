using UnityEngine;

namespace Pacman
{
    public class CountdownTimer
    {
        private bool Running = false;
        private float Countdown { get; set; }

        public CountdownTimer(float timerLength)
        {
            Countdown = timerLength;
        }

        public void StartCountdownTimer()
        {
            Running = true;
        }

        public void StopCountdownTimer()
        {
            Running = false;
        }

        public void ResetTimer(float timerLength)
        {
            Countdown = timerLength;
        }

        private void Update()
        {
            if (Running && Countdown >= 0)
            {
                Countdown -= Time.deltaTime;
                if (Countdown <= 0) { Running = false; }
            }
        }
    }
}