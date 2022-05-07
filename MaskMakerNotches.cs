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
using RandomizerMod;

namespace MaskMakerNotches
{
    public class MaskMakerNotches : Mod
    {
        internal static MaskMakerNotches Instance;
        internal MMPlacement[] plcs;
        internal CoordinateLocation[] locs;
        internal static int NumberOfNotches = 6;

        public MaskMakerNotches() : base("MaskMakerNotches")
        {
            Instance = this;
        }

        public override string GetVersion() => "v0.1";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing MMN Mod");

            Instance = this;
            On.UIManager.StartNewGame += UIManager_StartNewGame;
            
            Log("MMN Mod Initialized");
        }
        private void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            ItemChangerMod.CreateSettingsProfile();
            plcs = new MMPlacement[3];
            locs = new CoordinateLocation[3];
            for (int i = 0; i < NumberOfNotches+1; i++)
            {
                locs[i] = new MMLocation(25f + i, 7f, $"MMNotchLoc{i}");
                Finder.DefineCustomLocation(locs[i]);
                plcs[i] = new MMPlacement($"MMNotchPlace{i}", locs[i]);
            }
            ItemChangerMod.AddPlacements(plcs);

            orig(self, permaDeath, bossRush);
        }
        private static void SetupRefs(RandomizerMod.RC.RequestBuilder rb)
        {
            string[] locnames = new string[NumberOfNotches];
            for (int i = 0; i < locnames.Length; i++)
            {
                locnames[i] = $"MMNotchLoc{i}";
            }
            foreach(string loc in locnames)
            {
                rb.EditLocationRequest(loc, info =>
                {
                    info.getLocationDef = () => new()
                    {
                        Name = loc,
                        SceneName = Finder.GetLocation(loc).sceneName,
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false,
                    };
                });
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