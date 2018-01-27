using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using UnityEngine;
using System;

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
    }

    [Serializable]
    public class PlayerEvent : Event {
        public string player_name;
    }

    // Sent when new player is added to game
    [Serializable]
    public class PlayerAdded : PlayerEvent {
    }

    // Testing event. To be removed later.
    [Serializable]
    public class InputMessage : PlayerEvent {
        public string message;
    }

    // Sent right after game was set up, so Unity can show location people can connect to.
    [Serializable]
    public class RoomCode : Event {
        public string room_code;
    }

    // Sent whenever a player moves on their device.
    [Serializable]
    public class PlayerMove : PlayerEvent {
        public float pointer_x;
        public float pointer_y;
    }

    // Sent whenever a player stops moving using their device.
    [Serializable]
    public class PlayerRelease : PlayerEvent {
    }

    // Sent whenever a player is disconnected because of inactivity/broken socket.
    [Serializable]
    public class PlayerDisconnected : PlayerEvent {
    }

}

public class NetworkController : MonoBehaviour {

    public GameObject playerPrefab;
    Dictionary<string, Unit> players = new Dictionary<string, Unit>();

    void Awake() {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start() {
        startServer();
    }

    // Update is called once per frame
    void Update() {
        processNetworkMessage();
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

    void processNetworkMessage() {
        NetworkMessage msg = getItemFromQueue();
        if (msg != null) {
            // do some processing here, like update the player state
            // JsonUtility.FromJSON(msg.ToString());
            // Debug.Log(msg.ToString());
            SnappyServerEvent.Event networkevent = SnappyServerEvent.Event.DeserializeFromJSON(msg.ToString());
            if(networkevent is SnappyServerEvent.RoomCode){
                Debug.Log(((SnappyServerEvent.RoomCode)networkevent).room_code);
            }else if(networkevent is SnappyServerEvent.PlayerAdded){
                handleNetworkEvent((SnappyServerEvent.PlayerAdded)networkevent);
            }else if(networkevent is SnappyServerEvent.PlayerMove){
                handleNetworkEvent((SnappyServerEvent.PlayerMove)networkevent);
            }else if(networkevent is SnappyServerEvent.PlayerRelease){
                handleNetworkEvent((SnappyServerEvent.PlayerRelease)networkevent);
            }else{
                Debug.Log(networkevent);
            }
        }
    }

    void handleNetworkEvent(SnappyServerEvent.PlayerAdded message) {
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
        GameObject playerObject = Instantiate(playerPrefab, randomPos, Quaternion.identity) as GameObject;
        players.Add(message.player_name, playerObject.GetComponent<Unit>());
    }

    void handleNetworkEvent(SnappyServerEvent.PlayerMove message) {
        Unit player = players[message.player_name];
        player.addForce(100*message.pointer_x, 100*-message.pointer_y);
    }

    void handleNetworkEvent(SnappyServerEvent.PlayerRelease message) {
        Unit player = players[message.player_name];
        player.addForce(0, 0);
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

    static void connect() {
        if (client == null) {
            string server = "localhost";
            int port = 8002;
            client = new TcpClient(server, port);
            Stream stream = client.GetStream();
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }
    }

    public static void send(NetworkMessage msg) {
        msg.WriteToStream(writer);
        writer.Flush();
    }
}
