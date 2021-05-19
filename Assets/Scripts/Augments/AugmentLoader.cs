using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class AugmentLoader : MonoBehaviour
{
    public static string LoadAugmentJson()
    {
        if (Resources.Load("AugmentData"))
        {
            return Resources.Load("AugmentData").ToString();
        }
        return null;
    }
   
    public static T[] ReadAugmentData<T>(string augData) where T : Augment
    {
        string[] augments = augData.Split('\n');
        List<T> augs = new List<T>();
        List<T> fusedAugs = new List<T>();
        // Make sure that you're only getting augments that are pure augs
        for (int i = 0; i < augments.Length; i++)
        {
            if (augments[i] != string.Empty && !augments[i].Contains("null"))
            {
                if (GetAugmentType(augments[i]) == AugmentType.standard && typeof(T).Equals(typeof(Augment)))
                {
                    augs.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(augments[i]));
                }
                else if (GetAugmentType(augments[i]) == AugmentType.projectile && typeof(T).Equals(typeof(ProjectileAugment)))
                {
                    augs.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(augments[i]));
                }
                else if (GetAugmentType(augments[i]) == AugmentType.cone && typeof(T).Equals(typeof(ConeAugment)))
                {
                    augs.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(augments[i]));
                }
            }
        }
        List<T> theme = new List<T>();
        List<T> augType = new List<T>();
        for(int i = 0; i < augs.Count; i++)
        {
            switch (GetAugmentStage(JsonUtility.ToJson(augs[i])))
            {
                case AugmentStage.theme:
                    theme.Add(augs[i]);
                    break;
                case AugmentStage.type:
                    augType.Add(augs[i]);
                    break;
            }
        }
        // The last element will've always been fused with everything
        for (int i = 0; i < theme.Count; i++)
        {
            for (int j = 0; j < augType.Count; j++)
            {
                // Fuse the types to the themes
                if (typeof(T).Equals(typeof(ProjectileAugment)))
                {
                    fusedAugs.Add((T)(Augment)AugmentFuser.VerbCombine((ProjectileAugment)(Augment)theme[i], (ProjectileAugment)(Augment)augType[j]));
                }
                else if (typeof(T).Equals(typeof(ConeAugment)))
                {
                    Debug.Log((theme[i] as ConeAugment).GetConeData().f_radius);

                    fusedAugs.Add((T)(Augment)AugmentFuser.VerbCombine((ConeAugment)(Augment)theme[i], (ConeAugment)(Augment)augType[j]));

                    Debug.Log((fusedAugs[fusedAugs.Count - 1] as ConeAugment).Name + " : " + (fusedAugs[fusedAugs.Count-1] as ConeAugment).GetConeData().f_radius);
                }
                else if (typeof(T).Equals(typeof(Augment)))
                {
                    fusedAugs.Add((T)AugmentFuser.VerbCombine(theme[i], augType[j]));
                }
                fusedAugs[fusedAugs.Count-1].Level = 1;
                fusedAugs[fusedAugs.Count - 1].Stage = AugmentStage.full;
            }
        }
        return fusedAugs.ToArray();
    }

    /// <summary>
    /// TODO IMPLEMENT THIS PROPERLY
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_augString"></param>
    /// <param name="_augArray"></param>
    /// <param name="arrayInd"></param>
    /// <returns></returns>
    private static Element GetJsonElement<T>(string[] _augString, T[] _augArray, int arrayInd)
    {
        /// TODO FIX THIS SHIT OH MY FUCKING GOD
        string[] elem = _augString[arrayInd].Split(new string[] { "eo_element" }, System.StringSplitOptions.None);
        string actualElements = elem[1].Split('[', ']')[1];
        if (!string.IsNullOrEmpty(actualElements) && actualElements != " ")
        {
            return (Element)int.Parse(actualElements);
        }



        // This will return goo. Change this
        return 0;
        //Debug.Log("Elements: " + augs[i].AugElement.Length);
    }

    private static AugmentType GetAugmentType(string _atString)
    {
        string enumString = GetSpecificVar(_atString, "at_type");
        return (AugmentType)int.Parse(enumString);
    }

    private static AugmentStage GetAugmentStage(string _asString)
    {
        string enumString = GetSpecificVar(_asString, "as_stage");
        return (AugmentStage)int.Parse(enumString);
    }

    private static string GetSpecificVar(string _s_toSplit, string _s_splitter)
    {
        string[] split = _s_toSplit.Split(':');
        string outputValue = string.Empty;
        for (int j = 0; j < split.Length; j++)
        {
            if (split[j].Contains(_s_splitter))
            {
                outputValue = split[j + 1].Contains(",") ? split[j + 1].Split(',')[0] : split[j + 1].Split('}')[0];
            }
        }
        return outputValue;
    }

}
