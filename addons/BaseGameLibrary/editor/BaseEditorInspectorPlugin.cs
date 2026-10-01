using System;
using System.Collections.Generic;
using System.Reflection;

using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.editor;

#if TOOLS
[Tool]
public partial class BaseEditorInspectorPlugin : EditorInspectorPlugin
{
    public bool _CanHandleIfContains(GodotObject p_object, HashSet<Type> p_TypeToCheck)
    {
        if (DEBUG_LOGS)
            GD.Print($"[TestBoxInspectorPlugin] _CanHandle called for object with ID {p_object} / {p_object.GetScript()}");
		
        Type type = p_object.GetType();
        PropertyInfo[] sProperties =
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (var property in sProperties)
        {
            if (p_TypeToCheck.Contains(property.PropertyType))
            {
                if (DEBUG_LOGS)
                    GD.Print($"[{GetType()}] Found {property.PropertyType} properties: {property.Name}");
                return true;
            }
        }

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            if (p_TypeToCheck.Contains(field.FieldType))
            {
                if (DEBUG_LOGS)
                    GD.Print($"[{GetType()}] Found {field.FieldType} field: {field.Name}");
                return true;
            }
        }
		
        return false;
    }
    
    protected HashSet<Type> _HandledTypeOfValue = new ();

    public override bool _CanHandle(GodotObject p_object) => _CanHandleIfContains(p_object, _HandledTypeOfValue);

    public virtual bool _ParseHandledProperty(
        GodotObject p_object,
        MemberInfo? p_memberInfo,
        Type p_memberType,
        string p_name,
        PropertyHint p_hintType,
        string p_hintString)
    {
        return false;
    }

    public override bool _ParseProperty(
        GodotObject p_object,
        Variant.Type p_type,
        string p_name,
        PropertyHint p_hintType,
        string p_hintString,
        PropertyUsageFlags p_usageFlags,
        bool p_wide)
    {
        Type type = p_object.GetType();

        if (type.GetProperty(p_name) is {} sProperty && _HandledTypeOfValue.Contains(sProperty.PropertyType))
            return _ParseHandledProperty(p_object, sProperty, sProperty.PropertyType, p_name, p_hintType, p_hintString);
        if (type.GetField(p_name) is {} sField && _HandledTypeOfValue.Contains(sField.FieldType))
            return _ParseHandledProperty(p_object, sField, sField.FieldType, p_name, p_hintType, p_hintString);

        return false;
    }

    private const bool DEBUG_LOGS = true;
}
#endif