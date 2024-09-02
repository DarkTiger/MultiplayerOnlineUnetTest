using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class MPMenuScript : MonoBehaviour 
{
    private GameObject pnlMenuMP;
    private GameObject pnlCreateMP;
    private GameObject pnlInsertJoinPassword;
    private InputField inputFieldNickName;
    private InputField inputFieldServerName;
    private InputField inputFieldCreatePassword;
    private InputField inputFieldJoinPassword;
    private Button btnCreateMPServer;
    private Button btnJoinMPServer;
    private Button btnExitMP;
    private Button btnStartMP;
    private Button btnBackMP;
    private Button btnJoinWithPassword;
    private Button btnBackFromPassword;
    private Slider sliderMaxPlayers;
    private Text lblMaxPlayer;
    private Toggle togglePrivateServer;
    private Canvas NMCanvasMenu;
    private CustomNetworkManager networkManager;

    public Texture2D sprCursor;

    [HideInInspector]
    public Text lblJoinWrongPassword;
   
    [HideInInspector]
    public string playerName = "";


	void Start () 
    {
        networkManager = GameObject.Find("NetManager").GetComponent<CustomNetworkManager>();

        pnlMenuMP = GameObject.Find("PnlMenuMP");
        pnlCreateMP = GameObject.Find("PnlCreateMP");
        pnlInsertJoinPassword = GameObject.Find("PnlInsertJoinPassword");

        lblJoinWrongPassword = GameObject.Find("lblJoinWrongPassword").GetComponent<Text>();
        inputFieldNickName = GameObject.Find("inputNickname").GetComponent<InputField>();
        inputFieldServerName = GameObject.Find("inputServerName").GetComponent<InputField>();
        inputFieldCreatePassword = GameObject.Find("InputServerPassword").GetComponent<InputField>();
        inputFieldJoinPassword = GameObject.Find("inputJoinPassword").GetComponent<InputField>();
        btnCreateMPServer = GameObject.Find("btnCreateMP").GetComponent<Button>();
        btnJoinMPServer = GameObject.Find("btnJoinMP").GetComponent<Button>();
        btnExitMP = GameObject.Find("btnExitMP").GetComponent<Button>();
        btnStartMP = GameObject.Find("btnStartMP").GetComponent<Button>();
        btnBackMP = GameObject.Find("btnBackMP").GetComponent<Button>();
        btnJoinWithPassword = GameObject.Find("btnJoinWithPassword").GetComponent<Button>();
        btnBackFromPassword = GameObject.Find("btnBackFromPassword").GetComponent<Button>(); 
        sliderMaxPlayers = GameObject.Find("SliderMaxPlayers").GetComponent<Slider>();
        lblMaxPlayer = GameObject.Find("lblMaxPlayers").GetComponent<Text>();
        togglePrivateServer = GameObject.Find("TogglePrivateServer").GetComponent<Toggle>();
        NMCanvasMenu = GameObject.Find("NMCanvasMenu").GetComponent<Canvas>();
                
        pnlInsertJoinPassword.SetActive(false);
        pnlCreateMP.SetActive(false);

        Cursor.SetCursor(sprCursor, Vector2.zero, CursorMode.Auto);

        networkManager.SearchOnlineMatches();
    }
        
    public void InputNickname_OnEndEdit()
    {
        string nickName = inputFieldNickName.text;

        if (nickName.Trim() != "" && nickName != null)
        {
            playerName = nickName;
            btnCreateMPServer.interactable = true;

            if (networkManager.selectedMatch != null)
            {
                btnJoinMPServer.interactable = true;
            }
        }
    }

    public void BtnCreateMP_OnClick()
    {
        pnlMenuMP.SetActive(false);
        pnlCreateMP.SetActive(true);

        inputFieldServerName.text = "Server di " + playerName;
    }

    public void BtnStartMP_OnClick()
    {
        string name = inputFieldServerName.text;
        uint size = (uint)sliderMaxPlayers.value;
        bool isPrivate = togglePrivateServer.isOn;
        string password = inputFieldCreatePassword.text;

        Text lblErrorCreateMP = GameObject.Find("lblErrorCreateMP").GetComponent<Text>();
        List<MatchDesc> matchList = networkManager.matchList;
        foreach (MatchDesc match in matchList)
        {
            if (match.name == inputFieldServerName.text)
            {
                lblErrorCreateMP.text = "Name Already Exists!";
                return;
            }
        }

        lblErrorCreateMP.text = "";
        networkManager.StartOnlineServer(name, size, isPrivate, password);
    }

    public void BtnBackMP_OnClick()
    {
        Text lblErrorCreateMP = GameObject.Find("lblErrorCreateMP").GetComponent<Text>();
        lblErrorCreateMP.text = "";

        pnlMenuMP.SetActive(true);
        pnlCreateMP.SetActive(false);
    }

    public void BtnJoinMP_OnClick()
    {
        MatchDesc selectedMatch = networkManager.selectedMatch;

        if (selectedMatch != null)
        {
            if (selectedMatch.isPrivate)
            {
                pnlInsertJoinPassword.SetActive(true);
            }
            else
            {
                networkManager.JoinOnlineMatch("");  
            }
        }
    }

    public void BtnRefreshListMP_OnClick()
    {
        networkManager.SearchOnlineMatches();
    }

    public void SliderMaxPlayers_OnValueChanged()
    {
        lblMaxPlayer.text = "Max Players: " + (int)sliderMaxPlayers.value;
    }

    public void TogglePrivateServer_OnValueChanged()
    {
        inputFieldCreatePassword.interactable = togglePrivateServer.isOn;
    }

    public void btnJoinWithPassword_OnClick()
    {
        string password = inputFieldJoinPassword.text;
        networkManager.JoinOnlineMatch(password); 
    }

    public void btnBackFromPassword_OnClick()
    {
        HidePnlInsertJoinPassword();
    }

    public void ShowPnlInsertJoinPassword()
    {
        pnlInsertJoinPassword.SetActive(true);
    }

    public void HidePnlInsertJoinPassword()
    {
        inputFieldJoinPassword.text = "";
        pnlInsertJoinPassword.SetActive(false);
    }

    public void ShowMPMenu()
    {
        Cursor.visible = true;
        NMCanvasMenu.enabled = true;
        pnlMenuMP.SetActive(true);
        pnlCreateMP.SetActive(false);
    }

	void Update () 
    {
        if (networkManager.selectedMatch != null && !inputFieldNickName.isFocused && inputFieldNickName.text.Trim() != "")
        {
            btnJoinMPServer.interactable = true;
        }

        if (inputFieldNickName.isFocused)
        {
            btnCreateMPServer.interactable = false;
            btnJoinMPServer.interactable = false;
        }

        if (pnlCreateMP != null)
        {    
            if (inputFieldServerName.text.Trim() != "" && (togglePrivateServer.isOn && inputFieldCreatePassword.text.Trim() != "" || !togglePrivateServer.isOn))
            {
                btnStartMP.interactable = true;
            }
            else
            {
                btnStartMP.interactable = false;
            }
        }
	}
}
