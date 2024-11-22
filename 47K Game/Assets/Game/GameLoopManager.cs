using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class GameLoopManager : MonoBehaviour
{
    public static List<TowerBehavior> TowersInGame;
    public static Vector3[] NodePositions;
    public static float[] NodeDistances;

    private static Queue<EnemyDamageData> DamageData;
    private static Queue<Enemy> EnemiesToRemove;
    private static Queue<int> EnemyIDsToSummon;

    private PlayerStats PlayerStatistics;

    public Transform NodeParent;
    public bool LoopShouldEnd;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStatistics = FindObjectOfType<PlayerStats>();

        DamageData = new Queue<EnemyDamageData>();
        TowersInGame = new List<TowerBehavior>();
        EnemyIDsToSummon = new Queue<int>();
        EnemiesToRemove = new Queue<Enemy>();
        EntitySummoner.Init();

        NodePositions = new Vector3[NodeParent.childCount];

        for(int i = 0; i < NodePositions.Length; i++)
        {
            NodePositions[i] = NodeParent.GetChild(i).position;
        }


        NodeDistances = new float[NodePositions.Length - 1];

        for(int i = 0; i < NodeDistances.Length; i++)
        {
            NodeDistances[i] = Vector3.Distance(NodePositions[i], NodePositions[i + 1]);
        }



        StartCoroutine(GameLoop());
        InvokeRepeating("SummonTest", 0f, 1f);
    }

    void SummonTest()
    {
        EnqueEnemyIDToSummon(2);
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

//CHANGED .tempjob to .persistent
            NativeArray<Vector3> NodesToUse = new NativeArray<Vector3>(NodePositions, Allocator.Persistent);
            NativeArray<float> EnemySpeeds = new NativeArray<float>(EntitySummoner.EnemiesInGame.Count, Allocator.Persistent);
            NativeArray<int> NodeIndices = new NativeArray<int>(EntitySummoner.EnemiesInGame.Count, Allocator.Persistent);
            TransformAccessArray EnemyAccess = new TransformAccessArray(EntitySummoner.EnemiesInGameTransform.ToArray(), 2);

            for(int i = 0; i < EntitySummoner.EnemiesInGame.Count; i++)
            {
                EnemySpeeds[i] = EntitySummoner.EnemiesInGame[i].Speed;
                NodeIndices[i] = EntitySummoner.EnemiesInGame[i].NodeIndex;
            }

            MoveEnemiesJob MoveJob = new MoveEnemiesJob
            {
                NodePositions = NodesToUse,
                EnemySpeed = EnemySpeeds,
                NodeIndex = NodeIndices,
                deltaTime = Time.deltaTime
            };

            JobHandle MoveJobHandle = MoveJob.Schedule(EnemyAccess);

            MoveJobHandle.Complete();

            for(int i = 0; i < EntitySummoner.EnemiesInGame.Count; i++)
            {
                EntitySummoner.EnemiesInGame[i].NodeIndex = NodeIndices[i];

                if(EntitySummoner.EnemiesInGame[i].NodeIndex == NodePositions.Length)
                {
                    EnqueueEnemyToRemove(EntitySummoner.EnemiesInGame[i]);
                    PlayerStatistics.DamageCastle((int) EntitySummoner.EnemiesInGame[i].AttackPower);//TESTING DAMAGE
                }
            }

            NodesToUse.Dispose();
            EnemySpeeds.Dispose();
            EnemyAccess.Dispose();
//            NodesToUse.Dispose();

            //Tick Towers

            foreach(TowerBehavior tower in TowersInGame)
            {
                tower.Target = TowerTargeting.GetTarget(tower, TowerTargeting.TargetType.First);
                tower.Tick();
            }

            //Apply Effects

            //Damage Enemies

            if(DamageData.Count > 0)
            {
                for(int i = 0; i< DamageData.Count;i++)
                {
                    EnemyDamageData CurrentDamageData = DamageData.Dequeue();
                    CurrentDamageData.TargetedEnemy.Health -= CurrentDamageData.TotalDamage = CurrentDamageData.Resistance;

                    PlayerStatistics.AddMoney((int) CurrentDamageData.TotalDamage);

                    if(CurrentDamageData.TargetedEnemy.Health <= 0f)
                    {
                        EnqueueEnemyToRemove(CurrentDamageData.TargetedEnemy);
                    }
                }
            }

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

    public static void EnqueueDamageData(EnemyDamageData damagedata)
    {
        DamageData.Enqueue(damagedata);
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

public struct EnemyDamageData
{
    public EnemyDamageData(Enemy target, float damage, float resistance)
    {
        TargetedEnemy = target;
        TotalDamage = damage;
        Resistance = resistance;
    }
    public Enemy TargetedEnemy;
    public float TotalDamage;
    public float Resistance;
}


public struct MoveEnemiesJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> NodePositions;

    [NativeDisableParallelForRestriction]
    public NativeArray<int> NodeIndex;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> EnemySpeed;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        if(NodeIndex[index] < NodePositions.Length)
        {
            Vector3 PositionToMoveTo = NodePositions[NodeIndex[index]];

            transform.position = Vector3.MoveTowards(transform.position, PositionToMoveTo, EnemySpeed[index] * deltaTime);

            if(transform.position == PositionToMoveTo)
            {
                NodeIndex[index]++;
            }

        }
    }
}