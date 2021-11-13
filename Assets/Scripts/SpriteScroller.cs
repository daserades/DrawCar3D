﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroller : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] new Vector3 temp;

    Vector2 offset;
    Material material;

    Transform target;

    private void OnEnable()
    {
        DrawnBody.newBodyCreated += DrawnBody_newBodyCreated;
    }

    private void OnDisable()
    {
        DrawnBody.newBodyCreated -= DrawnBody_newBodyCreated;
    }

    private void DrawnBody_newBodyCreated(DrawnBody NewDrawnBody)
    {
        target = NewDrawnBody.transform;
    }

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;

    }

    private void Update()
    {
        // parallax effect
        if (target != null)
        {
            offset = target.GetComponent<DrawnBody>().GetDrawnBodyVelocity() * Time.deltaTime * moveSpeed;
            material.mainTextureOffset += offset;
        }

        // TODO move BG dynamically
    }
}