using System.Linq.Expressions;
using Godot;
using Godot.Collections;

namespace ElementGodot.Tags;

public enum EQueryType
{
    e_Undefined,
    e_AnyMatchTag,
    e_ExactMatchTag,
    e_NoMatchTag,
    e_AnyMatchExpression,
    e_ExactMatchExpression,
    e_NoMatchExpression
}

[Tool]
[GlobalClass]
public partial class TagQuery : Resource
{
    [Export]
    public EQueryType _QueryType = EQueryType.e_Undefined;
    [Export]
    public TagContainer _Tags;
    [Export]
    public Array<TagQuery> _Expressions;

    public TagQuery()
    {
        _Tags = new();
        _Expressions = new();
    }

    public TagQuery(Array<TagQuery> p_expressionses)
    {
        _Expressions = p_expressionses;
        _Tags = new ();
    }
    
    public TagQuery(TagContainer p_tags)
    {
        _Tags = p_tags;
        _Expressions = new ();
    }

    public TagQuery SetType(EQueryType p_tag)
    {
        _QueryType = p_tag;
        return this;
    }

    public TagQuery AddExpression(TagQuery p_expression)
    {
        _Expressions.Add(p_expression);
        return this;
    }
    
    public TagQuery AddTag(Tag p_targetTeammate)
    {
        _Tags.Add(p_targetTeammate);
        return this;
    }
    public TagQuery AddTag(StringName p_targetTeammate)
    {
        if (Tag.RequestTag(p_targetTeammate, ETagFetch.e_ThrowOnError) is {} sTag)
            _Tags.Add( sTag);
        return this;
    }

    public bool IsTag()
    {
        return _QueryType == EQueryType.e_AnyMatchExpression ||
               _QueryType == EQueryType.e_NoMatchExpression ||
               _QueryType == EQueryType.e_ExactMatchExpression;
    }
    
    public bool IsExpression()
    {
        return _QueryType == EQueryType.e_AnyMatchExpression ||
               _QueryType == EQueryType.e_NoMatchExpression ||
               _QueryType == EQueryType.e_ExactMatchExpression;
    }
    
    public bool Matches(TagContainer p_tagContainer)
    {
        switch (_QueryType)
        {
            case EQueryType.e_AnyMatchExpression:
                foreach (var tag in _Expressions)
                    if (tag.Matches(p_tagContainer))
                        return true;
                return false;
            case EQueryType.e_ExactMatchExpression:
                foreach (var tag in _Expressions)
                    if (!tag.Matches(p_tagContainer))
                        return false;
                return true;
            case EQueryType.e_NoMatchExpression:
                foreach (var tag in _Expressions)
                    if (tag.Matches(p_tagContainer))
                        return false;
                return true;
            
            case EQueryType.e_AnyMatchTag:
                foreach (var tag in _Tags._Tags)
                    if (p_tagContainer.Contains(tag))
                        return true;
                return false;
            case EQueryType.e_ExactMatchTag:
                foreach (var tag in _Tags._Tags)
                    if (!p_tagContainer.Contains(tag))
                        return false;
                return true;
            case EQueryType.e_NoMatchTag:
                foreach (var tag in _Tags._Tags)
                    if (p_tagContainer.Contains(tag))
                        return false;
                return true;
        }
                        
        return false;
    }
    
    public override void _ValidateProperty(Dictionary p_property)
    {
        if ((_QueryType == EQueryType.e_Undefined || IsExpression())
            && p_property["name"].AsStringName() == PropertyName._Tags)
            p_property["usage"] = (int)PropertyUsageFlags.NoEditor;

        if ((_QueryType == EQueryType.e_Undefined || !IsExpression())
            && p_property["name"].AsStringName() == PropertyName._Tags)
            p_property["usage"] = (int)PropertyUsageFlags.NoEditor;
    }
}