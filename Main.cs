using MelonLoader;
using HarmonyLib;
using System.Collections.Generic;
using TheForest.World;
using UnityEngine;
using System;
using TheForest.Items.World;
using TheForest.Utils;
using TheForest.Items.Inventory;
using MyTrueGear;
using System.Threading;
using System.Linq;
using TheForest.Player.Clothing;
using TheForest.Items;



namespace TheForest_TrueGear
{
    public static class BuildInfo
    {
        public const string Name = "TheForest_TrueGear"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "TrueGear Mod for The Forest"; // Description for the Mod.  (Set as null if none)
        public const string Author = "HuangLY"; // Author of the Mod.  (MUST BE SET)
        public const string Company = "TrueGear"; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    

    public class TheForest_TrueGear : MelonMod
    {
        private static int equipId = 0;

        private static bool isInWater = false;
        private static bool isShiver = false;
        private static bool checkThrowRelease = false;

        private static string[] tags = { "Untagged","Player","Grabber","Float"};

        private static TrueGearMod _TrueGear = null;

        private static bool canWave = true;
        private static bool canWeaponRecoil = true;

        private static bool isHeartBeat = false;
        private static bool isHunger = false;
        private static float playerHealth = 0;

        private static bool isEquip = false;



        public override void OnInitializeMelon() {
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(TheForest_TrueGear));

            _TrueGear = new TrueGearMod();

            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("HeartBeat");
            _TrueGear.Play("HeartBeat");


            MelonLogger.Msg("OnApplicationStart");
        }


        private static KeyValuePair<float, float> GetAngle(Transform transform, Vector3 hitPoint)
        {
            Vector3 hitPos = hitPoint - transform.position;
            float hitAngle = Mathf.Atan2(hitPos.x, hitPos.z) * Mathf.Rad2Deg;
            hitAngle = hitAngle % 360f;
            if (hitAngle < 0f)
            {
                hitAngle += 360f;
            }
            hitAngle += 100f;
            if (hitAngle > 360f)
            {
                hitAngle -= 360f;
            }

            float verticalDifference = hitPoint.y - transform.position.y;
            verticalDifference += 130f;

            return new KeyValuePair<float, float>(hitAngle, verticalDifference);
        }

        //  //////////////////////////////////////////////////////////
        
        /*
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "Update")]
        public static void PlayerStats_Update_PostPatch(PlayerStats __instance)
        {
            
        }
        */
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "HealthChange")]
        public static void PlayerStats_HealthChange_PostPatch(PlayerStats __instance)
        {
            playerHealth = __instance.HealthTarget;
            if (__instance.HealthTarget <= 25.0f && !isHeartBeat)
            {
                isHeartBeat = true;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StartHeartBeat");
                _TrueGear.StartHeartBeat();
            }
            else if (__instance.HealthTarget > 25.0f && isHeartBeat)
            {
                isHeartBeat = false;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StopHeartBeat");
                _TrueGear.StopHeartBeat();
            }
            /*
            if (__instance.HealthTarget <= 0)
            {
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("PlayerDeath");
                MelonLogger.Msg("StopHeartBeat");
                MelonLogger.Msg("StopDiving");
                MelonLogger.Msg("StopRaining");
                MelonLogger.Msg("StopShiver");               
                
            }
            */
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "KillPlayer")]
        public static void PlayerStats_KillPlayer_PostPatch(PlayerStats __instance)
        {
            _TrueGear.StopHeartBeat();
            _TrueGear.StopHunger();
            _TrueGear.StopRaining();
            _TrueGear.StopShiver();
            _TrueGear.Play("PlayerDeath");
            isInWater = false;
            isShiver = false;
            isHeartBeat = false;
            isRaining = false;
            isHunger = false;
        }


        //  //////////////////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "CheckStats")]
        public static void PlayerStats_CheckStats_PostPatch(PlayerStats __instance)
        {
            
            if (__instance.IsCold && !isShiver)
            {
                isShiver = true;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StartShiver");
                _TrueGear.StartShiver();
                return;
            }
            else if (!__instance.IsCold && isShiver)
            {
                isShiver = false;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StopShiver");
                _TrueGear.StopShiver();
            }            
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayColdSfx")]
        public static void PlayerSfx_PlayColdSfx_PostPatch()
        {
            if (!isShiver) {
                isShiver = true;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StartShiver");
                _TrueGear.StartShiver();
            }
            
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "StopColdSfx")]
        public static void PlayerSfx_StopColdSfx_PostPatch()
        {
            if (isShiver)
            {
                isShiver = false;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StopShiver");
                _TrueGear.StopShiver();
            }
            
        }

        //  //////////////////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "Drink")]
        public static void PlayerStats_Drink_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Drinking");
            _TrueGear.Play("Drinking");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "DrinkBooze")]
        public static void PlayerStats_DrinkBooze_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Drinking");
            _TrueGear.Play("Drinking");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "DrinkLake")]
        public static void PlayerStats_DrinkLake_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Drinking");
            _TrueGear.Play("Drinking");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayDrink")]
        public static void PlayerSfx_PlayDrink_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Drinking");
            _TrueGear.Play("Drinking");
        }

        //  //////////////////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayEat")]
        public static void PlayerSfx_PlayEat_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Eating");
            _TrueGear.Play("Eating");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayEatMeds")]
        public static void PlayerSfx_PlayEatMeds_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Eating");
            _TrueGear.Play("Eating");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayEatMeat")]
        public static void PlayerSfx_PlayEatMeat_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Eating");
            _TrueGear.Play("Eating");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayEatPoison")]
        public static void PlayerSfx_PlayEatPoison_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Eating");
            _TrueGear.Play("Eating");
        }

        //  //////////////////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayStaminaBreath")]
        public static void PlayerSfx_PlayStaminaBreath_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Breathing");
            _TrueGear.Play("Breathing");
        }

        //  //////////////////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayOpenInventory")]
        public static void PlayerSfx_PlayOpenInventory_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("OpenInventory");
            _TrueGear.Play("OpenInventory");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayCloseInventory")]
        public static void PlayerSfx_PlayCloseInventory_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("CloseInventory");
            _TrueGear.Play("CloseInventory");
        }

        //  //////////////////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(WaterViz), "Update")]
        public static void WaterViz_Update_PostPatch(WaterViz __instance)
        {
            if (__instance.InWater && !isInWater)
            {
                isInWater = true;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("InWater");
                _TrueGear.Play("InWater");
            }
            else if (!__instance.InWater && isInWater)
            {
                isInWater = false;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("OutWater");
                _TrueGear.Play("OutWater");
            }            
        }

        //  //////////////////////////////////////////////////////////
        private static bool isRaining = false;
        [HarmonyPostfix, HarmonyPatch(typeof(RainSfx), "OnEnable")]
        public static void RainSfx_OnEnable_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("StartRaining");
            _TrueGear.StartRaining();
            isRaining = true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RainSfx), "OnDisable")]
        public static void RainSfx_OnDisable_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("StopRaining");
            _TrueGear.StopRaining();
            isRaining = false;
        }

        //  //////////////////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(WeatherSystem), "Lightning")]
        public static void WeatherSystem_Lightning_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("Thunder");
            _TrueGear.Play("Thunder");
        }

        //  //////////////////////////////////////////////////////////
        private static bool canDamage = true;
        [HarmonyPrefix, HarmonyPatch(typeof(PlayerStats), "Hit")]
        public static bool PlayerStats_Hit_PrePatch(PlayerStats __instance, PlayerStats.DamageType type)
        {
            if (!canDamage)
            {
                return true;
            }
            canDamage = false;
            Timer damageTimer = new Timer(DamageTimerCallBack,null,100,Timeout.Infinite);
            MelonLogger.Msg("-------------------------------------");            
            switch (type)
            {
                case PlayerStats.DamageType.Physical:


                    /*
                    MelonLogger.Msg($"PlayerPosition :{__instance.transform.position}");
                    MelonLogger.Msg($"PlayergameObjectPosition :{__instance.gameObject.transform.position}");
                    MelonLogger.Msg($"damageColliderGameObjectPosition :{damageCollider.gameObject.transform.position}");
                    MelonLogger.Msg($"damageColliderPosition :{damageCollider.transform.position}");

                    MelonLogger.Msg($"Player.name :{__instance.name}");
                    MelonLogger.Msg($"PlayerFullName :{__instance.gameObject.GetFullName()}");

                    MelonLogger.Msg($"damageCollider.name :{damageCollider.name}");
                    MelonLogger.Msg($"FullName :{damageCollider.gameObject.GetFullName()}");

                    MelonLogger.Msg($"hitDir :{__instance.hitReaction.hitDir}");
                    */
                    Collider[] hitColliders = Physics.OverlapSphere(__instance.transform.position, 100);
                    GameObject nearestObject = null;
                    float minDistance = Mathf.Infinity;
                    KeyValuePair<float, float> angle = new KeyValuePair<float, float>();

                    foreach (var hitCollider in hitColliders)
                    {
                        float distance = Vector3.Distance(__instance.transform.position, hitCollider.transform.position);
                        if (distance < minDistance)
                        {
                            
                            if (hitCollider.gameObject.GetFullName().Contains("mutant") || hitCollider.gameObject.GetFullName().Contains("armsy") || hitCollider.gameObject.GetFullName().Contains("LEADER") || hitCollider.gameObject.GetFullName().Contains("creepy") || hitCollider.gameObject.GetFullName().Contains("cannibal") || hitCollider.gameObject.GetFullName().Contains("LEADER"))
                            {
                                minDistance = distance;
                                nearestObject = hitCollider.gameObject;
                                angle = GetAngle(__instance.transform, nearestObject.transform.position);
                            }
                        }
                    }

                    MelonLogger.Msg($"PhysicalDamage, {angle.Key}, {angle.Value}");
                    _TrueGear.PlayAngle("PhysicalDamage",angle.Key,angle.Value);
                    break;
                case PlayerStats.DamageType.Poison:
                    MelonLogger.Msg("PoisonDamage");
                    _TrueGear.Play("PoisonDamage");
                    break;
                case PlayerStats.DamageType.Drowning:
                    MelonLogger.Msg("DrowningDamage");
                    _TrueGear.Play("DrowningDamage");
                    break;
                case PlayerStats.DamageType.Fire:
                    MelonLogger.Msg("FireDamage");
                    _TrueGear.Play("FireDamage");
                    break;
                case PlayerStats.DamageType.Frost:
                    MelonLogger.Msg("FrostDamage");
                    _TrueGear.Play("FrostDamage");
                    break;
                default:
                    return true;
            }
            return true;
        }
        private static void DamageTimerCallBack(object o)
        { 
            canDamage = true;
        }


        [HarmonyPostfix, HarmonyPatch(typeof(FirstPersonCharacter), "OnCollisionEnterProxied")]
        public static void FirstPersonCharacter_OnCollisionEnterProxied_PostPatch(FirstPersonCharacter __instance, Collision coll)
        {
            if (coll.impulse.y >= 100f)
            {
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("FallDamage");
                _TrueGear.Play("FallDamage");
                return;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "Fell")]
        public static void PlayerStats_Fell_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("FallDamage");
            _TrueGear.Play("FallDamage");
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "Explosion", new Type[] { typeof(float), typeof(bool) })]
        public static void PlayerStats_Explosion2_PostPatch(PlayerStats __instance, float dist, bool fromPlayer)
        {
            Collider[] hitColliders = Physics.OverlapSphere(__instance.transform.position, 100);
            GameObject nearestObject = null;
            float minDistance = Mathf.Infinity;
            KeyValuePair<float, float> angle = new KeyValuePair<float, float>();

            foreach (var hitCollider in hitColliders)
            {
                float distance = Vector3.Distance(__instance.transform.position, hitCollider.transform.position);
                if (distance < minDistance)
                {
                    //MelonLogger.Msg(hitCollider.gameObject.GetFullName()); 
                    if (hitCollider.gameObject.GetFullName().Contains("bomb") || hitCollider.gameObject.GetFullName().Contains("explode"))
                    {
                        minDistance = distance;
                        nearestObject = hitCollider.gameObject;
                        angle = GetAngle(__instance.transform, nearestObject.transform.position);
                    }
                }
            }

            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg($"ExplosionDamage,{angle.Key},{angle.Value}");
            _TrueGear.PlayAngle("ExplosionDamage",angle.Key,angle.Value);
        }
        

        //  //////////////////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "KnockOut")]
        public static void PlayerStats_KnockOut_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("PlayerKnockOut");
            MelonLogger.Msg("StartHeartBeat");
            _TrueGear.Play("PlayerKnockOut");
            _TrueGear.StartHeartBeat();
            isHeartBeat = true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "WakeFromKnockOut")]
        public static void PlayerStats_WakeFromKnockOut_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("PlayerWakeFromKnockOut");
            _TrueGear.Play("PlayerWakeFromKnockOut");
        }

        private static bool canLowRecoil = true;
        private static void LowRecoilTimerCallBack(object o)
        {
            canLowRecoil = true;
        }
        //  //////////////////////////////////////////////////////////
        [HarmonyPostfix, HarmonyPatch(typeof(weaponInfo), "OnTriggerEnter")]
        public static void weaponInfo_OnTriggerEnter_PostPatch(weaponInfo __instance, Collider other)
        {

            /*
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("WeaponRecoilRight");
            MelonLogger.Msg("GO Type:" + other.gameObject.GetType());
            MelonLogger.Msg("GO FullName:" + other.gameObject.GetFullName());
            MelonLogger.Msg("GO tag:" + other.gameObject.tag);
            MelonLogger.Msg("GO name:" + other.gameObject.name);
            MelonLogger.Msg("GO layer:" + other.gameObject.layer);
            MelonLogger.Msg("isTrigger:" + other.isTrigger);
            MelonLogger.Msg("name:" + other.name);
            MelonLogger.Msg("tag:" + other.tag);
            return;
            */
            

            //MelonLogger.Msg("GO tag:" + other.gameObject.tag);
            //MelonLogger.Msg(equipId);
            if (equipId == 261 || equipId == 288)
            {
                if (!canLowRecoil)
                {
                    return;
                }
                canLowRecoil = false;
                Timer LowRecoilTimer = new Timer(LowRecoilTimerCallBack,null,250,Timeout.Infinite);
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("WeaponLowRecoilRight");
                _TrueGear.Play("WeaponLowRecoilRight");
                return;
            }
            if (tags.Contains(other.gameObject.tag))
            {
                if (!canWave)
                {
                    return;
                }
                canWave = false;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("WeaponWaveRight");
                _TrueGear.Play("WeaponWaveRight");
                Timer weaponWaveTimer = new Timer(WeaponWaveTimerCallBack, null, 100, Timeout.Infinite);
            }
            else
            {
                if (!canWeaponRecoil)
                {
                    return;
                }
                canWeaponRecoil = false;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("WeaponRecoilRight");
                _TrueGear.Play("WeaponRecoilRight");
                Timer weaponRecoilTimer = new Timer(WeaponRecoilTimerCallBack, null, 500, Timeout.Infinite);
            }      
        }

        private static void WeaponWaveTimerCallBack(object o)
        {
            canWave = true;
        }

        private static void WeaponRecoilTimerCallBack(object o)
        {
            canWeaponRecoil = true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GunShoot), "Start")]
        public static void GunShoot_Start_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("GunShotRight");
            _TrueGear.Play("GunShotRight");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayShootFlareSfx")]
        public static void PlayerSfx_PlayShootFlareSfx_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("GunShotRight");
            _TrueGear.Play("GunShotRight");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayShootFlintLockSfx")]
        public static void PlayerSfx_PlayShootFlintLockSfx_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("GunShotRight");
            _TrueGear.Play("GunShotRight");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(crossbowController), "fireProjectile")]
        public static void crossbowController_fireProjectile_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("CrossbowShotRight");
            _TrueGear.Play("CrossbowShotRight");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(BowController), "OnAmmoFired")]
        public static void BowController_OnAmmoFired_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("BowShotRight");
            _TrueGear.Play("BowShotRight");
        }

        //  //////////////////////////////////////////////////////////
        [HarmonyPostfix, HarmonyPatch(typeof(planeEvents), "fallForward1")]
        public static void planeEvents_fallForward1_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("StartPlaneFall");
            _TrueGear.StartPlaneFall();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(planeEvents), "crashStop")]
        public static void planeEvents_crashStop_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("StopPlaneFall");
            _TrueGear.StopPlaneFall();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(planeEvents), "fallForward2")]
        public static void planeEvents_fallForward2_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("StopPlaneFall");
            MelonLogger.Msg("PlaneHitGround");
            _TrueGear.Play("PlaneHitGround");
            _TrueGear.StopPlaneFall();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(planeEvents), "hitGround")]
        public static void planeEvents_hitGround_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("PlaneHitGround");
            _TrueGear.Play("PlaneHitGround");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(planeEvents), "goBlack")]
        public static void planeEvents_goBlack_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("StopPlaneFall");
            MelonLogger.Msg("StartHeartBeat");
            _TrueGear.StopPlaneFall();
            _TrueGear.StartHeartBeat();
            isHeartBeat = true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(planeCrashHeight), "skipPlaneCrash")]
        public static void planeCrashHeight_skipPlaneCrash_PostPatch()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("skipPlaneCrash");
            _TrueGear.Play("skipPlaneCrash");
        }

        //  //////////////////////////////////////////////////////////
        /*
        [HarmonyPostfix,HarmonyPatch(typeof(enemyWeaponMelee), "OnTriggerEnter")]
        public static void enemyWeaponMelee_OnTriggerEnter_PostPatch(Collider other)
        {
            damageCollider = other;
        }
        */
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerInventory), "AddItem")]
        public static void PlayerInventory_AddItem_PostPatch(PlayerInventory __instance, int itemId)
        {
            if (isEquip)
            { 
                return;
            }
            if (__instance.ItemFilter != null)
            {
                return;
            }
            if (itemId == 34 || itemId == 60 || itemId == 104)
            {
                return;
            }
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("InventoryAddItem");        //Pickup Item
            _TrueGear.Play("InventoryAddItem");
            MelonLogger.Msg(itemId);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerInventory), "RemoveItem")]
        public static void PlayerInventory_RemoveItem_PostPatch(PlayerInventory __instance, int itemId)
        {
            if (isEquip)
            {
                return;
            }
            if (__instance.ItemFilter != null)
            {
                return;
            }
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("InventoryRemoveItem");        //Pickup Item
            _TrueGear.Play("InventoryRemoveItem");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerSfx), "PlayInventorySound")]
        public static void PlayerSfx_PlayInventorySound_PostPatch(PlayerSfx __instance, Item.SFXCommands command)
        {
            switch (command)
            {
                case Item.SFXCommands.PlayEat:
                case Item.SFXCommands.PlayEatMeds:
                    MelonLogger.Msg("-------------------------------------");
                    MelonLogger.Msg("Eating");
                    _TrueGear.Play("Eating");
                    break;
                case Item.SFXCommands.PlayDrink:
                    MelonLogger.Msg("-------------------------------------");
                    MelonLogger.Msg("Drinking");
                    _TrueGear.Play("Drinking");
                    break;
            }
        }



        [HarmonyPostfix, HarmonyPatch(typeof(PlayerInventory), "Equip",new Type[] { typeof(int), typeof(bool) })]
        public static void PlayerInventory_Equip_PostPatch(PlayerInventory __instance, int itemId)
        {
            if (isEquip)
            {
                return;
            }
            isEquip = true;
            if (itemId == 51 || itemId == 48)
            {
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("LeftEquip");
                _TrueGear.Play("LeftEquip");
            }
            else
            {
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("Equip");
                _TrueGear.Play("Equip");
            }            
            equipId = itemId;
            MelonLogger.Msg(itemId);
            Timer equipTimer = new Timer(EquipTimerCallBack,null,20,Timeout.Infinite);
        }

        private static void EquipTimerCallBack(object o)
        { 
            isEquip = false;
        }

        //[HarmonyPostfix, HarmonyPatch(typeof(PickUp), "ToggleIcons")]
        //public static void PickUp_ToggleIcons_PostPatch(PickUp __instance,bool pickup)
        //{
        //    if (__instance._myPickUp && __instance._myPickUp.activeSelf != pickup)
        //    {
        //        if (pickup && __instance.ShouldPlayVRHaptic())
        //        {
        //            MelonLogger.Msg("-------------------------------------");
        //            MelonLogger.Msg("PickupItem");
        //        }
        //    }
        //}

        [HarmonyPostfix, HarmonyPatch(typeof(VRThrowable), "Update")]
        public static void VRThrowable_Update_PostPatch(VRThrowable __instance)
        {
            if (LocalPlayer.Inventory.CurrentView != PlayerInventory.PlayerViews.World)
            {
                return;
            }
            if (TheForest.Utils.Input.GetButton(TheForest.Utils.Input.VRThrowInput))
            {
                if (!checkThrowRelease)
                {
                    MelonLogger.Msg("-------------------------------------");
                    MelonLogger.Msg("ChangeMode");
                    _TrueGear.Play("ChangeMode");
                }
                checkThrowRelease = true;
            }
            else if (checkThrowRelease)
            {
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("Throw");
                _TrueGear.Play("Throw");
                checkThrowRelease = false;
            }
        }

        //  //////////////////////////////////////////////////////////

        //  //////////////////////////////////////////////////////////

        /*
        [HarmonyPrefix, HarmonyPatch(typeof(ForestVRHapticControl), "PlayHitRumble")]
        private static void ForestVRHapticControl_PlayHitRumble_Prefix()
        {
            if (!ForestVRHapticControl.IsHitRumbleEnabled())
            {
                return;
            }
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("PlayHitRumble");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ForestVRHapticControl), "PlayItemHover")]
        private static void ForestVRHapticControl_PlayItemHover_Prefix()
        {
            if (!ForestVRHapticControl.IsItemHoverEnabled())
            {
                return;
            }
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("PlayItemHover");
        }
        */
        [HarmonyPostfix, HarmonyPatch(typeof(MenuMain), "OnEnable")]
        private static void MenuMain_OnEnable_Postfix()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("StopHeartBeat");
            MelonLogger.Msg("StopRaining");
            MelonLogger.Msg("StopShiver");

            _TrueGear.StopHeartBeat();
            _TrueGear.StopHunger();
            _TrueGear.StopRaining();
            _TrueGear.StopShiver();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MenuMain), "OnPlay")]
        private static void MenuMain_OnPlay_Postfix()
        {
            MelonLogger.Msg("-------------------------------------");
            if (playerHealth <= 25.0f)
            {
                playerHealth = 0;
                isHeartBeat = true;
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StartHeartBeat");
                _TrueGear.StartHeartBeat();
            }
            if (isRaining)
            {
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StartRaining");
                _TrueGear.StartRaining();
            }
            if (isShiver)
            {
                MelonLogger.Msg("-------------------------------------");
                MelonLogger.Msg("StartShiver");
                _TrueGear.StartShiver();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MenuMain), "OnExitMenu")]
        private static void MenuMain_OnExitMenu_Postfix()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("OnExitMenu");
            playerHealth = 0;
            isHeartBeat = false;
            isRaining = false;
            isShiver = false;
            isInWater = false;
        }

        [HarmonyPostfix,HarmonyPatch(typeof(PlayerClothing), "RefreshVisibleClothing")]
        private static void PlayerClothing_RefreshVisibleClothing_Postfix()
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("RefreshVisibleClothing");
            _TrueGear.Play("RefreshVisibleClothing");
        }

        /*
        [HarmonyPostfix, HarmonyPatch(typeof(FirstPersonCharacter), "OnCollisionEnterProxied")]
        private static void FirstPersonCharacter_OnCollisionEnterProxied_Postfix(FirstPersonCharacter __instance, Collision coll)
        {
            string name = coll.collider.name;
            string[] source = new string[]
            {
                    "armsy",
                    "mutant",
                    "LEADER"
            };
            Transform transform = __instance.transform;
            foreach (ContactPoint contactPoint in coll.contacts)
            {
                Vector3 vector = contactPoint.point - transform.position;
                if (vector.y < 0f)
                {
                    if (vector.y <= -2f)
                    {
                        if (!source.Any(new Func<string, bool>(name.Contains)))
                        {
                            goto IL_11A;
                        }
                    }
                    KeyValuePair<float, float> angleAndShift = getAngleAndShift(transform, contactPoint.point);
                    angle = angleAndShift;
                    
                    MelonLogger.Msg("-------------------------------------");
                    MelonLogger.Msg($"PhysicalDamage, {angleAndShift.Key}, {angleAndShift.Value}");
                    MelonLogger.Msg(name);
                    _TrueGear.PlayAngle("PhysicalDamage", angleAndShift.Key, angleAndShift.Value);
                    
                }
            IL_11A:;
            }
        }
        */


        [HarmonyPostfix, HarmonyPatch(typeof(PlayerStats), "Update")]
        private static void PlayerStats_Update_Postfix(PlayerStats __instance)
        {
            if (TheForest.Utils.Scene.Atmosphere == null)
            {
                return;
            }
            if (SteamDSConfig.isDedicatedServer)
            {
                return;
            }

            if (__instance.Fullness <= 0.25 && !isHunger)
            {
                isHunger = true;
                _TrueGear.StartHunger();
            }
            else if(__instance.Fullness > 0.25 && isHunger)
            {
                isHunger = false;
                _TrueGear.StopHunger();
            }
        }


        [HarmonyPostfix, HarmonyPatch(typeof(endPlaneAnimEvents), "enableWingExplosion")]
        private static void endPlaneAnimEvents_enableWingExplosion_Postfix(endPlaneAnimEvents __instance)
        {
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("PlaneHitGround");
            _TrueGear.Play("PlaneHitGround");
        }

        private static bool canAttackTree = true;
        [HarmonyPostfix, HarmonyPatch(typeof(chainSawAttackSetup), "setCuttingTreeParameter")]
        private static void chainSawAttackSetup_setCuttingTreeParameter_Postfix(chainSawAttackSetup __instance, float value)
        {
            if (value <= 0f)
            {
                return;
            }
            if (!canAttackTree)
            {
                return;
            }
            canAttackTree = false;
            Timer AttackTreeTimer = new Timer(AttackTreeTimerCallBack, null,110, Timeout.Infinite);
            MelonLogger.Msg("-------------------------------------");
            MelonLogger.Msg("ChainSawAttackTree");
            MelonLogger.Msg(value);
        }

        private static void AttackTreeTimerCallBack(object o)
        {
            canAttackTree = true;
        }

    }
}