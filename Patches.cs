using HarmonyLib;
using System;
using System.Collections.Generic;
using CommonModNS;

namespace EnemyDifficultyModNS
{
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.CardCanBeSold))]
    internal class WorldManager_CanCardBeSold
    {
        public static bool AllowSales = true;
        public static bool AllowGoldToShellConversion = false;

        private static bool test(string id)
        {
            GameBoard curBoard = WorldManager.instance.GetCurrentBoardSafe();
            if (id == Cards.gold && (AllowGoldToShellConversion || !curBoard.BoardOptions.UsesShells)) return true;
            if (id == Cards.shell && (AllowGoldToShellConversion || curBoard.BoardOptions.UsesShells)) return true;
            return false;
        }

        private static void Prefix(WorldManager __instance, GameCard card)
        {
            if (!AllowSales) return;
            if (card.IsPartOfStack())
            {
                foreach (GameCard item in card.GetAllCardsInStack())
                {
                    if (test(item.CardData.Id))
                    {
                        item.CardData.Value = 1;
                    }
                }
            }
            else if (test(card.CardData.Id))
            {
                card.CardData.Value = 1;
            }
        }

        private static void Postfix(WorldManager __instance, ref bool __result, GameCard card)
        {
            if (!AllowSales) return;
            if (card.IsPartOfStack())
            {
                foreach (GameCard item in card.GetAllCardsInStack())
                {
                    if (test(item.CardData.Id))
                    {
                        item.CardData.Value = -1;
                    }
                }
            }
            else if (test(card.CardData.Id))
            {
                card.CardData.Value = -1;
            }

            if (test(card.CardData.Id)) __result = true;
        }
    }

    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.SellCard))]
    internal class WorldManager_SellCard
    {
        public static bool AllowSales = true;
        public static bool AllowGoldToShellConversion = true;

        private static bool test(string id)
        {
            GameBoard curBoard = WorldManager.instance.GetCurrentBoardSafe();
            if (id == Cards.gold && (AllowGoldToShellConversion || !curBoard.BoardOptions.UsesShells)) return true;
            if (id == Cards.shell && (AllowGoldToShellConversion || curBoard.BoardOptions.UsesShells)) return true;
            return false;
        }

        private static void Prefix(WorldManager __instance, GameCard card)
        {
            if (!AllowSales) return;
            if (card.IsPartOfStack())
            {
                foreach (GameCard item in card.GetAllCardsInStack())
                {
                    if (test(item.CardData.Id))
                    {
                        item.CardData.Value = 1;
                    }
                }
            }
            else if (test(card.CardData.Id))
            {
                card.CardData.Value = 1;
            }
        }

        private static void Postfix(WorldManager __instance, GameCard card)
        {
            if (!AllowSales) return;
            if (card.IsPartOfStack())
            {
                foreach (GameCard item in card.GetAllCardsInStack())
                {
                    if (test(item.CardData.Id))
                    {
                        item.CardData.Value = -1;
                    }
                }
            }
            else if (test(card.CardData.Id))
            {
                card.CardData.Value = -1;
            }
        }
    }
}
