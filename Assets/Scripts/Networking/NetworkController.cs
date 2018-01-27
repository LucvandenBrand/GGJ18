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
    Dictionary<string, Unit> players = new Dictionary<string, Unit>();


    //public GameObject DebugText;

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
        checkOtherControllers();
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

    public GameObject add_player(string player_name) {
        Debug.Log("Player Connected: " + player_name);
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 100);
        GameObject playerObject = Instantiate(playerPrefab, randomPos, Quaternion.identity) as GameObject;
        players.Add(player_name, playerObject.GetComponent<Unit>());
        return playerObject;
    }

    public void send_message(string player_name, string message){
        Debug.Log("Sent message by player " + player_name + ": " + message);
    }

    public void send_room_code(string room_code){
        Debug.Log("Room Code: " + room_code);
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

    List<String> inputStringsVertical = new List<String>{ "VerticalArrow", "VerticalWASD" };
    List<String> inputStringsHorizontal = new List<String> { "HorizontalArrow", "HorizontalWASD" };
    private void checkOtherControllers()
    {
        for (int i=0; i<inputStringsHorizontal.Count; i++)
        {
            if (Input.GetAxisRaw(inputStringsVertical[i]) > 0.5)
            {
                GameObject curPlayer = add_player(inputStringsVertical[i]);
                KeysToVirtual keys = curPlayer.AddComponent<KeysToVirtual>();
                keys.horizontalString = inputStringsVertical[i];
                keys.verticalString = inputStringsHorizontal[i];
                keys.toBeControlled = curPlayer.GetComponent<Unit>();

                inputStringsHorizontal.RemoveAt(i);
                inputStringsVertical.RemoveAt(i);
            }
        }
    }
}
