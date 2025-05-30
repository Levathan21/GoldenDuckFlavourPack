﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace GDFP.HarmonyPatches;

[HarmonyPatch(typeof(FloatMenuMakerMap))]
public static class FloatMenuMakerMap_Patch
{
    [HarmonyPatch(nameof(FloatMenuMakerMap.ChoicesAtFor))]
    [HarmonyPostfix]
    public static void ChoicesAtForPatch(ref List<FloatMenuOption> __result, Vector3 clickPos, Pawn pawn)
    {
        if(__result.NullOrEmpty()) return;
        IntVec3 intVec3 = IntVec3.FromVector3(clickPos);

        List<Thing> thingList = intVec3.GetThingList(pawn.Map);
        foreach (Building_Quackaai portal in thingList.OfType<Building_Quackaai>())
        {
            if(portal is Building_QuackaaiExit) continue;

            if (portal.IsOpen && (portal.planetMap == null || !portal.planetMap.mapPawns.FactionsOnMap().Contains(Faction.OfPlayer)))
            {
                __result.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(portal.CloseCommandString, delegate
                {
                    Job job = JobMaker.MakeJob(GDFPDefOf.GDFP_CloseGate, portal);
                    job.playerForced = true;
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
                }, MenuOptionPriority.High, null, null, 0f, null, null, true, 0), pawn, portal, "ReservedBy", null));
            }
        }
    }
}
