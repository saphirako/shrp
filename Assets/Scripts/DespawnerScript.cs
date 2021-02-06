/*
    NAME:       DespawnerScript.cs
    AUTHOR:     saphirako
    DATE:       6 NOV 2018

    DESCRIPTION:
    Mercilessly remove the GameObject attached to any collider that touches the collider on
    this script's GameObject

    LEGAL:
    Copyright © 2020 Saphirako
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnerScript : MonoBehaviour {
    public void OnTriggerEnter2D (Collider2D toBeDestroyed) {
        if (toBeDestroyed.attachedRigidbody.velocity.y < 0) {
            try {
                Destroy (toBeDestroyed.transform.parent.gameObject);
            } catch {
                Destroy (toBeDestroyed.transform.gameObject);   // This will happen when the active Player hits the Despawner
            }
        }
    }
}