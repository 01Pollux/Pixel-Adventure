using UnityEngine;

namespace Mechanics
{
    public class RandomBackgroundSelector : MonoBehaviour
    {
        [SerializeField] Sprite[] m_Sprites;

        private void Awake()
        {
            SpriteRenderer sprite_renderer = GetComponent<SpriteRenderer>();
            sprite_renderer.sprite = m_Sprites[Random.Range(0, m_Sprites.Length - 1)];
        }
    }
}