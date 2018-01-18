using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasButtonPlay : CanvasButton {

    protected override void OnClick()
    {
        base.OnClick();
        GameManager.Play();
    }
}
