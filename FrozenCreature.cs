using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BS;

namespace IceWind
{

    public class FrozenCreature
    {
        public static Color freezeColor = new Color(0.6933f, 0.9663f, 1f);
        public static float freezeSpeed = 0.5f;
        public static float UnfreezeSpeed = 0.05f;
        public List<ParticleSystem> frozenVFXs;


        public Color baseColor;
        public Creature creature;
        public Animator animator;
        public Material material;
        public float defaultAnimatorSpeed;
        public float defaultLocomotionSpeed;
        public float freezePercent;
        public bool isBeingFrozen;
        public bool activeVFX;

        public void UpdateFrozenStatus()
        {
            if (!creature)
            {
                //IceWindController.frozenCreatures.Remove(this);
                //IceWindController.frozenCreatures.Sort();
                return;
            }


            if (isBeingFrozen)
            {
                freezePercent += freezeSpeed * Time.deltaTime;
                if (freezePercent > 1)
                {
                    return;
                }
            }
            else
            {
                freezePercent -= UnfreezeSpeed * Time.deltaTime;
                if (freezePercent < 0f)
                {
                    freezePercent = 0f;
                }
            }

            if (activeVFX && (freezePercent <= 0f))
            {
                DeactivateParticles();
                return;
            }
            else if(!activeVFX && (freezePercent > 0f))
            {
                ActivateParticles();
            }


            Color currentColor = Color.Lerp(baseColor, freezeColor, freezePercent);
            material.SetColor("_BaseColor", currentColor);

            float currentAnimatorSpeed = Mathf.Lerp(defaultAnimatorSpeed, 0, freezePercent);
            animator.speed = currentAnimatorSpeed;

            float currentLocomotionSpeed = Mathf.Lerp(defaultLocomotionSpeed, 0, freezePercent);
            creature.locomotion.speed = currentLocomotionSpeed;

        }

        public void DeactivateParticles()
        {
            
            if(frozenVFXs.Count != 0)
            {
                foreach (ParticleSystem ps in frozenVFXs)
                {
                    ps.Stop();
                }
            }


            activeVFX = false;
        }

        public void ActivateParticles()
        {
            if(frozenVFXs.Count != 0)
            {
                foreach (ParticleSystem ps in frozenVFXs)
                {
                    ps.Play();
                }
            }

            activeVFX = true;
        }


        public void AddParticlesToCreature(ParticleSystem ps)
        {

            frozenVFXs = new List<ParticleSystem>();

            foreach(Collider col in creature.gameObject.GetComponentsInChildren<Collider>())
            {
                ParticleSystem _ps = IceWindController.AddParticlesToCollider(col, ps);

                if (_ps)
                {
                    frozenVFXs.Add(_ps);
                }
            }
        }

        
    }
}
