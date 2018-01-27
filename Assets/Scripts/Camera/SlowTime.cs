using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTime : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    public float timeSpeed = 1.0f;

	// Slow down time by an amount.
	void Update() {
        Time.timeScale = timeSpeed;
    }
}
