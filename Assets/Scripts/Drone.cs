using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    private Transform _player;
    private bool _isAttack = true;
    private Animator _animator;
    private SpriteRenderer sr;
    private LineRenderer _lr;
    private GameObject _hitCollider;
    private Transform _laserPosition;
    private AudioSource _audioSource;
    [HideInInspector] public bool isSound;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layerMask2;
    [SerializeField] private AudioClip laserClip;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        _lr = transform.GetChild(0).GetComponent<LineRenderer>();
        _hitCollider = transform.GetChild(1).gameObject;
        _laserPosition = transform.GetChild(2).transform;
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _isAttack = true;
    }

    private void FixedUpdate()
    {
        MovetoPlayer();
        FindEnemy();
    }


    void MovetoPlayer()
    {
        float distance = Vector2.Distance(_player.position, transform.position);

        if (distance > 2f)
            transform.position = Vector2.MoveTowards(transform.position, _player.position,
                speed * Time.deltaTime);
    }

    void FindEnemy()
    {
        LayerMask combinedLayerMask = layerMask | layerMask2;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 20f, combinedLayerMask);

        Vector2 pos = FindCloserEnemy(enemies);
        LookAtEnemy(pos);
        AttacktoEnemy(pos);
    }

    Vector2 FindCloserEnemy(Collider2D[] enemies)
    {
        Vector2 enemy = Vector2.zero;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (i == 0)
                enemy = enemies[0].transform.position;

            Vector2 nextEnemy = enemies[i].transform.position;
            float distance = Vector2.Distance(transform.position, nextEnemy);
            float distance2 = Vector2.Distance(transform.position, enemy);

            if (distance < distance2)
                enemy = nextEnemy;
        }

        return enemy;
    }

    void LookAtEnemy(Vector2 pos)
    {
        if (!_animator.enabled)
            return;
        float distanceY = Mathf.Abs(pos.y - transform.position.y);
        if (distanceY > 2f)
        {
            if (pos.y > transform.position.y)
                _animator.Play("Drone-Back");
            else
                _animator.Play("Drone-On");
            _laserPosition.localPosition = new Vector2(0, -0.084f);
        }
        else
        {
            _animator.Play("Drone-Horizontal");
            sr.flipX = pos.x < transform.position.x;
            _laserPosition.localPosition = new Vector2(0.404f, -0.05f);
        }
    }

    void AttacktoEnemy(Vector2 enemy)
    {
        if (enemy == Vector2.zero)
            return;
        float distance = Vector2.Distance(transform.position, enemy);
        if (_isAttack && distance < 6f)
        {
            _isAttack = false;
            PlaySound();
            _lr.gameObject.SetActive(true);
            _hitCollider.SetActive(true);
            _lr.SetPosition(0, transform.position);
            _lr.SetPosition(1, enemy);
            _hitCollider.transform.position = _lr.GetPosition(1);
            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    void PlaySound()
    {
        if (!isSound)
            return;
        _audioSource.PlayOneShot(laserClip);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        print(other.gameObject.name);
    }

    IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(.1f);
        _lr.gameObject.SetActive(false);
        _hitCollider.SetActive(false);
    }

    IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(2f);
        _isAttack = true;
    }
}