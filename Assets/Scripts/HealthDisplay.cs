using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {

	public Text damageText;
	public static HealthDisplay instance;

	public GameObject textPref;
	Dictionary<Transform,Text> texts = new Dictionary<Transform, Text>();

	void Start(){
		instance = this;
	}

	public void PlayerDamageUpdate(float damage){

		damageText.text = "Damage: " + damage.ToString("0") + "%"; 
	}

	public void CreateText(Transform trans){
		Text t = Instantiate(textPref).GetComponent<Text>();
		texts.Add (trans, t);
		t.transform.SetParent (transform);
	}

	void FixedUpdate(){
		foreach (KeyValuePair<Transform,Text> kv in texts) {
			if (Vector3.Angle (Camera.main.transform.forward, kv.Key.position - Camera.main.transform.position) > 90f) {
				kv.Value.rectTransform.position = new Vector3 (-100, -100, 0);
			} else {
				Vector3 p = Camera.main.WorldToScreenPoint (kv.Key.position);
				p.z = 0;
				kv.Value.rectTransform.position = p;
			}
		}
	}

	public void UpdateRemoteDamage(Transform trans, float damage){
		texts[trans].text = damage.ToString("0") + "%";
		//texts[trans].rectTransform.position = Camera.main.WorldToScreenPoint(trans.position);
	}
}
