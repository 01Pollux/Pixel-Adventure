using Interfaces;
using UnityEngine;

namespace Helpers
{
    public struct BoxDirCollision : DirCollision
    {
        private BoxCollider2D m_Collider;
        private LayerMask m_GroundMask;
        private Vector2 m_Up, m_Right;

        public TouchType touchType
        {
            get;
            set;
        }


        public BoxDirCollision(BoxCollider2D collider, UnityEngine.Transform transform, LayerMask layer) :
            this(collider, (Vector2)transform.up, (Vector2)transform.right, layer)
        { }

        public BoxDirCollision(BoxCollider2D collider, Vector2 up, Vector2 right, LayerMask layer)
        {
            m_Collider = collider;
            m_GroundMask = layer;

            m_Up = up;
            m_Right = right;
            touchType = TouchType.None;
        }


        public RaycastHit2D CollidesWith(Vector2 direction, LayerMask mask, float distance = 0.05f) =>
            Physics2D.BoxCast(m_Collider.bounds.center, m_Collider.bounds.size, 0f, direction, distance, mask);

        public void Update()
        {
            touchType = TouchType.None;

            if (CollidesWith(m_Right * -1, m_GroundMask))   touchType |= TouchType.Left;
            if (CollidesWith(m_Right, m_GroundMask))        touchType |= TouchType.Right;
            
            if (CollidesWith(m_Up, m_GroundMask))        touchType |= TouchType.Top;
            if (CollidesWith(m_Up * -1, m_GroundMask))   touchType |= TouchType.Bottom;
        }
    }
}
