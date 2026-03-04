using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodotLib.Debug;
using GodotLib.UI;
using GodotLib.Util;
using Turtles.addons.godot_lib.UI;
using ZLinq;
using static System.StringComparison;

namespace GodotLib.Logging;

[GlobalClass]
public partial class LogViewerPanel : DockablePanel
{
    private const string SeverityLevelKey = "window_logger_severity_level";
    private const string DisabledLoggersKey = "window_logger_disabled_loggers";

    private readonly StringBuilder sb = new();
    private HashSet<string> disabledLoggers;
    private SeverityLevel severityLevels = SeverityLevel.All;
    
    private ItemList logList;
    private Button clearButton;
    private MenuButton severitySelector;
    private PopupMenu severityMenu;
    private PopupMenu loggerMenu;
    private MenuButton loggerSelector;
    private ScrollBar listScrollBar;
    private LineEdit searchBar;
    
    private int lastDisplayedLog;
    private const float UpdateInterval = 0.1f; // Update UI every 100ms
    private float timeSinceLastUpdate;
    private bool autoScrollEnabled = true;
    private bool flagScrollToBottom;
    private bool flagRefreshLoggers;
    
    public override void _Ready()
    {
        base._Ready();

        SetTitle("Log Viewer");

        var optionsMenu = UseOptionsMenu();
        optionsMenu.AddCheckItem("Auto scroll");
        optionsMenu.SetItemChecked(0, autoScrollEnabled);

        logList = GetNode<ItemList>("%LogList");
        listScrollBar = logList.GetVScrollBar();
        logList.Draw += OnLogListDraw;
        logList.GuiInput += OnLogListGuiInput;

        clearButton = GetNode<Button>("%ClearButton");

        severitySelector = GetNode<MenuButton>("%SeveritySelector");
        severitySelector.Text = severityLevels.ToString();
        
        severityMenu = severitySelector.GetPopup();
        severityMenu.IdPressed += id => OnSeverityLevelSelected((int)id);
        severityMenu.HideOnCheckableItemSelection = false;
        
        loggerSelector = GetNode<MenuButton>("%LoggerSelector");
        loggerMenu = loggerSelector.GetPopup();
        loggerMenu.IndexPressed += idx => OnLoggerSelected((int)idx);
        loggerMenu.HideOnCheckableItemSelection = false;
        
        searchBar = GetNode<LineEdit>("%SearchBar");
        searchBar.TextChanged += _ => RefreshLogDisplay();

        clearButton.Pressed += logList.Clear;

        LogManager.Instance.LoggerRegistered += _ => flagRefreshLoggers = true;
    }

    public override void _Process(double delta)
    {
        timeSinceLastUpdate += (float)delta;

        if (!IsVisibleInTree() || timeSinceLastUpdate < UpdateInterval) return;

        if (flagRefreshLoggers)
        {
            RefreshLoggerList();
        }
        
        UpdateLogDisplay();
        timeSinceLastUpdate = 0f;
    }

    private void UpdateLogDisplay()
    {
        var logs = LogManager.Instance.GetLogsFrom(lastDisplayedLog);
        if (logs.Length == 0) return;

        AddLogsToList(logs);
        lastDisplayedLog += logs.Length;
    }

    private void RefreshLogDisplay()
    {
        logList.Clear();
        lastDisplayedLog = 0;

        var logs = LogManager.Instance.GetAllLogs();
        AddLogsToList(logs);

        lastDisplayedLog = logs.Length;
    }

    private void AddLogsToList(CircularBuffer<LogEntry>.RangeView logs)
    {
        var searchText = searchBar.Text;
        var search = !searchText.IsNullOrEmpty();

        foreach (var log in logs)
        {
            if (!ShouldDisplayLog(log)) continue;
            if (search && !MatchesSearch(searchText, log)) continue;

            var icon = LogManager.GetIcon(log.Severity);
            logList.AddItem(log.GetFormattedText(false), icon);
            
            var itemIndex = logList.ItemCount - 1;
            var color = LogManager.GetColor(log.Severity);
            logList.SetItemCustomFgColor(itemIndex, color);
            logList.SetItemMetadata(itemIndex, log.Index);
        }

        if (autoScrollEnabled && logs.Length > 0) flagScrollToBottom = true;
    }

    private bool ShouldDisplayLog(LogEntry log)
    {
        if (!severityLevels.HasFlag(log.Severity)) return false;
        if (disabledLoggers.Contains(log.LoggerName)) return false;
        
        return true;
    }

    private bool MatchesSearch(string searchText, LogEntry log)
    {
        if (log.Message.Contains(searchText, InvariantCultureIgnoreCase)) return true;
        if (log.LoggerName.Contains(searchText, InvariantCultureIgnoreCase)) return true;

        return false;
    }

    protected override void OnOptionsMenuItemClicked(int menuId, bool isChecked)
    {
        switch (menuId)
        {
            case 0:
                autoScrollEnabled = isChecked;
                UpdateLogDisplay();
                break;
        }
    }

    private void OnSeverityLevelSelected(int id)
    {
        var index = severityMenu.GetItemIndex(id);
        var isChecked = severityMenu.HandleCheckState(index);
        severityLevels.Set((SeverityLevel)id, isChecked);
        severitySelector.Text = severityLevels.ToString();
        RefreshLogDisplay();
    }

    private void OnLoggerSelected(int idx)
    {
        var name = loggerMenu.GetItemText(idx);
        var isChecked = loggerMenu.HandleCheckState(idx);

        if (isChecked) disabledLoggers.Remove(name);
        else disabledLoggers.Add(name);

        loggerSelector.Text = $"{loggerMenu.ItemCount - disabledLoggers.Count} selected";
        RefreshLogDisplay();
    }
    
    public override void SaveState()
    {
        base.SaveState();
        DebugManager.SaveConfig(SeverityLevelKey, severityLevels);
        
        var disabled = disabledLoggers.ToArray();
        DebugManager.SaveConfig(DisabledLoggersKey, disabled);
    }

    public override void RestoreState()
    {
        base.RestoreState();

        severityLevels = DebugManager.LoadConfig(SeverityLevelKey, SeverityLevel.All);
        severitySelector.Text = severityLevels.ToString();
        
        foreach (var level in Enum.GetValues<SeverityLevel>())
        {
            if (level is SeverityLevel.None or SeverityLevel.All) continue;
            
            severityMenu.AddCheckItem(level.ToString(), (int)level);
            var index = severityMenu.GetItemIndex((int)level);
            severityMenu.SetItemChecked(index, severityLevels.HasFlag(level));
        }

        var disabled = DebugManager.LoadConfig(DisabledLoggersKey, Array.Empty<string>());
        disabledLoggers = new HashSet<string>(disabled);

        flagRefreshLoggers = true;
        RefreshLogDisplay();
    }

    private void RefreshLoggerList()
    {
        loggerMenu.Clear();
        foreach (var loggerName in LogManager.Instance.GetLoggers().Order())
        {
            var index = loggerMenu.ItemCount;
            loggerMenu.AddCheckItem(loggerName);
            loggerMenu.SetItemChecked(index, !disabledLoggers.Contains(loggerName));
        }
        
        loggerSelector.Text = $"{loggerMenu.ItemCount - disabledLoggers.Count} selected";
        flagRefreshLoggers = false;
        RefreshLogDisplay();
    }
    
    private void OnLogListDraw()
    {
        if (flagScrollToBottom && !logList.HasFocus())
        {
            listScrollBar.Value = double.MaxValue;
        }

        flagScrollToBottom = false;
    }

    private void OnLogListGuiInput(InputEvent evt)
    {
        if (evt is InputEventKey { Pressed: true, Echo: false } keyEvent )
        {
            if (keyEvent.IsCommandOrControlPressed() && keyEvent.Keycode == Key.C)
            {
                CopySelectedLogs();
            }
        }
    }

    private void CopySelectedLogs()
    {
        var selectedIndices = logList.GetSelectedItems();
        if (selectedIndices.Length == 0) return;

        foreach (var index in selectedIndices)
        {
            var logIdx = logList.GetItemMetadata(index).AsInt32();
            var logEntry = LogManager.Instance.GetLog(logIdx);
            sb.AppendLine(logEntry.GetFormattedText());
        }

        DisplayServer.ClipboardSet(sb.ToString());
        sb.Clear();
    }
}