using UnityEngine;

namespace ImprovedTimers {
    /// <summary>
    /// Timer that counts down from a specific value to zero.
    /// </summary>
    public class CountdownTimer : Timer {
        public CountdownTimer(float value) : base(value) { }

        public override void Tick() {
            
            if (IsRunning && CurrentTime > 0) {
                CurrentTime -= Time.deltaTime;
            }

            if (IsRunning && CurrentTime <= 0) {
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
        public override float Progress => CurrentTime / initialTime;
    }
    
    public class CountdownTimerRepeat : Timer
    {
        private int _repeat;

        public CountdownTimerRepeat(float value, int repeat) : base(value)
        {
            _repeat = repeat - 1;
        }
        
        
        public override void Tick() {
            
            if (IsRunning && CurrentTime > 0) {
                CurrentTime -= Time.deltaTime;
            }

            if (IsRunning && CurrentTime <= 0) {
                if (_repeat > 0)
                {
                    // Reset
                    CurrentTime = initialTime;
                    
                    OnTimerRaised.Invoke();
                    _repeat--;
                    //Debug.Log("Repeat: " + _repeat);
                }
                else
                {
                    //Debug.Log("CountdownTimer Stop");
                    Stop();
                }
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
        public override float Progress => CurrentTime / initialTime;
    }
}
