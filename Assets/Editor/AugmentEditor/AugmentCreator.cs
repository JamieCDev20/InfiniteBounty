using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class AugmentCreator
{
    
    private const string rs = "/Resources/";
    public static void CreateAugment(string _s_parsedData)
    {
        // The path to grab assets
        string path = Application.dataPath;

        // Create the augment to read in whatever augment data
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
        GameObject newPrefab = new GameObject();
        AugmentGo newAugmentGo = newPrefab.AddComponent<AugmentGo>();
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
        PrefabUtility.SaveAsPrefabAssetAndConnect(newPrefab, path + rs + "/Augments/" + newAugmentGo.Aug.Name + ".prefab", InteractionMode.UserAction);
        // Remove the gameobject from the scene
        Object.DestroyImmediate(newPrefab);
    }

    private GameObject CreateGameObject(string _s_name, string _s_meshProperties)
    {
        // Set the game object and its name
        GameObject newAugment = new GameObject();
        newAugment.name = _s_name;
        // Set its visuals
        MeshRenderer tex = newAugment.AddComponent<MeshRenderer>();
        MeshFilter mesh = newAugment.AddComponent<MeshFilter>();
        tex.sharedMaterial = Resources.Load<Material>(_s_meshProperties.Split(':', '}')[1].Replace(rs, "").Replace(".mat", ""));
        mesh.sharedMesh = Resources.Load<Mesh>(_s_meshProperties.Split(':', '}')[3].Replace(rs, "").Replace(".fbx", ""));
        return newAugment;
        
    }

    

    private ProjectileAugment CreateProjectile(string _s_projectileData)
    {
        // Create the projectile augment
        ProjectileAugment projAug = new ProjectileAugment();
        // Read its parameters
        int shots = int.Parse(_s_projectileData.Split(':', ',')[1]);
        float grav = float.Parse(_s_projectileData.Split(':', ',')[3]);
        PhysicMaterial pm = (PhysicMaterial)Resources.Load(_s_projectileData.Split(':', ',')[5]);
        float scale = float.Parse(_s_projectileData.Split(':', ',')[7]);
        // Set the parameters
        projAug.InitProjectile(shots, grav, pm, scale);
        return projAug;
    }

    private ConeAugment CreateCone(string _s_coneData)
    {
        // Create the Cone Augment
        ConeAugment coneAug = new ConeAugment();
        // Read its parameters
        float ang = float.Parse(_s_coneData.Split(':', ',')[1]);
        float len = float.Parse(_s_coneData.Split(':', '}')[2]);
        // Set its parameters
        coneAug.InitCone(ang, len);
        return coneAug;
    }
}
