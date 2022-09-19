using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredPlayerSegment : MonoBehaviour
{
    public ObjectColor playerSegmentColor;
    [HideInInspector]
    public MeshRenderer segmentRenderer;

    private void Awake()
    {
        this.segmentRenderer = GetComponent<MeshRenderer>();
    }
}
