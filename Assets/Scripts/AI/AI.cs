using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private Player _ai;
    private IEnumerable<Planet> _map;
    private float _percentageShipsToSend = 25;

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
            if (SendShipToUnderSiegeNeighbor(planet))
            {
                continue;
            }

            Planet targetPlanet = SearchNearEnemyPlanet(planet);

            if (targetPlanet == null)
            {
                continue;
            }

            if (planet.ShipCount > planet.MaxShipCount * (_percentageShipsToSend / 100f))
            {
                TransferShip(planet, targetPlanet);
            }
        }
    }

    private bool SendShipToUnderSiegeNeighbor(Planet planet)
    {
        foreach (Planet neighbour in planet.Neighbors.Keys)
        {
            if (neighbour.State == Planet.PlanetState.UnderSiege)
            {
                TransferShip(planet, neighbour);
                return true;
            }
        }

        return false;
    }

    private Planet SearchNearEnemyPlanet(Planet planet)
    {
        List<Planet> searched = new List<Planet>();
        Queue<Planet> searchQueue = new Queue<Planet>();
        Dictionary<Planet, Planet> previous = new Dictionary<Planet, Planet>();

        searchQueue.Enqueue(planet);

        searched.Add(planet);
        Planet searchedPlanet = null;

        while (searchQueue.Count > 0)
        {
            Planet currentPlanet = searchQueue.Dequeue();

            foreach(Planet neighbor in currentPlanet.Neighbors.Keys)
            {
                if (!searched.Contains(neighbor))
                {
                    searchQueue.Enqueue(neighbor);
                    searched.Add(neighbor);
                    previous.Add(neighbor, currentPlanet);

                    if (neighbor.Owner != _ai)
                    {
                        searchedPlanet = neighbor;
                        searchQueue.Clear();
                        break;
                    }
                }
            }
        }

        if (searchedPlanet == null)
        {
            return null;
        }

        List<Planet> path = new List<Planet>();

        Planet previousPlanet = searchedPlanet;

        while (previous.ContainsKey(previousPlanet))
        {
            previousPlanet = previous[previousPlanet];
            path.Add(previousPlanet);
        }

        path.Reverse();

        if (path.Count == 1)
        {
            return searchedPlanet;
        }
        else
        {
            return path[1];
        }
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