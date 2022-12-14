using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ObjectColor { Blue, Grey, Red, Purple }

public static class CollisionObserver
{
    private static Dictionary<ObjectColor, bool> matchingColorCollisionDict;
    private static Dictionary<ObjectColor, bool> genericCollisionDict;

    public static void SetupCollisionObserver()
    {
        CollisionObserver.matchingColorCollisionDict = new Dictionary<ObjectColor, bool>();
        CollisionObserver.matchingColorCollisionDict[ObjectColor.Blue] = false;
        CollisionObserver.matchingColorCollisionDict[ObjectColor.Grey] = false;
        CollisionObserver.matchingColorCollisionDict[ObjectColor.Red] = false;
        CollisionObserver.matchingColorCollisionDict[ObjectColor.Purple] = false;

        CollisionObserver.genericCollisionDict = new Dictionary<ObjectColor, bool>();
        CollisionObserver.genericCollisionDict[ObjectColor.Blue] = false;
        CollisionObserver.genericCollisionDict[ObjectColor.Grey] = false;
        CollisionObserver.genericCollisionDict[ObjectColor.Red] = false;
        CollisionObserver.genericCollisionDict[ObjectColor.Purple] = false;
    }

    public static void PrintDicts()
    {
        Debug.LogError("Blue\nMatching: " + CollisionObserver.matchingColorCollisionDict[ObjectColor.Blue] + " Generic: " + CollisionObserver.genericCollisionDict[ObjectColor.Blue] + 
                       "\nGrey\nMatching: " + CollisionObserver.matchingColorCollisionDict[ObjectColor.Grey] + " Generic: " + CollisionObserver.genericCollisionDict[ObjectColor.Grey] +
                       "\nRed\nMatching: " + CollisionObserver.matchingColorCollisionDict[ObjectColor.Red] + " Generic: " + CollisionObserver.genericCollisionDict[ObjectColor.Red] +
                       "\nPurple\nMatching: " + CollisionObserver.matchingColorCollisionDict[ObjectColor.Purple] + " Generic: " + CollisionObserver.genericCollisionDict[ObjectColor.Purple]);
    }

    public static bool ObjectIsCollidingWithMatchingPlatform(ObjectColor testObjectColor)
    {
        //Debug.LogError(testObjectColor + " is match colliding: " + CollisionObserver.matchingColorCollisionDict[testObjectColor]);
        return CollisionObserver.matchingColorCollisionDict[testObjectColor];
    }

    public static bool ObjectIsCollidingWithAnyPlatform(ObjectColor testObjectColor)
    {
        return CollisionObserver.genericCollisionDict[testObjectColor];
    }

    public static bool IsMatchingCollisionFree()
    {
        return (CollisionObserver.matchingColorCollisionDict[ObjectColor.Blue] == false &&
                CollisionObserver.matchingColorCollisionDict[ObjectColor.Grey] == false &&
                CollisionObserver.matchingColorCollisionDict[ObjectColor.Red] == false &&
                CollisionObserver.matchingColorCollisionDict[ObjectColor.Purple] == false);
    }

    public static bool IsCollisionFree()
    {
        return (CollisionObserver.genericCollisionDict[ObjectColor.Blue] == false &&
                CollisionObserver.genericCollisionDict[ObjectColor.Grey] == false &&
                CollisionObserver.genericCollisionDict[ObjectColor.Red] == false &&
                CollisionObserver.genericCollisionDict[ObjectColor.Purple] == false);
    }

    public static void NotifyCollisionChange(ObjectColor platformColor, ObjectColor playerSegmentColor, 
                                             bool currentlyColliding, bool isGeneric = false)
    {        
        if (currentlyColliding == false)
        {
            CollisionObserver.matchingColorCollisionDict[playerSegmentColor] = false;
            CollisionObserver.genericCollisionDict[playerSegmentColor] = false;
        }
        else if (isGeneric == false && platformColor == playerSegmentColor)
        {
            CollisionObserver.matchingColorCollisionDict[playerSegmentColor] = true;
            CollisionObserver.genericCollisionDict[playerSegmentColor] = true;
        }
        else
        {
            CollisionObserver.matchingColorCollisionDict[playerSegmentColor] = false;
            CollisionObserver.genericCollisionDict[playerSegmentColor] = true;
        }
    }    
}
