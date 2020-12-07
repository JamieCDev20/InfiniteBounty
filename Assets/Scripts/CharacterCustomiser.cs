using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomiser : MonoBehaviour, IInteractible
{

    private AppearanceChanger ac_user;
    private CameraController cc_cam;
    private PlayerInputManager pim;
    [SerializeField] private Camera c_mirrorCam;
    [SerializeField] private GameObject go_uiStuff;
    private bool b_isBeingUsed;

    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            ac_user = interactor.GetComponent<AppearanceChanger>();
            pim = interactor.GetComponent<PlayerInputManager>();
            pim.b_shouldPassInputs = false;

            cc_cam = pim.GetCamera();
            cc_cam.gameObject.SetActive(false);
            c_mirrorCam.gameObject.SetActive(true);
            go_uiStuff.SetActive(true);

            ac_user.GetComponent<Rigidbody>().isKinematic = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            b_isBeingUsed = true;
        }
    }

    public void EndInteract()
    {
        pim.b_shouldPassInputs = true;
        ac_user.GetComponent<Rigidbody>().isKinematic = false;
        go_uiStuff.SetActive(false);
        cc_cam.gameObject.SetActive(true);
        c_mirrorCam.gameObject.SetActive(false);
        b_isBeingUsed = false;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
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

}
