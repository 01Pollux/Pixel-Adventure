using UnityEngine;
using EventSystem;
using System;
using System.Threading;

namespace UI
{
    public class FruitText : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI m_FruitText;
        private int m_FruitCount;
        private bool m_FruitChanged;

        private void Start()
        {
            m_FruitText = GetComponent<TMPro.TextMeshProUGUI>();
        }

        private void OnEnable() => GameMessenger.AddListener(EGameEvent.FruitConsume, OnFruitConsume);
        private void OnDisable() => GameMessenger.RemoveListener(EGameEvent.FruitConsume, OnFruitConsume);

        private void Update()
        {
            if (m_FruitChanged)
            {
                m_FruitChanged = false;
                m_FruitText.text = $"Fruits: {Interlocked.Increment(ref m_FruitCount)}";
            }
        }

        private void OnFruitConsume(object sender, EventArgs _) => m_FruitChanged = true;
    }
}