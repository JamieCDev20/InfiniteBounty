using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelect : MonoBehaviourPun, IInteractible
{

    public static ModeSelect x;
    private AudioSource as_source;
    private int i_currentIndex;
    [SerializeField] private LoadIntoLevel lil_teleportButton;
    [Space, SerializeField] private GameObject[] goA_highlightPositions = new GameObject[3];
    [Space, SerializeField] private string[] sA_sceneNames = new string[3];

    private void Start()
    {
        x = this;
        as_source = GetComponent<AudioSource>();

        PhotonNetwork.RegisterPhotonView(photonView);
        //if (PhotonNetwork.IsMasterClient)
            //Interacted();
    }

    public void Interacted()
    {
        goA_highlightPositions[i_currentIndex].SetActive(false);

        i_currentIndex++;

        if (i_currentIndex >= goA_highlightPositions.Length)
            i_currentIndex = 0;

        //print(PhotonNetwork.CurrentRoom.PlayerCount == 1 + "/ " + i_currentIndex);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && i_currentIndex == goA_highlightPositions.Length - 1)
            i_currentIndex = 0;

        goA_highlightPositions[i_currentIndex].SetActive(true);
        lil_teleportButton?.SetLevelToLoad(sA_sceneNames[i_currentIndex]);

        photonView.RPC(nameof(SetCurrentMode), RpcTarget.Others, i_currentIndex);

        as_source.Play();
        TutorialManager.x.UsedShiftChanger();
    }

    [PunRPC]
    public void SetCurrentMode(int _i_newMode)
    {
        goA_highlightPositions[i_currentIndex].SetActive(false);

        i_currentIndex = _i_newMode;

        goA_highlightPositions[i_currentIndex].SetActive(true);
        lil_teleportButton.SetLevelToLoad(sA_sceneNames[i_currentIndex]);
    }

    public string GetModeName()
    {
        return sA_sceneNames[i_currentIndex];
    }

    public void Interacted(Transform interactor) { }

}
