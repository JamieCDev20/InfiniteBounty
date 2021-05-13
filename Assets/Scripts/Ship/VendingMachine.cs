using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VendingMachine : SubjectBase, IInteractible
{
    [SerializeField] private AugmentPropertyDisplayer apd;
    [SerializeField] private Transform t_camParent;
    private Transform t_camPositionToReturnTo;
    private PlayerInputManager pim;
    private AugmentManager augMan;
    [SerializeField] private int i_timesToLerpCam = 15;
    [SerializeField] private float f_lerpTime = 0.5f;
    [SerializeField] private float f_cameraMovementT = 0.3f;
    private int i_currentAugmentIndex;
    private AugmentGo[] aA_avaliableAugments = new AugmentGo[9];
    [SerializeField] private Canvas go_vendingCanvas;
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
    [SerializeField] private AudioClip ac_coinsSound;

    [Header("Spittin' out Augments")]
    [SerializeField] private GameObject go_augmentPrefab;
    [SerializeField] private Transform t_augmentSpawnPoint;
    private Animator anim;

    public bool IsBeingUsed { get { return b_isBeingUsed; } }

    private void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        GetAugments(augMan.GetRandomAugments(aA_avaliableAugments.Length < augMan.GetNumberOfAugments() ? aA_avaliableAugments.Length : augMan.GetNumberOfAugments(), tA_augmentPositions));
    }

    public void Init(AugmentManager _am)
    {
        anim = GetComponent<Animator>();
        as_source = GetComponent<AudioSource>();
        augMan = _am;

        GetAugments(augMan.GetRandomAugments(aA_avaliableAugments.Length < augMan.GetNumberOfAugments() ? aA_avaliableAugments.Length : augMan.GetNumberOfAugments(), tA_augmentPositions));
        int _i = UnityEngine.Random.Range(0, 9);
        i_currentAugmentIndex = 0;
        t_augmentHighlight.position = tA_augmentPositions[_i].position;
        AddObserver(FindObjectOfType<SaveManager>());
        apd.Init();
        if(aA_avaliableAugments.Length > 0)
            apd.UpdatePropertyText(aA_avaliableAugments[i_currentAugmentIndex].Aug);
        ClickedAugment(0);
    }

    #region Interactions

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            pim = interactor.GetComponent<PlayerInputManager>();
            interactor.position = t_playerPos.position;
            interactor.transform.forward = t_playerPos.forward;

            b_isBeingUsed = true;
            PlayerMover pm = pim.GetComponent<PlayerMover>();
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;
            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;

            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.enabled = false;
            _pa.SetShootability(false);
            _pa.StopWalking();

            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            go_vendingCanvas.transform.localScale = Vector2.one * 0.739f;
            go_vendingCanvas.enabled = true; ;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ClickedAugment(0);
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
        go_vendingCanvas.transform.localScale = Vector2.zero;
        go_vendingCanvas.enabled = false;
        pim.GetCamera().enabled = true;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.enabled = true;
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

    public void ClickedAugment(int _i_augmentIndex)
    {
        if (b_isBeingUsed)
        {
            apd.RemoveAugmentProperties();
            i_currentAugmentIndex = _i_augmentIndex;
            t_augmentHighlight.position = tA_augmentPositions[_i_augmentIndex].position;
            UpdateAugmentDisplay();
            apd.UpdatePropertyText(aA_avaliableAugments[i_currentAugmentIndex].Aug);

            as_source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            as_source.PlayOneShot(acA_beeps[UnityEngine.Random.Range(0, acA_beeps.Length)]);
        }
    }

    private void UpdateAugmentDisplay()
    {
        apd.AugDisplay.t_augmentName.text = aA_avaliableAugments[i_currentAugmentIndex].Aug.Name;
        switch (aA_avaliableAugments[i_currentAugmentIndex].Aug.at_type)
        {
            case AugmentType.standard:
                apd.SetFitIcon(0);
                break;
            case AugmentType.projectile:
                apd.SetFitIcon(1);
                break;
            case AugmentType.cone:
                apd.SetFitIcon(2);
                break;
        }

        apd.AugDisplay.t_levelNumber.text = aA_avaliableAugments[i_currentAugmentIndex].Aug.Level.ToString();
        apd.AugDisplay.t_augmentCost.text = aA_avaliableAugments[i_currentAugmentIndex].Aug.Cost.ToString();
    }

    public void BuyAugment()
    {
        if (b_isBeingUsed)
        {
            if(pim.GetComponent<NugManager>().Nugs >= aA_avaliableAugments[i_currentAugmentIndex].Aug.Cost)
            {
                if (rbA_augmentRigidbodies[i_currentAugmentIndex])
                {
                    as_source.pitch = 1;
                    as_source.PlayOneShot(ac_whirringClip);
                    StartCoroutine(MoveAugmentForward(rbA_augmentRigidbodies[i_currentAugmentIndex]));
                    //StartCoroutine(SpitOutAugment(aA_avaliableAugments[i_currentAugmentIndex].Aug));
                    Augment[] grabbedAugment = new Augment[1];
                    grabbedAugment[0] = aA_avaliableAugments[i_currentAugmentIndex].Aug;
                    InfoText.x?.OnNotify(new InfoTextEvent("You've purchased a " + grabbedAugment[0].Name));
                    pim.GetComponent<NugManager>().CollectNugs(-grabbedAugment[0].Cost, false);
                    SaveEvent se = new SaveEvent(new PlayerSaveData(pim.GetComponent<NugManager>().Nugs, -1, -1, null, null, null, null,
                        new AugmentSave[] { new AugmentSave(grabbedAugment[0].Stage, grabbedAugment[0].at_type, 1, new int[1] { AugmentManager.x.GetAugmentIndex(grabbedAugment[0].at_type, grabbedAugment[0].Name) }) },
                        null, 0)); ;
                    Notify(se);
                    aA_avaliableAugments[i_currentAugmentIndex] = augMan.GetRandomAugment(aA_avaliableAugments.Length);
                    rbA_augmentRigidbodies[i_currentAugmentIndex] = null;
                }
                as_source.PlayOneShot(ac_coinsSound);
            }
            else
            {
                // Play unable to buy sound
            }
        }
    }
    private IEnumerator MoveAugmentForward(Rigidbody _rb)
    {
        for (int i = 0; i < 87; i++)
        {
            yield return new WaitForSeconds(0.016f);
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
        if (b_isBeingUsed)
            EndInteract();
    }

}

[System.Serializable]
public struct AugmentDisplay
{
    public Text t_levelNumber;
    public Text t_augmentName;
    //public Text t_augmentFits;
    public GameObject[] goA_fitIcons;
    public Text t_augmentEffects;
    public Text t_augmentCost;
}