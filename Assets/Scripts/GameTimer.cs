using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState {
	Starting,
	Lobby,
	Game,
	Score
}

public class GameTimer : MonoBehaviour {
	public float timerDuration;
	private float timeLeft;
	private bool running = false;
	private GameState state = GameState.Starting;
	private bool animationRunning = false;
	public Animator countdownAnimator;
	public Text timeField;

	[SerializeField] GameObject playerWinPrefab;
	[SerializeField] GameMaster gm;
	[SerializeField] int maxScore;
	
	
	public void startRound(){
        Debug.Log("straten");
		GetComponent<GameMaster>().ResetPlayers();
		state = GameState.Lobby;
        GetComponent<LobbyManager>().openLobby();
	}

	// Use this for initialization
	public void startRunning()
	{
		if (state != GameState.Game)
		{
			resetTimer();
		}
	}
	
	private void resetTimer()
	{
		timeLeft = timerDuration;
		state = GameState.Game;
		running = true;
		
		animationRunning = false;
		timeField.enabled = true;
	}

	public void stopRunning()
	{
		running = false;
		state = GameState.Score;
	}

	// Update is called once per frame
	void Update()
	{
		// running can hopefully be removed if isSick is implemented
		if (state == GameState.Game){//GetComponent<NetworkController>().IsSick() && running)
		{
// 			print("GAMING");
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
			if (timeLeft <= 1)
			{
				timerFinished();
			}
		} 
// 		else {
// 			if (timeLeft < 0){
// 				Restart();
// 			}
// 			// show the score and wait for restart
			
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
