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
        DestroyAllDontDestroyOnLoadObjects();
        PhotonNetwork.LeaveRoom();
        SceneManager.UnloadSceneAsync(0);
        SceneManager.LoadScene(0);
    }

    public void DestroyAllDontDestroyOnLoadObjects()
    {

        var go = new GameObject("Sacrificial Lamb");
        DontDestroyOnLoad(go);

        foreach (var root in go.scene.GetRootGameObjects())
            Destroy(root);

    }

}
