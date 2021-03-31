using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class FuseSaver : MonoBehaviour, ObserverBase
{
    public static FuseSaver x;

    private Augment[] fusedAugs;
    private ProjectileAugment[] fusedProj;
    private ConeAugment[] fusedCone;
    private string filePath;

    #region Get/Sets
    public Augment[] FusedAugments { get { return fusedAugs; } }
    public ProjectileAugment[] FusedProjectiles { get { return fusedProj; } }
    public ConeAugment[] FusedCones { get{ return fusedCone; } }
    #endregion

    // Start is called before the first frame update
    public void Init()
    {
        filePath = Application.dataPath + "/Resources/FusedAugmentData.JSON";
        if (x != null)
        {
            if (x != this)
                Destroy(x);
        }
        else
            x = this;

        DontDestroyOnLoad(this);
        LoadFusedAugments();
    }


    private void LoadFusedAugments()
    {
        if(Resources.Load("FusedAugmentData") != null)
        {
            string fusedstring = AugmentLoader.LoadFusedAugmentJson();
            if(fusedstring != null)
            {
                fusedAugs = AugmentLoader.ReadAugmentData<Augment>(fusedstring);
                fusedProj = AugmentLoader.ReadAugmentData<ProjectileAugment>(fusedstring);
                fusedCone = AugmentLoader.ReadAugmentData<ConeAugment>(fusedstring);
            }
            DebugTheArraysMaaang();
        }
        else
        {
            File.Create(filePath);
        }
    }
    public void OnNotify(ObserverEvent oe_event)
    {
        switch (oe_event)
        {
            case FuseEvent fuseEvent:
                switch (fuseEvent.Augs[2].at_type)
                {
                    case AugmentType.standard:
                        if (!CheckDuplicateAugment(fusedAugs, fuseEvent.Augs[2].Name))
                            fusedAugs = AddToArray(fusedAugs, fuseEvent.Augs[2]);
                        break;
                    case AugmentType.projectile:
                        if (!CheckDuplicateAugment(fusedProj, fuseEvent.Augs[2].Name))
                        {
                            ProjectileAugment proj = fuseEvent.Proj[2];
                            fusedProj = AddToArray(fusedProj, proj);
                        }
                        break;
                    case AugmentType.cone:
                        if (!CheckDuplicateAugment(fusedCone, fuseEvent.Augs[2].Name))
                            fusedCone = AddToArray(fusedCone, fuseEvent.Cone[2]);
                        break;
                }
                Debug.Log(fuseEvent.Augs[2].Name);
                SaveFusedAugments();
                break;
        }
    }

    private bool CheckDuplicateAugment(Augment[] _arrayToCheck, string _nameToCheck)
    {
        if (Utils.ArrayIsNullOrZero(_arrayToCheck))
            return false;
        foreach(Augment aug in _arrayToCheck)
        {
            if (aug.Name == _nameToCheck)
                return true;
        }
        return false;
    }

    private T[] AddToArray<T>(T[] _arrayToAddTo, T castedItemToAdd)
    {
        return Utils.AddToArray(_arrayToAddTo, castedItemToAdd);
    }

    private void SaveFusedAugments()
    {
        string fusedData = string.Empty;
        if(!Utils.ArrayIsNullOrZero(fusedAugs))
            fusedData = JsonConvert.SerializeObject(fusedAugs);
        if(!Utils.ArrayIsNullOrZero(fusedProj))
            fusedData += JsonConvert.SerializeObject(fusedProj);
        if(!Utils.ArrayIsNullOrZero(fusedCone))
            fusedData += JsonConvert.SerializeObject(fusedCone);
        File.WriteAllText(filePath, fusedData);
    }
    private void DebugTheArraysMaaang()
    {
        if (!Utils.ArrayIsNullOrZero(fusedAugs))
        {
            foreach (Augment aug in fusedAugs)
                Debug.Log(string.Format("Length: {0} | Name: {1}", fusedAugs.Length, aug.Name));
        }
        if (!Utils.ArrayIsNullOrZero(fusedProj))
        {
            foreach (ProjectileAugment proj in fusedProj)
                Debug.Log(string.Format("Length: {0} | Name: {1}", fusedProj.Length, proj.Name));
        }
        if (!Utils.ArrayIsNullOrZero(fusedCone))
        {
            foreach(ConeAugment cone in fusedCone)
                Debug.Log(string.Format("Length: {0} | Name: {1}", fusedCone.Length, cone.Name));

        }
    }
}
