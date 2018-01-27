using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysToVirtual : MonoBehaviour {
    public string horizontalString;
    public string verticalString;
    public Unit toBeControlled;
	
	// Update is called once per frame
	void Update () {
        toBeControlled.addVirtualForce(Input.GetAxisRaw(verticalString), Input.GetAxisRaw(horizontalString));
    }
}
