﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{

    public static TagManager x;

    //Variables
    #region Serialised


    #endregion

    #region Private

    private Dictionary<string, HashSet<GameObject>> taggedObjects = new Dictionary<string, HashSet<GameObject>>() { { "All", new HashSet<GameObject>() } };

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        if (x != null)
            Destroy(gameObject);
        else
            x = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //GetAllTaggedObjects();
    }

    public void Reset()
    {

    }

    #endregion

    #region Private Voids

    private void GetAllTaggedObjects()
    {

        foreach (TaggableObject t in FindObjectsOfType<TaggableObject>())
        {
            AddTaggedObject(t);
        }

    }

    #endregion

    #region Public Voids

    public void AddTaggedObject(TaggableObject _t)
    {

        foreach (string tag in _t.GetTags())
        {
            if (taggedObjects.ContainsKey(tag))
            {
                taggedObjects[tag].Add(_t.gameObject);
            }
            else
            {
                taggedObjects.Add(tag, new HashSet<GameObject>() { _t.gameObject });
            }
        }

    }

    public void RemoveTaggedObject(TaggableObject _t)
    {
        foreach (string tag in _t.GetTags())
        {
            if (taggedObjects.ContainsKey(tag))
            {
                taggedObjects[tag].Remove(_t.gameObject);
            }
        }
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    public HashSet<GameObject> GetTagSet(string _tag)
    {
        if(taggedObjects.ContainsKey(_tag))
            return taggedObjects[_tag];
        return null;
    }

    #endregion

}
