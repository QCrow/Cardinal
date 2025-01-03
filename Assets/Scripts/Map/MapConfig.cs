using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Map
{
    [CreateAssetMenu]
    public class MapConfig : ScriptableObject
    {
        public List<NodeBlueprint> nodeBlueprints;

        [Tooltip("Nodes that will be used on layers with Randomize Nodes > 0")]
        public List<NodeType> randomNodes = new List<NodeType>
        {
            NodeType.MysteryEvent, NodeType.Store, NodeType.Treasure, NodeType.MinorEnemy, NodeType.RestSite
        };

        public int GridWidth => Mathf.Max(numOfPreBossNodes.max, numOfStartingNodes.max);

        [TitleGroup("Node Counts")]
        [LabelText("Pre-Boss Nodes")]
        public IntMinMax numOfPreBossNodes;

        [TitleGroup("Node Counts")]
        [LabelText("Starting Nodes")]
        public IntMinMax numOfStartingNodes;

        [Tooltip("Increase this number to generate more paths")]
        public int extraPaths;

        public List<MapLayer> layers;
    }
}
