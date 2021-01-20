using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    /// <summary>
    /// It... It adds an object to an array... I don't know what more you want from me...
    /// </summary>
    /// <typeparam name="T">Type of the arrayyyy</typeparam>
    /// <param name="t_arrayToResize">The actual array</param>
    /// <param name="t_objectToAdd">The object you want to add to the actual array.</param>
    /// <returns></returns>
    public static T[] AddToArray<T>(T[] t_arrayToResize, T t_objectToAdd)
    {
        if(t_arrayToResize != null)
        {
            if(t_arrayToResize.Length > 0)
            {
                int newSize = t_arrayToResize.Length + 1;
                Debug.Log(newSize);
                T[] temp = new T[newSize];
                for (int i = 0; i < t_arrayToResize.Length; i++)
                    temp[i] = t_arrayToResize[i];
                temp[newSize - 1] = t_objectToAdd;
                t_arrayToResize = temp;
            }
            else
            {
                t_arrayToResize = new T[1];
                t_arrayToResize[0] = t_objectToAdd;
            }
        }
        else
        {
            t_arrayToResize = new T[1];
            t_arrayToResize[0] = t_objectToAdd;
        }
        return t_arrayToResize;
    }

    /// <summary>
    /// Guess what this does, dumbass...
    /// </summary>
    /// <typeparam name="T">Type of array.</typeparam>
    /// <param name="a">Array one</param>
    /// <param name="b">Array two</param>
    /// <returns>a new array with both values</returns>
    public static T[] CombineArrays<T>(T[] a, T[] b)
    {
        T[] output = new T[a.Length + b.Length];
        for (int i = 0; i < a.Length; i++)
        {
            output[i] = a[i];
        }
        for (int j = 0; j < b.Length; j++)
        {
            output[a.Length + j] = b[j];
        }
        return output;
    }
}
