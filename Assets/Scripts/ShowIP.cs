using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowIP : MonoBehaviour {

	// Use this for initialization
	void Start () {
      Debug.Log(Network.player.ipAddress);
      this.gameObject.GetComponent<Text>().text = "http://" + Network.player.ipAddress + ":4000";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
