using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNugCreator : MonoBehaviour
{
    [SerializeField] GameObject nug;

    public void CreateNug()
    {
        Instantiate(nug);
        nug.transform.position = transform.position;
    }
}
