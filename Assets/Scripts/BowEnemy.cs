using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowEnemy : Enemy
{
    [SerializeField] private float maxDistance;
    [SerializeField] private float followDistance;
    public GameObject arrowInstance;
    [HideInInspector] public bool isAttack;
    private Bow2 _bow2;

    protected override void Awake()
    {
        base.Awake();
        _bow2 = transform.GetChild(0).GetComponent<Bow2>();
    }

    protected override void FollowPlayer()
    {
        float distance = Vector2.Distance(Player.transform.position, transform.position);
        if (followDistance < distance)
            base.FollowPlayer();
        else
            Rb.velocity = Vector2.zero;
        if (distance < maxDistance && !isAttack && speed != 0)
            Attack();
    }

    void Attack()
    {
        isAttack = true;
        _bow2.Attack();
    }
}