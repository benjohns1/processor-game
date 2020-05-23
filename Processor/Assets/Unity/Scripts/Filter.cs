using System;
using SupplyChain;

namespace Unity.Scripts
{
    [Serializable]
    public class Filter
    {
        public bool allShapes;
        public Shape[] includeShapes;
        public Shape[] excludeShapes;

        public SupplyChain.Filter GetFilter()
        {
            return new SupplyChain.Filter(allShapes, includeShapes, excludeShapes);
        }
    }
}