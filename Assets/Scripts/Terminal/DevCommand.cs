using System;

[AttributeUsage(AttributeTargets.Method)]
public class DevCommandAttribute : Attribute
{
    public string Description { get; }
    public string[] Aliases { get; }

    public DevCommandAttribute(string description, params string[] aliases)
    {
        Description = description;
        Aliases = aliases;
    }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class DevCommandParameterAttribute : Attribute
{
    public bool IsOptional { get; }
    public object DefaultValue { get; }

    public DevCommandParameterAttribute(bool isOptional = false, object defaultValue = null)
    {
        IsOptional = isOptional;
        DefaultValue = defaultValue;
    }
}
