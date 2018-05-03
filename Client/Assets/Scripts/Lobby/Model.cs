using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    private float speed = 30;

	void Start ()
    {

    }
	
	void Update ()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
