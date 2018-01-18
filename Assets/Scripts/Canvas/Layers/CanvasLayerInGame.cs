using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLayerInGame : CanvasLayer
{
    private static CanvasLayerInGame instance;

    public GameObject unitPanel;
    public CanvasButtonAction unitAction;
    public Text unitPanelName;

    public GameObject cityPanel;
    public CanvasButtonAction cityAction;
    public Text cityPanelName;

    private int selectedIndex = 0;
    private WorldTile lastSelectedTile;

    private void OnEnable()
    {
        instance = this;
    }

    private void Awake()
    {
        instance = this;

        unitPanel.SetActive(false);
        unitAction.gameObject.SetActive(false);

        cityPanel.SetActive(false);
        cityAction.gameObject.SetActive(false);
    }
    
    public static void RefreshSelectedTile()
    {
        if (!instance) instance = FindObjectOfType<CanvasLayerInGame>();

        if(!Config.Temporary.selectedTile) return;
        
        if (instance.lastSelectedTile != Config.Temporary.selectedTile)
        {
            instance.selectedIndex = 0;
            instance.lastSelectedTile = Config.Temporary.selectedTile;
        }
        else
        {
            instance.selectedIndex++;
            if(instance.selectedIndex >= Config.Temporary.selectedTile.Units.Count + Config.Temporary.selectedTile.Improvements.Count)
            {
                instance.selectedIndex = 0;
            }
        }

        if(Config.Temporary.selectedTile.Units.Count + Config.Temporary.selectedTile.Improvements.Count > 0)
        {
            if(Config.Temporary.selectedTile.Units.Count > instance.selectedIndex)
            {
                //unit interface
                Config.Temporary.selectedImprovement = null;
                Config.Temporary.selectedUnit = Config.Temporary.selectedTile.Units[instance.selectedIndex];
                instance.unitPanel.SetActive(true);
                instance.cityPanel.SetActive(false);
                instance.unitPanelName.text = Config.Temporary.selectedUnit.Unit.name;

                CanvasButtonAction[] buttons = instance.unitPanel.transform.GetComponentsInChildren<CanvasButtonAction>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i] != instance.unitAction)
                    {
                        Destroy(buttons[i].gameObject);
                    }
                }

                for (int i = 0; i < Config.Temporary.selectedUnit.Unit.tasks.Count; i++)
                {
                    CanvasButtonAction newButton = Instantiate(instance.unitAction.gameObject).GetComponent<CanvasButtonAction>();
                    newButton.transform.SetParent(instance.unitPanel.transform);
                    newButton.gameObject.SetActive(true);

                    newButton.RectTransform.anchoredPosition = new Vector2(0, i * 30);
                    newButton.RectTransform.sizeDelta = new Vector2(0, 30);
                    newButton.Initialize(Config.Temporary.selectedUnit.Unit.tasks[i], Config.Temporary.selectedUnit);
                }
            }
            else if (Config.Temporary.selectedTile.Improvements.Count > instance.selectedIndex - Config.Temporary.selectedTile.Units.Count)
            {
                Config.Temporary.selectedUnit = null;
                Config.Temporary.selectedImprovement = Config.Temporary.selectedTile.Improvements[instance.selectedIndex - Config.Temporary.selectedTile.Units.Count];
                bool isCity = Config.Temporary.selectedImprovement is ImprovementObjectCapital || Config.Temporary.selectedImprovement is ImprovementObjectCity;

                if (isCity)
                {
                    instance.unitPanel.SetActive(false);
                    instance.cityPanel.SetActive(true);
                    instance.cityPanelName.text = ((ImprovementObjectCity)Config.Temporary.selectedImprovement).cityName;
                }
                else
                {
                    instance.unitPanel.SetActive(true);
                    instance.cityPanel.SetActive(false);
                    instance.unitPanelName.text = Config.Temporary.selectedImprovement.Improvement.name;
                }

                if (isCity)
                {
                    //city interface
                    CanvasButtonAction[] buttons = instance.cityPanel.transform.GetComponentsInChildren<CanvasButtonAction>();
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        if (buttons[i] != instance.cityAction)
                        {
                            Destroy(buttons[i].gameObject);
                        }
                    }

                    for (int i = 0; i < GameManager.Units.Count; i++)
                    {
                        CanvasButtonAction newButton = Instantiate(instance.cityAction.gameObject).GetComponent<CanvasButtonAction>();
                        newButton.transform.SetParent(instance.cityPanel.transform);
                        newButton.gameObject.SetActive(true);

                        Task.Action action = new Task.Action
                        {
                            type = Task.ActionType.ProduceUnit,
                            value = GameManager.Units[i].name
                        };

                        Task task = ScriptableObject.CreateInstance<Task>();
                        task.loseTurn = false;
                        task.friendlyName = "Build " + GameManager.Units[i].name;
                        task.actions.Add(action);

                        newButton.RectTransform.anchoredPosition = new Vector2(0, i * -30);
                        newButton.RectTransform.sizeDelta = new Vector2(0, 30);
                        newButton.Initialize(task, Config.Temporary.selectedImprovement);
                    }
                }
                else
                {
                    //improvement interface
                    CanvasButtonAction[] buttons = instance.unitPanel.transform.GetComponentsInChildren<CanvasButtonAction>();
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        if (buttons[i] != instance.unitAction)
                        {
                            Destroy(buttons[i].gameObject);
                        }
                    }

                    for (int i = 0; i < Config.Temporary.selectedImprovement.Improvement.tasks.Count; i++)
                    {
                        CanvasButtonAction newButton = Instantiate(instance.unitAction.gameObject).GetComponent<CanvasButtonAction>();
                        newButton.transform.SetParent(instance.unitPanel.transform);
                        newButton.gameObject.SetActive(true);

                        newButton.RectTransform.anchoredPosition = new Vector2(0, i * 30);
                        newButton.RectTransform.sizeDelta = new Vector2(0, 30);
                        newButton.Initialize(Config.Temporary.selectedImprovement.Improvement.tasks[i], Config.Temporary.selectedImprovement);
                    }
                }
            }
        }
        else
        {
            Config.Temporary.selectedUnit = null;
            Config.Temporary.selectedImprovement = null;

            instance.unitPanel.SetActive(false);
            instance.cityPanel.SetActive(false);
        }
    }
}
