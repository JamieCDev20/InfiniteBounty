using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Camera c_cam;
    private GameObject go_camPivot;
    private Rigidbody rb_rigidbody;
    private Animator a_anim;
    private int i_currentHealth;
    [SerializeField] private RectTransform rt_healthBar;

    [Header("Controls")]
    [SerializeField] private float f_camSensitivity;
    [SerializeField] private float f_walkSpeed;
    [SerializeField] private float f_useRange;
    [SerializeField] private float f_jumpForce;
    [SerializeField] private float f_gravity;
    private bool b_isGrounded;

    [Header("Right Gun Stats")]
    [SerializeField] private float f_timeBetweenShotsRight;
    private float f_currentFireTimerRight;
    [SerializeField] private float f_firePowerRight;
    [SerializeField] private GameObject go_firePointRight;
    [SerializeField] private GameObject go_bulletPrefabRight;
    [SerializeField] private int i_bulletsPerShotRight;
    [SerializeField] private GameObject go_weaponRightParent;
    [SerializeField] private GameObject[] goA_weaponsRight = new GameObject[0];

    [Header("Left Gun Stats")]
    [SerializeField] private float f_timeBetweenShotsLeft;
    private float f_currentFireTimerLeft;
    [SerializeField] private float f_firePowerLeft;
    [SerializeField] private GameObject go_firePointLeft;
    [SerializeField] private GameObject go_bulletPrefabLeft;
    [SerializeField] private int i_bulletsPerShotLeft;
    [SerializeField] private GameObject go_weaponLeftParent;
    [SerializeField] private GameObject[] goA_weaponsLeft = new GameObject[0];

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        a_anim = GetComponentInChildren<Animator>();

        c_cam = Camera.main;
        go_camPivot = c_cam.transform.parent.gameObject;
        rb_rigidbody = GetComponent<Rigidbody>();
        i_currentHealth = 5;
    }

    private void Update()
    {
        bool _b_isSprinting = Input.GetButton("Sprint");
        b_isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.11f, 0), 0.1f);

        rb_rigidbody.AddForce(Vector3.Scale((
           ((Input.GetAxis("Horizontal") * c_cam.transform.right) + (Input.GetAxis("Vertical") * c_cam.transform.forward)).normalized * f_walkSpeed * (_b_isSprinting ? 2 : 1)) //Walking inputs
            , new Vector3(1, 0, 1)) + new Vector3(0, !b_isGrounded ? -f_gravity : 0, 0)); //Removing the vertical axis from walking & applying extra gravity

        a_anim.SetFloat("Y", Input.GetAxis("Vertical") * (_b_isSprinting ? 2 : 1));
        a_anim.SetFloat("X", Input.GetAxis("Horizontal") * (_b_isSprinting ? 2 : 1));


        //Cam & looking
        transform.forward = Vector3.Lerp(transform.forward, Vector3.Scale(go_camPivot.transform.forward, new Vector3(1, 0, 1)), 0.4f);
        go_camPivot.transform.position = transform.position + new Vector3(0, 1, 0);
        go_camPivot.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * f_camSensitivity);
        go_camPivot.transform.localEulerAngles = new Vector3(go_camPivot.transform.localEulerAngles.x, go_camPivot.transform.localEulerAngles.y, 0);

        go_weaponRightParent.transform.forward = c_cam.transform.forward;
        go_weaponLeftParent.transform.forward = c_cam.transform.forward;

        if (Input.GetButtonDown("Jump")) Jump();
        if (Input.GetButtonDown("Use")) AttemptUse();
        if (Input.GetButton("Fire1")) FireLeft();
        if (Input.GetButton("Fire2")) FireRight();

        f_currentFireTimerRight -= Time.deltaTime;
        f_currentFireTimerLeft -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Systemic _s = collision.transform.GetComponent<Systemic>();
        if (_s != null)
        {
            if (_s.b_fire) TakeDamage(1);
            if (_s.b_electric) TakeDamage(1);
        }
    }

    private void TakeDamage(int _i_incomingDamage)
    {
        i_currentHealth -= _i_incomingDamage;

        if (i_currentHealth < 0)
            i_currentHealth = 0;

        rt_healthBar.localScale = new Vector3((float)i_currentHealth / 10, 1, 1);

        print(_i_incomingDamage);
        if (i_currentHealth == 0) Death();

    }

    private void Death()
    {
        i_currentHealth = 10;
        rt_healthBar.localScale = new Vector3((float)i_currentHealth / 10, 1, 1);

        for (int i = 0; i < LocationController.x.goA_playerObjects.Length; i++)
            LocationController.x.goA_playerObjects[i].transform.position = new Vector3(0, 0, i * 1.5f);
        LocationController.x.UnloadArea();
    }

    private void Jump()
    {
        if (b_isGrounded)
            rb_rigidbody.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
    }

    private void FireRight()
    {
        RaycastHit _hit;
        if (Physics.Raycast(c_cam.transform.position, c_cam.transform.forward, out _hit, f_useRange))
        {
            if (_hit.transform.tag == "Weapon")
            {
                WeaponBlock _wb_newWeapon = _hit.transform.GetComponent<WeaponBlock>();
                f_timeBetweenShotsRight = _wb_newWeapon.f_timeBetweenShots;
                f_firePowerRight = _wb_newWeapon.f_firePower;
                go_bulletPrefabRight = _wb_newWeapon.go_bulletPrefab;
                i_bulletsPerShotRight = _wb_newWeapon.i_bulletsPerShot;
                for (int i = 0; i < goA_weaponsRight.Length; i++)
                    goA_weaponsRight[i].SetActive(false);
                goA_weaponsRight[_wb_newWeapon.i_weaponVisualIndex].SetActive(true);
            }
        }

        if (f_currentFireTimerRight <= 0)
        {
            for (int i = 0; i < i_bulletsPerShotRight; i++)
            {
                GameObject _go_bullet = Instantiate(go_bulletPrefabRight, go_firePointRight.transform.position, c_cam.transform.rotation);
                _go_bullet.transform.Rotate(new Vector3(-1 + Random.value * 2, -1 + Random.value * 2, -1 + Random.value * 2) * (i + 1));

                _go_bullet.GetComponent<Rigidbody>().AddForce(_go_bullet.transform.forward * f_firePowerRight, ForceMode.Impulse);
                _go_bullet.SetActive(true);

                f_currentFireTimerRight = f_timeBetweenShotsRight;
            }
        }
    }

    private void FireLeft()
    {
        RaycastHit _hit;
        if (Physics.Raycast(c_cam.transform.position, c_cam.transform.forward, out _hit, f_useRange))
        {
            if (_hit.transform.tag == "Weapon")
            {
                WeaponBlock _wb_newWeapon = _hit.transform.GetComponent<WeaponBlock>();
                f_timeBetweenShotsLeft = _wb_newWeapon.f_timeBetweenShots;
                f_firePowerLeft = _wb_newWeapon.f_firePower;
                go_bulletPrefabLeft = _wb_newWeapon.go_bulletPrefab;
                i_bulletsPerShotLeft = _wb_newWeapon.i_bulletsPerShot;
                for (int i = 0; i < goA_weaponsLeft.Length; i++)
                    goA_weaponsLeft[i].SetActive(false);
                goA_weaponsLeft[_wb_newWeapon.i_weaponVisualIndex].SetActive(true);

            }
        }

        if (f_currentFireTimerLeft <= 0)
        {
            for (int i = 0; i < i_bulletsPerShotLeft; i++)
            {
                GameObject _go_bullet = Instantiate(go_bulletPrefabLeft, go_firePointLeft.transform.position, c_cam.transform.rotation);
                _go_bullet.transform.Rotate(new Vector3(-1 + Random.value * 2, -1 + Random.value * 2, -1 + Random.value * 2) * (i + 1));
                _go_bullet.GetComponent<Rigidbody>().AddForce(_go_bullet.transform.forward * f_firePowerLeft, ForceMode.Impulse);
                _go_bullet.SetActive(true);

                f_currentFireTimerLeft = f_timeBetweenShotsLeft;
            }
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