using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	Rigidbody rb;

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

	void Start () {
		rb = GetComponent<Rigidbody> ();
		camPivot = transform.GetChild (0);
		if (isLocalPlayer) {
			Transform cam = Camera.main.transform;
			cam.SetParent (camPivot);
			cam.localPosition = new Vector3 (0.78f,1.62f,-4.97f);
		}
	}

	void Update(){
		if (isLocalPlayer) {
			rotX += Input.GetAxis ("Mouse X") * mouseSen;
			rotY += Input.GetAxis ("Mouse Y") * mouseSen;
			rotY = Mathf.Clamp (rotY, -85f, 85f);

			if (Input.GetButtonDown ("Jump")) {
				yDir = jumpForce;
			}
		} else {
			camPivot.rotation = Quaternion.AngleAxis (sRotX, Vector3.up) * Quaternion.AngleAxis (sRotY, Vector3.left);
		}
	}

	void FixedUpdate () {
		if (isLocalPlayer) {
			camPivot.rotation = Quaternion.AngleAxis (rotX, Vector3.up);

			dir = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			dir = camPivot.TransformDirection (dir);
			dir.y = 0;
			dir = dir.normalized * runSpeed;
			dir.y = yDir;

			rb.velocity = dir;
			yDir -= gravity * Time.deltaTime;
		}

	}

	void LateUpdate(){
		if (isLocalPlayer) {
			camPivot.rotation = Quaternion.AngleAxis (rotX, Vector3.up) * Quaternion.AngleAxis (rotY, Vector3.left);
			CmdUpdateRot (rotX, rotY);
		}
	}

	[Command]
	void CmdUpdateRot(float x, float y){
		sRotX = x;
		sRotY = y;
	}
}
