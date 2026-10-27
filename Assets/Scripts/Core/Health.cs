using System;
using UnityEngine;

/// <summary>
/// Lebenspunkte für Truppen und Türme (gemeinsame Basis, Dev 1)
/// </summary>
public class Health : MonoBehaviour
{
    public Team team;
    public float maxHealth = 100f;
    public float Current { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<Health> OnDied;
    public event Action<float, float> OnHealthChanged;   

    public void Init(Team t, float max)
    {
        team = t;
        maxHealth = max;
        Current = max;
        IsDead = false;
        OnHealthChanged?.Invoke(Current, maxHealth);
    }

    void Awake()
    {
        if (Current <= 0f && IsDead == false)
        {
            Current = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead)
        {
            return;
        }

        Current = Mathf.Max(0f, Current - amount);
        OnHealthChanged?.Invoke(Current, maxHealth);

        if (Current <= 0f)
        {
            IsDead = true;
            OnDied?.Invoke(this);
        }
    }
}