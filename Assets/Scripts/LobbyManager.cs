using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {
    private NetworkController nc;
    //public Image startPanel;
    public GameObject circle;
    public GameObject textInvection;
    private GameObject curCircle, curText;

    private bool lobbyOpen = false;
	// Use this for initialization
	void Start () {
        nc = GetComponent<NetworkController>();
        closeLobby();
        openLobby(); // put this at end of animation
    }

    public void closeLobby()
    {
        lobbyOpen = false;
        Destroy(curCircle);
        Destroy(curText);
        //startPanel.enabled = false;
    }

    public void openLobby()
    {
        lobbyOpen = true;
        curCircle = Instantiate(circle);
        curText = Instantiate(textInvection);
        //startPanel.enabled = true;
    }

    // Update is called once per frame
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
