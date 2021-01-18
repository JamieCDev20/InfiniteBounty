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

    public void Interacted(Transform interactor)
    {
        print("I've been interacted with");

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
        i_currentAugmentIndex = _i_augmentIndex;
        t_augmentHighlight.position = tA_augmentPositions[_i_augmentIndex].position;
        UpdateAugmentDisplay();
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
        print("Should've bought an augment, but I don't know how");
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