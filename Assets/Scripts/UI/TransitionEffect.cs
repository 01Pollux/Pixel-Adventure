using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TransitionEffect : MonoBehaviour
    {
        //[SerializeField] private Sprite m_TransitionSprite;
        [SerializeField] private Vector2Int m_Size;

        [SerializeField] private Texture2D m_TransitionTexture;
        private Color32[] m_TransitionColor;

        void Start()
        {
            RawImage target_image = GetComponent<RawImage>();

            m_TransitionTexture = new(m_Size.x * 10, m_Size.y * 10)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };

            target_image.texture = m_TransitionTexture;

            m_TransitionColor = new Color32[m_Size.x * m_Size.y];
            for (int i = 0; i < m_Size.x; i++)
            {
                for (int j = 0; j < m_Size.y; j++)
                    m_TransitionColor[i * m_Size.y + j] = Color.red;
            }

            //Color32[] empty_color = new Color32[m_TransitionSprite.texture.width * 10 * m_TransitionSprite.texture.height * 10];
            //m_TransitionTexture.SetPixels32(empty_color);

            //m_TransitionPixels = new Color32[m_TransitionSprite.texture.width * m_TransitionSprite.texture.height];
            //for (int i = 0; i < m_TransitionPixels.Length; i++)
            //    m_TransitionPixels[i] = Color.red;


            StartCoroutine(CoStartTransition());
        }

        private IEnumerator CoStartTransition()
        {
            Vector2Int xy_end = Vector2Int.zero;
            Vector2Int xy_inverse = Vector2Int.zero;

            Vector2Int xy = Vector2Int.zero;
            Vector2Int xy_limit = Vector2Int.one * 10;

            while (true)
            {
                Debug.Log(xy);

                for (int i = 0; i < 2; i++)
                {
                    if (xy_end[i] == 1)
                        continue;

                    if (xy_inverse[i] == 0)
                    {
                        if (i == 0)
                            m_TransitionTexture.SetPixels32(xy.x * m_Size.x, 0, m_Size.x, m_Size.y, m_TransitionColor);
                        else
                            m_TransitionTexture.SetPixels32(0, xy.y * m_Size.y, m_Size.x, m_Size.y, m_TransitionColor);
                    }
                    else
                    {
                        if (i == 0)
                            m_TransitionTexture.SetPixels32(((10 - xy.x) % 10) * m_Size.x, m_Size.y * 9, m_Size.x, m_Size.y, m_TransitionColor);
                        else
                            m_TransitionTexture.SetPixels32(m_Size.x * 9, ((10 - xy.y) % 10) * m_Size.y, m_Size.x, m_Size.y, m_TransitionColor);
                    }
                }

                m_TransitionTexture.Apply();

                for (int i = 0; i < 2; i++)
                {
                    if (xy_end[i] == 0)
                    {
                        if (xy_inverse[i] == 0)
                        {
                            xy[i]++;
                            if (xy[i] >= xy_limit[i])
                            {
                                xy[i] = xy_limit[i] - 1;
                                xy_inverse[i] = 1;
                            }
                        }
                        else
                        {
                            xy[i]--;
                            if (xy[i] < 0)
                            {
                                xy[i] = 0;
                                xy_end[i] = 1;
                            }
                        }
                    }
                }

                if (xy_end.x == 1 && xy_end.y == 1)
                    break;

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}