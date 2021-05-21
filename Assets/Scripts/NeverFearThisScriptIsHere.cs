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
            if (root.GetComponent<PhotonHandler>() != null || root.transform != transform)
                Destroy(root);
        }
        StartCoroutine(SaveUsLamb());

    }

    IEnumerator SaveUsLamb()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
        Destroy(gameObject);
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
