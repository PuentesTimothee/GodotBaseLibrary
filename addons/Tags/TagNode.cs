using System.Linq;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;
using Microsoft.VisualBasic;

namespace ElementGodot.Tags;

[Tool]
public partial class TagNode(TagNode? p_parentNode, StringName p_keyName) : RefCounted
{
    public TagNode() : this(null, "")
    {

    }

    public TagNode? _ParentTagNode = p_parentNode;
    public StringName _TagKey = p_keyName;

    public Array<TagNode> _Childs = new();

    public override string ToString() => _TagKey;

    public Variant SerializeToVariant()
    {
        Array sChilds = new();
        foreach (TagNode sNode in _Childs)
            sChilds.Add(sNode.SerializeToVariant());
        if (sChilds.Count > 0)
        {
            if (_TagKey.IsEmpty)
                return sChilds;
            Dictionary sOut = new();
            sOut[_TagKey] = sChilds;
            return sOut;
        }
        return _TagKey;
    }
    
    public StringName CompleteTagKey()
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

    public TagNode? FindLinkedNode(string[] p_childName, int nIndex = 0)
    {
        if (p_childName.Length <= nIndex) return null;

        foreach (TagNode sNode in _Childs)
        {
            if (sNode._TagKey == p_childName[nIndex])
            {
                if (p_childName.Length <= nIndex)
                    return sNode;
                return sNode.FindLinkedNode(p_childName, nIndex + 1);
            }
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

    public void AddChild(Variant p_pData)
    {
        TagNode? sFoundChild = null;
        
        if (p_pData.VariantType == Variant.Type.Array && p_pData.AsGodotArray() is { } pArray)
        {
            foreach (Variant sKey in pArray)
            {
                if (sKey.VariantType == Variant.Type.Dictionary && sKey.AsGodotDictionary() is { } sDictionary)
                {
                    string sName = sDictionary.Keys.First().AsString();
                    Variant sChilds = sDictionary.Values.First();
                    sFoundChild = _Childs._FindByPredicate(p_sParsed => p_sParsed._TagKey == sName) ?? new(this, sName);
                    sFoundChild.AddChild(sChilds);
                }
                else if (sKey.VariantType == Variant.Type.String && sKey.AsString() is { } sString)
                    sFoundChild = _Childs._FindByPredicate(p_sParsed => p_sParsed._TagKey == sString) ?? new(this, sString);

                if (sFoundChild != null)
                    _Childs.Add(sFoundChild);
            }
        }
        else if (p_pData.VariantType == Variant.Type.String && p_pData.AsString() is { } sString)
        {
            
        }

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