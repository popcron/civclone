using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasButtonChangeLayer : CanvasButton {

    public string layer;
    protected override void OnClick()
    {
        base.OnClick();
        CanvasManager.Layer = layer;
    }
}
