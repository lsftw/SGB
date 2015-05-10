using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private const string typeName = "SGB";
    private const string gameName = "coaxed-into-a-snafu"; //Room name

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
        MasterServer.RegisterHost(typeName, gameName);
		//
		//
    }

    void OnServerInitialized()
    {
        SpawnPlayer();
		GenerateWorld();
    }

    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    void OnConnectedToServer()
    {
        SpawnPlayer();
		GenerateWorld();
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
            MasterServer.RequestHostList(typeName);
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
