using System.Collections.Generic;
using Godot;

namespace ElementGodot.BaseGameLibrary.Tags;

public partial class TagContainer(HashSet<Tag> p_tags) : Resource
{
    public HashSet<Tag> _Tags => p_tags;
    
    public TagContainer() : this(new()){}
    
    public bool Contains(Tag p_tagResource) => p_tags.Contains(p_tagResource);
    public void Add(Tag p_tagResource) => p_tags.Add(p_tagResource);
}