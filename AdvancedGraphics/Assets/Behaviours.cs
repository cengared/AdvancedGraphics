using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Behaviours : MonoBehaviour {
	public GUISkin guiSkin;
	private bool normalMode = false;
	private List<GameObject> characters;
	private Dictionary<string, Vector2> rockVectors;
	private Dictionary<string, Vector2> wallVectors;
	private string guiMessage;
	private Rect guiRect;
	private float cTime = 0.0f, sTime = 0.0f, waitTime = 3.0f;
	private bool showGui = false;
	private bool decreaseLight = false;
	private float normalAmbientIntensity;
	private float normalLightIntensity;
	private Light directionalLight;
	
	// Use this for initialization
	void Start () {
		guiMessage = "";
		guiRect = new Rect(0, 0, Screen.width, Screen.height);
		normalAmbientIntensity = RenderSettings.ambientIntensity;
		GameObject lightObj = GameObject.Find ("Directional Light");
		directionalLight = lightObj.GetComponent<Light> ();
		normalLightIntensity = directionalLight.intensity;

		GameObject temp = GameObject.Find ("NPCs");
		characters = new List<GameObject>();
		foreach (Transform t in temp.transform)
			characters.Add (t.gameObject);

		rockVectors = new Dictionary<string, Vector2> ();
		temp = GameObject.Find ("InteriorLandscape/Rocks");
		foreach (Transform t in temp.transform) {
			Renderer rend = t.GetComponent<Renderer> ();
			rockVectors.Add(t.name, rend.material.mainTextureScale);
		}

		wallVectors = new Dictionary<string, Vector2> ();
		temp = GameObject.Find ("Park/Walls");
		foreach (Transform t in temp.transform) {
			Renderer rend = t.GetComponent<Renderer> ();
			wallVectors.Add(t.name, rend.material.mainTextureScale);
		}
	}

	void OnGUI() {
		GUI.skin = guiSkin;
		if (showGui)
			GUI.Label(guiRect, guiMessage);
	}

	// Update is called once per frame
	void Update () {
		cTime = Time.time;
		if (Input.GetKeyUp (KeyCode.Return)) {
			sTime = Time.time;
			normalMode = !normalMode;
			if (normalMode) 
				guiMessage = "Normal Viewpoint Enabled";
			else 
				guiMessage = "Autistic Viewpoint Enabled";
			showGui = true;
		}

		if (sTime != 0.0f) {
			if (cTime - sTime > waitTime) {
				sTime = 0.0f;
				showGui = false;
			}
		}

		if (!normalMode) {
			foreach(GameObject c in characters) {
				float dist = Vector3.Distance(transform.position, c.transform.position);
				if (dist < 5f) {
					c.transform.LookAt(transform.position);
				}
			}
			if (decreaseLight && (RenderSettings.ambientIntensity < 2.9f || directionalLight.intensity > 0.1f)) {
				RenderSettings.ambientIntensity = RenderSettings.ambientIntensity / 0.99f;
				directionalLight.intensity = directionalLight.intensity * 0.99f;
			}
		}

	}

	void OnTriggerEnter(Collider other) {
		if (!normalMode) {
			if (other.tag == "rock") {
				Renderer rend = other.GetComponent<Renderer> ();
				Vector2 old = rend.material.mainTextureScale;
				rend.material.mainTextureScale = new Vector2 (old.x / 4, old.y / 4);
			}
			if (other.tag == "wall") {
				Renderer rend = other.GetComponent<Renderer> ();
				Vector2 old = rend.material.mainTextureScale;
				rend.material.mainTextureScale = new Vector2 (old.x / 2, old.y / 2);
			}
			if (other.tag == "tree")
				decreaseLight = true;
		}

	}

	void OnTriggerExit(Collider other) {
		if (!normalMode) {
			if (other.tag == "rock") {
				Vector2 reset = rockVectors [other.name];
				Renderer rend = other.GetComponent<Renderer> ();
				rend.material.mainTextureScale = reset;
			}
			if (other.tag == "wall") {
				Vector2 reset = wallVectors [other.name];
				Renderer rend = other.GetComponent<Renderer> ();
				rend.material.mainTextureScale = reset;
			}
			if (other.tag == "tree") {
				decreaseLight = false;
				RenderSettings.ambientIntensity = normalAmbientIntensity;
				directionalLight.intensity = normalLightIntensity;
			}
		}

	}

}
