using System;
using SupplyChain;

namespace Unity.Scripts
{
    [Serializable]
    public class UnityFilter
    {
        public bool allShapes;
        public Shape[] includeShapes;
        public Shape[] excludeShapes;

        public Filter GetFilter()
        {
            return new Filter(allShapes, includeShapes, excludeShapes);
        }
    }
}