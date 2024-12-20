using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private LayerMask PlacementCheckMask;
    [SerializeField] private LayerMask PlacementCollideMask;

    [SerializeField] private PlayerStats PlayerStatistics;

    [SerializeField] private Camera PlayerCamera;

    private GameObject CurrentPlacingTower;

    // Set a fixed height for the tower placement
    [SerializeField] private float fixedHeight = 0.02f; // Adjust this value as needed

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
            RaycastHit HitInfo;

            if (Physics.Raycast(camray, out HitInfo, 100f, PlacementCollideMask))
            {
                // Create a new position with a consistent y value
                Vector3 newPosition = HitInfo.point;
                newPosition.y = fixedHeight; // Set to the fixed height

                CurrentPlacingTower.transform.position = newPosition;
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                Destroy(CurrentPlacingTower);
                CurrentPlacingTower = null;
                return;
            }

            if (Input.GetMouseButtonDown(0) && HitInfo.collider.gameObject != null)
            {
                if(!HitInfo.collider.gameObject.CompareTag("Can Not Place"))
                {
                    BoxCollider TowerCollider = CurrentPlacingTower.gameObject.GetComponent<BoxCollider>();
                    TowerCollider.isTrigger = true;

                    Vector3 BoxCenter = CurrentPlacingTower.gameObject.transform.position + TowerCollider.center;
                    Vector3 HalfExtents = TowerCollider.size / 2;
                    if(Physics.CheckBox(BoxCenter, HalfExtents, Quaternion.identity, PlacementCheckMask, QueryTriggerInteraction.Ignore))
                    {
                        GameLoopManager.TowersInGame.Add(CurrentPlacingTower.GetComponent<TowerBehavior>());

                        TowerCollider.isTrigger = false;
                        CurrentPlacingTower = null;
                    }

                }

            }
        }
    }

    public void SetTowerToPlace(GameObject tower)
    {
        int TowerSummonCost = tower.GetComponent<TowerBehavior>().SummonCost;

        if(PlayerStatistics.GetMoney() >= TowerSummonCost)
        {
            CurrentPlacingTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
            PlayerStatistics.AddMoney(-TowerSummonCost);
        }
        else
        {
            Debug.Log("YOU NEED MORE MONEY TO PURCHASE A " + tower.name);
        }
        
    }
}
