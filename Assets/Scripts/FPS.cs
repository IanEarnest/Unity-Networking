using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

	int framesPerSecond;
	float t;

	void Update () {
		// Every 2 seconds update framesPerSecond.
		t = Time.deltaTime + t;
		if(t>1){
			framesPerSecond = (int)(1.0f / Time.smoothDeltaTime);
			t = 0;
		}
	}

	void OnGUI(){
		if (Network.isServer){
			for(int i = 0; i < Network.connections.Length; i++) {
				GUI.Label(new Rect(Screen.width - 115, 15 * (i+1), 115, 30), "Player " + Network.connections[i] + " ping: " + Network.GetAveragePing(Network.connections[i]) + " ms");
			}
		}
		if (Network.isClient){
			GUI.Label(new Rect(Screen.width - 65, 15, 65, 30), "ping: " + Network.GetAveragePing(Network.connections[0]) + " ms");
		}
		GUI.Label(new Rect(Screen.width - 50, 0, 50, 30), "FPS: " + framesPerSecond);
	}
}
