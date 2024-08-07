using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBomb : Bomb
{
    [SerializeField] private GameObject ıceBoxInstance;

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            float originalSpeed = enemy.speed;
            enemy.Animator.enabled = false;
            enemy.speed = 0;
            GameObject ıceBox = Instantiate(ıceBoxInstance, enemy.transform.position, Quaternion.identity);
            if (!_isExplosion)
                Explosion();
            StartCoroutine(Ice(enemy, originalSpeed, ıceBox));
        }
    }

    IEnumerator Ice(Enemy enemy, float speed, GameObject ıcebox)
    {
        yield return new WaitForSeconds(.7f);
        if (bombAnimator != null)
            Destroy(bombAnimator.gameObject);
        float animationLength = ıcebox.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length;
        yield return new WaitForSeconds(animationLength);
        if (enemy != null)
        {
            enemy.speed = enemy.baseSpeed;
            enemy.Animator.enabled = true;
        }

        Destroy(ıcebox);
    }
}