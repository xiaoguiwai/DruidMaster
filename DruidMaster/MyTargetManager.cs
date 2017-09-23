using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WowNet.Client;
using WowNet.Client.AI;
using WowNet.Client.Enums;
using WowNet.Client.Enums.Unit;
using WowNet.Client.Objects;
using WowNet.Common.Extensions;
using WowNet.Common.Tools;
using WowNet.Common.AI;
using static WowNet.Client.Structs.Spells.Monk;

namespace DruidMaster
{
    class MyTargetManager
    {
        //各类设置  
        //丰饶预铺层数
        public int FengRao = 4;
        public int health_lifebloomCheck = 100;
        public int Regrowth_healthcheck
        {
            get
            {
                if (Me.HasAura(16870))
                {
                    return 90;
                }
                else
                {
                    return 70;
                }
            }
        }
        public int Lifebloom_Healthcheck
        {
            get
            {
                return 85;
            }
        }
        public int Wildgrowth_HeadCount
        {
            get
            {
                return 3;
            }
             
        }
        public int HealingTouch_HeallthCheck = 70;
        public int SwiftmendHealtheCheck
        {
            get
            {
                return 40;
            }
        }
        public int BarkskinHealthCheck
        {
            get
            {
                return 50;
            }
        }
        public int IronbarkHealthCheck
        {
            get
            {
                return 40;
            }
        }
        public int EssenceofGHanirArtifact_Healthcheck_multi
        {
            get
            {
                return 50;
            }
        }
        public int EssenceofGHanirArtifact_Healthcheck_single
        {
            get
            {
                return 40;
            }
        }
        public int FriendsinDanger = 3;

        //各类单位集合
        public static List<WowPlayer> players;
        public static List<WowPlayer> friends;
        public static List<WowPlayer> friendsAndme;
        public static List<WowPlayer> enemies;

        //各类单位目标
        public static WowLocalPlayer Me;
        public WowPlayer lowestHealthFriend;
        public WowPlayer Target;
        public WowUnit MycurrentTarget;
        public int lowhealthfriendCOunt;
        public int friednsinDangerCount;

        public List<IAbility> Empty = new List<IAbility>();
        public List<IAbility> BattlePreparing = new List<IAbility>();
        public List<IAbility> GeneralSpellList = new List<IAbility>();
        public List<IAbility> HearTheLowestTarget = new List<IAbility>();
        public List<IAbility> InstantSpellList = new List<IAbility>();

        
        public bool Rejuvenation_use_check
        {
            get
            {

                if (lowestHealthFriend.HealthPercentage >=95 && RejuventurationCount < 4)
                {
                    if (Need_Rejuvenation(Me))
                    {
                        Target = Me;
                        return true;
                    }
                    if (MycurrentTarget != null && Need_Rejuvenation(MycurrentTarget) && MycurrentTarget.IsAPlayer && MycurrentTarget.IsFriendly)
                    {
                        Target = MycurrentTarget as WowPlayer;
                        return true;
                    }

                    Target = friendsAndme.FirstOrDefault(x => Need_Rejuvenation(x));
                    return true;
                }

                if (lowestHealthFriend.HealthPercentage < Regrowth_healthcheck && Need_Rejuvenation(lowestHealthFriend))
                {
                    Target = lowestHealthFriend;
                    return true;
                }


                if (lowestHealthFriend.HealthPercentage >= Regrowth_healthcheck)
                {
                    Target = friendsAndme.FirstOrDefault(x => x.HealthPercentage < 95 && Need_Rejuvenation(x));
                if (Target != null)
                    {
                        return true;
                    }
                }

                Target = Me;
                return false;
                //if (Target.HealthPercentage > 80)
                //{
                //    return !Target.HasAura(774);

                //}
                //else
                //{
                //    return !Target.HasAura(774) || !Target.HasAura(155777);
                //}


            }
            
        }
        public bool Healingtouch_use_check
        {
            get
            {
                Target = Me;
                return false;

            }

        }
        public bool Lifebloom_use_check
        {
            get
            {
                if (lowestHealthFriend.HealthPercentage < Lifebloom_Healthcheck && Can_Lifbloom_Cast())
                {
                    Target = lowestHealthFriend;
                    return true;
                }

                Target = Me;
                return false;
            }

        }
        public bool Regrowth_use_check
        {
            get
            {
                if(lowestHealthFriend.HealthPercentage<Regrowth_healthcheck)
                {
                    Target = lowestHealthFriend;
                    return true;
                }
                Target = Me;
                return false;

            }

        }
        public bool WildGrouth_use_check
        {
            get
            {
                if (lowhealthfriendCOunt >= Wildgrowth_HeadCount)
                {
                    Target = lowestHealthFriend;
                    return true;
                }
                Target = Me;
                return false;
            }

        }
        public bool SwiftMent_use_check
        {
            get
            {
                if (lowestHealthFriend.HealthPercentage < SwiftmendHealtheCheck)
                {
                    Target = lowestHealthFriend;
                    return true;
                }
                Target = Me;
                return false;

            }

        }
        public bool BarkSkin_use_check
        {
            get
            {
                if (Me.HealthPercentage < BarkskinHealthCheck)
                {
                    Target = Me;
                    return true;
                }
                Target = Me;
                return false;

            }

        }
        public bool IronBark_use_check
        {
            get
            {
                if (lowestHealthFriend.HealthPercentage < IronbarkHealthCheck)
                {
                    Target = lowestHealthFriend;
                    return true;
                }

                Target = Me;
                return false;

            }

        }
        public bool EssenceofGHanirArtifact_use_check
        {
            get
            {
                if (lowestHealthFriend.HealthPercentage < EssenceofGHanirArtifact_Healthcheck_single)
                {
                    Target = Me;
                    return true;
                }
                if (friednsinDangerCount > FriendsinDanger)
                {
                    Target = Me;
                    return true;
                }
                Target = Me;
                return false;

            }

        }
        public bool Innervate_use_check
        {
            get
            {
                if (lowhealthfriendCOunt >= 4 || friednsinDangerCount >= 3)
                {
                    Target = Me;
                    return true;
                }
                Target = Me;
                return false;

            }

        }

        public int RejuventurationCount
        {
            get
            {
                return Me.GetAura(207640).StackCount;

            }

        }
        //目标排序方法（按照血量）
        public static int CompareByHeathPercentage(WowPlayer player1, WowPlayer player2)
        {
           
            if (player1.HealthPercentage == player2.HealthPercentage)
            {
                return 0;
            }
            else
            {
                if (player1.HealthPercentage > player2.HealthPercentage)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }


        }


        public MyTargetManager()
        {

        }

        //public void PVE_Checks_Update()
        //{
        //    Lifebloom_use_check = Can_Lifbloom_Cast();
        //    Rejuvenation_use_check = true;
        //    Healingtouch_use_check = true;
        //    Regrowth_use_check = UpdateRegrowthhealth();
        //    WildGrouth_use_check = UpdateWildGrowthCheck();
        //    BarkSkin_use_check = true;
        //    IronBark_use_check = true;
        //    Innervate_use_check = UpdateInnervateCheck();
        //}

        //获得用于战斗逻辑的各项战斗条件
        public void UpdateEnvironment()
        {
            
            Target = null;
            MycurrentTarget = null;

            //各类单位集合更新数据 
            Me = Game.Instance.Player;
            Me.Update();
            players = Game.Instance.Manager.Objects.OfType<WowPlayer>().Where(x => x.Distance < 40 && x.InLoS&&!x.IsACorpse).ToList();
            foreach (var player in players)
            {
                player.Update();
            }
            friends = players.Where(x => x.IsFriendly).ToList();
            friendsAndme = friends;
            friendsAndme.Add(Me);
            friendsAndme.Sort(CompareByHeathPercentage);
            //血量最低队友
            lowestHealthFriend = friendsAndme.First();
            friednsinDangerCount = friendsAndme.Where(x => x.HealthPercentage < EssenceofGHanirArtifact_Healthcheck_multi).Count();
            //更新血量低于70%的队友数量
            lowhealthfriendCOunt = friendsAndme.Where(x => x.HealthPercentage < 70).Count();
            enemies = players.Where(x => x.IsHostile).ToList();
            MycurrentTarget = Me.GetTargetUnit();
            MycurrentTarget.Update();//必须刷新，之前没刷新过
            


            //更新生命绽放使用条件
            //PVE_Checks_Update();

        }


        //对是否使用生命绽放进行检测


        public bool Need_Rejuvenation(WowUnit target)
        {
            if (target.HealthPercentage > 80)
            {
                return !(target.HasAura(774)||target.HasAura(155777));

            }
            else
            {
                return !target.HasAura(774) || !target.HasAura(155777);
            }
        }
        public bool UpdateWildGrowthCheck()
        {
            if (lowhealthfriendCOunt >= 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool UpdateSwiftMend(WowUnit target)
        {
            if (target.HealthPercentage < SwiftmendHealtheCheck)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Can_Lifbloom_Cast()
        {
            foreach (var player in friendsAndme)
            {
                player.Update();
                if (player.HasAura(33763) && player.HealthPercentage < health_lifebloomCheck)
                {
                    return false;

                }

            }
            return true;
        }
        //public bool UpdateRegrowthhealth()
        //{
        //    if (Me.HasAura(16870))
        //    {
        //        Regrowth_healthcheck = 90;
        //        return true;
        //    }
        //    else
        //    {
        //        Regrowth_healthcheck = 70;
        //        return true;
        //    }
        //}
        public bool UpdateBarkskinCheck(WowUnit target)
        {
            if (target == Me && target.HealthPercentage < BarkskinHealthCheck)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool UpdateIronBarkCheck(WowUnit target)
        {
            if (target.HealthPercentage < IronbarkHealthCheck)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool UpdateEssenceofGHanirArtifactCheck(WowUnit target)
        {
            if (target.HealthPercentage < EssenceofGHanirArtifact_Healthcheck_single)
            {
                target = Me;
                return true;
            }
            if (friednsinDangerCount > FriendsinDanger)
            {
                target = Me;
                return true;
            }
            target = Me;
            return false;
        }
        public bool UpdateInnervateCheck()
        {
            if (lowhealthfriendCOunt >= 4 || friednsinDangerCount >= 3)
            {
                Target = Me;
                return true;
            }
            return false;
        }

        public List<IAbility> Solution_General()
        {
            

            return GeneralSpellList;
            
        }


        public List<IAbility> Solution_InstSpell()
        {
            return InstantSpellList;
        }
        //public List<IAbility> Solution_BattlePreparing()
        //{




        //    if (RejuventurationCount < FengRao)
        //    {
        //        if (Need_Rejuvenation(Me))
        //        {
        //            Target = Me;
        //        }
        //        else if (MycurrentTarget != null && Need_Rejuvenation(MycurrentTarget) && MycurrentTarget.IsAPlayer && MycurrentTarget.IsFriendly)
        //        {
        //            Target = MycurrentTarget as WowPlayer;
        //        }
        //        else
        //        {
        //            Target = friendsAndme.FirstOrDefault(x => Need_Rejuvenation(x) == true);
        //        }
        //    }
        //    return BattlePreparing;
        //}


        //public List<IAbility> Solution_BattlePreparing_withpeoplehirt()
        //{



        //    Target = friendsAndme.FirstOrDefault(x => x.HealthPercentage < 95 && (Need_Rejuvenation(x) == true || Lifebloom_use_check == true || x.HealthPercentage < Regrowth_healthcheck));





        //    if (Target != null)
        //    {
        //        if (Target.HealthPercentage < Regrowth_healthcheck)
        //        {
        //            Regrowth_use_check = true;
        //        }
        //        else { Regrowth_use_check = false; }

        //        if (Target.HealthPercentage < HealingTouch_HeallthCheck && FengRao >= 7)
        //        {
        //            Healingtouch_use_check = true;
        //        }
        //        else { Healingtouch_use_check = false; }

        //        return GeneralSpellList;
        //    }
        //    else
        //    {
        //        return Solution_BattlePreparing();
        //    }
        //}

        //public List<IAbility> Protection_and_Control()
        //{
        //    if (Me.HealthPercentage < BarkskinHealthCheck && UpdateBarkskinCheck(Me))
        //    {
        //        Target = Me;
        //        return InstantSpellList;
        //    }
        //    if (UpdateIronBarkCheck(lowestHealthFriend))
        //    {
        //        Target = lowestHealthFriend;
        //        return InstantSpellList;

        //    }
        //    return Empty;



        //}




        //public List<IAbility> Solution_Level1_allmorethan90()
        //{
        //    if (Lifebloom_use_check == true)
        //    {
        //        Target = lowestHealthFriend;
        //        return GeneralSpellList;
        //    } else 
        //    {
        //        Target = friendsAndme.FirstOrDefault(x => (!x.HasAura(774) || !x.HasAura(155777))&&x.HealthPercentage<90);
        //        if (Target != null)
        //        {
        //            Rejuvenation_use_check = true;
        //            return GeneralSpellList;
        //        }
        //        return Solution_BattlePreparing();
        //    }


        //}
        public WowUnit SwiftmendTarget()
        {
            var excecute = SwiftMent_use_check;

            return Target;
        }
        public WowUnit WildgrowthTarget()
        {
            var excecute = WildGrouth_use_check;

            return Target;
        }
        public WowUnit RegrowthTarget()
        {
            var excecute = Regrowth_use_check;

            return Target;
        }

        public WowUnit HealingtouchTarget()
        {
            var excecute = Healingtouch_use_check;

            return Target;
        }

        public WowUnit LifebloomTarget()
        {
            var excecute = Lifebloom_use_check;

            return Target;
        }

        public WowUnit RejuvenationTarget()
        {
            var excecute = Rejuvenation_use_check;

            return Target;
        }

        public WowUnit BarkskinTarget()
        {
            var excecute = BarkSkin_use_check;

            return Target;
        }

        public WowUnit IronbarkTarget()
        {
            var excecute = IronBark_use_check;

            return Target;
        }

        public WowUnit InnervateTarget()
        {
            var excecute = Innervate_use_check;

            return Target;
        }

        public WowUnit EssenceofGHanirArtifactTarget()
        {
            var excecute = EssenceofGHanirArtifact_use_check;

            return Target;
        }
        

        public WowUnit Gettarget()
        {
           
            
            return Target;
        }
    }
}
