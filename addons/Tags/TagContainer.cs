using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace ElementGodot.Tags;

[Tool]
[GlobalClass]
public partial class TagContainer : RefCounted
{
    public HashSet<Tag> _Tags;

    public TagContainer(HashSet<Tag> p_tags) => _Tags = p_tags;
    public TagContainer() => _Tags = new();

    public void Add(Tag p_tag) => _Tags.Add(p_tag);

    public bool Contains(Tag p_tag) => _Tags.Contains(p_tag);
}