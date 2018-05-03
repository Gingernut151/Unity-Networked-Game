using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public string scene;
    public Color loadToColor = Color.black;
    public float fadeTime = 1.5f;

	public void ChangeScene ()
    {
        Initiate.Fade(scene, loadToColor, fadeTime);
    }

	public void SetScene(string sceneName)
    {
        scene = sceneName;
	}
}
