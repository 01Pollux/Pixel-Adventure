using UnityEngine;

namespace Interfaces
{
    public interface IHealth
    {
        public int Health { get; set; }

        public int MaxHealth { get; }

        public float CurrentDamageCooldown { get; set; }

        public float DamageCooldown { get; }

        void OnTakeDamage(IDamageInfo damage_info);

        void OnTakeDamageFatal(IDamageInfo damage_info);
    }

    public static class IHealthHelper
    {
        public static bool TakeDamage(this IHealth health, IDamageInfo damage_info)
        {
            if (health.CurrentDamageCooldown > Time.time)
                return false;

            health.CurrentDamageCooldown = Time.time + health.DamageCooldown;

            bool was_dead = health.Health <= 0;
            if (!was_dead)
            {
                health.Health -= damage_info.Damage;
                if (health.Health <= 0)
                    health.OnTakeDamageFatal(damage_info);
                else
                    health.OnTakeDamage(damage_info);

                return true;
            }
            return false;
        }
    }
}