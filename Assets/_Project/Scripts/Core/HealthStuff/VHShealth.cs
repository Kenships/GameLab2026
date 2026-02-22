using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class VHSHealth : Health
{
    public Slider hpBar;
    public override void EffectHealth(int healthChange)
    {
        base.EffectHealth(healthChange);


        float displayValue;

        if ((currentHealth / maxHealth) < 0)
        {
            displayValue = 0;
        }else
        {
            displayValue = (float)currentHealth / maxHealth;
        }

        hpBar.value = displayValue;
    }
    public override void Kill()
    {
        Debug.Log("Enemies won (VHS health bellow 0)");
    }
}
