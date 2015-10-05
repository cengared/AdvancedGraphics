#pragma strict

var xSpeed : float = 1.0f;
var ySpeed : float = 1.0f;
var zSpeed : float = 1.0f;

function Start () {
	

}

function Update () {
	transform.Translate(xSpeed*Time.deltaTime, ySpeed*Time.deltaTime, zSpeed*Time.deltaTime);
}
