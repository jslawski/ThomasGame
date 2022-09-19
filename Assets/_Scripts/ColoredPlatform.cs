using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredPlatform : MonoBehaviour
{
    public ObjectColor platformColor;

    private MeshRenderer platformRenderer;
    
    private AudioObserver audioObserver;

    [SerializeField]
    private bool isGeneric = false;

    private void Awake()
    {
        this.platformRenderer = GetComponent<MeshRenderer>();

        this.audioObserver = GameObject.Find("AudioObserver").GetComponent<AudioObserver>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            ObjectColor segmentColor = collision.collider.gameObject.GetComponent<ColoredPlayerSegment>().playerSegmentColor;

            CollisionObserver.NotifyCollisionChange(this.platformColor, segmentColor, true, this.isGeneric);

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (CollisionObserver.IsMatchingCollisionFree())
            {
                playerRb.velocity = new Vector3(0.0f, playerRb.velocity.y, 0.0f);
            }
            else
            {
                playerRb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }

            ColoredPlayerSegment playerSegment = collision.collider.gameObject.GetComponent<ColoredPlayerSegment>();

            Material activeMaterial = Resources.Load<Material>("Materials/" + playerSegment.playerSegmentColor.ToString() + "/active");

            if (this.isGeneric == false && this.platformColor == playerSegment.playerSegmentColor)
            {
                playerSegment.segmentRenderer.material = activeMaterial;
                this.platformRenderer.material = activeMaterial;

                this.audioObserver.NotifyAudioTrigger(AudioTrigger.Attach);
            }
            else
            {
                this.audioObserver.NotifyAudioTrigger(AudioTrigger.Collide);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().AttemptNudgePlayer();

            ObjectColor segmentColor = collision.collider.gameObject.GetComponent<ColoredPlayerSegment>().playerSegmentColor;
            CollisionObserver.NotifyCollisionChange(this.platformColor, segmentColor, false);

            ColoredPlayerSegment playerSegment = collision.collider.gameObject.GetComponent<ColoredPlayerSegment>();

            Material inactiveMaterial = Resources.Load<Material>("Materials/" + playerSegment.playerSegmentColor.ToString() + "/inactive");


            if (this.isGeneric == false && this.platformColor == playerSegment.playerSegmentColor)
            {
                playerSegment.segmentRenderer.material = inactiveMaterial;
                this.platformRenderer.material = inactiveMaterial;

                this.audioObserver.NotifyAudioTrigger(AudioTrigger.Detach);
            }
        }
    }
}
