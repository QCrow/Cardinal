using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalFlag
{
    public string Value { get; }

    public TerminalFlag(string value)
    {
        Value = value;
    }

    // Optional: Add some utility methods for flag comparisons or checks
    public bool Is(string flag) => Value.Equals(flag, StringComparison.OrdinalIgnoreCase);

    public override string ToString()
    {
        return Value;
    }
}
