using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggableObject : MonoBehaviour
{

    [SerializeField] private string objectTag;

    public string GetTag()
    {
        return objectTag;
    }

    private void OnEnable()
    {
        try
        {
            TagManager.x.AddTaggedObject(this);

        }
        catch
        {
            FindObjectOfType<TagManager>().AddTaggedObject(this);
        }
    }

    private void OnDisable()
    {
        TagManager.x.RemoveTaggedObject(this);
    }

}
