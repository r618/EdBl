using MultiProjectorWarpSystem;
using UnityEngine;

public class EdBl : MonoBehaviour
{
    [SerializeField] InteractiveFloor.ProjectionWarpSystem projectionWarpSystem;

    void Awake()
    {
        Debug.Log("connected displays: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Debug.LogFormat("Display {0} activating..", i);
            Display.displays[i].Activate();
        }

        // Screen.SetResolution(Display.displays[0].systemWidth * Display.displays.Length, Display.displays[0].systemHeight, FullScreenMode.FullScreenWindow);
    }

    // MainCamera
    // RT
    // Size
    // HDR

    bool eblGui = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            this.eblGui = !this.eblGui;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void OnGUI()
    {
        if (!this.eblGui)
            return;

        for (int i = 0; i < Display.displays.Length; i++)
            GUILayout.Label(string.Format("#{0} {1}x{2} [rendering at {3}x{4}]", i, Display.displays[i].systemWidth, Display.displays[i].systemHeight, Display.displays[i].renderingWidth, Display.displays[i].renderingHeight));


    }
}