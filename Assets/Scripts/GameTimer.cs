using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {
    public float timerDuration;
    private float timeLeft;
    private bool running = false;
    private bool animationRunning = false;
    public Animator countdownAnimator;
    public Text timeField;

    [SerializeField] GameObject playerWinPrefab;
    [SerializeField] GameMaster gm;
    [SerializeField] int maxScore;

    // Use this for initialization
    public void startRunning()
    {
        if (!running)
        {
            resetTimer();
        }
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
            timeField.text = timeLeft.ToString("0");
            if (timeLeft <= 4)
            {
                if (!animationRunning)
                {
                    timeField.enabled = false;
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
        countdownAnimator.SetTrigger("StartCount");

    }

    void timerFinished()
    {
        Debug.Log("TimerDone");
        Unit winner = gm.DetermanScore();
        if (winner.score >= maxScore)
        {
            // Game Over
            stopRunning();
            GetComponent<NetworkController>().FinalScreen();
        }
        Instantiate(playerWinPrefab, winner.transform.position, Quaternion.identity);
        resetTimer();
    }

    private void resetTimer()
    {
        timeLeft = timerDuration;
        running = true;
        animationRunning = false;
        timeField.enabled = true;
    }
}
