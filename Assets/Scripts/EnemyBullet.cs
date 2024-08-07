using System;
using UnityEngine;


public class EnemyBullet : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject,2f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<Player>().TakeDamage();
        }
    }
}
