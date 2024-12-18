using ATS_API.Effects;
using ATS_API.Helpers;
using BepInEx;
using Eremite;
using Eremite.Controller;
using Eremite.Controller.Generator;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Model.Effects.Hooked;
using HarmonyLib;

namespace ModTemplate
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        private Harmony harmony;

        private void Awake()
        {
            Instance = this;
            harmony = Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(MainController), nameof(MainController.OnServicesReady))]
        [HarmonyPostfix]
        private static void HookMainControllerSetup()
        {
            // This method will run after game load (Roughly on entering the main menu)
            // At this point a lot of the game's data will be available.
            // Your main entry point to access this data will be `Serviceable.Settings` or `MainController.Instance.Settings`
            Instance.Logger.LogInfo($"Performing game initialization on behalf of {PluginInfo.PLUGIN_GUID}.");
            Instance.Logger.LogInfo($"The game has loaded {MainController.Instance.Settings.effects.Length} effects.");
            //foreach (EffectModel effectModel in MB.Settings.effects)
            //{
            //    if (effectModel.IsPerk)
            //    {
            //        Instance.Logger.LogInfo(effectModel.GetType().Name);
            //        Instance.Logger.LogInfo("Found perk with name: " + effectModel.name + " and display name " + effectModel.displayName);
            //    }



            //}
            foreach (LocaText cityName in MB.Settings.worldConfig.citiesNames)
            {
                Instance.Logger.LogInfo("City object: " + cityName + " City name: " + cityName.Text + " City key: " + cityName.key);
            }

        }

        [HarmonyPatch(typeof(MainController), nameof(MainController.InitReferences))]
        [HarmonyPostfix]
        private static void PostSetupMainController()
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

        }


        [HarmonyPatch(typeof(GameController), nameof(GameController.StartGame))]
        [HarmonyPostfix]
        private static void HookEveryGameStart()
        {
            // Too difficult to predict when GameController will exist and I can hook observers to it
            // So just use Harmony and save us all some time. This method will run after every game start
            var isNewGame = MB.GameSaveService.IsNewGame();
            Instance.Logger.LogInfo($"Entered a game. Is this a new game: {isNewGame}.");



            if (isNewGame)
            {
                SO.EffectsService.GrantWildcardPick(1);
                Plugin.Instance.Logger.LogInfo("New wildcard pick granted for new game");

                EffectModel woodCutter = MB.Settings.GetEffect("Resolve for Glade");
                woodCutter.Apply();
                Plugin.Instance.Logger.LogInfo("Resolve for Glade added.");

                EffectModel stickStick = MB.Settings.GetEffect($"{PluginInfo.PLUGIN_GUID}_Stick Gatherers");
                stickStick.Apply();
                Plugin.Instance.Logger.LogInfo("Stick Gatherers added.");

            }
        }
        [HarmonyPatch(typeof(GameLoader), nameof(GameLoader.LoadGame))]
        [HarmonyPostfix]
        private static void SetUpInitialState()
        {
            MB.MetaStateService.Perks.bonusCornerstonesRerolls = 99;
            Plugin.Instance.Logger.LogInfo("Cornerstone rerolls set to 99.");
        }


    }
}
