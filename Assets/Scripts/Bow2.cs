using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow2 : MonoBehaviour
{
    private GameObject arrowInstance;
    private Animator anim;
    private Transform player;
    private BowEnemy bw;
    [SerializeField] private Transform point;

    private void Awake()
    {
        arrowInstance = transform.parent.GetComponent<BowEnemy>().arrowInstance;
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        bw = transform.parent.GetComponent<BowEnemy>();
    }

    private void Update()
    {
        BowRotate();
    }

    void BowRotate()
    {
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = rotation;
    }

   public void Shoot()
    {
        Vector2 dir = player.position - transform.position;
        dir.Normalize();
        GameObject arrow = Instantiate(arrowInstance,point.transform.position,Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * 500);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        arrow.transform.rotation = rotation;
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    public void Reload()
    {
        bw.isAttack = false;
    }
}
