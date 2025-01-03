using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ConditionNode
{
    public ConditionNodeType nodeType;

    [SerializeReference] private BaseCondition leafCondition;
    [SerializeField] private List<ConditionNode> children;

    public bool Evaluate()
    {
        switch (nodeType)
        {
            case ConditionNodeType.Leaf:
                return leafCondition != null && leafCondition.Evaluate();

            case ConditionNodeType.And:
                foreach (var child in children)
                {
                    if (!child.Evaluate()) return false;
                }
                return true;

            case ConditionNodeType.Or:
                foreach (var child in children)
                {
                    if (child.Evaluate()) return true;
                }
                return false;

            case ConditionNodeType.Not:
                // Usually just one child for NOT
                if (children.Count > 0)
                {
                    return !children[0].Evaluate();
                }
                return true;
        }
        return false;
    }
}


public enum ConditionNodeType
{
    Leaf,
    And,
    Or,
    Not
}
