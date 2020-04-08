using UnityEngine;
using BS;

namespace IceWind
{
    public class IceWindLevelModule : LevelModule
    {
        public Body playerBody;
        public IceWindController leftIceWind;
        public IceWindController rightIceWind;
        public Spell iceWindSpellR;
        public Spell iceWindSpellL;
        


        public override void OnLevelLoaded(LevelDefinition levelDefinition)
        {
            initialized = true; // Set it to true when your script are loaded
        }

        public override void Update(LevelDefinition levelDefinition)
        {
            if (!Player.local)
            {
                return;
            }
            else
            {
                if (!Player.local.body)
                {
                    leftIceWind = null;
                    rightIceWind = null;
                    return;
                }
                else
                {
                    if (!Player.local.body.handLeft)
                    {
                        return;
                    }
                    else 
                    {
                        GetIceWindSpell(Player.local.body.handLeft);
                        GetIceWindSpell(Player.local.body.handRight);
                    }

                    
                }
     
            }
        }

        public void DebugInput()
        {
            if (PlayerControl.handLeft.alternateUseAxis > 0)
            {
                Debug.Log("AlternateUseAxis > 0");
            }

            if (PlayerControl.handLeft.alternateUsePressed)
            {
                Debug.Log("AlternateUsePressed");
            }

            if (PlayerControl.handLeft.castPressed)
            {
                Debug.Log("Cast pressed");
            }

            if (PlayerControl.handLeft.indexCurl > 0)
            {
                Debug.Log("IndexCurl > 0");
            }

            if (PlayerControl.handLeft.usePressed)
            {
                Debug.Log("Use pressed");
            }
        }

        public void GetIceWindSpell(BodyHand hand)
        {
            if (!hand.caster)
            {
                return;
            }

            if (!hand.caster.currentSpell)
            {
                return;
            }

            

            switch (hand.side)
            {
                case Side.Left:
                    if (leftIceWind)
                    {
                        if(hand.caster.currentSpell != iceWindSpellL)
                        {
                            leftIceWind.active = false;
                        }   
                        return;
                    }
                    else
                    {
                        if (hand.caster.currentSpell.name == "IceSpell(Clone)")
                        {
                            iceWindSpellL = hand.caster.currentSpell;
                            leftIceWind = hand.caster.currentSpell.gameObject.AddComponent<IceWindController>();
                            leftIceWind.hand = hand;
                            leftIceWind.active = true;
                        }

                    }
                    return;

                case Side.Right:
                    if (rightIceWind)
                    {
                        if(hand.caster.currentSpell != iceWindSpellR)
                        {
                            rightIceWind.active = false;
                        }
                        return;
                    }
                    else
                    {
                        if(hand.caster.currentSpell.name == "IceSpell(Clone)")
                        {
                            iceWindSpellR = hand.caster.currentSpell;
                            rightIceWind = hand.caster.currentSpell.gameObject.AddComponent<IceWindController>();
                            rightIceWind.hand = hand;
                            rightIceWind.active = true;
                        }

                    }
                    return;

                default:
                    return;


            }







        }

    }

}

