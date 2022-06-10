using Interfaces;
using UnityEngine;

namespace Mechanics
{
    public class AppleFruit :
        MonoBehaviour,
        IFruit
    {
        public void OnTouch(LocalPlayerController player_controller) => Destroy(gameObject);
    }
}