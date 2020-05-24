using UnityEngine;

namespace Unity.Scripts
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class LineCollider : MonoBehaviour
    {
        [SerializeField] private float width = 0.2f;
        private BoxCollider2D col;
        
        private void Awake()
        {
            col = GetComponent<BoxCollider2D>();
        }

        public void SetCollider(Vector2 start, Vector2 end, float z)
        {
            var length = Vector3.Distance(start, end);
            col.size = new Vector2(length, width);
            var midpoint = (start + end) / 2;
            var rotation = Quaternion.FromToRotation(Vector2.right, end - start);
            col.transform.SetPositionAndRotation(new Vector3(midpoint.x, midpoint.y, z), rotation);
        }
    }
}