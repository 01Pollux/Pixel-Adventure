using UnityEngine;

namespace Interfaces
{
    public interface IDamageInfo
    {
        GameObject parentObj { get; }

        int Damage { get; }

        float KnockbackMod { get; }
    }
}
