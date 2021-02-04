using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Lunchbox : MobilityTool
{
    [Header("Lunchbox Stats")]
    [SerializeField] private GameObject go_sandwichPrefab;
    private float f_coolDown;
    [SerializeField] private Vector3 v_lidOpenRotation;
    [SerializeField] private GameObject go_lidObject;
    private bool b_isOpen;
    [SerializeField] private Transform[] tA_sandWichFirePoints = new Transform[4];
    [SerializeField] private float f_sandwichForce;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip ac_closingSound;
    private AudioSource as_source;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (b_isOpen)
        {
            f_coolDown -= Time.deltaTime;
            if (f_coolDown < 0) CloseLid();
        }
    }

    private void CloseLid()
    {
        go_lidObject.transform.localEulerAngles = Vector3.zero;
        as_source.PlayOneShot(ac_closingSound);
        b_isOpen = false;
    }

    public override void Use(Vector3 _v)
    {
        if (!b_isOpen)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject _go_sandwich = PoolManager.x.SpawnObject(go_sandwichPrefab, tA_sandWichFirePoints[i].transform.position + transform.forward, Quaternion.identity);
                _go_sandwich.GetComponent<Rigidbody>().AddForce(tA_sandWichFirePoints[i].transform.forward * f_sandwichForce, ForceMode.Impulse);
            }
            go_lidObject.transform.localEulerAngles = v_lidOpenRotation;
            PlayAudio(ac_activationSound);

            f_coolDown = f_timeBetweenUsage;
            b_isOpen = true;
        }
    }

    public override void PlayParticles(bool val) { }

}
