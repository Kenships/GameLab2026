using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.GridObjects.Interactables;
using _Project.Scripts.GridObjects.Interface;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : PickupObjectBase, ITimeControllable
{
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private float emissionRateToAngleRatio = 14f;
    [SerializeField] private float angleMultiplier = 0.6f;
    [SerializeField] private float radiusMultiplier = 1.2f;

    [Header("Flamethrower Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float timesOfDamagePerSec = 4f;

    private RangeDetector rangeDetector; // rangeType is sector
    private List<Transform> enemies;

    private bool isDamagingEnemies = false;
    public bool IsWinding { get; set; }

    private void Start()
    {
        enemies = new List<Transform>();
        rangeDetector = GetComponent<RangeDetector>();
        if (!rangeDetector)
        {
            Debug.Log("missing rangeDetector");
            return;
        }
        if (!particle)
        {
            Debug.Log("missing particle");
            return;
        }

        UpdateAngle(rangeDetector.angle * angleMultiplier);
        UpdateDistance(rangeDetector.radius * radiusMultiplier);
    }
    private void UpdateAngle(float angle)
    {
        var shape = particle.shape;
        shape.angle = angle;

        var emission = particle.emission;
        emission.rateOverTime = angle * emissionRateToAngleRatio;
    }

    private void UpdateDistance(float distance)
    {
        var main = particle.main;

        float currentLifetime = main.startLifetime.constant;

        main.startSpeed = distance / currentLifetime;
    }
    private void Update()
    {
        enemies = rangeDetector.GetTransformsInRange();
        if (enemies.Count > 0 && !isDamagingEnemies)
        {
            isDamagingEnemies = true;
            StartCoroutine(DamageEnemiesCycle());
        }
    }
    private IEnumerator DamageEnemiesCycle()
    {
        while (true)
        {
            enemies = rangeDetector.GetTransformsInRange();

            if (enemies.Count <= 0)
            {
                isDamagingEnemies = false;
                yield break;
            }

            foreach(Transform t in enemies)
            {
                t.GetComponent<IDamageable>().Damage(damage);
            }

            yield return new WaitForSeconds(1f / timesOfDamagePerSec);
        }
    }

    public void CancelFastForward()
    {

    }

    public void CancelRewind()
    {

    }

    public void FastForward()
    {

    }

    public void Rewind()
    {

    }
}
