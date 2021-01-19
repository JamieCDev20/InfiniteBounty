using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachine : MonoBehaviour, IInteractible
{

    [SerializeField] private Transform t_camParent;
    private Transform t_camPositionToReturnTo;
    private PlayerInputManager pim;
    [SerializeField] private int i_timesToLerpCam = 15;
    [SerializeField] private float f_cameraMovementT = 0.3f;
    private int i_currentAugmentIndex;
    private Augment[] aA_avaliableAugments = new Augment[9];
    [SerializeField] private Canvas c_vendingCanvas;
    [SerializeField] private VendingMachineDisplay vmd_vendingMachineDisplay;
    [SerializeField] private Transform[] tA_augmentPositions = new Transform[0];
    [SerializeField] private Transform t_augmentHighlight;
    [SerializeField] private Rigidbody[] rbA_augmentRigidbodies = new Rigidbody[0];

    [Header("Audio Thangs")]
    [SerializeField] private AudioClip ac_whirringClip;
    private AudioSource as_source;
    [SerializeField] private AudioClip[] acA_beeps = new AudioClip[0];
    private bool b_isBeingUsed;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        ClickedAugment(UnityEngine.Random.Range(0, 9));
        UpdateAugmentDisplay();
    }

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            b_isBeingUsed = true;
            pim = interactor.GetComponent<PlayerInputManager>();
            PlayerMover pm = pim.GetComponent<PlayerMover>();
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;
            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;

            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            c_vendingCanvas.enabled = true;

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

        c_vendingCanvas.enabled = false;
        pim.GetCamera().enabled = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        b_isBeingUsed = false;
    }

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
                StartCoroutine(SpitOutAugment(aA_avaliableAugments[i_currentAugmentIndex]));
                aA_avaliableAugments[i_currentAugmentIndex] = null;
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

    public void GetAugments(Augment[] _aA_augments, ProjectileAugment[] _paA_projs, ConeAugment[] _caA_cones)
    {
        Debug.LogError("AUGMENTS OBTAINED. PROCEEDING WIHT RETIAL.");
        aA_avaliableAugments = _aA_augments;
    }

    private IEnumerator SpitOutAugment(Augment _aA_augmentToVomUp)
    {
        yield return new WaitForSeconds(3);
        Debug.LogError("should be spitting an augment, but I don't know how to 3d-ify them");
    }


    [System.Serializable]
    private struct VendingMachineDisplay
    {
        public Text t_levelNumber;
        public Text t_augmentName;
        public Text t_augmentType;
        public Text t_augmentFits;
        public Text t_augmentEffects;
        public Text t_augmentCost;
    }
}