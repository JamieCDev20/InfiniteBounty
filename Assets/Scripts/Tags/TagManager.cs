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
        x = this;
    }

    private void Start()
    {
        //GetAllTaggedObjects();
    }

    #endregion

    #region Private Voids

    private void GetAllTaggedObjects()
    {

        foreach (TagableObject t in FindObjectsOfType<TagableObject>())
        {
            AddTaggedObject(t);
        }

    }

    #endregion

    #region Public Voids

    public void AddTaggedObject(TagableObject _t)
    {
        if (taggedObjects.ContainsKey(_t.GetTag()))
        {
            taggedObjects[_t.GetTag()].Add(_t.gameObject);
        }
        else
        {
            taggedObjects.Add(_t.GetTag(), new HashSet<GameObject>() { _t.gameObject });
        }
    }

    public void RemoveTaggedObject(TagableObject _t)
    {
        if (taggedObjects.ContainsKey(_t.GetTag()))
        {
            taggedObjects[_t.GetTag()].Remove(_t.gameObject);
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
