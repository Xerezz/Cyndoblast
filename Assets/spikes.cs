using UnityEngine;
using System.Collections;

public class spikes : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.rotation = Quaternion.LookRotation (Vector3.forward, new Vector3(2.0f*Mathf.PI*Random.value,2.0f*Mathf.PI*Random.value,0));
		float scale = 0.2f + Random.value * 0.3f;
		transform.localScale = new Vector3 (scale, scale,0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
