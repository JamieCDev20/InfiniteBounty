using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachine : SubjectBase, IInteractible
{

    [SerializeField] private Transform t_camParent;
    private Transform t_camPositionToReturnTo;
    private PlayerInputManager pim;
    private AugmentManager augMan;
    [SerializeField] private int i_timesToLerpCam = 15;
    [SerializeField] private float f_lerpTime = 0.5f;
    [SerializeField] private float f_cameraMovementT = 0.3f;
    private int i_currentAugmentIndex;
    private AugmentGo[] aA_avaliableAugments = new AugmentGo[9];
    [SerializeField] private Canvas c_vendingCanvas;
    [SerializeField] private AugmentDisplay vmd_vendingMachineDisplay;
    [SerializeField] private Transform[] tA_augmentPositions = new Transform[0];
    [SerializeField] private Transform t_augmentHighlight;
    [SerializeField] private Rigidbody[] rbA_augmentRigidbodies = new Rigidbody[0];
    [SerializeField] private Transform t_playerPos;

    [Header("Audio Thangs")]
    [SerializeField] private AudioClip ac_whirringClip;
    private AudioSource as_source;
    [SerializeField] private AudioClip[] acA_beeps = new AudioClip[0];
    private bool b_isBeingUsed;

    [Header("Spittin' out Augments")]
    [SerializeField] private GameObject go_augmentPrefab;
    [SerializeField] private Transform t_augmentSpawnPoint;
    private Animator anim;

    public void Init(AugmentManager _am)
    {
        anim = GetComponent<Animator>();
        as_source = GetComponent<AudioSource>();

        int _i = UnityEngine.Random.Range(0, 9);
        i_currentAugmentIndex = _i;
        t_augmentHighlight.position = tA_augmentPositions[_i].position;
        UpdateAugmentDisplay();
        AddObserver(FindObjectOfType<SaveManager>());
        augMan = _am;
        GetAugments(augMan.GetRandomAugments(aA_avaliableAugments.Length));
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

            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            c_vendingCanvas.enabled = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Interacted() { }

    public void EndInteract()
    {
        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;

        StartCoroutine(MoveCamera(t_camPositionToReturnTo, pim.GetCamera().transform, false));

        c_vendingCanvas.enabled = false;
        pim.GetCamera().enabled = true;
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

        while (t < 1)
        {
            _t.localPosition = Vector3.Lerp(start, Vector3.zero, t);
            _t.rotation = Quaternion.Lerp(iRot, _t_transformToMoveTo.rotation, t);
            t += (Time.deltaTime * (1/f_lerpTime));
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

    public void ClickedAugment(int _i_augmentIndex)
    {
        if (b_isBeingUsed)
        {
            i_currentAugmentIndex = _i_augmentIndex;
            t_augmentHighlight.position = tA_augmentPositions[_i_augmentIndex].position;
            UpdateAugmentDisplay();

            as_source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            as_source.PlayOneShot(acA_beeps[UnityEngine.Random.Range(0, acA_beeps.Length)]);
        }
    }

    private void UpdateAugmentDisplay()
    {
        print("Is trying to update the Augment display to match Augment " + i_currentAugmentIndex);

        /*
        vmd_vendingMachineDisplay.t_levelNumber.text = aA_avaliableAugments[i_currentAugmentIndex].Level;
        vmd_vendingMachineDisplay.t_augmentName.text = aA_avaliableAugments[i_currentAugmentIndex].Name;
        vmd_vendingMachineDisplay.t_augmentType.text = aA_avaliableAugments[i_currentAugmentIndex].type;
        vmd_vendingMachineDisplay.t_augmentFits.text = aA_avaliableAugments[i_currentAugmentIndex].fits;
        vmd_vendingMachineDisplay.t_augmentEffects.text = aA_avaliableAugments[i_currentAugmentIndex].effects;
        vmd_vendingMachineDisplay.t_augmentCost.text = aA_avaliableAugments[i_currentAugmentIndex].cost;
        */
    }

    public void BuyAugment()
    {
        if (b_isBeingUsed)
            if (rbA_augmentRigidbodies[i_currentAugmentIndex])
            {
                as_source.pitch = 1;
                as_source.PlayOneShot(ac_whirringClip);
                StartCoroutine(MoveAugmentForward(rbA_augmentRigidbodies[i_currentAugmentIndex]));
                StartCoroutine(SpitOutAugment(aA_avaliableAugments[i_currentAugmentIndex].Aug));
                Augment[] grabbedAugment = new Augment[1];
                grabbedAugment[0] = aA_avaliableAugments[i_currentAugmentIndex].Aug;
                // Player Save data needs: 0, Cost of Augment, Augment Reference
                SaveEvent se = new SaveEvent(new PlayerSaveData(0, 0, grabbedAugment));
                Notify(se);
                aA_avaliableAugments[i_currentAugmentIndex] = augMan.GetRandomAugment(aA_avaliableAugments.Length);
                rbA_augmentRigidbodies[i_currentAugmentIndex] = null;
            }
    }
    private IEnumerator MoveAugmentForward(Rigidbody _rb)
    {
        for (int i = 0; i < 87; i++)
        {
            yield return new WaitForEndOfFrame();
            _rb.transform.position -= transform.right * 0.003f;
        }
        yield return new WaitForSeconds(0.4f);
        _rb.isKinematic = false;
        _rb.AddForce((-transform.right * 5) + transform.up, ForceMode.Impulse);
        _rb.AddTorque(new Vector3(UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360)), ForceMode.Impulse);
    }

    public void GetAugments(AugmentGo[] _aA_augments)
    {
        aA_avaliableAugments = _aA_augments;
    }

    private IEnumerator SpitOutAugment(Augment _aA_augmentToVomUp)
    {
        yield return new WaitForSeconds(1.8f);
        anim.SetTrigger("MoveFlap");
        yield return new WaitForSeconds(0.2f);
        GameObject _g = PoolManager.x.SpawnObject(go_augmentPrefab, t_augmentSpawnPoint.position, new Quaternion(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
        _g.GetComponent<Rigidbody>().AddForce(t_augmentSpawnPoint.forward * 5, ForceMode.Impulse);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        EndInteract();
    }

}

[System.Serializable]
public struct AugmentDisplay
{
    public Text t_levelNumber;
    public Text t_augmentName;
    public Text t_augmentType;
    public Text t_augmentFits;
    public Text t_augmentEffects;
    public Text t_augmentCost;
}