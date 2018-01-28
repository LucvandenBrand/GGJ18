using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {
    private NetworkController nc;
    public GameObject circle;
    public GameObject textInvection;
    private GameObject curCircle, curText;

    private bool lobbyOpen = false;
	void Start () {
        nc = GetComponent<NetworkController>();
        closeLobby();
    }

    public void closeLobby()
    {
        lobbyOpen = false;
        Destroy(curCircle);
        Destroy(curText);
    }

    public void openLobby()
    {
        GetComponent<GameMaster>().ResetPlayers();
        curCircle = Instantiate(circle);
        curText = Instantiate(textInvection);
        lobbyOpen = true;
    }

    void Update()
    {
        if (lobbyOpen)
        {
            if (nc.players.Count >= 2)
            {
                if (checkProperLocation())
                {
                    Debug.Log("Begin!");
                    beginGame();
                }
            }
        }
    }

    private void beginGame()
    {
        closeLobby();
        GetComponent<GameTimer>().startRunning();
    }

    private bool checkProperLocation()
    {
        List<Unit> playerList = new List<Unit>(nc.players.Values);
        Vector3 middle = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        foreach (Unit player in playerList)
        {
            if (Mathf.Sqrt(Mathf.Pow(Mathf.Abs(player.transform.position.x), 2) + Mathf.Pow(Mathf.Abs(player.transform.position.y), 2)) > 5)
            {
                return false;
            }
        }
        return true;
    }
}
