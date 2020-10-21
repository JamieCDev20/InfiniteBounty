using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBlock : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] internal float f_timeBetweenShots;
    [SerializeField] internal float f_firePower;
    [SerializeField] internal GameObject go_bulletPrefab;
    [SerializeField] internal int i_bulletsPerShot;
    [SerializeField] internal int i_weaponVisualIndex;

}
