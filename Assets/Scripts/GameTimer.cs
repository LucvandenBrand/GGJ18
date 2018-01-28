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
		timeLeft -= Time.deltaTime;
		if (running)
		{
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
		} else {
			if (timeLeft < 0){
				Restart();
			}
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
		resetTimer();
		if (winner == null){
			return;
		}
		Instantiate(playerWinPrefab, winner.transform.position, Quaternion.identity);
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
		timeField.enabled = true;
	}
	
	private void showScore(){
		
		stopRunning();
		timeLeft += 1;
		GetComponent<NetworkController>().FinalScreen();
	}
	
	void Restart() {
		
		print("RESTARTING");
		startRunning();
		GetComponent<GameMaster>().ResetPlayers();
	}
	
}
