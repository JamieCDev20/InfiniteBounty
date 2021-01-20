using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    [SerializeField] GameObject augRef;
    [SerializeField] Augment[] A_augs;
    [SerializeField] ProjectileAugment[] A_projAugs;
    [SerializeField] ConeAugment[] A_coneAugs;
    private List<GameObject> go_augments = new List<GameObject>();

    public void Start()
    {
        A_augs = AugmentLoader.LoadInitialAugments();
        A_projAugs = AugmentLoader.LoadProjectileAugments();
        A_coneAugs = AugmentLoader.LoadConeAugments();
        SpawnPhysicalAugments();
        GetAllAugmentGameObjects();
        FindObjectOfType<VendingMachine>().Init(this);
    }

    /// <summary>
    /// Spawns Physical augments to be sent to the players weapons, the vending machine, and inventory
    /// </summary>
    private void SpawnPhysicalAugments()
    {
        if(A_augs != null && A_projAugs != null && A_coneAugs != null)
        {
            PoolManager.x.CreateNewPool(augRef, A_augs.Length + A_projAugs.Length + A_coneAugs.Length);
            int iter = 0;
            foreach(IPoolable pool_aug in PoolManager.x.GetPooledObjects(augRef))
            {
                GameObject go_aug = pool_aug.GetGameObject();
                AugmentGo ago = go_aug.GetComponent<AugmentGo>();
                //
                if (iter < A_augs.Length)
                    ago.Aug = A_augs[iter];
                else if (iter < A_augs.Length + A_projAugs.Length)
                    ago.Aug = A_projAugs[iter - A_augs.Length];
                else if (iter < A_augs.Length + A_projAugs.Length + A_coneAugs.Length)
                    ago.Aug = A_coneAugs[iter - (A_augs.Length + A_projAugs.Length)];
                iter++;
            }
        }
    }

    private void GetAllAugmentGameObjects()
    {
        HashSet<IPoolable> poolables = PoolManager.x.GetPooledObjects(augRef);
        foreach (IPoolable pooledAug in poolables)
            go_augments.Add(pooledAug.GetGameObject());
    }

    public AugmentGo[] GetRandomAugments(int _i_size)
    {
        AugmentGo[] augs = new AugmentGo[_i_size];
        for (int i = 0; i < _i_size; i++)
            augs[i] = go_augments[Random.Range(0, go_augments.Count - 1)].GetComponent<AugmentGo>();
        return augs;
    }

    public AugmentGo GetRandomAugment(int _i_maxSize)
    {
        return go_augments[Random.Range(0, _i_maxSize)].GetComponent<AugmentGo>();
    }
}
