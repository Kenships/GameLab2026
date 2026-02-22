using UnityEngine;

public class VHSManager : MonoBehaviour
{
    private IDamageable healthScript;
    public int VhsMaxHealth;


    void Start()
    {
        healthScript = GetComponent<IDamageable>();
        healthScript.Initialize(VhsMaxHealth);
    }

}
