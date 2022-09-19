using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ObjectColor { Red, Blue, Purple, Grey, None }

public class CollisionObserver : MonoBehaviour
{
    public Dictionary<ObjectColor, bool> colorCollisionDict;

    private void Awake()
    {
        this.colorCollisionDict = new Dictionary<ObjectColor, bool>();
    }

    public void NotifyCollisionChange(ObjectColor platformColor, ObjectColor playerSegmentColor, bool currentlyColliding)
    {
        if (currentlyColliding == false)
        {
            this.colorCollisionDict[playerSegmentColor] = false;
        }
        else if (platformColor == playerSegmentColor)
        {
            this.colorCollisionDict[playerSegmentColor] = true;
        }
        else
        {
            this.colorCollisionDict[playerSegmentColor] = false;
        }
    }
}
