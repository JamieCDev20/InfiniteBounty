using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Microwave : SubjectBase, IInteractible
{
    private int i_currentlyClickedAugment;
    private Augment aug_slotA;
    private Augment aug_slotB;
    private Augment fusedAug;
    private bool b_inUse;
    private PlayerInputManager pim;
    private List<Augment> aL_allAugmentsOwned = new List<Augment>();
    [SerializeField] private AugmentPropertyDisplayer apd;
    [SerializeField] private Transform t_playerPos;
    [SerializeField] private Canvas microwaveCanvas;
    [SerializeField] private Button fuseButton;
    [SerializeField] private AugmentFuser fuser;


    // Start is called before the first frame update
    void Init()
    {
        AddObserver(FindObjectOfType<SaveManager>());
    }

    public void Interacted(Transform interactor)
    {
        pim = interactor.GetComponent<PlayerInputManager>();
        interactor.position = t_playerPos.position;
        // Move camera & other tings

        apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);
        apd.ClickAugment(0);
    }
    public void Interacted(){ }

    public void UnInteract()
    {
        // NIIICK DO FANCY DANCY UI SHIT HERE MY BOII
    }

    public void SetAugment()
    {
        // Put an augment in the empty slot
        if (aug_slotA == null)
        {
            aug_slotA = aL_allAugmentsOwned[i_currentlyClickedAugment];
            apd.AugType = aL_allAugmentsOwned[i_currentlyClickedAugment].at_type;
        }
        else if (aug_slotB == null)
        {
            aug_slotB = aL_allAugmentsOwned[i_currentlyClickedAugment];
            apd.AugType = aL_allAugmentsOwned[i_currentlyClickedAugment].at_type;
        }
        // Reveal fusion button, or reload augment list
        if (aug_slotA != null && aug_slotB != null)
            RevealFuseButton();
        else
        {
            apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameType, false);

        }
    }

    public void RemoveAugment(int _slotID)
    {
        if (_slotID == 0)
            aug_slotA = null;
        else if (_slotID == 1)
            aug_slotB = null;
        fuseButton.interactable = false;
    }

    public void Fuse()
    {
        fuseButton.interactable = false;
        fusedAug = fuser.FuseAugments(aug_slotA, aug_slotB);
        apd.UpdatePropertyText(fusedAug);
        PlayerSaveData psd = new PlayerSaveData(-1, -1, -1, null, null, null, null, new Augment[] { fusedAug }, null, -1);
        Notify(new SaveEvent(psd));
        // TODO:
        // Create fused augments file for all fused augments to be saved at.
        // Make ClearSaveData clear fused augments list
    }

    private void RevealFuseButton()
    {
        fuseButton.interactable = true;
    }
}
