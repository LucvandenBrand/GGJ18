using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles all events of the intro animation.
public class IntroEvents : MonoBehaviour {

	[SerializeField] GameTimer timer;

	// Sneeze done.
    public void EndAnimation()
    {
        Debug.Log("Animation Done!");
		timer.startRound();
        
    }

    // Animation start.
    public void StartAnimation()
    {
        Debug.Log("Animation started!");
    }
}
