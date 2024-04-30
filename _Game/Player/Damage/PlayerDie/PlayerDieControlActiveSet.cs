using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts._Game.Player.Damage.PlayerDie
{
    public class PlayerDieControlActiveSet : MonoBehaviour, IOnPlayerDie
    {
        [SerializeField]
        bool controlActiveSet = false;

        [SerializeField]
        PlayerInputBase playerInputBase;

        public void OnPlayerDie(float? damagedAmount = null, Collision collision = null, DamageGiver damageGiver = null, DamageTaker damageTaker = null)
        {
            playerInputBase.ControlActive = controlActiveSet;
        }
    }
}