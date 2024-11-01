using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public static Vector3[] NodePositions;
    private static Queue<Enemy> EnemiesToRemove;
    private static Queue<int> EnemyIDsToSummon;

    public Transform NodeParent;
    public bool LoopShouldEnd;

    // Start is called before the first frame update
    void Start()
    {
        EnemyIDsToSummon = new Queue<int>();
        EntitySummoner.Init();

        NodePositions = new Vector3[NodeParent.childCount];

        for(int i = 0; i < NodePositions.Length; i++)
        {
            NodePositions[i] = NodeParent.GetChild(i).position;
        }

        StartCoroutine(GameLoop());
        InvokeRepeating("SummonTest", 0f, 1f);
        InvokeRepeating("RemoveTest", 0f, 05f);
    }

    void RemoveTest()
    {
        if(EntitySummoner.EnemiesInGame.Count>0)
        {
            EntitySummoner.RemoveEnemy(EntitySummoner.EnemiesInGame[Random.Range(0, EntitySummoner.EnemiesInGame.Count)]);
        }
    }

    void SummonTest()
    {
        EnqueEnemyIDToSummon(1);
    }

    IEnumerator GameLoop()
    {
        while (LoopShouldEnd == false)
        {
            //Spawn Enemies

            if(EnemyIDsToSummon.Count> 0)
            {
                for(int i = 0; i< EnemyIDsToSummon.Count;i++)
                {
                    EntitySummoner.SummonEnemy(EnemyIDsToSummon.Dequeue());
                }
            }

            //Spawn Towers

            //Move Enemies

            //Tiick Towers

            //Apply Effects

            //Damage Enemies

            //Remove Enemies

            if(EnemiesToRemove.Count > 0)
            {
                for(int i = 0; i< EnemiesToRemove.Count;i++)
                {
                    EntitySummoner.RemoveEnemy(EnemiesToRemove.Dequeue());
                }
            }

            //Remove Towers

            yield return null;
        }
    }

    public static void EnqueEnemyIDToSummon(int ID)
    {
        EnemyIDsToSummon.Enqueue(ID);
    }

    public static void EnqueueEnemyToRemove(Enemy EnemyToRemove)
    {
        EnemiesToRemove.Enqueue(EnemyToRemove);
    }
}
