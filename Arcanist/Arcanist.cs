// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Root;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using Newtonsoft.Json;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics.Components;
using static Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbilityResourceLogic;
using Kingmaker.UnitLogic.ActivatableAbilities;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

namespace EldritchArcana
{
    static class ArcanistClass
    {
        static LibraryScriptableObject library => Main.library;
        internal static BlueprintCharacterClass arcanist;
        internal static BlueprintCharacterClass[] arcanistArray;
        internal static BlueprintFeature arcanistCantrips;
        static internal BlueprintFeature Arcanist_reservoir;



        internal static void Load()
        {
            if (ArcanistClass.arcanist != null) return;
            var sorcerer = Helpers.GetClass("b3a505fb61437dc4097f43c3f8f9a4cf");
            var wizard = Helpers.GetClass("ba34257984f4c41408ce1dc2004e342e");
            string arcanistGuid = "26416b7c382e32ccb5b615bc17ccc797";
            string ArcanistSpellbookGuid = "550161ffb24541148f787f1d4561fd6c";
            string ArcanistProficienciesGuid = "6fa029dd97a344be84ac6ee53d921cb9";
            string ArcanistCantripsGuid = "904f17a28c1d3fde8a3afb036c4a9b42";
            var Arcanistguidlist = new string[100];
            String baseguid = "a3a5ccc9c670e6c4ca4a686d23";
            int x = 0;
            for (long i = 29999; i < 30099; i++)
            {
                Arcanistguidlist[x] = baseguid + i.ToString();
                x++;
            }

            var arcanist = ArcanistClass.arcanist = Helpers.Create<BlueprintCharacterClass>();
            arcanistArray = new BlueprintCharacterClass[] { ArcanistClass.arcanist };
            ArcanistClass.arcanist.name = "ArcanistClass";
            library.AddAsset(ArcanistClass.arcanist, arcanistGuid);
            ArcanistClass.arcanist.LocalizedName = Helpers.CreateString("Arcanist.Name", "Arcanist(W.I.P)");
            ArcanistClass.arcanist.LocalizedDescription = Helpers.CreateString("Arcanist.Description", "Arcanists are scholars of all things magical.\n" +
                "They constantly seek out new forms of magic to discover how they work, and in many cases, to collect the energy of such magic for their own uses. Many arcanists are seen as reckless," +
                "\nmore concerned with the potency of magic than the ramifications of unleashing such power.");
            ArcanistClass.arcanist.m_Icon = sorcerer.Icon;
            ArcanistClass.arcanist.SkillPoints = 1;
            ArcanistClass.arcanist.HitDie = DiceType.D6;
            ArcanistClass.arcanist.BaseAttackBonus = sorcerer.BaseAttackBonus;
            ArcanistClass.arcanist.FortitudeSave = sorcerer.FortitudeSave;
            ArcanistClass.arcanist.ReflexSave = wizard.ReflexSave;
            ArcanistClass.arcanist.WillSave = wizard.WillSave;

            var spellbook = Helpers.Create<BlueprintSpellbook>();
            spellbook.name = "ArcanistSpellbook";
            library.AddAsset(spellbook, ArcanistSpellbookGuid);
            spellbook.Name = ArcanistClass.arcanist.LocalizedName;
            var magusSpellLevels = library.Get<BlueprintSpellsTable>("6326b540f7c6a604f9d6f82cc0e2293c");
            var wizardLevels = library.Get<BlueprintSpellsTable>("78bb94ed2e75122428232950bb09e97b");
            spellbook.SpellsPerDay = wizardLevels;
            spellbook.SpellList = wizard.Spellbook.SpellList;
            spellbook.SpellsPerLevel = 2;
            spellbook.Spontaneous = false;
            spellbook.IsArcane = true;
            spellbook.AllSpellsKnown = false;
            spellbook.CanCopyScrolls = true;
            spellbook.CastingAttribute = StatType.Intelligence;
            spellbook.CharacterClass = ArcanistClass.arcanist;
            spellbook.CantripsType = CantripsType.Cantrips;
            ArcanistClass.arcanist.Spellbook = spellbook;

            // Consolidated skills make this a bit of a judgement call. Explanation below.
            ArcanistClass.arcanist.ClassSkills = new StatType[] {
                // Warpriests have Diplomacy, Intimidate and Sense Motive (which in PF:K is like Persuasion).
                StatType.SkillPersuasion,
                // Warpriests have Climb/Swim
                StatType.SkillAthletics,
                // Warpriests have Knowledge (religion) which is the main part of the consolidated skill.
                StatType.SkillLoreReligion,
                // Warpriests have Survial and Handle Animal
                StatType.SkillLoreNature
            };
            


            ArcanistClass.arcanist.IsDivineCaster = false;
            ArcanistClass.arcanist.IsArcaneCaster = true;

            var paladin = library.Get<BlueprintCharacterClass>("bfa11238e7ae3544bbeb4d0b92e897ec");
            //var ArcanePoolFeature = library.Get<BlueprintFeature>("3ce9bb90749c21249adc639031d5eed1");//magus
            ArcanistClass.arcanist.StartingGold = paladin.StartingGold; // all classes start with 411.
            ArcanistClass.arcanist.PrimaryColor = paladin.PrimaryColor;
            ArcanistClass.arcanist.SecondaryColor = paladin.SecondaryColor;

            ArcanistClass.arcanist.RecommendedAttributes = new StatType[] { StatType.Intelligence,StatType.Dexterity };
            ArcanistClass.arcanist.NotRecommendedAttributes = new StatType[] { StatType.Strength };

            ArcanistClass.arcanist.EquipmentEntities = wizard.EquipmentEntities;
            ArcanistClass.arcanist.MaleEquipmentEntities = wizard.MaleEquipmentEntities;
            ArcanistClass.arcanist.FemaleEquipmentEntities = sorcerer.FemaleEquipmentEntities;
            
            // Both of the restrictions here are relevant (no atheism feature, no animal class).
            ArcanistClass.arcanist.ComponentsArray = wizard.ComponentsArray;
            ArcanistClass.arcanist.HideIfRestricted = true;
            ArcanistClass.arcanist.AddComponent(ArcanistClass.arcanist.PrerequisiteClassLevel(1));

            ArcanistClass.arcanist.StartingItems = wizard.StartingItems;
            
            var progression = Helpers.CreateProgression("ArcanistProgression",
                ArcanistClass.arcanist.Name,
                ArcanistClass.arcanist.Description,
                Arcanistguidlist[50],
                ArcanistClass.arcanist.Icon,
                FeatureGroup.None);
            progression.Classes = arcanistArray;
            var entries = new List<LevelEntry>();


            //cantrips.SetDescription("arcanists learn a number of cantrips, or 0-level spells. These spells are cast like any other spell, but they do not consume any slots and may be used again.");
            //
            

            var cantrips = Helpers.createCantrips("ArcanistCantripsFeature",
                                                   "Cantrips",
                                                   "Arcanists can cast a number of cantrips, or 0-level spells. These spells are cast like any other spell, but they are not expended when cast and may be used again.",
                                                   Helpers.NiceIcons(999),//replaceicon
                                                   ArcanistCantripsGuid,
                                                   arcanist,
                                                   StatType.Intelligence,
                                                   arcanist.Spellbook.SpellList.SpellsByLevel[0].Spells.ToArray());
        
            

            var proficiencies = library.CopyAndAdd<BlueprintFeature>(
                "a98d7cc4e30fe6c4bb3a2c2f69acc3fe", // wizard proficiencies
                "ArcanistProficiencies",
                ArcanistProficienciesGuid);
            proficiencies.SetName("arcanist Proficiencies");
            proficiencies.SetDescription("A arcanist is proficient with all simple weapons, as well as the favored weapon of their deity.");

            var detectMagic = library.Get<BlueprintFeature>("ee0b69e90bac14446a4cf9a050f87f2e");
            var deitySelection = library.Get<BlueprintFeatureSelection>("59e7a76987fe3b547b9cce045f4db3e4");


            var MagusSpellRecallFeature = library.CopyAndAdd<BlueprintFeature>("61fc0521e9992624e9c518060bf89c0f", "ArcanistPool",Arcanistguidlist[1]);
            var MagusSpellRecall = library.CopyAndAdd<BlueprintAbility>("1bd76e00b6e056d42a8ecc1031dd43b4","ArcanistSpellrecall", Arcanistguidlist[2]);
            MagusSpellRecallFeature.SetDescription("Arcanist Reservoir use this reservoir to empower your spells through varius means.");
            createArcaneReservoir();
            var Yog = library.CopyAndAdd<BlueprintFeature>("a3a5ccc9c670e6f4ca4a686d23b89900", "Yog", Arcanistguidlist[98]);
            Yog.SetDescription("praise yog");
            /*
            foreach (var componen in Yog.ComponentsArray)
            {
                Log.Write(componen.name);
            }/*
            foreach (StatType stat in (StatType[])Enum.GetValues(typeof(StatType)))
            {
                Log.Write(stat.ToString());
            }
            */
            var ExploitSelection = CreateArcanistExploitSelection();
            entries.Add(Helpers.LevelEntry(1,
            proficiencies,
            deitySelection,
            cantrips,
            //ArcanePoolFeature,
            ExploitSelection,
            Arcanist_reservoir,
            detectMagic
            ));
            var WizardFeatSelection = library.Get<BlueprintFeatureSelection>("8c3102c2ff3b69444b139a98521a4899");
            var fighterFeat = library.Get<BlueprintFeatureSelection>("41c8486641f7d6d4283ca9dae4147a9f");
            //entries.Add(Helpers.LevelEntry(2)); 
            entries.Add(Helpers.LevelEntry(3, ExploitSelection, WizardFeatSelection));
            //entries.Add(Helpers.LevelEntry(4)); 
            entries.Add(Helpers.LevelEntry(5, ExploitSelection, WizardFeatSelection)); 
            //entries.Add(Helpers.LevelEntry(6));
            entries.Add(Helpers.LevelEntry(7, ExploitSelection, WizardFeatSelection)); 
            //entries.Add(Helpers.LevelEntry(8)); 
            entries.Add(Helpers.LevelEntry(9, ExploitSelection, WizardFeatSelection));
            //entries.Add(Helpers.LevelEntry(10)); 
            entries.Add(Helpers.LevelEntry(11, ExploitSelection, WizardFeatSelection)); 
            //entries.Add(Helpers.LevelEntry(12));
            entries.Add(Helpers.LevelEntry(13, ExploitSelection, WizardFeatSelection)); 
            //entries.Add(Helpers.LevelEntry(14)); 
            entries.Add(Helpers.LevelEntry(15, ExploitSelection, WizardFeatSelection));
            //entries.Add(Helpers.LevelEntry(16)); 
            entries.Add(Helpers.LevelEntry(17, ExploitSelection, WizardFeatSelection)); 
            //entries.Add(Helpers.LevelEntry(18));
            entries.Add(Helpers.LevelEntry(19, ExploitSelection, WizardFeatSelection)); 
            entries.Add(Helpers.LevelEntry(20,Yog)); 
            progression.UIDeterminatorsGroup = new BlueprintFeatureBase[] {
                // TODO: 1st level stuff
            };
            progression.UIGroups = Helpers.CreateUIGroups(); // TODO
            progression.LevelEntries = entries.ToArray();

            ArcanistClass.arcanist.Progression = progression;

            ArcanistClass.arcanist.Archetypes = Array.Empty<BlueprintArchetype>();

            ArcanistClass.arcanist.RegisterClass();
            
        }


        //modified createConduitSurge





        static BlueprintFeatureSelection CreateArcanistExploitSelection()
        {
            //Familiar(Su)
            var selection = Helpers.CreateFeatureSelection("ArcanistExploitSelection", "Select Exploit",
                "By bending and sometimes even breaking the rules of magic, the arcanist learns to exploit gaps and exceptions in the laws of magic. Some of these exploits allow her to break down various forms of magic, adding their essence to her arcane reservoir. At 1st level and every 2 levels thereafter, the arcanist learns a new arcane exploit selected from the following list. An arcanist exploit cannot be selected more than once. Once an arcanist exploit has been selected, it cannot be changed. Most arcanist exploits require the arcanist to expend points from her arcane reservoir to function. Unless otherwise noted, the saving throw DC for an arcanist exploit is equal to 10 + 1/2 the arcanist’s level + the arcanist’s Charisma modifier..",
                "4e685b25900246939394662b7faac125",
                null,
                UpdateLevelUpDeterminatorText.Group);
            

            var choices = new List<BlueprintFeature>();
            //var fam = library.Get<BlueprintFeatureSelection>("363cab72f77c47745bf3a8807074d183");
            var fam = library.CopyAndAdd<BlueprintFeatureSelection>("363cab72f77c47745bf3a8807074d183", "ExploitFamiliar", "365cab72f77c47745bf3a8807074d024");
            fam.DlcType = Kingmaker.Blueprints.Root.DlcType.None;
            fam.ComponentsArray = new BlueprintComponent[0];
            fam.SetDescription("Possilbe Arcanist exploit.");

            fam.IgnorePrerequisites = true;
            //fam.
            fam.PrerequisiteFeature(false);
            //Helpers.ReplaceComponent<Prer>
            //fam.PrerequisiteFeature(null);
            choices.Add(fam);
            selection.SetFeatures(choices);

            return selection;
        }








        static void createArcaneReservoir()
        {
            string abilitydescription = "An arcanist has an innate pool of magical energy that she can draw upon to fuel her arcanist exploits and enhance her spells. The arcanist’s arcane reservoir can hold a maximum amount of magical energy.\n" +
                "Points from the arcanist reservoir are used to fuel many of the arcanist’s powers. In addition, the arcanist can expend 1 point from her arcane reservoir as a free action whenever she casts an arcanist spell. If she does, she can choose to increase the caster level by 1 or increase the spell’s DC by 1.\n" +
                "You gain 3 at the minimum and extra charges based on your arcanist level.";
            var resource = Helpers.CreateAbilityResource("ArcanistReservoirResource","Reservoir charge",
                                             "One charge from the reservoir",
                                             "0dc32000b6e056d42a8ecc9921dd43c1",
                                             Helpers.NiceIcons(3),
                                             null
                                             );
            //resource.SetIncreasedByStat(3, StatType.SkillKnowledgeArcana);
            resource.SetIncreasedByLevel(3, 1,new BlueprintCharacterClass[] { arcanist });

            var surge = Helpers.Create<NewMechanics.ArcaneEmpowermentLevel>();
            var surge2 = Helpers.Create<NewMechanics.ArcaneEmpowermentDC>();
            //surge.buff = library.Get<BlueprintBuff>("df3950af5a783bd4d91ab73eb8fa0fd3"); //stagerred
            //surge.save_type = SavingThrowType.Fortitude;
            surge.rate = DurationRate.Minutes;
            surge.dice_value = Helpers.CreateContextDiceValue(DiceType.One, Common.createSimpleContextValue(1), Helpers.CreateContextValue(AbilityRankType.DamageBonus));
            surge.resource = resource;
            surge2.rate = DurationRate.Minutes;
            surge2.dice_value = Helpers.CreateContextDiceValue(DiceType.One, Common.createSimpleContextValue(1), Helpers.CreateContextValue(AbilityRankType.DamageBonus));
            surge2.resource = resource;

            //var shadow_evocation = library.Get<BlueprintAbility>("237427308e48c3341b3d532b9d3a001f");
            var MagusSpellRecall = library.Get<BlueprintAbility>("1bd76e00b6e056d42a8ecc1031dd43b4");
            var buff = Helpers.CreateBuff("ArcanistReservoirEmpowerBuffLevel",
                              "Arcane Exploit Level Increase",
                              "At 1st level The Arcanist can use its arcane pool to increase the level of a spell.",
                              "0cd76e00b2e056d32a8ecc1031dd3333",
                              MagusSpellRecall.Icon,
                              null,
                              surge,
                              Helpers.CreateContextRankConfig(baseValueType: Kingmaker.UnitLogic.Mechanics.Components.ContextRankBaseValueType.ClassLevel, classes: arcanistArray,
                                                              progression: ContextRankProgression.Custom, type: AbilityRankType.DamageBonus,
                                                              customProgression: new(int, int)[] {
                                                                            (7, -1),
                                                                            (20, 0)
                                                                })
                              );
            var buff2 = Helpers.CreateBuff("ArcanistReservoirEmpowerBuffDC",
                              "Arcane Exploit Spell DC",
                              "At 1st level The Arcanist can use its arcane pool to increase the dc of a spell.",
                              "0cd76e00b2e056d32a8ecc1031dd1313",
                              MagusSpellRecall.Icon,
                              null,
                              surge2,
                              Helpers.CreateContextRankConfig(baseValueType: Kingmaker.UnitLogic.Mechanics.Components.ContextRankBaseValueType.ClassLevel, classes: arcanistArray,
                                                              progression: ContextRankProgression.Custom, type: AbilityRankType.DamageBonus,
                                                              customProgression: new(int, int)[] {
                                                                            (7, -1),
                                                                            (20, 0)
                                                                })
                              );


            var ability = Helpers.CreateActivatableAbility("ArcanistIncreaseSpellLevelAbility",
                                                           buff.Name,
                                                           buff.Description,
                                                           "0cd76e00b2e056d32a8ecc1031dd2222",
                                                           buff.Icon,
                                                           buff,
                                                           AbilityActivationType.Immediately,
                                                           CommandType.Free,
                                                           null,
                                                           Helpers.CreateActivatableResourceLogic(resource, ResourceSpendType.Never)
                                                           );

            var ability2 = Helpers.CreateActivatableAbility("ArcanistIncreaseSpellDCAbility",
                                                           buff2.Name,
                                                           buff2.Description,
                                                           "5cd76e00b2e056d32a8ecc1031dd2121",
                                                           buff2.Icon,
                                                           buff2,
                                                           AbilityActivationType.Immediately,
                                                           CommandType.Free,
                                                           null,
                                                           Helpers.CreateActivatableResourceLogic(resource, ResourceSpendType.Never)
                                                           );

            Arcanist_reservoir = Helpers.CreateFeature("ArcanistReservoirFeature",
                                             "Arcane Exploits",
                                             abilitydescription,
                                             "0cd76e00b2e056d32a8ecc1031dd1111",
                                             ability.Icon,
                                             FeatureGroup.None,
                                             Helpers.CreateAddFact(ability),
                                             Helpers.CreateAddFact(ability2),
                                             Helpers.CreateAddAbilityResource(resource)
                                             );
        }










    }
}