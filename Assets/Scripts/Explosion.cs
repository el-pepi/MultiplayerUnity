using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour {

	float timer = 0.2f;

	void Start () {
		enabled = isServer;		
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0) {
			NetworkServer.Destroy (gameObject);
		}
	}
}
