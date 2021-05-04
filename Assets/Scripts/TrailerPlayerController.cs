using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrailerPlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    Vector3 movement;
    private void Update()
    {
        movement = Vector3.Lerp(movement, new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (Input.GetButton("Sprint") ? 2 : 1), 0.3f);
        anim.SetFloat("X", movement.x);
        anim.SetFloat("Y", movement.z);
        anim.SetBool("ShootingRight", Input.GetMouseButton(1));
        anim.SetBool("ShootingLeft", Input.GetMouseButton(0));
        if (Input.GetKeyDown(KeyCode.K))
            anim.SetTrigger("GetKnockedDown");
        anim.SetBool("GetRevived", Input.GetKey(KeyCode.R));
    }
}
