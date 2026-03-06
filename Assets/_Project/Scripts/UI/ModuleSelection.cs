using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.Core.InputManagement.Interfaces;
using Obvious.Soap;
using Sisus.Init;

public class ModuleSelection : MonoBehaviour<INESActionReader>
{
    [Header("References")]
    [SerializeField] private ModuleSectionUIComponent[] textmodules;
    
    [SerializeField] private ScriptableEventGameObject spawnModuleEvent;
    [SerializeField] private List<Module> modules;
    
    [Header("UI Settings")]
    [SerializeField] private Vector3 selectedScale = new(0.4f, 1f, 1.1f);
    [SerializeField] private Vector3 unselectedScale = new(0.3f, 0.9f, 1f);
    
    private INESActionReader _nesActionReader;
    private SceneUnloader _sceneUnloader;

    protected override void Init(INESActionReader actionReader)
    {
        _nesActionReader = actionReader;
    }

    //Zero Indexed
    private int _selectedModulenumber = 1;

    private void Start()
    {
        _sceneUnloader = GetComponent<SceneUnloader>();
        
        //TODO migrate to loading modules manually
        //Module[] modulesArray = Resources.LoadAll<Module>("Modules");

        //modules.AddRange(modulesArray);

        SetModuleSelections();
        SelectingModule();

        _nesActionReader.OnDPadInput += HandlePad;
        _nesActionReader.OnTapInteract += AquireModule;
    }

    private void HandlePad(Vector2 direction)
    {
        HandleselectedModulenumb((int)direction.x);
        SelectingModule();
    }

    private void HandleselectedModulenumb(int delta)
    {
        Debug.Log($"Delta: {modules.Count}");
        
        _selectedModulenumber = (_selectedModulenumber + delta) % textmodules.Length;

        if (_selectedModulenumber < 0)
        {
            _selectedModulenumber += textmodules.Length;
        }
    }


    private Module ReturnRandandRemove()
    {
        if (modules.Count == 0) return null;
        
        int rand = Random.Range(0, modules.Count);
        Module toReturn = modules[rand];
        modules.Remove(modules[rand]);

        return toReturn;
    }

    private void SetModuleSelections()
    {
        foreach (ModuleSectionUIComponent moduleUI in textmodules)
        {
            SetModuleUI(moduleUI);
        }
    }

    private void SetModuleUI(ModuleSectionUIComponent moduleUI)
    {
        Module selectedModule = ReturnRandandRemove();
        moduleUI.selectedModule = selectedModule;
        moduleUI.name.text = selectedModule.name;
        moduleUI.description.text = selectedModule.description;
    }

    
    private void SelectingModule()
    {
        for (int i = 0; i < textmodules.Length; i++)
        {
            if (i == _selectedModulenumber)
            {
                textmodules[i].transform.localScale = selectedScale;
                textmodules[i].Button.SetActive(true);
                textmodules[i].transform.SetAsLastSibling();
            }
            else
            {
                textmodules[i].transform.localScale = unselectedScale;
                textmodules[i].Button.SetActive(false);
            }
        }
    }

    private void AquireModule()
    {
        Module selectedModule = textmodules[_selectedModulenumber].selectedModule;

        spawnModuleEvent.Raise(selectedModule.gameObject);
        Time.timeScale = 1f;
        _sceneUnloader.UnloadScene();
    }
}
