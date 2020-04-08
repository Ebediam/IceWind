using UnityEngine;
using BS;

namespace IceWind
{
    public class IceWindLevelModule : LevelModule
    {
        public Body playerBody;
        public IceWindController leftIceWind;
        public IceWindController rightIceWind;
        

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
                        return;
                    }
                    else
                    {
                        if (hand.caster.currentSpell.name == "IceSpell(Clone)")
                        {
                            leftIceWind = hand.caster.currentSpell.gameObject.AddComponent<IceWindController>();
                            leftIceWind.side = Side.Left;
                        }
                    }
                    return;

                case Side.Right:
                    if (rightIceWind)
                    {
                        return;
                    }
                    else
                    {
                        if(hand.caster.currentSpell.name == "IceSpell(Clone)")
                        {
                            rightIceWind = hand.caster.currentSpell.gameObject.AddComponent<IceWindController>();
                            rightIceWind.side = Side.Right;
                        }
                    }
                    return;

                default:
                    return;


            }







        }

    }

}

