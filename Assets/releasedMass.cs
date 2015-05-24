using UnityEngine;
using System.Collections;

public class releasedMass : MonoBehaviour {


	private float mass = 1.0f;
	private float xVelocity = 0;
	private float yVelocity = 0;
	private bool ready = false;

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3 (0, 0, 1);
	}
	[RPC]
	public void setVariable(float m, float x, float y){
		mass = m;
		xVelocity = x;
		yVelocity = y;
	}

	public float getMass(){
		return mass;
	}

	public bool getReady(){
		return ready;
	}

	public void setReady(){
		ready = true;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3 (Mathf.Sqrt(mass/Mathf.PI)/2, Mathf.Sqrt(mass/Mathf.PI)/2, 1);
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
		if (Mathf.Abs(xVelocity) < 0.1f) {
			xVelocity = 0;
		}
		if (Mathf.Abs(yVelocity) < 0.1f) {
			yVelocity = 0;
		}
		Vector3 bounds = Camera.main.WorldToViewportPoint (transform.position);
		if (bounds.x > 1)
			xVelocity = - Mathf.Abs(xVelocity);
		if (bounds.x < 0)
			xVelocity = Mathf.Abs(xVelocity);
		if (bounds.y > 1)
			yVelocity = - Mathf.Abs(yVelocity);
		if (bounds.y < 0)
			yVelocity = Mathf.Abs(yVelocity);
		transform.Translate (new Vector3(xVelocity,yVelocity,0) * Time.deltaTime, Camera.main.transform);
	}
}
