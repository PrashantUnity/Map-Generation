using System;
using System.Collections;
using System.Collections.Generic;

public class Utility 
{
    public static List<T> SuffeledArray<T>(List<T> array,int seed)
    {
        var rand = new Random(seed);
        for (int i = 0; i < array.Count; i++)
        {
            var randIndex = rand.Next(i,array.Count);

            // swaping 
            var tempItem = array[randIndex];
            array[randIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }
    public static T[] SuffeledArray<T>(T[] array, int seed)
    {
        var rand = new Random(seed);
        for (int i = 0; i < array.Length; i++)
        {
            var randIndex = rand.Next(i, array.Length);

            // swaping 
            var tempItem = array[randIndex];
            array[randIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }
}
