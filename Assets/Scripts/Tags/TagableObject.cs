using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagableObject : MonoBehaviour
{

    [SerializeField] private string objectTag;

    public string GetTag()
    {
        return objectTag;
    }

    private void Start()
    {
        TagManager.x.AddTaggedObject(this);
    }

    private void OnDestroy()
    {
        TagManager.x.RemoveTaggedObject(this);
    }

}
