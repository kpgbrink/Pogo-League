using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts._Game.Player.Damage.PlayerDie
{
    public class PlayerDieShrink : MonoBehaviour, IOnPlayerDie
    {
        [SerializeField]
        [Range(.1f, .999f)]
        float transformMult;

        [SerializeField]
        [Range(.01f, .999f)]
        float transformEndMultDestroy;

        [SerializeField]
        Transform shrinkTransform;

        bool Shrinking { get; set; } = false;

        Vector3 OriginalLocalScale { get; set; }

        // TODO make it make the children smaller as an option
        Transform ShrinkingTransform
        {
            get
            {
                if (shrinkTransform != null)
                {
                    return shrinkTransform;
                }
                return transform;
            }
        }


        public void Start()
        {
            OriginalLocalScale = ShrinkingTransform.localScale;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Shrinking)
            {
                ShrinkingTransform.localScale *= transformMult;
                if (ShrinkingTransform.localScale.magnitude < OriginalLocalScale.magnitude * transformEndMultDestroy)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void OnPlayerDie(
            float? damagedAmount = null,
            Collision collision = null,
            DamageGiver damageGiver = null,
            DamageTaker damageTaker = null)
        {
            Shrinking = true;
        }
    }
}
