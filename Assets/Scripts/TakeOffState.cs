using UnityEngine;

public class TakeOffState : IShipState
{
    public IShipState Update(Ship ship)
    {
        if (Vector3.Distance(ship.transform.position, ship.CurrentPlanet.Position) > ship.CurrentPlanet.Radius + 1)
        {
            return new HoldingState(ship);
        }

        ship.transform.Translate(ship.Speed * Time.deltaTime * ship.transform.forward, Space.World);

        return this;
    }
}
