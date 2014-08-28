using UnityEngine;
using System.Collections;

public class Level1Menu : MonoBehaviour {

	Rect MainMenuWindowRect = new Rect(0, 50, 150, 100);
	
	// MainMenuWindow
	void MainMenuWindow(int windowID) {
		if (GUILayout.Button("Lobby")) {
			Application.LoadLevel("Lobby");
		}
	}
	
	void OnGUI(){
		MainMenuWindowRect = GUILayout.Window(0, MainMenuWindowRect, MainMenuWindow, "Main Menu");
	}
}
