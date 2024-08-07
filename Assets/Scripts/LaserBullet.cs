using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBullet : Bullet
{
    private byte _touchingEnemy;
    private Rigidbody2D rb;

    [SerializeField] float bombRadius;

    [SerializeField] private LayerMask layerMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    Vector2 FindNewEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, bombRadius, layerMask);
        Vector2 closerEnemy = Vector2.zero;
        float oldDistance = 50;
        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (oldDistance > distance)
            {
                oldDistance = distance;
                closerEnemy = enemy.transform.position;
            }
        }

        return closerEnemy;
    }

    void GoNewEnemy(Vector2 pos)
    {
        if(pos==Vector2.zero)
            Destroy(gameObject);
        Vector2 goPos = new Vector2(pos.x-transform.position.x, pos.y - transform.position.y);
        goPos.Normalize();
        rb.velocity=Vector2.zero;
        rb.AddForce(goPos*800f);
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (_touchingEnemy >= 2 || col.CompareTag("Flag"))
            base.OnTriggerEnter2D(col);
        else if (col.CompareTag("Enemy"))
        {
            _touchingEnemy++;
            col.gameObject.layer = 1;
            Vector2 pos = FindNewEnemy();
            GoNewEnemy(pos);
        }
    }
}