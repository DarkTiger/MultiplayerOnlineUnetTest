using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManagerController : NetworkBehaviour 
{
    public override void OnStartClient()
    {
        Debug.Log("NEWWW");
    }
}
