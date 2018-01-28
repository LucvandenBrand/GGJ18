using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles all events of the intro animation.
public class IntroEvents : MonoBehaviour {

	// Sneeze done.
    public void EndAnimation()
    {
        Debug.Log("Animation Done!");
    }

    // Animation start.
    public void StartAnimation()
    {
        Debug.Log("Animation started!");
    }
}
