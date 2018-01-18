#pragma warning disable

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public class Console
{
    //For checking if the window is open
    public static bool Open
    {
        get
        {
            if (!ConsoleManager.instance) ConsoleManager.instance = GameObject.FindObjectOfType<ConsoleManager>();

            return ConsoleManager.instance.open;
        }
    }

    //For running commands using Console.Run
    public static void Run(string command)
    {
        if (!ConsoleManager.instance) ConsoleManager.instance = GameObject.FindObjectOfType<ConsoleManager>();

        ConsoleManager.instance.Run(command);
    }

    //For writing lines using Console.WriteLine
    public static void WriteLine(string text, ConsoleManager.MessageType type = ConsoleManager.MessageType.Log)
    {
        if (!ConsoleManager.instance) ConsoleManager.instance = GameObject.FindObjectOfType<ConsoleManager>();

        ConsoleManager.instance.WriteLine(text, type);
    }
}

public class ConsoleManager : MonoBehaviour
{
    public class CommandAttribute : Attribute
    {
        public string name;
        public string description;
        public string usage;
    }

    public enum MessageType
    {
        Log,
        Error,
        Warning,
        Message,
        User
    }

    [Serializable]
    public class Bind
    {
        public string command;
        public string key;
        public KeyCode keyCode;

        public Bind(string key, string command)
        {
            this.key = key;
            this.keyCode = ToKeyCode(key);
            this.command = command;
        }
    }

    public int consoleLines = 16;
    public float consoleSpeed = 16f;
    int consoleScroll = 0;
    
    public RectTransform consoleWindow;
    public Text consoleText;
    public InputField consoleInput;
    public float consoleLineSize = 25f;
    public float consoleWindowSize = 300f;

    public Color logColor;
    public Color errorColor;
    public Color warningColor;
    public Color userColor;
    public Color valueColor;
    public Color identifierColor;
    public Color messageColor;

    public static ConsoleManager instance;
    public bool open;

    public List<string> entries = new List<string>();

    public enum ParamType
    {
        Float,
        Int,
        String,
        Bool
    }

    [Serializable]
    public class Command
    {
        public string name;
        public string description;
        public string usage;
        public List<ParamType> parameters = new List<ParamType>();
        public string methodName = "";
        public MethodInfo method;

        public string Parameters
        {
            get
            {
                string paramText = "";
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        paramText += "<" + parameters[i].ToString() + "> ";
                    }
                    if (parameters.Count > 0)
                    {
                        paramText = paramText.Substring(0, paramText.Length - 1);
                    }
                }
                return "<color=" + ConsoleManager.ToHex(ConsoleManager.instance.valueColor) + ">" + paramText.ToLower() + "</color>";
            }
        }
    }

    public List<Bind> binds = new List<Bind>();
    public List<Command> commands = new List<Command>();
    public List<string> history = new List<string>();
    int historyIndex = -1;
    string savedCommand;

    void Awake()
    {
        instance = this;
        LoadBinds();
        StartCommands();

        Welcome();
        consoleWindow.sizeDelta = new Vector2(0, -10);
        consoleWindow.gameObject.SetActive(false);
    }

    public static void Welcome()
    {
        instance.WriteLine("Yo", MessageType.Message);
    }

    #region Commands

    [Command(name = "clear", description = "Clears the console window")]
    public void Command_Clear()
    {
        entries.Clear();
    }

    [Command(name = "help", description = "Outputs a list of all commands")]
    public void Command_HelpHelp(string command)
    {
        for (int i = 0; i < commands.Count; i++)
        {
            if (commands[i].name == command)
            {
                WriteLine("SIGNATURE FOR " + command, MessageType.Message);
                WriteLine("    " + commands[i].name + " " + commands[i].Parameters + " = " + commands[i].description, MessageType.Message);
                WriteLine("    EXAMPLE USAGE: " + commands[i].usage, MessageType.Message);
                return;
            }
        }

        WriteLine("NO COMMAND FOUND WITH THE NAME " + command, MessageType.Error);
    }

    [Command(name = "help", description = "Describes a specified command")]
    public void Command_Help()
    {
        WriteLine("ALL COMMANDS", MessageType.Message);
        for (int i = 0; i < commands.Count; i++)
        {
            WriteLine("    " + commands[i].name + " " + commands[i].Parameters + " = " + commands[i].description, MessageType.Message);
        }
    }

    [Command(name = "bind", description = "Binds a command to a key", usage = "bind c clear")]
    public void Command_Bind(string key, string command)
    {
        AddBind(key, command);
        WriteLine("BINDED <color=" + ToHex(identifierColor) + ">" + command + "</color> TO <color=" + ToHex(valueColor) + ">" + key + "</color>", MessageType.Message);
    }

    [Command(name = "unbind", description = "Unbinds all commands from a key")]
    public void Command_Unbind(string key)
    {
        RemoveBind(key);
        WriteLine("UNBINDED ALL FROM <color=" + ToHex(valueColor) + ">" + key + "</color>", MessageType.Message);
    }

    [Command(name = "reload", description = "Reloads the commands console")]
    public void Command_Reload()
    {
        Command_Clear();
        StartCommands();
    }

    [Command(name = "exec", description = "Executes a method in the assembly", usage = "ConsoleManager.Welcome")]
    public void Command_Exec(string method)
    {
        if(!method.Contains("."))
        {
            WriteLine("WRONG FORMAT", MessageType.Error);
            return;
        }

        string className = method.Split('.')[0];
        string methodName = method.Split('.')[1];

        Type classType = Type.GetType(className);
        if (classType == null)
        {
            WriteLine("CLASS OF TYPE " + className + " NOT FOUND", MessageType.Error);
            return;
        }

        var methods = classType.GetMethods();
        for (int i = 0; i < methods.Length; i++)
        {
            if (methods[i].Name == methodName && methods[i].IsStatic)
            {
                string output = ParseOutput(methods[i].Invoke(null, null));
                WriteLine("OUTPUT: " + output, MessageType.Message);
                return;
            }
        }

        WriteLine("METHOD OF BY NAME " + methodName + " NOT FOUND", MessageType.Error);
    }

    //custom commands

    [Command(name = "reset_camera", description = "Resets camera orientation")]
    public void Command_ResetCamera()
    {
        CameraManager.Rotation = 0;
    }

    [Command(name = "next_turn", description = "Forces next turn")]
    public void Command_NextTurn()
    {
        if (!GameManager.instance) GameManager.instance = FindObjectOfType<GameManager>();

        GameManager.instance.NextTurn();
    }

    [Command(name = "spawn", description = "Spawns a unit on a selected tile")]
    public void Command_Spawn(string unit)
    {
        //claims the selected tile
        if (Config.Temporary.selectedTile)
        {
            GameManager.PlaceUnit(unit, Config.Temporary.selectedTile, Config.GameSave.nation);
        }
        else
        {
            Nation nation = Config.GameSave.nation;
            if(nation.Capital)
            {
                GameManager.PlaceUnit(unit, nation.Capital.Tile, Config.GameSave.nation);
                WriteLine("NO TILE SELECTED, SPAWNED AT CAPITAL INSTEAD", MessageType.Warning);
            }
            else
            {
                WriteLine("NO TILE SELECTED OR CAPITAL FOUND TO SPAWN AT", MessageType.Error);
            }
        }
    }

    [Command(name = "set_moves", description = "Changes a selected unit's move count")]
    public void Command_SetMoves(int moves)
    {
        //claims the selected tile
        if (Config.Temporary.selectedUnit)
        {
            Config.Temporary.selectedUnit.Moves = moves;
        }
        else
        {
            WriteLine("NO UNIT SELECTED", MessageType.Error);
        }
    }

    private string ParseOutput(object value)
    {
        if (value == null) return "null";
        string result = "";

        Type valueType = value.GetType();
        Debug.Log(valueType.IsArray);

        if(valueType.IsArray)
        {
            if(typeof(string).IsAssignableFrom(valueType.GetElementType()))
            {
                //string array
                string[] array = (string[])value;
                result = string.Join("\n", array);
            }
            else
            {
                result = valueType.Name + "[]";
            }
        }
        else
        {
            result = value.ToString();
        }

        return result;
    }

    #endregion

    public static KeyCode ToKeyCode(string key)
    {
        key = key.ToUpper();

        if (key.Length == 1 && char.IsLetterOrDigit(key[0]))
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), key, true);
        }

        return KeyCode.None;
    }

    private void StartCommands()
    {
        commands.Clear();
        
        var methods = GetType().GetMethods();
        for (int i = 0; i < methods.Length; i++)
        {
            foreach (Attribute attribute in Attribute.GetCustomAttributes(methods[i]))
            {
                if (attribute.GetType() == typeof(CommandAttribute))
                {
                    CommandAttribute commandAttribute = (CommandAttribute)attribute;
                    List<ParamType> paramTypes = new List<ParamType>();
                    var parameters = methods[i].GetParameters();
                    for (int p = 0; p < parameters.Length; p++)
                    {
                        if(parameters[p].ParameterType == typeof(string))
                        {
                            paramTypes.Add(ParamType.String);
                        }
                        else if (parameters[p].ParameterType == typeof(bool))
                        {
                            paramTypes.Add(ParamType.Bool);
                        }
                        else if (parameters[p].ParameterType == typeof(int))
                        {
                            paramTypes.Add(ParamType.Int);
                        }
                        else if (parameters[p].ParameterType == typeof(float))
                        {
                            paramTypes.Add(ParamType.Float);
                        }
                    }

                    RegisterCommand(commandAttribute, paramTypes, methods[i].Name);
                }
            }
        }
    }

    private void RegisterCommand(CommandAttribute commandAttribute, List<ParamType> parameters, string methodName)
    {
        Command command = new Command()
        {
            name = commandAttribute.name,
            parameters = parameters ?? (new List<ParamType>()),
            methodName = methodName,
            description = commandAttribute.description,
            usage = commandAttribute.usage
        };

        AssignMethod(command);
        commands.Add(command);
    }

    private void AssignMethod(Command command)
    {
        var methods = GetType().GetMethods();
        for (int i = 0; i < methods.Length; i++)
        {
            if (methods[i].Name == command.methodName && methods[i].GetParameters().Length == command.parameters.Count)
            {
                command.method = methods[i];
                return;
            }
        }
    }

    private void OnGUI()
    {
        if(Event.current != null && Event.current.type == EventType.KeyDown)
        {
            if(Event.current.keyCode == KeyCode.Return)
            {
                Run();
            }
            else if(Event.current.keyCode == KeyCode.BackQuote)
            {
                open = !open;
                if (open)
                {
                    UpdateText();

                    consoleInput.Select();
                    consoleInput.ActivateInputField();
                }
                else
                {
                    consoleInput.DeactivateInputField();
                }

                consoleInput.text = consoleInput.text.Replace("`", "");
            }
        }
    }

    private void Update()
    {
        if (!open)
        {
            for (int i = 0; i < binds.Count; i++)
            {
                if (Input.GetKeyDown(binds[i].keyCode))
                {
                    Run(binds[i].command);
                }
            }
        }

        if (open)
        {
            if (consoleWindow.sizeDelta.y < consoleWindowSize)
            {
                consoleWindow.sizeDelta += Vector2.up * Time.deltaTime * consoleSpeed * 100f;
                if (consoleWindow.sizeDelta.y > consoleWindowSize)
                {
                    consoleWindow.sizeDelta = new Vector2(0, consoleWindowSize);
                }
            }
            if (!consoleWindow.gameObject.activeSelf)
            {
                consoleWindow.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && history.Count > 0)
            {
                if (historyIndex == -1)
                {
                    savedCommand = consoleInput.text;
                }
                historyIndex++;
                if (historyIndex >= history.Count)
                {
                    historyIndex = history.Count - 1;
                }
                consoleInput.text = history[historyIndex];
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && history.Count > 0)
            {
                if (historyIndex == 0)
                {
                    consoleInput.text = savedCommand;
                }
                historyIndex--;
                if (historyIndex < -1)
                {
                    historyIndex = -1;
                }
                else if (historyIndex >= 0)
                {
                    consoleInput.text = history[historyIndex];
                }
            }

            bool updateText = false;
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                consoleScroll++;
                updateText = true;
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                consoleScroll--;
                updateText = true;
            }
            if (consoleScroll < 1) consoleScroll = 1;
            if (consoleScroll >= entries.Count) consoleScroll = entries.Count;

            if (updateText)
            {
                UpdateText();
            }
        }
        else
        {
            if (consoleWindow.sizeDelta.y > -consoleLineSize)
            {
                consoleWindow.sizeDelta -= Vector2.up * Time.deltaTime * consoleSpeed * 100f;
                if (consoleWindow.sizeDelta.y <= -consoleLineSize)
                {
                    consoleWindow.sizeDelta = new Vector2(0, -consoleLineSize);
                    consoleWindow.gameObject.SetActive(false);
                }
            }
        }

        consoleText.enabled = open;
        instance = this;
    }

    private void UpdateText()
    {
        consoleText.text = "";
        for (int i = 0; i < consoleLines; i++)
        {
            int index = entries.Count - (consoleLines - 1 + consoleScroll) + i;
            //index += consoleScroll;

            if (index >= 0 && entries.Count > index)
            {
                if (consoleText.text != "")
                {
                    consoleText.text += "\n";
                }
                consoleText.text += entries[index];
            }
        }
    }

    internal static string ToHex(Color color)
    {
        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);

        return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }

    public void WriteLine(string text, MessageType type)
    {
        if (type == MessageType.Warning && !Config.Settings.showWarnings) return;
        if (type == MessageType.Error && !Config.Settings.showErrors) return;
        if (type == MessageType.Log && !Config.Settings.showLogs) return;

        Color color = logColor;
        if (type == MessageType.Error) color = errorColor;
        if (type == MessageType.Message) color = messageColor;
        if (type == MessageType.User) color = userColor;
        if (type == MessageType.Warning) color = warningColor;

        entries.Add("<color=" + ToHex(color) + ">" + text + "</color>");

        consoleScroll = 1;
        UpdateText();
    }

    #region Binds

    private void LoadBinds()
    {
        binds.Clear();

        if (!Directory.Exists(ConfigManager.Root + "/Console"))
        {
            Directory.CreateDirectory(ConfigManager.Root + "/Console");
        }
        if (!File.Exists(ConfigManager.Root + "/Console/Binds.txt"))
        {
            File.Create(ConfigManager.Root + "/Console/Binds.txt");
            return;
        }

        string[] bindsData = File.ReadAllLines(ConfigManager.Root + "/Console/Binds.txt");
        for (int i = 0; i < bindsData.Length; i++)
        {
            if (bindsData[i].Contains("="))
            {
                string key = bindsData[i].Split('=')[0];
                string command = bindsData[i].Split('=')[1];

                binds.Add(new Bind(key, command));
            }
        }
    }

    private void AddBind(string key, string command)
    {
        for (int i = 0; i < binds.Count; i++)
        {
            if (binds[i].key == key)
            {
                binds.RemoveAt(i); break;
            }
        }

        binds.Add(new Bind(key, command));

        SaveBinds();
    }

    private void RemoveBind(string key)
    {
        for (int i = 0; i < binds.Count; i++)
        {
            if (binds[i].key == key)
            {
                binds.RemoveAt(i); break;
            }
        }

        SaveBinds();
    }

    private void SaveBinds()
    {
        if (!Directory.Exists(ConfigManager.Root + "/Console"))
        {
            Directory.CreateDirectory(ConfigManager.Root + "/Console");
        }

        string varText = "";
        for (int i = 0; i < binds.Count; i++)
        {
            varText += binds[i].key + "=" + binds[i].command;
            if (i < binds.Count - 1)
            {
                varText += "\n";
            }
        }

        varText = varText.Substring(0, varText.Length - 1);
        File.WriteAllText(ConfigManager.Root + "/Console/Binds.txt", varText);
    }

    #endregion

    private void ProcessCommand(string cmd, List<string> parameters)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            if (parameters[i] == "") parameters.RemoveAt(i);
        }

        for (int i = 0; i < commands.Count; i++)
        {
            if (commands[i].name == cmd)
            {
                if (commands[i].parameters.Count == parameters.Count)
                {
                    if (commands[i].method == null) AssignMethod(commands[i]);

                    if (commands[i].method != null)
                    {
                        List<object> parameterObjects = new List<object>();
                        if (commands[i].parameters != null && commands[i].parameters.Count > 0)
                        {
                            if (commands[i].parameters.Count != parameters.Count)
                            {
                                WriteLine("UNEXPECT PARAMETERS, `help " + cmd + "` FOR MORE INFO", MessageType.Error);
                                return;
                            }
                            else
                            {
                                for (int p = 0; p < commands[i].parameters.Count; p++)
                                {
                                    object paramObject = parameters[p];
                                    if (commands[i].parameters[p] == ParamType.Bool)
                                    {
                                        paramObject = parameters[p].ToLower() == "true" ? true : false;
                                    }
                                    else if (commands[i].parameters[p] == ParamType.String)
                                    {
                                        paramObject = parameters[p];
                                    }
                                    else if (commands[i].parameters[p] == ParamType.Float)
                                    {
                                        float result = 0;
                                        if (float.TryParse(parameters[p], out result))
                                        {
                                            paramObject = result;
                                        }
                                        else
                                        {
                                            WriteLine("EXPECTED FLOAT FOR PARAMETER #" + p + ", RECIEVED " + parameters[p], MessageType.Error);
                                            return;
                                        }
                                    }
                                    else if (commands[i].parameters[p] == ParamType.Int)
                                    {
                                        int result = 0;
                                        if (int.TryParse(parameters[p], out result))
                                        {
                                            paramObject = result;
                                        }
                                        else
                                        {
                                            WriteLine("EXPECTED INT FOR PARAMETER #" + p + ", RECIEVED " + parameters[p], MessageType.Error);
                                            return;
                                        }
                                    }
                                    parameterObjects.Add(paramObject);
                                }
                            }
                        }

                        object returnValue = commands[i].method.Invoke(this, parameterObjects.ToArray());
                        if (returnValue != null)
                        {
                            WriteLine(returnValue.ToString(), MessageType.Message);
                        }

                        return;
                    }
                }
            }
        }

        if (parameters.Count >= 1)
        {
            if (cmd.Contains("."))
            {
                object variableValue = null;
                string configName = cmd.Split('.')[0];
                string variableName = cmd.Split('.')[1];
                string rest = "";
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (i == parameters.Count - 1)
                    {
                        rest += parameters[i];
                    }
                    else
                    {
                        rest += parameters[i] + " ";
                    }
                }

                var config = ConfigManager.GetConfig(configName);
                if(config != null)
                {
                    var field = ConfigManager.GetField(configName, variableName);
                    if (field != null)
                    {
                        bool valid = false;
                        if (field.Type == typeof(string))
                        {
                            variableValue = rest;
                            valid = true;
                        }
                        else if (field.Type == typeof(float))
                        {
                            variableValue = float.Parse(rest);
                            valid = true;
                        }
                        else if (field.Type == typeof(int))
                        {
                            variableValue = int.Parse(rest);
                            valid = true;
                        }
                        else if (field.Type == typeof(bool))
                        {
                            variableValue = (rest.ToLower() == "true" || rest == "1");
                            valid = true;
                        }
                        else
                        {
                            WriteLine("COULD NOT SET VALUE, VARIABLE TYPE NOT SUPPORTED (" + field.Type + ")", MessageType.Error);
                        }

                        if (valid)
                        {
                            ConfigManager.SetValue(configName, variableName, variableValue);
                            WriteLine("VALUE SET SUCCESSFULLY (" + variableName + "=" + variableValue + ")", MessageType.Message);
                        }
                    }
                    else
                    {
                        WriteLine("VARIABLE OF TYPE " + variableName + " NOT FOUND", MessageType.Error);
                    }
                }
                else
                {
                    WriteLine("CONFIG OF TYPE " + configName + " NOT FOUND", MessageType.Error);
                }
            }
            else
            {
                WriteLine("WRONG FORMAT, EXPECTED (CONFIG.VARIABLE VALUE)", MessageType.Error);
            }
        }

        UpdateText();
    }

    public void Run()
    {
        string txt = consoleInput.text;
        if (txt == "" || txt == "`") return;
        consoleInput.text = "";

        WriteLine(txt, MessageType.User);
        Run(txt);

        consoleInput.ActivateInputField();
    }

    public void Run(string txt)
    {
        if (txt == "" || txt == "`") return;

        history.Insert(0, txt);
        List<string> commandsFound = new List<string>();

        if (txt.Contains(";"))
        {
            bool insideQuotes = false;
            int lastCommandStart = 0;
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '"') insideQuotes = !insideQuotes;
                if (txt[i] == ';' && !insideQuotes)
                {
                    string commFound = txt.Substring(lastCommandStart, i - lastCommandStart);
                    commandsFound.Add(commFound);
                    lastCommandStart = i + 1;
                }
                if (i == txt.Length - 1)
                {
                    string commFound = txt.Substring(lastCommandStart);
                    commandsFound.Add(commFound);
                }
            }
        }
        else
        {
            commandsFound.Add(txt);
        }

        //command checking
        for (int i = 0; i < commandsFound.Count; i++)
        {
            string command = commandsFound[i];
            string[] paramaterArray = new string[] { };
            if (command.Contains(" "))
            {
                command = command.Split(' ')[0];
                paramaterArray = commandsFound[i].Substring(command.Length + 1).Split(' ');
            }

            string parameters = "";
            string stringFound = "";
            int startingIndex = -1;

            for (int p = 0; p < paramaterArray.Length; p++)
            {
                if (paramaterArray[p].StartsWith("\"") && startingIndex == -1)
                {
                    startingIndex = p;
                }
                if (startingIndex != -1)
                {
                    stringFound += paramaterArray[p] + " ";
                    if (paramaterArray[p].EndsWith("\""))
                    {
                        parameters += stringFound.TrimEnd().Replace("\"", "") + "*";

                        startingIndex = -1;
                        stringFound = "";
                    }
                }
                else
                {
                    parameters += paramaterArray[p] + "*";
                }
            }

            if (parameters.Length > 1)
            {
                parameters = parameters.Substring(0, parameters.Length - 1);
            }
            ProcessCommand(command, parameters.Split('*').ToList());
        }
    }
}
