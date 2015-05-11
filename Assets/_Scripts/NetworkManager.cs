using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Handles basic server/client connection.
 * Delays world generation/player spawning until two players are on same server.
 * That way, Network.Instantiate is sent to ALL players, since both players are connected when world spawns.
 */
public class NetworkManager : MonoBehaviour
{
    private const string GAME_NAME = "SGB";
    private string[] ROOM_NAMES = {"coaxed-into-a-snafu", "on-the-ruse-cruise", "busted-your-trust"};
	private string roomName;

	// if true, waits for a client to connect to server before initializing game
	// necessary for multiplayer Network.Instantiate of block world to work
	// (since if a client connects after server world instantiation, they don't receive any instantiate calls)
	public const bool WAIT_FOR_TWO_PLAYERS = true;
	private bool waitingForAnotherPlayer;

    private bool isRefreshingHostList = false;
    private HostData[] hostList;

	public GameObject worldGeneratorPrefab;
    public GameObject playerPrefab;
	public Text textDisplay;

    void OnGUI()
    {
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
    }
	//
	private string getRandomRoomName()
	{
		return ROOM_NAMES[Random.Range(0, ROOM_NAMES.Length)];
	}
    private void StartServer()
    {
		roomName = getRandomRoomName();
        Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(GAME_NAME, roomName);
		//
		//

    }

    void OnServerInitialized()
    {
		if (WAIT_FOR_TWO_PLAYERS) {
			waitingForAnotherPlayer = true;

			// TODO show gui text about waiting for player
			//GUI.Text(new Rect(200, 200, 250, 100), "Start Server");
		} else {
			StartGame();
		}
    }
    void OnPlayerConnected(NetworkPlayer player) {
		//Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
		if (WAIT_FOR_TWO_PLAYERS && waitingForAnotherPlayer) {
			waitingForAnotherPlayer = false;
			StartGame();
		}
    }
	void StartGame() {
		SpawnPlayer();
		GenerateWorld();
		//textDisplay = GetComponent<Canvas> ().GetComponent<"CenterText">;
		//
	}

    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    void OnConnectedToServer()
    {
        StartGame();
    }

    void Update()
    {
        if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
        {
            isRefreshingHostList = false;
            hostList = MasterServer.PollHostList();
        }
		UpdateGuiText();
    }
	void UpdateGuiText()
	{
		if (WAIT_FOR_TWO_PLAYERS && waitingForAnotherPlayer) {
			textDisplay.text = "Waiting for another player to connect...\n" + 
				"Your Room Name: " + roomName;
		} else {
			textDisplay.text = "";
		}
	}

    private void RefreshHostList()
    {
        if (!isRefreshingHostList)
        {
            isRefreshingHostList = true;
            MasterServer.RequestHostList(GAME_NAME);
        }
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
