using System.Linq;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;

namespace ElementGodot.Scripts.UI.Tooltip;

public partial class CW_Tooltip : PanelContainer
{
    public record ContainerHandle(Container _Handle, CW_Tooltip _Owner)
    {
        /*~ContainerHandle()
        {
            if (IsInstanceValid(_Owner) && !_Owner.IsQueuedForDeletion())
                _Owner._currentContainerStack.Remove(_Handle);
        }*/
    }

    private Array<Container> _currentContainerStack = new ();
    private Container _currentContainer => _currentContainerStack.Last();

    private SizeFlags _vSizeFlags = SizeFlags.Expand;
    private SizeFlags _hSizeFlags = SizeFlags.ExpandFill;
    
    public CW_Tooltip()
    {
        CustomMinimumSize = new(250, 10);
    }

    public void SetSizeFlags(SizeFlags p_vFlag = SizeFlags.ExpandFill, SizeFlags p_hFlag = SizeFlags.ExpandFill)
    {
        _hSizeFlags = p_hFlag;
        _vSizeFlags = p_vFlag;
    }

    public RichTextLabel _Label_Tr_Format(string p_string, params object?[] p_args) => _Label(this.Tr_Format(p_string, p_args));
    public RichTextLabel _Label_Tr(string p_string) => _Label(Tr(p_string));

    public RichTextLabel _Label(string p_label)
    {
        RichTextLabel sLabel = new RichTextLabel();
        sLabel.Text = p_label;
        sLabel.FitContent = true;
        _currentContainer.AddChild(sLabel);
        
        sLabel.BbcodeEnabled = true;

        sLabel.SizeFlagsHorizontal = _hSizeFlags;
        sLabel.SizeFlagsVertical = _vSizeFlags;
        
        return sLabel;
    }
    
    public ContainerHandle _BeginContainer<TContainer>() where TContainer : Container, new()
    {
        TContainer sBox = new TContainer();
        if (_currentContainerStack.Count == 0)
            AddChild(sBox);
        else
            _currentContainer.AddChild(sBox);
        _currentContainerStack.Add(sBox);


        sBox.SizeFlagsHorizontal = _hSizeFlags;
        sBox.SizeFlagsVertical = _vSizeFlags;

        return new (sBox, this);
    }
    
    public bool _EndContainer<TContainer>() where TContainer : Container, new()
    {
        if (_currentContainer is TContainer)
            _currentContainerStack.Remove(_currentContainer);
        return true;
    }
}