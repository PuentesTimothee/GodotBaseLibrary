using System;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Settings;

    

public enum ESettingType
{
    e_Undefined,
    e_Bool,
    e_Int,
    e_Dropdown
}

//public delegate void OnSettingChanged(GameSingleSetting p_setting);

public abstract partial class GameSingleSetting : Resource
{
    public readonly ESettingsSection _Section;
    public readonly ESettingType _Type;
    public bool _NeedApply = true;
    
    public GameSingleSetting(ESettingsSection p_section, ESettingType p_type)
    {
        _Section = p_section;
        _Type = p_type;
    }

    public virtual void Init() {}
    public abstract void LoadValueFromState();
    public abstract bool LoadValueFromSave(Variant p_sVariant);
    public abstract Control MakeControl();
}

public abstract partial class GameSingleSetting_Specific<TType> : GameSingleSetting
{
    protected TType? _Value = default;
    protected TType? _DefaultValue = default;

    protected GameSingleSetting_Specific(ESettingsSection p_section, ESettingType p_type) : base(p_section, p_type)
    {
    }

    public TType? GetValue() => _Value;

    public abstract void OnValueChanged(TType? p_value, TType? p_previous);

    public void SetValue(TType p_setting, bool p_bTriggerCallback = true)
    {
        if (_Value is not null && _Value.Equals(p_setting))
            return;
    
        TType? pPrevious = _Value;
        _Value = p_setting;
        
        if (p_bTriggerCallback)
            OnValueChanged(_Value, pPrevious);
    }

    public abstract bool IsModified();
    
    public void ToDefault(bool p_bTriggerCallback = true)
    {
        if (_Value is not null && _Value.Equals(_DefaultValue))
            return;
    
        TType? pPrevious = _Value;
        _Value = _DefaultValue;
        
        if (p_bTriggerCallback)
            OnValueChanged(_Value, pPrevious);
    }

    public override bool LoadValueFromSave(Variant p_sVariant)
    {
        if (p_sVariant is TType pType)
        {
            SetValue(pType, true);
            return true;
        }

        return false;
    }
}

public abstract partial class GameSingleSetting_Dropdown : GameSingleSetting_Specific<string>
{
    public override Control MakeControl()
    {
        OptionButton sOption = new ();
        int nIndex = 0;
        foreach (string sPossibleValue in _PossiblesValues)
            sOption.AddItem(sPossibleValue, nIndex++);
        sOption.Selected = GetValueIndex();
        sOption.ItemSelected += SetValueByInput;
        return sOption;
    }
    public void SetValueByInput(long p_bValue) => SetValue(_PossiblesValues[Mathf.CeilToInt(p_bValue)]);

    public override bool IsModified() => _Value != null && String.Compare(_Value, _DefaultValue, StringComparison.Ordinal) != 0;

    public Array<string> _PossiblesValues = new();
    
    public int GetValueIndex() => _PossiblesValues.IndexOf(_Value);
    
    public GameSingleSetting_Dropdown(ESettingsSection p_settingsSection) : base(p_settingsSection, ESettingType.e_Dropdown)
    {
    }
}

public abstract partial class GameSingleSetting_Double : GameSingleSetting_Specific<double>
{
    public override Control MakeControl()
    {
        HSlider sOption = new();
        sOption.Step = _Step;
        sOption.MaxValue = _Range._Max;
        sOption.MinValue = _Range._Min;
        sOption.Value = GetValue();
        sOption.ValueChanged += SetValueByInput;
        return sOption;
    }
    public void SetValueByInput(double p_bValue) => SetValue(p_bValue);
    
    public override bool IsModified() => Math.Abs(_Value - _DefaultValue) > 0.001f;
    
    public BaseRange<double> _Range = new (0, 100);
    public int _Step = 1;
    
    public GameSingleSetting_Double(ESettingsSection p_settingsSection) : base(p_settingsSection, ESettingType.e_Int)
    {
    }


}
public abstract partial class GameSingleSetting_Bool : GameSingleSetting_Specific<bool>
{
    public override Control MakeControl()
    {
        CheckButton sOption = new ();
        sOption.ToggleMode = GetValue();
        sOption.Toggled += SetValueByInput;
        return sOption;
    }
    public void SetValueByInput(bool p_bValue) => SetValue(p_bValue);
    
    public override bool IsModified() => _Value != _DefaultValue;

    public GameSingleSetting_Bool(ESettingsSection p_settingsSection) : base(p_settingsSection, ESettingType.e_Bool)
    {
    }
}