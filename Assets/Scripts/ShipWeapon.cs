using System.Collections;
using UnityEngine;

public class ShipWeapon
{
    private readonly Ship _owner;
    private readonly LineRenderer _laser;
    private readonly float _damage = 2f;
    private readonly float _attackDelay = 3f;
    private readonly float _attackRange = 4f;
    private bool _canShoot = true;

    public float AttackRange => _attackRange;
    public bool CanShoot => _canShoot;

    public ShipWeapon(Ship owner, LineRenderer laser, float damage)
    {
        _owner = owner;
        _laser = laser;
        _damage = damage;
    }

    public void Shoot(Ship enemy, Vector3 laserStart, Vector3 laserEnd)
    {
        if (!_canShoot)
        {
            return;
        }

        enemy.ApplyDamage(_damage);
        _laser.positionCount = 2;
        _laser.SetPosition(0, laserStart);
        _laser.SetPosition(1, laserEnd);
        _owner.StartCoroutine(RefreshLaser());
        _owner.StartCoroutine(Reload());
        _canShoot = false;
    }

    private IEnumerator RefreshLaser()
    {
        yield return new WaitForSeconds(1f);
        _laser.positionCount = 0;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(_attackDelay);
        _canShoot = true;
    }
}
