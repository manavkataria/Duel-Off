using UnityEngine;
using System.Collections;

/** Prototype Phase output
 * -- Temporary base script for Main Menu and next level loading 
 * -- progression for loading bar
 */ 
public class LoadingScreen : MonoBehaviour {

	//void Start () { DBAccess.instance.initDB(); }

    void OnGUI()
    {
       // if (GUI.Button(new Rect(Screen.width / 2 - 20, Screen.height / 2 - 20, 40, 40), "lvl2"))
        //{
         //   StartCoroutine(levelAsyncTest());
       // }
    }

    IEnumerator levelAsyncTest()
    {
        AsyncOperation async = Application.LoadLevelAsync("DemoScene");

        while (!async.isDone)
        {
            Debug.Log("Load Level Progress: " + async.progress);
            yield return null;
        }
        Debug.Log("Load Level is Complete!");
    }

}
