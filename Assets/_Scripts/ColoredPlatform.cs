using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredPlatform : MonoBehaviour
{
    public ObjectColor platformColor;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            ObjectColor segmentColor = collision.collider.gameObject.GetComponent<ColoredPlayerSegment>().playerSegmentColor;
            CollisionObserver.NotifyCollisionChange(this.platformColor, segmentColor, true);

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (CollisionObserver.IsMatchingCollisionFree())
            {
                playerRb.velocity = new Vector3(0.0f, playerRb.velocity.y, 0.0f);
            }
            else
            {
                playerRb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
        
        //Debug.LogError(this.gameObject.name + " collides with " +collision.collider.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().AttemptNudgePlayer();

            ObjectColor segmentColor = collision.collider.gameObject.GetComponent<ColoredPlayerSegment>().playerSegmentColor;
            CollisionObserver.NotifyCollisionChange(this.platformColor, segmentColor, false);
        }

        //Debug.LogError(this.gameObject.name + " EXITS " + collision.collider.gameObject.name);
    }
}
