using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BS;
using UnityEngine.AI;

namespace IceWind
{ 
    public class FrozenCreature
    {
        public static Color freezeColor = new Color(0.6933f, 0.9663f, 1f);
        public static float freezeSpeed = 0.5f;
        public static float UnfreezeSpeed = 0.05f;

        public Color baseColor;
        public Creature creature;
        public Animator animator;
        public Material material;
        public float defaultAnimatorSpeed;
        public float defaultLocomotionSpeed;
        public float freezePercent;
        public bool isBeingFrozen;
        


        public void UpdateFrozenStatus()
        {
            if (!creature)
            {
                IceWindController.frozenCreatures.Remove(this);
                IceWindController.frozenCreatures.Sort();
                return;
            }


            Debug.Log("UpdateFrozenStatus");
            if (isBeingFrozen)
            {
                freezePercent += freezeSpeed * Time.deltaTime;
                if(freezePercent > 1)
                {
                    return;
                }
            }
            else
            {
                freezePercent -= UnfreezeSpeed * Time.deltaTime;
                if(freezePercent < 0f)
                {
                    freezePercent = 0f;
                    return;
                }
            }

            Color currentColor = Color.Lerp(baseColor, freezeColor, freezePercent);
            material.SetColor("_BaseColor", currentColor);

            float currentAnimatorSpeed = Mathf.Lerp(defaultAnimatorSpeed, 0, freezePercent);
            animator.speed = currentAnimatorSpeed;

            float currentLocomotionSpeed = Mathf.Lerp(defaultLocomotionSpeed, 0, freezePercent);
            creature.locomotion.speed = currentLocomotionSpeed;

            Debug.Log("The creature is " + (freezePercent * 100f) + "% frozen");

        }


    }


    public class IceWindController:MonoBehaviour
    {
        public static List<FrozenCreature> frozenCreatures = new List<FrozenCreature>();
        public ParticleSystem iceWindVFX;
        public Side side;
        public TriggerDetector trigger;
        public AudioSource iceWindSFX;

        public void Start()
        {
            Debug.Log("-----------ICE WIND CONTROLLER STARTS---------");

            trigger = transform.Find("AttackCone").gameObject.AddComponent<TriggerDetector>();
            trigger.controller = this;

            if(side == Side.Left)
            {
                transform.Find("AttackCone").Rotate(transform.Find("AttackCone").up, 180f);
            }


            iceWindVFX = transform.Find("AttackCone").Find("IceWindVFX").GetComponent<ParticleSystem>();
            iceWindSFX = transform.Find("IcyWindSFX").GetComponent<AudioSource>();
                        

        }

        public void Update()
        {
            switch (side)
            {
                case Side.Left:
                    if (PlayerControl.handLeft.castPressed)
                    {
                        CastIceWind();
                    }
                    else
                    {
                        StopIceWind();
                    }
                    break;

                case Side.Right:
                    if (PlayerControl.handRight.castPressed)
                    {
                        CastIceWind();
                    }
                    else
                    {
                        StopIceWind();
                    }
                    break;

            }
            
            if(frozenCreatures.Count == 0)
            {
                return;
            }

            foreach(FrozenCreature frozenCreature in frozenCreatures)
            {
                frozenCreature.UpdateFrozenStatus();
            }


        }

        public void CastIceWind()
        {
            if (!iceWindVFX.isPlaying)
            {
                iceWindVFX.Play();
                trigger.collider.enabled = true;
                iceWindSFX.Play();
            }
        }

        public void StopIceWind()
        {
            if (iceWindVFX.isPlaying)
            {
                iceWindVFX.Stop();
                iceWindSFX.Stop();
                //trigger.collider.enabled = false;                
            }
        }

        public void CollisionDetected(Collider other)
        {
            if (!iceWindVFX.isPlaying)
            {
                return;
            }

            Creature creature = other.GetComponentInParent<Creature>();

            if (!creature)
            {
                return;
            }

            if(creature == Creature.player)
            {
                return;
            }


            
            if(frozenCreatures.Count > 0)
            {
                foreach(FrozenCreature _frozenCreature in frozenCreatures)
                {
                    if(_frozenCreature.creature == creature)
                    {
                        _frozenCreature.isBeingFrozen = true;
                        return;
                    }
                }
            }

            FrozenCreature frozenCreature = new FrozenCreature();
            frozenCreature.creature = creature;            
            frozenCreature.animator = creature.gameObject.GetComponentInChildren<Animator>();
            frozenCreature.material = creature.bodyMeshRenderer.material;
            frozenCreature.baseColor = frozenCreature.material.GetColor("_BaseColor");
            frozenCreature.defaultAnimatorSpeed = frozenCreature.animator.speed;
            frozenCreature.defaultLocomotionSpeed = creature.locomotion.speed;
            frozenCreature.isBeingFrozen = true;
            frozenCreatures.Add(frozenCreature);
           

        }

        
        public void CollisionExit(Collider other)
        {
            if (frozenCreatures.Count == 0)
            {
                return;
            }

            Creature creature = other.GetComponentInParent<Creature>();

            if (!creature)
            {
                return;
            }

            if (creature == Creature.player)
            {
                return;
            }
            
            foreach (FrozenCreature _frozenCreature in frozenCreatures)
            {
                if (_frozenCreature.creature == creature)
                {
                    _frozenCreature.isBeingFrozen = false;
                    return;
                    
                }
            }           


        }

        public void ShowCreatureInfo(Creature creature)
        {
            Debug.Log("---------------Collision with enemy detected: " + creature.name+", starting log-------------");
            Debug.Log("---NAVIGATION---");
            Debug.Log("Acceleration: " + creature.navigation.acceleration);
            Debug.Log("CurrentRunSpeedRatio: "+creature.navigation.currentRunSpeedRatio);
            Debug.Log("RadgollMultiplier: " + creature.navigation.navRagdollMult);
            Debug.Log("NavState: " + creature.navigation.navState);
            Debug.Log("RunDistance: " + creature.navigation.runDistance);
            Debug.Log("RunSpeedRatio: " + creature.navigation.runSpeedRatio);
            Debug.Log("UseAcceleration: "+creature.navigation.useAcceleration);

            Debug.Log("ActionCycleSpeed: " + creature.actionCycleSpeed);

        }

    }
}
