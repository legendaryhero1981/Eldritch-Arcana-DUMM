// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.UnitLogic.FactLogic;

using System;
using System.Collections.Generic;
using System.Linq;

using RES = EldritchArcana.Properties.Resources;

namespace EldritchArcana
{
    static class DrawbackFeats
    {
        static LibraryScriptableObject library => Main.library;

        static BlueprintCharacterClass magus;

        static string[] DrawFeatGuids = new string[10];
        ///internal static BlueprintParametrizedFeature spellPerfection;

        internal static void Load()
        {
            if (!Main.settings.DrawbackForextraTraits) return;
            magus = library.Get<BlueprintCharacterClass>("45a4607686d96a1498891b3286121780");

            for (int i = 1000; i < 1010; i++)
            {
                DrawFeatGuids[i - 1000] = $"{i}63947738b04ecc8e069e050751cc";
                //Log.Write($"{i}63947738b04ecc8e069e050751cc");
            }
            // Load metamagic feats
            //var metamagicFeats = SafeLoad(MetamagicFeats.CreateMetamagicFeats, "Metamagic feats")?.ToArray();
            var DrawbackFeat = Array.Empty<BlueprintFeature>();
            var feats = DrawbackFeat.ToList();

            // Add metamagics to Magus/Sorcerer bonus feat list.
            //var feats = ;
            //----------------------------drawbackfeats testing
            var noFrail = Helpers.PrerequisiteNoFeature(null);
            var noOneleg = Helpers.PrerequisiteNoFeature(null);
            var noSpellVul = Helpers.PrerequisiteNoFeature(null);


            var Frailfeat = CreateFrail();
            var OneLegged = CreateOneLegged();
            var SpellVulnerability = CreateSpellVulnerability();

            noFrail.Feature = Frailfeat;
            noOneleg.Feature = OneLegged;
            noSpellVul.Feature = SpellVulnerability;

            var NoDrawback = new PrerequisiteNoFeature[] { noFrail, noOneleg, noSpellVul };


            feats.Add(Frailfeat);
            feats.Add(OneLegged);
            feats.Add(SpellVulnerability);

            //feats.Add(CreateSpellVulnerability());
            var BasicFeatSelection = library.Get<BlueprintFeatureSelection>("247a4068296e8be42890143f451b4b45");
            foreach (var feat in feats)
            {
                feat.AddComponents(NoDrawback);
                SelectFeature_Apply_Patch.onApplyFeature.Add(feat, (state, unit) =>
                {
                    BasicFeatSelection.AddSelection(state, unit, 1);
                    BasicFeatSelection.AddSelection(state, unit, 1);
                });
            }


            library.AddFeats(feats.ToArray());

        }

        internal static T SafeLoad<T>(Func<T> load, String name) => Main.SafeLoad(load, name);




        static BlueprintFeature CreateFrail()
        {
            var frailsprite = Image2Sprite.Create("Mods/EldritchArcana/sprites/frail.png");
            var fraily = new BlueprintComponent[64];
            fraily[0] = Helpers.CreateAddStatBonus(StatType.HitPoints, -3, ModifierDescriptor.Crippled);
            for (int i = 2; i < 65; i++)
            {
                fraily[i - 1] = Helpers.CreateAddStatBonusOnLevel(StatType.HitPoints, i * -1, ModifierDescriptor.Penalty, i, i);
            };

            var feat = Helpers.CreateFeature("Frail", RES.DrawbackFrailFeatureName_info,
                RES.DrawbackFrailFeatureDescription_info,
                "0639446638b04ecc85e069e050751bfb",
                frailsprite,//Helpers.NiceIcons(9),
                FeatureGroup.Feat,
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Disease; s.Bonus = -1; s.ModifierDescriptor = ModifierDescriptor.Crippled; }),
                //Helpers.Create<FeyFoundlingLogic>(s => { s.dieModefier = 2;}),
                //Helpers.CreateAddStatBonusOnLevel(StatType.HitPoints,-3,ModifierDescriptor.Penalty,1),
                PrerequisiteCharacterLevelExact.Create(1));
            feat.AddComponents(fraily);
            return feat;
        }

        static BlueprintFeature CreateOneLegged()
        {
            var frailsprite = Image2Sprite.Create("Mods/EldritchArcana/sprites/peg_leg.png");
            Log.Write(DrawFeatGuids[0]);
            var feat = Helpers.CreateFeature("OneLegged", RES.DrawbackOneLeggedFeatureName_info,
                RES.DrawbackOneLeggedFeatureDescription_info,
                DrawFeatGuids[0],
                frailsprite,//Helpers.NiceIcons(38),
                FeatureGroup.Feat,
                Helpers.CreateAddStatBonus(StatType.Speed, -10, ModifierDescriptor.Crippled),
                Helpers.CreateAddStatBonus(StatType.Initiative, -2, ModifierDescriptor.Crippled),
                PrerequisiteCharacterLevelExact.Create(1));

            return feat;
        }
        static BlueprintFeature CreateSpellVulnerability()
        {
            //Log.Write(DrawFeatGuids[1]);
            var spellvulsprite = Image2Sprite.Create("Mods/EldritchArcana/sprites/spell_vulnerability.png");
            int SpellVunrabilityBonus = -4;
            var components = new List<BlueprintComponent> { };

            var schoolTypes = new Dictionary<SpellSchool, DamageEnergyType>()
            {
                {SpellSchool.Abjuration,DamageEnergyType.Cold},
                {SpellSchool.Conjuration,DamageEnergyType.Acid},
                {SpellSchool.Enchantment,DamageEnergyType.Sonic},
                {SpellSchool.Evocation,DamageEnergyType.Fire},
                {SpellSchool.Illusion,DamageEnergyType.Electricity},
                {SpellSchool.Necromancy,DamageEnergyType.Unholy},
                {SpellSchool.Transmutation,DamageEnergyType.Divine}
            };

            var spellSchoolChoiceFeature = schoolTypes.Select(schoolType =>
            {
                var result = Helpers.CreateFeature($"SpellVulnerability{schoolType.Key}",
                    string.Format(RES.SpellVulnerabilityFeatureName_info, schoolType.Key),
                    string.Format(RES.SpellVulnerabilityFeatureDescription_info, SpellVunrabilityBonus, schoolType.Key,
                        schoolType.Value), Helpers.MergeIds(DrawFeatGuids[1], Helpers.spellSchoolGuid(schoolType.Key)),
                    Helpers.GetIcon(Helpers.spellSchoolGuid(schoolType.Key)), FeatureGroup.None,
                    Helpers.Create<SavingThrowBonusAgainstSchool>(a =>
                    {
                        a.School = schoolType.Key;
                        a.Value = SpellVunrabilityBonus;
                        a.ModifierDescriptor = ModifierDescriptor.Penalty;
                    }));
                result.AddComponent(Helpers.Create<AddEnergyVulnerability>(a => a.Type = schoolType.Value));
                return result;
            }).ToArray();

            //var noFeature = Helpers.PrerequisiteNoFeature(null);

            var feat = Helpers.CreateFeatureSelection("SpellVulnerability", RES.DrawbackSpellVulnerabilityFeatureName_info,
                string.Format(RES.DrawbackSpellVulnerabilityFeatureDescription_info, SpellVunrabilityBonus),
                DrawFeatGuids[1],
                spellvulsprite,//Helpers.NiceIcons(15),
                FeatureGroup.Feat,
                PrerequisiteCharacterLevelExact.Create(1));
            //feat.AddComponents(ElementalWeaknesChoiceFeature);
            //noFeature.Feature = feat;
            feat.SetFeatures(spellSchoolChoiceFeature);
            //feat.AddComponents(ikweethetniet);
            //components.AddRange(ikweethetniet);

            return feat;
        }

    }
}
