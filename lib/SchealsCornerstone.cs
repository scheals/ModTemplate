using System;
using System.ComponentModel;
using ATS_API;
using ATS_API.Effects;
using ATS_API.Helpers;
using BepInEx;
using Eremite;
using Eremite.Controller;
using Eremite.Controller.Generator;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Model.Effects.Hooked;
using SchealsLearningMod;

namespace SchealsLearningMod.lib
{
    internal class SchealsCornerstones
    {
        public static void addStickGatherers()
        {
            string effectName = "Stick Gatherers";
            string effectIconPath = "StickStick.jpg";
            int amount = 7;

            GoodModel woodGoodModel = MB.Settings.GetGood(GoodsTypes.Mat_Raw_Wood.ToName());
            GoodRef woodGoodRef = new() { good = woodGoodModel, amount = amount };
            EffectBuilder<GoodsPerMinEffectModel> builder = new(PluginInfo.PLUGIN_GUID, effectName, effectIconPath);
            builder.SetRarity(EffectRarity.Epic);
            builder.SetPositive(true);
            builder.SetDrawLimit(5);
            builder.SetAvailableInAllBiomesAndSeasons();
            builder.SetObtainedAsCornerstone();
            builder.SetLabel("Scheals' Perk");
            builder.SetDisplayName(effectName);
            builder.SetDescription($"Gain {amount} of Wood {woodGoodModel.GetTextIcon()} every minute.");
            builder.EffectModel.good = woodGoodRef;

            Plugin.myLog.LogInfo($"Added: {builder.Name}");
        }
    }
}
