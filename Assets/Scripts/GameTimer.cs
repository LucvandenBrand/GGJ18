using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour {
    public float timerDuration;
    private float timeLeft;
    private bool running = false;
    private bool animationRunning = false;

    // Use this for initialization
    public void startRunning()
    {
        timeLeft = timerDuration;
        running = true;
        animationRunning = false;
    }

    public void stopRunning()
    {
        running = false;
    }

    public bool isRunning()
    {
        return running;
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 3)
            {
                if (!animationRunning)
                {
                    animationRunning = true;
                    animation();
                } 
                
            }
            if (timeLeft <= 0)
            {
                timerFinished();
            }
        }
    }

    void animation()
    {
        Debug.Log("Start animation");
    }

    void timerFinished()
    {
        Debug.Log("TimerDone");
        resetTimer();
    }

    private void resetTimer()
    {
        timeLeft = timerDuration;
        running = true;
        animationRunning = false;
    }
}
