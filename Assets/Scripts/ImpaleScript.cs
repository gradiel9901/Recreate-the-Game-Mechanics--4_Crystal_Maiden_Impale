using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpaleScript : MonoBehaviour
{
    public bool Debug;
    public GameObject impaleHitFX;
    public int maximumLength;
    public float separation;
    public float spawnDelay;
    public float damageDelay;
    public float height;
    public float radius;
    public float force;
    public float yOffset;
    public LayerMask layerMask;

    [SerializeField]
    private int damageAmount = 50;

    [SerializeField]
    private float cubeDestroyDelay = 2.0f;

    [HideInInspector]
    public GameObject fxParent;
    [HideInInspector]
    public int currentLength;
    private bool isLast = false, hasSpawnedNext = false, hasDamaged;
    private ParticleSystem PS;
    private float spawnDelayTimer, damageDelayTimer;

    private void Start()
    {
        if (currentLength == 0) { fxParent = gameObject; }

        PS = GetComponent<ParticleSystem>();

        spawnDelayTimer = spawnDelay;
        damageDelayTimer = damageDelay;
    }

    private void Update()
    {
        if (spawnDelayTimer <= 0 && !hasSpawnedNext) { CreateImpaleOBJ(); }

        if (spawnDelayTimer > 0) { spawnDelayTimer -= Time.deltaTime; }

        if (damageDelayTimer > 0) { damageDelayTimer -= Time.deltaTime; }

        if (damageDelayTimer <= 0 && !hasDamaged) { AOEDamage(); }

        if (!PS.isPlaying && isLast) { Destroy(fxParent); }
    }

    void CreateImpaleOBJ()
    {
        if (currentLength < maximumLength)
        {
            var raycastPosition = transform.position + transform.forward * separation;
            raycastPosition.y += height;
            RaycastHit hit;
            if (Physics.Raycast(raycastPosition, Vector3.down, out hit, height + 1, layerMask))
            {
                if (hit.transform != transform)
                {
                    var spawnLoc = hit.point;
                    spawnLoc.y += yOffset;
                    hasSpawnedNext = true;
                    var obj = Instantiate(gameObject, transform);
                    obj.transform.position = spawnLoc;
                    obj.transform.rotation = transform.rotation;
                    var impale = obj.GetComponent<ImpaleObject>();
                    impale.currentLength = currentLength + 1;
                    impale.maximumLength = maximumLength;
                    impale.fxParent = fxParent;
                }
                else { isLast = true; } 
            }
            else { isLast = true; }                
        }
        else { isLast = true; }                   
    }

    void AOEDamage()
    {
        hasDamaged = true;

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in objectsInRange)
        {
            Rigidbody enemy = col.GetComponent<Rigidbody>();

            if (enemy != null)
            {
                enemy.AddForce(Vector3.up * force, ForceMode.VelocityChange);

                var fx = Instantiate(impaleHitFX, enemy.transform.position, Quaternion.identity);

                Destroy(fx, 2);

                CubeHP cubeHP = col.GetComponent<CubeHP>();
                if (cubeHP != null)
                {
                    cubeHP.TakeDamage(damageAmount);

                    StartCoroutine(DelayedDestroyCube(cubeHP));
                }
            }
        }
    }

    IEnumerator DelayedDestroyCube(CubeHP cubeHP)
    {
        yield return new WaitForSeconds(cubeDestroyDelay);

        if (cubeHP.currentHealth <= 0)
        {
            Destroy(cubeHP.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if (Debug)
        {
            if (hasDamaged)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, radius);
            }
        }
    }
}

public class ImpaleObject : MonoBehaviour
{
    public int currentLength;
    public int maximumLength;
    public GameObject fxParent;
}
