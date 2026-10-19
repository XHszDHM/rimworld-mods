using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VCEPUWasteFix
{
    // Replaces the broken CompWasteProducer.ProduceWaste postfix shipped by
    // Vanilla Chemfuel Expanded - Unofficial Expansion (packageId bambaryla.VCEPU).
    //
    // Original bug (VCEPlusUltra.CompWasteProducer_ProduceWaste_Patch.Postfix):
    //     foreach (Thing item in owner) { if (item.def == Wastepack) owner.TryDrop(item, ...); }
    // TryDrop removes the thing from the owner while the enumerator is live, so the
    // next MoveNext throws "Collection was modified; enumeration operation may not execute".
    //
    // This patch unpatches that postfix at startup and installs an equivalent one that
    // snapshots the matching things first, then drops them.
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        static Bootstrap()
        {
            try
            {
                Harmony harmony = new Harmony("xhszdhm.vcepu.wastefix");
                MethodInfo target = AccessTools.Method(typeof(CompWasteProducer), "ProduceWaste", new Type[] { typeof(int) });
                if (target == null)
                {
                    Log.Error("[VCEPUWasteFix] CompWasteProducer.ProduceWaste(int) not found; aborting.");
                    return;
                }

                MethodInfo bad = AccessTools.Method("VCEPlusUltra.CompWasteProducer_ProduceWaste_Patch:Postfix");
                if (bad == null)
                {
                    Log.Message("[VCEPUWasteFix] VCEPU postfix not present (VCEPU missing or renamed); nothing to do.");
                    return;
                }

                harmony.Unpatch(target, bad);
                Patches info = Harmony.GetPatchInfo(target);
                bool stillThere = false;
                if (info != null)
                {
                    foreach (Patch p in info.Postfixes)
                    {
                        if (p.PatchMethod == bad)
                        {
                            stillThere = true;
                            break;
                        }
                    }
                }
                if (stillThere)
                {
                    Log.Error("[VCEPUWasteFix] Unpatch failed; skipping replacement to avoid double-dropping.");
                    return;
                }
                Log.Message("[VCEPUWasteFix] removed VCEPU Postfix (Collection-was-modified bug).");

                MethodInfo replacement = typeof(WasteDropFix).GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);
                harmony.Patch(target, postfix: new HarmonyMethod(replacement));
                Log.Message("[VCEPUWasteFix] installed safe replacement Postfix.");
            }
            catch (Exception e)
            {
                Log.Error("[VCEPUWasteFix] init failed: " + e);
            }
        }
    }

    public static class WasteDropFix
    {
        public static void Postfix(CompWasteProducer __instance)
        {
            try
            {
                ThingOwner owner = ThingOwnerUtility.TryGetInnerInteractableThingOwner(__instance.parent);
                if (owner == null)
                {
                    return;
                }

                List<Thing> drops = null;
                foreach (Thing t in owner)
                {
                    if (t.def == ThingDefOf.Wastepack)
                    {
                        if (drops == null)
                        {
                            drops = new List<Thing>();
                        }
                        drops.Add(t);
                    }
                }

                if (drops == null)
                {
                    return;
                }

                IntVec3 pos = __instance.parent.Position;
                Map map = __instance.parent.Map;
                for (int i = 0; i < drops.Count; i++)
                {
                    Thing made;
                    owner.TryDrop(drops[i], pos, map, ThingPlaceMode.Near, drops[i].stackCount, out made);
                }
            }
            catch (Exception e)
            {
                Log.Error("[VCEPUWasteFix] drop failed: " + e);
            }
        }
    }
}
