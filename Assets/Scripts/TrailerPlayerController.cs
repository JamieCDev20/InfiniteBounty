using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrailerPlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float TransitionSpeed = 0.2f;
    Vector3 movement;
    public GameObject sputnik1;
    public GameObject sputnik2;
    public GameObject sputnik3;
    public GameObject sputnik4;


    private void Update()
    {
        movement = Vector3.Lerp(movement, new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (Input.GetButton("Sprint") ? 2 : 1) * (Input.GetKey(KeyCode.LeftControl) ? 2 : 1), TransitionSpeed);
        anim.SetFloat("X", movement.x);
        anim.SetFloat("Y", movement.z);
        anim.SetBool("ShootingRight", Input.GetMouseButton(1));
        anim.SetBool("ShootingLeft", Input.GetMouseButton(0));
        if (Input.GetKeyDown(KeyCode.K))
            anim.SetTrigger("GetKnockedDown");
        anim.SetBool("GetRevived", Input.GetKey(KeyCode.R));
        if (Input.GetMouseButton(0))
        {
            sputnik1.SetActive(false);
            sputnik2.SetActive(true);
        }
        if (Input.GetMouseButton(1))
        {
            sputnik3.SetActive(false);
            sputnik4.SetActive(true);
        }
    }
}
