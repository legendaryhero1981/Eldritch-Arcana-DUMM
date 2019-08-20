// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using Newtonsoft.Json;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

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
            var frailsprite = Image2Sprite.Create("Mods/EldritchArcana/sprites/Icon_Frail.png");
            var fraily = new BlueprintComponent[64];
            fraily[0] = Helpers.CreateAddStatBonus(StatType.HitPoints, -3, ModifierDescriptor.Crippled);
            for (int i = 2; i < 65; i++)
            {
                fraily[i - 1] = Helpers.CreateAddStatBonusOnLevel(StatType.HitPoints, i * -1, ModifierDescriptor.Penalty, i, i);
            };

            var feat = Helpers.CreateFeature("Frail", "Frail",
                "You have A frail body and you break bones easily also your health is not great so you have -1 vs disease saves.\n" +
                "if you pick a Drawback at level one you can choose an extra feat on top\n" +
                "Drawback: -3 hp at level 1 and You lose 1 additional hit points.For every Hit Die you possess.",
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
            var frailsprite = Image2Sprite.Create("Mods/EldritchArcana/sprites/Icon_Peg_Leg.png");
            Log.Write(DrawFeatGuids[0]);
            var feat = Helpers.CreateFeature("OneLegged", "Peg leg",
                "if you pick a Drawback at level one you can choose an extra feat on top\n" +
                "Drawback: Even with a peg leg, you lose 10 feet from your race’s normal speed.\n" +
                "Drawback: You lose 2 initiative",
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
            var spellvulsprite = Image2Sprite.Create("Mods/EldritchArcana/sprites/Icon_spell_Vulnerability.png");
            int SpellVunrabilityBonus = -4;
            var components = new List<BlueprintComponent> { };
            var SpellschoolChoiceFeature = (new SpellSchool[]
            {
                SpellSchool.Abjuration,
                SpellSchool.Conjuration,
                //SpellSchool.Divination,
                SpellSchool.Enchantment,
                SpellSchool.Evocation,
                SpellSchool.Illusion,
                SpellSchool.Necromancy,
                SpellSchool.Transmutation,
                //SpellSchool.Universalist
            }).Select((school) => Helpers.CreateFeature($"SpellVulnerability{school}", $"SpellVulnerability-{school}",
            $" you have {SpellVunrabilityBonus} on saves vs {school}", Helpers.MergeIds(DrawFeatGuids[1], Helpers.spellSchoolGuid(school)),
            Helpers.GetIcon(Helpers.spellSchoolGuid(school)), FeatureGroup.None,
            Helpers.Create<SavingThrowBonusAgainstSchool>(a =>
            {
                a.School = school;
                a.Value = SpellVunrabilityBonus;
                a.ModifierDescriptor = ModifierDescriptor.Penalty;
            }))).ToArray();

            var ElementalWeaknes = new DamageEnergyType[] {
                DamageEnergyType.Cold,
                DamageEnergyType.Acid,
                //divination
                DamageEnergyType.Sonic,
                DamageEnergyType.Fire,
                DamageEnergyType.Electricity,
                DamageEnergyType.Unholy,
                DamageEnergyType.Divine,
                //universalist
            };

            BlueprintFeature feature = SpellschoolChoiceFeature[1];
            for (int i = 0; i < 7; i++)
            {
                feature = SpellschoolChoiceFeature[i];
                feature.SetDescription(feature.GetDescription() + $" and Elementalweakness {ElementalWeaknes[i]}");
                feature.AddComponent(Helpers.Create<AddEnergyVulnerability>(a => { a.Type = ElementalWeaknes[i]; }));
            }

            //var noFeature = Helpers.PrerequisiteNoFeature(null);

            var feat = Helpers.CreateFeatureSelection("SpellVulnerability", "Spell Vulnerability",
                "if you pick a Drawback at level one you can choose an extra feat on top\n" +
                $"Bane: choose a spellschool you have {SpellVunrabilityBonus} on saves vs that spellschool.",//\n(except universalist and divination becouse there are no saves of those catagory)
                DrawFeatGuids[1],
                spellvulsprite,//Helpers.NiceIcons(15),
                FeatureGroup.Feat,
                PrerequisiteCharacterLevelExact.Create(1));
            //feat.AddComponents(ElementalWeaknesChoiceFeature);
            //noFeature.Feature = feat;
            feat.SetFeatures(SpellschoolChoiceFeature);


            //feat.AddComponents(ikweethetniet);
            //components.AddRange(ikweethetniet);

            return feat;
        }

    }
}



