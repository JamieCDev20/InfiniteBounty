using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager x;
    [SerializeField] GameObject augRef;
    [SerializeField] Augment[] A_augs;
    [SerializeField] ProjectileAugment[] A_projAugs;
    [SerializeField] ConeAugment[] A_coneAugs;
    [SerializeField] private List<GameObject> go_augments = new List<GameObject>();

    public void Start()
    {
        if (x != null)
        {
            if (x != this)
                Destroy(gameObject);
        }
        else
            x = this;

        string augstr = AugmentLoader.LoadAugmentJson();
        A_augs = AugmentLoader.ReadAugmentData<Augment>(augstr);
        A_projAugs = AugmentLoader.ReadAugmentData<ProjectileAugment>(augstr);
        A_coneAugs = AugmentLoader.ReadAugmentData<ConeAugment>(augstr);
        SpawnPhysicalAugments();
        GetAllAugmentGameObjects();
        FindObjectOfType<VendingMachine>().Init(this);
        DontDestroyOnLoad(gameObject);

    }

    /// <summary>
    /// Spawns Physical augments to be sent to the players weapons, the vending machine, and inventory
    /// </summary>
    private void SpawnPhysicalAugments()
    {
        if (A_augs != null && A_projAugs != null && A_coneAugs != null)
        {
            if (!PoolManager.x.CheckIfPoolExists(augRef))
                PoolManager.x.CreateNewPool(augRef, A_augs.Length + A_projAugs.Length + A_coneAugs.Length);
            int iter = 0;
            foreach (IPoolable pool_aug in PoolManager.x.GetPooledObjects(augRef))
            {
                GameObject go_aug = pool_aug.GetGameObject();
                AugmentGo ago = go_aug.GetComponent<AugmentGo>();
                if (iter < A_augs.Length)
                    ago.Aug = A_augs[iter];
                else if (iter < A_augs.Length + A_projAugs.Length)
                    ago.Aug = A_projAugs[iter - A_augs.Length];
                else if (iter < A_augs.Length + A_projAugs.Length + A_coneAugs.Length)
                    ago.Aug = A_coneAugs[iter - (A_augs.Length + A_projAugs.Length)];
                ago.Mat = Resources.Load<Material>(ago.Aug.AugmentMaterial);
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

    public int GetNumberOfAugments()
    {
        return A_augs.Length + A_projAugs.Length + A_coneAugs.Length;
    }

    public AugmentGo GetAugment(string _s_augName)
    {
        foreach (GameObject augGo in go_augments)
        {
            AugmentGo aug = augGo.GetComponent<AugmentGo>();
            if (aug.Aug.Name == _s_augName)
                return aug;
        }
        return null;
    }

    public AugmentGo[] GetRandomAugments(int _i_size)
    {
        AugmentGo[] augs = new AugmentGo[_i_size];
        for (int i = 0; i < _i_size; i++)
            augs[i] = go_augments[UnityEngine.Random.Range(0, go_augments.Count - 1)].GetComponent<AugmentGo>();
        return augs;
    }

    public AugmentGo GetRandomAugment(int _i_maxSize)
    {
        return go_augments[UnityEngine.Random.Range(0, _i_maxSize <= go_augments.Count ? _i_maxSize + 1 : go_augments.Count + 1)].GetComponent<AugmentGo>();
    }
}
