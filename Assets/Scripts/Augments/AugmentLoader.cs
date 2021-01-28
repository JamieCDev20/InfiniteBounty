using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AugmentLoader : MonoBehaviour
{
    public static Augment[] LoadInitialAugments()
    {
        if(Resources.Load("AugmentData"))
        {
            string augmentString = Resources.Load("AugmentData").ToString();
            Augment[] newAugs = ReadAugmentData(augmentString);
            return newAugs;
        }
        return null;
    }

    public static ProjectileAugment[] LoadProjectileAugments()
    {
        if (Resources.Load("AugmentData"))
        {
            string augmentString = Resources.Load("AugmentData").ToString();
            ProjectileAugment[] newAugs = ReadProjectileAugment(augmentString);
            return newAugs;
        }
        return null;
    }

    public static ConeAugment[] LoadConeAugments()
    {
        if (Resources.Load("AugmentData"))
        {
            string augmentString = Resources.Load("AugmentData").ToString();
            ConeAugment[] newAugs = ReadConeAugment(augmentString);
            return newAugs;
        }
        return null;
    }

    private static Augment[] ReadAugmentData(string augData)
    {
        string[] augments = augData.Split('\n');
        List<Augment> augs = new List<Augment>();
        List<Augment> fusedAugs = new List<Augment>();
        // Make sure that you're only getting augments that are pure augs
        for (int i = 0; i < augments.Length; i++)
        {
            if (augments[i] != string.Empty)
            {
                if (GetAugmentType(augments[i]) == AugmentType.standard)
                {
                    augs.Add(JsonUtility.FromJson<Augment>(augments[i]));
                    Debug.Log(augs[i].Name);
                }
            }
        }
        for(int i = 0; i < augs.Count - 1; i++)
        {
            for (int j = 0; j < augs.Count; j++)
            {
                if(j > i)
                {
                    fusedAugs.Add(augs[i] + augs[j]);
                }
            }
        }
        foreach (Augment fa in fusedAugs)
            Debug.Log(fa.Name);
        return fusedAugs.ToArray();
    }

    private static ProjectileAugment[] ReadProjectileAugment(string augData)
    {
        string[] augments = augData.Split('\n');
        List<ProjectileAugment> augs = new List<ProjectileAugment>();
        List<ProjectileAugment> fusedAugs = new List<ProjectileAugment>();
        for (int i = 0; i < augments.Length; i++)
        {
            if(augments[i] != string.Empty)
                if (GetAugmentType(augments[i]) == AugmentType.projectile)
                    augs.Add(JsonUtility.FromJson<ProjectileAugment>(augments[i]));
        }
        for (int i = 0; i < augs.Count; i++)
        {
            for (int j = 0; j < augs.Count; j++)
            {
                if (augs[i] != augs[j])
                {
                    //if (!fusedAugs.Contains(AugmentFuser.VerbCombine(augs[i], augs[j]))
                      //  fusedAugs.Add(AugmentFuser.VerbCombine(augs[i], augs[j]));
                }
            }
        }
        return augs.ToArray();
    }

    private static ConeAugment[] ReadConeAugment(string augData)
    {
        string[] augments = augData.Split('\n');
        List<ConeAugment> augs = new List<ConeAugment>();
        List<ConeAugment> fusedAugs = new List<ConeAugment>();
        for (int i = 0; i < augments.Length; i++)
        {
            if(augments[i] != string.Empty)
                if (GetAugmentType(augments[i]) == AugmentType.cone)
                    augs.Add(JsonUtility.FromJson<ConeAugment>(augments[i]));
        }
        for (int i = 0; i < augs.Count; i++)
        {
            for (int j = 0; j < augs.Count; j++)
            {
                if (augs[i] != augs[j])
                {
                    //if (!fusedAugs.Contains(AugmentFuser.VerbCombine(augs[i], augs[j]))
                       // fusedAugs.Add(AugmentFuser.VerbCombine(augs[i], augs[j]));
                }
            }
        }
        return augs.ToArray();
    }

    private static AugmentType GetAugmentType(string _atString)
    {
        string[] atString = _atString.Split(':');
        string enumString = string.Empty;
        for (int j = 0; j < atString.Length; j++)
        {
            if (atString[j].Contains("at_type"))
            {
                enumString = atString[j + 1].Contains(",") ? atString[j + 1].Split(',')[0] : atString[j + 1].Split('}')[0];
            }
        }
        return (AugmentType)int.Parse(enumString);
    }

}
