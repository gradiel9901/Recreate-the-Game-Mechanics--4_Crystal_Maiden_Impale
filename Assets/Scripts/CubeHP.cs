using System.Collections;
using UnityEngine;

public class CubeHP : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    [HideInInspector]
    public int currentHealth;

    [SerializeField]
    private float deathDelay = 2.0f;

    private bool isAirborne = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0 && !isAirborne)
        {
            isAirborne = true;
            StartCoroutine(HandleDeathWithDelay());
        }
    }

    private IEnumerator HandleDeathWithDelay()
    {
        yield return new WaitForSeconds(deathDelay);

        Destroy(gameObject);
    }
}
