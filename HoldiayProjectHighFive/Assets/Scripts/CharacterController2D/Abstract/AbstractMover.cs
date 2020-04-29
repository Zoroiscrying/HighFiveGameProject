using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    //velocity and if use gravity
    Vector2 Velocity { get; set; }
    bool UseGravity { get; set; }
    //Colliding Relevant
    LayerMask ColliderLayerMask { get; set; }
    event Action<Collider2D> OnColliderEnter;
    event Action<RaycastHit2D, TouchDir> OnCollidedEvent;
    //Trigger Relevant
    LayerMask TriggerLayerMast { get; set; }
    event Action<Collider> OnTriggerEnter;
    event Action<RaycastHit2D, TouchDir> OnTriggeredEvent;
}

public enum TouchDir
{
    top,
    bottom,
    left,
    right
}
