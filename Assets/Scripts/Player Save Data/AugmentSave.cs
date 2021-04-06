using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AugmentSave
{
    [Newtonsoft.Json.JsonProperty]
    public (AugmentStage augStage, AugmentType augType, int level, int[] indicies) SavedAugment;

    public AugmentSave() { }

    public AugmentSave((AugmentStage _stage, AugmentType _type, int _level, int[] _inds) saved)
    {
        SavedAugment = saved;
    }
    public AugmentSave(AugmentStage _stage, AugmentType _type, int _level, int[] _inds)
    {
        SavedAugment.augStage = _stage;
        SavedAugment.augType = _type;
        SavedAugment.level = _level;
        SavedAugment.indicies = _inds;
    }
    public AugmentSave((int _stage, int _type, int _level, int[] _inds) saved)
    {
        SavedAugment.augStage = (AugmentStage)saved._stage;
        SavedAugment.augType = (AugmentType)saved._type;
        SavedAugment.level = saved._level;
        SavedAugment.indicies = saved._inds;
    }
    public AugmentSave(int _stage, int _type, int _level, int[] _inds)
    {
        SavedAugment.augStage = (AugmentStage)_stage;
        SavedAugment.augType = (AugmentType)_type;
        SavedAugment.level = _level;
        SavedAugment.indicies = _inds;
    }

    public static bool operator ==(AugmentSave _augOne, AugmentSave _augTwo)
    {
        return _augOne.SavedAugment.level == _augTwo.SavedAugment.level ? _augOne / _augTwo : false;
    }
    public static bool operator !=(AugmentSave _augOne, AugmentSave _augTwo)
    {
        return !(_augOne == _augTwo);
    }

    public static bool operator /(AugmentSave _augOne, AugmentSave _augTwo)
    {
        if (_augOne.SavedAugment.augStage == _augTwo.SavedAugment.augStage && _augOne.SavedAugment.augType == _augTwo.SavedAugment.augType && _augOne.SavedAugment.indicies.Length == _augTwo.SavedAugment.indicies.Length)
        {
            for (int i = 0; i < _augTwo.SavedAugment.indicies.Length; i++)
                if (_augOne.SavedAugment.indicies[i] != _augTwo.SavedAugment.indicies[i])
                    return false;
            return true;
        }
        return false;
    }
}
