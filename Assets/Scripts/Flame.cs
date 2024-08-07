using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
   [SerializeField] private BoxCollider2D _collider2D;
   public float damage;
    private bool _isActive;
    private void OnEnable()
    {
        StartCoroutine(ColliderEffect());
    }

    private void OnDisable()
    {
        StopCoroutine(ColliderEffect());
    }

    IEnumerator ColliderEffect()
    {
        _isActive = !_isActive;
        _collider2D.enabled = _isActive;
        yield return new WaitForSeconds(.1f);
        StartCoroutine(ColliderEffect());
    }
}
