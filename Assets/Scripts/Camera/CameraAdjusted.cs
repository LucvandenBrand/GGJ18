using UnityEngine;

/* Scales the object every frame to perfectly fit the main camera.
 * Best used for quads that need to be used as a background. */
public class CameraAdjusted : MonoBehaviour
{	
	void Update ()
    {
        ScaleMesh();
	}

    void ScaleMesh()
    {
        float screenX = Screen.width;
        float screenY = Screen.height;
        float height = Camera.main.orthographicSize * 2;
        float width = height * screenX / screenY;
        transform.localScale = new Vector3(width, height, 1);
    }
}