using System.IO;
using System.Linq;
using ElementGodot.BaseGameLibrary.Helpers;
using ElementGodot.BaseGameLibrary.Tags;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.UI.Core;

[GlobalClass]
public partial class CW_MenuManager : CW_Control
{
    public static Tag Tag_CombatMenu = null!;
    public static Tag Tag_FloorSelection = null!;
    
    public static Tag Tag_SettingMenu = null!;

    public static Tag Tag_MainMenu = null!;
    public static Tag Tag_MainMenu_StartGame = null!;
    
    public static Tag Tag_PauseMenu = null!;
    
    protected System.Collections.Generic.Dictionary<Tag, CW_MenuContainer> _MenuContainers = new();
    [Export] public Control? _Root = null!;
    
    protected Array<CW_MenuContainer> _Stack = new();

    // Warn users if the value hasn't been set.
    public override string[] _GetConfigurationWarnings()
    {
        Array<string> sData = new();
        if (_Root == null)
            sData.Add("Must initialize property '_Root'.");
        return sData.ToArray();
    }
    
    public CW_MenuManager()
    {
        CW_MenuManager.Tag_CombatMenu = Tag.RequestTag("menu.combat", ETagFetch.e_CreateOnError);
        CW_MenuManager.Tag_FloorSelection = Tag.RequestTag("menu.floor_selection", ETagFetch.e_CreateOnError);
        
        CW_MenuManager.Tag_SettingMenu = Tag.RequestTag("menu.setting", ETagFetch.e_CreateOnError);

        CW_MenuManager.Tag_PauseMenu = Tag.RequestTag("menu.pause", ETagFetch.e_CreateOnError);
        
        CW_MenuManager.Tag_MainMenu = Tag.RequestTag("menu.main", ETagFetch.e_CreateOnError);
        CW_MenuManager.Tag_MainMenu_StartGame = Tag.RequestTag("menu.main.start", ETagFetch.e_CreateOnError);
    }
    
    public override void _Ready()
    {
        base._Ready();
        
        _Root ??= this;

        foreach (Node child in _Root.GetChildren())
        {
            if (child is CW_MenuContainer sContainer)
            {
                if (!_MenuContainers.TryAdd(sContainer._LinkedTag, sContainer))
                    throw new InvalidDataException("Duplicate menu detected");
                sContainer.On_Activated += OnMenuActivated;
                sContainer.On_Deactivated += OnMenuDeactivated;
            }
        }
    }

    //
    //
    // Open menu
    public void _OpenMenu(Tag p_menuTag)
    {
        if (_MenuContainers.TryGetValue(p_menuTag, out CW_MenuContainer? menuContainer) && !menuContainer._IsActivated)
            menuContainer._Activate();
        else
            MyLogger._LogErrorCommon($"MenuManager._OpenMenu: {p_menuTag} already openned");
    }
    
    private void OnMenuActivated(CW_ActivatableContainer p_obj) => _OpenMenuInternal(p_obj as CW_MenuContainer);
    protected void _OpenMenuInternal(CW_MenuContainer? p_menuContainer)
    {
        if (p_menuContainer is null || _Stack.Contains(p_menuContainer))
            return;
        _Root!.MoveChild(p_menuContainer, -1);
        _Stack.Add(p_menuContainer);
    }
    // Open menu
    //
    //
    
    //
    //
    // Close menu
    public void _CloseAllMenu()
    {
        Array<CW_MenuContainer> sTag = new(_Stack);
        foreach (CW_MenuContainer menuContainer in sTag)
            _CloseMenu(menuContainer._LinkedTag);
    }
    

    public void _CloseMenu(Tag p_menuTag)
    {
        if (_MenuContainers.TryGetValue(p_menuTag, out CW_MenuContainer? menuContainer) && menuContainer._IsActivated)
        {
            _Stack.Remove(menuContainer);
            menuContainer._Deactivate();
        }
        else
            MyLogger._LogErrorCommon($"MenuManager._CloseMenu: {p_menuTag} not openned");
    }
    private void OnMenuDeactivated(CW_ActivatableContainer p_obj) => _CloseMenuInternal(p_obj as CW_MenuContainer);
    protected void _CloseMenuInternal(CW_MenuContainer? p_menuContainer)
    {
        if (p_menuContainer is null || !_Stack.Contains(p_menuContainer))
            return;
        _Stack.Remove(p_menuContainer);
    }
    // Close menu
    //
    //
}