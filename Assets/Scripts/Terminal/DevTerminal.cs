using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevTerminal : MonoBehaviour
{
    public static DevTerminal Instance { get; private set; }

    [SerializeField] private Canvas terminalCanvas; // The canvas to toggle
    [SerializeField] private TMP_Text terminalDisplay;  // The text area that simulates the terminal
    [SerializeField] private ScrollRect scrollRect; // ScrollRect to allow scrolling

    private bool isTerminalOpen = false;
    private string currentInput = string.Empty;     // Current command being typed
    private string allOutput = string.Empty;        // Store all terminal output for display
    private string promptSymbol = "> ";             // Prompt symbol
    private bool contentChanged = false;            // Flag to track if the content has changed

    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;  // Used to track the current position in history

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Toggle the terminal with the ` key
        if (Input.GetKeyDown(KeyCode.F1)) 
        {
            isTerminalOpen = !isTerminalOpen;
            terminalCanvas.gameObject.SetActive(isTerminalOpen);
            InputManager.Instance.IsTerminalOpen = isTerminalOpen;
            return;
        }

        if (isTerminalOpen)
        {
            // Handle arrow key navigation through command history
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                NavigateHistoryUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                NavigateHistoryDown();
            }
            // Handle character input
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Handle backspace
                {
                    if (currentInput.Length > 0)
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                else if (c == '\n' || c == '\r') // Handle enter key
                {
                    ProcessInput(currentInput);
                    currentInput = string.Empty; // Clear input after processing
                }
                else
                {
                    currentInput += c; // Append typed character to current input
                }
            }

            // Update the terminal display, showing the previous output and current input with prompt
            terminalDisplay.text = allOutput + "\n" + promptSymbol + currentInput;


            if (contentChanged)
            {
                AdjustContentHeight();
                Canvas.ForceUpdateCanvases();

                //Debug.Log("terminal display" + terminalDisplay.rectTransform.sizeDelta.y);
                //Debug.Log("terminal canvas" + terminalCanvas.GetComponent<RectTransform>().rect.height);
                if (terminalDisplay.rectTransform.sizeDelta.y > terminalCanvas.GetComponent<RectTransform>().rect.height)
                {
                    scrollRect.verticalNormalizedPosition = 0f;  // Scroll to bottom
                }
                else
                {
                    scrollRect.verticalNormalizedPosition = 1f;  // Scroll to top
                }
                contentChanged = false;  // Reset the flag
            }
        }
    }

    private void NavigateHistoryUp()
    {
        // Move up in history but skip consecutive duplicates
        if (historyIndex > 0)
        {
            do
            {
                historyIndex--;
            } while (historyIndex > 0 && commandHistory[historyIndex -1] == commandHistory[historyIndex]);

            currentInput = commandHistory[historyIndex];
        }
    }


    private void NavigateHistoryDown()
    {
        // Move down in history but stop at the most recent command
        if (historyIndex < commandHistory.Count - 1)
        {
            do
            {
                historyIndex++;
            } while (historyIndex < commandHistory.Count - 1 && commandHistory[historyIndex] == commandHistory[historyIndex - 1]);

            currentInput = commandHistory[historyIndex];
        }
        else
        {
            historyIndex = commandHistory.Count;
            currentInput = string.Empty;
        }
    }

    /// <summary>
    /// Adjusts the content size of the terminal based on the height of the text.
    /// </summary>
    private void AdjustContentHeight()
    {
        // Get the preferred height of the text content
        float textHeight = terminalDisplay.preferredHeight;

        // Adjust the height of the content RectTransform
        terminalDisplay.rectTransform.sizeDelta = new Vector2(terminalDisplay.rectTransform.sizeDelta.x, textHeight);
    }

    /// <summary>
    /// Processes the command typed by the user.
    /// </summary>
    /// <param name="input">The command input by the user.</param>
    private void ProcessInput(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            commandHistory.Add(input);
            historyIndex = commandHistory.Count; // Reset history index to point to the end of the list
        }
        // Store the input command in the output before processing
        allOutput += "\n" + promptSymbol + input;

        // Split input into command and parameters
        string[] parts = input.Split(' ');
        if (parts.Length == 0) return;

        string command = parts[0];
        string[] parameters = parts.Skip(1).ToArray();

        if (command.Equals("clear", StringComparison.OrdinalIgnoreCase))
        {
            ClearTerminal();
            return;
        }

        // Handle "help" command
        if (command.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            DisplayHelp();
            contentChanged = true;
            return;
        }

        // Execute the command
        ExecuteCommand(command, parameters);

        contentChanged = true;
    }

    /// <summary>
    /// Executes the command by looking for methods with the DevCommand attribute.
    /// </summary>
    private void ExecuteCommand(string commandName, string[] parameters)
    {
        // Split parameters into flags and arguments
        List<string> flags = new();
        List<string> arguments = new();

        foreach (var param in parameters)
        {
            if (param.StartsWith("-"))
                flags.Add(param); // Add flag to the list
            else
                arguments.Add(param); // Add as argument
        }

        // Get all methods with DevCommandAttribute
        var methods = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            .Where(m => m.GetCustomAttributes(typeof(DevCommandAttribute), false).Length > 0)
            .ToArray();

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<DevCommandAttribute>();

            // Check if command matches alias (ignoring method name)
            if (attribute.Aliases.Contains(commandName, StringComparer.OrdinalIgnoreCase))
            {
                // Delegate argument parsing and validation to a helper method
                if (!TryInvokeMethod(method, arguments.ToArray(), flags.ToArray()))
                {
                    allOutput += $"\nError: Failed to execute {commandName}.";
                }
                return;
            }
        }

        allOutput += $"\nUnknown command: {commandName}";
    }

    private bool TryInvokeMethod(MethodInfo method, string[] arguments, string[] flags)
    {
        var methodParams = method.GetParameters();
        int paramLength = methodParams.Length;

        var attribute = method.GetCustomAttribute<DevCommandAttribute>();
        string[] validFlags = attribute?.ValidFlags ?? Array.Empty<string>();

        // Validate flags against the valid flags defined in the attribute
        foreach (var flag in flags)
        {
            if (!validFlags.Contains(flag, StringComparer.OrdinalIgnoreCase))
            {
                allOutput += $"\nError: Invalid flag '{flag}' for command '{method.Name}'. Valid flags: {string.Join(", ", validFlags)}";
                return false;
            }
        }

        // Check if the method accepts flags (as an array of TerminalFlag objects)
        bool methodAcceptsFlags = paramLength > 0 && methodParams[paramLength - 1].ParameterType == typeof(TerminalFlag[]);

        object[] parsedParams;

        // If the method accepts flags, we need to process both arguments and flags
        if (methodAcceptsFlags)
        {
            parsedParams = new object[paramLength];

            // Check if the first parameter is a string[] (arguments array)
            if (methodParams[0].ParameterType == typeof(string[]))
            {
                // Pass the arguments array directly
                parsedParams[0] = arguments;
            }
            else
            {
                // Process each argument and check for optional parameters
                for (int i = 0; i < paramLength - 1; i++)  // Exclude the last parameter (flags)
                {
                    var paramInfo = methodParams[i];
                    if (i < arguments.Length)
                    {
                        // Convert provided arguments
                        try
                        {
                            parsedParams[i] = ConvertArgument(paramInfo.ParameterType, arguments[i]);
                        }
                        catch (Exception ex)
                        {
                            allOutput += $"\nError: {ex.Message}";
                            return false;
                        }
                    }
                    else if (paramInfo.IsOptional)
                    {
                        // Use default value for optional arguments
                        parsedParams[i] = paramInfo.DefaultValue;
                    }
                    else
                    {
                        allOutput += $"\nError: Missing required argument '{paramInfo.Name}' for command '{method.Name}'.";
                        return false;
                    }
                }
            }

            // Convert string flags to TerminalFlag objects and pass them as the last argument
            parsedParams[paramLength - 1] = flags.Select(f => new TerminalFlag(f)).ToArray();
        }
        else
        {
            // Ensure that the number of arguments is valid, considering optional parameters
            int requiredArgsCount = methodParams.Count(p => !p.IsOptional);
            if (arguments.Length < requiredArgsCount || arguments.Length > paramLength)
            {
                allOutput += $"\nError: The command '{method.Name}' expects between {requiredArgsCount} and {paramLength} arguments, but {arguments.Length} were provided.";
                return false;
            }

            parsedParams = new object[paramLength];

            // Convert each argument to its expected type or use the default value if it's optional
            for (int i = 0; i < methodParams.Length; i++)
            {
                var paramInfo = methodParams[i];
                if (i < arguments.Length)
                {
                    // Convert provided arguments
                    try
                    {
                        parsedParams[i] = ConvertArgument(paramInfo.ParameterType, arguments[i]);
                    }
                    catch (Exception ex)
                    {
                        allOutput += $"\nError: {ex.Message}";
                        return false;
                    }
                }
                else if (paramInfo.IsOptional)
                {
                    // Use default value for optional arguments
                    parsedParams[i] = paramInfo.DefaultValue;
                }
                else
                {
                    allOutput += $"\nError: Missing required argument '{paramInfo.Name}' for command '{method.Name}'.";
                    return false;
                }
            }
        }

        try
        {
            object instance = null;
            if (!method.IsStatic)
            {
                instance = FindObjectOfType(method.DeclaringType);
            }

            method.Invoke(instance, parsedParams);

            // Handle flags after invocation
            if (flags.Length > 0)
            {
                allOutput += $"\nExecuted {method.Name} with flags: {string.Join(", ", flags)}";
            }
            else
            {
                allOutput += $"\nExecuted {method.Name}";
            }

            return true;
        }
        catch (Exception e)
        {
            allOutput += $"\nError during method invocation: {e.Message}";
            return false;
        }
    }


    private object ConvertArgument(Type targetType, string argument)
    {
        if (targetType.IsEnum)
        {
            // Handle enum conversion (e.g., ResourceType)
            if (Enum.TryParse(targetType, argument, true, out var enumValue))
            {
                return enumValue;
            }
            throw new ArgumentException($"Invalid value '{argument}' for enum type {targetType.Name}.");
        }

        if (targetType == typeof(TerminalFlag))
        {
            // Handle flag conversion
            return new TerminalFlag(argument);
        }

        try
        {
            // Convert argument to the target type (e.g., int, float, etc.)
            return Convert.ChangeType(argument, targetType);
        }
        catch
        {
            throw new ArgumentException($"Argument '{argument}' could not be converted to {targetType.Name}.");
        }
    }

    /// <summary>
    /// Clears the terminal output.
    /// </summary>
    private void ClearTerminal()
    {
        allOutput = string.Empty;
        commandHistory.Clear();  // Clear all command history
        historyIndex = -1;       // Reset the history index

        AdjustContentHeight();  // Ensure the content size is updated after clearing

        // Scroll to the top after clearing the terminal
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }


    /// <summary>
    /// Displays available commands with descriptions from the DevCommandAttribute.
    /// </summary>
    private void DisplayHelp()
    {
        allOutput += "\nAvailable Commands:";

        // Get all methods with the DevCommand attribute
        var methods = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            .Where(m => m.GetCustomAttributes(typeof(DevCommandAttribute), false).Length > 0);

        // Padding values for alignment
        //int commandPadding = 8;  // Padding for command alignment
        int descriptionPadding = 80;  // Padding for description alignment

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<DevCommandAttribute>();

            // Ensure the alias and description are padded for alignment
            string description = attribute.Description.PadRight(descriptionPadding);  // Pad description
            string usage = attribute.Usage;  // Get usage from attribute

            // Add the formatted command to the terminal output
            allOutput += $"{description}Usage: {usage}";
        }
    }



    /// <summary>
    /// Logs a message to the terminal display.
    /// </summary>
    public void LogToTerminal(string message)
    {
        allOutput += "\n" + message;
        contentChanged = true;
    }
}
