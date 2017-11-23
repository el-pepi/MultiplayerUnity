using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	CharacterController cc;

	Vector3 dir = Vector3.zero;
	float yDir;

	public float runSpeed = 5f;
	public float gravity = 8f;
	public float jumpForce = 10f;

	[SyncVar]
	float sRotX;
	[SyncVar]
	float sRotY;

	float rotX;
	float rotY;

	public float mouseSen = 10;

	Transform camPivot;
	Transform cam;

	[SyncVar(hook = "OnDamageChange")]
	float damage;

	Vector3 explosionForce;

	NetworkStartPosition[] spawnPoints;

	ParticleSystem smoke;

	[SyncVar(hook = "OnSmokeToggle")]
	bool smokeEnabled = false;

	GameObject enterToRespawn;
	public bool dead = false;

	GameObject deathCam;

	void Start () {
		cc = GetComponent<CharacterController>();

		camPivot = transform.GetChild (0);

		smoke = GetComponentInChildren<ParticleSystem> ();

		if (isLocalPlayer) {
			Transform cam = Camera.main.transform;
			cam.SetParent (camPivot);
			cam.localPosition = Vector3.zero;//new Vector3 (0.78f, 1.62f, -4.97f);

			spawnPoints = FindObjectsOfType<NetworkStartPosition> ();

			enterToRespawn = GameObject.Find ("EnterToRespawn");
			enterToRespawn.SetActive (false);

			deathCam = GameObject.Find ("DeathCam");
			deathCam.SetActive (false);
		} else {
			HealthDisplay.instance.CreateText (transform);
		}

	}

	void Update(){
		if (dead) {
			if (Input.GetKeyDown (KeyCode.Return)) {
				transform.position = spawnPoints[Random.Range(0,spawnPoints.Length)].transform.position;
				enterToRespawn.SetActive (false);
				dead = false;
				rotX = 0;
				rotY = 0;
				deathCam.SetActive (false);

				CmdRespawn ();
			}
			return;
		}
		if (isLocalPlayer) {
			rotX += Input.GetAxis ("Mouse X") * mouseSen;
			rotY += Input.GetAxis ("Mouse Y") * mouseSen;
			rotY = Mathf.Clamp (rotY, -85f, 85f);

			if (cc.isGrounded && Input.GetButtonDown ("Jump")) {
				yDir = jumpForce;
			}
		} else {
			camPivot.rotation = Quaternion.AngleAxis (sRotX, Vector3.up) * Quaternion.AngleAxis (sRotY, Vector3.left);
		}
	}

	void FixedUpdate () {
		if (dead) {
			return;
		}
		if (isLocalPlayer) {
			camPivot.rotation = Quaternion.AngleAxis (rotX, Vector3.up);

			dir = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			dir = camPivot.TransformDirection (dir);
			dir.y = 0;
			dir = dir.normalized * runSpeed;


			if ((explosionForce + dir * 1.2f * Time.deltaTime).magnitude < explosionForce.magnitude) {
				explosionForce += dir * 1.2f * Time.deltaTime;
			}


			dir.y = yDir;

			dir += explosionForce;

			cc.Move (dir * Time.deltaTime);

			if (!cc.isGrounded) {
				yDir -= gravity * Time.deltaTime;
			} else {
				yDir = -1f;
				explosionForce = Vector3.zero;
				if (smokeEnabled) {
					CmdSetSmoke (false);
				}
			}



			if (transform.position.y < -20f) {
				dead = true;
				enterToRespawn.SetActive (true);

				deathCam.SetActive (true);
			}
		}
	}

	void LateUpdate(){
		if (dead) {
			return;
		}
		if (isLocalPlayer) {
			camPivot.rotation = Quaternion.AngleAxis (rotX, Vector3.up) * Quaternion.AngleAxis (rotY, Vector3.left);
			CmdUpdateRot (rotX, rotY);
		}
	}

	[Command]
	void CmdRespawn(){
		//transform.position = spawnPoints[Random.Range(0,spawnPoints.Length)].transform.position;
		damage = 0;
		smokeEnabled = false;
	}

	[Command]
	void CmdUpdateRot(float x, float y){
		sRotX = x;
		sRotY = y;
	}

	void OnTriggerEnter(Collider c){
		if (isLocalPlayer && c.tag == "Explosion") {
			float tmpdamage = damage + 10f;

			explosionForce = (transform.position - c.transform.position).normalized * tmpdamage;
			explosionForce.x *= 1.2f;
 			CmdSetSmoke (true);
			CmdAddDamage (10f);
		}
        if(isLocalPlayer && c.tag == "Health")
        {
            CmdDestroy(c.gameObject);
            CmdAddDamage(-10f);
        }
	}

    [Command]
    void CmdDestroy(GameObject g)
    {
        NetworkServer.Destroy(g);
    }

	[Command]
	void CmdAddDamage(float amount){
		damage += amount;
        if (damage < 0)
        {
            damage = 0;
        }
	}

	void OnDamageChange(float damage){
		if (isLocalPlayer) {
			HealthDisplay.instance.PlayerDamageUpdate (damage);
		} else {
			HealthDisplay.instance.UpdateRemoteDamage (transform,damage);
		}
		this.damage = damage;
	}

	[Command]
	void CmdSetSmoke(bool value){
		smokeEnabled = value;
	}

	void OnSmokeToggle(bool smokeEnabled){
		this.smokeEnabled = smokeEnabled;
		smoke.enableEmission = smokeEnabled;
	}
}
