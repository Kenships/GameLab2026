using System;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Util.ExtensionMethods;
using UnityEngine;

public class VHSManager : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private HealthBar healthBar;
    
    [Header("VHS Settings")]
    [SerializeField] private int vhsMaxHealth;

    private Health _myHealth;
    void Awake()
    {
        _myHealth = gameObject.GetOrAdd<Health>();
        _myHealth.Initialize(vhsMaxHealth);
        healthBar.Initialize(_myHealth);
    }

    private void Start()
    {
        _myHealth.OnDeath += Kill;
    }

    public void Damage(float damage)
    {
        _myHealth.AddToHealth(-damage);
    }
    
    private void Kill()
    {
        Destroy(gameObject);
    }
}
