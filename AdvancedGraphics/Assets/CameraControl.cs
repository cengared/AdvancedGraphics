using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float speed = 10.0F;
	public float rotationSpeed = 100.0F;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		float rotateV = Input.GetAxis ("Vertical") * rotationSpeed;
		float rotateH = Input.GetAxis ("Horizontal") * rotationSpeed;

		rotateV *= Time.deltaTime;
		rotateH *= Time.deltaTime;

		transform.Rotate (0, rotateH, 0);
		transform.Rotate (-rotateV, 0, 0);

		// raycast vector
		Vector3 fwd = transform.TransformDirection (Vector3.forward);
		if (Physics.Raycast (transform.position, fwd, 10)) {
			print ("There is something in front of the object!");
		}

		// draw the ray debug
		Debug.DrawRay(transform.position, fwd*100, Color.green);

		RaycastHit hit;
		//if (Physics.Raycast(transform.position, fwd, hit)


	}
}

