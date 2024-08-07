using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float speed;
    [SerializeField] private Transform weapons;
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private Transform weaponInfoPanel;
    [SerializeField] private Image dashImage;
    [SerializeField] private Image healthBar;
    [SerializeField] private Sprite deadImage;
    [SerializeField] private Sprite deadImage2;
    [SerializeField] private Sprite liveImage;
    [SerializeField] private Sprite liveImage2;
    [SerializeField] private AudioSource walkAudio;
    [SerializeField] private AudioSource clipAudio;
    [SerializeField] private AudioClip medkitSound;
    [SerializeField] private AudioClip deadSound;
    [HideInInspector] public bool isHealthSystem;
    [HideInInspector] public bool isSound;
    public RuntimeAnimatorController player1Anim;
    public RuntimeAnimatorController player2Anim;
    private TextMeshProUGUI _currentAmmo;
    private TextMeshProUGUI _maxAmmo;
    private Weapon _weapon;
    public Animator animator;
    private SpriteRenderer sr;
    private PlayerInput _playerInput;
    private Animator _previousSlot;
    private float _slotWaitTime = 1;
    private bool _corotineControl;
    private bool _isAttack;
    private bool _isFlip;
    private GameObject _previousWeapon;
    private bool _isChangeWeapon;
    private int _previousWeaponIndex;
    private int _scroolWeapon;
    private Vector2 _dashDirection;
    public bool ısDash;
    bool isDashing, _isMoving = true;
    private Tween _dashCoolDown;
    private Tween _healthAnimation;
    private float _health = 100;
    private float _healthCount = 2f;
    private bool _isHealthCorotine;
    [HideInInspector] public bool isCameraShake;
    [HideInInspector] public CircleCollider2D circleCollider2D;
    private CameraShake _cameraShake;
    [SerializeField] private GameObject pausePanel;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _weapon = weapons.transform.GetChild(0).GetComponent<Weapon>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        _playerInput = GetComponent<PlayerInput>();
        _currentAmmo = weaponInfoPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
        _maxAmmo = weaponInfoPanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        _cameraShake = GameManager.instance.cameraShake;
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        _playerInput.actions["ChangeWeapon"].performed += ChangeWeapon;
        _isFlip = true;
        _health = 100;
        healthBar.fillAmount = 1f;
        StartCoroutine(nameof(FlipControl));
        _scroolWeapon = 1;
        if (ısDash)
        {
            isDashing = false;
            _rb.velocity = Vector2.zero;
            _isMoving = true;
        }
    }

    private void OnDisable()
    {
        _isFlip = false;
        weaponPanel.SetActive(false);
        weaponInfoPanel.gameObject.SetActive(false);
        _slotWaitTime = 2;
        _corotineControl = false;
        _playerInput.actions["ChangeWeapon"].performed -= ChangeWeapon;
    }

    private void Update()
    {
        if (_weapon != null && Input.GetMouseButton(0))
        {
            _weapon.ShootControl();
        }

        MouseScroll();
    }

    public void TakeDamage()
    {
        clipAudio.PlayOneShot(deadSound);
        if (isHealthSystem)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
            if (_health >= 100)
                healthBar.fillAmount = 1f;
            _health -= 15;
            _health = Mathf.Max(0, _health);
            _healthCount = 2f;
            if (!_isHealthCorotine)
                StartCoroutine(nameof(HealthBarVisible));
            if (_healthAnimation == null)
                _healthAnimation = healthBar.DOFillAmount(_health / 100, .25f)
                    .SetEase(Ease.Linear)
                    .OnComplete((() => { _healthAnimation = null; }));
            if (isCameraShake)
                _cameraShake.Shake("Damage");
            if (_health <= 0)
            {
                if (isSound)
                    clipAudio.PlayOneShot(deadSound);
                Died();
            }
        }
        else
        {
            if (isSound)
                clipAudio.PlayOneShot(deadSound);
            Died();
        }
    }

    void Died()
    {
        StartCoroutine(nameof(colliderBack));
        if (animator.enabled)
            animator.Play("PlayerDied");
        else
        {
            if (animator.runtimeAnimatorController == player1Anim)
                sr.sprite = deadImage;
            else
                sr.sprite = deadImage2;
        }

        _rb.velocity = Vector2.zero;
        GameManager.instance.PlayerDied();
        _health = 100;
        if (isCameraShake)
            _cameraShake.Shake("Damage");
        if (animator.enabled)
            StartCoroutine(nameof(PlayBack));
    }

    public void UpdateWeaponAmmo(float ammo, float nextAmmo)
    {
        _currentAmmo.text = ammo.ToString("0");
        _maxAmmo.text = nextAmmo.ToString();
    }

    void RotateWeaponPanel(int index2)
    {
        _isChangeWeapon = false;
        /* bool isPick = weapons.GetChild(index2 - 1).GetComponent<Weapon>().isPick;
         if (index2 == _previousWeaponIndex || !isPick)
         {
             _isChangeWeapon = false;
             return;
         }
 
         float angle = index2 > _previousWeaponIndex ? 180 : -180;
         _previousWeaponIndex = index2;
         RectTransform panel = weaponInfoPanel.GetComponent<RectTransform>();
         panel.DORotate(new Vector3(0f, 0f, angle), 1f).SetEase(Ease.OutQuad).From()
             .OnComplete((() => { _isChangeWeapon = false; })); */
    }

    void ChangeAnimation(Vector2 dir)
    {
        if (dir.x == 1 || dir.x == -1)
            animator.Play("PlayerIdleHorizontal");
        else if (dir.y == 1)
            animator.Play("PlayerIdleBack");
        else if (dir.y == -1)
            animator.Play("PlayerIdle");

        sr.flipX = dir.x != 0 ? dir.x <= 0 : sr.flipX;
    }

    public void ChangeWeaponinCode(int index)
    {
        Animator currentSlot = weaponPanel.transform.GetChild(index - 1).GetComponent<Animator>();
        RotateWeaponPanel(index);
        if (_previousSlot == currentSlot)
            return;
        weaponPanel.SetActive(true);
        _slotWaitTime = 2;
        currentSlot.SetTrigger("isChoose");
        if (_previousSlot != null)
            _previousSlot.SetTrigger("isBack");
        _previousSlot = currentSlot;
        if (!_corotineControl)
            StartCoroutine(SlotAnimation());
        _corotineControl = true;
        if (weapons.childCount < index || !weapons.GetChild(index - 1).GetComponent<Weapon>().isPick)
            return;

        GameObject currentWeapon = weapons.GetChild(index - 1).gameObject;
        currentWeapon.SetActive(true);
        _weapon = currentWeapon.GetComponent<Weapon>();
        if (_previousWeapon == null)
        {
            _previousWeapon = weapons.GetChild(index - 1).gameObject;
            return;
        }

        if (currentWeapon != _previousWeapon)
        {
            _previousWeapon.SetActive(false);
            _previousWeapon = currentWeapon.gameObject;
        }
    }

    #region Player Input

    void OnMove(InputValue value)
    {
        if (!_isMoving)
            return;
        Vector2 direction = value.Get<Vector2>();
        _dashDirection = direction;
        _rb.velocity = direction * speed;
        ChangeAnimation(direction);
        PlayWalkSound(direction);
    }

    void ChangeWeapon(InputAction.CallbackContext ctx)
    {
        int numKeyValue;
        int.TryParse(ctx.control.name, out numKeyValue);
        if (!weapons.GetChild(0).GetComponent<Weapon>().isPick || _isChangeWeapon || numKeyValue == 0)
            return;
        _isChangeWeapon = true;
        _scroolWeapon = numKeyValue;
        ChangeWeaponinCode(numKeyValue);
    }

    void OnDash()
    {
        if (ısDash && !isDashing)
        {
            animator.Play("PlayerDash");
            isDashing = true;
            _isMoving = false;
            _dashDirection.Normalize();
            _rb.AddForce(_dashDirection * 400f);
            StartCoroutine(nameof(DashContinue));
        }
    }

    void OnPause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        _playerInput.enabled = false;
    }

    #endregion


    void MouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int currentValue = scroll > 0 ? 1 : -1;
            _scroolWeapon += currentValue;
            _scroolWeapon = Mathf.Clamp(_scroolWeapon, 1, 9);
            _isChangeWeapon = true;
            ChangeWeaponinCode(_scroolWeapon);
        }
    }


    void PlayWalkSound(Vector2 dir)
    {
        if (!isSound)
            return;
        if (dir != Vector2.zero && !walkAudio.isPlaying)
            walkAudio.Play();
        else if (dir == Vector2.zero && walkAudio.isPlaying)
            walkAudio.Stop();
    }

    void DashCoolDown()
    {
        dashImage.transform.parent.gameObject.SetActive(true);
        _dashCoolDown = dashImage.DOFillAmount(1, 5f)
            .SetEase(Ease.Linear)
            .OnComplete((() =>
            {
                isDashing = false;
                dashImage.transform.parent.gameObject.SetActive(false);
                dashImage.fillAmount = 0;
            }));
    }

    public void ChangeSprite()
    {
        sr.sprite = animator.runtimeAnimatorController == player1Anim ? liveImage : liveImage2;
    }

    IEnumerator DashContinue()
    {
        yield return new WaitForSeconds(.7f);
        _rb.velocity = Vector2.zero;
        _isMoving = true;
        DashCoolDown();
    }

    IEnumerator colliderBack()
    {
        circleCollider2D.enabled = false;
        yield return new WaitForSeconds(2f);
        circleCollider2D.enabled = true;
    }

    IEnumerator SlotAnimation()
    {
        while (_slotWaitTime > 0)
        {
            _slotWaitTime -= Time.deltaTime;
            yield return null;
        }

        weaponPanel.SetActive(false);
        _slotWaitTime = 2;
        _corotineControl = false;
    }

    IEnumerator FlipControl()
    {
        while (_isFlip)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 distance = mousePos - transform.position;

            sr.flipX = distance.x < 0;
            yield return null;
        }
    }

    IEnumerator HealthBarVisible()
    {
        _isHealthCorotine = true;
        while (_healthCount > 0)
        {
            _healthCount -= Time.deltaTime;
            yield return null;
        }

        healthBar.transform.parent.gameObject.SetActive(false);
        _isHealthCorotine = false;
    }

    IEnumerator PlayBack()
    {
        yield return new WaitForSeconds(2f);
        animator.Play("PlayerDiedBack");
        yield return new WaitForSeconds(1f);
        circleCollider2D.enabled = true;
        _rb.velocity = Vector2.zero;
        _isMoving = true;
        if (ısDash)
            isDashing = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Flag"))
        {
            GameManager.instance.StartCoroutine(GameManager.instance.YouWin());
        }
        else if (col.CompareTag("MedKit"))
        {
            Destroy(col.gameObject);
            healthBar.transform.parent.gameObject.SetActive(true);
            _health += 15;
            _health = Mathf.Min(100, _health);
            _healthCount = 2f;
            if (!_isHealthCorotine)
                StartCoroutine(nameof(HealthBarVisible));
            if (isSound)
                clipAudio.PlayOneShot(medkitSound);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }
}