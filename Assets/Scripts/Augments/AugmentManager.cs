using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    [SerializeField] GameObject augRef;
    [SerializeField] Augment[] A_augs;
    [SerializeField] ProjectileAugment[] A_projAugs;
    [SerializeField] ConeAugment[] A_coneAugs;

    public void Start()
    {
        A_augs = AugmentLoader.LoadInitialAugments();
        A_projAugs = AugmentLoader.LoadProjectileAugments();
        A_coneAugs = AugmentLoader.LoadConeAugments();
        Debug.Log("Chuck a debug.");
        SpawnPhysicalAugments();
    }

    /// <summary>
    /// Spawns Physical augments to be sent to the players weapons, the vending machine, and inventory
    /// </summary>
    private void SpawnPhysicalAugments()
    {
        PoolManager.x.CreateNewPool(augRef, A_augs.Length + A_projAugs.Length + A_coneAugs.Length);
        int iter = 0;
        foreach(IPoolable pool_aug in PoolManager.x.GetPooledObject(augRef))
        {
            GameObject go_aug = pool_aug.GetGameObject();
            AugmentGo ago = go_aug.GetComponent<AugmentGo>();
            //
            if (iter < A_augs.Length)
                ago.Aug = A_augs[iter];
            else if (iter < A_augs.Length + A_projAugs.Length)
                ago.Aug = A_projAugs[iter - A_projAugs.Length];
            else if (iter < A_augs.Length + A_projAugs.Length + A_coneAugs.Length)
                ago.Aug = A_coneAugs[iter - (A_augs.Length + A_projAugs.Length)];
            iter++;
        }
    }
}
