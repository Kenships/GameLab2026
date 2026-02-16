using System;
using UnityEngine;

public class RecordDiscShooter : MonoBehaviour
{
  
    [SerializeField] private GameObject recordDiscPrefab;
    [SerializeField] private Transform spawnPoint;

    
    [SerializeField] private Vector3 shootDirection = Vector3.forward;
    [SerializeField] private float shootSpeed = 10f;
    [SerializeField] private float timeBetweenShots = 1f;
    
    
    // [SerializeField] private float bulletScale = 1f;
    [SerializeField] private float bulletLifetime = 5f;

    private float shootTimer;
  
    
    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = timeBetweenShots;
        }
    }

    private void Shoot()
    {
        GameObject disc = Instantiate(
            recordDiscPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
        
      
        //disc.transform.localScale = Vector3.one * bulletScale;
        
        Rigidbody rb = disc.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = disc.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;

        // Normalize the direction so speed is consistent regardless of direction vector magnitude
        Vector3 direction = shootDirection.normalized;

        // Apply velocity
        rb.linearVelocity = direction * shootSpeed;

        // Destroy the bullet after its lifetime expires so we don't flood the scene
        Destroy(disc, bulletLifetime);
    }
}