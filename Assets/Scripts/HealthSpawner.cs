using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthSpawner : NetworkBehaviour {

    public float spawnTimer = 5f;
    float actualTimer = 0;
    GameObject actualHP;

    public GameObject healthPrefab;

	// Use this for initialization
	void Start () {
		if(isServer == false)
        {
            enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(actualHP != null)
        {
            return;
        }
        actualTimer -= Time.deltaTime;
        if (actualTimer <= 0)
        {
            actualTimer = spawnTimer;

            actualHP = Instantiate(healthPrefab,transform.position,transform.rotation);
            NetworkServer.Spawn(actualHP);
        }
	}
}
