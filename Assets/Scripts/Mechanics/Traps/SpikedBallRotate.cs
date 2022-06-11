using Interfaces;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Mechanics.Traps
{
    public class SpikedBallRotate :
        MonoBehaviour,
        IDamageInfo
    {
        [Header("Movements")]
        [SerializeField] private Vector2 m_RotationPoint;
        [Tooltip("If the angle is zero, the object will always rotate 360 degrees")]
        [SerializeField] private float m_RotationAngle = 45f;
        [SerializeField] private float m_RotationSpeed = 1f;
        [SerializeField] private int m_RotationIndex = 1;
        [SerializeField] private bool m_AntiClockwise;

        private Vector3 m_PositionToStart;

        private void Start()
        {
            m_PositionToStart = (Vector3)m_RotationPoint - transform.position;
            StartCoroutine(AnimateRandomChild(GetComponentsInChildren<Animator>().ToList()));
        }


        private void Update()
        {
            float new_rotation = m_RotationSpeed * m_RotationAngle * Time.deltaTime;

            if (m_RotationIndex != 0)
            {
                Vector3 cur_position = (Vector3)m_RotationPoint - transform.position;
                float cur_angle = Vector3.SignedAngle(m_PositionToStart, cur_position, Vector3.forward);

                if (m_RotationIndex == -1 && cur_angle <= -m_RotationAngle)
                    m_RotationIndex = 1;
                else if (m_RotationIndex == 1 && cur_angle >= m_RotationAngle)
                    m_RotationIndex = -1;

                new_rotation *= m_RotationIndex;
            }
            else if (!m_AntiClockwise)
                new_rotation *= -1f;

            transform.RotateAround(m_RotationPoint, Vector3.forward, new_rotation);
        }

        private void OnDrawGizmosSelected() => Gizmos.DrawLine(transform.position, m_RotationPoint);

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.name == "Player" && collision.collider.gameObject.TryGetComponent<IHealth>(out var health))
                health.TakeDamage(this);
        }

        private IEnumerator AnimateRandomChild(List<Animator> animators)
        {
            var rand = new System.Random();
            while (animators.Count > 0)
            {
                int idx = rand.Next(0, animators.Count);
                animators[idx].SetTrigger("Animate");
                animators.RemoveAt(idx);

                yield return new WaitForSeconds(1f / rand.Next(5, 10));
            }
        }


        public GameObject parentObj => gameObject;
        public int Damage => 10;
        public float KnockbackMod => 3f;
    }
}