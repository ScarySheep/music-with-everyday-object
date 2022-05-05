using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARDebug : MonoBehaviour
{
    private static GameObject debug;
    private static Text debugText;

    private static void checkInstance(){
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag("Debug");
        if(gameObjects.Length == 0){
            debug = Instantiate(Resources.Load("Prefabs/DebugUI", typeof(GameObject))) as GameObject;
            debugText = debug.GetComponentInChildren(typeof(Text)) as Text;
        }
    }

    private static void setText(string text){
        checkInstance();
        debugText.text = text;
    }

    public static void log(string text){
        setText(text);
    }
}
