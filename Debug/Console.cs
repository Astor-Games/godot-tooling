// Copyright (c) 2020–2025 Mansur Isaev and contributors - MIT License
// Ported to C# by Joaquin Muñiz

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using GodotLib.UI;
using GodotLib.Util;
using static GodotLib.Debug.Assertions;
using Convert = System.Convert;

namespace GodotLib.Debug;

[GlobalClass]
public partial class Console : DockablePanel
{
    private class CommandData
    {
        public string Name;
        public string Description;
        public Delegate Action;
        public ParameterInfo[] Parameters;
        public object[] DefaultArgs;
    }
    
    private readonly SortedDictionary<string, CommandData> commands = new(StringComparer.OrdinalIgnoreCase);

    private readonly List<string> history = new();
    private int historyIndex;
    
    private RichTextLabel output;
    private LineEdit input;
    private RichTextLabel tooltip;

    private int autocompleteIndex = -1;
    private string autocompletePrompt = "";

    public bool HasCommand(string command) => commands.ContainsKey(command);

    public void AddCommand(string commandName, Delegate action, string description = "")
    {
        commandName = commandName.ToLowerInvariant();
        
        AssertFalse(HasCommand(commandName), $"Command '{commandName}' already exists.");
        AssertTrue(commandName.IsValidIdentifier(), $"Invalid command name: '{commandName}'.");
        
        var method = action.Method;
        var parameters = method.GetParameters();
        var defaultArgs = parameters.Where(p => p.HasDefaultValue).Select(p => p.DefaultValue).ToArray();
        
        if (description.IsNullOrEmpty()) 
            description = "[i]No description available.[/i]";
        
        var command = new CommandData
        {
            Name = commandName,
            Action = action,
            Description = description,
            Parameters = parameters,
            DefaultArgs = defaultArgs
        };

        commands.Add(commandName, command);
    }

    public void ExecuteCommand(string line)
    {
        try
        {
            var parts = line.Split(Array.Empty<string>(),
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            history.Add(line);
            historyIndex = history.Count;
            PrintCommand(line);

            var commandName = parts[0];
            var args = parts.AsSpan(1);

            if (!commands.TryGetValue(commandName, out var command))
            {
                PrintError($"Command '{commandName}' not found.");
                return;
            }

            if (!ValidateArgumentCount(args, command))
                return;

            var arguments = new object[command.Parameters.Length];

            for (var i = 0; i < arguments.Length; i++)
            {
                var parameter = command.Parameters[i];

                if (i < args.Length)
                {
                    arguments[i] = ParseArgument(args[i], parameter.ParameterType);
                }
                else
                {
                    arguments[i] = parameter.DefaultValue;
                }
            }

            var result = command.Action.DynamicInvoke(arguments);
            Print(result?.ToString());
        }
        catch (Exception e)
        {
            PrintError(e.Message);
        }
    }
    
    private bool ValidateArgumentCount(Span<string> args, CommandData cmd)
    {
        var defaultArgs = cmd.DefaultArgs.Length;
        var expectedMax = cmd.Parameters.Length;
        var expectedMin = expectedMax - defaultArgs;

        if (args.Length < expectedMin || args.Length > expectedMax)
        {
            var msg = defaultArgs > 0
                ? $"Invalid argument count: Expected between {expectedMin} and {expectedMax}, received {args.Length}."
                : $"Invalid argument count: Expected {expectedMax}, received {args.Length}.";
            PrintError(msg);
            return false;
        }
        return true;
    }

    private static object ParseArgument(string value, Type type)
    {
        if (value.StartsWith('{') || value.StartsWith('['))
            return JsonSerializer.Deserialize(value, type);
        
        return Convert.ChangeType(value, type);
    }

    public void Print(string text)
    {
        output.AppendText(text);
        output.Newline();
    }
    
    private void PrintCommand(string text)
    {
        output.PushColor(Colors.Gray);
        output.AppendText("> " + text);
        output.Newline();
        output.Pop();
    }

    public void PrintWarning(string text)
    {
        output.PushColor(Colors.Yellow);
        output.AppendText(text);
        output.Newline();
        output.Pop();
    }

    public void PrintError(string text)
    {
        output.PushColor(Colors.Red);
        output.AppendText(text);
        output.Newline();
        output.Pop();
    }

    private string GetPrevCommand()
    {
        historyIndex = Mathf.Wrap(historyIndex - 1, 0, history.Count);
        return history.Count == 0 ? "" : history[historyIndex];
    }

    private string GetNextCommand()
    {
        historyIndex = Mathf.Wrap(historyIndex + 1, 0, history.Count);
        return history.Count == 0 ? "" : history[historyIndex];
    }

    private string AutocompleteCommand(string prefix, int selectedIndex = -1)
    {
        if (string.IsNullOrEmpty(prefix))
            return prefix;

        var i = 0;
        foreach (var cmd in GetCommandList(prefix))
        {
            if (i++ == selectedIndex)
                return cmd + " ";
        }
        return prefix;
    }

    private IList<string> AutocompleteList(string prefix, int selectedIndex = -1)
    {
        if (string.IsNullOrEmpty(prefix))
            return [];
        
        var list = new List<string>();
        
        var i = 0;
        foreach (var cmd in GetCommandList(prefix))
        {
            list.Add(i++ == selectedIndex ? $"[u]{cmd}[/u]" : cmd);
        }
        return list;
    }

    private IEnumerable<string> GetCommandList(string prefix = null)
    {
        foreach (var cmd in commands.Keys)
        {
            if (string.IsNullOrEmpty(prefix) || cmd.StartsWith(prefix))
                yield return cmd;
        }
    }

    public void Clear()
    {
        output.Clear();
        history.Clear();
        historyIndex = 0;
    }
    
    public override void ToggleVisibility()
    {
        base.ToggleVisibility();
        if (Visible)
        {
            Godot.Input.MouseMode = Godot.Input.MouseModeEnum.Visible;
            input.GrabFocus();
        }
    }

    public void Quit()
    {
        Clear();
        Visible = false;
    }

    private void PrintHelp(string commandName = null)
    {
        if (commandName.IsNullOrEmpty())
        {
            output.PushTable(2);
            foreach (var cmd in GetCommandList())
            {
                var description = commands[cmd].Description;
            
                output.PushCell();
                output.PushColor(Colors.White);
                output.PushMeta(cmd);
                output.AddText(cmd); 
                output.Pop();
                output.Pop();
                output.Pop();
            
                output.PushCell();
                output.PushColor(Colors.Gray);
                output.AddText(description);
                output.Pop();
                output.Pop();
            }
        
            output.Pop();
            return;
        }

        if (!commands.TryGetValue(commandName, out var command))
        {
            PrintError($"Command '{commandName}' not found.");
            return;
        }
        
        output.PushColor(Colors.Gray);
        output.AddText(command.Description);
        output.Newline();
        output.AddText($"Usage: {commandName} ");
        output.PushItalics();
        
        var requiredParams = string.Join(" ", command.Parameters.Where(p => !p.HasDefaultValue).Select(p => p.Name!.ToSnakeCase()));
        var optionalParams = string.Join(" ", command.Parameters.Where(p => p.HasDefaultValue).Select(p => p.Name!.ToSnakeCase()));
        var paramLine = optionalParams.IsNullOrEmpty() ? requiredParams : $"{requiredParams} [{optionalParams}]";
        output.AddText(paramLine);
        output.PopAll();
    }

    public override void _EnterTree()
    {
        VisibilityChanged += OnVisibilityChanged;
    }

    public override void _Ready()
    {
        CreateUI();

        AddCommand("clear", Clear, "Clears the console history.");
        AddCommand("help", PrintHelp, "Shows information for the provided command. If no command is provided, lists all available commands.");
        AddCommand("quit", Quit, "Closes the console.");
        
        base._Ready();
    }

    private void CreateUI()
    {
        // Output box
        output = GetNode<RichTextLabel>("%Output");
        output.SizeFlagsVertical = SizeFlags.ExpandFill;
        output.ScrollFollowing = true;
        output.SelectionEnabled = true;
        output.FocusMode = FocusModeEnum.None;
        output.MetaClicked += meta => SetInputText(meta.AsString());
        
        // Input box
        input = GetNode<LineEdit>("%Input");
        input.ContextMenuEnabled = false;
        input.ClearButtonEnabled = true;
        input.CaretBlink = true;
        input.PlaceholderText = "Command";
        input.Editable = true;
        input.TextChanged += OnInputTextChanged;
        input.GuiInput += OnInputGuiEvent;
        
        // Tooltip
        tooltip = new RichTextLabel
        {
            ThemeTypeVariation = "TooltipPanel",
            BbcodeEnabled = true,
            AutowrapMode = TextServer.AutowrapMode.Off,
            FitContent = true, 
            GrowVertical= GrowDirection.Begin
        };
        tooltip.SetOffset(Side.Left, 4f);
        tooltip.SetOffset(Side.Bottom, -4f);
        tooltip.Hide();
        tooltip.AddThemeStyleboxOverride("normal", GetThemeStylebox("panel", "TooltipPanel"));
        input.AddChild(tooltip);
    }

    private void SetInputText(string text)
    {
        input.Text = text;
        input.CaretColumn = text.Length;
        input.EmitSignal(LineEdit.SignalName.TextChanged, text);
    }

    private void OnInputTextChanged(string text)
    {
        ShowAutocomplete(string.IsNullOrEmpty(autocompletePrompt) ? text : autocompletePrompt);
    }

    private void ShowAutocomplete(string text)
    {
        var autocomplete = AutocompleteList(text, autocompleteIndex);
        if (autocomplete.Count == 0)
        {
            tooltip.Hide();
        }
        else
        {
            tooltip.Text = string.Join("\n", autocomplete);
            tooltip.Show();
        }
    }

    private void CycleAutocomplete(int direction)
    {
        var autocomplete = AutocompleteList(autocompletePrompt);
        if (autocomplete.Count == 0)
            return;

        autocompleteIndex = Mathf.Wrap(autocompleteIndex + direction, 0, autocomplete.Count);
        SetInputText(AutocompleteCommand(autocompletePrompt, autocompleteIndex));
    }

    private void OnInputGuiEvent(InputEvent evt)
    {
        if (evt.IsActionType() && !evt.IsAction("ui_text_indent"))
        {
            autocompleteIndex = -1;
            autocompletePrompt = "";
            ShowAutocomplete(input.Text);
        }

        if (evt.IsActionPressed("ui_text_submit"))
        {
            ExecuteCommand(input.Text);
            input.Clear();
        }
        else if (evt.IsActionPressed("ui_text_completion_accept"))
        {
            if (string.IsNullOrEmpty(autocompletePrompt))
                autocompletePrompt = input.Text;

            CycleAutocomplete(Godot.Input.IsKeyPressed(Key.Shift) ? -1 : 1);
        }
        else if (evt.IsActionPressed("ui_text_caret_up"))
        {
            SetInputText(GetPrevCommand());
        }
        else if (evt.IsActionPressed("ui_text_caret_down"))
        {
            SetInputText(GetNextCommand());
        }
        else
        {
            return;
        }

        input.AcceptEvent();
    }
    
    private void OnVisibilityChanged()
    {
        if (IsVisibleInTree())
        {
            input.GrabFocus();
            input.AcceptEvent();
            input.Clear();
        }
    }
}
