using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordEnemy : Enemy
{
    [SerializeField] private Transform _sword;
    [SerializeField] private GameObject swordInstance;
    [SerializeField] private Animator swordAnim;
    [SerializeField] private float maxDistance;
    [SerializeField] private float reloadTimer;
    private bool _isAttack;

    protected override void FollowPlayer()
    {
        base.FollowPlayer();
        float distance = Vector2.Distance(Player.transform.position, transform.position);
        if (distance < maxDistance && !_isAttack && speed != 0)
            Attack();
    }

    void Attack()
    {
        _isAttack = true;
        SwordRotate();
    }

    void SwordRotate()
    {
        Vector3 direction = Player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        _sword.rotation = rotation;
        Shoot(direction);
    }

    void Shoot(Vector2 dir)
    {
        GameObject sword = Instantiate(swordInstance, transform);
        Rigidbody2D rb = sword.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * 500);
        sword.transform.rotation = _sword.transform.rotation;
        swordAnim.Play("Reload");
        StartCoroutine(nameof(CoolDown));
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(reloadTimer);
        _isAttack = false;
    }
}