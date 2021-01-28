using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class AugmentCreator
{
    List<Augment> verbs = new List<Augment>();
    List<ProjectileAugment> projVerbs = new List<ProjectileAugment>();
    List<ConeAugment> coneVerbs = new List<ConeAugment>();
    private const string rs = "/Resources/";
    public static void CreateAugment(string _s_parsedData)
    {
        // The path to grab assets
        string path = Application.persistentDataPath;

        // Create the augment to read in Augment Data from JSON
        Augment aug = null;
        try
        {
            ProjectileAugment pa = JsonUtility.FromJson<ProjectileAugment>(_s_parsedData);
            aug = pa;
        }
        catch (System.InvalidCastException e) { }
        try
        {
            ConeAugment ca = JsonUtility.FromJson<ConeAugment>(_s_parsedData);
            aug = ca;
        }
        catch (System.InvalidCastException e) { }
        if (aug == null)
            aug = JsonUtility.FromJson<Augment>(_s_parsedData);
        // Create the prefab
        GameObject newAugment = new GameObject();
        AugmentGo newAugmentGo = newAugment.AddComponent<AugmentGo>();
        switch (aug)
        {
            case ProjectileAugment pa:
                newAugmentGo.Aug = pa;
                break;
            case ConeAugment ca:
                newAugmentGo.Aug = ca;
                break;
        }
        if (newAugmentGo.Aug == null)
            newAugmentGo.Aug = aug;
        //PoolManager.x.CreateNewPool(newAugment);
    }
}
