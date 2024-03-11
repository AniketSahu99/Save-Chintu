using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food
{
    public int foodNumber;
    public int headingPathNumber;
    public Food nextFood;
    public Food prevFood;
    public bool isAttachedToList;


    public Food(int fN)
    {
        headingPathNumber = 1;
        foodNumber = fN;
        nextFood = null;
        prevFood = null;
        isAttachedToList = false;
    }
}
