using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts._Game.Player.Damage.PlayerDie
{
    public class PlayerDieSetInactive : MonoBehaviour, IOnPlayerDie
    {
        public List<GameObject> gameObjects;
        public List<Rigidbody> rbs;
        public List<Collider> cols;

        public void OnPlayerDie(float? damagedAmount = null, Collision collision = null, DamageGiver damageGiver = null, DamageTaker damageTaker = null)
        {
            gameObjects.ForEach(o => o.SetActive(false));
            rbs.ForEach(o => {
                o.isKinematic = true;
                o.constraints = RigidbodyConstraints.FreezeAll;
            });
            cols.ForEach(o => o.enabled = false);
        }
    }
}
