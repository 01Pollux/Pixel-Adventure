using UnityEngine;

namespace Mechanics
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Vector2 m_ScrollDelta;
        [SerializeField] private float m_ScrollSpeed;

        private Vector2 m_SpriteSize;
        private Vector2 m_StartPosition;

        private void Start()
        {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture texture = sprite.texture;

            m_SpriteSize.x = texture.width / sprite.pixelsPerUnit;
            m_SpriteSize.y = texture.height / sprite.pixelsPerUnit;

            m_StartPosition = transform.position;
        }

        private void LateUpdate()
        {
            transform.position += (Vector3)m_ScrollDelta * m_ScrollSpeed;
            Vector2 move_offset = m_StartPosition - (Vector2)transform.position;

            if (Mathf.Abs(move_offset.x) >= m_SpriteSize.x)
            {
                float delta_x = move_offset.x % m_SpriteSize.x;
                transform.position = new Vector3(m_StartPosition.x + delta_x, transform.position.y);
            }
            if (Mathf.Abs(move_offset.y) >= m_SpriteSize.y)
            {
                float delta_x = move_offset.y % m_SpriteSize.y;
                transform.position = new Vector3(transform.position.x, m_StartPosition.y + delta_x);
            }
        }
    }
}