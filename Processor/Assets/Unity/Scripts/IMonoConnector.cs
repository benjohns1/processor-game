using SupplyChain.Graph;

namespace Unity.Scripts
{
    public interface IMonoConnector
    {
        IConnector GetConnector();
    }
}