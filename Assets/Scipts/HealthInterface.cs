using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable { void Damage(DamageInfo damageInfo); }

public interface IHealable {void Heal(int healAmount); int GetHealth(); int GetMaxHealth();  }

public class DamageInfo
{
    public int damageAmount = 0;
    public Vector2 knockback = Vector2.zero;
    public bool ignoreIFrames = false;

    public DamageInfo(int damageAmount, Vector2 knockback, bool ignoreIFrames)
    {
        this.damageAmount = damageAmount;
        this.knockback = knockback;
        this.ignoreIFrames = ignoreIFrames;
    }
}