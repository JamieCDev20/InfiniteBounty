using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager x;
    FuseSaver fuseSave;
    [SerializeField] GameObject augRef;
    [SerializeField] Augment[] A_augs;
    [SerializeField] ProjectileAugment[] A_projAugs;
    [SerializeField] ConeAugment[] A_coneAugs;
    [SerializeField] private List<GameObject> go_augments = new List<GameObject>();

    public Dictionary<string, int[]> siAD_fusedToInds = new Dictionary<string, int[]>();

    private int unfusedStandard;
    private int unfusedProjectile;
    private int unfusedCone;

    public void Init()
    {
        if (x != null)
        {
            if (x != this)
                Destroy(gameObject);
        }
        else
            x = this;

        DontDestroyOnLoad(gameObject);
        fuseSave = FindObjectOfType<FuseSaver>();
    }

    public void JoinedRoom()
    {
        ResetInit();
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void ResetInit()
    {
        string augstr = AugmentLoader.LoadAugmentJson();
        A_augs = AugmentLoader.ReadAugmentData<Augment>(augstr);
        A_projAugs = AugmentLoader.ReadAugmentData<ProjectileAugment>(augstr);
        A_coneAugs = AugmentLoader.ReadAugmentData<ConeAugment>(augstr);
        unfusedStandard = A_augs.Length;
        unfusedProjectile = A_projAugs.Length;
        unfusedCone = A_coneAugs.Length;
        if (fuseSave != null)
        {
            if (fuseSave.FusedAugments != null)
                A_augs = Utils.CombineArrays(A_augs, fuseSave.FusedAugments);
            if (fuseSave.FusedProjectiles != null)
                A_projAugs = Utils.CombineArrays(A_projAugs, fuseSave.FusedProjectiles);
            if (fuseSave.FusedCones != null)
                A_coneAugs = Utils.CombineArrays(A_coneAugs, fuseSave.FusedCones);
        }
        if(Utils.ArrayIsNullOrZero(go_augments.ToArray()))
            GetAllAugmentGameObjects();
        SpawnPhysicalAugments();
        InitAugmentScripts();
    }

    private void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        if (s.name.Contains("Lob"))
            ResetInit();
    }

    private void InitAugmentScripts()
    {
        FindObjectOfType<FuseSaver>().Init();
        FindObjectOfType<VendingMachine>().Init(this);
        FindObjectOfType<Microwave>().Init();
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

    public void AddToDict(string _name, int[] _indices)
    {
        if (!siAD_fusedToInds.ContainsKey(_name))
            siAD_fusedToInds.Add(_name, _indices);
    }

    public int[] GetIndicesByName(string _name)
    {
        return siAD_fusedToInds[_name];
    }

    public int GetNumberOfStandardAugments()
    {
        return A_augs.Length;
    }

    public int GetNumberOfProjectileAugments()
    {
        return A_projAugs.Length;
    }

    public int GetNumberOfConeAugments()
    {
        return A_coneAugs.Length;
    }

    public int GetNumberOfAugments()
    {
        return A_augs.Length + A_projAugs.Length + A_coneAugs.Length;
    }

    public int[] GetAugmentIndicies(AugmentType _type, Augment[] _augs)
    {
        int[] _augInds = new int[_augs.Length];
        for (int i = 0; i < _augInds.Length; i++)
            _augInds[i] = GetAugmentIndex(_type, _augs[i].Name);
        return _augInds;
    }

    public int GetAugmentIndex(AugmentType _type, string _name)
    {
        switch (_type)
        {
            case AugmentType.projectile:
                for (int i = 0; i < A_projAugs.Length; i++)
                    if (A_projAugs[i].Name == _name)
                        return i;
                break;
            case AugmentType.cone:
                for (int i = 0; i < A_coneAugs.Length; i++)
                    if (A_coneAugs[i].Name == _name)
                        return i;
                break;
            case AugmentType.standard:
                for (int i = 0; i < A_augs.Length; i++)
                    if (A_augs[i].Name == _name)
                        return i;
                break;
        }
        return -1;
    }

    public Augment GetStandardAugmentAt(AugmentStage _stage, int[] _index)
    {
        return _stage == AugmentStage.full ? A_augs[_index[0]] : Augment.Combine(A_augs[_index[0]], A_augs[_index[1]]);
    }

    public ProjectileAugment GetProjectileAugmentAt(AugmentStage _stage, int[] _index)
    {
        return _index.Length < 2 ? A_projAugs[_index[0]] : ProjectileAugment.Combine(A_projAugs[_index[0]], A_projAugs[_index[1]]);
    }

    public ConeAugment GetConeAugmentAt(AugmentStage _stage, int[] _index)
    {
        return _stage == AugmentStage.full ? A_coneAugs[_index[0]] : ConeAugment.Combine(A_coneAugs[_index[0]], A_coneAugs[_index[1]]);
    }

    public Augment GetStandardAugment(string _s_augName)
    {
        foreach (Augment aug in A_augs)
            if (aug.Name == _s_augName)
                return aug;
        return null;
    }

    public ProjectileAugment GetProjectileAugment(string _s_augName)
    {
        foreach (ProjectileAugment aug in A_projAugs)
            if (aug.Name == _s_augName)
                return aug;
        return null;
    }
    public ConeAugment GetConeAugment(string _s_augName)
    {
        foreach (ConeAugment aug in A_coneAugs)
            if (aug.Name == _s_augName)
                return aug;
        return null;
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

    public AugmentGo[] GetRandomAugments(int _i_size, Transform[] _transforms)
    {
        AugmentGo[] augs = new AugmentGo[_i_size];
        for (int i = 0; i < augs.Length; i++)
        {
            augs[i] = PoolManager.x.SpawnObject(augRef, _transforms[i].position).GetComponent<AugmentGo>();
            augs[i].GetComponent<Rigidbody>().isKinematic = true;
            int augIndex = UnityEngine.Random.Range(0, A_augs.Length + A_projAugs.Length + A_coneAugs.Length);
            if (augIndex < A_augs.Length)
            {
                augs[i].Aug = A_augs[augIndex];
            }
            else if (augIndex < A_augs.Length + A_projAugs.Length)
            {
                augs[i].Aug = A_projAugs[augIndex - A_augs.Length];
            }
            else if (augIndex < A_augs.Length + A_projAugs.Length + A_coneAugs.Length)
            {
                augs[i].Aug = A_coneAugs[augIndex - (A_augs.Length + A_projAugs.Length)];
            }
        }
        return augs;
    }

    public AugmentGo GetRandomAugment(int _i_maxSize)
    {
        return go_augments[UnityEngine.Random.Range(0, _i_maxSize <= go_augments.Count ? _i_maxSize + 1 : go_augments.Count + 1)].GetComponent<AugmentGo>();
    }

    public void RemoveAugment(AugmentType type, string nam)
    {
        switch (type)
        {
            case AugmentType.standard:
                A_augs = Utils.OrderedRemove(A_augs, GetAugmentIndex(type, nam));
                break;
            case AugmentType.projectile:
                //Debug.Log("Augm ind: " + GetAugmentIndex(type, nam));
                A_projAugs = Utils.OrderedRemove(A_projAugs, GetAugmentIndex(type, nam));
                break;
            case AugmentType.cone:
                A_coneAugs = Utils.OrderedRemove(A_coneAugs, GetAugmentIndex(type, nam));
                break;
            default:
                break;
        }
    }

}
