using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class Weapon : MonoBehaviour
{
    [Header("Weapon Public Values")] public bool isPick;
    public string weaponName;
    protected float _nextFireTime;
    protected float _ammoCount;
    protected float _maxAmmoCount;
    protected Animator anim;
    protected Player Player;
    private bool _isStart;
    private Tween _punchTween;
    private Tween _weaponInfoTween;
    private float _yAxis;
    private CameraShake _cameraShake;
    private bool _isAnimation;
    protected AudioSource Audio;
    [HideInInspector] public bool isCameraShake;
    private bool _isParticle;
    [HideInInspector] public bool isSoundWeapon=false;

    [Header("Weapon Variables")] [SerializeField]
    private float rotationSpeed;

    [SerializeField] private AudioSource reloadAudio;
    [SerializeField] protected Transform ammoPoint;
    [SerializeField] protected GameObject ammo;
    [SerializeField] private float fireRate;
    [SerializeField] private float firePower;
    [SerializeField] private float baseAmmoCount;
    [SerializeField] private float baseMaxAmmoCount;
    [SerializeField] private Image weaponFill;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] private Transform rotationObj;
    [SerializeField] private GameObject firingParticle;
    [SerializeField] protected AudioClip weaponSound;
    [SerializeField] protected AudioClip reloadSound;

    [Header("Weapon Punch Variables")] [SerializeField]
    private float duration;

    [SerializeField] private int vibrato;
    [SerializeField] private float elasticity;

    [Header("Weapon Info Variables")] [SerializeField]
    private Transform weaponInfoPanel;

    [SerializeField] private float ınfoDuration;
    [SerializeField] private float ınfoStrenth;
    [SerializeField] private int ınfoVibrato;
    [SerializeField] private Sprite weaponSprite;
    [SerializeField] private float weaponSpriteWidth;
    
    [Header("Cursor")]
    public Texture2D cursor;
    private Vector2 cursorHotspot;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        _ammoCount = baseAmmoCount;
        _maxAmmoCount = baseMaxAmmoCount;
        Player = transform.parent.parent.GetComponent<Player>();
        _yAxis = ammoPoint.transform.localPosition.y;
        _cameraShake = GameManager.instance.cameraShake;
        Audio = GetComponent<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        if (_ammoCount == 0 && _maxAmmoCount > 0)
        {
            anim.enabled = true;
            anim.SetTrigger("isReload");
        }

        Player.UpdateWeaponAmmo(_ammoCount, _maxAmmoCount);
        /* if (ammoPoint.GetComponent<Animator>() != null)
         {
             Animator animator = ammoPoint.GetComponent<Animator>();
             ammoPoint.GetComponent<Animator>().Play("Idle");
             ammoPoint.GetComponent<SpriteRenderer>().sprite = null;
         } */
        _isParticle = GameManager.instance.isParticle;
        _isAnimation = GameManager.instance.isAnimation;
        isSoundWeapon = GameManager.instance.isSound;
        ChangeCursor();
    }

    protected virtual void Update()
    {
        if (_ammoCount > 0 || weaponFill != null)
            LookMouse();
        if (Input.GetMouseButtonUp(0))
        {
            if (weaponName == "Mini Gun" || weaponName == "Machine Gun")
                SoundEffect();
        }
    }

    public void ReloadAgain()
    {
        _ammoCount = baseAmmoCount;
        _maxAmmoCount = baseMaxAmmoCount;
    }

    public virtual void ReloadCompleted()
    {
        _ammoCount = baseAmmoCount;
        _maxAmmoCount -= baseAmmoCount;
        anim.enabled = false;
        Player.UpdateWeaponAmmo(_ammoCount, _maxAmmoCount);
        if (isSoundWeapon && reloadSound != null)
            reloadAudio.PlayOneShot(reloadSound);
    }

    public virtual void ShootControl()
    {
        if (Time.time > _nextFireTime && _ammoCount > 0)
        {
            GoShoot();
        }
    }

    void ChangeCursor()
    {
        cursorHotspot = new Vector2(cursor.width / 2, cursor.height / 2);
        Cursor.SetCursor(cursor, cursorHotspot, CursorMode.Auto);
    }

    public virtual void GoShoot()
    {
        /* if (ammoPoint.GetComponent<Animator>() != null)
         {
             Animator animator = ammoPoint.GetComponent<Animator>();
             if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                 ammoPoint.GetComponent<Animator>().SetTrigger("isAttack");
         } */
        if (GameManager.instance.isParticle)
            CreateParticle();
        if (!Audio.isPlaying && weaponName != "Flame Gun")
            SoundEffect();
        Shoot();
        WeaponInfoAnimation();
        if (isCameraShake)
            _cameraShake.Shake(weaponName);
        _nextFireTime = Time.time + 1f / fireRate;
        _ammoCount--;
        Player.UpdateWeaponAmmo(_ammoCount, _maxAmmoCount);
        if (_maxAmmoCount > 0)
        {
            if(_ammoCount <=0 && weaponFill==null)
                Audio.Stop();
            anim.enabled = _ammoCount <= 0;
            anim.SetTrigger("isReload");
        }
        
    }

    void CreateParticle()
    {
        if (firingParticle != null)
            Instantiate(firingParticle, ammoPoint.transform);
    }

    void SoundEffect()
    {
        if (!isSoundWeapon || _ammoCount <=0)
            return;
        if (!Audio.isPlaying && weaponSound != null && isSoundWeapon)
        {
            if (Audio.loop)
                Audio.Play();
            else
                Audio.PlayOneShot(weaponSound);
        }
        else if (Audio.isPlaying && Audio.loop && weaponSound != null && isSoundWeapon)
            Audio.Stop();
    }

    void WeaponInfoAnimation()
    {
       /* if (_weaponInfoTween == null)
        {
            _weaponInfoTween = weaponInfoPanel.DOShakePosition(ınfoDuration, ınfoStrenth, ınfoVibrato, 90f)
                .OnComplete((() => { _weaponInfoTween = null; }));
        } */
    }

    protected void Shoot()
    {
        GameObject bullet = Instantiate(ammo, ammoPoint.position, ammoPoint.localRotation);
        // Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2 direction = (mousePosition - ammoPoint.transform.position);
        bullet.GetComponent<Rigidbody2D>().AddForce(ammoPoint.right * firePower);
        bullet.transform.rotation = ammoPoint.parent.transform.rotation;
        if (weaponFill == null && _isAnimation)
            AttackAnimation();
    }


    void LookMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePosition.z = 0f;

        Vector3 characterPosition = rotationObj.position;

        Vector3 direction = mousePosition - characterPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        rotationObj.localRotation = Quaternion.Slerp(rotationObj.rotation, rotation, rotationSpeed * Time.deltaTime);
        if (sr != null)
            ChangeYAxis(direction);
    }

    protected virtual void ChangeYAxis(Vector2 dir)
    {
        dir.Normalize();
        sr.flipY = dir.x < 0;
        if (sr.flipY && _yAxis > 0 || !sr.flipY && _yAxis < 0)
            _yAxis *= -1;
        ammoPoint.transform.localPosition = new Vector2(ammoPoint.transform.localPosition.x, _yAxis);
    }

    void AttackAnimation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 punchDirection = (mousePosition - transform.position) * -1;
        punchDirection.Normalize();
        if (_punchTween == null)
            _punchTween = transform.DOPunchPosition(punchDirection, duration, vibrato, elasticity)
                .SetEase(Ease.OutCubic)
                .OnComplete((() => { _punchTween = null; }));
    }
}