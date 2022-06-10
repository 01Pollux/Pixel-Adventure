using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "SO/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public float Speed = 400f;

        public float JumpForce = 15f;
    }
}
