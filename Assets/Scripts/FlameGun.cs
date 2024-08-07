using UnityEngine;


public class FlameGun : Weapon
{
    private bool _isFlameActive;
    [SerializeField] private GameObject smokeParticle;


    protected override void OnEnable()
    {
        base.OnEnable();
        ammo.SetActive(false);
        smokeParticle.SetActive(false);
        anim.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            _isFlameActive = true;
            if (isSoundWeapon)
                Audio.Play();
            GoShoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isFlameActive = false;
            if (isSoundWeapon)
                Audio.Stop();
            GoShoot();
        }
    }

    public override void ShootControl()
    {
        if (_maxAmmoCount <= 0 && _ammoCount <= 0)
        {
            ammo.SetActive(false);
            smokeParticle.SetActive(false);
            return;
        }

        _ammoCount -= Time.deltaTime * 10;
        if (_maxAmmoCount > 0 && _ammoCount <= 0 && ammo.activeSelf)
        {
            Audio.Stop();
            ammo.SetActive(false);
            smokeParticle.SetActive(false);
            anim.enabled = _ammoCount <= 0;
            anim.SetTrigger("isReload");
        }

        Player.UpdateWeaponAmmo(_ammoCount, _maxAmmoCount);
    }

    protected override void ChangeYAxis(Vector2 dir)
    {
        base.ChangeYAxis(dir);
        float yAxis = !sr.flipY ? 0.075f : -0.075f;
        float xAxis = !sr.flipY ? -90f : 90f;
        ammoPoint.localPosition = new Vector2(ammoPoint.localPosition.x, yAxis);
        smokeParticle.transform.localRotation = Quaternion.Euler(xAxis, 0, 0);
    }

    public override void GoShoot()
    {
        if (_ammoCount <= 0)
            return;
        ammo.SetActive(_isFlameActive);
        smokeParticle.SetActive(_isFlameActive);
    }
}