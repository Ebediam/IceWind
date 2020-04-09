using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace IceWind
{
    public enum TargetType
    {
        Avatar,
        NPC,
        HeldItem,
        Item,
        Environment
    }

    public class FrozenItem : MonoBehaviour
    {
        public Item item;
        public float iceDamage = 0.1f;

        public List<ParticleSystem> frozenVFXs;

        public bool active;

        public float imbuingDuration;

        public bool itemJustFrozen;

        public bool removeIceImbuingOnUngrab = true;



        public ParticleSystem freezeVFX = null;

        float timer;
        float justFrozenTimer = 1f;

        public void Start()
        {
            item = this.GetComponent<Item>();
            item.OnCollisionEvent += OnFrozenCollision;
            item.OnUngrabEvent += OnUngrab;
            item.OnGrabEvent += OnGrab;
        }

        public void Update()
        {
            if (itemJustFrozen)
            {
                timer += Time.deltaTime;

                if(timer > justFrozenTimer)
                {
                    timer = 0f;
                    itemJustFrozen = false;
                }
            }


        }

        public void OnUngrab(Handle handle, Interactor interaction, bool throwing)
        {
            if (removeIceImbuingOnUngrab)
            {
                DeactivateTimer();   
            }
        }


        public void DeactivateTimer()
        {
            Invoke("Deactivate", IceWindLevelModule.iceImbuingDuration);
        }

        public void OnGrab(Handle handle, Interactor interactor)
        {
            if (removeIceImbuingOnUngrab)
            {
                CancelInvoke("Deactivate");
            }

        }



        public void Deactivate()
        {
            active = false;
            itemJustFrozen = true;
            CancelInvoke("Deactivate");
            if (frozenVFXs.Count == 0)
            {
                return;
            }

            foreach (ParticleSystem ps in frozenVFXs)
            {
                ps.Stop();
            }
        }
        public void ResetTimer()
        {
            CancelInvoke("Deactivate");
            DeactivateTimer();
        }

        public void Activate()
        {
            active = true;
            itemJustFrozen = true;
            ResetTimer();
            if(frozenVFXs.Count == 0)
            {
                return;
            }

            foreach(ParticleSystem ps in frozenVFXs)
            {
                ps.Play();
            }
        }

        public void OnFrozenCollision(ref CollisionStruct collisionStruct)
        {
            if (!active)
            {
                return;
            }

            switch (GetTargetType(collisionStruct))
            {
                case TargetType.Avatar:
                    return;

                case TargetType.Environment:
                    return;

                case TargetType.NPC:
                    IceWindController.TryFreezeCreature(collisionStruct.targetCollider.GetComponentInParent<Creature>(), false, 0.1f, freezeVFX);
                    break;

                    
            }

        }


        public TargetType GetTargetType(CollisionStruct collisionInstance)
        {
            TargetType targetType;

            if (collisionInstance.targetCollider.transform.GetComponentInParent<Creature>())
            {

                //Debug.Log("You hit a creature with name "+collisionInstance.targetCollider.name+", and the creature is "+ collisionInstance.targetCollider.transform.GetComponentInParent<Creature>().state);
                targetType = TargetType.NPC;
                return targetType;
            }
            else if (collisionInstance.otherItem)
            {
                if (collisionInstance.targetCollider.GetComponentInParent<Player>())
                {
                    //Debug.Log("You hit yourself");
                    targetType = TargetType.Avatar;
                    return targetType;
                }
                else
                {

                    //Debug.Log("You hit an item with name " + collisionInstance.otherItem.name);
                    if (collisionInstance.otherItem.handlers.Count > 0)
                    {
                        //Debug.Log("...and it's being handled");
                        targetType = TargetType.HeldItem;
                        return targetType;
                    }
                    else
                    {
                        //Debug.Log("...and it's not being handled");
                        targetType = TargetType.Item;
                        return targetType;
                    }
                }

            }
            else
            {
                //Debug.Log("You hit the environment with collider name "+collisionInstance.targetCollider.name);
                targetType = TargetType.Environment;
                return targetType;
            }

        }


    }
}
