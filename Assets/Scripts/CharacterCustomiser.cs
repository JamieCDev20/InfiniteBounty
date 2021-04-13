using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomiser : MonoBehaviourPunCallbacks, IInteractible
{

    private AppearanceChanger ac_user;
    private CameraController cc_cam;
    private PlayerInputManager pim;
    [SerializeField] private Camera c_mirrorCam;
    [SerializeField] private GameObject go_uiStuff;
    private bool b_isBeingUsed;
    [SerializeField] private GameObject[] goA_playerSpots;
    [SerializeField] private GameObject[] goA_camSpots;

    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            ac_user = interactor.GetComponent<AppearanceChanger>();
            pim = interactor.GetComponent<PlayerInputManager>();
            pim.GetComponent<PlayerAnimator>().SetShootability(false);
            pim.b_shouldPassInputs = false;

            cc_cam = pim.GetCamera();
            cc_cam.gameObject.SetActive(false);
            c_mirrorCam.gameObject.SetActive(true);
            go_uiStuff.SetActive(true);

            ac_user.GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<ToolTip>().enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            interactor.position = goA_playerSpots[pim.GetID()].transform.position;
            interactor.forward = goA_playerSpots[pim.GetID()].transform.forward;
            c_mirrorCam.transform.position = goA_camSpots[pim.GetID()].transform.position;

            b_isBeingUsed = true;

        }
    }

    public void EndInteract()
    {
        if (!b_isBeingUsed)
            return;
        pim.b_shouldPassInputs = true;
        ac_user.GetComponent<Rigidbody>().isKinematic = false;
        pim.GetComponent<PlayerAnimator>().SetShootability(true);
        go_uiStuff.SetActive(false);
        cc_cam.gameObject.SetActive(true);
        c_mirrorCam.gameObject.SetActive(false);
        b_isBeingUsed = false;
        GetComponent<ToolTip>().enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Saving");
        FindObjectOfType<SaveManager>().OnNotify(new SaveEvent(new PlayerSaveData(-1, -1, -1,
            new int[] { ac_user.Body, ac_user.Head, ac_user.Arms, ac_user.Feet },
            null, null, null, null, null, -1)));

        TutorialManager.x.InteractedWithReflectron();
    }


    public void NextHead()
    {
        ac_user.NextHead();
    }
    public void LastHead()
    {
        ac_user.LastHead();
    }

    public void NextBody()
    {
        ac_user.NextBody();
    }
    public void LastBody()
    {
        ac_user.LastBody();
    }

    public void NextArms()
    {
        ac_user.NextArms();
    }
    public void LastArms()
    {
        ac_user.LastArms();
    }

    public void NextLegs()
    {
        ac_user.NextFeet();
    }
    public void LastLegs()
    {
        ac_user.LastFeet();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        EndInteract();
    }

}
