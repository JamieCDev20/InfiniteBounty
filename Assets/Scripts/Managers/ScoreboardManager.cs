using Photon.Pun;
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

    [Header("Bonus Objective Things")]
    [SerializeField] private Text t_bonusObjectiveText;
    [SerializeField] private Text t_diffScaleText;
    [SerializeField] private int i_bonusMoney = 1000;
    [SerializeField] private int i_nuggCountToCompleteBonus = 400;

    [Header("Expenses")]
    [SerializeField] private Text[] tA_expenseNameText = new Text[0];
    [SerializeField] private Text[] tA_expenseCostText = new Text[0];
    [SerializeField] private string[] sA_possibleExpenses = new string[0];


    public void Start()
    {
        for (int i = 0; i < tA_expenseNameText.Length; i++)
        {
            tA_expenseNameText[i].text = "";
            tA_expenseCostText[i].text = "";
        }

        //if (PhotonNetwork.InRoom)
        //    UniversalNugManager.x?.DoScoring();

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
        int _i_scaledMoney = 0;
        int _i_bonusScaledMoney = 0;
        float _f_currentMoneyMult = DifficultyManager.x.ReturnCurrentDifficulty().f_moneyMult;

        for (int i = 0; i < pc; i++)
        {
            so_playerScoreObjects[i].nameText.text = PhotonNetwork.CurrentRoom.Players[i + 1].NickName;

            //Goo
            so_playerScoreObjects[i].gooText.text = values[pc - 1 - i][0].ToString();
            if (DiversifierManager.x.ReturnBonusObjective() == BonusObjective.BonusGoo && values[pc - 1 - i][0] >= i_nuggCountToCompleteBonus)
                _i_bonusScaledMoney = Mathf.RoundToInt(i_bonusMoney * _f_currentMoneyMult);

            //Hydro
            so_playerScoreObjects[i].hydroText.text = values[pc - 1 - i][1].ToString();
            if (DiversifierManager.x.ReturnBonusObjective() == BonusObjective.BonusHydro && values[pc - 1 - i][1] >= i_nuggCountToCompleteBonus)
                _i_bonusScaledMoney = Mathf.RoundToInt(i_bonusMoney * _f_currentMoneyMult);

            //Tasty
            so_playerScoreObjects[i].tastyText.text = values[pc - 1 - i][2].ToString();
            if (DiversifierManager.x.ReturnBonusObjective() == BonusObjective.BonusTasty && values[pc - 1 - i][2] >= i_nuggCountToCompleteBonus)
                _i_bonusScaledMoney = Mathf.RoundToInt(i_bonusMoney * _f_currentMoneyMult);

            //Thunder
            so_playerScoreObjects[i].thunderText.text = values[pc - 1 - i][3].ToString();
            if (DiversifierManager.x.ReturnBonusObjective() == BonusObjective.BonusThunder && values[pc - 1 - i][3] >= i_nuggCountToCompleteBonus)
                _i_bonusScaledMoney = Mathf.RoundToInt(i_bonusMoney * _f_currentMoneyMult);

            //Boom
            so_playerScoreObjects[i].boomText.text = values[pc - 1 - i][4].ToString();
            if (DiversifierManager.x.ReturnBonusObjective() == BonusObjective.BonusBoom && values[pc - 1 - i][4] >= i_nuggCountToCompleteBonus)
                _i_bonusScaledMoney = Mathf.RoundToInt(i_bonusMoney * _f_currentMoneyMult);

            //Magma
            so_playerScoreObjects[i].magmaText.text = values[pc - 1 - i][5].ToString();
            if (DiversifierManager.x.ReturnBonusObjective() == BonusObjective.BonusMagma && values[pc - 1 - i][0] >= i_nuggCountToCompleteBonus)
                _i_bonusScaledMoney = Mathf.RoundToInt(i_bonusMoney * _f_currentMoneyMult);

            playerTotal = UniversalNugManager.x.CalculateValues(values[pc - 1 - i]);

            so_playerScoreObjects[i].bucksText.text = Mathf.RoundToInt(playerTotal).ToString();
            totalEarned += Mathf.RoundToInt(playerTotal * _f_currentMoneyMult);
        }

        for (int i = 0; i < tA_expenseNameText.Length; i++)
        {
            tA_expenseNameText[i].text = "";
            tA_expenseCostText[i].text = "";
        }

        Random rand = new Random();
        Random.InitState(values[0][0] + _names.GetHashCode());

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount + 2; i++)
        {
            tA_expenseNameText[i].text = sA_possibleExpenses[Random.Range(0, sA_possibleExpenses.Length)];
            int _i = Random.Range(0, 100);
            tA_expenseCostText[i].text = "-£" + _i;
            _i_bonusScaledMoney -= _i;
        }

        t_totalEarned.text = Mathf.RoundToInt(totalEarned + _i_bonusScaledMoney).ToString();
        NetworkedPlayer.x.CollectEndLevelNugs(Mathf.RoundToInt((totalEarned / PhotonNetwork.CurrentRoom.PlayerCount) + _i_bonusScaledMoney));

        if (_i_bonusScaledMoney > 0)
            t_bonusObjectiveText.text = "BONUS £1000";
        else
            t_bonusObjectiveText.text = "BONUS FAILED";

        t_diffScaleText.text = "Risk Bonus = " + _f_currentMoneyMult + "x";
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
        if (!PhotonNetwork.IsConnectedAndReady)
            return;
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
        FindObjectOfType<ToolTipper>().StartShowing();
        FindObjectOfType<HUDController>().StartShowing();

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
            _t.localPosition = Vector3.Lerp(start, _b_comingIntoMachine ? Vector3.zero : Vector3.forward * -4, t);
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