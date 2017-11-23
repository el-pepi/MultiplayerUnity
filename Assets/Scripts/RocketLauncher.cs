using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RocketLauncher : NetworkBehaviour {

	public float coolDown = 0.7f;

	float actualCoolDown = 0;
	public GameObject rocketPrefab;

	public Transform cam;

	//CharacterController cc;

	PlayerMovement pm;

	void Start () {
	//	cc = GetComponent<CharacterController> ();
		if (isLocalPlayer == false) {
			enabled = false;
		} else {
			pm = GetComponent<PlayerMovement> ();
		}
	}

	void LateUpdate () {
		if (pm.dead) {
			return;
		}

		if (actualCoolDown > 0f) {
			actualCoolDown -= Time.deltaTime;
		} else {
			if (Input.GetButtonDown ("Fire1")) {
				Vector3 pos = transform.position + Camera.main.transform.forward * 1.5f;
				Vector3 dir = cam.position + cam.forward * 20f - pos;

				CmdShoot (pos,dir);

				Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}

	[Command]
	void CmdShoot(Vector3 pos, Vector3 dir){
		GameObject b = Instantiate (rocketPrefab,pos,Quaternion.LookRotation(dir));

		NetworkServer.SpawnWithClientAuthority (b,connectionToClient);
	}
}
