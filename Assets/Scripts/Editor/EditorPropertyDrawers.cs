using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HexCoordinate))]
public class HexCoordinatesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HexCoordinate coordinates = new HexCoordinate(property.FindPropertyRelative("x").intValue, property.FindPropertyRelative("z").intValue);
        EditorGUI.LabelField(position, label.text, coordinates.ToString());
    }
}

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 range = property.vector2Value;
            float min = range.x;
            float max = range.y;
            MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;
            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(position, label, ref min, ref max, attr.min, attr.max);
            if (EditorGUI.EndChangeCheck())
            {
                range.x = min;
                range.y = max;
                property.vector2Value = range;
            }
        }
        else
        {
            EditorGUI.LabelField(position, label, "Use only with Vector2");
        }
    }
}

[CustomPropertyDrawer(typeof(ConfigSetting))]
public class ConfigSettingDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.hasMultipleDifferentValues)
        {
            EditorGUI.LabelField(position, label.text, "---");
            return;
        }

        EditorGUI.LabelField(position, label);
        SerializedProperty configName = property.FindPropertyRelative("config");
        SerializedProperty variableName = property.FindPropertyRelative("variable");

        List<Type> types = ConfigManager.ConfigTypes;

        List<string> options = new List<string>();
        int selectedIndex = 0;
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i].Name == configName.stringValue)
            {
                selectedIndex = options.Count;
            }

            options.Add(types[i].Name);
        }

        int newIndex = EditorGUI.Popup(new Rect(position.x, position.y + 15, position.width, 15), "    Config", selectedIndex, options.ToArray());
        configName.stringValue = options[newIndex];

        Type configType = ConfigManager.GetConfig(configName.stringValue);
        if (configType != null)
        {
            System.Reflection.FieldInfo[] fields = configType.GetFields();
            options.Clear();
            selectedIndex = 0;
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name != "name")
                {
                    if (fields[i].Name == variableName.stringValue)
                    {
                        selectedIndex = options.Count;
                    }

                    options.Add(fields[i].Name);
                }
            }
            
            if(options.Count == 0)
            {
                EditorGUI.LabelField(new Rect(position.x, position.y + 30, position.width, 15), "    Variable", "null", EditorStyles.miniLabel);
                variableName.stringValue = "";
            }
            else
            {
                newIndex = EditorGUI.Popup(new Rect(position.x, position.y + 30, position.width, 15), "    Variable", selectedIndex, options.ToArray());
                variableName.stringValue = options[newIndex];
            }
        }
        else
        {
            EditorGUI.LabelField(new Rect(position.x, position.y + 30, position.width, 15), "    Variable", "null", EditorStyles.miniLabel);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 30;
    }
}
