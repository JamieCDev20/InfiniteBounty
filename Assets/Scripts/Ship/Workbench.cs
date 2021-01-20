using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviourPunCallbacks, IInteractible
{

    private PlayerInputManager pim;
    private bool b_isBeingUsed;
    private Transform t_camPositionToReturnTo;
    private SaveManager saveMan;
    [SerializeField] private Canvas c_workbenchCanvas;
    [SerializeField] private Transform t_playerPos;

    [Header("Camera & Movement")]
    [SerializeField] private Transform t_camParent;
    [SerializeField] private int i_timesToLerpCam = 50;
    [SerializeField] private float f_lerpTime = 0.5f;
    [SerializeField] private float f_cameraMovementT = 0.4f;

    [Header("Button Info")]
    [SerializeField] private GameObject go_augmentButton;
    [SerializeField] private RectTransform rt_augmentButtonParent;
    [SerializeField] private float f_augmentButtonHeight = 85;
    private List<GameObject> goL_augmentButtonPool = new List<GameObject>();
    private int i_currentAugmentIndex;
    [SerializeField] private Scrollbar s_slider;

    [Header("Augment Display")]
    [SerializeField] private AugmentDisplay ad_display;
    private List<Augment> aL_allAugmentsOwned = new List<Augment>();


    #region Interactions

    public void Init(SaveManager _sm)
    {
        saveMan = _sm;
    }

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
            pm.GetComponent<PlayerAnimator>().enabled = false;

            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.SetShootability(false);
            _pa.StopWalking();

            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            c_workbenchCanvas.enabled = true;

            if(saveMan.SaveData.purchasedAugments != null)
            {
                Augment[] augs = saveMan.SaveData.purchasedAugments;
                InitAugmentList(augs, false);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Interacted() { }

    public void EndInteract()
    {
        PlayerMover pm = pim.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;

        StartCoroutine(MoveCamera(t_camPositionToReturnTo, pim.GetCamera().transform, false));

        c_workbenchCanvas.enabled = false;
        pim.GetCamera().enabled = true;
        pm.GetComponent<PlayerAnimator>().enabled = true;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.SetShootability(true);
        _pa.StartWalking();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        b_isBeingUsed = false;
    }

    #endregion

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

        //for (int i = 0; i < i_timesToLerpCam; i++)
        //{
        //    _t.localPosition = Vector3.Lerp(_t.localPosition, Vector3.zero, f_cameraMovementT);
        //    _t.localEulerAngles = Vector3.Lerp(_t.localEulerAngles, Vector3.zero, f_cameraMovementT);
        //    yield return new WaitForEndOfFrame();
        //}

        while (t < 1)
        {
            _t.localPosition = Vector3.Lerp(start, Vector3.zero, t);
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

    internal void InitAugmentList(Augment[] _aA_augmentsInList, bool _b_shouldAddToExistingList)
    {
        if (!_b_shouldAddToExistingList)
            aL_allAugmentsOwned.Clear();

        aL_allAugmentsOwned.AddRange(_aA_augmentsInList);

        UpdateAugmentListDisplay(AugmentDisplayType.ShowAll);
    }

    private void UpdateAugmentListDisplay(AugmentDisplayType _adt_whichToShow)
    {
        List<Augment> _aL_augmentsToShow = new List<Augment>();

        switch (_adt_whichToShow)
        {
            case AugmentDisplayType.ShowAll:
                _aL_augmentsToShow.AddRange(aL_allAugmentsOwned);
                break;

            case AugmentDisplayType.ShowSameType:
                for (int i = 0; i < aL_allAugmentsOwned.Count; i++)
                {
                    /*
                        if(aL_allAugmentsOwned[i].type == currentWeapon.type)
                            _aL_augmentsToShow.Add(aL_allAugmentsOwned[i]);
                    */
                }
                break;
        }

        for (int i = 0; i < aL_allAugmentsOwned.Count; i++)
        {
            if (goL_augmentButtonPool.Count <= i)
                goL_augmentButtonPool.Add(Instantiate(go_augmentButton, rt_augmentButtonParent));
            goL_augmentButtonPool[i].SetActive(true);
            goL_augmentButtonPool[i].transform.localPosition = new Vector3(0, (-i * f_augmentButtonHeight) - 70, 0);
            goL_augmentButtonPool[i].GetComponent<Button>().onClick.AddListener(delegate { ClickAugment(i); });
            //goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[0].text = _aA_augmentsInList[i].level;
            goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[0].text = aL_allAugmentsOwned[i].Name;
        }

        rt_augmentButtonParent.sizeDelta = new Vector2(rt_augmentButtonParent.sizeDelta.x, f_augmentButtonHeight * (aL_allAugmentsOwned.Count + 1));
        s_slider.value = 1;
    }


    #region Button Functions

    public void ApplyAugment()
    {
        print("APPLY AUGMENT");
    }

    public void ClickAugment(int _i_augmentIndexClicked)
    {
        goL_augmentButtonPool[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = false;
        i_currentAugmentIndex = _i_augmentIndexClicked;
        goL_augmentButtonPool[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = true;

        /*
        aL_augmentsInPool[i_currentAugment].t_levelNumber.text = aA_avaliableAugments[i_currentAugmentIndex].Level;
        aL_augmentsInPool[i_currentAugment].t_augmentName.text = aA_avaliableAugments[i_currentAugmentIndex].Name;
        aL_augmentsInPool[i_currentAugment].t_augmentType.text = aA_avaliableAugments[i_currentAugmentIndex].type;
        aL_augmentsInPool[i_currentAugment].t_augmentFits.text = aA_avaliableAugments[i_currentAugmentIndex].fits;
        aL_augmentsInPool[i_currentAugment].t_augmentEffects.text = aA_avaliableAugments[i_currentAugmentIndex].effects;        
        */
    }

    #endregion

    #region Swap Weapons

    public void ChangeWeapon(int _i_weaponIndexChange)
    {

    }


    #endregion

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        EndInteract();
    }

}

public enum AugmentDisplayType
{
    ShowAll, ShowEquipped, ShowSameType
}
