using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;

[Serializable]
public class ConfigSetting
{
    public string config = "null";
    public string variable = "null";

    private Type configType;

    public ConfigField Field
    {
        get
        {
            return ConfigManager.GetField(config, variable);
        }
    }
}

[Serializable]
public class Config
{
    public static ConfigTemporary Temporary
    {
        get
        {
            if (!ConfigManager.instance) ConfigManager.instance = UnityEngine.Object.FindObjectOfType<ConfigManager>();
            if (!ConfigManager.instance) return null;

            return ConfigManager.instance.configTemporary;
        }
    }

    public static ConfigControls Controls
    {
        get
        {
            if (!ConfigManager.instance) ConfigManager.instance = UnityEngine.Object.FindObjectOfType<ConfigManager>();

            return ConfigManager.instance.configControls;
        }
    }

    public static ConfigSettings Settings
    {
        get
        {
            if (!ConfigManager.instance) ConfigManager.instance = UnityEngine.Object.FindObjectOfType<ConfigManager>();
            return ConfigManager.instance.configSettings;
        }
    }

    public static ConfigGameSave GameSave
    {
        get
        {
            if (!ConfigManager.instance) ConfigManager.instance = UnityEngine.Object.FindObjectOfType<ConfigManager>();
            return ConfigManager.instance.configGameSave;
        }
        set
        {
            if (!ConfigManager.instance) ConfigManager.instance = UnityEngine.Object.FindObjectOfType<ConfigManager>();
            ConfigManager.instance.configGameSave = value;
        }
    }
}

[Serializable]
public class ConfigTemporary : Config
{
    public WorldTile selectedTile;
    public UnitObject selectedUnit;
    public ImprovementObject selectedImprovement;
}

[Serializable]
public class ConfigSettings : Config
{
    public float volume = 50f;
    public bool showWarnings = true;
    public bool showErrors = true;
    public bool showLogs = true;
}

[Serializable]
public class ConfigControls : Config
{

}

[Serializable]
public class ConfigGameSave : Config
{
    public string nation = "Popcronia";
    public string map = "EarthHuge";
    public int turn = 0;
    public int width = 160;
    public int height = 80;

    public int science = 0;
    public int sciencePerTurn = 1;

    public int culture = 0;
    public int culturePerTurn = 0;

    public int faith = 0;
    public int faithPerTurn = 0;

    public int gold = 100;
    public int goldPerTurn = 1;
}

[Serializable]
public class ConfigField
{
    private FieldInfo field;
    private object value;
    private Type valueType;

    public object Value
    {
        get
        {
            return field.GetValue(value);
        }
        set
        {
            field.SetValue(this.value, value);
        }
    }

    public bool IsEnum
    {
        get
        {
            return field.GetValue(value).GetType().IsEnum;
        }
    }

    public Type Type
    {
        get
        {
            if (valueType == null) valueType = field.FieldType;
            return valueType;
        }
    }

    public ConfigField(FieldInfo field, Type configType, string variableName, object variableValue)
    {
        this.field = field;
        this.value = variableValue;
    }
}

public class ConfigManager : MonoBehaviour {
    
    public ConfigTemporary configTemporary;
    public ConfigGameSave configGameSave;
    public ConfigControls configControls;
    public ConfigSettings configSettings;
    
    public static ConfigManager instance;

    private void OnEnable()
    {
        instance = this;
    }

    void Awake()
    {
        instance = this;
        Load();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        if (!Application.isPlaying) return;

        if (!Directory.Exists(Root + "/Configs"))
        {
            Directory.CreateDirectory(Root + "/Configs");
        }

        List<Type> types = ConfigTypes;
        FieldInfo[] fields = GetType().GetFields();
        for (int i = 0; i < types.Count; i++)
        {
            for (int f = 0; f < fields.Length; f++)
            {
                if(fields[f].FieldType == types[i] && types[i] != typeof(ConfigTemporary))
                {
                    string json = JsonUtility.ToJson(fields[f].GetValue(this), true);
                    File.WriteAllText(Root + "/Configs/" + types[i].Name + ".txt", json);

                    RefreshConfig(types[i]);
                }
            }
        }
    }

    public static void ResetAll()
    {
        if (!instance) instance = FindObjectOfType<ConfigManager>();

        if (!Directory.Exists(Root + "/Configs"))
        {
            Directory.CreateDirectory(Root + "/Configs");
        }

        List<Type> types = ConfigTypes;
        FieldInfo[] fields = instance.GetType().GetFields();
        for (int i = 0; i < types.Count; i++)
        {
            for (int f = 0; f < fields.Length; f++)
            {
                if (fields[f].FieldType == types[i] && types[i] != typeof(ConfigTemporary))
                {
                    object instance = Activator.CreateInstance(types[i]);
                    string json = JsonUtility.ToJson(instance, true);
                    File.WriteAllText(Root + "/Configs/" + types[i].Name + ".txt", json);

                    RefreshConfig(types[i]);
                }
            }
        }
    }

    public static string Root
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath + "/Game";
#else
            return Directory.GetParent(Application.dataPath).FullName;
#endif
        }
    }

    public static void Load()
    {
        if (!instance) instance = FindObjectOfType<ConfigManager>();

        if (!Directory.Exists(Root + "/Configs"))
        {
            Directory.CreateDirectory(Root + "/Configs");
        }
        
        List<Type> types = ConfigTypes;
        for (int i = 0; i < types.Count; i++)
        {
            RefreshConfig(types[i]);
        }
    }

    public static ConfigField GetField(string configName, string variableName)
    {
        Type configType = GetConfig(configName);
        return GetField(configType, variableName);
    }

    public static ConfigField GetField(Type configType, string variableName)
    {
        if (!instance) instance = FindObjectOfType<ConfigManager>();
        
        FieldInfo[] fields = instance.GetType().GetFields();
        for(int i = 0; i < fields.Length;i++)
        {
            if (fields[i].FieldType == configType)
            {
                object config = fields[i].GetValue(instance);
                fields = fields[i].FieldType.GetFields();
                i = 0;
                for (i = 0; i < fields.Length; i++)
                {
                    if (fields[i].Name == variableName)
                    {
                        ConfigField field = new ConfigField(fields[i], configType, variableName, config);

                        return field;
                    }
                }
            }
        }

        return null;
    }

    public static object GetValue(Type configType, string variableName)
    {
        if (!instance) instance = FindObjectOfType<ConfigManager>();
        
        FieldInfo[] fields = instance.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].FieldType == configType)
            {
                object config = fields[i].GetValue(instance);
                fields = fields[i].FieldType.GetFields();
                for (i = 0; i < fields.Length; i++)
                {
                    if (fields[i].Name == variableName)
                    {
                        object value = fields[i].GetValue(config);
                        return value;
                    }
                }
            }
        }

        return null;
    }

    public static object GetValue(string config, string variable)
    {
        Type configType = GetConfig(config);
        return GetValue(configType, variable);
    }

    public static void SetValue(string config, string variable, object val)
    {
        if (!instance) instance = FindObjectOfType<ConfigManager>();

        Type configType = GetConfig(config);
        FieldInfo[] fields = instance.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].FieldType == configType)
            {
                object configObject = fields[i].GetValue(instance);
                fields = fields[i].FieldType.GetFields();
                for (i = 0; i < fields.Length; i++)
                {
                    if (fields[i].Name == variable)
                    {
                        fields[i].SetValue(configObject, val);
                    }
                }
            }
        }
    }

    public static Type GetConfig(string name)
    {
        if (!instance) instance = FindObjectOfType<ConfigManager>();

        List<Type> types = ConfigTypes;
        for (int i = 0; i < types.Count;i++)
        {
            if (types[i].Name.ToLower() == name.ToLower())
            {
                return types[i];
            }
        }

        return null;
    }

    public static List<Type> ConfigTypes
    {
        get
        {
            return Assembly.GetAssembly(typeof(Config)).GetTypes().Where(type => type.IsSubclassOf(typeof(Config))).ToList();
        }
    }
    
    static void RefreshConfig(Type type)
    {
        FieldInfo[] fields = instance.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].FieldType == type)
            {
                if (File.Exists(Root + "/Configs/" + type.Name + ".txt"))
                {
                    string json = File.ReadAllText(Root + "/Configs/" + type.Name + ".txt");
                    JsonUtility.FromJsonOverwrite(json, fields[i].GetValue(instance));
                }
                else
                {
                    string json = JsonUtility.ToJson(fields[i].GetValue(instance), true);
                    File.WriteAllText(Root + "/Configs/" + type.Name + ".txt", json);
                }

                return;
            }
        }
    }
}
