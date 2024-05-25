using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public void OnPauseClick()
    {
        if (MainMenu.Instance == null)
        {
            return;
        }

        MainMenu.Instance.InGameMenuEnable();
    }
}
