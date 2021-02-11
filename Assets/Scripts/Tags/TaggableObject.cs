using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggableObject : MonoBehaviour
{

    [SerializeField] private string[] objectTags;

    //what is your tag?
    public string[] GetTags()
    {
        return objectTags;
    }

    //Add yaself to the tag manager when you are enabled/spawned/got out of pool
    private void OnEnable()
    {
        for (int i = 0; i < objectTags.Length; i++)
        {

        }
    }

    //Remove self from tag manager when disabled/killed/returned to pool/ all that other stuff
    private void OnDisable()
    {
        TagManager.x.RemoveTaggedObject(this);
    }

}
