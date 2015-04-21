using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Helper class for basic utility methods
/// </summary>
public static class CivGridUtility
{
    /// <summary>
    /// Converts a two-dimensional array into a single array
    /// </summary>
    /// <param name="doubleArray">The two-dimensional array array to convert into a single array</param>
    /// <param name="singleArray">The converted array</param>
    public static void ToSingleArray(CombineInstance[,] doubleArray, out CombineInstance[] singleArray)
    {
        //list to copy the values from the two-dimensional array into
        List<CombineInstance> combineList = new List<CombineInstance>();

        //cycle through all the CombineInstances and copy them into the List
        foreach (CombineInstance combine in doubleArray)
        {
            combineList.Add(combine);
        }

        //convert our List that holds all the CombineInstances into a single array
        singleArray = combineList.ToArray();
    }
}
