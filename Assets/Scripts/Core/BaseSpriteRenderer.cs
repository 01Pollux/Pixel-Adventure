using UnityEngine;

namespace Core
{
    public class BaseMoveable
    {
        private Rigidbody2D m_Rigidbody;

        public BaseMoveable(Rigidbody2D rigidbody) => m_Rigidbody = rigidbody;


        public Rigidbody2D @Rigidbody => m_Rigidbody;

        public Vector2 Velocity
        {
            get => m_Rigidbody.velocity;
            set => m_Rigidbody.velocity = value;
        }
        
        public float YVelocity
        {
            get => Velocity.y;
            set => Velocity = new(Velocity.x, value);
        }

        public float XVelocity
        {
            get => Velocity.x;
            set => Velocity = new(value, Velocity.y);
        }
    }
}
