using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera c_cam;
    private GameObject go_camPivot;
    private Rigidbody rb_rigidbody;

    [SerializeField] private float f_camSensitivity;
    [SerializeField] private float f_walkSpeed;
    [SerializeField] private float f_useRange;

    [Header("TEMP Gun Stats")]
    [SerializeField] private float f_timeBetweenShots;
    private float f_currentFireTimer;
    [SerializeField] private float f_firePower;
    [SerializeField] private GameObject go_firePoint;
    [SerializeField] private GameObject go_bulletPrefab;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        c_cam = Camera.main;
        go_camPivot = c_cam.transform.parent.gameObject;
        rb_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb_rigidbody.velocity = Vector3.Scale(((Input.GetAxis("Horizontal") * c_cam.transform.right) + (Input.GetAxis("Vertical") * c_cam.transform.forward)).normalized * f_walkSpeed, new Vector3(1, 0, 1));

        if (rb_rigidbody.velocity.magnitude > 0.1f)
            transform.forward = Vector3.Lerp(transform.forward, Vector3.Scale(rb_rigidbody.velocity.normalized, new Vector3(1, 0, 1)), 0.5f);

        go_camPivot.transform.position = transform.position + new Vector3(0, 1, 0);
        go_camPivot.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * f_camSensitivity);
        go_camPivot.transform.localEulerAngles = new Vector3(go_camPivot.transform.localEulerAngles.x, go_camPivot.transform.localEulerAngles.y, 0);

        if (Input.GetButtonDown("Use")) AttemptUse();
        if (Input.GetButton("Fire2")) FireRight();

        f_currentFireTimer -= Time.deltaTime;
    }

    private void FireRight()
    {
        if (f_currentFireTimer <= 0)
        {
            GameObject _go_bullet = Instantiate(go_bulletPrefab, go_firePoint.transform.position, c_cam.transform.rotation);

            RaycastHit _hit;
            if (Physics.Raycast(c_cam.transform.position, c_cam.transform.forward, out _hit))
            {
                print("Firing at point");
                _go_bullet.transform.LookAt(_hit.point);
                _go_bullet.GetComponent<Rigidbody>().AddForce(_go_bullet.transform.forward * f_firePower, ForceMode.Impulse);
            }
            else
            {
                _go_bullet.GetComponent<Rigidbody>().AddForce(c_cam.transform.forward * f_firePower, ForceMode.Impulse);
            }
            f_currentFireTimer = f_timeBetweenShots;
        }
    }

    private void AttemptUse()
    {
        RaycastHit _hit;
        if (Physics.Raycast(c_cam.transform.position, c_cam.transform.forward, out _hit, f_useRange))
        {
            _hit.collider.GetComponent<IUseable>().OnUse();
        }
    }
}
