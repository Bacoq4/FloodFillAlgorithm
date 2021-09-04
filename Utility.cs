using System.Collections;
using System.Collections.Generic;


public static class Utility  
{
    public static List<T> ShuffleList<T>(List<T> list , int seed){

        System.Random prng = new System.Random(seed);

        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = prng.Next(i,list.Count);

            T tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
        }

        return list;
    }
}
