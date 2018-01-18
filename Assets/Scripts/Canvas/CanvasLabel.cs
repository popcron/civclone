using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLabel : MonoBehaviour {

    public string prefix = "";
    public string suffix = "";
    public ConfigSetting configSetting;

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (configSetting.Field.Value.ToString() != text.text)
        {
            text.text = prefix + configSetting.Field.Value.ToString() + suffix;
        }
    }
}
