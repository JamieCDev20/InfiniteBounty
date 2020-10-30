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

    public override void Use()
    {
        GameObject newProj = PoolManager.x.SpawnNewObject(go_hitBox, t_firePoint.position, t_firePoint.rotation);
        Bullet newBullet = newProj.GetComponent<Bullet>();
        newBullet.Setup(at_augments);
        newBullet.MoveBullet(t_firePoint.forward, f_shotSpeed);
    }
}
