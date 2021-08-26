using System.Collections;
using System.Collections.Generic;


public static class Utility  
{
    // alınan dizinin elemanlarının karıştırılma metodu
    public static List<T> ShuffleList<T>(List<T> list , int seed){

        System.Random prng = new System.Random(seed);

        for (int i = 0; i < list.Count - 1; i++)
        {
            // dizinin elemanları karıştırılıyor.
            int randomIndex = prng.Next(i,list.Count);

            // basit bir şekilde elemanlar birbirinin yerine geçiyor.
            T tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
        }

        return list;
    }
}
