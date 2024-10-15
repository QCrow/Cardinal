using UnityEngine;

public class MapNode
{
    public int Row;
    public int Col;
    public MapNodeType NodeType;

    public bool IsVisited;

    public MapNode(MapNodeType nodeType)
    {
        NodeType = nodeType;
    }

    public void OnEnter()
    {

    }
}