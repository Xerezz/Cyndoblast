﻿using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private string typeName = "Cnidoblast";
	private string gameName = "RoomName";
	private bool started = false;

	private void StartServer(){
		//MasterServer.ipAddress = "127.0.0.1";
		Network.minimumAllocatableViewIDs = 500;
		Network.InitializeServer (8, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost (typeName, gameName, "An epic cell game!");
	}

	void OnServerInitialized(){
		Debug.Log ("Server Initialized");
		//SpawnPlayer ();
	}

	private void SpawnPlayer (){
		Network.Instantiate (Resources.Load ("Prefabs/player", typeof(GameObject)), new Vector3(0,0,0), Quaternion.identity, 0);
	}

	private void Begin(){
		this.GetComponent<NetworkView> ().RPC ("SpawnPlayers",RPCMode.All,null);
		started = true;
	}

	void OnGUI(){
		if (Network.isServer && !started) {
			if(GUI.Button(new Rect(100, 100, 250, 100), "Start"))
				Begin();
		}
		if (Network.isClient && !started) {
			GUI.Button(new Rect(100, 100, 250, 100), "Waiting for Server");
		}
		if (!Network.isClient && !Network.isServer) {
			if(GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();

			if(GUI.Button(new Rect(100, 210, 250, 100), "Refresh Host List"))
				RefreshHostList();

			gameName = GUI.TextField(new Rect(100, 320, 250, 100),gameName, 25);

			if(GUI.Button(new Rect(100, 430, 250, 100), "Quit"))
				Application.Quit();

			if(hostList != null){
				for(int i = 0; i < hostList.Length; i++){
					if(GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}

	private HostData[] hostList;

	private void RefreshHostList(){
		MasterServer.RequestHostList (typeName);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent){
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
		if (msEvent == MasterServerEvent.RegistrationSucceeded)
			Debug.Log ("Sever Registered");
	}

	private void JoinServer(HostData hostData){
		Network.Connect (hostData);
	}

	void OnConnectedToServer(){
		Debug.Log("Joined Server");
		//SpawnPlayer ();
	}

	[RPC]
	public void SpawnPlayers(){
		if (!started) {
			started = true;
			SpawnPlayer ();
		}
	}

	// Use this for initialization
	void Start () {
		Screen.SetResolution (1024, 768, false);
		MasterServer.ClearHostList();
		MasterServer.RequestHostList (typeName);
	}
	
	// Update is called once per frame
	void Update () {
		if(MasterServer.PollHostList().Length != 0){
			hostList = MasterServer.PollHostList();
			MasterServer.ClearHostList();
		}
		if (Network.isServer && started) {
			this.GetComponent<NetworkView> ().RPC ("SpawnPlayers",RPCMode.All,null);
			if (Random.value < 0.01) {
				GameObject releaseMass = (GameObject)Network.Instantiate (Resources.Load ("Prefabs/mass", typeof(GameObject)), Camera.main.ViewportToWorldPoint(new Vector3(Random.value, Random.value, 0)), Quaternion.identity, 0);
				object[] args = {
					Random.value,
					0.0f,
					0.0f
				};
				releaseMass.GetComponent<releasedMass> ().GetComponent<NetworkView>().RPC("setVariable", RPCMode.All,args);
			}
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Network.Disconnect();
			started = false;
		}
	}
}
