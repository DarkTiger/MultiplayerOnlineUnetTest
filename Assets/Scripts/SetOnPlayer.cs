using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetOnPlayer : NetworkBehaviour 
{
    public Transform target;
    public Camera playerCam;
    private bool nameIsSet = false;
    public string targetName = "";

    void Start()
    {
        GameObject objPlayerCam = GameObject.FindGameObjectWithTag("PlayerCam");
        playerCam = new Camera();
        playerCam = objPlayerCam.GetComponent<Camera>();
    }
    
	void Update() 
    {
        if (target != null)
        {
            Vector3 tmpPos = target.localPosition;
            tmpPos.y += 0.75f;
            transform.position = tmpPos;

            SetTextRotation();

            if (!nameIsSet)
            {
                targetName = target.name;
                transform.name = targetName + " Name";

                TextMesh[] textMeshes = GetComponentsInChildren<TextMesh>();
                foreach (TextMesh textMesh in textMeshes)
                {
                    textMesh.text = targetName;
                }
                
                nameIsSet = true;
            }
        }
	}

    void SetTextRotation()
    {
        if (playerCam != null)
        {
            transform.LookAt(playerCam.transform.position);
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }  
}
