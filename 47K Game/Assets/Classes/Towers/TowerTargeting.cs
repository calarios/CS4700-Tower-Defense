using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TowerTargeting
{
    public enum TargetType
    {
        First,
        Last,
        Close
    }

    public static Enemy GetTarget(TowerBehavior CurrentTower, TargetType TargetMethod)
    {
        Collider[] EnemiesInRange = Physics.OverlapSphere(CurrentTower.transform.position, CurrentTower.Range, CurrentTower.EnemiesLayer);
        if (EnemiesInRange.Length == 0) return null;

        NativeArray<EnemyData> EnemiesToCalculate = new NativeArray<EnemyData>(EnemiesInRange.Length, Allocator.TempJob);
        NativeArray<Vector3> NodePositions = new NativeArray<Vector3>(GameLoopManager.NodePositions, Allocator.TempJob);
        NativeArray<float> NodeDistances = new NativeArray<float>(GameLoopManager.NodeDistances, Allocator.TempJob);
        NativeArray<float> Results = new NativeArray<float>(EnemiesInRange.Length, Allocator.TempJob);
        NativeArray<int> BestIndex = new NativeArray<int>(1, Allocator.TempJob);

        for (int i = 0; i < EnemiesToCalculate.Length; i++)
        {
            Enemy CurrentEnemy = EnemiesInRange[i].transform.parent.GetComponent<Enemy>();

            // Add null check for CurrentEnemy
            if (CurrentEnemy == null || !CurrentEnemy.gameObject.activeInHierarchy) 
                continue;

            int EnemyIndexList = EntitySummoner.EnemiesInGame.FindIndex(x => x == CurrentEnemy);

            EnemiesToCalculate[i] = new EnemyData(
                CurrentEnemy.transform.position,
                Mathf.Clamp(CurrentEnemy.NodeIndex, 0, GameLoopManager.NodePositions.Length - 1),
                CurrentEnemy.Health,
                EnemyIndexList
            );
        }

        SearchForEnemy EnemySearchJob = new SearchForEnemy
        {
            _EnemiesToCalculate = EnemiesToCalculate,
            _NodePositions = NodePositions,
            _NodeDistances = NodeDistances,
            Results = Results,
            BestIndex = BestIndex,
            TowerPosition = CurrentTower.transform.position,
            TargetingType = (int)TargetMethod
        };

        JobHandle SearchJobHandle = EnemySearchJob.Schedule(EnemiesToCalculate.Length, default(JobHandle));
        SearchJobHandle.Complete();

        int TargetEnemyIndex = BestIndex[0] >= 0 ? EnemiesToCalculate[BestIndex[0]].EnemyIndex : -1;

        // Dispose of NativeArrays
        EnemiesToCalculate.Dispose();
        NodePositions.Dispose();
        NodeDistances.Dispose();
        Results.Dispose();
        BestIndex.Dispose();

        return TargetEnemyIndex >= 0 ? EntitySummoner.EnemiesInGame[TargetEnemyIndex] : null;
    }

    struct EnemyData
    {
        public EnemyData(Vector3 position, int nodeIndex, float health, int enemyIndex)
        {
            EnemyPosition = position;
            NodeIndex = nodeIndex;
            EnemyIndex = enemyIndex;
            Health = health;
        }

        public Vector3 EnemyPosition;
        public int NodeIndex;
        public int EnemyIndex;
        public float Health;
    }

    struct SearchForEnemy : IJobFor
    {
        [ReadOnly] public NativeArray<EnemyData> _EnemiesToCalculate;
        [ReadOnly] public NativeArray<Vector3> _NodePositions;
        [ReadOnly] public NativeArray<float> _NodeDistances;
        public NativeArray<float> Results;
        public NativeArray<int> BestIndex;
        [ReadOnly] public Vector3 TowerPosition;
        [ReadOnly] public int TargetingType;

        public void Execute(int index)
        {
            float Metric = TargetingType == 2
                ? Vector3.Distance(TowerPosition, _EnemiesToCalculate[index].EnemyPosition)
                : GetDistanceToEnd(_EnemiesToCalculate[index]);

            Results[index] = Metric;

            if (ShouldUpdate(Metric, Results[BestIndex[0]], index))
            {
                BestIndex[0] = index;
            }
        }

        private float GetDistanceToEnd(EnemyData enemy)
        {
            if (enemy.NodeIndex < 0 || enemy.NodeIndex >= _NodePositions.Length)
            {
                Debug.LogError($"Invalid NodeIndex: {enemy.NodeIndex}");
                return float.MaxValue; // Or another default value
            }

            float totalDistance = Vector3.Distance(enemy.EnemyPosition, _NodePositions[enemy.NodeIndex]);

            for (int i = enemy.NodeIndex; i < _NodeDistances.Length; i++)
            {
                totalDistance += _NodeDistances[i];
            }

            return totalDistance;
        }


        private bool ShouldUpdate(float currentMetric, float bestMetric, int index)
        {
            if (BestIndex[0] == -1) return true;

            return TargetingType switch
            {
                0 => currentMetric < bestMetric, // First
                1 => currentMetric > bestMetric, // Last
                2 => currentMetric < bestMetric, // Close
                _ => false,
            };
        }
    }
}
