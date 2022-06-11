using System.Collections;
using UnityEngine;

namespace Mechanics
{
    public class PlayerConsumeFruit : MonoBehaviour
    {
        private Animator m_Animator;

        private void Start() => m_Animator = GetComponent<Animator>();

        private IEnumerator CoOnPlayerTouch()
        {
            m_Animator.SetTrigger("Pop");
            EventSystem.GameMessenger.RaiseAsync(EventSystem.EGameEvent.FruitConsume, this);

            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name == "Player")
            {
                this.enabled = false;
                StartCoroutine(CoOnPlayerTouch());
            }
        }
    }
}