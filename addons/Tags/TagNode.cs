using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Tags;

public partial class TagNode(TagNode? p_ParentNode, StringName p_keyName) : GodotObject
{
    public TagNode() : this(null, "")
    {
        
    }
    
    public TagNode? _ParentTagNode = p_ParentNode;
    public Array<TagNode> _Childs = new();
    public StringName _TagKey = p_keyName;

    public StringName _CompleteTagKey 
    {
        get
        {
            string sName = _TagKey;
            TagNode? node = _ParentTagNode;
            while (node is not null && node._TagKey != "")
            {
                sName = node._TagKey + "." + sName;
                node = node._ParentTagNode;
            }
            return sName;
        }
    }
    
    public TagNode? FindLinkedNode(string[] p_childName, int nIndex = 0)
    {
        if (p_childName.Length <= nIndex) return null;
        
        foreach (TagNode sNode in _Childs)
        {
            if (sNode._TagKey == p_childName[nIndex])
                return sNode.FindLinkedNode(p_childName, nIndex + 1);
        }

        return null;
    }
    
    public bool IsChildOf(TagNode p_possibleParent)
    {
        if (this == p_possibleParent)
            return true;

        if (_ParentTagNode is null)
            return false;
        
        return _ParentTagNode.IsChildOf(p_possibleParent);
    }
    
    public void AddChild(string[] p_childName, int nIndex = 0)
    {
        if (p_childName.Length <= nIndex) return;
        
        foreach (TagNode sNode in _Childs)
        {
            if (sNode._TagKey == p_childName[nIndex])
            {
                sNode.AddChild(p_childName, nIndex + 1);
                return;
            }
        }

        TagNode sNew = new(this, p_childName[nIndex]);
        sNew._TagKey = p_childName[nIndex];
        sNew.AddChild(p_childName, nIndex + 1);
        _Childs.Add(sNew);
    }

    public void DestroyNodes()
    {
        foreach (TagNode sNode in _Childs)
        {
            sNode.DestroyNodes();
            sNode.Free();
        }
        _Childs.Clear();
    }
}