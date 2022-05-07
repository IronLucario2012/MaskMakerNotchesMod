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

namespace MaskMakerNotches
{
    public class MaskMakerNotches : Mod
    {
        internal static MaskMakerNotches Instance;
        internal MMPlacement[] plcs;

        public MaskMakerNotches() : base("MaskMakerNotches")
        {
            Instance = this;
        }

        public override string GetVersion() => "v0.1";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing MMN Mod");

            Instance = this;
            ItemChangerMod.CreateSettingsProfile();
            plcs = new MMPlacement[3];
            CoordinateLocation[] locs = new CoordinateLocation[3];
            for (int i = 0; i < 3; i++)
            {
                locs[i] = new MMLocation(30f+i,7f);
                plcs[i] = new MMPlacement("MMNotch" + i, locs[i]);
            }
            ItemChangerMod.AddPlacements(plcs);
            Log("MMN Mod Initialized");
        }
    }
    internal class MMLocation : CoordinateLocation
    {
        public MMLocation(float X, float Y)
        {
            x = X;
            y = Y;
            sceneName = "Room_Mask_Maker";
        }
        public override string ToString()
        {
            return $"Scene: {sceneName}, X: {x}, Y: {y}";
        }
    }
    internal class MMPlacement : MutablePlacement
    {
        public MMPlacement(string Name, ContainerLocation loc) : base(Name)
        {
            Cost = new GeoCost(1000);
            Location = loc;
            containerType = Container.Shiny;
            Items.Add(new NotchItem());
        }
    }
}