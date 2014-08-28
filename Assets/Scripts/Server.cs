using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {

	// Project settings - Network - Debug, set to full.
	// Project settings - Player, set to run in background.

	// Improvements:
	// 
	// Compare to other multiplayer projects I have made.
	// Setup multiplayer players cubes and levels.
	// Make debug window persistent through different scenes.
	// Put on kongregate.
	// Look at other unity multiplayer games.
	// Look at ball3D.com lobby system.

	/*  Game rules
	Players: 2
	Time: Turn based, 2 minutes each 
	Win condition: completed in least turns. 
	
	Lobby
	Limit players: 2
	Auto join or start new server.
	Auto start game.
	Add chat.
	Add guest names.(random?)
	*/




	// Change gameTypeName and name to whatever default should be.
	string gameTypeName = "Game Type 1";
	string gameName = "Game 1";
	string netComment = "";
	string netPass = "";
	int maxPlayers = 3;
	int port = 25000;
	bool useNat;
	HostData[] hostData;
	string playerName = "Name";

	public GameObject playerObject;

	public GUIText debugText;

	string globalChat = "Chat";
	public string chatText;

	Vector2 chatScroll;
	Vector2 serverListScroll;
	Vector2 debugScroll;

	// Resize debug window.
	bool debugExpanded;
	bool debugHidden;
	Rect debugExpandRect = new Rect(300, 0, 350, 250);
	Rect debugNormalRect = new Rect(300, 0, 175, 150);
	Rect debugHiddenRect = new Rect(300, 0, 0, 0);

	// Window rectangles.
	Rect welcomeWindowRect = new Rect(0, 50, 150, 50);
	Rect serverListWindowRect = new Rect(270, 50, 500, 100);

	Rect serverWindowRect = new Rect(0, 50, 150, 50);
	Rect clientWindowRect = new Rect(0, 50, 150, 50);
	Rect chatWindowRect = new Rect(160, 50, 250, 250);
	Rect debugWindowRect = new Rect(300, 0, 175, 150);



	void Start(){
		// Check if need to use nat punchthroug, might not need this.
		useNat = !Network.HavePublicAddress();
		// Set player random name.
		playerName = "Steve" + Random.Range(1000,9999);

		// Hide debug at start.
		debugHidden = true;
		debugWindowRect = debugHiddenRect;
	}

	void Update () {
		// When connected, push users to level 1, try to keep everyone loaded on same level

		// and have their own player moving and seeing others.
	}

	void Awake(){
		// When GUI first loads refresh host list.
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(gameTypeName);
	}



	// WelcomeWindow
	void WelcomeWindow(int windowID) {
		if (GUILayout.Button("Main Menu")) {
			Application.LoadLevel("Main Menu");
		}
		if (GUILayout.Button("Refresh Host List")) {
			MasterServer.ClearHostList();
			MasterServer.RequestHostList(gameTypeName);
		}

		// Player name
		GUILayout.BeginHorizontal();
		GUILayout.Label("Your player name :", GUILayout.Width(120));
		playerName = GUILayout.TextField(playerName, 10, GUILayout.Width(120));
		GUILayout.EndHorizontal();

		GUILayout.Space (50);

		if(GUILayout.Button("Start a new server", GUILayout.Width(240))){
			// Use NAT punchthrough if no public IP present
			debugText.text += "\nGame Type Name: " + gameTypeName;
			debugText.text += "\nGame Name: " + gameName;
			if(netPass != ""){
				Network.incomingPassword = netPass;
				debugText.text += "\nPass: " + netPass;
			}
			if(port == 0){
				port = 25000;
			}
			if(maxPlayers == 0){
				maxPlayers = 5;
			}
			debugText.text += "\nMax Players: " + maxPlayers;
			debugText.text += "\nPort: " + port;
			debugText.text += "\nNat?: " + useNat;
			Network.InitializeServer(maxPlayers, port, useNat);
			
			
			if(netComment != ""){
				MasterServer.RegisterHost(gameTypeName, gameName, netComment); // Register this server onto a master server.
				debugText.text += "\nComment: " + netComment;
			}
			else{
				MasterServer.RegisterHost(gameTypeName, gameName);
			}
		}

		// Game name
		GUILayout.BeginHorizontal();
		GUILayout.Label("Game Name: ", GUILayout.Width(120));
		gameName = GUILayout.TextField(gameName, 20, GUILayout.Width(120));
		GUILayout.EndHorizontal();
	}

	// ServerWindow
	void ServerWindow(int windowID) {
		if (GUILayout.Button("Launch level 1")) {
			//Application.LoadLevel("Level 1");
			networkView.RPC ("GlobalLevel1", RPCMode.All);
		}
		if (GUILayout.Button("Launch level 2")) {
			//Application.LoadLevel("Level 2");
			networkView.RPC ("GlobalLevel2", RPCMode.All);
		}

		if (GUILayout.Button("Disconnect server")) {
			// Message hosts saying server disconnected.
			debugText.text += "\nDisconnected server";
			Network.Disconnect();
			MasterServer.UnregisterHost();
		}

		// At least one player connected to server.
		if (Network.connections.Length > 0){
			if (GUILayout.Button("Kick all players")){
				for(int i = 0; i < Network.connections.Length; i++) {
					Debug.Log("Disconnecting: " + Network.connections[i].ipAddress + ":" + Network.connections[i].port);
					Network.CloseConnection(Network.connections[i], true);
				}
			}
		}

		// Buttons to kick each player.
		for(int i = 0; i < Network.connections.Length; i++) {
			if(GUILayout.Button("Kick player \n" + Network.connections[i].port)){
				Debug.Log("Disconnecting: " + Network.connections[i].ipAddress + ":" + Network.connections[i].port);
				Network.CloseConnection(Network.connections[i], true);
			}
		}
		
	}

	// ClientWindow
	void ClientWindow(int windowID) {
		if (GUILayout.Button("Disconnect from server")) {
			Debug.Log("Disconnecting: " + Network.connections[0].ipAddress + ":" + Network.connections[0].port);
			Network.CloseConnection(Network.connections[0], true);
		}

		
	}

	// ServerListWindow
	void ServerListWindow(int windowID) {
		// No hosts.
		if (MasterServer.PollHostList().Length == 0) {
			GUILayout.Label("No Hosts");
		}
		// Display hosts.
		else if (MasterServer.PollHostList().Length != 0) {
			hostData = MasterServer.PollHostList();
			int i = 0;
			//GUILayout.Label("          Name                     " + "                Players/max" + "                             pass?");

			GUILayout.BeginHorizontal();
			GUILayout.Label("  Name      " + "Players/max" + "     pass?");
			GUILayout.EndHorizontal();

			serverListScroll = GUILayout.BeginScrollView (serverListScroll, false, true);
			while (i < hostData.Length) {
				HostData h = hostData[i]; // Less to write
				
				// Displays all the hosts. Could use a foreach with a GUI button for each.
				//Debug.Log("Game name: " + hostData[i].gameName);


				// Change some information for Server List Hosts buttons.
				// Display Server List Hosts longways.
				// Display host list
				/*
				if(GUILayout.Button(h.gameName + "                                    " + h.connectedPlayers + "/" + h.playerLimit + "                                    " + h.passwordProtected)){
					// Connect to existing server
					Network.Connect(h.ip, port, netPass); // "127.0.0.1"
					debugText.text += "\nConnecting to " + h.gameName;
				}
				*/
				GUILayout.BeginHorizontal();
				// If there is a password, add an input box.
				if(h.passwordProtected == true){
					GUILayout.Label(h.gameName + "          " + h.connectedPlayers + "/" + h.playerLimit + "             " + h.passwordProtected);
					GUILayout.Label("Pass: ", GUILayout.Width(40));
					netPass = GUILayout.TextField(netPass, 20, GUILayout.Width(60));
				}
				else{
					GUILayout.Label(h.gameName + "          " + h.connectedPlayers + "/" + h.playerLimit + "             " + h.passwordProtected);
				}
				if(GUILayout.Button("Connect", GUILayout.Width(120))){
					Network.Connect(h.ip, port, netPass);
					debugText.text += "\nConnecting to " + h.gameName;
				}
				GUILayout.EndHorizontal();

				i++;
			}
			GUILayout.EndScrollView();
			//MasterServer.ClearHostList();
		}
		
	}

	// ChatWindow
	void ChatWindow(int windowID) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Players in chat: " + (Network.connections.Length + 1) + "/" + maxPlayers, GUILayout.Width(220));
		GUILayout.Label("Welcome " + playerName, GUILayout.Width(220));
		if(Network.isServer){
			if(GUILayout.Button("Clear chat")){
				globalChat = "";
				networkView.RPC ("SyncChat", RPCMode.All, globalChat);
			}
		}
		GUILayout.EndHorizontal();

		// Chat box, contains all chat.
		chatScroll = GUILayout.BeginScrollView (chatScroll, false, true);
		// Need to display player name left of input message box and on every message sent.
		GUILayout.Box(globalChat);
		GUILayout.EndScrollView();

		// Player input.
		GUILayout.BeginHorizontal();
		GUILayout.Label(playerName + ":", GUILayout.Width(100));
		chatText = GUILayout.TextField(chatText, GUILayout.Width(200));

		// Press button or enter to send message.
		if(GUILayout.Button("Send", GUILayout.Width(60)) || Event.current.type == EventType.KeyDown && chatText != ""){
			// Need to change what is sent, 
			// currently the whole chat is sent but 
			// when the chat is too big it can not be sent.
			PrintText(chatText, new NetworkMessageInfo());
			chatText = "";
			chatScroll.y = 100000; // Set scroll to bottom.
			// All text is wiped when a new person joins.
			networkView.RPC ("SyncChat", RPCMode.All, globalChat);
		}

		// Button sends a message to everyone connected.
		if(GUILayout.Button("Hi to everyone", GUILayout.Width(120))){
			networkView.RPC ("PrintText", RPCMode.All, "Hello everyone.");
		}
		GUILayout.EndHorizontal();
		
	}


	// DebugWindow
	void DebugWindow(int windowID) {
		// Scroll view with debug information inside.
		if(debugHidden == false){
			debugScroll = GUILayout.BeginScrollView (debugScroll, false, true);
			GUILayout.Box(debugText.text);
			GUILayout.EndScrollView();
		}

		// Two buttons, Shrink/Expand and Hide/Show.
		GUILayout.BeginHorizontal();
		if(debugHidden == false){
			if(debugExpanded == false){
				if(GUILayout.Button("Expand")){
					debugExpanded = true;
					debugWindowRect = debugExpandRect;
				}
			}
			if(debugExpanded == true){
				if(GUILayout.Button("Shrink")){
					debugExpanded = false;
					debugWindowRect = debugNormalRect;
				}
			}
		}
		if(debugHidden == true){
			if(GUILayout.Button("Show")){
				debugHidden = false;
				if(debugExpanded == true){
					debugWindowRect = debugExpandRect;
				}
				else{
					debugWindowRect = debugNormalRect;
				}
			}
		}
		if(debugHidden == false){
			if(GUILayout.Button("Hide")){
				debugHidden = true;
				debugWindowRect = debugHiddenRect;
			}
		}
		GUILayout.EndHorizontal();
		GUI.DragWindow();
	}

	void OnGUI(){
		// Set a new GUIText as debug/ information for player.

		// If game is not a client and is not a server show Welcome and Server List.
		if(!Network.isClient && !Network.isServer){
			// WelcomeWindow
			welcomeWindowRect = GUILayout.Window(0, welcomeWindowRect, WelcomeWindow, "Welcome");
			serverListWindowRect = GUILayout.Window(3, serverListWindowRect, ServerListWindow, "Server List");
		}

		// If game is server show ServerWindow.
		if (Network.isServer){
			serverWindowRect = GUILayout.Window(1, serverWindowRect, ServerWindow, "Server");
		}
		
		// If game is client show ClientWindow.
		if (Network.isClient){
			clientWindowRect = GUILayout.Window(2, clientWindowRect, ClientWindow, "Client");
		}

		// If game is client or server show ChatWindow.
		if(Network.isClient || Network.isServer){
			chatWindowRect = GUILayout.Window(4, chatWindowRect, ChatWindow, "Chat");
		}
		if(Input.GetKey(KeyCode.Space)){
			Instantiate(playerObject, Vector3.zero, Quaternion.identity);
		}
		// Debug window.
		debugWindowRect = GUILayout.Window(5, debugWindowRect, DebugWindow, "Debug");
	}


	// RPC examples
	[@RPC]
	void PrintText(string text, NetworkMessageInfo info){
		globalChat += "\n" + playerName + " Says: " + text;
		// RPC example 2 //networkView.RPC ("PrintText", RPCMode.All, "Hello everyone.");
	}
	[@RPC]
	void PrintText(string text){
		globalChat += "\n" + text;
		// RPC example 1 //PrintText("Hello 1");
	}
	[@RPC]
	void SyncChat(string globalChat2){
		globalChat = globalChat2;
	}
	[@RPC]
	void GlobalLevel1(){
		Application.LoadLevel("Level 1");
	}
	[@RPC]
	void GlobalLevel2(){
		Application.LoadLevel("Level 2");
	}


	// Server methods.
	void OnConnectedToServer() {
		//SpawnPlayer();
		Network.Instantiate(playerObject, Vector3.zero, Quaternion.identity, 0);
		//Display "<Player name> connected" and disconnected.
		PrintText("Connected", new NetworkMessageInfo());
		networkView.RPC ("SyncChat", RPCMode.All, globalChat);
	}

	void OnFailedToConnect(NetworkConnectionError error) {
		debugText.text += "\nCould not connect to server: " + error;
	}

	void OnNetworkInstantiate(NetworkMessageInfo info) {
		debugText.text += "\nNew object instantiated by " + info.sender;
	}

	private int playerCount = 0;
	void OnPlayerConnected(NetworkPlayer player) {
		debugText.text += "\nPlayer " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port;
		// When someone connects, update everyones chat.
		networkView.RPC ("SyncChat", RPCMode.All, globalChat);
		// Instantiate player cube.
		//Display "<Player name> connected" and disconnected.
	}

	// Clean up player object.
	void OnPlayerDisconnected(NetworkPlayer player) {
		debugText.text += "\nClean up after player " + player;
		//Display "<Player name> connected" and disconnected.
		PrintText("Disconnected", new NetworkMessageInfo());
		networkView.RPC ("SyncChat", RPCMode.All, globalChat);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer)
			debugText.text += "\nLocal server connection disconnected";
		else
			if (info == NetworkDisconnection.LostConnection)
				debugText.text += "\nLost connection to the server";
		else
			debugText.text += "\nSuccessfully diconnected from the server";

		/*
		foreach(GameObject playerObject in GameObject.FindGameObjectsWithTag("Players")){
			Destroy(playerObject);
		}
		 */
	}

	// Change this to player object or such.
	public int currentHealth;
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		int health = 0;
		if (stream.isWriting) {
			health = currentHealth;
			stream.Serialize(ref health);
			debugText.text += "\nHealth: " + health;
		} else {
			stream.Serialize(ref health);
			currentHealth = health;
			debugText.text += "\nHealth: " + currentHealth;
		}
	}

	void OnServerInitialized() {
		debugText.text += "\nServer initialized and ready";
		Network.Instantiate(playerObject, Vector3.zero, Quaternion.identity, 0);
		//Instantiate(playerObject, Vector3.zero, Quaternion.identity);
	}
}
