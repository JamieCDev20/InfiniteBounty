using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlaymodeStateFix
{
    
    static PlaymodeStateFix()
    {
        EditorApplication.playModeStateChanged += ChangedPlayMode;
    }

    private static void ChangedPlayMode(PlayModeStateChange obj)
    {

        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                EditorSceneManager.OpenScene(EditorSceneManager.GetSceneByBuildIndex(0).path);
                break;
            case PlayModeStateChange.ExitingEditMode:
                EditorSceneManager.SaveOpenScenes();
                break;
            case PlayModeStateChange.EnteredPlayMode:
                break;
            case PlayModeStateChange.ExitingPlayMode:
                UniversalOverlord.x?.CantLoadScene();
                break;
            default:
                break;
        }
    }


}
