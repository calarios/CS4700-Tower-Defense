using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{

    public static List<Enemy> EnemiesInGame;
    public static List<Transform> EnemiesInGameTransform;
    public static Dictionary<int, GameObject> EnemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;

    private static bool IsInitialized;

    public static void Init()
    {
        if(!IsInitialized)
        {
            EnemyPrefabs = new Dictionary<int, GameObject>();
            EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
            EnemiesInGameTransform = new List<Transform>();
            EnemiesInGame = new List<Enemy>();

            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");
            Debug.Log(Enemies[0].name);

            foreach(EnemySummonData enemy in Enemies)
            {
                EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
                EnemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
            }

            IsInitialized = true;
        }
        else
        {
            Debug.Log("ENTITYSUMMONER: THIS CLASS IS ALREADY INITIALIZED");
        }
    }

    public static Enemy SummonEnemy(int EnemyID)
{
    Enemy SummonedEnemy = null;

    if(EnemyPrefabs.ContainsKey(EnemyID))
    {
        Queue<Enemy> ReferencedQueue = EnemyObjectPools[EnemyID];
        if(ReferencedQueue.Count > 0)
        {
            // Dequeue Enemy and initialize
            SummonedEnemy = ReferencedQueue.Dequeue();

            // Check if enemy has been destroyed before initializing
            if(SummonedEnemy == null || !SummonedEnemy.gameObject.activeInHierarchy)
            {
                SummonedEnemy = InstantiateNewEnemy(EnemyID);
            }
            else
            {
                SummonedEnemy.Init();
                SummonedEnemy.gameObject.SetActive(true);
            }
        }
        else
        {
            SummonedEnemy = InstantiateNewEnemy(EnemyID);
        }
    }
    else
    {
        Debug.Log($"ENTITYSUMMONER: ENEMY WITH ID OF {EnemyID} DOES NOT EXIST!");
        return null;
    }

    EnemiesInGame.Add(SummonedEnemy);
    EnemiesInGameTransform.Add(SummonedEnemy.transform);

    return SummonedEnemy;
}

private static Enemy InstantiateNewEnemy(int EnemyID)
{
    GameObject NewEnemy = Instantiate(EnemyPrefabs[EnemyID], GameLoopManager.NodePositions[0], Quaternion.identity);
    Enemy SummonedEnemy = NewEnemy.GetComponent<Enemy>();
    SummonedEnemy.Init();
    return SummonedEnemy;
}


    public static void RemoveEnemy(Enemy EnemyToRemove)
    {
        if (EnemyToRemove == null || !EnemyToRemove.gameObject.activeInHierarchy) return;

        EnemyObjectPools[EnemyToRemove.ID].Enqueue(EnemyToRemove);
        EnemyToRemove.gameObject.SetActive(false);
        EnemiesInGameTransform.Remove(EnemyToRemove.transform);
        EnemiesInGame.Remove(EnemyToRemove);
    }



}
