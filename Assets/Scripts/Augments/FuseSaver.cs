﻿using System.Collections;
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
    public ConeAugment[] FusedCones { get { return fusedCone; } }
    #endregion

    // Start is called before the first frame update
    public void Init()
    {
        filePath = Application.persistentDataPath + "/FusedAugmentData.JSON";
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
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
            DestroySaveData();
        }
        if (File.ReadAllText(filePath) != null)
        {
            string fusedstring = File.ReadAllText(filePath);
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
            foreach (AugmentSave saved in _savedData)
            {
                switch (saved.SavedAugment.augType)
                {
                    case AugmentType.projectile:
                        _newProj.Add(AugmentManager.x.GetProjectileAugmentAt(saved.SavedAugment.augStage, saved.SavedAugment.indicies));
                        _newProj[_newProj.Count - 1].Stage = AugmentStage.fused;
                        break;
                    case AugmentType.cone:
                        _newCone.Add(AugmentManager.x.GetConeAugmentAt(saved.SavedAugment.augStage, saved.SavedAugment.indicies));
                        _newCone[_newCone.Count - 1].Stage = AugmentStage.fused;
                        break;
                    case AugmentType.standard:
                        _newAugs.Add(AugmentManager.x.GetStandardAugmentAt(saved.SavedAugment.augStage, saved.SavedAugment.indicies));
                        _newAugs[_newAugs.Count - 1].Stage = AugmentStage.fused;
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
                Debug.Log("Augment Level: " + fuseEvent.SavedAug.SavedAugment.level);
                switch (fuseEvent.SavedAug.SavedAugment.augStage)
                {
                    case AugmentStage.full:
                        switch (fuseEvent.SavedAug.SavedAugment.augType)
                        {
                            case AugmentType.projectile:
                                fusedProj = Utils.AddToArray(fusedProj, AugmentManager.x.GetProjectileAugmentAt(AugmentStage.fused, fuseEvent.SavedAug.SavedAugment.indicies));
                                break;
                            case AugmentType.cone:
                                fusedCone = Utils.AddToArray(fusedCone, AugmentManager.x.GetConeAugmentAt(AugmentStage.fused, fuseEvent.SavedAug.SavedAugment.indicies));
                                break;
                            case AugmentType.standard:
                                fusedAugs = Utils.AddToArray(fusedAugs, AugmentManager.x.GetStandardAugmentAt(AugmentStage.fused, fuseEvent.SavedAug.SavedAugment.indicies));
                                break;
                        }
                        break;
                }
                SaveFusedAugments();
                break;
        }
    }

    public void RemoveStandardFromSave(Augment _save)
    {
        fusedAugs = Utils.OrderedRemove(fusedAugs, GetAugmentIndex(fusedAugs, _save));
        _savedData = Utils.OrderedRemove(_savedData, GetAugmentSaveIndex(new AugmentSave(_save.Stage, _save.at_type, _save.Level, AugmentManager.x.GetIndicesByName(_save.Name))));
    }

    public void RemoveProjectileFromSave(ProjectileAugment _save)
    {
        fusedProj = Utils.OrderedRemove(fusedProj, GetAugmentIndex(fusedProj, _save));
        _savedData = Utils.OrderedRemove(_savedData, GetAugmentSaveIndex(new AugmentSave(_save.Stage, _save.at_type, _save.Level, AugmentManager.x.GetIndicesByName(_save.Name))));
    }
    public void RemoveConeFromSave(ConeAugment _save)
    {
        fusedCone = Utils.OrderedRemove(fusedCone, GetAugmentIndex(fusedCone, _save));
        _savedData = Utils.OrderedRemove(_savedData, GetAugmentSaveIndex(new AugmentSave(_save.Stage, _save.at_type, _save.Level, AugmentManager.x.GetIndicesByName(_save.Name))));
    }
    public int GetAugmentIndex(Augment[] toCheck, Augment aug)
    {
        for (int i = 0; i < toCheck.Length; i++)
        {
            if (toCheck[i].Name == aug.Name)
                return i;
        }
        return -1;
    }

    public int ReturnAugmentSaveIndexByAugment(Augment aug)
    {
        for (int i = 0; i < _savedData.Length; i++)
        {
            Debug.Log(i);
            switch (aug.at_type)
            {
                case AugmentType.standard:
                    if (AugmentManager.x.GetStandardAugmentAt(_savedData[i].SavedAugment.augStage, _savedData[i].SavedAugment.indicies) == aug)
                        return i;
                    break;
                case AugmentType.projectile:
                    if (AugmentManager.x.GetProjectileAugmentAt(_savedData[i].SavedAugment.augStage, _savedData[i].SavedAugment.indicies) == (ProjectileAugment)aug)
                        return i;
                    break;
                case AugmentType.cone:
                    if (AugmentManager.x.GetConeAugmentAt(_savedData[i].SavedAugment.augStage, _savedData[i].SavedAugment.indicies) == (ConeAugment)aug)
                        return i;
                    break;
                default:
                    break;
            }
        }
        return -1;
    }

    public int GetAugmentSaveIndex(AugmentSave _augSave)
    {
        for (int i = 0; i < _savedData.Length; i++)
        {
            if (_savedData[i] == _augSave)
            {
                return i;
            }
        }
        return -1;
    }

    private bool CheckDuplicateAugment(Augment[] _arrayToCheck, string _nameToCheck)
    {
        if (Utils.ArrayIsNullOrZero(_arrayToCheck))
            return false;
        foreach (Augment aug in _arrayToCheck)
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

    public void DestroySaveData()
    {
        _savedData = new AugmentSave[0];
        string emptyFuseData = JsonConvert.SerializeObject(_savedData);
        File.WriteAllText(filePath, emptyFuseData);
        //File.Delete(filePath);
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
            foreach (ConeAugment cone in fusedCone)
                Debug.Log(string.Format("Length: {0} | Name: {1}", fusedCone.Length, cone.Name));

        }
    }
}
