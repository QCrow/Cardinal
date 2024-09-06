using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevTerminal : MonoBehaviour
{
    [SerializeField] private Canvas terminalCanvas; // The canvas to toggle
    [SerializeField] private TMP_Text terminalDisplay;  // The text area that simulates the terminal
    [SerializeField] private ScrollRect scrollRect; // ScrollRect to allow scrolling
    //[SerializeField] private RectTransform contentRect; // The content RectTransform inside ScrollRect that expands with text

    private bool isTerminalOpen = false;
    private string currentInput = string.Empty;     // Current command being typed
    private string allOutput = string.Empty;        // Store all terminal output for display
    private string promptSymbol = "> ";             // Prompt symbol
    private bool contentChanged = false;            // Flag to track if the content has changed

    private void Update()
    {
        // Toggle the terminal with the ` key
        if (Input.GetKeyDown(KeyCode.BackQuote)) // ` is the backtick/tilde key
        {
            isTerminalOpen = !isTerminalOpen;
            terminalCanvas.gameObject.SetActive(isTerminalOpen);
            return;
        }

        if (isTerminalOpen)
        {
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

                Debug.Log("terminal display" + terminalDisplay.rectTransform.sizeDelta.y);
                Debug.Log("terminal canvas" + terminalCanvas.GetComponent<RectTransform>().rect.height);
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
        object[] parsedParams = new object[methodParams.Length];

        // Ensure the exact number of arguments is provided
        if (arguments.Length != methodParams.Length)
        {
            allOutput += $"\nError: The command '{method.Name}' expects {methodParams.Length} arguments, but {arguments.Length} were provided.";
            return false;
        }

        // Parse and assign arguments
        for (int i = 0; i < methodParams.Length; i++)
        {
            var paramInfo = methodParams[i];

            try
            {
                // Convert the argument to the appropriate type
                parsedParams[i] = ConvertArgument(paramInfo.ParameterType, arguments[i]);
            }
            catch (Exception ex)
            {
                allOutput += $"\nError: {ex.Message}";
                return false;
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

        var methods = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            .Where(m => m.GetCustomAttributes(typeof(DevCommandAttribute), false).Length > 0);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<DevCommandAttribute>();
            allOutput += $"{attribute.Description}";
        }
    }
}
