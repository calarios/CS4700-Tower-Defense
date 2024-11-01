using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

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
        EnemiesToRemove = new Queue<Enemy>();


        EnemyIDsToSummon = new Queue<int>();
        EntitySummoner.Init();

        NodePositions = new Vector3[NodeParent.childCount];

        for(int i = 0; i < NodePositions.Length; i++)
        {
            NodePositions[i] = NodeParent.GetChild(i).position;
        }

        StartCoroutine(GameLoop());
        InvokeRepeating("SummonTest", 0f, 1f);
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

            NativeArray<int> NodeIndices = new NativeArray<int>(EntitySummoner.EnemiesInGame.Count,Allocator.TempJob);
            NativeArray<float> EnemySpeeds = new NativeArray<float>(EntitySummoner.EnemiesInGame.Count,Allocator.TempJob);
            NativeArray<Vector3> NodesToUse = new NativeArray<Vector3>(NodePositions, Allocator.TempJob);
            TransformAccessArray EnemyAccess = new TransformAccessArray(EntitySummoner.EnemiesInGameTransform.ToArray(), 2);

            for(int i = 0; i < EntitySummoner.EnemiesInGame.Count;i++)
            {
                EnemySpeeds[i] = EntitySummoner.EnemiesInGame[i].Speed;
                NodeIndices[i] = EntitySummoner.EnemiesInGame[i].NodeIndex;
            }

            MoveEnemiesJob MoveJob = new MoveEnemiesJob
            {
                NodePositions = NodesToUse,
                EnemySpeed = EnemySpeeds,
                NodeIndex = NodeIndices,
                deltatime = Time.deltaTime
            };

            JobHandle MoveJobHandle = MoveJob.Schedule(EnemyAccess);
            MoveJobHandle.Complete();

            for(int i = 0; i < EntitySummoner.EnemiesInGame.Count; i++)
            {
                EntitySummoner.EnemiesInGame[i].NodeIndex = NodeIndices[i];

                if(EntitySummoner.EnemiesInGame[i].NodeIndex == NodePositions.Length)
                {
                    EnqueEnemyToRemove(EntitySummoner.EnemiesInGame[i]);
                }
            }

            NodesToUse.Dispose();
            EnemySpeeds.Dispose();
            EnemyAccess.Dispose();
            NodesToUse.Dispose();


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

    public static void EnqueEnemyToRemove(Enemy EnemyToRemove)
    {
        EnemiesToRemove.Enqueue(EnemyToRemove);
    }
}

public struct MoveEnemiesJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> NodeIndex;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> EnemySpeed;

    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> NodePositions;
    public float deltatime;

    public void Execute(int index, TransformAccess transform)
    {
        if(NodeIndex[index] < NodePositions.Length)
        {

        }

        Vector3 PositionToMoveTo = NodePositions[NodeIndex[index]];

        transform.position = Vector3.MoveTowards(transform.position, PositionToMoveTo, EnemySpeed[index] * deltatime);

        if(transform.position == PositionToMoveTo)
        {
            NodeIndex[index]++;
        }
    }
}