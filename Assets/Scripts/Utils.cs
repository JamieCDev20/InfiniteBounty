﻿using System.Collections;
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
        if (ArrayIsNullOrZero<T>(t_arrayToResize))
        {
            t_arrayToResize = new T[1];
            t_arrayToResize[0] = t_objectToAdd;
        }
        else if (t_arrayToResize.Length > 0)
        {
            int newSize = t_arrayToResize.Length + 1;
            T[] temp = new T[newSize];
            for (int i = 0; i < t_arrayToResize.Length; i++)
                temp[i] = t_arrayToResize[i];
            temp[newSize - 1] = t_objectToAdd;
            t_arrayToResize = temp;
        }
        return t_arrayToResize;
    }

    /// <summary>
    /// Reduce the number of items in the array. by a given amount. If no size is provided, 1 item is removed by default.
    /// </summary>
    /// <typeparam name="T">Type of array.</typeparam>
    /// <param name="_arrayToReduce">Array to be reduced.</param>
    /// <returns>The newly reduced array.</returns>
    public static T[] ReduceArraySize<T>(T[] _arrayToReduce)
    {
        return ReduceArraySize<T>(_arrayToReduce, 1);
    }
    /// <summary>
    /// Reduce the number of items in the array by a given amount. If no size is provided, 1 item is removed by default.
    /// </summary>
    /// <typeparam name="T">Type of array</typeparam>
    /// <param name="_arrayToReduce">Array to be reduced.</param>
    /// <param name="_amountToReduce">Amount to reduce the array by.</param>
    /// <returns>The newly reduced array.</returns>
    public static T[] ReduceArraySize<T>(T[] _arrayToReduce, int _amountToReduce)
    {
        if (ArrayIsNullOrZero<T>(_arrayToReduce))
            return null;
        T[] tmp = new T[_arrayToReduce.Length - _amountToReduce];
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i] = _arrayToReduce[i];
        }
        _arrayToReduce = tmp;
        return _arrayToReduce;
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
        if (ArrayIsNullOrZero(a))
            return b;
        if (ArrayIsNullOrZero(b))
            return a;
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

    public static bool ArrayIsNullOrZero<T>(T[] _arrayToCheck)
    {
        if (_arrayToCheck == null)
            return true;
        if (_arrayToCheck.Length == 0)
            return true;
        return false;
    }

    public static T[] Remove<T>(T[] _arrayToRemove, T _itemToRemove)
    {
        if (ArrayIsNullOrZero<T>(_arrayToRemove))
            return null;
        _arrayToRemove = Swap<T>(_arrayToRemove, _itemToRemove, _arrayToRemove[_arrayToRemove.Length -1]);
        return ReduceArraySize<T>(_arrayToRemove);
    }

    //public static T[] OrderedRemove<T>(T[] _arrayToRemove, T _itemToRemove)
    //{
    //    for(int i = 0; i < _arrayToRemove.Length; i++)
    //    {
    //        if(_arrayToRemove[i].ToString() == _itemToRemove.ToString())
    //        {
    //            Debug.Log(_arrayToRemove[i].ToString() + " " + _itemToRemove.ToString());
    //            return OrderedRemove(_arrayToRemove, i);
    //        }
    //    }
    //    return _arrayToRemove;
    //}

    public static T[] OrderedRemove<T>(T[] _arrayToRemove, int _itemToRemove)
    {
        if (ArrayIsNullOrZero<T>(_arrayToRemove))
            return null;

        if (_arrayToRemove.Length == 1)
            return new T[0];
        if(_itemToRemove != -1)
        {
            Debug.Log(string.Format("Length: {0} | Index: {1}", _arrayToRemove[_itemToRemove], _itemToRemove));
        }
        else
            Debug.Log(string.Format("Length: {0} | Index: {1}", _arrayToRemove.Length, _itemToRemove));

        bool skipped = false;
        T[] newArray = new T[_arrayToRemove.Length - 1];
        for (int i = 0; i < _arrayToRemove.Length; i++)
        {
            if (i == _itemToRemove)
            {
                skipped = true;
                continue;
            }
            // If this errors it means we've not found the item we wish to remove...
            newArray[skipped ? i - 1 : i] = _arrayToRemove[i];
        }
        return newArray;
    }

    public static T[] RemoveDuplicates<T>(T[] _arrayToRemove)
    {
        List<T> temp = new List<T>();
        for (int i = 0; i < _arrayToRemove.Length; i++)
        {
            if (!temp.Contains(_arrayToRemove[i]))
                temp.Add(_arrayToRemove[i]);
        }
        return temp.ToArray();
    }

    public static Vector3 RandomUnitVector()
    {
        return new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
    }

}
