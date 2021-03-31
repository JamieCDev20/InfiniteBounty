using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AugmentSave
{
    [Newtonsoft.Json.JsonProperty]
    public (AugmentStage augStage, AugmentType augType, int[] indicies) SavedAugment;

    public AugmentSave() { }

    public AugmentSave((AugmentStage _stage, AugmentType _type, int[] _inds) saved)
    {
        SavedAugment = saved;
    }
    public AugmentSave(AugmentStage _stage, AugmentType _type, int[] _inds)
    {
        SavedAugment.augStage = _stage;
        SavedAugment.augType = _type;
        SavedAugment.indicies = _inds;
    }
    public AugmentSave((int _stage, int _type, int[] _inds) saved)
    {
        SavedAugment.augStage = (AugmentStage)saved._stage;
        SavedAugment.augType = (AugmentType)saved._type;
        SavedAugment.indicies = saved._inds;
    }
    public AugmentSave(int _stage, int _type, int[] _inds)
    {
        SavedAugment.augStage = (AugmentStage)_stage;
        SavedAugment.augType = (AugmentType)_type;
        SavedAugment.indicies = _inds;
    }
}
