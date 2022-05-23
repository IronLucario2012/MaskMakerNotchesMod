using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
using ItemChanger;
using ItemChanger.Placements;
using ItemChanger.Locations;
using ItemChanger.Items;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using RandomizerCore.Logic;
using System.IO;
using System.Linq;

namespace MaskMakerNotches
{
    public class MaskMakerNotches : Mod
    {
        internal static MaskMakerNotches Instance;
        internal MMPlacement[] plcs;
        internal CoordinateLocation[] locs;
        internal static int NumberOfNotches = 6;
        internal static string[] locnames = new string[NumberOfNotches];

        public MaskMakerNotches() : base("MaskMakerNotches")
        {
            Instance = this;
            for (int i = 0; i < locnames.Length; i++)
            {
                locnames[i] = $"MMNotchLoc{i}";
            }
        }

        public override string GetVersion() => "v1.0";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing MMN Mod");
            Instance = this;
            ItemChangerMod.CreateSettingsProfile();
            plcs = new MMPlacement[NumberOfNotches];
            locs = new CoordinateLocation[NumberOfNotches];
            for (int i = 0; i < locs.Length; i++)
            {
                locs[i] = new MMLocation(25f + i, 7f, locnames[i]);
                Finder.DefineCustomLocation(locs[i]);
                NotchItem nitem = new()
                {
                    name = $"MMNotch{i}"
                };
                Finder.DefineCustomItem(nitem);
                plcs[i] = new MMPlacement($"{locnames[i]}-Place", locs[i]);
            }
            On.UIManager.StartNewGame += UIManager_StartNewGame;
            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                Hook();
            }
            Log("MMN Mod Initialized");
        }
        private void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            ItemChangerMod.AddPlacements(plcs);

            orig(self, permaDeath, bossRush);
        }
        public static void Hook()
        {
            RequestBuilder.OnUpdate.Subscribe(-499.7f, SetupRefs);
            RequestBuilder.OnUpdate.Subscribe(0.3f, AddNotches);
            RCData.RuntimeLogicOverride.Subscribe(50f, ApplyLogic);
        }
        private static void SetupRefs(RequestBuilder rb)
        {
            
            foreach (string loc in locnames)
            {
                rb.EditLocationRequest(loc, info =>
                {
                    info.getLocationDef = () => new()
                    {
                        Name = loc,
                        SceneName = Finder.GetLocation(loc).sceneName,
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false
                    };
                });
            }
            rb.OnGetGroupFor.Subscribe(0f, MatchCharmGroup);

            static bool MatchCharmGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
            {
                if (IsLocation(item) && (type == RequestBuilder.ElementType.Unknown || type == RequestBuilder.ElementType.Location))
                {
                    gb = rb.GetGroupFor(ItemNames.Charm_Notch);
                    return true;
                }
                gb = default;
                return false;
            }
            static bool IsLocation(string loc)
            {
                bool val = false;
                for (int i = 0; i < locnames.Length; i++)
                {
                    if (locnames[i].Equals(loc))
                    {
                        val = true;
                    }
                }
                return val;
            }
        }
        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            using Stream s = typeof(MaskMakerNotches).Assembly.GetManifestResourceStream("MaskMakerNotches.Resources.logic.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Locations, s);
        }
        private static void AddNotches(RequestBuilder rb)
        {
            rb.AddItemByName(ItemNames.Charm_Notch, NumberOfNotches);
            foreach (string loc in locnames)
            {
                rb.AddLocationByName(loc);
            }
        }
    }
    internal class MMLocation : CoordinateLocation
    {
        public MMLocation(float X, float Y, string Name)
        {
            name = Name;
            x = X;
            y = Y;
            sceneName = "Room_Mask_Maker";
        }
    }
    internal class MMPlacement : MutablePlacement
    {
        public MMPlacement(string Name, ContainerLocation loc) : base(Name)
        {
            Cost = new GeoCost(1000);
            Location = loc;
            containerType = Container.Shiny;
            Items.Add(Finder.GetItem(ItemNames.Charm_Notch));
        }
    }
}