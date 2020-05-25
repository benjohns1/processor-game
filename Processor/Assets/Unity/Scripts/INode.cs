using SupplyChain.Graph;
using UnityEngine;

namespace Unity.Scripts
{
    public interface INode
    {
        SupplyChain.Graph.INode GetNode();
        bool Init(NodeGraph g);
        GameObject GameObject();
    }
}