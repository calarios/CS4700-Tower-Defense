using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehavior : MonoBehaviour
{
    public LayerMask EnemiesLayer;

    public Enemy Target;
    public Transform TowerPivot;
    public float Damage;
    public float Firerate;
    public float Range;

    private float Delay;

    private IDamageMethod CurrentDamageMethodClass;

    // Start is called before the first frame update
    void Start()
    {
        CurrentDamageMethodClass = GetComponent<IDamageMethod>();

        if(CurrentDamageMethodClass == null)
        {
            Debug.LogError("TOWERS: No damage class attatched to given tower!");
        }
        else
        {
            CurrentDamageMethodClass.Init(Damage, Firerate);
        }

        Delay = 1/Firerate;
    }

    public void Tick()
    {
        CurrentDamageMethodClass.DamageTick(Target);

        if(Target != null)
        {
            TowerPivot.transform.rotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        }
    }
}
