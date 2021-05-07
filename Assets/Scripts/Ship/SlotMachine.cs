using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviourPunCallbacks, IInteractible
{
    private Animator anim;
    private PhotonView view;

    [Header("Interactable Things That Moves the Camera")]
    [SerializeField] private Transform t_playerPos;
    private bool b_isBeingUsed;
    public bool IsBeingUsed { get { return b_isBeingUsed; } }
    private PlayerInputManager pim;
    private Transform t_camPositionToReturnTo;
    [SerializeField] private Transform t_camParent;
    [SerializeField] private float f_lerpTime = 0.5f;

    [Header("Wheels")]
    [SerializeField] private WheelData[] wdA_wheels = new WheelData[3];

    private Diversifier[] dA_activeDiversifiers = new Diversifier[3];
    private NugManager nm_nugMan;

    [Header("UI References")]
    [SerializeField] private Text[] tA_nameTexts = new Text[0];
    [SerializeField] private Text[] tA_descriptionTexts = new Text[0];
    [SerializeField] private Image[] iA_diverSquareSprites = new Image[0];
    [SerializeField] private Image[] iA_diverPlusMinusSprites = new Image[0];
    [SerializeField] private GameObject go_infoCanvas;
    [SerializeField] private Sprite s_spinningSprite;

    [Header("Info Buttons")]
    [SerializeField] private GameObject go_infoHighlight;
    [SerializeField] private Transform[] tA_buttonPositions = new Transform[0];
    private int i_currentButtonHighlighted;

    [Header("MONEYS")]
    [SerializeField] private int i_currentCost;
    [SerializeField] private float f_costMultPerSpin;
    [SerializeField] private TextMeshPro tmp_costText;
    private bool b_isSpinning;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        tmp_costText.text = "";

        GetDiversifiersFromDifficulty();

        SetWheels(wdA_wheels[0], UnityEngine.Random.Range(0, wdA_wheels[0].dA_wheelDiversifiers.Length));
        SetWheels(wdA_wheels[1], UnityEngine.Random.Range(0, wdA_wheels[1].dA_wheelDiversifiers.Length));
        SetWheels(wdA_wheels[2], UnityEngine.Random.Range(0, wdA_wheels[2].dA_wheelDiversifiers.Length));

        DiversifierManager.x.ReceiveDiversifiers(dA_activeDiversifiers);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        PullLeverFree();
    }

    private void GetDiversifiersFromDifficulty()
    {
        SetDiversifiersByDifficulty(DifficultyManager.x.ReturnCurrentDifficulty().dA_diversifierSet);
    }

    internal void SetDiversifiersByDifficulty(Diversifier[] _dA_first)
    {
        wdA_wheels[0].dA_wheelDiversifiers = _dA_first;
        wdA_wheels[1].dA_wheelDiversifiers = _dA_first;
        wdA_wheels[2].dA_wheelDiversifiers = _dA_first;
        /*
        SetWheels(wdA_wheels[0], (int)dA_activeDiversifiers[0]);
        SetWheels(wdA_wheels[1], (int)dA_activeDiversifiers[0]);
        SetWheels(wdA_wheels[2], (int)dA_activeDiversifiers[0]);
        */
    }

    private void SetWheels(WheelData _wd_wheel, int _i_diversifierToRoll)
    {
        dA_activeDiversifiers[_wd_wheel.i_wheelIndex] = _wd_wheel.dA_wheelDiversifiers[_i_diversifierToRoll];

        int _i;
        _wd_wheel.srL_wheelSprites[0].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i_diversifierToRoll]].s_image;

        //Sprite below
        _i = _i_diversifierToRoll - 1;
        if (_i < 0)
            _i = _wd_wheel.dA_wheelDiversifiers.Length - 1;
        _wd_wheel.srL_wheelSprites[1].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i]].s_image;

        //Sprite Above
        _i = _i_diversifierToRoll + 1;
        if (_i >= _wd_wheel.dA_wheelDiversifiers.Length)
            _i = 0;
        _wd_wheel.srL_wheelSprites[2].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i]].s_image;
    }

    #region Interactions

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            interactor.position = t_playerPos.position;
            interactor.transform.forward = t_playerPos.forward;

            b_isBeingUsed = true;
            pim = interactor.GetComponent<PlayerInputManager>();
            PlayerMover pm = pim.GetComponent<PlayerMover>();
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;
            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.SetShootability(false);
            _pa.StopWalking();
            nm_nugMan = pim.GetComponent<NugManager>();
            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            tmp_costText.text = "£" + i_currentCost;

            go_infoCanvas.SetActive(true);
        }
    }


    public void Interacted() { }

    public void EndInteract()
    {
        LockInDivers();

        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;

        StartCoroutine(MoveCamera(t_camPositionToReturnTo, pim.GetCamera().transform, false));

        go_infoCanvas.SetActive(false);
        pim.GetCamera().enabled = true;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.SetShootability(true);
        _pa.StartWalking();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        b_isBeingUsed = false;
        tmp_costText.text = "";
    }

    private void LockInDivers()
    {
        DiversifierManager.x.ReceiveDiversifiers(dA_activeDiversifiers);
    }


    public IEnumerator MoveCamera(Transform _t_transformToMoveTo, Transform _t_cameraToMove, bool _b_comingIntoMachine)
    {
        Transform _t = Camera.main.transform;
        float t = 0;
        if (_b_comingIntoMachine)
            _t.parent = t_camParent;
        else
            Camera.main.transform.parent = _t_cameraToMove;

        Vector3 start = _t.localPosition;
        Quaternion iRot = _t.rotation;

        while (t < 1)
        {
            _t.localPosition = Vector3.Lerp(start, _b_comingIntoMachine ? Vector3.zero : Vector3.forward * -4, t);
            _t.rotation = Quaternion.Lerp(iRot, _t_transformToMoveTo.rotation, t);
            t += (Time.deltaTime * (1 / f_lerpTime));
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

    #endregion

    private IEnumerator RollWheel(WheelData _wd_wheel, float _f_startDelay, int _i_diversifierToRoll)
    {
        yield return new WaitForSeconds(_f_startDelay);
        int _i_currentIndex = 0;
        b_isSpinning = true;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.009f);
            _wd_wheel.go_wheelSpinner.transform.Rotate(Vector3.right * 22.5f, Space.Self);
            yield return new WaitForSeconds(0.009f);
            _wd_wheel.go_wheelSpinner.transform.Rotate(Vector3.right * -22.5f, Space.Self);

            int _i_;

            //Centre Sprite
            _wd_wheel.srL_wheelSprites[0].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i_currentIndex]].s_image;
            DisplayDiversifierInfo(_wd_wheel.i_wheelIndex,
    DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i_currentIndex]].s_name,
    DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i_currentIndex]].s_desc,
    DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i_currentIndex]].s_image);

            //Sprite below
            _i_ = _i_currentIndex - 1;
            if (_i_ < 0)
                _i_ = _wd_wheel.dA_wheelDiversifiers.Length - 1;
            _wd_wheel.srL_wheelSprites[1].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i_]].s_image;

            //Sprite Above
            _i_ = _i_currentIndex + 1;
            if (_i_ >= _wd_wheel.dA_wheelDiversifiers.Length)
                _i_ = 0;
            _wd_wheel.srL_wheelSprites[2].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i_]].s_image;

            _i_currentIndex++;
            if (_i_currentIndex >= _wd_wheel.dA_wheelDiversifiers.Length)
                _i_currentIndex = 0;

        }
        dA_activeDiversifiers[_wd_wheel.i_wheelIndex] = _wd_wheel.dA_wheelDiversifiers[_i_diversifierToRoll];

        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.009f);
            _wd_wheel.go_wheelSpinner.transform.Rotate(Vector3.right * 11.25f, Space.Self);
        }


        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.02f);
            _wd_wheel.go_wheelSpinner.transform.Rotate(-Vector3.right * 22.5f, Space.Self);
        }

        int _i = _i_currentIndex;
        _wd_wheel.srL_wheelSprites[0].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i_diversifierToRoll]].s_image;

        //Sprite below
        _i = _i_diversifierToRoll - 1;
        if (_i < 0)
            _i = _wd_wheel.dA_wheelDiversifiers.Length - 1;
        _wd_wheel.srL_wheelSprites[1].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i]].s_image;

        //Sprite Above
        _i = _i_diversifierToRoll + 1;
        if (_i >= _wd_wheel.dA_wheelDiversifiers.Length)
            _i = 0;
        _wd_wheel.srL_wheelSprites[2].sprite = DiversifierManager.x.diA_diversifiers[(int)_wd_wheel.dA_wheelDiversifiers[_i]].s_image;

        b_isSpinning = false;
        DisplayDiversifierInfo(0);
        DisplayDiversifierInfo(1);
        DisplayDiversifierInfo(2);

        if (view.IsMine)
            anim.SetBool("PullLever", false);
        tmp_costText.text = "£" + i_currentCost;
    }

    [PunRPC]
    public void SyncedRollsRPC(int _i_firstRoll, int _i_secondRoll, int _i_thirdRoll)
    {
        StartCoroutine(RollWheel(wdA_wheels[0], 0.19f, _i_firstRoll));
        StartCoroutine(RollWheel(wdA_wheels[1], 0.28f, _i_secondRoll));
        StartCoroutine(RollWheel(wdA_wheels[2], 0.37f, _i_thirdRoll));
    }

    internal void PullLever()
    {
        if (b_isSpinning) return;

        if (nm_nugMan.Nugs >= i_currentCost)
        {
            nm_nugMan.CollectNugs(-i_currentCost, false);

            view.RPC(nameof(UpCostRPC), RpcTarget.All);
            anim.SetBool("PullLever", true);
            view.RPC(nameof(SyncedRollsRPC), RpcTarget.All,
                UnityEngine.Random.Range(0, wdA_wheels[0].dA_wheelDiversifiers.Length),
                UnityEngine.Random.Range(0, wdA_wheels[1].dA_wheelDiversifiers.Length),
                UnityEngine.Random.Range(0, wdA_wheels[2].dA_wheelDiversifiers.Length));

            DisplayDiversifierInfo(0, "SPINNING", "Sit tight whilst Infinite Bounty's patented, copyrighted & trademarked DMSN-HPR finds you a new dimension to harvest!", s_spinningSprite);
            DisplayDiversifierInfo(1, "SPINNING", "Sit tight whilst Infinite Bounty's patented, copyrighted & trademarked DMSN-HPR finds you a new dimension to harvest!", s_spinningSprite);
            DisplayDiversifierInfo(2, "SPINNING", "Sit tight whilst Infinite Bounty's patented, copyrighted & trademarked DMSN-HPR finds you a new dimension to harvest!", s_spinningSprite);

        }
        else
            anim.SetTrigger("FailedToBuy");
    }
    internal void PullLeverFree()
    {
        anim.SetBool("PullLever", true);
        view.RPC(nameof(SyncedRollsRPC), RpcTarget.All,
            UnityEngine.Random.Range(0, wdA_wheels[0].dA_wheelDiversifiers.Length),
            UnityEngine.Random.Range(0, wdA_wheels[1].dA_wheelDiversifiers.Length),
            UnityEngine.Random.Range(0, wdA_wheels[2].dA_wheelDiversifiers.Length));

        DisplayDiversifierInfo(0, "SPINNING", "Sit tight whilst Infinite Bounty's patented, copyrighted & trademarked DMSN-HPR finds you a new dimension to harvest!", s_spinningSprite);
        DisplayDiversifierInfo(1, "SPINNING", "Sit tight whilst Infinite Bounty's patented, copyrighted & trademarked DMSN-HPR finds you a new dimension to harvest!", s_spinningSprite);
        DisplayDiversifierInfo(2, "SPINNING", "Sit tight whilst Infinite Bounty's patented, copyrighted & trademarked DMSN-HPR finds you a new dimension to harvest!", s_spinningSprite);
    }



    [PunRPC]
    public void UpCostRPC()
    {
        i_currentCost = Mathf.RoundToInt(i_currentCost * f_costMultPerSpin);
    }

    internal void DisplayDiversifierInfo(int _i_index)
    {
        if (b_isSpinning) return;

        i_currentButtonHighlighted = _i_index;

        tA_descriptionTexts[_i_index].text = DiversifierManager.x.diA_diversifiers[(int)dA_activeDiversifiers[_i_index]].s_desc;
        tA_nameTexts[_i_index].text = DiversifierManager.x.diA_diversifiers[(int)dA_activeDiversifiers[_i_index]].s_name;

        if (!DiversifierManager.x.diA_diversifiers[(int)dA_activeDiversifiers[_i_index]].b_hasSilhouetteBreak)
        {
            iA_diverSquareSprites[_i_index].enabled = true;
            iA_diverSquareSprites[_i_index].sprite = DiversifierManager.x.diA_diversifiers[(int)dA_activeDiversifiers[_i_index]].s_image;
            iA_diverPlusMinusSprites[_i_index].enabled = false;
        }
        else
        {
            iA_diverPlusMinusSprites[_i_index].enabled = true;
            iA_diverPlusMinusSprites[_i_index].sprite = DiversifierManager.x.diA_diversifiers[(int)dA_activeDiversifiers[_i_index]].s_image;
            iA_diverSquareSprites[_i_index].enabled = false;
        }

        //go_infoHighlight.transform.position = tA_buttonPositions[_i_index].position;
    }

    internal void DisplayDiversifierInfo(int _i_index, string _s_title, string _s_desc, Sprite _s_image)
    {
        tA_nameTexts[_i_index].text = _s_title;
        tA_descriptionTexts[_i_index].text = _s_desc;

        if (!DiversifierManager.x.diA_diversifiers[(int)dA_activeDiversifiers[_i_index]].b_hasSilhouetteBreak)
        {
            iA_diverSquareSprites[_i_index].enabled = true;
            iA_diverSquareSprites[_i_index].sprite = _s_image;
            iA_diverPlusMinusSprites[_i_index].enabled = false;
        }
        else
        {
            iA_diverPlusMinusSprites[_i_index].enabled = true;
            iA_diverPlusMinusSprites[_i_index].sprite = _s_image;
            iA_diverSquareSprites[_i_index].enabled = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (b_isBeingUsed)
            EndInteract();
    }

    [System.Serializable]
    private struct WheelData
    {
        public GameObject go_wheelSpinner;
        public List<SpriteRenderer> srL_wheelSprites;
        public int i_wheelIndex;
        [HideInInspector] public Diversifier[] dA_wheelDiversifiers;
    }
}

[System.Serializable]
public struct DiversifierInfo
{
    public string s_name;
    [TextArea] public string s_desc;
    public string s_shortHandDesc;
    [Space]
    public Sprite s_image;
    public bool b_hasSilhouetteBreak;
}