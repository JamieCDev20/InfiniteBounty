using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MobilityTool
{

    [Header("Bubble Shield Things")]
    [SerializeField] private GameObject go_shieldPrefab;
    private List<GameObject> goL_shieldPool = new List<GameObject>();
    [SerializeField] private Transform t_firePoint;
    [SerializeField] private float f_orbLifeTime;
    [SerializeField] private float f_shootForce;
    private float f_useTimer;
    [SerializeField] private GameObject go_growth;
    private bool b_isOnCoolDown;
    [SerializeField] private Material m_onCooldownMaterial;
    [SerializeField] private Material m_offCooldownMaterial;

    private Rigidbody rb_currentOrb;

    private void Awake()
    {
        f_useTimer = f_timeBetweenUsage;
        go_growth.GetComponent<MeshRenderer>().material = m_offCooldownMaterial;
        for (int i = 0; i < 2; i++)
        {
            GameObject _go = Instantiate(go_shieldPrefab, transform);
            _go.SetActive(false);
            goL_shieldPool.Add(_go);
        }
    }

    private void Update()
    {
        if (f_useTimer <= f_timeBetweenUsage)
            go_growth.transform.localScale = Vector3.one * (float)(f_useTimer / f_timeBetweenUsage);
        else if (b_isOnCoolDown)
        {
            b_isOnCoolDown = false;
            go_growth.GetComponent<MeshRenderer>().material = m_offCooldownMaterial;
        }

        f_useTimer += Time.deltaTime;
    }

    public override void Use(Vector3 _v_forwards)
    {
        ShootOrb(_v_forwards);
    }
    public override void Use() { }
    public override void NetUse(Vector3 _v_forwards) { }



    private void ShootOrb(Vector3 _v_forwards)
    {
        if (f_useTimer > f_timeBetweenUsage)
        {
            GameObject _go = goL_shieldPool[0];
            goL_shieldPool.RemoveAt(0);
            _go.transform.parent = null;
            _go.transform.position = t_firePoint.position;
            _go.SetActive(true);
            rb_currentOrb = _go.GetComponent<Rigidbody>();
            rb_currentOrb.isKinematic = false;
            rb_currentOrb.velocity = Vector3.zero;
            rb_currentOrb.AddForce(_v_forwards.normalized * f_shootForce, ForceMode.VelocityChange);

            StartCoroutine(ReturnOrbToPool(_go));
            f_useTimer = 0;
            b_isOnCoolDown = true;
            go_growth.GetComponent<MeshRenderer>().material = m_onCooldownMaterial;
        }
    }

    private IEnumerator ReturnOrbToPool(GameObject _go_orb)
    {
        yield return new WaitForSeconds(f_orbLifeTime);
        _go_orb.SetActive(false);
        goL_shieldPool.Add(_go_orb);
        rb_currentOrb = null;
        _go_orb.transform.parent = transform;
    }


}
