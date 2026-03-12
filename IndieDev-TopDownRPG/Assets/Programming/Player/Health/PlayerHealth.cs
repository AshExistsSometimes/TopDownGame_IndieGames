using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public float Health = 6;
    public float MaxHealth = 6;

    public bool PlayerDead = false;

    public bool Invincible = false;
    public float IFrameTime = 0.3f;

    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDeath;



    private void Start()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (Invincible) { return;  }

        Invincible = true;
        Health -= damage;

        if (Health <= 0) { Die(); }

        OnPlayerDamaged?.Invoke();

        Debug.Log("Player took " + damage + "damage");

        StartCoroutine(IFrameCountdown(IFrameTime));

    }

    public void Die()
    {
        PlayerDead = true;
        OnPlayerDeath?.Invoke();


        Debug.Log("Player died");
        gameObject.SetActive(false);
    }

    private IEnumerator IFrameCountdown(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Invincible = false;
    }
}
