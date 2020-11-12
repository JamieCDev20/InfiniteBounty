using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggableObject : MonoBehaviour
{

    [SerializeField] private string objectTag;

    //what is your tag?
    public string GetTag()
    {
        return objectTag;
    }

    //Add yaself to the tag manager when you are enabled/spawned/got out of pool
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

    //Remove self from tag manager when disabled/killed/returned to pool/ all that other stuff
    private void OnDisable()
    {
        TagManager.x.RemoveTaggedObject(this);
    }

}
