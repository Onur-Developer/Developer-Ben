using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    #region Editor Values

    public float speed;
    [SerializeField] private GameObject deadBodyInstance;
    [SerializeField] private GameObject medKitInstance;
    [SerializeField] private int maxDeadAnimation;
    [SerializeField] private float health;
    [SerializeField] private Image healthBar;
    [SerializeField] private AudioClip[] deathSounds;
    public int spawnChance;

    #endregion

    #region Private Values

    protected Rigidbody2D Rb;
    protected GameObject Player;
    [HideInInspector] public Animator Animator;
    private SpriteRenderer sr;
    [HideInInspector] public float baseSpeed;
    [HideInInspector] public bool isHealthSystem;
    private float _healthCount = 2f;
    private bool _isBody;
    private bool _isCorotineWorking;
    private bool _isMedKit;
    private bool _isAnimation;
    private bool _isSound;
    private float _maxHealth;

    #endregion

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Player = GameObject.FindWithTag("Player");
        sr = GetComponent<SpriteRenderer>();
        baseSpeed = speed;
        _maxHealth = health;
        isHealthSystem = GameManager.instance.isHealthSystem;
        _isAnimation = GameManager.instance.isAnimation;
        _isMedKit = GameManager.instance.isMedKit;
        _isSound = GameManager.instance.isSound;
        Animator.enabled = _isAnimation;
    }

    private void OnDestroy()
    {
        if (!_isBody)
            return;
        GameManager.instance.EnemyCount = -1;
        GameManager.instance.headCount.IncreaseValue();
        CreateDeadBody();
        CreateMedKit();
    }

    void CreateDeadBody()
    {
        GameObject deadBody = Instantiate(deadBodyInstance, transform.parent);
        deadBody.transform.position = transform.position;
        if (_isAnimation)
        {
            Animator animator = deadBody.GetComponent<Animator>();
            int randomValue = Random.Range(0, maxDeadAnimation);
            animator.Play("Death" + randomValue);
        }

        if (_isSound)
        {
            AudioSource audioSource = deadBody.GetComponent<AudioSource>();
            int randomIndex = Random.Range(0, deathSounds.Length);
            audioSource.PlayOneShot(deathSounds[randomIndex]);
            audioSource.pitch = Random.Range(.9f, 1.1f);
        }
    }

    void CreateMedKit()
    {
        if (!_isMedKit)
            return;

        int randomValue = Random.Range(0, 101);

        if (randomValue < 10)
        {
            GameObject medkit = Instantiate(medKitInstance, transform.position, Quaternion.identity);
            medkit.transform.GetChild(0).gameObject.SetActive(GameManager.instance.isLight);
        }
    }


    private void FixedUpdate()
    {
        if (Player != null)
            FollowPlayer();
    }


    protected virtual void FollowPlayer()
    {
        Vector2 direction = Player.transform.position - transform.position;
        direction.Normalize();
        Rb.velocity = direction * speed;
        ChangeAnimation(direction);
    }

    void ChangeAnimation(Vector2 location)
    {
        if (health <= 0 || !_isAnimation)
            return;
        if (location.x > 0)
            Animator.Play("EnemyIdleHorizontal");
        else if (location.y > 0)
            Animator.Play("EnemyIdleBack");
        else if (location.y < 0)
            Animator.Play("EnemyIdleOn");

        sr.flipX = location.x < 0;
    }

    void TakeDamage(float damage)
    {
        if (isHealthSystem)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
            health -= damage;
            health = Mathf.Max(0, health);
            healthBar.fillAmount = health / _maxHealth;
            _healthCount = 1f;
            if (health <= 0)
                Died();
            else if (!_isCorotineWorking)
                StartCoroutine(nameof(HealthBarVisible));
        }
        else if (damage > 0)
            Died();
    }

    void Died()
    {
        _isBody = true;
        Destroy(gameObject);
    }

    IEnumerator HealthBarVisible()
    {
        _isCorotineWorking = true;
        while (_healthCount > 0)
        {
            _healthCount -= Time.deltaTime;
            yield return null;
        }

        healthBar.transform.parent.gameObject.SetActive(false);
        _isCorotineWorking = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet"))
        {
            float comingDamage = col.GetComponent<Bullet>().damage;
            TakeDamage(comingDamage);
        }
        else if (col.CompareTag("Flame"))
        {
            float comingDamage = col.transform.parent.GetComponent<Flame>().damage;
            TakeDamage(comingDamage);
        }
        else if (col.CompareTag("Mud"))
        {
            speed /= 2;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Mud"))
        {
            speed = baseSpeed;
        }
    }
}