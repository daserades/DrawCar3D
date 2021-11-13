using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCollider : MonoBehaviour
{
    void Awake()
    {
        if (!GetComponent<Rigidbody>())
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        if (!GetComponent<MeshCollider>())
        {
            var meshCollider = gameObject.AddComponent<MeshCollider>();
            var mesh = GetComponent<MeshFilter>().mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
}
