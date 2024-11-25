using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerDamage : MonoBehaviour, IDamageMethod
{
    [SerializeField] private Transform LazerPivot;
    [SerializeField] private LineRenderer LazerRenderer;
    [SerializeField] public AudioClip LazerSFX;

    private float Damage;
    private float Firerate;
    private float Delay;

    public void Init(float Damage, float Firerate)
    {
        this.Damage = Damage;
        this.Firerate = Firerate;
        Delay = 1f / Firerate;
    }

    public void DamageTick(Enemy Target)
    {
        
        if(Target)
        {
            LazerRenderer.enabled = true;
            LazerRenderer.SetPosition(0, LazerPivot.position);
            LazerRenderer.SetPosition(1, Target.RootPart.position);
            

            if(Delay > 0f)
            {
                Delay -= Time.deltaTime;
                return;
            }

            
            GameLoopManager.EnqueueDamageData(new EnemyDamageData(Target, Damage, Target.DamageResistance));
            SoundManager.Instance.Play(LazerSFX);
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
            Delay = 1f/Firerate;
            return;
        }
        
        LazerRenderer.enabled = false;        
    }
}
