using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviour, IInteractible
{

    private PlayerInputManager pim;
    private bool b_isBeingUsed;
    private Transform t_camPositionToReturnTo;
    [SerializeField] private Canvas c_workbenchCanvas;
    [SerializeField] private Transform t_playerPos;

    [Header("Camera & Movement")]
    [SerializeField] private Transform t_camParent;
    [SerializeField] private int i_timesToLerpCam = 50;
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
    private List<Augment> aL_augmentsInList = new List<Augment>();


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
            pm.GetComponent<PlayerAnimator>().enabled = false;

            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.SetShootability(false);
            _pa.StopWalking();

            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            c_workbenchCanvas.enabled = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        InitAugmentList(new Augment[30]);
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

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        b_isBeingUsed = false;
    }

    #endregion


    public IEnumerator MoveCamera(Transform _t_transformToMoveTo, Transform _t_cameraToMove, bool _b_comingIntoMachine)
    {
        Transform _t = Camera.main.transform;

        if (_b_comingIntoMachine)
            _t.parent = t_camParent;
        else
            Camera.main.transform.parent = _t_cameraToMove;

        for (int i = 0; i < i_timesToLerpCam; i++)
        {
            _t.localPosition = Vector3.Lerp(_t.localPosition, Vector3.zero, f_cameraMovementT);
            _t.localEulerAngles = Vector3.Lerp(_t.localEulerAngles, Vector3.zero, f_cameraMovementT);
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

    private void InitAugmentList(Augment[] _aA_augmentsInList)
    {
        aL_augmentsInList.AddRange(_aA_augmentsInList);

        for (int i = 0; i < 30; i++)
        {
            if (goL_augmentButtonPool.Count <= i)
                goL_augmentButtonPool.Add(Instantiate(go_augmentButton, rt_augmentButtonParent));

            goL_augmentButtonPool[i].SetActive(true);

            goL_augmentButtonPool[i].transform.localPosition = new Vector3(0, (-i * f_augmentButtonHeight) - 70, 0);
            goL_augmentButtonPool[i].GetComponent<Button>().onClick.AddListener(delegate { ClickAugment(i); });
            //goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[0].text = _aA_augmentsInList[i].level;
            //goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[1].text = _aA_augmentsInList[i].Name;
        }

        rt_augmentButtonParent.sizeDelta = new Vector2(rt_augmentButtonParent.sizeDelta.x, f_augmentButtonHeight * ((30) + 1));
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


}
