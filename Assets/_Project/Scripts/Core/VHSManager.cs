using _Project.Scripts.Core.HealthManagement;
using UnityEngine;

public class VHSManager : MonoBehaviour
{
    [SerializeField] private int vhsMaxHealth;


    void Start()
    {
        var healthScript = GetComponent<Health>();
        healthScript.Initialize(vhsMaxHealth);
    }

}
