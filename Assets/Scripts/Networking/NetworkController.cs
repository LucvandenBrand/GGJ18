using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using UnityEngine;
using System;

using UnityEngine.UI;

namespace SnappyServerEvent {
	[Serializable]
	public class Event {
		public string type;

		public static Event DeserializeFromJSON(string json_str) {
			Event raw_event = JsonUtility.FromJson<Event>(json_str);
			switch(raw_event.type){
				case "player_added" :
					return JsonUtility.FromJson<PlayerAdded>(json_str);
				case "input_message" :
					return JsonUtility.FromJson<InputMessage>(json_str);
				case "room_code" :
					return JsonUtility.FromJson<RoomCode>(json_str);
				case "player_move" :
					return JsonUtility.FromJson<PlayerMove>(json_str);
				case "player_release" :
					return JsonUtility.FromJson<PlayerRelease>(json_str);
				case "player_disconnected" :
					return JsonUtility.FromJson<PlayerDisconnected>(json_str);
				default:
					return raw_event;
			}
		}

		public virtual void handle(NetworkController network_controller){}
	}

	[Serializable]
	public class PlayerEvent : Event {
		public string player_name;
	}

	// Sent when new player is added to game
	[Serializable]
	public class PlayerAdded : PlayerEvent {

		public override void handle(NetworkController network_controller){
			network_controller.add_player(this.player_name);
		}
	}

	// Testing event. To be removed later.
	[Serializable]
	public class InputMessage : PlayerEvent {
		public string message;
		public override void handle(NetworkController network_controller){
			network_controller.send_message(this.player_name, this.message);
		}
	}

	// Sent right after game was set up, so Unity can show location people can connect to.
	[Serializable]
	public class RoomCode : Event {
		public string room_code;

		public override void handle(NetworkController network_controller){
			network_controller.send_room_code(this.room_code);
		}
	}

	// Sent whenever a player moves on their device.
	[Serializable]
	public class PlayerMove : PlayerEvent {
		public float pointer_x;
		public float pointer_y;

		public override void handle(NetworkController network_controller){
			network_controller.player_move(this.player_name, this.pointer_x, this.pointer_y);
		}
	}

	// Sent whenever a player stops moving using their device.
	[Serializable]
	public class PlayerRelease : PlayerEvent {

		public override void handle(NetworkController network_controller){
			network_controller.player_release(this.player_name);
		}
	}

	// Sent whenever a player is disconnected because of inactivity/broken socket.
	[Serializable]
	public class PlayerDisconnected : PlayerEvent {
		public override void handle(NetworkController network_controller){
			network_controller.player_disconnected(this.player_name);
		}
	}
}

public class NetworkController : MonoBehaviour {

	public GameObject playerPrefab;
	public Dictionary<string, Unit> players = new Dictionary<string, Unit>();
	int healthyPlayers = 0;


	// The Animation to call on game over.
	public Animator cameraAnimator;

	// The text to show the room in.
	public Text roomText;

	// The text to show the score in.
	public Text scoreText;

  // The text to show the timer in usually, used for winner text on score screen.
  public Text timerText;

	void Awake() {
		DontDestroyOnLoad(this);
	}

	// Use this for initialization
	void Start() {
		startServer();
	}

	// Update is called once per frame
	void Update() {
		processNetworkMessages();
		checkManualControllers();
		ClampPlayers();
	}

	static TcpClient client = null;
	static BinaryReader reader = null;
	static BinaryWriter writer = null;
	static Thread networkThread = null;
	private static Queue<NetworkMessage> messageQueue = new Queue<NetworkMessage>();

	static void addItemToQueue(NetworkMessage item) {
		lock(messageQueue) {
			messageQueue.Enqueue(item);
		}
	}

	static NetworkMessage getItemFromQueue() {
		lock(messageQueue) {
			if (messageQueue.Count > 0) {
				return messageQueue.Dequeue();
			} else {
				return null;
			}
		}
	}

	void processNetworkMessages() {
		NetworkMessage msg = getItemFromQueue();
		while (msg != null) {
			SnappyServerEvent.Event networkevent = SnappyServerEvent.Event.DeserializeFromJSON(msg.ToString());
			networkevent.handle(this);

			msg = getItemFromQueue();
		}
	}

	public GameObject add_player(string player_name, bool localControls=false) {
		Debug.Log("Player Connected: " + player_name);
		Vector3 randomPos = new Vector3(10, UnityEngine.Random.Range(-1f, 1f), 5);
		GameObject playerObject = Instantiate(playerPrefab, randomPos, Quaternion.identity) as GameObject;
		playerObject.name = player_name;

		Unit playerUnit = playerObject.GetComponent<Unit>();
		playerUnit.name = player_name;
		
		playerObject.GetComponentInChildren<ConeMaker>().makeCone(localControls);

		players.Add(player_name, playerUnit);

		if (players.Count >= 2)
		{
			checkTimerStart();
		}

		return playerObject;
	}

	private void checkTimerStart()
	{
// 		GameTimer gameTimer = GetComponent<GameTimer>();
// 		gameTimer.startRunning();

	}

	public void send_message(string player_name, string message){
		Debug.Log("Sent message by player " + player_name + ": " + message);
	}

	public void send_room_code(string room_code){
		roomText.text = room_code;
	}

	public void player_move(string player_name, float pointer_x, float pointer_y) {
		Unit player = players[player_name];
		//Text DebugTextText = DebugText.GetComponent<Text>();
		//DebugTextText.text = "" + pointer_x;
		// Debug.Log(pointer_x);
		player.addVirtualForce(pointer_x, -pointer_y);
	}

	public void player_release(string player_name) {
		Unit player = players[player_name];
		player.addVirtualForce(0, 0);

	}

	// TODO: Proper disconnection logic.
	public void player_disconnected(string player_name) {
		Debug.Log("Player Disconnected: " + player_name);
		// Unit player = players[player_name];
		// player.addVirtualForce(0, 0);
	}

	static void startServer() {
		Debug.Log("Attempting to start server...");
		if (networkThread == null) {
			connect();
			networkThread = new Thread(() => {
					Debug.Log("NetworkThread starting...");
					while (reader != null) {
						NetworkMessage msg = NetworkMessage.ReadFromStream(reader);
						addItemToQueue(msg);
					}
					lock(networkThread) {
						networkThread = null;
					}
				});
			networkThread.Start();
		}
	}

	private static Stream stream;

	static void connect() {
		if (client == null) {
			string server = "localhost";
			int port = 8002;
			client = new TcpClient(server, port);
			stream = client.GetStream();
			reader = new BinaryReader(stream);
			writer = new BinaryWriter(stream);
		}
	}

	public static void send(NetworkMessage msg) {
		msg.WriteToStream(writer);
		writer.Flush();
	}

	public void OnDestroy()
	{
		networkThread.Abort();
		stream.Close();
	}

	// Runs for all non-networked controllers.
	List<String> inputStringsVertical = new List<String>{ "VerticalArrow", "VerticalWASD" };
	List<String> inputStringsHorizontal = new List<String> { "HorizontalArrow", "HorizontalWASD" };
	private void checkManualControllers()
	{
		for (int i=0; i<inputStringsHorizontal.Count; i++)
		{
			if (Input.GetAxisRaw(inputStringsVertical[i]) > 0.5)
			{
				GameObject curPlayer = add_player(inputStringsVertical[i], true);
				KeysToVirtual keys = curPlayer.AddComponent<KeysToVirtual>();
				keys.horizontalString = inputStringsVertical[i];
				keys.verticalString = inputStringsHorizontal[i];
				keys.toBeControlled = curPlayer.GetComponent<Unit>();

				inputStringsHorizontal.RemoveAt(i);
				inputStringsVertical.RemoveAt(i);
			}
		}
	}

	private void showScore() { 

		// Show the score
		List<Unit> playerList = new List<Unit>(players.Values);
		playerList.Sort((q, p) => p.score.CompareTo(q.score));

		string scoreString = "SCORE:\n";

		foreach (Unit player in playerList)
		{
			scoreString += player.name + ": " + player.score + "\n";
		}
		scoreText.text = scoreString;

    timerText.text = playerList[0].name + " is the Ultimate Virus";
	}

	/* Players are disallowed to move outside of the screen. Preventing death. */
	private void ClampPlayers()
	{
		foreach (Unit player in players.Values)
		{
			var pos = Camera.main.WorldToViewportPoint(player.transform.position);
			pos.x = Mathf.Clamp(pos.x, 0.1f, 0.9f);
			pos.y = Mathf.Clamp(pos.y, 0.1f, 0.9f);
			player.transform.position = Camera.main.ViewportToWorldPoint(pos);
		}
	}

	public void FinalScreen()
	{
		Debug.Log("Game Over!");
		cameraAnimator.SetTrigger("End-Sick");
		showScore();
	}
	
	public bool IsSick(){
		// TODO: implement this to check if the sick animation is playing
		print(cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sick"));
// 		return cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sick");
		return true;//cameraAnimator.IsPlaying("Sick");
	}
}
