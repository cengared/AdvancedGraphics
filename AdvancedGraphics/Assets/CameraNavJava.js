#pragma strict

var rotationSpeed : float = 100.0;
var colourTriggered : boolean = false;
var spinSpeed : float = 100.0;
var originalColour : Color;
var planeRenderer : Renderer;

function Start () {

}

function Update () {
	// Get the horizontal and vertical axis.
	// By default they are mapped to the arrow keys.
	// The value is in the range -1 to 1
	var rotateV : float = Input.GetAxis ("Vertical") * rotationSpeed;
	var rotateH : float = Input.GetAxis ("Horizontal") * rotationSpeed;
	
	rotateV *= Time.deltaTime;
	rotateH *= Time.deltaTime;

	transform.Rotate (0, rotateH, 0);
	transform.Rotate (-rotateV, 0, 0);
	
	// debug
	var fwd = transform.TransformDirection (Vector3.forward);
	if (Physics.Raycast (transform.position, fwd, 10)) {
		print ("There is something in front of the object!");
	}
	
	Debug.DrawRay(transform.position, fwd*100, Color.green);
	// end of debug
	
	// raycast hit
	var hit : RaycastHit;
	if (Physics.Raycast (transform.position, fwd, hit)) {
		var hitObject = hit.collider.name;
		
		switch(hitObject)
		{
			case "Cube1":
				print("hit Cube1");
				resetPlane();
				objectRotate(hit.collider.gameObject, Vector3.up);
				break;
			case "Cube2":
				print("hit Cube2");
				resetPlane();
				objectRotate(hit.collider.gameObject, Vector3.left);
				break;
			case "Sphere":
				print("hit Sphere");
				resetPlane();
				objectRotate(hit.collider.gameObject, Vector3.forward);
				break;
			case "Plane":
				print("hit Plane");
				planeRenderer = hit.collider.GetComponent.<Renderer>();
				changeToBlack(planeRenderer);
				break;
			default:
				print ("hit something odd!");
				resetPlane();
				break;
		}
	}
}

function changeToBlack(rend : Renderer)
{

	if (!colourTriggered)
	{
		originalColour = rend.material.color;
		rend.material.color = Color.black;
		colourTriggered = true;
	}
}

function resetPlane()
{
	planeRenderer.material.color = originalColour;
	colourTriggered = false;	
}

function objectRotate(spinObject : GameObject, direction : Vector3)
{
	spinObject.transform.Rotate(direction, spinSpeed*Time.deltaTime);
}


