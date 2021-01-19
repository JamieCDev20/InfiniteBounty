using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AugmentLoader : MonoBehaviour
{
    private const string rs = "/Resources/";
    public static Augment[] LoadInitialAugments()
    {
        if(File.Exists(Application.dataPath + rs + "AugmentData.json"))
        {
            string augmentString = File.ReadAllText(Application.dataPath + rs + "AugmentData.json");
            Augment[] newAugs = ReadAugmentData(augmentString);
            return newAugs;
        }
        return null;
    }
    public static ProjectileAugment[] LoadProjectileAugments()
    {
        if (File.Exists(Application.dataPath + rs + "AugmentData.json"))
        {
            string augmentString = File.ReadAllText(Application.dataPath + rs + "AugmentData.json");
            ProjectileAugment[] newAugs = ReadProjectileAugment(augmentString);
            return newAugs;
        }
        return null;
    }
    public static ConeAugment[] LoadConeAugments()
    {
        if (File.Exists(Application.dataPath + rs + "AugmentData.json"))
        {
            string augmentString = File.ReadAllText(Application.dataPath + rs + "AugmentData.json");
            ConeAugment[] newAugs = ReadConeAugment(augmentString);
            return newAugs;
        }
        return null;
    }
    private static Augment[] ReadAugmentData(string augData)
    {
        string[] augments = augData.Split('\n');
        List<Augment> augs = new List<Augment>();
        // Make sure that you're only getting augments that are pure augs
        for (int i = 0; i < augments.Length; i++)
        {
            if (augments[i] != string.Empty)
            {
                if (GetAugmentType(augments[i]) == AugmentType.standard)
                    augs.Add(JsonUtility.FromJson<Augment>(augments[i]));
            }
        }
        return augs.ToArray();
    }
    private static ProjectileAugment[] ReadProjectileAugment(string augData)
    {
        string[] augments = augData.Split('\n');
        List<ProjectileAugment> augs = new List<ProjectileAugment>();
        for (int i = 0; i < augments.Length; i++)
        {
            if(augments[i] != string.Empty)
                if (GetAugmentType(augments[i]) == AugmentType.projectile)
                    augs.Add(JsonUtility.FromJson<ProjectileAugment>(augments[i]));
        }
        return augs.ToArray();
    }
    private static ConeAugment[] ReadConeAugment(string augData)
    {
        string[] augments = augData.Split('\n');
        List<ConeAugment> augs = new List<ConeAugment>();
        for (int i = 0; i < augments.Length; i++)
        {
            if(augments[i] != string.Empty)
                if (GetAugmentType(augments[i]) == AugmentType.cone)
                    augs.Add(JsonUtility.FromJson<ConeAugment>(augments[i]));
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
