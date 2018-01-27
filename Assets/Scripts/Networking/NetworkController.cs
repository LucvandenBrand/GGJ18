using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using UnityEngine;
using System;

[Serializable]
public class SnappyServerEvent {
    public string type;

    public static SnappyServerEvent DeserializeFromJSON(string json_str) {
        SnappyServerEvent raw_event = JsonUtility.FromJson<SnappyServerEvent>(json_str);
        switch(raw_event.type){
            case "player_added" :
                return JsonUtility.FromJson<PlayerAddedServerEvent>(json_str);
            case "input_message" :
                return JsonUtility.FromJson<InputMessageServerEvent>(json_str);
            case "room_code" :
                return JsonUtility.FromJson<RoomCodeServerEvent>(json_str);
            default:
                return raw_event;
        }
    }
}

[Serializable]
public class PlayerAddedServerEvent : SnappyServerEvent {
    public string player_name;
}

[Serializable]
public class InputMessageServerEvent : SnappyServerEvent {
    public string player_name;
    public string message;
}

[Serializable]
public class RoomCodeServerEvent : SnappyServerEvent {
    public string room_code;
}


public class NetworkController : MonoBehaviour {

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

    static void processNetworkMessage() {
        NetworkMessage msg = getItemFromQueue();
        if (msg != null) {
      // do some processing here, like update the player state
            // JsonUtility.FromJSON(msg.ToString());
            Debug.Log(msg.ToString());
            SnappyServerEvent networkevent = SnappyServerEvent.DeserializeFromJSON(msg.ToString());
            Debug.Log(networkevent);
        }
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
