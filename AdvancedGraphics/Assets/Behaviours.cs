using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Behaviours : MonoBehaviour {
	public GUISkin guiSkin;
	private bool normalMode = true;
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
	private bool colliding = false;
		
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
		if (Input.GetKeyDown (KeyCode.Return)) {
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
				guiMessage = "";
			}
		}

		if (!normalMode) {
			bool toReset = false;
			foreach(GameObject c in characters) {
				float dist = Vector3.Distance(transform.position, c.transform.position);
				float step = 2.0f * Time.deltaTime;
				Vector3 oldDirection = c.transform.position - c.transform.forward;
				if (c.tag == "standing") {
					if (dist < 4f) {
						if (Input.GetKeyDown (KeyCode.Space)) {
							guiMessage = "This is how it can seem that everyone is looking at you.";
							showGui = true;
							sTime = Time.time;
						}
						Vector3 direction = transform.position - c.transform.position;
						Vector3 newDirection = Vector3.RotateTowards(c.transform.forward, direction, step, 0.0f);
						c.transform.rotation = Quaternion.LookRotation(newDirection);
						toReset = true;
					} else {
						if (toReset) {
							Vector3 resetDirection = Vector3.RotateTowards(c.transform.forward, oldDirection, step, 0.0f);
							c.transform.rotation = Quaternion.LookRotation(resetDirection);
						}
					}
				}
				if (c.tag == "running") {
					if (dist < 5f) {
						if (Input.GetKeyDown (KeyCode.Space)) {
							guiMessage = "This is how it can seem that everyone is looking at you no matter what they are doing.";
							showGui = true;
							sTime = Time.time;
						}
						c.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().setInterrupt(true);
						toReset = true;
					} else {
						if(toReset) {
							c.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().setInterrupt(false);
						}
					}
				}
			}
			if (decreaseLight) {
				if (RenderSettings.ambientIntensity < 2.9f || directionalLight.intensity > 0.1f) {
					RenderSettings.ambientIntensity = RenderSettings.ambientIntensity / 0.99f;
					directionalLight.intensity = directionalLight.intensity * 0.99f;
				}
			} else {
				RenderSettings.ambientIntensity = normalAmbientIntensity;
				directionalLight.intensity = normalLightIntensity;
			}
		
			if (normalMode) {
				GameObject[] g = GameObject.FindGameObjectsWithTag("running");
				foreach(GameObject o in g)
					if (o.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().getInterrupt())
						o.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().setInterrupt(false);
			}
		}

		if (colliding && Input.GetKeyDown (KeyCode.Space)) {
			showGui = true;
			sTime = Time.time;
		}

		if (Input.GetKey (KeyCode.Escape))
			Application.Quit ();

	}

	void OnTriggerEnter(Collider other) {
		if (!normalMode) {
			if (other.tag == "rock") {
				colliding = true;
				guiMessage = "This is to demonstrate how textures can looked zoomed in.";
				Renderer rend = other.GetComponent<Renderer> ();
				Vector2 old = rend.material.mainTextureScale;
				rend.material.mainTextureScale = new Vector2 (old.x / 4, old.y / 4);
			}
			if (other.tag == "wall") {
				colliding = true;
				guiMessage = "This is to demonstrate how surfaces can looked zoomed in.";
				Renderer rend = other.GetComponent<Renderer> ();
				Vector2 old = rend.material.mainTextureScale;
				rend.material.mainTextureScale = new Vector2 (old.x / 2, old.y / 2);
			}
			if (other.tag == "tree")  {
				colliding = true;
				guiMessage = "This is to demonstrate how attention can be focused on one object and everything else is blanked out.";
				decreaseLight = true;
			}

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
			if (other.tag == "tree") 
				decreaseLight = false;
		}
		colliding = false;
	}

}
