﻿using Modding;
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
            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                Hook();
            }
            Log("MMN Mod Initialized");
        }
        private void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            ItemChangerMod.CreateSettingsProfile();
            plcs = new MMPlacement[NumberOfNotches];
            locs = new CoordinateLocation[NumberOfNotches];
            for (int i = 0; i < locs.Length; i++)
            {
                locs[i] = new MMLocation(25f + i, 7f, $"MMNotchLoc{i}");
                Finder.DefineCustomLocation(locs[i]);
                plcs[i] = new MMPlacement($"MMNotchPlace{i}", locs[i]);
            }
            ItemChangerMod.AddPlacements(plcs);

            orig(self, permaDeath, bossRush);
        }
        public static void Hook()
        {
            RequestBuilder.OnUpdate.Subscribe(-499.7f, SetupRefs);
            RequestBuilder.OnUpdate.Subscribe(0.3f, AddNotches);
        }
        private static void SetupRefs(RequestBuilder rb)
        {
            string[] locnames = new string[NumberOfNotches];
            for (int i = 0; i < locnames.Length; i++)
            {
                locnames[i] = $"MMNotchLoc{i}";
            }
            foreach (string loc in locnames)
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
            rb.OnGetGroupFor.Subscribe(0f, MatchCharmGroup);

            static bool MatchCharmGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
            {
                gb = rb.GetGroupFor(ItemNames.Charm_Notch);

                return true;
            }
        }
        private static void AddNotches(RequestBuilder rb)
        {
            
            for (int i = 0; i < NumberOfNotches-1; i++)
            {
                rb.AddLocationByName($"MMNotchLoc{i}");
                rb.AddItemByName(ItemNames.Charm_Notch);
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