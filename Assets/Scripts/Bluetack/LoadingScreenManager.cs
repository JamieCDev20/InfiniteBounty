using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviourPun
{
    public static LoadingScreenManager x;
    private int i_currentPlayersLoadedLevel;
    private string s_sceneNameToLoad;
    private AsyncOperation asyncOperation;


    private void Awake()
    {
    }

    public void Init()
    {
        if (x == null)
        {
            x = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        photonView.ViewID = 9898989;
        PhotonNetwork.RegisterPhotonView(photonView);

    }
    internal void SetSceneToLoad(string _s_scene)
    {
        s_sceneNameToLoad = _s_scene;
    }

    [PunRPC]
    public void LoadedSceneRPC()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        i_currentPlayersLoadedLevel++;

        if (i_currentPlayersLoadedLevel >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            photonView.RPC(nameof(ActuallyLoadLevelRPC), RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void ActuallyLoadLevelRPC()
    {
        asyncOperation.allowSceneActivation = true;
        //PhotonNetwork.LoadLevel(s_sceneNameToLoad);
    }

    [PunRPC]
    public void CallLoadLevel(string _s_levelName)
    {
        StartCoroutine(ICallLoadLevel(_s_levelName));
    }
    private IEnumerator ICallLoadLevel(string _s_levelName)
    {
        //print("I've been told to call the funuctyion");
        //SetSceneToLoad(_s_levelName);
        //StartCoroutine(BeginLoadingSceneAsync());

        photonView.RPC(nameof(ShowLoadingScreen), RpcTarget.AllViaServer);
        yield return new WaitForSeconds(1);
        PhotonNetwork.LoadLevel(_s_levelName);
    }

    [PunRPC]
    public void ShowLoadingScreen()
    {
        FadeToBlack.x.ShowCover(0);
    }

    internal IEnumerator BeginLoadingSceneAsync()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(CallLoadLevel), RpcTarget.Others, s_sceneNameToLoad);

        asyncOperation = SceneManager.LoadSceneAsync(s_sceneNameToLoad);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            print(asyncOperation.progress * 100 + "%");
            if (asyncOperation.progress >= 0.9f)
            {
                photonView.RPC(nameof(LoadedSceneRPC), RpcTarget.All);
                break;
            }
            yield return null;
        }
    }

}