using UnityEngine;

public class HoldingState : IShipState
{
    private float _angle;

    public HoldingState(Ship ship)
    {
        ship.Agent.enabled = true;
        _angle = ship.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
    }

    public IShipState Update(Ship ship)
    {
        if (ship.CurrentPlanet.State == Planet.PlanetState.UnderSiege)
        {
            return new FightState(ship);
        }

        _angle += Time.deltaTime;

        float x = Mathf.Cos(_angle * 0.5f) * ship.HoldingRadius;
        float z = Mathf.Sin(_angle * 0.5f) * ship.HoldingRadius;

        Vector3 nextPoint = new Vector3(x, 0, z) + ship.CurrentPlanet.Position;

        ship.Agent.SetDestination(nextPoint);

        return this;
    }
}
