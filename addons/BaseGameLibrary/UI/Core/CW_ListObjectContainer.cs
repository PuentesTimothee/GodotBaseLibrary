using System;
using System.Collections.Generic;
using System.Linq;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.UI.Core;

public interface IControlListWidget
{
    public void _SetData(IControlListObject p_sData);
    public void _Clear();
}

public interface IControlListObject
{
}

[Tool]
[GlobalClass]
public partial class CW_ListObjectContainer : MarginContainer
{
    [Export] public Container? _Container = null;
    [Export] public PackedScene? _LineNode = null;

    private List<IControlListWidget> _currentCreatedChild = new();
    private List<IControlListObject> _currentObjects = new();

    // Warn users if the value hasn't been set.
    public override string[] _GetConfigurationWarnings()
    {
        Array<string> sData = new();
        if (_Container == null)
            sData.Add("Must initialize property '_MenuManagerPacked'.");
        if (_LineNode == null)
            sData.Add("Must initialize property '_LineNode'.");
        return sData.ToArray();
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        foreach (IControlListWidget sChild in _currentCreatedChild)
            (sChild as Node)?.QueueFree();
        _currentCreatedChild.Clear();
        _currentObjects.Clear();
    }

    public void _Clear()
    {
        List<IControlListObject> sList = new(_currentObjects);
        foreach (IControlListObject sObject in sList)
            _RemoveData(sObject);
        foreach (IControlListWidget pControl in _currentCreatedChild)
            pControl._Clear();
        _currentObjects.Clear();
    }


    public void _AddData(IControlListObject p_pObject)
    {
        _currentObjects.Add(p_pObject);

        int nIndex = _currentObjects.IndexOf(p_pObject);
        if (nIndex >= 0 && _TryGetObjectList(nIndex) is { } pInterface)
        {
            if (pInterface is Control pControl && pControl.GetParent() is { } pTrueParent)
                pTrueParent.RemoveChild(pControl);
            _Container?.AddChild(pInterface as Control, false, InternalMode.Back);
            pInterface._SetData(p_pObject);
        }
    }

    public void _RemoveData(IControlListObject p_pObject)
    {
        int nIndex = _currentObjects.IndexOf(p_pObject);
        if (nIndex >= 0 && _TryGetObjectList(nIndex) is { } pInterface)
        {
            if (pInterface is Control pControl)
                if (pControl.GetParent() is { } pTrueParent)
                    _Container?.RemoveChild(pControl);

            _currentObjects.Remove(p_pObject);
            pInterface._Clear();
        }
    }

    private IControlListWidget _TryGetObjectList(int p_nIndex)
    {
        while (p_nIndex >= 0 && _currentCreatedChild.Count <= p_nIndex)
            _AddObjectList(_currentCreatedChild.Count);

        return _currentCreatedChild[p_nIndex];
    }

    private IControlListWidget _AddObjectList(int p_nIndex)
    {
        if (_LineNode is null)
            throw new InvalidOperationException();

        if (_LineNode.Instantiate<Control>() is IControlListWidget widgetInterface)
        {
            _currentCreatedChild.Add(widgetInterface);
            return widgetInterface;
        }

        MyLogger._Log(LogSeverity.e_Default, LogType.e_Error,
            $"Coulnd't add a Control To The ObjectListContainer {Name}");
        throw new InvalidOperationException();
    }
}