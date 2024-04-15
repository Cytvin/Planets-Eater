using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _resourceCount;

    public void SetResourceCount(float count)
    {
        _resourceCount.SetText(Mathf.Round(count).ToString());
    }
}
