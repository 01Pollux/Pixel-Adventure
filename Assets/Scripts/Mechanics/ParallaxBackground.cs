using UnityEngine;

namespace Mechanics
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private float m_RepeatDistance = 24f;
        [SerializeField] private Vector2 m_MoveDistance = new(1f, -.5f);
        [SerializeField] private float m_MoveSpeed = 1.5f;

        private void Update()
        {
            transform.localPosition += (Vector3)(m_MoveSpeed * Time.deltaTime * m_MoveDistance);

            if (Vector2.Distance(transform.localPosition, Vector2.zero) > m_RepeatDistance)
                transform.localPosition = Vector3.zero;
        }
    }
}