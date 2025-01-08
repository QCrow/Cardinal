using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class Map
    {
        public List<Node> nodes;
        public List<Vector2Int> path;
        public Node lastNode;
        public string configName; // similar to the act name in Slay the Spire

        public Map(string configName, Node lastNode, List<Node> nodes, List<Vector2Int> path)
        {
            this.configName = configName;
            this.lastNode = lastNode;
            this.nodes = nodes;
            this.path = path;
        }

        //public Node GetLastNode()
        //{
        //    return nodes.FirstOrDefault(n => n.nodeType == NodeType.Boss);
        //}

        public float DistanceBetweenFirstAndLastLayers()
        {
            //Node lastNode = GetLastNode();
            Node firstLayerNode = nodes.FirstOrDefault(n => n.point.y == 0);

            if (lastNode == null || firstLayerNode == null)
                return 0f;

            return lastNode.position.y - firstLayerNode.position.y;
        }

        public Node GetNode(Vector2Int point)
        {
            return nodes.FirstOrDefault(n => n.point.Equals(point));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}

