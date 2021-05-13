using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMover : MoverBase
{

    [SerializeField] private float f_flyForce;
    private float stillTime;
    private bool b_lookAlongVelocity;
    private AudioSource as_source;
    [SerializeField] private AudioClip[] acA_flapSounds = new AudioClip[0];


    public override void Move(Vector3 _dir)
    {
        rb.AddForce(_dir.normalized * f_flyForce);
        if (b_lookAlongVelocity && rb.velocity.sqrMagnitude > 0.5f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(rb.velocity, Vector3.one - Vector3.up)), 0.3f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        stillTime = Time.realtimeSinceStartup;
        as_source = GetComponent<AudioSource>();
    }

    public void LookAtVelocity(bool v)
    {
        b_lookAlongVelocity = v;
    }

    private void FixedUpdate()
    {

        RaycastHit hit;
        float forw = 0;
        float down = 0;
        float fDis = 20;
        float dDis = 20;
        LayerMask mask = ~LayerMask.NameToLayer("Default");

        Physics.Raycast(transform.position, transform.forward, out hit, fDis, mask);
        if (hit.collider != null)
            forw = 1 - (hit.distance / fDis);
        Physics.Raycast(transform.position, -transform.up, out hit, dDis, mask);
        if (hit.collider != null)
            down = 1 - (hit.distance / dDis);

        rb.AddForce((Vector3.up * 9) + (((transform.up * forw) + (transform.up * down)) * f_flyForce * f_currentDiffMult));

        if (rb.velocity.sqrMagnitude > 1)
            stillTime = Time.realtimeSinceStartup;
        if (Time.realtimeSinceStartup - stillTime >= 5)
            GetComponent<IHitable>().Die();

    }

    private void Update()
    {
        if (!as_source.isPlaying)
        {
            as_source.clip = acA_flapSounds[Random.Range(0, acA_flapSounds.Length)];
            as_source.Play();
        }
    }

}
