using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasObjectPanel : MonoBehaviour
{
    public bool alwaysShowPanel = false;
    public bool alwaysShowStats = false;

    public Vector3 offset = new Vector3(0, 1, 0);

    private Canvas canvas;
    private Image panel;
    private Text nameText;
    private Text statsText;

    private UnitObject unitObject;
    private ImprovementObject improvementObject;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        panel = canvas.GetComponentInChildren<Image>();

        nameText = panel.transform.Find("Name").GetComponent<Text>();
        statsText = panel.transform.Find("Stats").GetComponent<Text>();

        unitObject = GetComponent<UnitObject>();
        improvementObject = GetComponent<ImprovementObject>();
    }

    private void Update()
    {
        Vector3 position = Camera.main.WorldToScreenPoint(transform.position + offset);
        panel.rectTransform.anchoredPosition = position;

        if(unitObject)
        {
            if(Config.Temporary.selectedUnit == unitObject || alwaysShowPanel)
            {
                panel.gameObject.SetActive(true);
                nameText.text = unitObject.Unit.name;

                if (Config.Temporary.selectedUnit == unitObject || alwaysShowStats)
                {
                    statsText.text = "";
                }
                else
                {
                    statsText.text = "";
                }
            }
            else
            {
                panel.gameObject.SetActive(false);
            }
        }
        else if(improvementObject)
        {
            if (Config.Temporary.selectedImprovement == improvementObject || alwaysShowPanel)
            {
                panel.gameObject.SetActive(true);
                nameText.text = improvementObject.GetName();

                if (Config.Temporary.selectedImprovement == improvementObject || alwaysShowStats)
                {
                    statsText.text = improvementObject.GetStats();
                }
                else
                {
                    statsText.text = "";
                }
            }
            else
            {
                panel.gameObject.SetActive(false);
            }
        }
        else
        {
            nameText.text = "Undefined";
            statsText.text = "";
        }
    }
}
