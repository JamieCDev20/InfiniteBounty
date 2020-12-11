using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class AugmentCreator
{
    
    private const string rs = "/Resources/";
    public void CreateAugment(string[] _s_parsedData)
    {
        // The path to grab assets
        string path = Application.dataPath;

        // Create the augment to read in whatever augment data
        Augment aug = new Augment();
        switch (_s_parsedData[1])
        {
            case "Projectile Augment":
                aug = CreateProjectile(_s_parsedData[6]);
                break;
            case "Cone Augment":
                aug = CreateCone(_s_parsedData[7]);
                break;
        }
        // Add the standard audment data on top
        CreateStandardAugments(aug, _s_parsedData);
        // Create the prefab
        GameObject newPrefab = CreateGameObject(_s_parsedData[0], _s_parsedData[8]);
        newPrefab.AddComponent<AugmentGo>().Aug = aug;
        PrefabUtility.SaveAsPrefabAssetAndConnect(newPrefab, path + rs + "/Augments/" + _s_parsedData[0] + ".prefab", InteractionMode.UserAction);
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

    private void CreateStandardAugments(Augment _aug, string[] _s_augmentData)
    {

        // Crunchy crunchy audio. Need to lay some ground rules here.
        AudioClip use = Resources.Load<AudioClip>(_s_augmentData[2].Split(':', '}')[1].Replace(rs, "").Replace(".wav", ""));
        AudioClip travel = Resources.Load<AudioClip>(_s_augmentData[2].Split(':', '}')[3].Replace(rs, "").Replace(".wav", ""));
        AudioClip hit = Resources.Load<AudioClip>(_s_augmentData[2].Split(':', '}')[5].Replace(rs, "").Replace(".wav", ""));
        
        _aug.InitAudio(use, travel, hit);

        // Info stuff
        float weight = float.Parse(_s_augmentData[3].Split(':', ',')[1]);
        float recoil = float.Parse(_s_augmentData[3].Split(':', ',')[3]);
        float speed = float.Parse(_s_augmentData[3].Split(':', ',')[5]);
        float heatsink = float.Parse(_s_augmentData[3].Split(':', ',')[7]);
        float knockback = float.Parse(_s_augmentData[3].Split(':', ',')[9]);
        float energy = float.Parse(_s_augmentData[3].Split(':', ',')[11]);
        float dmg = float.Parse(_s_augmentData[3].Split(':', ',')[13]);
        float lode = float.Parse(_s_augmentData[3].Split(':', ',')[15].Replace("}}", ""));

        _aug.InitInfo(weight, recoil, speed, heatsink, knockback, energy, dmg, lode);

        // Physical Data
        float width = float.Parse(_s_augmentData[4].Split(':', ',')[1]);
        float lifeTime = float.Parse(_s_augmentData[4].Split(':', ',')[3]);
        // For now the array and Game Object are Null... I need a better way to extract them
        _aug.InitPhysical(width, lifeTime, null, null);

        float explockback = float.Parse(_s_augmentData[5].Split(':', ',')[1]);
        float detonationTime = float.Parse(_s_augmentData[5].Split(':', ',')[3].Split('}')[0]);
        // Same with explosion. Extract the game object file names
        _aug.InitExplosion(explockback, lifeTime, null, null);
    }

    

    private ProjectileAugment CreateProjectile(string _s_projectileData)
    {
        // Create the projectile augment
        ProjectileAugment projAug = new ProjectileAugment();
        // Read its parameters
        int shots = int.Parse(_s_projectileData.Split(':', ',')[1]);
        float grav = float.Parse(_s_projectileData.Split(':', ',')[3]);
        PhysicMaterial pm = (PhysicMaterial)Resources.Load(_s_projectileData.Split(':', ',')[5]);
        Vector3 scale = new Vector3(float.Parse(_s_projectileData.Split(':', ',')[6]), float.Parse(_s_projectileData.Split(':', ',')[8]), float.Parse(_s_projectileData.Split(':', '}')[6]));
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
