using UnityEngine;

public class Bow : Weapon
{
    private bool _isAttack=true;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_ammoCount == 0 && _maxAmmoCount > 0)
        {
            anim.SetTrigger("isReload2");
        }
        else if (!_isAttack && _maxAmmoCount > 0)
        {
            anim.SetTrigger("isReload");
            anim.SetTrigger("isReload2");
        }
    }

    public override void ShootControl()
    {
        if (_ammoCount > 0 && _isAttack)
        {
            _isAttack = false;   
            anim.SetTrigger("Attack");
        }
    }

    public override void ReloadCompleted()
    {
        base.ReloadCompleted();
        _isAttack = true;
        anim.enabled = true;
    }

    public void Attack()
    {
        GoShoot();
    }
}