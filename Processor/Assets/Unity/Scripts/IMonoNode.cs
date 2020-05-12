using SupplyChain.Graph;

namespace Unity.Scripts
{
    public interface IMonoNode
    {
        INode GetNode();
        bool Init(NodeGraph g);
    }
}