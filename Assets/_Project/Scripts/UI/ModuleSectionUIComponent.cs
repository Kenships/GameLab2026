using _Project.Scripts.Core.Modules.Base_Class;
using TMPro;
using UnityEngine;

public class ModuleSectionUIComponent : MonoBehaviour
{
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public GameObject Button;
    public int selectionnumber;

    public Transform transform;

    public Module selectedModule;


    void Start()
    {
        transform = gameObject.transform;
    }
}
