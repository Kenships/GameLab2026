using _Project.Scripts.Core.ApplicationQuit;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.Core.InputManagement.Interfaces;
using Sisus.Init;
using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Multiplayer;

public class ModuleSelection : MonoBehaviour<INESActionReader>
{

    [SerializeField] private ModuleSectionUIComponent textmodule1;
    [SerializeField] private ModuleSectionUIComponent textmodule2;
    [SerializeField] private ModuleSectionUIComponent textmodule3;

    [SerializeField] private Transform SpawnPositionTarget;

    private INESActionReader _nesActionReader;

    public List<Module> modules = new List<Module>();

    private SceneUnloader _sceneUnloader;

    protected override void Init(INESActionReader actionReader)
    {
        _nesActionReader = actionReader;
    }

    private int selectedModulenumb = 2;


    void Start()
    {
        _sceneUnloader = GetComponent<SceneUnloader>();

        Module[] modulesArray = Resources.LoadAll<Module>("modules");

        foreach(Module module in modulesArray)
        {
            modules.Add(module);
        }

        SetModuleSelections();
        SelectingModule();

        _nesActionReader.OnDPadInput += HandlePad;
        _nesActionReader.OnTapInteract += AquireModule;
    }

    private void HandlePad(Vector2 Direction)
    {
        HandleselectedModulenumb((int)Direction.x);
        SelectingModule();
    }

    private void HandleselectedModulenumb(int mod)
    {
        int temp = selectedModulenumb;
        temp += mod;
        if (temp < 1) temp = 3;
        else if (temp > 3) temp = 1;
        selectedModulenumb = temp;
    }


    private Module ReturnRandandRemove()
    {
        int rand = Random.Range(0, modules.Count);
        Module toReturn = modules[rand];
        modules.Remove(modules[rand]);

        return toReturn;
    }

    private void SetModuleSelections()
    {
        SetModuleUI(textmodule1);
        SetModuleUI(textmodule2);
        SetModuleUI(textmodule3);
    }

    private void SetModuleUI(ModuleSectionUIComponent ModuleUI)
    {
        Module SelectedModule = ReturnRandandRemove();
        ModuleUI.selectedModule = SelectedModule;
        ModuleUI.name.text = SelectedModule.name;
        ModuleUI.description.text = SelectedModule.description;
    }

    
    private void SelectingModule()
    {
        if (textmodule1.selectionnumber == selectedModulenumb)
        {
            textmodule1.transform.localScale = new Vector3(0.4f, 1f, 1.1f);
            textmodule1.Button.SetActive(true);
            textmodule1.transform.SetAsLastSibling();
        }
        else
        {
            textmodule1.transform.localScale = new Vector3(0.3f, 0.9f, 1f);
            textmodule1.Button.SetActive(false);
        }

        if (textmodule2.selectionnumber == selectedModulenumb)
        {
            textmodule2.transform.localScale = new Vector3(0.4f, 1f, 1.1f);
            textmodule2.Button.SetActive(true);
            textmodule2.transform.SetAsLastSibling();
        }
        else
        {
            textmodule2.transform.localScale = new Vector3(0.3f, 0.9f, 1f);
            textmodule2.Button.SetActive(false);
        }

        if (textmodule3.selectionnumber == selectedModulenumb)
        {
            textmodule3.transform.localScale = new Vector3(0.4f, 1f, 1.1f);
            textmodule3.Button.SetActive(true);
            textmodule3.transform.SetAsLastSibling();
        }
        else
        {
            textmodule3.transform.localScale = new Vector3(0.3f, 0.9f, 1f);
            textmodule3.Button.SetActive(false);
        }
    }

    private void AquireModule()
    {
        Module temp;
        switch (selectedModulenumb)
        {
            case 1:
                temp = textmodule1.selectedModule;
                break;
            case 2:
                temp = textmodule2.selectedModule;
                break;
            case 3:
                temp = textmodule3.selectedModule;
                break;
            default:
                temp = textmodule2.selectedModule;
            break;

        }

        Instantiate(temp, SpawnPositionTarget.position, Quaternion.identity);
        Time.timeScale = 1f;
        _sceneUnloader.UnloadScene();
    }
}
