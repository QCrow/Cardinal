using System;

[AttributeUsage(AttributeTargets.Method)]
public class DevCommandAttribute : Attribute
{
    public string Description { get; }

    public string Usage { get; }
    public string[] Aliases { get; }
    public string[] ValidFlags { get; }

    public DevCommandAttribute(string description, string usage, string[] validFlags, params string[] aliases)
    {
        Description = description;
        Aliases = aliases;
        ValidFlags = validFlags;
        Usage = usage;
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
