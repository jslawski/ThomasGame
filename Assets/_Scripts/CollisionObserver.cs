using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ObjectColor { Blue, Grey, Red, Purple }

public static class CollisionObserver
{
    private static Dictionary<ObjectColor, bool> colorCollisionDict;

    private static void Awake()
    {
        CollisionObserver.colorCollisionDict = new Dictionary<ObjectColor, bool>();
    }

    public static bool ObjectIsCollidingWithMatchingPlatform(ObjectColor testObjectColor)
    {
        Debug.LogError(testObjectColor + " is match colliding: " + CollisionObserver.colorCollisionDict[testObjectColor]);

        return CollisionObserver.colorCollisionDict[testObjectColor];
    }

    public static void NotifyCollisionChange(ObjectColor platformColor, ObjectColor playerSegmentColor, bool currentlyColliding)
    {
        if (CollisionObserver.colorCollisionDict == null)
        {
            CollisionObserver.colorCollisionDict = new Dictionary<ObjectColor, bool>();
            CollisionObserver.colorCollisionDict[ObjectColor.Blue] = false;
            CollisionObserver.colorCollisionDict[ObjectColor.Grey] = false;
            CollisionObserver.colorCollisionDict[ObjectColor.Red] = false;
            CollisionObserver.colorCollisionDict[ObjectColor.Purple] = false;
        }
        
        if (currentlyColliding == false)
        {
            Debug.LogError("Not currently colliding");
            CollisionObserver.colorCollisionDict[playerSegmentColor] = false;
        }
        else if (platformColor == playerSegmentColor)
        {
            Debug.LogError("Match detected");
            CollisionObserver.colorCollisionDict[playerSegmentColor] = true;
        }
        else
        {
            Debug.LogError("Colliding color doesn't match");
            CollisionObserver.colorCollisionDict[playerSegmentColor] = false;
        }
    }
}
