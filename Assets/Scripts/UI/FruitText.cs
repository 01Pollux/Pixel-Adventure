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

        private object m_TextLock;

        private void Awake()
        {
            m_FruitText = GetComponent<TMPro.TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            GameMessenger.AddListener(EGameEvent.FruitConsume, OnFruitConsume);
            GameMessenger.AddListener(EGameEvent.PlayerSpawn, OnPlayerSpawn);
        }

        private void OnDisable()
        {
            GameMessenger.RemoveListener(EGameEvent.FruitConsume, OnFruitConsume);
            GameMessenger.RemoveListener(EGameEvent.PlayerSpawn, OnPlayerSpawn);
        }

        private void Update()
        {
            if (m_FruitChanged)
            {
                m_FruitChanged = false;
                m_FruitText.text = $"Fruits: {Interlocked.Increment(ref m_FruitCount)}";
            }
        }

        private void OnFruitConsume(object sender, EventArgs args) => m_FruitChanged = true;

        private void OnPlayerSpawn(object sernder, EventArgs args)
        {
            m_FruitChanged = false;
            m_FruitCount = 0;
            m_FruitText.text = "Fruits: 0";
        }
    }
}