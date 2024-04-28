using UnityEngine;

public class LandingState : IShipState
{
    public LandingState(Ship ship)
    {
        ship.Agent.enabled = false;
    }

    public IShipState Update(Ship ship)
    {
        if (ship.CurrentPlanet.Owner == ship.Owner)
        {
            return new HoldingState(ship);
        }
        else if (ship.CurrentPlanet.Owner != ship.Owner && ship.CurrentPlanet.Owner != null && ship.SearchEnemy() != null)
        {
            return new FightState(ship);
        }

        if (Vector3.Distance(ship.transform.position, ship.CurrentPlanet.Position) < ship.CurrentPlanet.Radius)
        {
            ship.CurrentPlanet.TakeForLanding(ship);
            Object.Destroy(ship.gameObject);
            Object.Destroy(ship);
        }
        else
        {
            ship.transform.LookAt(ship.CurrentPlanet.transform);
            ship.transform.Translate(ship.Speed * Time.deltaTime * ship.transform.forward, Space.World);
        }

        return this;
    }
}
