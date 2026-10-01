using System.Collections;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace ElementGodot.Tags;

[GlobalClass, Tool]
public partial class TagContainer : Resource, IEnumerable<Tag>
{
    [Export] public Array<Tag> _Tags { get; set; } = new();

    public TagContainer() { }

    public TagContainer(HashSet<Tag> p_tags)
    {
        foreach (Tag tag in p_tags)
            Add(tag);
    }

    public void Add(Tag p_tag)
    {
        if (!Contains(p_tag))
            _Tags.Add(p_tag);
    }

    public bool Contains(Tag p_tag)
    {
        foreach (Tag tag in _Tags)
        {
            if (tag.Equals(p_tag))
                return true;
        }

        return false;
    }

    public IEnumerator<Tag> GetEnumerator() => _Tags.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
