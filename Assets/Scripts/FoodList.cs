using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FoodList
{
    public Food head;

    public Food AddFoodBack(int maxSize = 1)
    {
        int rand = GenerateRandom(maxSize);
        Food newFood = new Food(rand);
        newFood.isAttachedToList = true;

        if (head == null)
        {
            head = newFood;
        }
        else
        {
            Food tail = GetTail(head);

            tail.prevFood = newFood;
            
            newFood.nextFood = tail;
        }
        return newFood;
    }

    public void DeleteFood(Food foodToDelete)
    {
        Food prev = null;
        if (foodToDelete != null)
            prev = foodToDelete.prevFood;

        Food next = null;
        if (foodToDelete != null)
            next = foodToDelete.nextFood;

        if (prev != null)
            prev.nextFood = next;

        if(next != null)
            next.prevFood = prev;

        if (foodToDelete == head)
            head = foodToDelete.prevFood;

        foodToDelete.prevFood = null;
        foodToDelete.nextFood = null;
    }
    public Food GetTail(Food temp)
    {
        while(temp.prevFood != null)
        {
            temp = temp.prevFood;
        }
        return temp;
    }

    public int GenerateRandom(int maxSize)
    {
        return Random.Range(0 , maxSize);
    }
}
