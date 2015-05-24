using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	private float mass = 1.0f;
	private float xVelocity = 0;
	private float yVelocity = 0;
	private float charge = 0;
	private bool charging = false;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.name == "mass(Clone)" && coll.gameObject.GetComponent<releasedMass>().getReady()) {
			mass += coll.gameObject.GetComponent<releasedMass>().getMass();
			Network.Destroy(coll.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D coll){
		if(coll.gameObject.name == "mass(Clone)") {
			coll.gameObject.GetComponent<releasedMass>().setReady();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (this.GetComponent<NetworkView>().isMine) {
			transform.localScale = new Vector3 (Mathf.Sqrt (mass / Mathf.PI) / 2, Mathf.Sqrt (mass / Mathf.PI) / 2, 1);
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			transform.rotation = Quaternion.LookRotation (Vector3.forward, mousePosition - transform.position);
			if (Input.GetMouseButton (0)) {
				charging = true;
				charge += 0.005f;
			} else {
				if (charging) {
					charging = false;
					if (charge > 0.5f)
						charge = 0.5f;
					move ();
				}
			}
			if (xVelocity < 0) {
				xVelocity += 0.05f;
			} else {
				xVelocity -= 0.05f;
			}
			if (yVelocity < 0) {
				yVelocity += 0.05f;
			} else {
				yVelocity -= 0.05f;
			}
			if (Mathf.Abs (xVelocity) < 0.1f) {
				xVelocity = 0;
			}
			if (Mathf.Abs (yVelocity) < 0.1f) {
				yVelocity = 0;
			}
			Vector3 bounds = Camera.main.WorldToViewportPoint (transform.position);
			if (bounds.x > 1)
				xVelocity = - Mathf.Abs (xVelocity);
			if (bounds.x < 0)
				xVelocity = Mathf.Abs (xVelocity);
			if (bounds.y > 1)
				yVelocity = - Mathf.Abs (yVelocity);
			if (bounds.y < 0)
				yVelocity = Mathf.Abs (yVelocity);
			transform.Translate (new Vector3 (xVelocity, yVelocity, 0) * Time.deltaTime, Camera.main.transform);
			
		}
	}

	void move(){
		float angle = (transform.eulerAngles.z - 270) * Mathf.Deg2Rad;
		xVelocity = 40*Mathf.Cos(angle) * -1 * charge/Mathf.Exp(mass/Mathf.PI/2);
		yVelocity = 40*Mathf.Sin(angle) * -1 * charge/Mathf.Exp(mass/Mathf.PI/2);
		float oldMass = mass;
		mass *= (1-charge)*0.9f;
		if (mass < 0.1f) {
			mass = 0.1f;
		}
		GameObject releasedMass = (GameObject)Network.Instantiate (Resources.Load ("Prefabs/mass", typeof(GameObject)), transform.position, Quaternion.identity, 0);
		object[] args = {
			oldMass - mass,
			40 * Mathf.Cos (angle) * (oldMass - mass) / Mathf.Exp ((oldMass - mass) / Mathf.PI / 2),
			40 * Mathf.Sin (angle) * (oldMass - mass) / Mathf.Exp ((oldMass - mass) / Mathf.PI / 2)
		};
		releasedMass.GetComponent<releasedMass> ().GetComponent<NetworkView>().RPC("setVariable", RPCMode.All,args);
		charge = 0;
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		Vector3 syncPosition = Vector3.zero;
		if (stream.isWriting) {
			syncPosition = transform.position;
			stream.Serialize(ref syncPosition);
		} else {
			stream.Serialize(ref syncPosition);
			transform.position = syncPosition;
		}
	}
}
