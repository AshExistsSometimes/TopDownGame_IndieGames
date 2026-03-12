using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject heartPrefab;

    public PlayerHealth playerHealth;

    List<HealthHeart> hearts = new List<HealthHeart>();

    private void Start()
    {
        DrawHearts();
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDamaged += DrawHearts;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDamaged -= DrawHearts;
    }

    public void DrawHearts()
    {
        // 8HP = 4 Hearts
        ClearHearts();

        // Determine how many hearts to make based off of max HP
        float maxHealthRemainder = playerHealth.MaxHealth % 2;
        int heartsToMake = (int)((playerHealth.MaxHealth / 2) + maxHealthRemainder);

        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();// Make Total number of needed hearts
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(playerHealth.Health - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }
}
