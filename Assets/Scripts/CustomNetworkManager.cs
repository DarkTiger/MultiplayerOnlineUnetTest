using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class CustomNetworkManager : NetworkManager
{
    private NetworkMatch networkMatch;
    //private Canvas menuCanvas;
    //private Canvas gameCanvas;
    private bool matchCreated;
    public Sprite sprMatchUISelected;
    public Sprite sprMatchUIDefault;
    private MPMenuScript mpMenuScript;

    [HideInInspector]
    public MatchDesc selectedMatch = null;

    [HideInInspector]
    public List<MatchDesc> matchList = new List<MatchDesc>();

    private CreateMatchResponse currentMatchResponse;


    void Awake()
    {
        //networkMatch = gameObject.AddComponent<NetworkMatch>();
        StartMatchMaker();
        networkMatch = GetComponent<NetworkMatch>();
    }

    void Start()
    {
        mpMenuScript = GameObject.Find("NMCanvasMenu").GetComponent<MPMenuScript>();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
    
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        //Debug.Log("OnServerReady");
        //AddClientsChatMessage("New Player Connected!");
        //gameCanvas.GetComponent<InGameHUDManager>().AddTextChat("New Player Connected: ");
        
        /*GameObject[] objNames = GameObject.FindGameObjectsWithTag("PlayerName");
        foreach (GameObject obj in objNames)
        {
            string playerName = obj.GetComponent<SetOnPlayer>().target.name;

            Debug.Log(playerName);
            obj.name = playerName + " Name";
            obj.GetComponentInChildren<TextMesh>().text = playerName;
        }*/

        //Debug.Log(conn.playerControllers[0].unetView.ToString() + " has been connected!");

        //Debug.Log("count: " + conn.clientOwnedObjects.Count);

        /*if (conn.hostId != -1)
        {
            Debug.Log(conn.playerControllers[conn.hostId].unetView.ToString() + " has been connected!");
        }*/
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject spawnGroup = GameObject.FindGameObjectWithTag("SpawnPoints");
        Transform[] spawns = spawnGroup.GetComponentsInChildren<Transform>();
        
        List<Vector3> spawnsPos = new List<Vector3>();
        foreach (Transform spawn in spawns)
        {
            spawnsPos.Add(spawn.position);
        }

        int indexSpawn = Random.Range(1, spawnsPos.Count);

        GameObject player = (GameObject)Instantiate(playerPrefab, spawnsPos[indexSpawn], Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
    
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.name != conn.playerControllers[conn.hostId].gameObject.name)
            {
                player.GetComponent<PlayerNetworkSetup>().CmdSendMessage(conn.playerControllers[conn.hostId].gameObject.name, "Disconnected");
                NetworkServer.DestroyPlayersForConnection(conn);
                return;
            }
        }
    }

    /*public void StartLanHost()
    {
        StartHost();
    }

    public void StartLanServer()
    {
        StartServer();
    }
    
    public void StopHost()
    {
        StopHost();
    }

    public void StopClient()
    {
        StopClient();
    }*/

    public void Disconnect()
    {
        /*if (matchMaker != null)
        {
            if (matchInfo != null)
            {
                for (int i = 0; i < matchList.Count; i++)
                {
                    Debug.Log(matchList[i].networkId + "  " + matchInfo.networkId);
                    if (matchList[i].networkId == matchInfo.networkId)
                    {
                        Debug.Log("RENAMED!!");
                        matchList[i].name = "MIO";
                    }
                }
            }
        }*/
        //Debug.Log(Network.connections.Length);

        /*if (Network.connections.Length > 0)
        {
            Debug.Log("StopClient");
            StopClient();
        }
        else
        {*/
            //SearchOnlineMatches();

            matchMaker.DestroyMatch(matchInfo.networkId, OnMatchDestroyed);
            matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, OnMatchDestroyed);
 
            //StopClient();
            StopHost();
            
        //}

        //matchMaker.DestroyMatch(matchInfo.networkId, OnMatchDestroyed);
                
        
    }

    public void OnMatchDestroyed(BasicResponse response)
    {
        Debug.Log("OnMatchMakerDrop");
    }

    public void StartOnlineServer(string name, uint size, bool isPrivate, string password)
    {
        CreateMatchRequest match = new CreateMatchRequest();
        if (isPrivate)
        {
            match.name = "(Private) " + name;
        }
        else
        {
            match.name = name;
        }
        
        match.size = size;
        match.advertise = !isPrivate;
        match.password = password;
        
        networkMatch.CreateMatch(match, OnMatchCreate);
    }

    public void OnMatchCreate(CreateMatchResponse matchResponse)
    {
        if (matchResponse.success)
        {
            /*matchCreated = true;
            Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
            NetworkServer.Listen(new MatchInfo(matchResponse), 9000);*/
                        
            //currentMatchResponse = matchResponse;
            StartHost(new MatchInfo(matchResponse));
        }
        else
        {
            //currentMatchResponse = null;
            Debug.LogError("Create match failed");
        }
    }

    public void SearchOnlineMatches()
    {
        StartMatchMaker();
        networkMatch = GetComponent<NetworkMatch>();

        selectedMatch = null;
        matchList.Clear();
                
        networkMatch.ListMatches(0, 20, "", OnMatchList);
    }

    public void OnMatchList(ListMatchResponse matchListResponse)
    {
        matchList = matchListResponse.matches;

        if (matchListResponse.success && matchListResponse.matches != null)
        {
            GameObject ScrollViewMatches = GameObject.Find("ScrollViewMatches");

            if (GameObject.FindGameObjectsWithTag("MatchButton").Length > 0)
            {
                GameObject[] goButtons = GameObject.FindGameObjectsWithTag("MatchButton");
                               
                foreach (GameObject goButton in goButtons)
                {
                    Destroy(goButton);
                }
            }

            int yOffset = 130;
            for (int i = 0; i < matchListResponse.matches.Count; i++)
            {
                //if (matchInfo != null)
                //{
                    //if (matchListResponse.matches[i].networkId != matchInfo.networkId)
                    //{
                        GameObject btn = new GameObject("btnMatch" + (i+1));
                        btn.tag = "MatchButton";
                        btn.AddComponent<Image>().sprite = sprMatchUIDefault;
                        btn.AddComponent<Button>();
                        btn.transform.parent = ScrollViewMatches.transform;
                        btn.transform.localScale = new Vector3(1, 1, 1);
                        btn.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollViewMatches.GetComponent<RectTransform>().sizeDelta.x - 4, 30);
                        ColorBlock cb = btn.GetComponent<Button>().colors;
                        cb.normalColor = new Color(1, 1, 1, 0.75f);
                        cb.highlightedColor = new Color(0.85f,0.85f,0.85f,1);
                        btn.GetComponent<Button>().colors = cb;


                        GameObject txtBtnMatchName = new GameObject("txtBtnMatchName" + (i + 1));
                        Text txtName = txtBtnMatchName.AddComponent<Text>();
                        txtName.GetComponent<Text>().text = matchListResponse.matches[i].name;
                        txtName.GetComponent<Text>().color = Color.black;
                        txtName.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                        txtName.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                        txtName.transform.parent = btn.transform;
                        txtName.transform.localScale = new Vector3(1, 1, 1);
                        txtName.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollViewMatches.GetComponent<RectTransform>().sizeDelta.x - 20, 30);

                        GameObject txtBtnMatchPlayers = new GameObject("txtBtnMatchPlayers" + (i + 1));
                        Text txtPlayers = txtBtnMatchPlayers.AddComponent<Text>();
                        txtPlayers.GetComponent<Text>().text = matchListResponse.matches[i].currentSize + "/" +  matchListResponse.matches[i].maxSize;
                        txtPlayers.GetComponent<Text>().color = Color.black;
                        txtPlayers.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                        txtPlayers.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                        txtPlayers.transform.parent = btn.transform;
                        txtPlayers.transform.localScale = new Vector3(1, 1, 1);
                        txtPlayers.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollViewMatches.GetComponent<RectTransform>().sizeDelta.x - 400, 30);


                        //Debug.Log(matchListResponse.extendedInfo.);

                        GameObject txtBtnMatchPing = new GameObject("txtBtnMatchPing" + (i + 1));
                        Text txtPing = txtBtnMatchPing.AddComponent<Text>();
                        //txtPing.GetComponent<Text>().text = Network.GetAveragePing(Network.connections[0]).ToString(); //matchListResponse.matches[i].;
                        txtPing.GetComponent<Text>().color = Color.black;
                        txtPing.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                        txtPing.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                        txtPing.transform.parent = btn.transform;
                        txtPing.transform.localScale = new Vector3(1, 1, 1);
                        txtPing.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollViewMatches.GetComponent<RectTransform>().sizeDelta.x - 600, 30);

                        btn.GetComponent<Button>().onClick.AddListener(delegate() { SelectMatch(btn, btn.GetComponentInChildren<Text>().text, matchListResponse); });
                        btn.transform.localPosition = new Vector3(0, yOffset, 0);
                
                        yOffset -= 30;
                    //}
                //}
            }
        }   
    }

    public void JoinOnlineMatch(string password)
    {
        if (selectedMatch != null)
        {
            networkMatch.JoinMatch(selectedMatch.networkId, password, OnJoinMatch);
        }
    }

    void OnJoinMatch(JoinMatchResponse matchJoin)
    {
        if (matchJoin.success)
        {
            mpMenuScript.lblJoinWrongPassword.text = "";

            Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
            StartClient(new MatchInfo(matchJoin));

            if (GameObject.FindGameObjectsWithTag("MatchButton").Length > 0)
            {
                GameObject[] goButtons = GameObject.FindGameObjectsWithTag("MatchButton");

                foreach (GameObject goButton in goButtons)
                {
                    Destroy(goButton);
                }
            }
        }
        else
        {
            mpMenuScript.lblJoinWrongPassword.text = "Wrong Password!";
        }
    }
    
    void SelectMatch(GameObject btn, string matchName, ListMatchResponse matchList)
    {
        if (GameObject.FindGameObjectsWithTag("MatchButton").Length > 0)
        {
            GameObject[] goButtons = GameObject.FindGameObjectsWithTag("MatchButton");

            foreach (GameObject goButton in goButtons)
            {
                goButton.GetComponent<Image>().sprite = sprMatchUIDefault;
                ColorBlock cb = goButton.GetComponent<Button>().colors;
                cb.normalColor = new Color(1,1,1,0.75f);
                cb.colorMultiplier = 1f;
                goButton.GetComponent<Button>().colors = cb;
            }
        }       

        btn.GetComponent<Image>().sprite = sprMatchUISelected;
        ColorBlock cbs = btn.GetComponent<Button>().colors;
        cbs.normalColor = new Color(1, 1, 1, 1);
        cbs.colorMultiplier = 5f;
        btn.GetComponent<Button>().colors = cbs;

        for (int i = 0; i < matchList.matches.Count; i++)
        {
            if (matchList.matches[i].name == matchName)
            {
                selectedMatch = matchList.matches[i];
            }
        }
    }

    public void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("SERVER ERROR");
    }

    public void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("CLIENT ERROR");
    }
        
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            name = "NetworkManager";
        }

        if (name == "NetworkManager" && SceneManager.GetActiveScene().name == "Menu")
        {
            mpMenuScript.ShowMPMenu();

            if (GameObject.Find("NetManager") != null)
            {
                SearchOnlineMatches();
                Destroy(GameObject.Find("NetManager"));
            }
        }
    }
}
