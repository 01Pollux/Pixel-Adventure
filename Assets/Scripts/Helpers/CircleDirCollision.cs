using Interfaces;
using UnityEngine;

namespace Helpers
{
    public struct CircleDirCollision : DirCollision
    {
        private readonly BoxCollider2D m_BoxCollider;
        private readonly Vector2[] m_Points;

        private readonly LayerMask m_GroundMask;
        private readonly float m_Radius;

        public TouchType touchType
        {
            get; 
            set;
        }


        public CircleDirCollision(BoxCollider2D bc, float radius, LayerMask layer)
        {
            m_BoxCollider = bc;

            var extent = bc.bounds.extents;
            m_Points = new Vector2[4];

            for (int i = 0; i < 2; i++)
            {
                m_Points[2 * i].x = extent.x * (i == 0 ? 1f : -1f);
                m_Points[2 * i + 1].y = extent.y * (i == 0 ? 1f : -1f);
            }

            m_Radius = radius;
            m_GroundMask = layer;

            touchType = TouchType.None;
        }


        public Collider2D CollidesWith(Vector2 point, LayerMask mask) =>
            Physics2D.OverlapCircle((Vector2)m_BoxCollider.bounds.center + point, m_Radius, mask);


        public void Update()
        {
            touchType = TouchType.None;

            for (int i = 0; i < 4; i++)
            {
                if (CollidesWith(m_Points[i], m_GroundMask))
                    touchType |= (TouchType)(1 << i);
            }
        }


        public void DrawGizmos(Color[] color)
        {
            for (int i = 0; i < 4; i++)
            {
                Gizmos.color = color[i];
                Gizmos.DrawWireSphere((Vector2)m_BoxCollider.bounds.center + m_Points[i], .25f);
                Gizmos.DrawWireSphere((Vector2)m_BoxCollider.bounds.center + m_Points[i], m_Radius);
            }
        }
    }
}
