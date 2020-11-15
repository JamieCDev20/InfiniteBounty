using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTool : WeaponTool
{
    [SerializeField] private float f_shotSpeed;
    [SerializeField] private int i_shotsPerRound;
    [SerializeField] private PhysicMaterial pm_physicsMat;
    [SerializeField] private Transform t_firePoint;
    private Ray r_flightPath;

    public override void Use(Vector3 _v_forwards)
    {
        if (b_usable)
        {
            SpawnBullet(_v_forwards);
            b_usable = false;
            StartCoroutine(TimeBetweenUsage());
        }
    }

    public void SpawnBullet(Vector3 _v_direction)
    {
        Bullet newBullet = PoolManager.x.SpawnObject(go_hitBox, t_firePoint.position, t_firePoint.rotation).GetComponent<Bullet>();
        newBullet.Setup(at_augments);
        newBullet.MoveBullet(_v_direction, f_shotSpeed);
    }

}
