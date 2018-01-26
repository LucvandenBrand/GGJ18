using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using UnityEngine;


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
            Debug.Log(msg.ToString());
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

        send(NetworkMessage.FromString("The quick brown fox jumps over the lazy dog"));
        send(NetworkMessage.FromString("Bar"));
        send(NetworkMessage.FromString("Baz"));
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
