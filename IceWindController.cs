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
    
    public enum FreezingType
    {
        Spell,
        Weapon
    }

    public class IceWindController:MonoBehaviour
    {
        public static List<FrozenCreature> frozenCreatures = new List<FrozenCreature>();
        public ParticleSystem iceWindVFX;
        public ParticleSystem frostVFX;
        public TriggerDetector trigger;
        public AudioSource iceWindSFX;
        public BodyHand hand;
        public bool active;




        public void Start()
        {

            trigger = transform.Find("AttackCone").gameObject.AddComponent<TriggerDetector>();
            trigger.controller = this;

            if(hand.side == Side.Left)
            {
                transform.Find("AttackCone").Rotate(transform.Find("AttackCone").up, 180f);
            }


            iceWindVFX = transform.Find("AttackCone").Find("IceWindVFX").GetComponent<ParticleSystem>();
            frostVFX = transform.Find("FrostVFX").GetComponent<ParticleSystem>();

            iceWindSFX = transform.Find("IcyWindSFX").GetComponent<AudioSource>();
                        

        }

        public void Update()
        {



            if (frozenCreatures.Count > 0)
            {
                foreach (FrozenCreature frozenCreature in frozenCreatures)
                {
                    frozenCreature.UpdateFrozenStatus();
                }
            }

            if (!active)
            {
                return;
            }

            
            switch (hand.side)
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
            

              
                           
          

        }

        public FrozenItem FreezeItem(Item item, ParticleSystem _ps)
        {

            FrozenItem frozenItem = item.gameObject.GetComponent<FrozenItem>();

            if (frozenItem != null)
            {

                if (frozenItem.active)
                {
                    if (!frozenItem.itemJustFrozen)
                    {
                        frozenItem.Deactivate();
                    }

                }
                else
                {
                    if (!frozenItem.itemJustFrozen)
                    {
                        frozenItem.Activate();
                    }


                }
                return frozenItem;
            }



            FrozenItem frozenIt = item.gameObject.AddComponent<FrozenItem>();
            frozenIt.frozenVFXs = new List<ParticleSystem>();


            if (item.definition.colliderGroups.Count == 0)
            {
                return frozenIt;
            }


            foreach (ColliderGroup colliderGroup in item.definition.colliderGroups)
            {                
                if (colliderGroup.imbueMagic)
                {

                    if(colliderGroup.colliders.Count == 0)
                    {
                        continue;
                    }

                    foreach (Collider col in colliderGroup.colliders)
                    {

                        ParticleSystem ps = AddParticlesToCollider(col, _ps);

                        if (ps != null)
                        {

                            if (frozenIt.freezeVFX == null)
                            {

                                frozenIt.freezeVFX = ps;

                            }
                            
                            frozenIt.frozenVFXs.Add(ps);
  
                        }
                        


                    }
                }
            }

            return frozenIt;
        }


        public static ParticleSystem AddParticlesToCollider(Collider col, ParticleSystem originalPS)
        {
            if (col.isTrigger)
            {
                return null;
            }

            ParticleSystem particles = Instantiate(originalPS.gameObject).GetComponent<ParticleSystem>();
            particles.Stop();


            ParticleSystem.ShapeModule shapeModule = particles.shape;
            particles.transform.parent = col.transform;
            particles.transform.position = col.bounds.center;
            particles.transform.rotation = col.transform.rotation;
            particles.transform.localScale = Vector3.one;

            ParticleSystem.EmissionModule emissionModule = particles.emission;

            //emissionModule.rateOverTimeMultiplier = col.bounds.size.x * col.bounds.size.y * col.bounds.size.z*particles.transform.localScale.x*particles.transform.localScale.y*particles.transform.localScale.z;
            emissionModule.rateOverTimeMultiplier *= col.bounds.size.magnitude;



            if (col is BoxCollider)
            {
                BoxCollider boxCol = col as BoxCollider;

                shapeModule.scale = MultipyVectors(boxCol.size, boxCol.transform.lossyScale);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphereCol = col as SphereCollider;
                shapeModule.shapeType = ParticleSystemShapeType.Sphere;
                shapeModule.radius = sphereCol.radius;
                shapeModule.radiusThickness = 0f;
            }
            else if (col is CapsuleCollider)
            {
                CapsuleCollider capCol = col as CapsuleCollider;

                float height = capCol.height;
                float radius = capCol.radius;
                if (capCol.direction == 0)
                {
                    shapeModule.scale = new Vector3(height * capCol.transform.lossyScale.x, radius * capCol.transform.lossyScale.y, radius * capCol.transform.lossyScale.z);
                }
                if (capCol.direction == 1)
                {
                    shapeModule.scale = new Vector3(radius * capCol.transform.lossyScale.x, height * capCol.transform.lossyScale.y, radius * capCol.transform.lossyScale.z);
                }
                if (capCol.direction == 2)
                {
                    shapeModule.scale = new Vector3(radius * capCol.transform.lossyScale.x, radius * capCol.transform.lossyScale.y, height * capCol.transform.lossyScale.z);
                }



            }
            else
            {
                shapeModule.scale = col.bounds.size;
            }

            particles.Play();
            return particles;
        }

        public void CastIceWind()
        {
            if (hand.interactor.grabbedHandle)
            {
                Item item = hand.interactor.grabbedHandle.item;

                if (item != null)
                {
                    FreezeItem(item, frostVFX);
                }

            }
            else 
            {
                if (!iceWindVFX.isPlaying)
                {
                    iceWindVFX.Play();
                    trigger.collider.enabled = true;
                    iceWindSFX.Play();
                }
            }


        }

        public void StopIceWind()
        {
            if (iceWindVFX.isPlaying)
            {
                iceWindVFX.Stop();
                iceWindSFX.Stop();
                trigger.collider.enabled = false;     
                if(frozenCreatures.Count != 0)
                {
                    foreach (FrozenCreature frozenCreature in frozenCreatures)
                    {
                        frozenCreature.isBeingFrozen = false;
                    }
                }


                
            }
        }

        public void CollisionDetected(Collider other)
        {
            if (!iceWindVFX.isPlaying)
            {
                return;
            }

            Creature creature = other.GetComponentInParent<Creature>();

            if (creature)
            {
                TryFreezeCreature(creature, true, 0f, frostVFX);
                return;
            }

            Item item = other.GetComponentInParent<Item>();

            if (item)
            {
                FrozenItem frozenItem = item.gameObject.GetComponent<FrozenItem>();

                if (frozenItem)
                {
                    if (frozenItem.active)
                    {
                        frozenItem.ResetTimer();
                    }
                    else
                    {
                        frozenItem.Activate();
                    }
                }
                else
                {
                    frozenItem = FreezeItem(item, frostVFX);
                    frozenItem.DeactivateTimer();
                }               

            }


            

           

        }

        public static void TryFreezeCreature(Creature creature, bool freezeOverTime, float freezePercentBonus, ParticleSystem ps)
        {

            if (creature == Creature.player)
            {
                return;
            }

            if (frozenCreatures.Count > 0)
            {
                foreach (FrozenCreature _frozenCreature in frozenCreatures)
                {
                    if (_frozenCreature.creature == creature)
                    {
                        if (freezeOverTime)
                        {
                            _frozenCreature.isBeingFrozen = true;
                        }

                        _frozenCreature.freezePercent += freezePercentBonus;

                        
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
            frozenCreatures.Add(frozenCreature);
            

            if (freezeOverTime)
            {
                frozenCreature.isBeingFrozen = true;
            }

            frozenCreature.freezePercent += freezePercentBonus;
            frozenCreature.AddParticlesToCreature(ps);

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

        public static Vector3 MultipyVectors(Vector3 a, Vector3 b)
        {
            Vector3 c = new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);

            return c;


        }

        public void KospyAddColliderParticle(Collider collider, ParticleSystem fx)
        {
            ParticleSystem.ShapeModule shape = fx.shape;

            if (collider is SphereCollider)
            {
                shape.shapeType = ParticleSystemShapeType.Sphere;
                fx.transform.localPosition = (collider as SphereCollider).center;
                shape.radius = (collider as SphereCollider).radius * collider.transform.lossyScale.magnitude;
            }
            else if (collider is CapsuleCollider)
            {
                shape.shapeType = ParticleSystemShapeType.Box;
                fx.transform.localPosition = (collider as CapsuleCollider).center;
                float height = (collider as CapsuleCollider).height;
                float radius = (collider as CapsuleCollider).radius;
                if ((collider as CapsuleCollider).direction == 0)
                {
                    shape.scale = new Vector3(height * collider.transform.lossyScale.x, radius * collider.transform.lossyScale.y, radius * collider.transform.lossyScale.z);
                }
                if ((collider as CapsuleCollider).direction == 1)
                {
                    shape.scale = new Vector3(radius * collider.transform.lossyScale.x, height * collider.transform.lossyScale.y, radius * collider.transform.lossyScale.z);
                }
                if ((collider as CapsuleCollider).direction == 2)
                {
                    shape.scale = new Vector3(radius * collider.transform.lossyScale.x, radius * collider.transform.lossyScale.y, height * collider.transform.lossyScale.z);
                }
            }
            else if (collider is BoxCollider)
            {
                shape.shapeType = ParticleSystemShapeType.Box;
                fx.transform.localPosition = (collider as BoxCollider).center;
                shape.scale = new Vector3((collider as BoxCollider).size.x * collider.transform.lossyScale.x, (collider as BoxCollider).size.y * collider.transform.lossyScale.y, (collider as BoxCollider).size.z * collider.transform.lossyScale.z);
            }
            else if (collider is MeshCollider)
            {
                shape.shapeType = ParticleSystemShapeType.Mesh;
                fx.transform.localPosition = Vector3.zero;
                shape.mesh = (collider as MeshCollider).sharedMesh;
                shape.scale = new Vector3(collider.transform.lossyScale.x, collider.transform.lossyScale.y, collider.transform.lossyScale.z);
            }
            fx.transform.localRotation = Quaternion.identity;
        }

    }
}
