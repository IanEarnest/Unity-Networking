using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	Rect MainMenuWindowRect = new Rect(0, 50, 150, 100);
	
	// MainMenuWindow
	void MainMenuWindow(int windowID) {
		if (GUILayout.Button("Lobby")) {
			Application.LoadLevel("Lobby");
		}
		//GUI.DragWindow();
	}
	
	void OnGUI(){
		MainMenuWindowRect = GUILayout.Window(0, MainMenuWindowRect, MainMenuWindow, "Main Menu");
	}
}
