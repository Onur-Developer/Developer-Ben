using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected float DestoryTiming = 5f;
    [SerializeField] private bool isLaser;
    public float damage;

    private void Start()
    {
        if (!isLaser)
            Destroy(gameObject, DestoryTiming);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            if (!isLaser)
                Destroy(gameObject);
        }
    }
}