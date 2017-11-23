using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Rocket : NetworkBehaviour {

	public GameObject explosion;

	bool ex = false;

	void Start(){
		GetComponent<Rigidbody>().velocity = transform.forward * 40f;
	}

	void OnCollisionEnter(Collision col){
		if (hasAuthority && ex == false) {
			if (col.transform.tag == "Player" && col.transform.GetComponent<PlayerMovement> ().isLocalPlayer) {
				return;
			}
			CmdOnHit (col.contacts[0].point);
			ex = true;
		}
	}
	void OnCollisionStay(Collision col){
		if (hasAuthority && ex == false) {
			if (col.transform.tag == "Player" && col.transform.GetComponent<PlayerMovement> ().isLocalPlayer) {
				return;
			}
			CmdOnHit (col.contacts[0].point);
			ex = true;
		}
	}

	void OnTriggerEnter(Collider col){
		if (hasAuthority && ex == false) {
			if (col.transform.tag == "Player" && col.transform.GetComponent<PlayerMovement> ().isLocalPlayer) {
				return;
			}
			CmdOnHit (transform.position);
			ex = true;
		}
	}

	[Command]
	void CmdOnHit(Vector3 pos){
		GameObject g = Instantiate (explosion,pos,Quaternion.identity);
		NetworkServer.Spawn (g);
		NetworkServer.Destroy (gameObject);
	}
}
