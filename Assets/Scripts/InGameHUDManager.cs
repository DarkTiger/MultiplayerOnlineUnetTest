using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class InGameHUDManager : NetworkBehaviour 
{
    private Text labelTextChat;
    private InputField inputTextChat;
    private GameObject pnlInGameMenu;
    private Button btnInGameDisconnect;
    private List<string> chatMessages;
    private CustomNetworkManager networkManager;
    private Canvas NMCanvasGame;

    public Texture2D sprCursor;


    void Start()
    { 
        chatMessages = new List<string>();

        pnlInGameMenu = GameObject.Find("pnlInGameMenu");
        labelTextChat = GameObject.Find("lblTextChat").GetComponent<Text>();
        inputTextChat = GameObject.Find("InputTextChat").GetComponent<InputField>();
        btnInGameDisconnect = GameObject.Find("btnInGameDisconnect").GetComponent<Button>();
        networkManager = GameObject.Find("NetManager").GetComponent<CustomNetworkManager>();
        NMCanvasGame = GameObject.Find("NMCanvasGame").GetComponent<Canvas>();

        Cursor.SetCursor(sprCursor, Vector2.zero, CursorMode.Auto);

        pnlInGameMenu.SetActive(false);
    }

    public void AddTextChat(string text)
    {
        if (chatMessages.Count > 7)
        {
            chatMessages.RemoveAt(0);
        }

        chatMessages.Add(text);

        labelTextChat.text = "";
        for (int i = 0; i < chatMessages.Count; i++)
        {
            labelTextChat.text += "\n" + chatMessages[i];
        }     
    }

    public void BtnInGameDisconnect_OnClick()
    {
        NMCanvasGame.enabled = false;
        networkManager.Disconnect();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) && !inputTextChat.isFocused && SceneManager.GetActiveScene().name == "Game")
        {
            inputTextChat.Select();
        }
        else if (SceneManager.GetActiveScene().name == "Menu")
        {
            chatMessages.Clear();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.visible = !pnlInGameMenu.activeSelf;
            pnlInGameMenu.SetActive(!pnlInGameMenu.activeSelf);   
        }
    }
}
