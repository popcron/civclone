using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInput : MonoBehaviour
{
    public ConfigSetting configSetting;

    InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<InputField>();

        inputField.onValueChanged.AddListener(Refresh);
        inputField.text = configSetting.Field.Value.ToString();
    }

    private void Refresh(string newValue)
    {
        if(configSetting.Field.Type == typeof(string))
        {
            configSetting.Field.Value = newValue;
        }
        else if (configSetting.Field.Type == typeof(float))
        {
            configSetting.Field.Value = float.Parse(newValue);
        }
        if (configSetting.Field.Type == typeof(int))
        {
            configSetting.Field.Value = int.Parse(newValue);
        }
        if (configSetting.Field.Type == typeof(bool))
        {
            configSetting.Field.Value = newValue == "true" || newValue == "1";
        }
    }
}
