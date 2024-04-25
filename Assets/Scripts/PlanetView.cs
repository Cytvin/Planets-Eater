using TMPro;
using UnityEngine;

public class PlanetView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _shipsCountText;

    public void Init()
    {
        _shipsCountText.SetText("0");
    }

    public void SetText(string text)
    {
        _shipsCountText.text = text;
    }
}
