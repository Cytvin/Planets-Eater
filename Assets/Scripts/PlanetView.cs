using TMPro;

public class PlanetView
{
    private TextMeshProUGUI _shipsCountText;

    public PlanetView(TextMeshProUGUI shipsCount)
    {
        _shipsCountText = shipsCount;
        _shipsCountText.SetText("0");
    }

    public void SetText(string text)
    {
        _shipsCountText.text = text;
    }
}
