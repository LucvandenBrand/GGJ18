using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadGameScene()
    {
        //Use a coroutine to load the Scene in the background
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}