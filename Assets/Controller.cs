using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	private float mass = 1.0f;
	private float xVelocity = 0;
	private float yVelocity = 0;
	private float charge = 0;
	private bool charging = false;
	private const int screenWidth = 80;
	private const int screenHeight = 40;
	private static int color = 1;

	// Use this for initialization
	void Start () {
		gameObject.tag = "Player";
		if (color == 1) {
			this.GetComponent<SpriteRenderer>().color = Color.blue;
		}else if (color == 2) {
			this.GetComponent<SpriteRenderer>().color = Color.red;
		}else if (color == 3) {
			this.GetComponent<SpriteRenderer>().color = Color.green;
		}else if (color == 4) {
			this.GetComponent<SpriteRenderer>().color = Color.black;
		}else if (color == 5) {
			this.GetComponent<SpriteRenderer>().color = Color.cyan;
		}else if (color == 6) {
			this.GetComponent<SpriteRenderer>().color = Color.white;
		}else if (color == 7) {
			this.GetComponent<SpriteRenderer>().color = Color.grey;
		}else if (color == 8) {
			this.GetComponent<SpriteRenderer>().color = Color.yellow;
		}
		color++;
	}

	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.name == "mass(Clone)" && coll.gameObject.GetComponent<releasedMass>().getReady()) {
			mass += coll.gameObject.GetComponent<releasedMass>().getMass();
			Destroy(coll.gameObject);
		}else if(coll.gameObject.name == "spike(Clone)") {
			mass *= 0.8f;
			if (mass < 0.1f) {
				mass = 0.1f;
			}
			xVelocity = - xVelocity;
			yVelocity = - yVelocity;
		}else if(coll.gameObject.name == "wall(Clone)") {
			xVelocity = - xVelocity;
			yVelocity = - yVelocity;
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
			this.GetComponent<Rigidbody2D>().mass = mass;
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
				xVelocity += 0.02f;
			} else {
				xVelocity -= 0.02f;
			}
			if (yVelocity < 0) {
				yVelocity += 0.02f;
			} else {
				yVelocity -= 0.02f;
			}
			if (Mathf.Abs (xVelocity) < 0.1f) {
				xVelocity = 0;
			}
			if (Mathf.Abs (yVelocity) < 0.1f) {
				yVelocity = 0;
			}
			Vector3 board = Camera.main.WorldToViewportPoint(transform.position);
			if (board.x > 1)
				xVelocity = - Mathf.Abs (xVelocity);
			if (board.x < 0)
				xVelocity = Mathf.Abs (xVelocity);
			if (board.y > 1)
				yVelocity = - Mathf.Abs (yVelocity);
			if (board.y < 0)
				yVelocity = Mathf.Abs (yVelocity);
			transform.Translate (new Vector3 (xVelocity, yVelocity, 0) * Time.deltaTime, Camera.main.transform);
			Camera.main.GetComponent<SmoothCamera>().target = transform;
		}
	}

	public float getMass(){
		return mass;
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
		GameObject releaseMass = (GameObject)Network.Instantiate (Resources.Load ("Prefabs/mass", typeof(GameObject)), transform.position, Quaternion.identity, 0);
		object[] args = {
			oldMass - mass,
			40 * Mathf.Cos (angle) * (oldMass - mass) / Mathf.Exp ((oldMass - mass) / Mathf.PI / 2),
			40 * Mathf.Sin (angle) * (oldMass - mass) / Mathf.Exp ((oldMass - mass) / Mathf.PI / 2)
		};
		releaseMass.GetComponent<releasedMass> ().GetComponent<NetworkView>().RPC("setVariable", RPCMode.All,args);
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
