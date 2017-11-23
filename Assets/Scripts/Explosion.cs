using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour {



	void Start () {
		StartCoroutine (DestroyAndStuff());
	}

	IEnumerator DestroyAndStuff(){
		yield return new WaitForSeconds (0.2f);
		GetComponent<Collider> ().enabled = false;
		if (isServer) {
			yield return new WaitForSeconds (0.7f);

			NetworkServer.Destroy (gameObject);
		}
	}
}
