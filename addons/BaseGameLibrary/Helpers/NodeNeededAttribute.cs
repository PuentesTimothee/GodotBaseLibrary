using System;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary.Helpers;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class NodeNeededAttribute : Attribute;
