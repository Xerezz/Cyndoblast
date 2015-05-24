using UnityEngine;
using System.Collections;

public class poisonGas : MonoBehaviour {

	float startTime;
	float mass = 0.0f;
	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}

	public float getMass(){
		return mass;
	}

	public void setMass(float m){
		mass = m;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime > 3) {
			float angle = Random.value * Mathf.PI*2;
			float xVelocity = 40*Mathf.Cos(angle) * -1 * 0.5f/Mathf.Exp(mass/Mathf.PI/2);
			float yVelocity = 40*Mathf.Sin(angle) * -1 * 0.5f/Mathf.Exp(mass/Mathf.PI/2);
			GameObject releaseMass = (GameObject)Network.Instantiate (Resources.Load ("Prefabs/mass", typeof(GameObject)), transform.position, Quaternion.identity, 0);
			object[] args = {
				mass,
				xVelocity,
				yVelocity
			};
			releaseMass.GetComponent<releasedMass> ().GetComponent<NetworkView>().RPC("setVariable", RPCMode.All,args);
			releaseMass.GetComponent<releasedMass> ().GetComponent<NetworkView>().RPC("makeReady", RPCMode.All,null);
			Destroy(gameObject);
		}
	}
}
