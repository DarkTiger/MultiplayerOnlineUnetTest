using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Timers;
using System;
using UnityEngine.EventSystems;


public class PlayerNetworkSetup : NetworkBehaviour
{   
    //[SerializeField]
    //Camera characterCam;
    //[SerializeField]
    //AudioListener audioListener;
    public Camera playerCam;
    public GameObject InGameHUDManager;

    private bool needUpdate = false;
    private int secondsToUpdate = 1;
    private Canvas networkManagerMenuCanvas;
    private Canvas networkManagerGameCanvas;
    private bool cameraIsCreated = false;
    private float secondsNow = 0;
    private bool secondsTaked = false;
    private InputField inputTextChat;
    

    public override void OnStartClient()
    {
        networkManagerMenuCanvas = GameObject.Find("NMCanvasMenu").GetComponent<Canvas>();
        networkManagerGameCanvas = GameObject.Find("NMCanvasGame").GetComponent<Canvas>();

        needUpdate = true;
    }

    public override void OnStartLocalPlayer()
    {
        networkManagerMenuCanvas.enabled = false;
        networkManagerGameCanvas.enabled = true;

        inputTextChat = GameObject.Find("InputTextChat").GetComponent<InputField>();
        GameObject.Find("Scene Camera").SetActive(false);
        
        Camera cam = (Camera)GameObject.Instantiate(playerCam, transform.position, transform.rotation);
        cam.transform.name = transform.name + " Cam";
        cam.GetComponent<CameraOrbitScript>().target = gameObject.transform;
        cam.enabled = true;
        GetComponent<PlayerController>().playerCam = cam;
        SetPlayerCamToObjNames(cam);

        GameObject objName = GetComponent<PlayerID>().objPlayerName;
        HideMyName(objName);

        //characterCam.enabled = true;
        //audioListener.enabled = true; 

        CmdSendMessage(transform.name, "Connected");

        /*if (!Debug.isDebugBuild)
        {
            Cursor.visible = false;
        }*/
    }

    void Start()
    {
        InGameHUDManager = GameObject.Find("NMCanvasGame");
        Cursor.visible = false;
    }

    void SetPlayerCamToObjNames(Camera camera)
    {
        GameObject[] objNames = GameObject.FindGameObjectsWithTag("PlayerName");
        foreach (GameObject obj in objNames)
        {
            obj.GetComponent<SetOnPlayer>().playerCam = camera;
        }
    }

    void HideMyName(GameObject objName)
    {
        MeshRenderer[] meshRenderers = objName.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (inputTextChat != null)
        {                   
            if (!inputTextChat.isFocused && inputTextChat.text.Trim() != "")
            {
                CmdSendMessage(transform.name, inputTextChat.text.Trim());
                inputTextChat.text = "";
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        if (needUpdate)
        {
            if (Mathf.Round(Time.time) > secondsNow)
            {
                secondsNow = Mathf.Round(Time.time) + secondsToUpdate;
                
                if (secondsTaked)
                {
                    SetNewClientName();
                    needUpdate = false;
                    secondsTaked = false;
                    secondsNow = 0;
                }

                secondsTaked = true;
            }   
        }
    }
        
    void SetNewClientName()
    {
        GameObject[] playersNamesObj = GameObject.FindGameObjectsWithTag("PlayerName");
        foreach (GameObject obj in playersNamesObj)
        {
            string name = obj.GetComponent<SetOnPlayer>().target.name;
            obj.name = name + " Name";
                
            TextMesh[] textMeshes = obj.GetComponentsInChildren<TextMesh>();
            foreach (TextMesh textMesh in textMeshes)
            {
                textMesh.text = name;
            }
        }
    }
    
    //ClientRpc VIENE ESEGUITO SU TUTTI COMPRESO IL SERVER 
    [ClientRpc]
    void RpcWriteChatMessage(string chatMessage)
    {
        GameObject hudManager = GameObject.Find("NMCanvasGame");
        hudManager.GetComponent<InGameHUDManager>().AddTextChat(chatMessage);
    }

    //Command VIENE ESEGUITO SUL SERVER
    [Command]
    public void CmdSendMessage(string playerName, string chatMessage)
    {
        chatMessage =  playerName + ": " + chatMessage;
        RpcWriteChatMessage(chatMessage);
    }
}
