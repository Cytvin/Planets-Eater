using UnityEngine;

public class FightState : IShipState
{
    private Ship _currentEnemy;

    public FightState(Ship ship)
    {
        ship.Agent.enabled = true;
    }

    public IShipState Update(Ship ship)
    {
        if (_currentEnemy == null)
        {
            _currentEnemy = ship.SearchEnemy();
        }

        if (_currentEnemy == null && ship.CurrentPlanet.Owner == ship.Owner)
        {
            return new HoldingState(ship);
        }
        else if (_currentEnemy == null && ship.CurrentPlanet.Owner != ship.Owner)
        {
            return new LandingState(ship);
        }

        Vector3 targetPosition = _currentEnemy.transform.position;
        float distanceToEnemy = Vector3.Distance(ship.transform.position, targetPosition);

        if (distanceToEnemy > ship.Weapon.AttackRange)
        {
            ship.Agent.SetDestination(targetPosition);
        }
        else
        {
            ship.Agent.SetDestination(ship.transform.position);
        }

        if (ship.Weapon.CanShoot && distanceToEnemy <= ship.Weapon.AttackRange)
        {
            ship.Weapon.Shoot(_currentEnemy, ship.transform.position, targetPosition);
        }

        return this;
    }
}
