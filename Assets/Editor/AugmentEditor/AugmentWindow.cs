using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class AugmentWindow : EditorWindow
{
    static AugmentWindow wind;
    int i_arrSize;
    int i_prevSize;
    private const float f_windWidth = 800;
    private const float f_windHeight = 600;
    #region GUI Variables
    private string[] i_dropdownOptions = { "Standard Augment", "Projectile Augment", "Cone Augment"};
    private AugmentType at_type;
    private int i_dropDownIndex = 0;
    private bool b_displayBaseAugments = true;
    private Vector2 scrollPos;
    private List<Color> tex_color;
    #endregion

    Augment a_melee;


    #region Augment Vars

    string s_augName;
    int i_cost;
    AugmentStage as_stage;

    #endregion

    #region Audio

    public AudioClip[] ac_useSound;
    public AudioClip[] ac_travelSound;
    public AudioClip[] ac_hitSound;

    #endregion

    #region Tool Properties

    AugmentProperties ap_toolProperties;

    #endregion

    #region Physical Tool

    public AugmentPhysicals phys_toolPhys;
    GameObject go_weaponProjectile;

    #endregion

    #region EXPLOSION
    AugmentExplosion ae_splosion;
    public GameObject[] go_explarticles = { };
    public Color[] c_trailRenderer = { };
    GameObject target;
    #endregion

    #region AugmentGO physicals

    Material mat_material;

    #endregion

    #region projectile

    AugmentProjectile apro;
    PhysicMaterial pm_mat;

    #endregion

    #region Cone

    AugmentCone acone;

    #endregion

    public Element[] elements;

    [MenuItem("Window/Augment Editor")]
    static void Init()
    {
        wind = GetWindow<AugmentWindow>();
        wind.position = new Rect(0, 0, f_windWidth, f_windHeight);
    }

    private void OnGUI()
    {
        /*
        GUI.contentColor = Color.white;
        GUI.color = Color.white;
        GUI.backgroundColor = Color.grey;
        */

        // Main GUI loop where things need to update
        GUILayout.Label("Augment Type", EditorStyles.boldLabel);
        at_type = (AugmentType)EditorGUILayout.EnumPopup("", at_type);
        EditorGUILayout.Space();
        GUILayout.Label("Augment Name", EditorStyles.label);
        s_augName = EditorGUILayout.TextArea(s_augName);
        as_stage = (AugmentStage)EditorGUILayout.EnumPopup("Augment Stage", as_stage);
        i_cost = EditorGUILayout.IntField("Cost", i_cost);
        DisplayGameObjectArgs();

        EditorGUILayout.BeginVertical("box");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.Space();
        if (i_dropDownIndex != 0)
        {
            b_displayBaseAugments = EditorGUILayout.Toggle("Show Base Augments", b_displayBaseAugments);
        }

        ShowAugmentToCreate();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        SaveAugment();
    }

    private void SaveAugment()
    {
        if (GUILayout.Button("Save Augment"))
        {
            string rs = "/Resources/";
            string path = Application.dataPath + rs;
            string augmentData = "";

            // Opening/Creating File
            TextReader tr;
            try
            {
                tr = new StreamReader(path + "AugmentData.json", true);
                tr.Close();
            }
            catch (FileNotFoundException fnf)
            {
                File.WriteAllText(path + "AugmentData.json", "");
            }
            tr = new StreamReader(path + "AugmentData.json", true);

            // We don't want Duplicate Names
            if (s_augName != "")
            {
                string saveText = tr.ReadToEnd();
                tr.Close();
                if (saveText != "")
                    if (CheckIfNameUsed(saveText, s_augName))
                    {
                        Debug.LogError("Augment Name Already Used");
                        return;
                    }
                // Making the save data a tiny bit more readable
                ap_toolProperties.s_name = s_augName;
            }
            else
            {
                Debug.LogError("Augment Needs a name");
                tr.Close();
                return;
            }

            ap_toolProperties.i_cost = i_cost;
            Augment outputAug = new Augment();
            outputAug.Cost = i_cost;
            // Save variables time!
            switch (at_type)
            {
                case AugmentType.standard:
                    InitStandardAugment(outputAug);
                    break;
                case AugmentType.projectile:
                    ProjectileAugment pOutput = new ProjectileAugment();
                    pOutput.Cost = i_cost;
                    InitStandardAugment(pOutput);
                    try
                    {
                        pOutput.InitProjectile(apro);
                        outputAug = pOutput;
                    }catch(InvalidCastException e) { }
                    break;
                case AugmentType.cone:
                    ConeAugment cOutput = new ConeAugment();
                    cOutput.Cost = i_cost;
                    InitStandardAugment(cOutput);
                    try
                    {
                        cOutput.InitCone(acone);
                        outputAug = cOutput;
                    }catch (InvalidCastException e) { }
                    break;
            }
            outputAug.at_type = at_type;
            outputAug.Stage = as_stage;
            augmentData = EditorJsonUtility.ToJson(outputAug);
            File.AppendAllText(path + "AugmentData.json", augmentData + "\n");
            tr.Close();
            Debug.Log(String.Format("{0} Created!", s_augName));
            wind = (AugmentWindow)EditorWindow.GetWindow(typeof(AugmentWindow), false);
            wind.Close();
            wind = (AugmentWindow)EditorWindow.GetWindow(typeof(AugmentWindow), false);
            wind.Show();
        }
    }

    private bool CheckIfNameUsed(string sr, string _s_name)
    {
        string[] newLine = sr.Split('\n');
        for (int i = 0; i < newLine.Length; i++)
        {
            if (newLine[i] != "")
            {
                // Grab the first parameter of each new line and compare its name
                if(newLine[i].Split(':', ',')[1].Replace("\"", String.Empty) == _s_name)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ShowAugmentToCreate()
    {
        // Display the augment data relevent to the augment type
        switch (at_type)
        {
            case AugmentType.standard:
                DisplayBaseAugments();
                break;
            case AugmentType.projectile:
                DisplayProjectileAugments();
                if (b_displayBaseAugments)
                    DisplayBaseAugments();
                break;
            case AugmentType.cone:
                DisplayConeAugments();
                if (b_displayBaseAugments)
                    DisplayBaseAugments();
                break;
        }
    }

    private void DisplayGameObjectArgs()
    {
        // Display a mesh and material field
        GUILayout.Label("Augment mesh", EditorStyles.boldLabel);
        mat_material = (Material)EditorGUILayout.ObjectField(mat_material, typeof(Material), true);
    }

    /// <summary>
    /// Display each editable parameter with its lable
    /// </summary>
    private void DisplayBaseAugments()
    {
        GUILayout.Label("Base Augments", EditorStyles.boldLabel);
        GUILayout.Label("Physical Properties", EditorStyles.boldLabel);
        ap_toolProperties.f_speed = EditorGUILayout.FloatField("Weapon Speed", ap_toolProperties.f_speed);
        ap_toolProperties.f_recoil = EditorGUILayout.FloatField("Recoil", ap_toolProperties.f_recoil);
        ap_toolProperties.i_damage = EditorGUILayout.IntField("Enemy Damage", ap_toolProperties.i_damage);
        ap_toolProperties.i_lodeDamage = EditorGUILayout.IntField("Lode Damage", ap_toolProperties.i_lodeDamage);
        ap_toolProperties.f_knockback = EditorGUILayout.FloatField("Knockback", ap_toolProperties.f_knockback);
        ap_toolProperties.f_weight = EditorGUILayout.FloatField("Weight", ap_toolProperties.f_weight);
        ap_toolProperties.f_heatsink = EditorGUILayout.FloatField("Heatsink", ap_toolProperties.f_heatsink);
        ap_toolProperties.f_energyGauge = EditorGUILayout.FloatField("Energy Gauge", ap_toolProperties.f_energyGauge);
        GUILayout.Label("Audio Attributes", EditorStyles.boldLabel);
        GUILayout.Label("Usage Sounds", EditorStyles.label);
        DisplayArray("ac_useSound");
        GUILayout.Label("Travel Sounds", EditorStyles.label);
        DisplayArray("ac_travelSound");
        GUILayout.Label("Hit Sounds", EditorStyles.label);
        DisplayArray("ac_hitSound");
        GUILayout.Label("Trail and Particles", EditorStyles.boldLabel);
        phys_toolPhys.f_trWidth = EditorGUILayout.FloatField("Trail Width", phys_toolPhys.f_trWidth);
        phys_toolPhys.f_trLifetime = EditorGUILayout.FloatField("Trail lifetime", phys_toolPhys.f_trLifetime);
        GUILayout.Label("Trail Colors", EditorStyles.label);
        DisplayArray("c_trailRenderer");
        go_weaponProjectile = (GameObject)EditorGUILayout.ObjectField("Projectile", go_weaponProjectile, typeof(GameObject), true);
        GUILayout.Label("EXPLOSION", EditorStyles.boldLabel);
        ae_splosion.f_explockBack = EditorGUILayout.FloatField("Explosion Knockback", ae_splosion.f_explockBack);
        ae_splosion.f_detonationTime = EditorGUILayout.FloatField("Detonation Time", ae_splosion.f_detonationTime);
        GUILayout.Label("Explosion Particles", EditorStyles.label);
        DisplayArray("go_explarticles");
        GUILayout.Label("Elements", EditorStyles.label);
        DisplayArray("elements");
    }

    public void DisplayArray(string _variableName)
    {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty goProperty = so.FindProperty(_variableName);
        EditorGUILayout.PropertyField(goProperty, true);
        so.ApplyModifiedProperties();
    }

    private void DisplayProjectileAugments()
    {
        GUILayout.Label("Projectile Augments", EditorStyles.boldLabel);
        apro.i_shotsPerRound = EditorGUILayout.IntField("Shots per Round", apro.i_shotsPerRound);
        apro.f_gravity = EditorGUILayout.FloatField("Gravity", apro.f_gravity);
        apro.f_bulletScale = EditorGUILayout.FloatField("Bullet Scale", apro.f_bulletScale);
        pm_mat = (PhysicMaterial)EditorGUILayout.ObjectField("Physics Material", pm_mat, typeof(PhysicMaterial), true);
    }

    private void DisplayConeAugments()
    {
        GUILayout.Label("Cone Augments", EditorStyles.boldLabel);
        // Width and length of the cone
        acone.f_angle = EditorGUILayout.FloatField("Cone Width", acone.f_angle);
        acone.f_radius = EditorGUILayout.FloatField("Cone Length", acone.f_radius);
    }

    private void InitStandardAugment(Augment outputAug)
    {
        outputAug.AugmentMaterial = mat_material.name;
        try
        {
            outputAug.InitAudio(ac_useSound, ac_travelSound, ac_hitSound);
        }
        catch (InvalidCastException e) { }
        try
        {
            outputAug.InitInfo(ap_toolProperties);
        }
        catch (InvalidCastException e) { }
        try
        {
            phys_toolPhys.A_trKeys = c_trailRenderer;
            outputAug.InitPhysical(phys_toolPhys);
        }
        catch (InvalidCastException e) { }
        try
        {
            if(go_explarticles.Length > 0)
            {
                ae_splosion.go_explarticles = new string[go_explarticles.Length];
                for(int i = 0; i < go_explarticles.Length; i++)
                    ae_splosion.go_explarticles[i] = go_explarticles[i].name;
            }
            outputAug.InitExplosion(ae_splosion);
        }
        catch (InvalidCastException e) { }
        try
        {
            outputAug.AugElement = elements;
        }
        catch (InvalidCastException e) { }
    }
}
