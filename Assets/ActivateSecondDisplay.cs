using UnityEngine;

public class ActivateDisplays : MonoBehaviour
{
    void Start()
    {
        // FORCE the main window to be only one monitor
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1920, 1080, false);

        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            Debug.Log("Activated Display 2");
        }
    }
}