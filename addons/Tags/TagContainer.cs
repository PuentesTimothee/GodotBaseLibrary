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

[Tool]
[GlobalClass]
public partial class TagContainerResource : Resource
{
    [Export]
    public Array<StringName> _Tags { get; set; }

    public TagContainer GetTagContainer()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_Tags is null)
            return new TagContainer();

        HashSet<Tag> tags = new();
        foreach (string tag in _Tags)
        {
            try
            {
                tags.Add(Tag.RequestTag(tag));
            }
            catch (TagNotRegisteredException)
            {
                GD.PushWarning($"Tag [{tag}] is not registered.");
            }
        }

        return new TagContainer(tags);
    }
    
    public TagContainerResource() => _Tags = new();
    public TagContainerResource(Array<StringName> p_tags) => _Tags = p_tags;
    
    public bool Contains(Tag p_tagResource) => _Tags.Contains(p_tagResource.FullName());
    public void Add(Tag p_tagResource) => _Tags.Add(p_tagResource.FullName());
}