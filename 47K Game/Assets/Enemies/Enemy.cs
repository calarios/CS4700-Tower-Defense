using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int NodeIndex;

    public Transform RootPart;
    public float DamageResistance = 1f;
    public float MaxHealth;
    public float Health;
    public float Speed;

    public float AttackPower;
    public int ID;
    public void Init()
    {
        Health = MaxHealth;
        transform.position = GameLoopManager.NodePositions[0];
        NodeIndex = 0;
    }
}
