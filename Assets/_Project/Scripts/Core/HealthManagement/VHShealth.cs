using _Project.Scripts.Core.HealthManagement;
using UnityEngine;
using UnityEngine.UI;

public class VHSHealth : Health
{
    // Maybe abstract out the hpBar
    public Slider hpBar;
    public override void Damage(int damage)
    {
        base.Damage(damage);


        float displayValue;

        if ((currentHealth / _maxHealth) < 0)
        {
            displayValue = 0;
        }else
        {
            displayValue = (float)currentHealth / _maxHealth;
        }

        hpBar.value = displayValue;
    }
    public override void Kill()
    {
        Debug.Log("Enemies won (VHS health bellow 0)");
    }
}
