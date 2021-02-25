using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardCamController : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}