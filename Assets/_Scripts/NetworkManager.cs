using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private const string GAME_NAME = "SGB";
    private const string ROOM_NAME = "coaxed-into-a-snafu";

	// if true, waits for a client to connect to server before initializing game
	// necessary for multiplayer Network.Instantiate of block world to work
	// (since if a client connects after server world instantiation, they don't receive any instantiate calls)
	public const bool WAIT_FOR_TWO_PLAYERS = true;
	private bool waitingForAnotherPlayer;

    private bool isRefreshingHostList = false;
    private HostData[] hostList;

	public GameObject worldGeneratorPrefab;
    public GameObject playerPrefab;

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
        }
    }
	//
    private void StartServer()
    {
        Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(GAME_NAME, ROOM_NAME);
		//
		//
    }

    void OnServerInitialized()
    {
		if (WAIT_FOR_TWO_PLAYERS) {
			waitingForAnotherPlayer = true;
			// TODO show gui text about waiting for player
			//GUI.Button(new Rect(200, 200, 250, 100), "Start Server")
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
        Network.Instantiate(playerPrefab, Vector3.up * 15, Quaternion.identity, 0);
    }
}
