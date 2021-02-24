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

    public static T[] Swap<T>(T[] _arrayToSwap, int _firstObj, int _secondObj)
    {
        T temp = _arrayToSwap[_secondObj];
        _arrayToSwap[_secondObj] = _arrayToSwap[_firstObj];
        _arrayToSwap[_firstObj] = temp;
        return _arrayToSwap;
    }

    public static T[] Swap<T>(T[] _arrayToSwap, T _firstObj, T _secondObj)
    {
        int _first = -1;
        int _second = -1;
        for(int i = 0; i < _arrayToSwap.Length; i++)
        {
            if (_arrayToSwap[i].Equals(_firstObj))
                _first = i;
            if (_arrayToSwap[i].Equals(_secondObj))
                _second = i;
        }
        if (_first == -1 || _second == -1)
            return _arrayToSwap;
        return Swap(_arrayToSwap, _first, _second);
    }
}
