using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	
	private const string GAME_NAME = "SGB";
	private const string ROOM_NAME = "coaxed-into-a-snafu";
	//
	// if true, waits for a client to connect to server before initializing game
	// necessary for multiplayer Network.Instantiate of block world to work
	// (since if a client connects after server world instantiation, they don't receive any instantiate calls)
	public const bool WAIT_FOR_TWO_PLAYERS = true;
	private bool waitingForAnotherPlayer;
	
	private bool isRefreshingHostList = false;
	private HostData[] hostList;
	
	public GameObject worldGeneratorPrefab;
	public GameObject playerPrefab;
	public Text waitingOnPlayersText;

	public GameObject Main_Canvas_Panel;
	public GameObject Hosting_Canvas_Panel;
	public GameObject Joining_Canvas_Panel;

	public Button Main_HostServerButton;
	public Button Main_SearchForHostsButton;
	public Button Main_QuitButton;

	public Button Hosting_CancelButton;
	//public Button Hosting_StartButton;

	public Button Joining_CancelButton;
	public Button Joining_RefreshHostsButton;

	public GameObject MenuPlane;
	public GameObject MinimapPlane;

	void DisableAllCanvasPanels(){
		Main_Canvas_Panel.SetActive (false);
		Transform[] tarr = Main_Canvas_Panel.transform.GetComponentsInChildren<Transform> ();
		foreach (Transform t in tarr) {
			t.gameObject.SetActive(false);
		}
		Hosting_Canvas_Panel.SetActive (false);
		tarr = Hosting_Canvas_Panel.transform.GetComponentsInChildren<Transform> ();
		foreach (Transform t in tarr) {
			t.gameObject.SetActive(false);
		}
		Joining_Canvas_Panel.SetActive (false);
		tarr = Joining_Canvas_Panel.transform.GetComponentsInChildren<Transform> ();
		foreach (Transform t in tarr) {
			t.gameObject.SetActive(false);
		}
		//
	}

	void DisplayCanvas(GameObject panel){
		DisableAllCanvasPanels ();

		//enable all children of the active canvas
		Transform[] tarr = panel.transform.GetComponentsInChildren<Transform> ();
		foreach (Transform t in tarr) {
			t.gameObject.SetActive(true);
		}
		panel.SetActive (true);
	}


	void Start () {
		DisplayCanvas(Main_Canvas_Panel);

		MinimapPlane.SetActive (false);

		//Main_SearchForHostsButton.onClick.AddListener(delegate{testDebug();});
		Main_HostServerButton.onClick.AddListener(StartServer);
		Main_SearchForHostsButton.onClick.AddListener(SwitchToJoin);
		Main_QuitButton.onClick.AddListener (Quit);

		Hosting_CancelButton.onClick.AddListener (CloseServer);
		//Hosting_StartButton.onClick.AddListener (StartGame);
		//Hosting_StartButton.interactable = false;
		//
		Joining_CancelButton.onClick.AddListener (CancelJoin);
		Joining_RefreshHostsButton.onClick.AddListener(RefreshHostList);
		//
		//
		//
	}
	void Quit (){
		Application.Quit ();
	}

	void OnGUI()
	{
		///
		if (Joining_Canvas_Panel.activeSelf) {
			if (!Network.isClient && !Network.isServer) {
				if (hostList != null) {
					for (int i = 0; i < hostList.Length; i++) {
						if (GUI.Button (new Rect (600, 110 + (110 * i), 250, 100), hostList [i].gameName)){
							DisableAllCanvasPanels ();
							JoinServer (hostList [i]);
						}
					}
				}
				//
			}
		}
		//
		/*
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
			//
		}
		//
		//
		*/
	} 
	//
	/*void testDebug(){
		Debug.Log ("clicked button!");
	}*/
	private void StartServer()
	{
		Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(GAME_NAME, ROOM_NAME);
		DisplayCanvas (Hosting_Canvas_Panel);
		//Hosting_StartButton.interactable = false;
		//
	}
	private void CloseServer(){
		Network.Disconnect ();
		MasterServer.UnregisterHost ();
		DisplayCanvas(Main_Canvas_Panel);
	}
	private void CancelJoin(){
		DisplayCanvas (Main_Canvas_Panel);
		//TODO remove server gui buttons!!!!
	}
	void OnServerInitialized()
	{
		if (WAIT_FOR_TWO_PLAYERS) {
			waitingForAnotherPlayer = true;
			//Hosting_StartButton.interactable = false; //can't click start until another player joins
			//GUI.Text(new Rect(200, 200, 250, 100), "Start Server");
			//
		} /*else {
			StartGame();
		}*/
	}
	void OnPlayerConnected(NetworkPlayer player) {
		//Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
		if (WAIT_FOR_TWO_PLAYERS && waitingForAnotherPlayer) {
			waitingForAnotherPlayer = false;
			//TODO do we need start button at all? just starting immediately for now
			// Enable start button
			//Hosting_StartButton.interactable = true;
			//DisableAllCanvasPanels();
			//MenuPlane.SetActive (false);
			StartGame();
		}

	}
	void StartGame() {
		DisableAllCanvasPanels();
		MinimapPlane.SetActive (true);
		MenuPlane.SetActive (false);
		SpawnPlayer();
		GenerateWorld();
		//textDisplay = GetComponent<Canvas> ().GetComponent<"CenterText">;
		//
	}
	private void JoinServer(HostData hostData)
	{
		//Debug.Log ("Joining server!");
		//DisableAllCanvasPanels ();
		//MenuPlane.SetActive (false);
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		//DisplayCanvas (Hosting_Canvas_Panel);
		//Hosting_StartButton.interactable = false; //false for the joining party
		StartGame();
	}
	
	void Update()
	{
		//if (Hosting_Canvas_Panel.activeSelf) {
		if (isRefreshingHostList && MasterServer.PollHostList ().Length > 0) {
			isRefreshingHostList = false;
			hostList = MasterServer.PollHostList ();
		}
		if (Hosting_Canvas_Panel.activeSelf) {
			UpdateGuiText ();
		}
	}
	void UpdateGuiText()
	{
		if (WAIT_FOR_TWO_PLAYERS && waitingForAnotherPlayer) {
			//waitingOnPlayersText.text = "Waiting for another player to connect...\n" + 
			//	"Your Room Name: " + ROOM_NAME;
			waitingOnPlayersText.text = "Game will start when another player connects.";
		} /*else {
			//Else is unnecessary - game starts immediately...
			waitingOnPlayersText.text = "Player connected, ready to start.";
		}*/
		//
		//
	}
	private void SwitchToJoin(){
		DisplayCanvas (Joining_Canvas_Panel);

		RefreshHostList ();
	}
	private void RefreshHostList()
	{
		if (hostList == null) {
			hostList = MasterServer.PollHostList ();
		}
		if (!isRefreshingHostList)
		{
			isRefreshingHostList = true;
			MasterServer.RequestHostList(GAME_NAME);
			hostList = MasterServer.PollHostList ();

		} 

		/*if (hostList != null)
		{

			for (int i = 0; i < hostList.Length; i++)
			{
				//TODO make buttons more like UI
				Debug.Log ("i = " + i + "hostList[i].gameName = "+hostList[i].gameName);

				if (GUI.Button(new Rect(300, 100 + (110 * i), 300, 100), hostList[i].gameName))
					JoinServer(hostList[i]);
				
				//
			}
		}*/
	}
	
	private void GenerateWorld()
	{
		Instantiate(worldGeneratorPrefab, new Vector3(0,0,0), Quaternion.identity);
	}
	private void SpawnPlayer()
	{
		//Spawn player at random x,z
		Network.Instantiate (playerPrefab, new Vector3 (Random.Range (-10.0F, 10.0F), 15, Random.Range (-10.0F, 10.0F)), Quaternion.identity, 0); 
		//Network.Instantiate(playerPrefab, Vector3.up * 15, Quaternion.identity, 0);
	}
}
