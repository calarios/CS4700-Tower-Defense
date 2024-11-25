using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageMethod
{
    public void DamageTick(Enemy Target);
    public void Init(float Damage, float firerate);
}

public class StandardDamage : MonoBehaviour, IDamageMethod
{
    [SerializeField] public AudioClip FireSFX;
    private float Damage;
    private float Firerate;
    private float Delay;

    [SerializeField] public AudioClip MagicSFX;

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
            if(Delay > 0f)
            {
                Delay -= Time.deltaTime;
                return;
            }

            GameLoopManager.EnqueueDamageData(new EnemyDamageData(Target, Damage, Target.DamageResistance));
            SoundManager.Instance.Play(FireSFX);

            

            Delay = 1f/Firerate;
        }

        
    }
} 