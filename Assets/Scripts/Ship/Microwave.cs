using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Microwave : SubjectBase, IInteractible
{
    private int i_currentlyClickedAugment;
    private bool b_inUse;
    private bool b_initted = false;
    private Transform t_camPositionToReturnTo;
    private Augment aug_slotA;
    private Augment aug_slotB;
    private Augment fusedAug;
    private PlayerInputManager pim;
    private List<Augment> aL_allAugmentsOwned = new List<Augment>();
    [SerializeField] private float f_lerpTime;
    [SerializeField] private AugmentPropertyDisplayer apd;
    [SerializeField] private Transform t_playerPos;
    [SerializeField] private Transform t_camParent;
    [SerializeField] private Canvas microwaveCanvas;
    [SerializeField] private Button fuseButton;
    [SerializeField] private AugmentFuser fuser;
    [SerializeField] private GameObject go_augButtonA;
    [SerializeField] private GameObject go_augButtonB;

    [SerializeField] private AudioClip fuseSound;
    [SerializeField] private AudioSource fuseSource;
    [SerializeField] private GameObject nuclearParts;
    [SerializeField] private ParticleSystem fuseParts;
    [SerializeField] private Button[] crossButtons;
    [SerializeField] private GameObject insideAugmentParent;
    [SerializeField] private Animator anim;

    public AugmentPropertyDisplayer AugPropertyDisplay { get { return apd; } }


    // Start is called before the first frame update
    public void Init()
    {
        AddObserver(FindObjectOfType<SaveManager>());
        AddObserver(FindObjectOfType<FuseSaver>());

    }

    public void Interacted(Transform interactor)
    {
        if (!b_inUse)
        {
            // Move player
            pim = interactor.GetComponent<PlayerInputManager>();
            interactor.position = t_playerPos.position;
            interactor.transform.forward = t_playerPos.forward;

            // Don't let the menu be used twice
            b_inUse = true;

            // Stop the player and camera from moving 
            PlayerMover pm = pim.GetComponent<PlayerMover>();
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;
            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
            // Stop player from shooting
            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.enabled = false;
            _pa.SetShootability(false);
            _pa.StopWalking();

            // Move camera
            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            microwaveCanvas.transform.localScale = Vector3.one * 0.739f;
            microwaveCanvas.enabled = true;

            // Display the UI
            aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);

            // Enable cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }
    public void Interacted() { }

    public void UnInteract()
    {
        // TODO:
        // Make the camera stuff its own class becasue we dupe this code like 3 times bruh
        // Make the player able to move
        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;

        StartCoroutine(MoveCamera(t_camPositionToReturnTo, pim.GetCamera().transform, false));

        // Player is able to move camera
        microwaveCanvas.transform.localScale = Vector3.zero;
        microwaveCanvas.enabled = false;
        pim.GetCamera().enabled = true;

        // Player is able to animate again!
        pm.GetComponent<PlayerAnimator>().enabled = true;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.SetShootability(true);
        _pa.StartWalking();

        // Remove the cursor and allow the bench to be used again
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        b_inUse = false;

        RemoveAugment(true);
        RemoveAugment(false);
    }

    public void SetAugment()
    {
        // Put an augment in the empty slot
        if (aug_slotA == null)
        {
            aug_slotA = aL_allAugmentsOwned[apd.CurrentAugIndex];
            apd.AugType = aL_allAugmentsOwned[apd.CurrentAugIndex].at_type;
            aug_slotA.Stage = aL_allAugmentsOwned[apd.CurrentAugIndex].Stage;
            go_augButtonA.GetComponentsInChildren<Text>()[0].text = aL_allAugmentsOwned[apd.CurrentAugIndex].Name;
            go_augButtonA.GetComponentsInChildren<Text>()[0].color = aL_allAugmentsOwned[apd.CurrentAugIndex].Stage == AugmentStage.full? Color.white : Color.yellow;
            go_augButtonA.GetComponentsInChildren<Text>()[1].text = "Lv " + aL_allAugmentsOwned[apd.CurrentAugIndex].Level.ToString();
            go_augButtonA.GetComponentsInChildren<Image>()[1].sprite = apd.FitIcons[(int)aL_allAugmentsOwned[apd.CurrentAugIndex].at_type];
            go_augButtonA.GetComponentsInChildren<Image>()[1].color = Color.white;
        }
        else if (aug_slotB == null)
        {
            aug_slotB = aL_allAugmentsOwned[apd.CurrentAugIndex];
            apd.AugType = aL_allAugmentsOwned[apd.CurrentAugIndex].at_type;
            aug_slotB.Stage = aL_allAugmentsOwned[apd.CurrentAugIndex].Stage;
            go_augButtonB.GetComponentsInChildren<Text>()[0].text = aL_allAugmentsOwned[apd.CurrentAugIndex].Name;
            go_augButtonB.GetComponentsInChildren<Text>()[0].color = aL_allAugmentsOwned[apd.CurrentAugIndex].Stage == AugmentStage.full ? Color.white : Color.yellow;
            go_augButtonB.GetComponentsInChildren<Text>()[1].text = "Lv " + aL_allAugmentsOwned[apd.CurrentAugIndex].Level.ToString();
            go_augButtonB.GetComponentsInChildren<Image>()[1].sprite = apd.FitIcons[(int)aL_allAugmentsOwned[apd.CurrentAugIndex].at_type];
            go_augButtonB.GetComponentsInChildren<Image>()[1].color = Color.white;
        }

        // Reveal fusion button, or reload augment list
        if (aug_slotA != null && aug_slotB != null)
        {
            aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.None, false);
            RevealFuseButton();
        }
        else
        {
            aL_allAugmentsOwned.Clear();

            if (aug_slotA?.Stage == AugmentStage.full || aug_slotB?.Stage == AugmentStage.full)
                aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameTypeNotFusedExcluding, false, aug_slotA != null ? (aug_slotA.Name, aug_slotA.Level) : aug_slotB != null ? (aug_slotB.Name, aug_slotB.Level) : ("", 0));
            else if (aug_slotA?.Stage == AugmentStage.fused || aug_slotB?.Stage == AugmentStage.fused)
            {
                apd.AugmentName = aug_slotA == null ? aug_slotB.Name : aug_slotA.Name;
                aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameNameExcluding, false, aug_slotA != null ? (aug_slotA.Name, aug_slotA.Level) : aug_slotB != null ? (aug_slotB.Name, aug_slotB.Level) : ("", 0));
            }

        }
    }

    public void RemoveAugment(bool _slotID)
    {

        AugmentDisplayType type = AugmentDisplayType.ShowAll;

        if (_slotID)
        {
            aug_slotA = null;
            go_augButtonA.GetComponentsInChildren<Text>()[0].text = "";
            go_augButtonA.GetComponentsInChildren<Text>()[1].text = "";
            go_augButtonA.GetComponentsInChildren<Image>()[1].color = Color.clear;
            if (aug_slotB != null)
            {
                type = aug_slotB.Stage == AugmentStage.fused? AugmentDisplayType.ShowSameNameExcluding : AugmentDisplayType.ShowSameTypeNotFusedExcluding;
                apd.AugType = aug_slotB.at_type;
            }
        }
        else if (!_slotID)
        {
            aug_slotB = null;
            go_augButtonB.GetComponentsInChildren<Text>()[0].text = "";
            go_augButtonB.GetComponentsInChildren<Text>()[1].text = "";
            go_augButtonB.GetComponentsInChildren<Image>()[1].color = Color.clear;
            if (aug_slotA != null)
            {
                type = aug_slotA.Stage == AugmentStage.fused? AugmentDisplayType.ShowSameNameExcluding : AugmentDisplayType.ShowSameTypeNotFusedExcluding;
                apd.AugType = aug_slotA.at_type;
            }

        }

        if (aug_slotA != null || aug_slotB != null)
            aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, type, false, _slotID ? (aug_slotB.Name, aug_slotB.Level) : (aug_slotA.Name, aug_slotA.Level));
        else
            aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);

        UnrevealFuseButton();
    }

    public void Fuse()
    {
        StartCoroutine(DoingADelayedFuse());
        // TODO:
        // Play animation
    }

    IEnumerator DoingADelayedFuse()
    {
        UnrevealFuseButton();
        //unreveal cross buttons

        int aLevel = aug_slotA.Level;
        int bLevel = aug_slotB.Level;
        fusedAug = fuser.FuseAugments(aug_slotA, aug_slotB);
        fusedAug.at_type = aug_slotA.at_type;

        //start fusing audio
        fuseSource.clip = fuseSound;
        fuseSource.Play();
        foreach (Button b in crossButtons)
        {
            b.interactable = false;
        }
        insideAugmentParent?.SetActive(true);
        nuclearParts?.SetActive(true);
        anim?.SetBool("IsCooking?", true);

        yield return new WaitForSeconds(3.1f);

        insideAugmentParent?.SetActive(false);
        nuclearParts?.SetActive(false);
        anim?.SetBool("IsCooking?", false);
        fuseParts.Play();

        apd.UpdatePropertyText(fusedAug);
        FuseEvent fe = new FuseEvent(new AugmentSave(fusedAug.Stage, fusedAug.at_type, fusedAug.Level, fusedAug.Stage == AugmentStage.fused ?
            AugmentManager.x.GetIndicesByName(fusedAug.Name) : new int[2] { AugmentManager.x.GetAugmentIndex(aug_slotA.at_type, aug_slotA.Name),
                AugmentManager.x.GetAugmentIndex(aug_slotB.at_type, aug_slotB.Name) }), aug_slotA.Stage, aLevel, bLevel);

        Notify(fe);
        aug_slotA.Level = aLevel;
        switch (aug_slotA.at_type)
        {
            case AugmentType.projectile:
                FuseSaver.x.RemoveProjectileFromSave((ProjectileAugment)aug_slotA);
                FuseSaver.x.RemoveProjectileFromSave((ProjectileAugment)aug_slotB);
                break;
            case AugmentType.cone:
                FuseSaver.x.RemoveConeFromSave((ConeAugment)aug_slotB);
                FuseSaver.x.RemoveConeFromSave((ConeAugment)aug_slotA);
                break;
            case AugmentType.standard:
                FuseSaver.x.RemoveStandardFromSave(aug_slotB);
                FuseSaver.x.RemoveStandardFromSave(aug_slotA);
                break;
        }
        RemoveAugment(true);
        RemoveAugment(false);
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);

        //rereveal cross buttons
        foreach (Button b in crossButtons)
        {
            b.interactable = true;
        }
    }

    private void RevealFuseButton()
    {
        fuseButton.gameObject.SetActive(true);
        fuseButton.interactable = true;
    }
    private void UnrevealFuseButton()
    {
        fuseButton.gameObject.SetActive(false);
        fuseButton.interactable = false;
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
            Camera mainCam = Camera.main;
            mainCam.GetComponent<CameraRespectWalls>().enabled = true;
            mainCam.transform.localPosition = new Vector3(0, 0, -4);
            mainCam.transform.localEulerAngles = new Vector3(10, 0, 0);
        }
    }
}
