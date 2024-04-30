using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageGiver : PlayerObject
{
    public string damageName;

    [Serializable]
    public class CanDamage
    {
        public bool selfDamage = true;
        public bool overrideDamageTimer = false;
    }
    public CanDamage canDamage;

    [Serializable]
    public class CalculateDamage
    {
        /// <summary>
        /// Minimum amount of speed needed for damage to take effect
        /// </summary>
        public float minSpeed;
        // Linear calculation from minSpeed to max relative to min and max damage

        public float maxSpeedMaxDamage;
        /// <summary>
        /// For now I'm using the default damage
        /// </summary>
        public float defaultDamage;

        public float minDamage;

        public float maxDamage;
    }

    public CalculateDamage calculateDamage;
    
    public UnityEvent damagedEvents;

    public Guid UniqueId { get; private set; }

    public void Start()
    {
        UniqueId = Guid.NewGuid();
    }
}
