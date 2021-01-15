using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    
    public static T[] ResizeArray<T>(T[] t_arrayToResize, T t_objectToAdd)
    {
        int newSize = t_arrayToResize.Length + 1;
        T[] temp = new T[newSize];
        for (int i = 0; i < t_arrayToResize.Length; i++)
            temp[i] = t_arrayToResize[i];
        temp[newSize] = t_objectToAdd;
        t_arrayToResize = temp;
        return t_arrayToResize;
    }
}
