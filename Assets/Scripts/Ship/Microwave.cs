using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Microwave : SubjectBase, IInteractible
{
    private int i_currentlyClickedAugment;
    private bool b_inUse;
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
            pim = interactor.GetComponent<PlayerInputManager>();
            //th_currentTh = interactor.GetComponent<ToolHandler>();
            PlayerMover pm = pim.GetComponent<PlayerMover>();
            // Stop the player and camera from moving 
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;
            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
            pm.GetComponent<PlayerAnimator>().enabled = false;
            // Stop player from shooting
            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.SetShootability(false);
            _pa.StopWalking();
            // Move camera
            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
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
    }

    public void SetAugment()
    {
        // Put an augment in the empty slot
        if (aug_slotA == null)
        {
            Debug.Log(1);
            aug_slotA = aL_allAugmentsOwned[apd.CurrentAugIndex];
            apd.AugType = aL_allAugmentsOwned[apd.CurrentAugIndex].at_type;
            //apd.AugmentButtons[apd.CurrentAugIndex].SetActive(false);
            //apd.AugmentButtons[apd.CurrentAugIndex].GetComponent<Button>().interactable = false;
            go_augButtonA.GetComponentsInChildren<Text>()[0].text = aL_allAugmentsOwned[apd.CurrentAugIndex].Name;
            go_augButtonA.GetComponentsInChildren<Text>()[1].text = "Lv " + aL_allAugmentsOwned[apd.CurrentAugIndex].Level.ToString();
        }
        else if (aug_slotB == null)
        {
            Debug.Log(2);
            aug_slotB = aL_allAugmentsOwned[apd.CurrentAugIndex];
            apd.AugType = aL_allAugmentsOwned[apd.CurrentAugIndex].at_type;
            //apd.AugmentButtons[apd.CurrentAugIndex].SetActive(false);
            //apd.AugmentButtons[apd.CurrentAugIndex].GetComponent<Button>().interactable = false;
            go_augButtonB.GetComponentsInChildren<Text>()[0].text = aL_allAugmentsOwned[apd.CurrentAugIndex].Name;
            go_augButtonB.GetComponentsInChildren<Text>()[1].text = "Lv " + aL_allAugmentsOwned[apd.CurrentAugIndex].Level.ToString();
        }
        // Reveal fusion button, or reload augment list
        if (aug_slotA != null && aug_slotB != null)
        {
            Debug.Log(3);
            aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameTypeExcluding, false, aug_slotA.Name, aug_slotB.Name);
            RevealFuseButton();
        }
        else
        {
            Debug.Log(4);

            aL_allAugmentsOwned.Clear();

            if (aug_slotA?.Stage == AugmentStage.full || aug_slotB.Stage == AugmentStage.full)
                aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameTypeExcluding, false, aug_slotA != null? aug_slotA.Name : aug_slotB != null? aug_slotB.Name : "");
            else if (aug_slotA?.Stage == AugmentStage.fused || aug_slotB.Stage == AugmentStage.fused)
                aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameName, false);

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
            if (aug_slotB != null)
            {
                type = AugmentDisplayType.ShowSameTypeExcluding;
                apd.AugType = aug_slotB.at_type;
            }
        }
        else if (!_slotID)
        {
            aug_slotB = null;
            go_augButtonB.GetComponentsInChildren<Text>()[0].text = "";
            go_augButtonB.GetComponentsInChildren<Text>()[1].text = "";
            if (aug_slotA != null)
            {
                type = AugmentDisplayType.ShowSameTypeExcluding;
                apd.AugType = aug_slotA.at_type;
            }

        }

        if(aug_slotA != null || aug_slotB != null)
            apd.InitAugmentList(aL_allAugmentsOwned, type, false, _slotID? aug_slotB.Name : aug_slotA.Name);
        else
            apd.InitAugmentList(aL_allAugmentsOwned, type, false);

        UnrevealFuseButton();
    }

    public void Fuse()
    {
        UnrevealFuseButton();
        fusedAug = fuser.FuseAugments(aug_slotA, aug_slotB);
        apd.UpdatePropertyText(fusedAug);
        FuseEvent fe = new FuseEvent(new AugmentSave(AugmentStage.fused, aug_slotA.at_type, new int[2] {AugmentManager.x.GetAugmentIndex(aug_slotA.at_type, aug_slotA.Name), AugmentManager.x.GetAugmentIndex(aug_slotB.at_type, aug_slotB.Name) }));
        Notify(fe);
        // TODO:
        // Create fused augments file for all fused augments to be saved at.
        // Make ClearSaveData clear fused augments list
        // Play animation
        //RemoveAugmentEvent rae = new RemoveAugmentEvent(aug_slotB);
        //Notify(new SaveEvent(rae));
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
            Camera mainCam = Camera.main;
            mainCam.GetComponent<CameraRespectWalls>().enabled = true;
            mainCam.transform.localPosition = new Vector3(0, 0, -4);
            mainCam.transform.localEulerAngles = new Vector3(10, 0, 0);
        }
    }
}
