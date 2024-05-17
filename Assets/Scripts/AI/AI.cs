using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private Player _ai;
    private IEnumerable<Planet> _map;
    private float _maxDistanceBetwenPlanet = 25f;

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
        foreach (Planet planet in _map.Where(p => p != startPlanet))
        {
            if (Vector3.Distance(planet.Position, startPlanet.Position) <= _maxDistanceBetwenPlanet && CanReceiveShips(planet))
            {
                return planet;
            }
        }

        return null;
    }

    private bool CanReceiveShips(Planet target)
    {
        if (target.Owner == _ai)
        {
            if (target.MaxOwnerShipToReceive == 0)
            {
                return false;
            }
        }

        if (target.GetMaxEnemyShipToReceive(_ai) == 0)
        {
            return false;
        }

        return true;
    }

    private void TransferShip(Planet from, Planet to)
    {
        if (from.ShipCount == 0)
        {
            //Debug.Log("Нет кораблей для отправки");
            return;
        }

        int shipAmount;

        if (from.Owner == to.Owner)
        {
            shipAmount = Mathf.Min(to.MaxOwnerShipToReceive, from.MaxShipToSend);
        }
        else
        {
            shipAmount = Mathf.Min(to.GetMaxEnemyShipToReceive(from.Owner), from.MaxShipToSend);
        }

        //Debug.Log($"Будет отправлено {shipAmount} кораблей");
        from.SendShipsByAmount(to, shipAmount);
    }
}
