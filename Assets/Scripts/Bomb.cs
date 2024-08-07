using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Bomb : Bullet
{
    protected Animator anim;
    protected bool _isExplosion;
    protected Rigidbody2D rb;
    private CircleCollider2D cr;
    [SerializeField] private bool isBomb;
    private bool _isParticle;
    private bool _isSound;
    private ParticleSystem _explosionParticle;
    [SerializeField] private AudioClip weaponSound;
    [SerializeField] protected Animator bombAnimator;
    [SerializeField] private Light2D light2d;
    private AudioSource _audioSource;

    private void Awake()
    {
        DestoryTiming = 7f;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cr = GetComponent<CircleCollider2D>();
        _explosionParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        light2d = GetComponent<Light2D>();
        _audioSource = GetComponent<AudioSource>();
        _isSound = GameManager.instance.isSound;
        light2d.enabled = GameManager.instance.isLight;
    }

    private void OnEnable()
    {
        _isParticle = GameManager.instance.isParticle;
    }


    private void Update()
    {
        if (!_isExplosion)
            transform.Rotate(0f, 0f, -500f * Time.deltaTime);
    }

    protected void Explosion()
    {
        if (GameManager.instance.isCameraShake && isBomb)
            GameManager.instance.cameraShake.Shake("Bomba");
        _isExplosion = true;
        if (_isSound)
            _audioSource.PlayOneShot(weaponSound);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.velocity = Vector2.zero;
        anim.Play("Explosion");
        if (_isParticle)
        {
            bombAnimator.Play("Explosion");
            //_explosionParticle.Play();
        }
    }

    public void BombEnd()
    {
        if (isBomb)
            Destroy(gameObject);
        else
            cr.enabled = false;
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !_isExplosion)
        {
            Explosion();
        }
    }
}