// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

using RES = EldritchArcana.Properties.Resources;

namespace EldritchArcana
{
    static class WishSpells
    {
        internal static BlueprintAbility miracle;
        internal static BlueprintAbility Wishy;

        static LibraryScriptableObject library => Main.library;

        internal static void Load()
        {
            Main.SafeLoad(LoadLesserMiracle, RES.LesserMiracleSpells_info);//this is neccecairy to declaire id's so the copys work.
            Main.SafeLoad(LoadMiracle, RES.MiracleSpells_info);
            Main.SafeLoad(LoadWish, RES.WishSpells_info);
            Main.SafeLoad(LoadLimitedWish, RES.LimitedWishSpells_info);
            Main.SafeLoad(LoadWishFabricate, RES.WishFabricateSpells_info);
            Log.Write("wishSPells loaded");
        }

        static void LoadLimitedWish()
        {
            var spell = Helpers.CreateAbility("LimitedWish", RES.LimitedWishSpells_info,
                RES.LimitedWishAbilityDescription_info,
                "9e70b011f2554c3ba0fe9060dc93fc6c",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/limited_wish.png"), // geniekind
                AbilityType.Spell,
                CommandType.Standard,
                AbilityRange.Personal,
                "", "",
                Helpers.CreateSpellComponent(SpellSchool.Universalist),
                Helpers.CreateSpellDescriptor());
            spell.CanTargetSelf = true;
            spell.AvailableMetamagic = Metamagic.Quicken | Metamagic.Heighten | Metamagic.Empower | Metamagic.Extend | Metamagic.Maximize | Metamagic.Reach;

            var variants = new List<BlueprintAbility>();
            for (int level = 1; level <= 6; level++)
            {
                variants.Add(CreateWishForSpellLevel(spell, level, 7));
            }
            spell.AddComponent(spell.CreateAbilityVariants(variants));
            spell.MaterialComponent = variants[0].MaterialComponent;

            spell.AddToSpellList(Helpers.wizardSpellList, 7);
            // Scroll uses Limited Wish (5th level) so you can choose divine spells.
            Helpers.AddSpellAndScroll(spell, "f948342d6a9f2ce49b6aa5f362569d72", 4); // scroll geniekind djinni 
        }

        static void LoadWishFabricate()
        {
            var spell = Helpers.CreateAbility("Wishy", RES.WishFabricateSpells_info,
                "",
                "9e70b011f25453cba0fe9060dc93fc6c",
                Helpers.GetIcon("6f1f99b38e471fa42b1b42f7549b4210"), // geniekind
                AbilityType.Spell,
                CommandType.Standard,
                AbilityRange.Personal,
                "", "",
                Helpers.CreateSpellComponent(SpellSchool.Transmutation),
                Helpers.CreateSpellDescriptor());
            spell.CanTargetSelf = true;
            spell.AvailableMetamagic = Metamagic.Quicken | Metamagic.Heighten | Metamagic.Empower | Metamagic.Extend | Metamagic.Maximize | Metamagic.Reach;

            var variants = new List<BlueprintAbility>();
            for (int level = 1; level <= 8; level++)
            {
                variants.Add(CreateWishForSpellLevel(spell, level, 9));
            }
            variants.AddRange(CreateWishForStatBonus(spell));
            spell.AddComponent(spell.CreateAbilityVariants(variants));
            spell.MaterialComponent = variants[0].MaterialComponent;

            //spell.MaterialComponent = variants[0].MaterialComponent;
            //variants[0].MaterialComponent;
            if (Main.settings.CheatCustomTraits)
            {
                spell.AddToSpellList(Helpers.wizardSpellList, 9);
                //if opdedin for cheats will display
            }
            // Scroll uses Limited Wish (5th level) so you can choose divine spells.
            Helpers.AddSpellAndScroll(spell, "f948342d6a9f2ce49b6aa5f362569d72", 5); // scroll geniekind djinni 
        }

        static void LoadWish()
        {
            var spell = Helpers.CreateAbility("Wish", RES.WishSpells_info,
                RES.WishAbilityDescription_info,
                "508802d7c0cb452ab7473c2e83c3f535",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/wish_spell.png"),
                AbilityType.Spell,
                CommandType.Standard,
                AbilityRange.Personal,
                "", "", Helpers.CreateSpellComponent(SpellSchool.Universalist),
                Helpers.CreateSpellDescriptor());
            spell.CanTargetSelf = true;
            spell.AvailableMetamagic = Metamagic.Quicken | Metamagic.Heighten | Metamagic.Empower | Metamagic.Extend | Metamagic.Maximize | Metamagic.Reach;

            var variants = new List<BlueprintAbility>();
            // TODO: add an option to remove injuries and afflictions.
            for (int level = 1; level <= 8; level++)
            {
                variants.Add(CreateWishForSpellLevel(spell, level, 9));
            }
            //variants.AddRange(CreateWishForStatBonus(spell));

            variants.AddRange(CreateWishForStatBonus(spell, spell.AssetGuid));
            spell.AddComponent(spell.CreateAbilityVariants(variants));
            spell.MaterialComponent = variants[0].MaterialComponent;

            spell.AddToSpellList(Helpers.wizardSpellList, 9);
            // Wish Scroll uses 7th level spells to offer the most choice (divine + arcane).
            Wishy = spell;
            Helpers.AddSpellAndScroll(spell, "f948342d6a9f2ce49b6aa5f362569d72", 6); // scroll geniekind djinni icon
            // Fix Draconic and Arcane bloodlines to have Wish as their 9th level spell.
            FixBloodlineSpell(spell, "4d491cf9631f7e9429444f4aed629791", "74ab07974fa1c424bbd6fc0e56114db6"); // arcane
            FixBloodlineSpell(spell, "7bd143ead2d6c3a409aad6ee22effe34", "32d443e6a4103d84b9243822c3abec97"); // draconic
        }

        internal static void FixBloodlineSpell(BlueprintAbility spell, String bloodlineId, String addSpellId)
        {
            var addSpellFeat = library.Get<BlueprintFeature>(addSpellId);
            addSpellFeat.SetNameDescriptionIcon(spell.Name, string.Format(RES.FixBloodlineSpellDescription_info, spell.Description), spell.Icon);

            // Fix the spell, and the spell recommendations.
            var addSpell = addSpellFeat.GetComponent<AddKnownSpell>();
            var oldSpell = addSpell.Spell;
            oldSpell.RemoveRecommendNoFeatureGroup(bloodlineId);
            spell.AddRecommendNoFeature(addSpellFeat);
            addSpell.Spell = spell;
        }

        static List<BlueprintAbility> CreateWishForStatBonus(BlueprintAbility wish, string noduplicateId = null)
        {


            var variants = new List<BlueprintAbility>();
            var stats = new StatType[] {
                StatType.Strength, StatType.Dexterity, StatType.Constitution,
                StatType.Intelligence, StatType.Wisdom, StatType.Charisma,
                StatType.Speed,StatType.Initiative,StatType.SneakAttack,
                StatType.TemporaryHitPoints
            };
            var statIds = new String[] {
                "0b183a3acaf5464eaad54276413cec04",
                "447dee7c06cf4ca89363f470d7992363",
                "5422c7b6b46c47a394f294db14a788a9",
                "a55a1ccd705a42e3b939d23e0f481d76",
                "74baf6d80a844254a8014758d4ef306a",
                "8734f42ae9bb4f8e8b5eb9c9b8d9a631",
                "8734f42ae9bb4f8e9c5eb9c9b8d9a632",
                "8734f42ae9bb4f9e9c5eb9c9b8d9a633",
                "8735f42ae9bb4f3e9c5eb9c9b8d9a634",
                "8735f42ae9bb4f3e9c5ef0c9b8d9a635",
            };

            for (int i = 0; i < stats.Length; i++)
            {
                //Boolean i >
                var stat = stats[i];
                var statId = statIds[i];
                var statName = LocalizedTexts.Instance.Stats.GetText(stat);

                var feat = Helpers.CreateFeature($"{wish.name}InherentBonus{stat}", $"{wish.Name} — {statName}",
                    string.Format(RES.WishFeatureDescription_info, statName),
                    Helpers.MergeIds(statId, "e9a878036b9e4df78f85d8558058fc56", noduplicateId),
                    wish.Icon,
                    FeatureGroup.None,
                    Helpers.CreateAddStatBonus(stat, 1, ModifierDescriptor.Inherent));
                feat.Ranks = 5;
                var buff = Helpers.CreateBuff($"{feat.name}Buff", feat.Name,
                    string.Format(RES.WishBuffDescription_info, statName),
                    Helpers.MergeIds(statId, "f6f99525b8da4e33bae58a13f4db5a98", noduplicateId),
                    wish.Icon, null,
                    Helpers.Create<WishStatBonusTemporary>(w => { w.PermanentBonus = feat; w.Stat = stat; }));
                buff.Stacking = StackingType.Prolong;

                var ability = Helpers.CreateAbility($"{feat.name}Ability", feat.Name, buff.Description,
                    Helpers.MergeIds(statId, "5d50b81a2af146f69775ca350e37494d", noduplicateId),
                    wish.Icon, AbilityType.Spell, CommandType.Standard,
                    AbilityRange.Medium, wish.LocalizedDuration, wish.LocalizedSavingThrow,
                    //Game.Instance.Player.Inventory.Add(itemByGuid, itemAmount)
                    Helpers.CreateRunActions(buff.CreateApplyBuff(Helpers.CreateContextDuration(2),
                        fromSpell: true, dispellable: false)));
                ability.CanTargetSelf = true;
                ability.CanTargetFriends = true;
                ability.AvailableMetamagic = Metamagic.Quicken;
                ability.EffectOnAlly = AbilityEffectOnUnit.Helpful;
                ability.MaterialComponent.Item = diamond.Value;
                ability.MaterialComponent.Count = 5;
                if (i % 8 == 1)
                {
                    ability.MaterialComponent.Count = Main.settings?.CheatCustomTraits == true ? -50 : 5;
                }/*
                else { 
                //ability.MaterialComponent.Item = summonedBow.Value;
                }*/

                variants.Add(ability);
            }
            return variants;
        }
        static readonly Lazy<BlueprintItem> bone = new Lazy<BlueprintItem>(() => library.Get<BlueprintItem>("6a7cdeb14fc6ef44580cf639c5cdc113"));


        static void LoadMiracle()
        {
            var spell = Helpers.CreateAbility("Miracle", RES.MiracleSpells_info,
                RES.MiracleAbilityDescription_info,
                "8ce2676c93de461b91596ef7e4e04c09",
                Helpers.GetIcon("fafd77c6bfa85c04ba31fdc1c962c914"), // restoration greater
                AbilityType.Spell,
                CommandType.Standard,
                AbilityRange.Personal,
                "", "",
                Helpers.CreateSpellComponent(SpellSchool.Evocation),
                Helpers.CreateSpellDescriptor());
            spell.CanTargetSelf = true;
            spell.AvailableMetamagic = Metamagic.Quicken | Metamagic.Heighten | Metamagic.Empower | Metamagic.Extend | Metamagic.Maximize | Metamagic.Reach;

            var variants = new List<BlueprintAbility>();
            for (int level = 1; level <= 8; level++)
            {
                variants.Add(CreateWishForSpellLevel(spell, level, 9, isMiracle: true));
            }
            spell.AddComponent(spell.CreateAbilityVariants(variants));
            spell.MaterialComponent = variants[0].MaterialComponent;

            // TODO: variant for mass Resurrection (cost 25,000)
            // TODO: variant for protection from natural disasters (cost 25,000)
            // Perhaps it should prevent some of the bad kingdom effects that can happen,
            // or other quest related effects (e.g. become immune to Kingdom penalties for a time)?
            spell.AddToSpellList(Helpers.clericSpellList, 9);
            miracle = spell;

            // Miracle Scroll uses 7th level spells to offer the most choice (divine + arcane).
            Helpers.AddSpellAndScroll(spell, "d441dfae9c6b21e47ae24eb13d8b4c4b", 6); // restoration greater.
        }


        static void LoadLesserMiracle()
        {
            var spell = Helpers.CreateAbility("LesserMiracle", RES.LesserMiracleAbilityName_info,
                RES.LesserMiracleAbilityDescription_info,
                "2ce3374c93de461b91596ef7e4e04c14",
                Helpers.GetIcon("fafd77c6bfa85c04ba31fdc1c962c914"), // restoration greater
                AbilityType.Spell,
                CommandType.Standard,
                AbilityRange.Personal,
                "", "",
                Helpers.CreateSpellComponent(SpellSchool.Evocation),
                Helpers.CreateSpellDescriptor());
            spell.CanTargetSelf = true;
            spell.AvailableMetamagic = Metamagic.Quicken | Metamagic.Heighten | Metamagic.Empower | Metamagic.Extend | Metamagic.Maximize | Metamagic.Reach;

            var variants = new List<BlueprintAbility>();
            for (int level = 1; level <= 7; level++)
            {
                variants.Add(CreateWishForSpellLevel(spell, level, 7, isMiracle: true));
            }
            spell.AddComponent(spell.CreateAbilityVariants(variants));
            spell.MaterialComponent = variants[0].MaterialComponent;

            variants.AddRange(CreateWishForStatBonus(spell, spell.AssetGuid));
            // TODO: variant for mass Resurrection (cost 25,000)
            // TODO: variant for protection from natural disasters (cost 25,000)
            // Perhaps it should prevent some of the bad kingdom effects that can happen,
            // or other quest related effects (e.g. become immune to Kingdom penalties for a time)?
            spell.AddToSpellList(Helpers.clericSpellList, 7);
            //miracle = spell;

            // Miracle Scroll uses 7th level spells to offer the most choice (divine + arcane).
            Helpers.AddSpellAndScroll(spell, "d441dfae9c6b21e47ae24eb13d8b4c4b", 5); // restoration greater.
        }

        static BlueprintAbility CreateWishForSpellLevel(BlueprintAbility wishSpell, int level, int wishLevel, bool isMiracle = false)
        {
            //make shure the ids are different becouse doublicate ids would overwrite each outher
            string MiracleID = isMiracle ? "6db719c91bcc4f31b997904ef1f873c9" : "7db719c91bcc4f31b997904ef1f873c8";

            var wishText = isMiracle ? RES.MiracleSpells_info : RES.WishSpells_info;
            var spell = Helpers.CreateAbility($"{wishSpell.name}{level}",
                string.Format(RES.WishLevelSpellName_info, wishSpell.Name, level),
                string.Format(RES.WishSpellDescription_info, wishText, wishSpell.Description),
                Helpers.MergeIds(wishSpell.AssetGuid, MiracleID, FavoredClassBonus.spellLevelGuids[level - 1]),
                wishSpell.Icon, wishSpell.Type, wishSpell.ActionType, wishSpell.Range,
                wishSpell.LocalizedDuration, wishSpell.LocalizedSavingThrow);
            spell.AvailableMetamagic = wishSpell.AvailableMetamagic;
            spell.CanTargetSelf = true;
            if (!isMiracle)
            {
                spell.MaterialComponent.Item = wishLevel < 9 ? diamondDust.Value : diamond.Value;
                spell.MaterialComponent.Count = wishLevel < 9 ? 30 : 5;
            }

            // To keep wish spells from overwhelming the game's UI, they're done as a series of nested selections:
            // - wish spell variant: choose spell level, or other effect
            // - ability menu: group spells by school
            //   (unless the spell has variants, in which case, put it at the top level)
            //
            // That means we only get a maximum of spell schools + variant spells, which keeps the size down.
            //
            // Also, these spells tend to be used for powerful effects, so it's likely they'll select one of the
            // highest spell levels first. That eliminates all of the low-level spell noise (e.g. "cure light wounds").
            //FavoredClassBonus.
            var buff = Helpers.CreateBuff($"{wishSpell.name}{level}Buff", wishSpell.Name, wishSpell.Description,
                    Helpers.MergeIds(wishSpell.AssetGuid, "5a203cb47d1942058a602da860e435d7", FavoredClassBonus.spellLevelGuids[level - 1]),
                    wishSpell.Icon,
                    null);
            buff.SetComponents(CreateWishSpellChoices(spell, buff, level, wishLevel, isMiracle));
            buff.Stacking = StackingType.Replace;
            buff.Frequency = DurationRate.Rounds;

            // Duration: 2 rounds. Should be enough time to make a wish (as a free action).
            var applyBuff = Helpers.CreateApplyBuff(buff, Helpers.CreateContextDuration(2),
                fromSpell: true, dispellable: false, toCaster: true);

            spell.SetComponents(Helpers.CreateRunActions(applyBuff), wishSpell.GetComponent<SpellDescriptorComponent>(), wishSpell.GetComponent<SpellComponent>());
            return spell;
        }

        static BlueprintComponent[] CreateWishSpellChoices(BlueprintAbility spellForLevel, BlueprintBuff buffForLevel, int level, int wishLevel, bool isMiracle = false)
        {

            //string MiracleID = "4a203cb47d1942059a602da860e435d7";
            //if (isMiracle == true)
            //{
            //    MiracleID = "7a203cb47d1942059a602da860e435d7";
            //}
            // 1. Get all spells for this spell level (minimum level across all classes that can cast it).
            var wishSpellsAtLevel = GetWishSpellAbilities()[level];
            // 2. Remove material costs, if applicable.
            int ignoreMaterials = isMiracle ? miracleIgnoreMaterialCost : (wishLevel < 9 ? limitedWishIgnoreMaterialCost : wishIgnoreMaterialCost);
            var wishSpellsAdjustMaterials = wishSpellsAtLevel.Select(s =>
            {
                var cost = s.MaterialComponent.GetCost();
                if (cost <= miracleIgnoreMaterialCost || cost > ignoreMaterials) return s;
                return noMaterialWishSpells[s];
            });
            // 3. Group spell choices by school. This lets us adjust based on the opposition school, if needed.
            var spellsBySchool = wishSpellsAdjustMaterials.GroupBy(s => s.School);

            var oppositionSchools = library.Get<BlueprintFeatureSelection>("6c29030e9fea36949877c43a6f94ff31");
            var components = new List<BlueprintComponent> {
                wishResource.CreateAddAbilityResource()
            };
            var endComponents = new List<BlueprintComponent>();
            foreach (var spells in spellsBySchool)
            {
                var school = spells.Key;
                var oppositionFeat = oppositionSchools.AllFeatures.FirstOrDefault(f => f.GetComponent<AddOppositionSchool>()?.School == school);
                if (oppositionFeat == null)
                {
                    Log.Write($"Invalid spell school {school}: {spells.StringJoin(s => s.name, ", ")}");
                    continue;
                }
                var specialistFeat = oppositionFeat.GetComponent<PrerequisiteNoFeature>().Feature;

                // Spell level adjustment:
                // -1 for non-wizard spell (wish) or non-cleric spell (miracle).
                // -1 for opposed school (wish).
                // For opposition schools, we'll check for the opposition school feature.
                //
                // Spells are grouped by minimum level they can be cast, by any class.
                // That allows Limited Wish to cast spells that are too high on the Wizard list,
                // but lower level in another class.
                //
                // For example: Joyful Rapture is a 5th level Bard spell, but a 7th level Wizard spell.
                // Limited Wish is allowed to cast it (it's a non-wizard spell 5th level or less).
                var classSpellList = isMiracle ? Helpers.clericSpellList : Helpers.wizardSpellList;
                var schoolIcon = specialistFeat.Icon;

                IEnumerable<BlueprintAbility> validSpells = spells;
                if (level == wishLevel - 1)
                {
                    // If this is the highest spell level that wish/miracle can cast,
                    // then the spell is required to be on our spell list, and not an opposition school.
                    validSpells = spells.Where(s => s.GetComponents<SpellListComponent>()
                        .Any(c => c.SpellList == classSpellList && c.SpellLevel < wishLevel)).ToList();
                    foreach (var s in validSpells)
                    {
                        s.AddComponent(WishSpellOppositionSchoolCheck.Create(buffForLevel, oppositionFeat));
                    }
                }
                else if (!isMiracle && level == wishLevel - 2)
                {
                    // For spells not from our spell list, we need to add an opposition check.
                    // Since only some abilities (in a group) will be disabled by this, it's easier
                    // to add a check to the individual ability variants.
                    var otherSpells = spells.Where(s => !s.GetComponents<SpellListComponent>()
                        .Any(c => c.SpellList == classSpellList && c.SpellLevel < wishLevel - 1));
                    foreach (var s in otherSpells)
                    {
                        s.AddComponent(WishSpellOppositionSchoolCheck.Create(buffForLevel, oppositionFeat));
                    }
                }
                //var submerged = Helpers.MergeIds(MiracleID, MiracleID);
                var schoolName = LocalizedTexts.Instance.SpellSchoolNames.GetText(school);
                BlueprintAbility[] variantSpells;
                var nonVariantSpells = validSpells.Where(s => !s.HasVariants).ToList();
                if (nonVariantSpells.Count > 1)
                {
                    //submerged = Helpers.MergeIds(, spellForLevel.AssetGuid);
                    variantSpells = validSpells.Where(s => s.HasVariants).ToArray();
                    // Group all of the non-variant spells together.
                    var schoolVariant = Helpers.CreateAbility($"{spellForLevel.name}{school}Spells",
                        string.Format(RES.WishSpellsSpellName_info, schoolName), spellForLevel.Description,
                        Helpers.MergeIds(spellForLevel.AssetGuid, spellSchoolGuids[(int)school]),
                        schoolIcon,
                        AbilityType.SpellLike,
                        CommandType.Free,
                        AbilityRange.Personal,
                        "", "");
                    schoolVariant.SetComponents(
                        wishResource.CreateResourceLogic(),
                        schoolVariant.CreateAbilityVariants(nonVariantSpells));
                    components.Add(WishAddSpells.Create($"{school}{level}", schoolVariant));
                }
                else
                {
                    variantSpells = validSpells.ToArray();
                }

                if (variantSpells.Length > 0)
                {
                    endComponents.Add(WishAddSpells.Create($"Variants{school}{level}", variantSpells));
                }
            }
            components.AddRange(endComponents);
            return components.ToArray();
        }

        static List<List<BlueprintAbility>> GetWishSpellAbilities()
        {
            // Transforms all spells into spell-like abilities, suitable for Wish/Miracle.
            if (wishResource != null) return wishSpells;

            var resource = Helpers.CreateAbilityResource($"WishResource", "", "", "fc39d0a85c984c71b013dfe601254986", null);
            resource.SetFixedResource(1);
            wishResource = resource;

            for (int i = 0; i < wishSpells.Capacity; i++)
            {
                wishSpells.Add(new List<BlueprintAbility>());
            }
            foreach (var spell in Helpers.allSpells)
            {
                if (spell.Parent != null) continue;

                var spellLists = spell.GetComponents<SpellListComponent>();
                if (spellLists.FirstOrDefault() == null) continue;

                var level = spellLists.Min(l => l.SpellLevel);
                if (level > 0 && level < wishSpells.Count)
                {
                    var ability = SpellToWishAbility(spell, "361fc62d90dc4e75b4c7858fcc0072b0");
                    wishSpells[level].Add(ability);
                    var cost = spell.MaterialComponent.GetCost();
                    // If the material cost is between the range of what Miracle and Wish allow for free,
                    // generate a version that doesn't cost anything.
                    if (cost > miracleIgnoreMaterialCost && cost <= wishIgnoreMaterialCost)
                    {
                        var noMaterialAbility = SpellToWishAbility(spell, "c7c0d7772ad04541a39e334f9025c87d", wishIgnoreMaterialCost);
                        noMaterialWishSpells.Add(ability, noMaterialAbility);
                    }
                }
            }
            // For backwards compatibility, create these spells but don't add them to the selections.
            foreach (var spell in Helpers.spellsWithResources)
            {
                if (spell.Parent != null) continue;

                var spellLists = spell.GetComponents<SpellListComponent>();
                if (spellLists.FirstOrDefault() == null) continue;

                SpellToWishAbility(spell, "361fc62d90dc4e75b4c7858fcc0072b0");
            }
            return wishSpells;
        }

        static BlueprintAbility SpellToWishAbility(BlueprintAbility spell, String idPart, int ignoreMaterialCost = miracleIgnoreMaterialCost)
        {
            var ability = library.CopyAndAdd(spell, $"Wish{spell.name}", spell.AssetGuid, idPart);
            ability.Type = AbilityType.SpellLike;
            ability.ActionType = CommandType.Free;
            if (ability.MaterialComponent.GetCost() <= ignoreMaterialCost)
            {
                ability.MaterialComponent = new BlueprintAbility.MaterialComponentData();
            }
            var components = ability.ComponentsArray.ToList();
            components.Add(wishResource.CreateResourceLogic());
            if (spell.HasVariants)
            {
                var c = ability.GetComponent<AbilityVariants>();
                components.Remove(c);
                var variants = c.Variants.Select(v => SpellToWishAbility(v, idPart, ignoreMaterialCost));
                components.Add(ability.CreateAbilityVariants(variants));
            }
            ability.SetComponents(components);
            return ability;
        }


        const int miracleIgnoreMaterialCost = 100;
        const int limitedWishIgnoreMaterialCost = 1000;
        const int wishIgnoreMaterialCost = 10000;

        static readonly Lazy<BlueprintItem> summonedBow = new Lazy<BlueprintItem>(
            () => library.Get<BlueprintItem>("2fe00e2c0591ecd4b9abee963373c9a7"));
        static readonly Lazy<BlueprintItem> diamond = new Lazy<BlueprintItem>(
            () => library.Get<BlueprintItem>("6a7cdeb14fc6ef44580cf639c5cdc113"));
        static readonly Lazy<BlueprintItem> diamondDust = new Lazy<BlueprintItem>(
            () => library.Get<BlueprintItem>("92752bbbf04dfa1439af186f48aee0e9"));

        static BlueprintAbilityResource wishResource;
        static readonly List<List<BlueprintAbility>> wishSpells = new List<List<BlueprintAbility>>(9);
        static readonly Dictionary<BlueprintAbility, BlueprintAbility> noMaterialWishSpells = new Dictionary<BlueprintAbility, BlueprintAbility>();

        static readonly String[] spellSchoolGuids = new String[] {
            "35525798a7f8444c953ecfedeb378928",
            "290d1187d3bb40d0b7e0960fc55a475a",
            "44be5bc066854ef590c846246b3a8ce9",
            "3bf31c44bf214dcdb1613d6d74133184",
            "6a744fe89de240cf8620be5e17c39402",
            "032c0c2e8acb4697b901e7c398e5b9f2",
            "75ba5e3ba72e458a83b184e8a5a5a8f4",
            "957fe6f09ccd4904b8c203d6b539c1dc",
            "f037512db3a340daa768433382ad84f9",
            "1f1b7e2a605140dc88b4698b07b6daaa",
            "645754ab72384339a7ec4b059aead29e",
            "05fbf55131e74265970e8ec82277dae2",
            "92e3106d7ed548c5a1761478126e189e",
            "3a3903a3aa13409685c9d64628980667",
            "2271f146297a44dd83e1033eff3920bc",
            "dc80e885be8a47108f472132d5e6a0b1",
            "02556febbb8d49499f4d324da4174765",
            "d215be637574434cb5d05f8d413e202e",
            "604f16935b4342069486378501589857",
            "7a5db121b6a345a993e392b2a092e07c",
        };

    }

    [AllowedOn(typeof(BlueprintAbility))]
    public class WishSpellOppositionSchoolCheck : BlueprintComponent, IAbilityCasterChecker
    {
        public BlueprintFeature OppositionSchool;
        public BlueprintBuff WishBuff;

        public static WishSpellOppositionSchoolCheck Create(BlueprintBuff wish, BlueprintFeature opposition)
        {
            var w = Helpers.Create<WishSpellOppositionSchoolCheck>();
            w.OppositionSchool = opposition;
            w.WishBuff = wish;
            return w;
        }

        public bool CorrectCaster(UnitEntityData caster)
        {
            var unit = caster.Descriptor;
            return !unit.HasFact(WishBuff) || !unit.HasFact(OppositionSchool);
        }

        public string GetReason() => RES.WishReason_error;
    }

    public class WishStatBonusTemporary : BuffLogic, IInitiatorRulebookHandler<RuleApplyBuff>
    {
        public BlueprintFeature PermanentBonus;
        public StatType Stat;
        public int MaxStacking = 5;

        public override void OnTurnOn()
        {
            int ranks = GetRanks();
            Log.Write($"Adding temporary inherent bonus +{ranks} to {Stat}");
            StoreModifier(Owner.Stats.GetStat(Stat).AddModifier(ranks, this, ModifierDescriptor.Inherent));
        }

        public override void OnTurnOff()
        {
            int ranks = GetRanks();
            Log.Write($"Adding permanent inherent bonus +{ranks} to {Stat}");
            var fact = Owner.AddFact<Feature>(PermanentBonus);
            for (int i = 1; i < ranks; i++)
            {
                fact.AddRank();
            }
            Context[AbilitySharedValue.StatBonus] = 0;
        }

        int GetRanks() => Math.Min(Context[AbilitySharedValue.StatBonus] + 1, MaxStacking);

        public void OnEventAboutToTrigger(RuleApplyBuff evt)
        {
            if (evt.Blueprint == Buff.Blueprint)
            {
                Context[AbilitySharedValue.StatBonus]++;
            }
        }

        public void OnEventDidTrigger(RuleApplyBuff evt)
        {
            OnTurnOn();
        }
    }

    [AllowedOn(typeof(BlueprintBuff))]
    public class WishAddSpells : BuffLogic, IInitiatorRulebookHandler<RuleCastSpell>
    {
        public BlueprintAbility[] Spells;

        public static WishAddSpells Create(String name, params BlueprintAbility[] spells)
        {
            var a = Helpers.Create<WishAddSpells>();
            a.name = $"WishAddSpells${name}";
            a.Spells = spells;
            return a;
        }

        [JsonProperty]
        private List<Ability> abilities = new List<Ability>();

        public override void OnFactActivate()
        {
            try
            {
                var context = Buff.Context.SourceAbilityContext;
                foreach (var spell in Spells)
                {
                    // Ensure the wish spells use the appropriate spell params and level.
                    // This lets a spell-like Wish propagate its params:
                    //
                    //   Wish spell -> Wish buff -> Wish activatable abilities -> wished-for spell
                    //
                    // (This is similar to how UnitPartTouch.Init implements sticky touch spells.)
                    var ability = Owner.AddFact<Ability>(spell);
                    ability.OverrideParams = context.Params;
                    ability.Data.OverrideSpellLevel = context.Params.SpellLevel;
                    ability.Data.SpellSource = context.Params.SpellSource;
                    abilities.Add(ability);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public override void OnFactDeactivate()
        {
            abilities.ForEach(Owner.RemoveFact);
            abilities.Clear();
        }

        public void OnEventAboutToTrigger(RuleCastSpell evt) { }

        public void OnEventDidTrigger(RuleCastSpell evt)
        {
            if (IsWishSpell(evt.Spell))
            {
                Buff.RemoveAfterDelay();
            }
        }

        bool IsWishSpell(AbilityData spell) => spell?.Fact != null && abilities.Contains(spell.Fact);
    }
}
