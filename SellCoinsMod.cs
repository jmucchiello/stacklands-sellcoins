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
                currentValueColor = Color.blue
            };

            configConvert = new ConfigEntryBool("sellcoinsmod_convert", Config, false, new ConfigUI()
            {
                NameTerm = "sellcoinsmod_convert",
                TooltipTerm = "sellcoinsmod_convert_tooltip"
            })
            {
                currentValueColor = Color.blue
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
}