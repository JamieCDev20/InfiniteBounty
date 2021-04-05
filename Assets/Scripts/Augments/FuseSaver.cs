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
    private AugmentSave[] _savedData;
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


    public void LoadFusedAugments()
    {
        if(Resources.Load("FusedAugmentData") != null)
        {
            string fusedstring = AugmentLoader.LoadFusedAugmentJson();
            if (fusedstring == string.Empty)
            {
                fusedAugs = new Augment[0];
                fusedProj = new ProjectileAugment[0];
                fusedCone = new ConeAugment[0];
                return;
            }

            _savedData = JsonConvert.DeserializeObject<AugmentSave[]>(fusedstring);
            List<Augment> _newAugs = new List<Augment>();
            List<ProjectileAugment> _newProj = new List<ProjectileAugment>();
            List<ConeAugment> _newCone = new List<ConeAugment>();
            foreach(AugmentSave saved in _savedData)
            {
                switch (saved.SavedAugment.augType)
                {
                    case AugmentType.projectile:
                        _newProj.Add(AugmentManager.x.GetProjectileAugmentAt(saved.SavedAugment.augStage, saved.SavedAugment.indicies));
                        break;
                    case AugmentType.cone:
                        _newCone.Add(AugmentManager.x.GetConeAugmentAt(saved.SavedAugment.augStage, saved.SavedAugment.indicies));
                        break;
                    case AugmentType.standard:
                        _newAugs.Add(AugmentManager.x.GetStandardAugmentAt(saved.SavedAugment.augStage, saved.SavedAugment.indicies));
                        break;
                }
            }
            fusedAugs = _newAugs.ToArray();
            fusedProj = _newProj.ToArray();
            fusedCone = _newCone.ToArray();
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
                //Debug.Log(_savedData.Length);
                _savedData = Utils.AddToArray(_savedData, fuseEvent.SavedAug);
                //Debug.Log(_savedData.Length);
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
        string fusedData = JsonConvert.SerializeObject(_savedData);
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
