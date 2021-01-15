using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NeverFearThisScriptIsHere : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        //PhotonNetwork.LeaveRoom();
    }

    public void DestroyAllDontDestroyOnLoadObjects()
    {

        var go = new GameObject("Sacrificial Lamb");
        DontDestroyOnLoad(go);

        foreach (var root in go.scene.GetRootGameObjects())
        {
            if (root.GetComponent<PhotonHandler>() != null)
                Destroy(root);
        }

    }

    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        //Debug.Log("LEFT THE ROOM");
        //DestroyAllDontDestroyOnLoadObjects();
        //SceneManager.UnloadSceneAsync(0);
        SceneManager.LoadScene(0);
    }

}
