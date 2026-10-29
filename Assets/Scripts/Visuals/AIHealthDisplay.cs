using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AIHealthDisplay : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameManager.AIHealthChanged += SetHealth;
    }
    
    void SetHealth(int health)
    {
        animator.SetInteger("Health", health);
    }
}
