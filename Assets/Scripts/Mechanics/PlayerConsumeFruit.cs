using Interfaces;
using UnityEngine;

namespace Mechanics
{
    public class PlayerConsumeFruit :
        MonoBehaviour,
        IFruit
    {
        public void OnTouch(PlayerController player_controller) => Destroy(gameObject);
    }
}