using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{

    [SerializeField] private ScoreObjects[] so_playerScoreObjects;
    [SerializeField] private Text t_totalEarned;

    [Header("Camera Locking Things")]
    private PlayerInputManager pim;
    private Camera c_cam;
    [SerializeField] private Transform[] tA_playerPos = new Transform[0];
    [SerializeField] private Transform t_camPos;
    private Transform t_camPositionToReturnTo;
    private bool b_isBeingUsed;

    public void Start()
    {
        if (PhotonNetwork.InRoom)
            UniversalNugManager.x?.DoScoring();

        ////yield return new WaitForEndOfFrame();

        if (FindObjectOfType<ScoreboardCamController>())
        {
            LockCam();
            GetComponent<TurnOnObjectWhenAroundPlayers>().EnableScreen();
        }
    }

    public void SetValues(int[][] values, string[] _names)
    {
        int totalEarned = 0;
        int playerTotal;
        int pc = PhotonNetwork.CurrentRoom.PlayerCount;

        //string s = "";
        //for (int i = 0; i < values.Length; i++)
        //{
        //    for (int j = 0; j < values[i].Length; j++)
        //    {
        //        s += $"{values[i][j]} ";
        //    }
        //    s += '\n';
        //}

        //Debug.Log(s);

        for (int i = 0; i < pc; i++)
        {
            so_playerScoreObjects[i].nameText.text = PhotonNetwork.CurrentRoom.Players[i + 1].NickName;

            so_playerScoreObjects[i].gooText.text = values[pc - 1 - i][0].ToString();
            so_playerScoreObjects[i].hydroText.text = values[pc - 1 - i][1].ToString();
            so_playerScoreObjects[i].tastyText.text = values[pc - 1 - i][2].ToString();
            so_playerScoreObjects[i].thunderText.text = values[pc - 1 - i][3].ToString();
            so_playerScoreObjects[i].boomText.text = values[pc - 1 - i][4].ToString();
            so_playerScoreObjects[i].magmaText.text = values[pc - 1 - i][5].ToString();
            playerTotal = UniversalNugManager.x.CalculateValues(values[pc - 1 - i]);
            so_playerScoreObjects[i].bucksText.text = playerTotal.ToString();
            totalEarned += playerTotal;
        }
        t_totalEarned.text = totalEarned.ToString();
        NetworkedPlayer.x.CollectEndLevelNugs(Mathf.RoundToInt(totalEarned / PhotonNetwork.CurrentRoom.PlayerCount));
        
    }

    #region Camera Locking

    private void Update()
    {
        if (b_isBeingUsed)
            if (Input.anyKeyDown)
                EndLockedCam();
    }

    private void LockCam()
    {
        Transform interactor = GameObject.FindGameObjectsWithTag("Player")[0].transform;

        pim = interactor.GetComponent<PlayerInputManager>();
        pim.enabled = false;

        PlayerMover pm = pim.GetComponent<PlayerMover>();
        c_cam = pim.GetCamera().GetComponentInChildren<Camera>();
        pm.GetComponent<Rigidbody>().isKinematic = true;
        pim.b_shouldPassInputs = false;

        interactor.position = tA_playerPos[pim.GetID()].position;
        interactor.transform.forward = tA_playerPos[pim.GetID()].forward;

        pm.enabled = false;
        t_camPositionToReturnTo = pim.GetCamera().transform;
        pim.GetCamera().enabled = false;
        FindObjectOfType<ToolTipper>().StopShowing();
        FindObjectOfType<HUDController>().StopShowing();
        Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.SetShootability(false);
        _pa.StopWalking();

        StartCoroutine(MoveCamera(t_camPos, c_cam.transform, true));

        b_isBeingUsed = true;
    }

    public void EndLockedCam()
    {
        StartCoroutine(MoveCamera(t_camPositionToReturnTo, c_cam.transform, false));

        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;
        pim.enabled = true;        
        FindObjectOfType<ToolTipper>().StopShowing();
        FindObjectOfType<HUDController>().StopShowing();        

        pim.GetCamera().enabled = true;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.SetShootability(true);
        _pa.StartWalking();

        b_isBeingUsed = false;
    }

    #endregion

    public IEnumerator MoveCamera(Transform _t_transformToMoveTo, Transform _t_cameraToMove, bool _b_comingIntoMachine)
    {
        Transform _t = Camera.main.transform;
        float t = 0;
        if (_b_comingIntoMachine)
            _t.parent = t_camPos;
        else
            _t.parent = _t_transformToMoveTo;

        Vector3 start = _t.localPosition;
        Quaternion iRot = _t.rotation;

        while (t < 1)
        {
            _t.localPosition = Vector3.Lerp(start, Vector3.zero, t);
            _t.rotation = Quaternion.Lerp(iRot, _t_transformToMoveTo.rotation, t);
            t += (Time.deltaTime * (1 / 0.3f));
            yield return new WaitForEndOfFrame();
        }

        if (_b_comingIntoMachine)
        {
            _t.localPosition = Vector3.zero;
            _t.localEulerAngles = Vector3.zero;
        }
        else
        {
            Camera.main.GetComponent<CameraRespectWalls>().enabled = true;
            Camera.main.transform.localPosition = new Vector3(0, 0, -4);
            Camera.main.transform.localEulerAngles = new Vector3(10, 0, 0);
        }
    }
}

[System.Serializable]
public class ScoreObjects
{
    public Text nameText;
    public Text boomText;
    public Text tastyText;
    public Text thunderText;
    public Text magmaText;
    public Text hydroText;
    public Text gooText;
    public Text bucksText;
}