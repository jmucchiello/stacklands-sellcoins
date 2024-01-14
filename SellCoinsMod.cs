using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using CommonModNS;
using EnemyDifficultyModNS;

namespace SellCoinsModNS
{
    public class SellCoinsMod : Mod
    {
        public static SellCoinsMod instance;
        public static void Log(string msg) => instance.Logger.Log(msg);
        public static void LogError(string msg) => instance.Logger.LogError(msg);

        private ConfigEntryBool configSellCoins;
        private ConfigEntryBool configConvert;

        private void Awake()
        {
            instance = this;
            CreateSettings();
            Harmony.PatchAll();
        }

        private void CreateSettings()
        {
            configSellCoins = new ConfigEntryBool("sellcoinsmod_enable", Config, true, new ConfigUI()
            {
                NameTerm = "sellcoinsmod_enable",
                TooltipTerm = "sellcoinsmod_enable_tooltip"
            })
            {
                currentValueColor = Color.blue,
                FontSize = 25
            };

            configConvert = new ConfigEntryBool("sellcoinsmod_convert", Config, false, new ConfigUI()
            {
                NameTerm = "sellcoinsmod_convert",
                TooltipTerm = "sellcoinsmod_convert_tooltip"
            })
            {
                currentValueColor = Color.blue,
                FontSize = 25
            };

            Config.OnSave += delegate () {
                ApplySettings();
            };
        }

        private void ApplySettings()
        {
            WorldManager_CanCardBeSold.AllowSales = configSellCoins.Value;
            WorldManager_CanCardBeSold.AllowGoldToShellConversion = configConvert.Value;
            WorldManager_SellCard.AllowSales = configSellCoins.Value;
            WorldManager_SellCard.AllowGoldToShellConversion = configConvert.Value;
            Log($"Allow Sales {WorldManager_SellCard.AllowSales} Convert {WorldManager_SellCard.AllowGoldToShellConversion}");
        }

        public override void Ready()
        {
            ApplySettings();
            Logger.Log("Ready!");
        }
    }

    [HarmonyPatch(typeof(Harvestable), "CanHaveCard")]
    internal class Harvestable_CanHaveCard
    {
        private static void Postfix(Harvestable __instance, ref bool __result, CardData otherCard)
        {
            if (__result) return;
            if (__instance.MyCardType == CardType.Locations && otherCard.MyCardType == CardType.Locations) __result = true;
            else if (__instance.MyCardType == CardType.Structures && !__instance.IsBuilding && otherCard.MyCardType == CardType.Structures && !__instance.IsBuilding) __result = true;
        }
    }

    [HarmonyPatch(typeof(Harvestable), "UpdateCard")]
    internal class Harvestable_UpdateCard
    {
        private static void Postfix(Harvestable __instance)
        {
            if (__instance.AllChildrenMatchPredicate(x => x is Harvestable && x.Id == __instance.Id || x is Villager)) return;

            GameCard cardWithStatusInStack = __instance.MyGameCard.GetCardWithStatusInStack();
            string actionId = __instance.GetActionId("CompleteHarvest");
            if (cardWithStatusInStack != null && cardWithStatusInStack.TimerRunning && cardWithStatusInStack.TimerActionId == actionId && __instance.AnyChildMatchesPredicate((CardData x) => x is Harvestable))
            {
                I.Log($"Harvestable.UpdateCard MATCH");
                cardWithStatusInStack.CancelTimer(actionId);                
            }
        }
    }
}