using UnityEngine;

public class FlyState : IShipState
{
    private Ship _currentEnemy;

    public FlyState(Ship ship, Planet flyTo)
    {
        ship.Agent.SetDestination(flyTo.Position);
    }

    public IShipState Update(Ship ship)
    {
        if (Vector3.Distance(ship.transform.position, ship.CurrentPlanet.Position) < ship.CurrentPlanet.SearchRadius)
        {
            _currentEnemy = ship.SearchEnemy();
        }

        if (_currentEnemy != null)
        {
            return new FightState(ship);
        }

        if (Vector3.Distance(ship.transform.position, ship.CurrentPlanet.Position) > ship.HoldingRadius)
        {
            return this;
        }

        if (ship.CurrentPlanet.Owner == ship.Owner)
        {
            return new HoldingState(ship);
        }
        else if (ship.CurrentPlanet.Owner == null)
        {
            return new LandingState(ship);
        }
        else
        {
            return new FightState(ship);
        }
    }
}
