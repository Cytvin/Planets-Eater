using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
    private Planet _from;
    private Planet _to;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Planet pickedPlanet = PickPLanet();

            if (pickedPlanet == null) 
            {
                return;
            }

            if (_from == null)
            {
                if (pickedPlanet.Owner != null) //Для теста, чтобы проверять поведение кораблей. Потом поменять на == Owner.Player
                {
                    _from = pickedPlanet;
                }
            }
            else
            {
                if (pickedPlanet != _from)
                {
                    _to = pickedPlanet;
                }
            }
        }

        if (_from != null && _to != null)
        {
            Transfer(_from, _to);
            _from = null;
            _to = null;
        }
    }

    public Planet PickPLanet()
    {
        Vector3 screenPoint = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            if (raycastHit.collider.TryGetComponent<Planet>(out Planet planet))
            {
                return planet;
            }
        }

        return null;
    }

    private void Transfer(Planet from, Planet to)
    {
        if (from.ShipCount == 0)
        {
            Debug.Log("Нет кораблей для отправки");
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

        Debug.Log($"Будет отправлено {shipAmount} кораблей");
        from.SendShipsByAmount(to, shipAmount);
    }
}