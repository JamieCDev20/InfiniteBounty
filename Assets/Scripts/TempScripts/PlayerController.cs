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
    private List<AugmentType> atL_activeAugmentsRight = new List<AugmentType>();

    [Header("Left Gun Stats")]
    [SerializeField] private float f_timeBetweenShotsLeft;
    private float f_currentFireTimerLeft;
    [SerializeField] private float f_firePowerLeft;
    [SerializeField] private GameObject go_firePointLeft;
    [SerializeField] private GameObject go_bulletPrefabLeft;
    [SerializeField] private int i_bulletsPerShotLeft;
    [SerializeField] private GameObject go_weaponLeftParent;
    [SerializeField] private GameObject[] goA_weaponsLeft = new GameObject[0];
    private List<AugmentType> atL_activeAugmentsLeft = new List<AugmentType>();

    [Header("Mobility")]
    [SerializeField] private float f_jetPackForce;
    [SerializeField] private RectTransform rt_jetpackHeat;
    private float f_jetpackHeat = 10;
    private bool b_flying;
    [SerializeField] private GameObject go_jetPackEffects;
    [Space, SerializeField] private float f_jumpDelay;

    [Header("Networking")]
    [SerializeField, Tooltip("Eventually every player will have an ID assigned by the network manager")] private int i_playerID;

    public int ID { get { return i_playerID; } set { i_playerID = value; } }

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

        if (Input.GetButton("Mobility")) Jetpack();
        else JetpackOff();
        rt_jetpackHeat.localScale = new Vector3((10 - (float)f_jetpackHeat) / 10, 1, 1);
        //print(10 - ((float)f_jetpackHeat / 10));

        rb_rigidbody.AddForce((Vector3.Scale(
           ((Input.GetAxis("Horizontal") * c_cam.transform.right) + (Input.GetAxis("Vertical") * c_cam.transform.forward)).normalized * f_walkSpeed * (_b_isSprinting ? 2 : 1) //Walking inputs
            , new Vector3(1, 0, 1)) + new Vector3(0, !b_isGrounded ? -f_gravity : 0, 0)) * Time.deltaTime); //Removing the vertical axis from walking & applying extra gravity

        if (!b_flying)
        {
            a_anim.SetFloat("Y", Input.GetAxis("Vertical") * (_b_isSprinting ? 2 : 1));
            a_anim.SetFloat("X", Input.GetAxis("Horizontal") * (_b_isSprinting ? 2 : 1));
        }
        else
        {
            a_anim.SetFloat("Y", 0);
            a_anim.SetFloat("X", 0);
        }

        a_anim.SetBool("ShootingRight", Input.GetButton("Fire2"));
        a_anim.SetBool("ShootingLeft", Input.GetButton("Fire1"));

        //Cam & looking
        transform.forward = Vector3.Lerp(transform.forward, Vector3.Scale(go_camPivot.transform.forward, new Vector3(1, 0, 1)), 0.4f);
        go_camPivot.transform.position = transform.position + new Vector3(0, 1, 0);
        go_camPivot.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * f_camSensitivity);
        go_camPivot.transform.localEulerAngles = new Vector3(go_camPivot.transform.localEulerAngles.x, go_camPivot.transform.localEulerAngles.y, 0);

        f_currentFireTimerRight -= Time.deltaTime;
        f_currentFireTimerLeft -= Time.deltaTime;

        a_anim.SetBool("JumpUp", false);
        if (Input.GetButtonDown("Jump")) Jump();
        if (Input.GetButtonDown("Use")) AttemptUse();

        if (b_flying || !b_isGrounded)
            a_anim.SetBool("FlyingPose", true);
        else
            a_anim.SetBool("FlyingPose", false);
    }


    private void Jetpack()
    {
        if (f_jetpackHeat > 0)
        {
            b_flying = true;
            go_jetPackEffects.SetActive(true);
            rb_rigidbody.AddForce(Vector3.up * f_jetPackForce * Time.deltaTime, ForceMode.Acceleration);
            b_isGrounded = true;
            f_jetpackHeat -= Time.deltaTime;
        }
        else JetpackOff();
    }
    private void JetpackOff()
    {
        b_flying = false;
        f_jetpackHeat += Time.deltaTime * 2;
        if (f_jetpackHeat > 10) f_jetpackHeat = 10;
        go_jetPackEffects.SetActive(false);
    }

    private void LateUpdate()
    {
        if (Input.GetButton("Fire2")) FireRight();
        if (Input.GetButton("Fire1")) FireLeft();
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
        Invoke("TrueJump", f_jumpDelay);
        a_anim.SetBool("JumpUp", true);
    }

    private void TrueJump()
    {
        if (b_isGrounded && !b_flying)
            rb_rigidbody.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
    }

    private void FireRight()
    {
        if (f_currentFireTimerRight <= 0)
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
                    return;
                }
                else if (_hit.transform.tag == "Augment")
                {
                    if (atL_activeAugmentsRight.Count < 3)
                    {
                        Augment _a = _hit.transform.GetComponent<Augment>();
                        switch (_a.GetAugment())
                        {
                            case AugmentType.Flame:
                                if (!atL_activeAugmentsRight.Contains(AugmentType.Flame))
                                    atL_activeAugmentsRight.Add(_a.GetAugment());
                                break;

                            case AugmentType.Electric:
                                if (!atL_activeAugmentsRight.Contains(AugmentType.Electric))
                                    atL_activeAugmentsRight.Add(_a.GetAugment());
                                break;

                            case AugmentType.Gooey:
                                if (!atL_activeAugmentsRight.Contains(AugmentType.Gooey))
                                    atL_activeAugmentsRight.Add(_a.GetAugment());
                                break;

                            case AugmentType.Soaked:
                                if (!atL_activeAugmentsRight.Contains(AugmentType.Soaked))
                                    atL_activeAugmentsRight.Add(_a.GetAugment());
                                break;

                            case AugmentType.Heavy:
                                if (!atL_activeAugmentsRight.Contains(AugmentType.Heavy))
                                {
                                    atL_activeAugmentsRight.Add(_a.GetAugment());
                                    f_timeBetweenShotsRight += 0.05f;
                                    f_firePowerRight *= 0.3f;
                                }
                                break;

                            case AugmentType.Size:
                                if (!atL_activeAugmentsRight.Contains(AugmentType.Size))
                                {
                                    atL_activeAugmentsRight.Add(_a.GetAugment());
                                    f_timeBetweenShotsRight += 0.05f;
                                }
                                break;

                            case AugmentType.Speed:
                                if (!atL_activeAugmentsRight.Contains(AugmentType.Speed))
                                {
                                    atL_activeAugmentsRight.Add(_a.GetAugment());
                                    f_timeBetweenShotsRight -= 0.05f;
                                }
                                break;

                            case AugmentType.Explosive:
                                if (!atL_activeAugmentsRight.Contains(AugmentType.Explosive))
                                {
                                    atL_activeAugmentsRight.Add(_a.GetAugment());
                                    f_timeBetweenShotsRight += 0.05f;
                                }
                                break;
                        }
                        f_timeBetweenShotsRight = Mathf.Clamp(f_timeBetweenShotsRight, 0.05f, 0.7f);
                    }
                    return;
                }
            }

            for (int i = 0; i < i_bulletsPerShotRight; i++)
            {
                GameObject _go_bullet = OldPoolManager.x.SpawnNewObject(go_bulletPrefabRight, go_firePointRight.transform.position, c_cam.transform.rotation);
                _go_bullet.transform.Rotate(new Vector3(-1 + Random.value * 2, -1 + Random.value * 2, -1 + Random.value * 2) * (i + 1));
                _go_bullet.GetComponent<Rigidbody>().AddForce(_go_bullet.transform.forward * f_firePowerRight, ForceMode.Impulse);
                _go_bullet.SetActive(true);
                _go_bullet.GetComponent<Bullet>().Setup(atL_activeAugmentsRight.ToArray());

                f_currentFireTimerRight = f_timeBetweenShotsRight;
            }
        }
    }

    private void FireLeft()
    {
        if (f_currentFireTimerLeft <= 0)
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
                    return;
                }
                else if (_hit.transform.tag == "Augment")
                {
                    if (atL_activeAugmentsLeft.Count < 3)
                    {
                        Augment _a = _hit.transform.GetComponent<Augment>();
                        switch (_a.GetAugment())
                        {
                            case AugmentType.Flame:
                                if (!atL_activeAugmentsLeft.Contains(AugmentType.Flame))
                                    atL_activeAugmentsLeft.Add(_a.GetAugment());
                                break;

                            case AugmentType.Electric:
                                if (!atL_activeAugmentsLeft.Contains(AugmentType.Electric))
                                    atL_activeAugmentsLeft.Add(_a.GetAugment());
                                break;

                            case AugmentType.Gooey:
                                if (!atL_activeAugmentsLeft.Contains(AugmentType.Gooey))
                                    atL_activeAugmentsLeft.Add(_a.GetAugment());
                                break;

                            case AugmentType.Soaked:
                                if (!atL_activeAugmentsLeft.Contains(AugmentType.Soaked))
                                    atL_activeAugmentsLeft.Add(_a.GetAugment());
                                break;

                            case AugmentType.Heavy:
                                if (!atL_activeAugmentsLeft.Contains(AugmentType.Heavy))
                                {
                                    atL_activeAugmentsLeft.Add(_a.GetAugment());
                                    f_timeBetweenShotsLeft += 0.05f;
                                    f_firePowerLeft *= 0.3f;
                                }
                                break;

                            case AugmentType.Size:
                                if (!atL_activeAugmentsLeft.Contains(AugmentType.Size))
                                {
                                    atL_activeAugmentsLeft.Add(_a.GetAugment());
                                    f_timeBetweenShotsLeft += 0.05f;
                                }
                                break;

                            case AugmentType.Speed:
                                if (!atL_activeAugmentsLeft.Contains(AugmentType.Speed))
                                {
                                    atL_activeAugmentsLeft.Add(_a.GetAugment());
                                    f_timeBetweenShotsLeft -= 0.05f;
                                }
                                break;

                            case AugmentType.Explosive:
                                if (!atL_activeAugmentsLeft.Contains(AugmentType.Explosive))
                                {
                                    atL_activeAugmentsLeft.Add(_a.GetAugment());
                                    f_timeBetweenShotsLeft += 0.05f;
                                }
                                break;
                        }
                        f_timeBetweenShotsLeft = Mathf.Clamp(f_timeBetweenShotsLeft, 0.05f, 0.7f);
                    }
                    return;
                }
            }

            for (int i = 0; i < i_bulletsPerShotLeft; i++)
            {
                GameObject _go_bullet = OldPoolManager.x.SpawnNewObject(go_bulletPrefabLeft, go_firePointLeft.transform.position, c_cam.transform.rotation);
                _go_bullet.transform.rotation = c_cam.transform.rotation;
                _go_bullet.transform.Rotate(new Vector3(-1 + Random.value * 2, -1 + Random.value * 2, -1 + Random.value * 2) * (i + 1));
                _go_bullet.GetComponent<Rigidbody>().AddForce(_go_bullet.transform.forward * f_firePowerLeft, ForceMode.Impulse);
                _go_bullet.SetActive(true);
                _go_bullet.GetComponent<Bullet>().Setup(atL_activeAugmentsLeft.ToArray());

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