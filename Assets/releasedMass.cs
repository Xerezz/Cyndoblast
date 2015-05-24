﻿using UnityEngine;
using System.Collections;

public class releasedMass : MonoBehaviour {


	private float mass = 1.0f;
	private float xVelocity = 0;
	private float yVelocity = 0;
	private bool ready = false;
	private const int screenWidth = 80;
	private const int screenHeight = 40;

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
	[RPC]
	public void makeReady(){
		ready = true;
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

	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.name == "spike(Clone)") {
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
	
	// Update is called once per frame
	void Update () {
		if(this.GetComponent<NetworkView>().isMine){
			object[] args = {
				mass,
				xVelocity,
				yVelocity
			};
			this.GetComponent<NetworkView>().RPC("setVariable", RPCMode.Others,args);
		}
		transform.localScale = new Vector3 (Mathf.Sqrt(mass/Mathf.PI)/2, Mathf.Sqrt(mass/Mathf.PI)/2, 1);
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
		if (Mathf.Abs(xVelocity) < 0.1f) {
			xVelocity = 0;
		}
		if (Mathf.Abs(yVelocity) < 0.1f) {
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
		transform.Translate (new Vector3(xVelocity,yVelocity,0) * Time.deltaTime, Camera.main.transform);
	}
}
