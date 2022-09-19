using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredPlatform : MonoBehaviour
{
    public ObjectColor platformColor;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            ObjectColor segmentColor = collision.collider.gameObject.GetComponent<ColoredPlayerSegment>().playerSegmentColor;
            CollisionObserver.NotifyCollisionChange(this.platformColor, segmentColor, true);
        }
        
        Debug.LogError(this.gameObject.name + " collides with " +collision.collider.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            ObjectColor segmentColor = collision.collider.gameObject.GetComponent<ColoredPlayerSegment>().playerSegmentColor;
            CollisionObserver.NotifyCollisionChange(this.platformColor, segmentColor, false);
        }

        Debug.LogError(this.gameObject.name + " EXITS " + collision.collider.gameObject.name);
    }
}
