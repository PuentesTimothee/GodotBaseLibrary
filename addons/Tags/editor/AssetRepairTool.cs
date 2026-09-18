// Copyright © Gamesmiths Guild.

#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Tags.editor;

[Tool]
public partial class AssetRepairTool : EditorPlugin
{
	public static void RepairAllAssetsTags()
	{
		var tagsManager = TagsManager.Instance;

		List<string> scenes = AssetRepairTool.GetScenePaths("res://");
		GD.Print($"Found {scenes.Count} scene(s) to process.");

		string[] openedScenes = EditorInterface.Singleton.GetOpenScenes();

		foreach (string originalScenePath in scenes)
		{
			// For some weird reason scenes from the GetScenePath are coming with 3 slashes instead of just two.
			string scenePath = originalScenePath.Replace("res:///", "res://");

			GD.Print($"Processing scene: {scenePath}.");
			PackedScene? packedScene = ResourceLoader.Load<PackedScene>(scenePath);

			if (packedScene is null)
			{
				GD.PrintErr($"Failed to load scene: {scenePath}.");
				continue;
			}

			Node sceneInstance = packedScene.Instantiate();
			bool modified = AssetRepairTool.ProcessNode(sceneInstance, tagsManager);

			if (!modified)
			{
				GD.Print($"No changes needed for {scenePath}.");
				continue;
			}

			// 'sceneInstance' is the modified scene instance in memory, need to save to disk and reload if needed.
			var newScene = new PackedScene();
			Error error = newScene.Pack(sceneInstance);
			if (error != Error.Ok)
			{
				GD.PrintErr($"Failed to pack scene: {error}.");
				continue;
			}

			error = ResourceSaver.Save(newScene, scenePath);
			if (error != Error.Ok)
			{
				GD.PrintErr($"Failed to save scene: {error}.");
				continue;
			}

			if (openedScenes.Contains(scenePath))
			{
				GD.Print($"Scene was opened, reloading background scene: {scenePath}.");
				EditorInterface.Singleton.ReloadSceneFromPath(scenePath);
			}
		}
	}

	/// <summary>
	/// Recursively get scene files from a folder.
	/// </summary>
	/// <param name="p_basePath">Current path iteration.</param>
	/// <returns>List of scenes found.</returns>
	private static List<string> GetScenePaths(string p_basePath)
	{
		var scenePaths = new List<string>();
		var dir = DirAccess.Open(p_basePath);

		if (dir is null)
		{
			GD.PrintErr($"Failed to open directory: {p_basePath}");
			return scenePaths;
		}

		// Start listing directory entries; skip navigational and hidden files.
		dir.ListDirBegin();
		while (true)
		{
			string fileName = dir.GetNext();
			if (string.IsNullOrEmpty(fileName))
			{
				break;
			}

			string filePath = $"{p_basePath}/{fileName}";
			if (dir.CurrentIsDir())
			{
				// Recursively scan subdirectories.
				scenePaths.AddRange(AssetRepairTool.GetScenePaths(filePath));
			}
			else if (fileName.EndsWith(".tscn", StringComparison.InvariantCultureIgnoreCase)
				|| fileName.EndsWith(".scn", StringComparison.InvariantCultureIgnoreCase))
			{
				scenePaths.Add(filePath);
			}
		}

		dir.ListDirEnd();
		return scenePaths;
	}

	/// <summary>
	/// Recursively process nodes; returns true if any ForgeEntity was modified.
	/// </summary>
	/// <param name="p_node">Current node iteration.</param>
	/// <param name="p_tagsManager">The tags manager used to validate tags.</param>
	/// <returns><see langword="true"/> if any ForgeEntity was modified.</returns>
	private static bool ProcessNode(Node p_node, TagsManager p_tagsManager)
	{
		bool modified = AssetRepairTool.ValidateNode(p_node, p_tagsManager);

		foreach (Node child in p_node.GetChildren())
		{
			modified |= AssetRepairTool.ProcessNode(child, p_tagsManager);
		}

		return modified;
	}

	private static bool ValidateNode(Node p_node, TagsManager p_tagsManager)
	{
		bool modified = false;
		foreach (Dictionary propertyInfo in p_node.GetPropertyList())
		{
			if (!propertyInfo.TryGetValue("class_name", out Variant className))
			{
				continue;
			}

			if (className.AsString() != "TagContainer")
			{
				continue;
			}

			if (!propertyInfo.TryGetValue("name", out Variant nameObj))
			{
				continue;
			}

			string propertyName = nameObj.AsString();
			Variant value = p_node.Get(propertyName);

			if (value.VariantType != Variant.Type.Object)
			{
				continue;
			}

			if (value.As<Resource>() is TagContainer tagContainer)
			{
				modified |= AssetRepairTool.ValidateTagContainerProperty(tagContainer, p_node.Name, p_tagsManager);
			}
		}

		return modified;
	}

	private static bool ValidateTagContainerProperty(
		TagContainer p_container,
		string p_nodeName,
		TagsManager p_tagsManager)
	{
		HashSet<Tag> originalTags = p_container._Tags;
		var newTags = new HashSet<Tag>();
		bool modified = false;

		foreach (Tag tag in originalTags)
		{
			try
			{
				Tag.RequestTag(tag._StringTag, ETagFetch.e_ThrowOnError);
				newTags.Add(tag);
			}
			catch (Exception)
			{
				GD.PrintRich(
					$"[color=LIGHT_STEEL_BLUE][RepairTool] Removing invalid tag [{tag}] from node {p_nodeName}.");
				modified = true;
			}
		}

		if (modified)
		{
			p_container._Tags.Clear();
			foreach (Tag tag in newTags)
				p_container._Tags.Add(tag);
		}

		return modified;
	}
}
#endif
