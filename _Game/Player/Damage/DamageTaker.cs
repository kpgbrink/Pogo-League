using Assets.Scripts;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DamageTaker : PlayerObject
{
    public float maxHealth;
    [Serializable]
    public class CanGetDamage
    {
        public bool receivesSelfDamage = true;
        public CountDownTimer damageBetweenTimer;

    }
    public CanGetDamage canGetDamage;

    [Serializable]
    public class OnDamaged
    {
        public Transform parentDamageObject;
        public bool recordDamageDoneToMe = true;
        public UnityEvent damagedEvents;
        public bool playerDiesOnDeath;
    }
    public OnDamaged onDamaged;

    public float Health { get; set; }
    bool StopTakingDamage { get; set; } = false;

    void Start()
    {
        Health = maxHealth;
    }

    public int? SpawnNum
    {
        get
        {
            if (Player == null) return null;
            return Player.ResetValuesOnSceneInit.SpawnNum;
        }
    }

    void FixedUpdate()
    {
        canGetDamage.damageBetweenTimer.CountDown();
    }



    private void OnCollisionEnter(Collision collision)
    {
        var damageGiver = collision.collider.transform.GetComponent<DamageGiver>();

        if (damageGiver == null) return;
        var impulse = collision.impulse;

        // Check timer
        if (!canGetDamage.damageBetweenTimer.IsFinished() && !damageGiver.canDamage.overrideDamageTimer) return;
        canGetDamage.damageBetweenTimer.StartTimer();

        var otherPlayerSpar = damageGiver.PlayerSpar;

        // Check if on same team
        var damageDoneToTeamate = FuncUtil.Invoke(() =>
        {
            if (PlayerSpar == null || otherPlayerSpar == null) return false;
            if (PlayerSpar.Team == otherPlayerSpar.Team && PlayerSpar.Team != 0)
            {
                return true;
            }
            return false;
        });

        var damageDoneToMe = FuncUtil.Invoke(() =>
        {
            // If the same player do not do damage.
            var otherPlayer = damageGiver.Player;
            if (Player == null || otherPlayer == null) return false;
            if (Player.PlayerNumber == otherPlayer.PlayerNumber)
            {
                return true;
            }
            return false;
        });

        // Don't damage teamate
        if (damageDoneToTeamate) return;
        // Don't damage myself if not able to
        if (damageDoneToMe)
        {
            if (!canGetDamage.receivesSelfDamage || !damageGiver.canDamage.selfDamage) return;
        }

        var damageCalculated = damageGiver.calculateDamage.defaultDamage;
        // TODO actually calculate damage

        Debug.Log($"Doing {damageCalculated} to {gameObject.name}");

        TakeDamage(damageCalculated, damageGiver, collision, damageDoneToMe, damageDoneToTeamate);
    }

    public void TakeDamage(float damageAmount, DamageGiver damageGiver, Collision collision, bool damageDoneToMe, bool damageDoneToTeamate)
    {
        if (StopTakingDamage) return;
        var realNewHealth = Health - damageAmount;
        Health = Mathf.Max(0, realNewHealth);
        Debug.Log($"{gameObject.name}{realNewHealth}");

        // Give damage to parent object if health is lower than zero
        if (realNewHealth < 0)
        {
            if (onDamaged.parentDamageObject != null)
            {
                var parentDamageTaker = onDamaged.parentDamageObject.GetComponent<DamageTaker>();
                if (parentDamageTaker)
                {
                    // This might have to be fixed idk.
                    parentDamageTaker.TakeDamage(Math.Abs(realNewHealth), damageGiver, collision, damageDoneToMe, damageDoneToTeamate);
                    return;
                }
            }
        }

        if (Player != null)
        {
            // Add to damageQueue to change the way the damage works
            Player.AddDamageQueue(damageGiver.damageName, damageAmount);
        }


        // Run Events on damage happening
        onDamaged.damagedEvents.Invoke();

        // Run Everything that implements IDamageTakerDamaged on me
        GetComponents<IOnDamageTaken>().Cast<IOnDamageTaken>().ToList().ForEach(o =>
            o.DamageTaken(
                damageAmount,
                collision,
                damageGiver,
                this
                )
        );
        // Run Everything that implements IDamageGiven on the damage giver.
        damageGiver.transform.GetComponents<IOnDamageGiven>().Cast<IOnDamageGiven>().ToList().ForEach(o =>
           o.DamageGiven(
               damageAmount,
               collision,
               damageGiver,
               this
               )
        );

        damageGiver.damagedEvents.Invoke();

        // Also don't record damage if done to myself or a teamate
        if (Player != null && onDamaged.recordDamageDoneToMe && !damageDoneToMe && !damageDoneToTeamate)
        {
            // Record that damage giver gave damage to me
            damageGiver.Player.AddDamageData(damageGiver.Player.ResetValuesOnSceneInit.DamagesGiven, Player.PlayerNumber, damageAmount);
            // Record that I took damage from this damage giver
            Player.AddDamageData(Player.ResetValuesOnSceneInit.DamagesTaken, damageGiver.Player.PlayerNumber, damageAmount);
        }

        //Debug.Log($"{Health}/{maxHealth}, Top Impulse: {topImpulse}, Bottom Impulse: {minImpulse}");

        if (Health <= 0 && onDamaged.playerDiesOnDeath)
        {
            StopTakingDamage = true;
            Debug.Log("Player dies");
            damageGiver.Player.AddKillData(Player.PlayerNumber);
            Player.Die(
                spawnNum: SpawnNum,
                damagedAmount: damageAmount,
                collision,
                damageGiver,
                this);
        }
    }
}
