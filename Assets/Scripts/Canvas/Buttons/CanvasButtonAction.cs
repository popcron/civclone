using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasButtonAction : CanvasButton {

    public Task task;

    private UnitObject unitObject;
    private ImprovementObject improvementObject;

    protected override void OnClick()
    {
        base.OnClick();
        Debug.Log("Action Button Click");

        if(unitObject)
        {
            unitObject.PerformAction(task);
        }
        if (improvementObject)
        {
            improvementObject.PerformAction(task);
        }
    }

    public void Initialize(Task task, UnitObject unitObject)
    {
        this.unitObject = unitObject;
        this.task = task;
        Text = task.friendlyName;

        Button.interactable = unitObject.Moves > 0;
    }

    public void Initialize(Task task, ImprovementObject improvementObject)
    {
        this.improvementObject = improvementObject;
        this.task = task;
        Text = task.friendlyName;
    }
}
