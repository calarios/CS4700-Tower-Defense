using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    private GameObject CurrentPlacingTower;

    // Set a fixed height for the tower placement
    [SerializeField] private float fixedHeight = 0f; // Adjust this value as needed

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentPlacingTower != null)
        {
            Ray camray = PlayerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(camray, out RaycastHit hitInfo, 100f))
            {
                // Create a new position with a consistent y value
                Vector3 newPosition = hitInfo.point;
                newPosition.y = fixedHeight; // Set to the fixed height

                CurrentPlacingTower.transform.position = newPosition;
            }

            if (Input.GetMouseButtonDown(0))
            {
                CurrentPlacingTower = null;
            }
        }
    }

    public void SetTowerToPlace(GameObject tower)
    {
        CurrentPlacingTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
    }
}
