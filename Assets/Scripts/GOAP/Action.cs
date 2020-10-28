using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GOAP Action", menuName = "GOAP/New Action")]
public class Action : ScriptableObject
{

    public Condition[] preconditions;
    [Space]
    public Condition[] postconditions;

    [HideInInspector]
    public List<string> signatures = new List<string>();

    public void SetUp()
    {
        signatures = new List<string>();
        for (int i = 0; i < preconditions.Length; i++)
        {
            int counterpartIndex = PrePostCounterpart(preconditions[i]);
            if(counterpartIndex > -1)
            {
                switch (preconditions[i].value)
                {
                    case true:
                        signatures.Add(string.Format("{0}-{1}-{2}", (int)preconditions[i].prefix, (postconditions[counterpartIndex].value ? 0 : 2), preconditions[i].fill));
                        break;
                    case false:
                        signatures.Add(string.Format("{0}-{1}-{2}", (int)preconditions[i].prefix, (postconditions[counterpartIndex].value ? 1 : 0), preconditions[i].fill));
                        break;
                }
            }
        }
    }

    private int PrePostCounterpart(Condition con)
    {
        int index = -1;

        for (int i = 0; i < postconditions.Length; i++)
        {
            if (postconditions[i].prefix == con.prefix)
                if (postconditions[i].fill == con.fill)
                    return i;
        }

        return index;
    }

    public static bool operator >(Action a, Action b)
    {
        return Array.TrueForAll(b.preconditions, v => a.postconditions.Contains(v));
    }
    public static bool operator <(Action a, Action b)
    {
        return Array.TrueForAll(a.preconditions, v => b.postconditions.Contains(v));
    }

    public static bool operator >(Condition[] a, Action b)
    {
        return Array.TrueForAll(b.preconditions, v => a.Contains(v));
    }
    public static bool operator <(Condition[] a, Action b)
    {
        return Array.TrueForAll(a, v => b.postconditions.Contains(v));
    }

    public static bool operator >(Action a, Condition[] b)
    {
        return Array.TrueForAll(b, v => a.postconditions.Contains(v));
    }
    public static bool operator <(Action a, Condition[] b)
    {
        return Array.TrueForAll(a.preconditions, v => b.Contains(v));
    }

}

[System.Serializable]
public struct Condition
{
    public Prefix prefix;
    public string fill;
    public bool value;

    /// <summary>
    /// Check how many things in a are not in b. eg: goal, postcons
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int Compare(Condition[] a, Condition[] b)
    {
        int v = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (!b.Contains(a[i]))
                v++;
        }
        return v;
    }

}

public enum Prefix
{
    has,
    pathTo,
    canSee,
    nextTo
}