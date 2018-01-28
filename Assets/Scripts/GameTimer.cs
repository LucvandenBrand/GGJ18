using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour {
    public float timerDuration;
    private float timeLeft;
    private bool running = false;
    private bool animationRunning = false;
    public Animator countdownAnimator;

    [SerializeField] GameObject playerWinPrefab;
    [SerializeField] GameMaster gm;
    [SerializeField] int maxScore;

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
		timeLeft -= Time.deltaTime;
        if (running)
        {
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
        } else {
			// show the score and wait for restart
			
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
        Instantiate(playerWinPrefab, winner.transform.position, Quaternion.identity);
        resetTimer();
        if (winner.score >= maxScore)
        {
            // Game Over
			showScore();
        }
    }

    private void resetTimer()
    {
        timeLeft = timerDuration;
        running = true;
        animationRunning = false;
    }
    
    private void showScore(){
		
		stopRunning();
		timeLeft += 30;
		GetComponent<NetworkController>().FinalScreen();
	}
		
}
