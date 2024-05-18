using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private Player _ai;
    private IEnumerable<Planet> _map;

    public void Init(Player ai, IEnumerable<Planet> map)
    {
        _ai = ai;
        _map = map;
    }

    private void Update()
    {
        IEnumerable<Planet> planets = _ai.Planets;

        foreach (Planet planet in planets)
        {
            Planet targetPlanet = SearchTargetPlanet(planet);

            if (targetPlanet == null)
            {
                continue;
            }

            TransferShip(planet, targetPlanet);
        }
    }

    private Planet SearchTargetPlanet(Planet startPlanet)
    {
        //TODO: Переделать алгоритм выбора планеты, на которую отправлять корабли
        foreach (Planet planet in _map.Where(p => p != startPlanet))
        {
            if (startPlanet.IsNeighbor(planet) && planet.GetMaxShipToReceiveByPlayer(_ai) > 0)
            {
                return planet;
            }
        }

        return null;
    }

    private void TransferShip(Planet from, Planet to)
    {
        if (from.ShipCount == 0)
        {
            return;
        }

        int shipAmount;

        shipAmount = Mathf.Min(to.GetMaxShipToReceiveByPlayer(from.Owner), from.MaxShipToSend);

        from.SendShipsByAmount(to, shipAmount);
    }
}
