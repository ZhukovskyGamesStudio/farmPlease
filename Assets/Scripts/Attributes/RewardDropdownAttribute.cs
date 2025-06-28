
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class RewardDropdownAttribute : PropertyAttribute {
    public Type[] EnumTypes { get; }

    public RewardDropdownAttribute(params Type[] enumTypes) {
        EnumTypes = enumTypes;
    }
}
