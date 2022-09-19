using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredPlayerSegment : MonoBehaviour
{
    public ObjectColor playerSegmentColor = ObjectColor.None;
    /*
    private Collider segmentCollider;

    private void Awake()
    {
        this.segmentCollider = GetComponent<Collider>();
    }

    private void LateUpdate()
    {
        Vector3 colliderCenter = this.segmentCollider.bounds.center;
        float xExtent = this.segmentCollider.bounds.extents.x;
        //this.segmentCollider.bounds.

        Vector3 raycastVectorLeft = new Vector3(colliderCenter.x - xExtent, colliderCenter.y, 0.0f);
        Vector3 raycastVectorRight = new Vector3(colliderCenter.x + xExtent, colliderCenter.y, 0.0f);

        Debug.DrawRay(raycastVectorLeft, -this.gameObject.transform.up, Color.green);
        Debug.DrawRay(raycastVectorRight, -this.gameObject.transform.up, Color.green);
    }
    */
}
