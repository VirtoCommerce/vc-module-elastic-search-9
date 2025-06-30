using System;

namespace VirtoCommerce.ElasticSearch9.Tests;

[AttributeUsage(AttributeTargets.Method)]
public class PriorityAttribute : Attribute
{
    public int Priority { get; }

    public PriorityAttribute(int priority)
    {
        Priority = priority;
    }
}
