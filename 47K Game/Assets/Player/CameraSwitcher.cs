using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera overheadCamera; // Reference to the overhead camera
    [SerializeField] private Camera firstPersonCamera; // Reference to the first-person camera

    [SerializeField] private GameObject firstPersonPlayer; // Reference to the first-person player object
    [SerializeField] private GameObject overheadPlayer; // Reference to the overhead player object

    private bool isFirstPerson = true; // Track the current camera mode

    private void Update()
    {
        // Switch between first-person and overhead view when the "Q" key is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isFirstPerson)
            {
                SwitchToOverhead();
            }
            else
            {
                SwitchToFirstPerson();
            }
        }
    }

    // Method to switch to first-person mode
    public void SwitchToFirstPerson()
    {
        // Enable first-person mode
        overheadCamera.enabled = false;
        firstPersonCamera.enabled = true;

        // Enable/disable player objects
        overheadPlayer.SetActive(false);
        firstPersonPlayer.SetActive(true);

        // Lock the cursor for first-person controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Update the current state
        isFirstPerson = true;
    }

    // Method to switch back to overhead mode
    public void SwitchToOverhead()
    {
        // Enable overhead mode
        overheadCamera.enabled = true;
        firstPersonCamera.enabled = false;

        // Enable/disable player objects
        overheadPlayer.SetActive(true);
        firstPersonPlayer.SetActive(false);

        // Unlock the cursor for overhead controls
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Update the current state
        isFirstPerson = false;
    }
}
