using System;
using UnityEngine;
using EventSystem;

namespace Core
{
    public class PlayerSpawn : MonoBehaviour
    {
        private void OnEnable() => GameMessenger.AddListener(EGameEvent.PlayerSpawn, OnPlayerSpawn);
        private void OnDisable() => GameMessenger.RemoveListener(EGameEvent.PlayerSpawn, OnPlayerSpawn);

        private void OnPlayerSpawn(object sender, EventArgs args) => GameManager.Instance.PlayerController.transform.position = transform.position;
    }
}
