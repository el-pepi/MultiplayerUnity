﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Rocket : NetworkBehaviour {

	public GameObject explosion;

	bool ex = false;


	/*void Start(){
		if (player.isLocalPlayer) {
			gameObject.layer = LayerMask.NameToLayer ("LocalRocket"); 
		}
	}*/

	void OnCollisionEnter(Collision col){
		if (hasAuthority) {
			if (col.transform.tag == "Player" && col.transform.GetComponent<PlayerMovement> ().isLocalPlayer) {
				return;
			}
			CmdOnHit (col.contacts[0].point);
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

	[Command]
	void CmdOnHit(Vector3 pos){
		GameObject g = Instantiate (explosion,pos,Quaternion.identity);
		NetworkServer.Spawn (g);
		NetworkServer.Destroy (gameObject);
	}
}