using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class AugmentWindow : EditorWindow
{
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

    #endregion

    #region AugmentGO physicals

    Material mat_material;
    Mesh m_mesh;

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
        AugmentWindow wind = GetWindow<AugmentWindow>();
        wind.position = new Rect(0, 0, f_windWidth, f_windHeight);
    }

    private void OnGUI()
    {

        GUI.contentColor = Color.white;
        GUI.color = Color.white;
        GUI.backgroundColor = Color.grey;
        
        // Main GUI loop where things need to update
        GUILayout.Label("Augment Type", EditorStyles.boldLabel);
        at_type = (AugmentType)EditorGUILayout.EnumPopup("", at_type);
        EditorGUILayout.Space();
        GUILayout.Label("Augment Name", EditorStyles.label);
        s_augName = EditorGUILayout.TextArea(s_augName);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DisplayGameObjectArgs();
        EditorGUILayout.Space();
        if (i_dropDownIndex != 0)
        {
            b_displayBaseAugments = EditorGUILayout.Toggle("Show Base Augments", b_displayBaseAugments);
        }

        ShowAugmentToCreate();
        EditorGUILayout.Space();
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

            Augment outputAug = new Augment();
            // Save variables time!
            switch (at_type)
            {
                case AugmentType.standard:
                    InitStandardAugment(outputAug);
                    break;
                case AugmentType.projectile:
                    ProjectileAugment pOutput = new ProjectileAugment();
                    InitStandardAugment(pOutput);
                    try
                    {
                        pOutput.InitProjectile(apro);
                        outputAug = pOutput;
                    }catch(InvalidCastException e) { }
                    break;
                case AugmentType.cone:
                    ConeAugment cOutput = new ConeAugment();
                    InitStandardAugment(cOutput);
                    try
                    {
                        cOutput.InitCone(acone);
                        outputAug = cOutput;
                    }catch (InvalidCastException e) { }
                    break;
            }
            outputAug.at_type = at_type;
            augmentData = EditorJsonUtility.ToJson(outputAug);
            File.AppendAllText(path + "AugmentData.json", augmentData + "\n");
            tr.Close();
            Debug.Log(String.Format("{0} Created!", s_augName));
            //AugmentCreator.CreateAugment(augmentData);
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
        m_mesh = (Mesh)EditorGUILayout.ObjectField(m_mesh, typeof(Mesh), true);
        mat_material = (Material)EditorGUILayout.ObjectField(mat_material, typeof(Material), true);
    }

    /// <summary>
    /// Display each editable parameter with its lable
    /// </summary>
    private void DisplayBaseAugments()
    {
        GUILayout.Label("Base Augments", EditorStyles.boldLabel);
        GUILayout.Label("Physical Properties", EditorStyles.boldLabel);
        GUILayout.Label("Weapon Speed", EditorStyles.label);
        ap_toolProperties.f_speed = EditorGUILayout.FloatField(ap_toolProperties.f_speed);
        GUILayout.Label("Recoil", EditorStyles.label);
        ap_toolProperties.f_recoil = EditorGUILayout.FloatField(ap_toolProperties.f_recoil);
        GUILayout.Label("Enemy Damage", EditorStyles.label);
        ap_toolProperties.i_damage = EditorGUILayout.IntField(ap_toolProperties.i_damage);
        GUILayout.Label("Lode Damage", EditorStyles.label);
        ap_toolProperties.i_lodeDamage = EditorGUILayout.IntField(ap_toolProperties.i_lodeDamage);
        GUILayout.Label("Knockback", EditorStyles.label);
        ap_toolProperties.f_knockback = EditorGUILayout.FloatField(ap_toolProperties.f_knockback);
        GUILayout.Label("Weight", EditorStyles.label);
        ap_toolProperties.f_weight = EditorGUILayout.FloatField(ap_toolProperties.f_weight);
        GUILayout.Label("Heatsink", EditorStyles.label);
        ap_toolProperties.f_heatsink = EditorGUILayout.FloatField(ap_toolProperties.f_heatsink);
        GUILayout.Label("Energy Gauge", EditorStyles.label);
        ap_toolProperties.f_energyGauge = EditorGUILayout.FloatField(ap_toolProperties.f_energyGauge);
        GUILayout.Label("Audio Attributes", EditorStyles.boldLabel);
        GUILayout.Label("Usage Sounds", EditorStyles.label);
        DisplayArray("ac_useSound");
        GUILayout.Label("Travel Sounds", EditorStyles.label);
        DisplayArray("ac_travelSound");
        GUILayout.Label("Hit Sounds", EditorStyles.label);
        DisplayArray("ac_hitSound");
        GUILayout.Label("Trail and Particles", EditorStyles.boldLabel);
        GUILayout.Label("Trail Width", EditorStyles.label);
        phys_toolPhys.f_trWidth = EditorGUILayout.FloatField(phys_toolPhys.f_trWidth);
        GUILayout.Label("Trail Lifetime", EditorStyles.label);
        phys_toolPhys.f_trLifetime = EditorGUILayout.FloatField(phys_toolPhys.f_trLifetime);
        GUILayout.Label("Trail Colors", EditorStyles.label);
        DisplayArray("c_trailRenderer");
        GUILayout.Label("Projectile", EditorStyles.label);
        go_weaponProjectile = (GameObject)EditorGUILayout.ObjectField(go_weaponProjectile, typeof(GameObject), true);
        GUILayout.Label("EXPLOSION", EditorStyles.boldLabel);
        GUILayout.Label("Explosion Knockback", EditorStyles.label);
        ae_splosion.f_explockBack = EditorGUILayout.FloatField(ae_splosion.f_explockBack);
        GUILayout.Label("Detonation Time", EditorStyles.label);
        ae_splosion.f_detonationTime = EditorGUILayout.FloatField(ae_splosion.f_detonationTime);
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
        GUILayout.Label("Shots Per Round", EditorStyles.label);
        apro.i_shotsPerRound = EditorGUILayout.IntField(apro.i_shotsPerRound);
        GUILayout.Label("Gravity", EditorStyles.label);
        apro.f_gravity = EditorGUILayout.FloatField(apro.f_gravity);
        apro.f_bulletScale = EditorGUILayout.FloatField("Bullet Scale", apro.f_bulletScale);
        GUILayout.Label("Physics Material", EditorStyles.label);
        pm_mat = (PhysicMaterial)EditorGUILayout.ObjectField(pm_mat, typeof(PhysicMaterial), true);
    }

    private void DisplayConeAugments()
    {
        GUILayout.Label("Cone Augments", EditorStyles.boldLabel);
        // Width and length of the cone
        GUILayout.Label("Cone", EditorStyles.label);
        acone.f_angle = EditorGUILayout.FloatField(acone.f_angle);
        GUILayout.Label("Length", EditorStyles.label);
        acone.f_radius = EditorGUILayout.FloatField(acone.f_radius);
    }

    private void InitStandardAugment(Augment outputAug)
    {
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
            ae_splosion.go_explarticles = go_explarticles;
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
