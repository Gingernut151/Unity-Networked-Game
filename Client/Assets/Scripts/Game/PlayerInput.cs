using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SharedLibrary;

public class PlayerInput : MonoBehaviour
{
    private float turnSpeed = 60.0f;
    private float forwardSpeed = 20.0f;

   	void Start ()
    {
		
	}

	void Update ()
    {
        //Rotation
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].position.x < ((int)Screen.width / 2.0f))
            {
                transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            float moveHorizontal = Input.GetAxis("Horizontal") * 2.0f;

            transform.Rotate(new Vector3(0.0f, moveHorizontal, 0.0f));
        }

        // Movement
        this.transform.position += this.transform.forward * (forwardSpeed * Time.deltaTime);

        Vector3 pos = this.transform.position;
        pos.y = 0.0f;

        this.transform.SetPositionAndRotation(pos, this.transform.rotation);
    }


}
