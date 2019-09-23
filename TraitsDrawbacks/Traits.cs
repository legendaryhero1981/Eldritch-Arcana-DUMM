// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using Harmony12;

using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Controllers.Combat;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UI.Common;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Class.LevelUp.Actions;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace EldritchArcana
{

    static class Traits
    {
        public static LibraryScriptableObject library => Main.library;
        static string[] guids = new string[] { "2e064038224e4c2cbbdcfd07af090a9e",
                "3e064038224e4c2cbbdcfd07af090a9f",                "4e064038224e4c2cbbdcfd07af090a9g",
                "5e064038224e4c2cbbdcfd07af090a9h",                "6e064038224e4c2cbbdcfd07af090a9i",
                "7e064038224e4c2cbbdcfd07af090a9j",                "8e064038224e4c2cbbdcfd07af090a9k",
                "9e064038224e4c2cbbdcfd07af090a9l",                "0f064038224e4c2cbbdcfd07af090a9m",
                "1f064038224e4c2cbbdcfd07af090a9n",                "2f064038224e4c2cbbdcfd07af090a9o",
                "3f064038224e4c2cbbdcfd07af090a9p",                "4f064038224e4c2cbbdcfd07af090a9p",
                "5f064038224e4c2cbbdcfd07af090a9p",                "6f064038224e4c2cbbdcfd07af090a9p",
                "7f064038224e4c2cbbdcfd07af090a9p",                "8f064038224e4c2cbbdcfd07af090a9p",
                "9f064038224e4c2cbbdcfd07af090a9p",                "0c064038224e4c2cbbdcfd07af090a9p",
                "1c064038224e4c2cbbdcfd07af090a9p",                "1c064038224e4c2cbbdcfd07af090a9p",
                };

        internal static void Load()
        {
            // Create the trait selection (https://www.d20pfsrd.com/traits/).
            // TODO: ideally we'd use FeatureGroup.Trait, but it's not recognized by the game code.
            var featureGroup = FeatureGroup.Feat;
            var traitSelection = Helpers.CreateFeatureSelection("TraitSelection1", "Traits",
                "Character traits are abilities that are not tied to your character’s race or class. They can enhance your character’s skills, racial abilities, class abilities, or other statistics, enabling you to further customize them. At its core, a character trait is approximately equal in power to half a feat, so two character traits are roughly equivalent to a bonus feat. Yet a character trait isn’t just another kind of power you can add on to your character—it’s a way to quantify (and encourage) building a character background that fits into your campaign world. Think of character traits as “story seeds” for your background; after you pick your two traits, you’ll have a point of inspiration from which to build your character’s personality and history. Alternatively, if you’ve already got a background in your head or written down for your character, you can view picking their traits as a way to quantify that background, just as picking race and class and ability scores quantifies their other strengths and weaknesses.",
                "f243a1b828714591b5fa0635b0cefb5b", null, featureGroup);
            var traitSelection2 = Helpers.CreateFeatureSelection("TraitSelection2", "Traits",
                traitSelection.Description, "d3a6541d2d384b1390d8ea26bb02b8cd", null, featureGroup);
            var traitSelection3 = Helpers.CreateFeatureSelection("TraitSelection3", "Traits",
                traitSelection.Description, "d4b7541d2d384b1390d8ea26bb02b8rt", null, featureGroup);

            var DrawbackSelection1 = Helpers.CreateFeatureSelection("DrawbackSelection3", "Drawbacks",
                "Drawbacks are traits in reverse. Instead of granting you a boon, they grant you a negative effect, typically in particular circumstances. " +
                "If you choose to take a drawback, you can take a third trait that you have access to. " +
                "You are not required to take a drawback.(disable in eldrich arcana menu and restart game)", "4db7541d2d384b1390d8ea26bb02b9tr", null, featureGroup);

            var choices = new List<BlueprintFeatureSelection>();
            var Drawbackchoices = new List<BlueprintFeatureSelection>();

            var Skip = Helpers.CreateFeatureSelection("NoChoice", "(Skip)",
                "skip this selection.",
                "afb4225be98a4b3e9717883f22068c28", null, FeatureGroup.None);
            choices.Add(Skip);
            Drawbackchoices.Add(Skip);
            var DrawbackEmotion = EmotionDrawbacks.CreateEmotionDrawbacks();
            var DrawbackPhysique = PhysiqueDrawbacks.CreatePhysiqueDrawbacks();
            Drawbackchoices.Add(DrawbackEmotion);
            Drawbackchoices.Add(DrawbackPhysique);
            choices.Add(CombatTraits.CreateCombatTraits());//,choices,"combattraits");
            choices.Add(FaithTraits.CreateFaithTraits());
            choices.Add(MagicTraits.CreateMagicTraits());
            BlueprintFeatureSelection adopted;
            choices.Add(SocialTraits.CreateSocialTraits(out adopted));
            choices.Add(EquipmentTraits.CreateEquipmentTraits());
            choices.Add(RaceTraits.CreateRaceTraits(adopted));
            choices.Add(CampaignTraits.CreateCampaignTraits());
            choices.Add(RegionalTraits.CreateRegionalTraits());
            // if main settings cheatcustomtrait = false it won't be added just be initialized
            var x = CreateCustomTraits(Main.settings.CheatCustomTraits);
            if(Main.settings.CheatCustomTraits) choices.Add(x);


            traitSelection.SetFeatures(choices);
            

            DrawbackSelection1.SetFeatures(Drawbackchoices);
            traitSelection2.SetFeatures(traitSelection.Features);
            traitSelection3.SetFeatures(traitSelection.Features);
            ApplyClassMechanics_Apply_Patch.onChargenApply.Add((state, unit) =>
            {
                traitSelection.AddSelection(state, unit, 1);
                traitSelection2.AddSelection(state, unit, 1);
                DrawbackSelection1.AddSelection(state, unit, 1);
            });
            //this only lets the player select a second feat if drawbackemotion is chosen
            SelectFeature_Apply_Patch.onApplyFeature.Add(DrawbackEmotion, (state, unit) =>
            {
                traitSelection3.AddSelection(state, unit, 1);
            });


            // Create the "Additional Traits" feat.
            var additionalTraits = Helpers.CreateFeature("AdditionalTraitsProgression",
                "Additional Traits",
                "You have more traits than normal.\nBenefit: You gain two character traits of your choice. These traits must be chosen from different lists, and cannot be chosen from lists from which you have already selected a character trait. You must meet any additional qualifications for the character traits you choose — this feat cannot enable you to select a dwarf character trait if you are an elf, for example.",
                "02dbb324cc334412a55e6d8f9fe87009",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/additional_traits.png"),//Helpers.GetIcon("0d3651b2cb0d89448b112e23214e744e"), // Extra Performance
                FeatureGroup.Feat);

            var additionalTrait1 = Helpers.CreateFeatureSelection("AdditionalTraitSelection1", "Traits",
                traitSelection.Description,
                "a85fbbe3c9184137a31a12f4b0b7904a", null, FeatureGroup.Feat);
            var additionalTrait2 = Helpers.CreateFeatureSelection("AdditionalTraitSelection2", "Traits",
                traitSelection.Description,
                "0fdd6f51c19c44938b9d64b147cf32f8", null, FeatureGroup.Feat);
            var additionalTrait3 = Helpers.CreateFeatureSelection("AdditionalTraitSelection3", "Traits",
                traitSelection.Description,
                "1cdd6f51c19c44938b9d64b147cf32d2", null, FeatureGroup.Feat);
            var additionalDrawback1 = Helpers.CreateFeatureSelection("AdditionalDrawbackSelection1", "Drawbacks",
                DrawbackSelection1.Description,
                "1ddd6f51c19c44938b9d64b147cc22c9", null, FeatureGroup.Feat);
            additionalTrait1.SetFeatures(traitSelection.Features);
            additionalTrait2.SetFeatures(traitSelection.Features);
            additionalTrait3.SetFeatures(traitSelection.Features);
            additionalDrawback1.SetFeatures(DrawbackSelection1.Features);

            SelectFeature_Apply_Patch.onApplyFeature.Add(additionalTraits, (state, unit) =>
            {
                additionalTrait1.AddSelection(state, unit, 1);
                additionalTrait2.AddSelection(state, unit, 1);
                additionalDrawback1.AddSelection(state, unit, 1);
                //HereHere
            });

            SelectFeature_Apply_Patch.onApplyFeature.Add(DrawbackPhysique, (state, unit) =>
            {
                additionalTrait3.AddSelection(state, unit, 1);
            });

            library.AddFeats(additionalTraits);
        }
       
       
       
        public static List<BlueprintAbility> CollectTieflingAbilities(BlueprintFeatureSelection selection)
        {
            var result = new List<BlueprintAbility>();
            foreach (var heritage in selection.AllFeatures)
            {
                foreach (var addFact in heritage.GetComponents<AddFacts>())
                {
                    result.AddRange(addFact.Facts.OfType<BlueprintAbility>());
                }
            }
            return result;
        }
       
        static BlueprintFeatureSelection CreateCustomTraits(Boolean optedin)
        {
            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var customTraits = Helpers.CreateFeatureSelection("CustomTrait", "Custom Trait{cheat}",
                "These traits are a little overpowered and are here just for fun, if you want to play fair don't pick em.",
                "9e41e60c929e46bc84ded046148d08ec", null, FeatureGroup.None);
            //noFeature.Feature = customTraits;
            var choices = new List<BlueprintFeature>();


            var metamagicMaster = Helpers.CreateFeatureSelection("MetamagicMasterTrait", "Metamagic expert([cheat])",
                "Your ability to alter your spell of choice is greater than expected.\nBenefit: Select one spell of 3rd level or below; when you use the chosen spell with a metamagic feat, it uses up three spell slots one level lower than it normally would.\nstarting level is still minimun",
                "00844f940e434033ab826e5ff5929011",
                Helpers.GetIcon("ee7dc126939e4d9438357fbd5980d459"), // Spell Penetration
                FeatureGroup.None);
            FillSpellSelection(metamagicMaster, 1, 3, Helpers.Create<ReduceMetamagicCostForSpell>(r => { r.Reduction = 3; r.MaxSpellLevel = 3; }));
            choices.Add(metamagicMaster);


            choices.Add(Helpers.CreateFeature("MysticTrait", "Mystic Prophecy",
                "no one knows where You were born. its a Mistery you where given a the divine blessing to complete a prophecy\nBenefit: At level 10 your prophecy will come true +2 wis,int +10 arcana",
                "e50acadda65b4028884dd4a74f14e228",
                Helpers.NiceIcons(19), // Iron Will
                FeatureGroup.None,
                Helpers.CreateAddStatBonusOnLevel(StatType.SkillKnowledgeArcana, 10, ModifierDescriptor.Trait, 10),
                Helpers.CreateAddStatBonusOnLevel(StatType.SkillKnowledgeWorld, 10, ModifierDescriptor.Trait, 10),
                Helpers.CreateAddStatBonusOnLevel(StatType.Intelligence, 2, ModifierDescriptor.Trait, 10),
                Helpers.CreateAddStatBonusOnLevel(StatType.Wisdom, 2, ModifierDescriptor.Trait, 10),
                Helpers.CreateAddStatBonusOnLevel(StatType.Charisma, 2, ModifierDescriptor.Trait, 10), PrerequisiteCharacterLevelExact.Create(1)
                ));


            var magicalStrongLineage = Helpers.CreateFeatureSelection("MagicalStrongLineageTrait", "Magical Strong Lineage({cheat})",
                "One of your parents was a gifted spellcaster who not only used metamagic often, but also developed many magical items and perhaps even a new spell or two—and you have inherited a fragment of this greatness.\nBenefit: Pick one spell when you choose this trait. When you apply metamagic feats to this spell that add at least 1 level to the spell, treat its actual level as 1 lower up to 2 for determining the spell’s final adjusted level.",
                "1785787fb62a4c529104ba53d0de99ae",
                Helpers.GetIcon("ee7dc126939e4d9438357fbd5980d459"), // Spell Penetration
                FeatureGroup.None);
            FillSpellSelection(magicalStrongLineage, 1, 9, Helpers.Create<ReduceMetamagicCostForSpell>(r => r.Reduction = 2));
            choices.Add(magicalStrongLineage);



            choices.Add(Helpers.CreateFeature("CustomMagicalKnackTrait", "Magical flat Knack",
                "You were raised, either wholly or in part, by a magical creature, either after it found you abandoned in the woods or because your parents often left you in the care of a magical minion. This constant exposure to magic has made its mysteries easy for you to understand, even when you turn your mind to other devotions and tasks.\nBenefit: Pick a class when you gain this trait—your caster level in that class gains a +2flat level added.",
                "8fd15d5aa003498ba7f976530d21e433",
                Helpers.GetIcon("16fa59cc9a72a6043b566b49184f53fe"), // Spell Focus
                FeatureGroup.None,
                //Helpers.Create<IncreaseCasterLevelUpToCharacterLevel>()
                Helpers.Create<IncreaseCasterLevelCharacterLevel>()
                ));

            choices.Add(Helpers.CreateFeature("BloodHavocTrait", "Blood havoc Damage Bonus",
                            "Whenever you cast a bloodrager or sorcerer spell that deals damage, add 1 point of damage per die rolled.",
                            "2dd15d5aa003498ba7f945530d22e444",
                            Helpers.NiceIcons(28), // M
                            FeatureGroup.None,
                            //Helpers.Create<IncreaseCasterLevelUpToCharacterLevel>()
                            Helpers.Create<OrcBloodlineArcana>()
                            ));


            var dwarfy = new BlueprintComponent[64];
            for (int i = 1; i < 65; i++)
            {
                dwarfy[i - 1] = Helpers.CreateAddStatBonusOnLevel(StatType.HitPoints, i * 2, ModifierDescriptor.Trait, i);
            };
            var dwarfReq = Helpers.PrerequisiteFeature(Helpers.dwarf);
            var bulkybattleborn = Helpers.CreateFeature("BulkyAfTrait", "Bulky Battleborn (Dwarf)",
                "Your greatest joy is being in the thick of battle and taking hits for the team,\nNote: this trait is not realy a cheat but it's homebrew so that's why its here. \nBenefit:you gain 2 extra hitpoints per level",
                "a987f5e69db44cdd99983985e37a6c3b",
                Helpers.GetIcon("121811173a614534e8720d7550aae253"), // Weapon Specialization
                FeatureGroup.None,
                dwarfReq);
            bulkybattleborn.AddComponents(dwarfy);
            choices.Add(bulkybattleborn);


            if (optedin) customTraits.SetFeatures(choices);
            return customTraits;
        }

        // Very large spell selections momentarily hang the UI, so we split the spells by level.
        // It also makes it easier to find the spell you're looking for.
        // (There's some `O(N^2)` at least, possibly a higher polynomial in the game code?)

        internal static void FillSpellSelection(BlueprintFeatureSelection selection, int minLevel, int maxLevel, params BlueprintComponent[] components)
        {
            FillSpellSelection(selection, minLevel, maxLevel, null, (_) => components);
        }

        internal static void FillSpellSelection(BlueprintFeatureSelection selection, int minLevel, int maxLevel, BlueprintSpellList spellList, Func<int, BlueprintComponent[]> createComponents, BlueprintCharacterClass learnSpellClass = null)
        {
            var choices = new List<BlueprintFeature>();
            for (int level = minLevel; level <= maxLevel; level++)
            {
                var spellChoice = Helpers.CreateParamSelection<SelectAnySpellAtLevel>(
                    $"{selection.name}Level{level}",
                    $"{selection.Name} (Spell Level {level})",
                    selection.Description,
                    Helpers.MergeIds(selection.AssetGuid, FavoredClassBonus.spellLevelGuids[level - 1]),
                    null,
                    FeatureGroup.None,
                    createComponents(level));
                spellChoice.SpellList = spellList;
                spellChoice.SpellLevel = level;
                spellChoice.SpellcasterClass = learnSpellClass;
                spellChoice.CheckNotKnown = learnSpellClass != null;
                choices.Add(spellChoice);
            }
            choices.Add(UndoSelection.Feature.Value);
            selection.SetFeatures(choices);
        }



        internal static List<BlueprintFeature> FillTripleSpellSelection(BlueprintFeatureSelection selection, int minLevel, int maxLevel, params BlueprintComponent[] components)
        {
            return FillTripleSpellSelection(selection, minLevel, maxLevel, null, (_) => components);
        }

        internal static List<BlueprintFeature> FillTripleSpellSelection(BlueprintFeatureSelection selection, int minLevel, int maxLevel, BlueprintSpellList spellList, Func<int, BlueprintComponent[]> createComponents, BlueprintCharacterClass learnSpellClass = null)
        {
            var choices = new List<BlueprintFeature>();
            
            //var choicelist = new List<BlueprintFeature>();
            string[] GuidList = new string[]
            {
                null,
                "3921FC7C8617472CAB3F86835D95FE62",
                "E8828D65304F4AE5AB8094F9893A0CCC",
                //"C77D3987222B4609A609A117BCF2E28D"
            };
            //foreach(string thirdGuid in GuidList) { 
            for (int level = minLevel; level <= maxLevel; level++)
            {
                var spellArrayAll = Helpers.Create<SelectAnySpellAtLevel>();
                
                var spellChoice = Helpers.CreateParamSelection<SelectAnySpellAtLevel>(
                    $"{selection.name}Level{level}",
                    $"{selection.Name} (Spell Level {level})",
                    selection.Description,
                    Helpers.MergeIds(selection.AssetGuid, FavoredClassBonus.spellLevelGuids[level - 1],GuidList[2]),
                    null,
                    FeatureGroup.None,
                    createComponents(level));
                spellChoice.SpellList = spellList;
                spellChoice.SpellLevel = level;
                spellChoice.SpellcasterClass = learnSpellClass;
                spellChoice.CheckNotKnown = learnSpellClass != null;
                choices.Add(spellChoice);

                
            }
            choices.Add(UndoSelection.Feature.Value);
            return choices;
        }
        
        public static BlueprintFeature CreateAddStatBonus(String name, String displayName, String description, String assetId, StatType skill, params BlueprintComponent[] extraComponents)
        {
            var components = extraComponents.ToList();
            components.Add(Helpers.Create<AddClassSkill>(a => a.Skill = skill));
            components.Add(Helpers.CreateAddStatBonus(skill, 1, ModifierDescriptor.Trait));
            var skillText = UIUtility.GetStatText(skill);
            return Helpers.CreateFeature(name,
                displayName,
                $"{description}\nBenefits: You gain a +1 trait bonus on {skillText} checks, and {skillText} is always a class skill for you.",
                assetId,
                Helpers.GetSkillFocus(skill).Icon,
                FeatureGroup.None,
                components.ToArray());
        }
    }

    [ComponentName("Add stat bonus based on character level")]
    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowedOn(typeof(BlueprintUnit))]
    [AllowMultipleComponents]
    public class AddStatBonusOnLevel : AddStatBonus, IHandleEntityComponent<UnitEntityData>, IUnitGainLevelHandler
    {
        public int MinLevel = 1;

        public int MaxLevelInclusive = 20;

        public override void OnTurnOn()
        {
            if (CheckLevel(Owner)) base.OnTurnOn();
        }

        void IHandleEntityComponent<UnitEntityData>.OnEntityCreated(UnitEntityData entity)
        {
            if (Fact == null && CheckLevel(entity.Descriptor)) base.OnEntityCreated(entity);
        }

        protected virtual bool CheckLevel(UnitDescriptor unit)
        {
            int level = unit.Progression.CharacterLevel;
            return level >= MinLevel && level <= MaxLevelInclusive;
        }

        void IUnitGainLevelHandler.HandleUnitGainLevel(UnitDescriptor unit, BlueprintCharacterClass @class)
        {
            if (unit != Owner) return;
            OnTurnOff();
            OnTurnOn();
        }
    }

    [AllowedOn(typeof(BlueprintParametrizedFeature))]
    public class IncreaseCasterLevelForSpell : ParametrizedFeatureComponent, IInitiatorRulebookHandler<RuleCalculateAbilityParams>
    {
        public int Bonus = 1;
        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {
            var spell = Param.Blueprint;
            if (evt.Spell != spell && evt.Spell?.Parent != spell) return;
            Log.Write($"Increase caster level of {spell.name} by {Bonus}");
            evt.AddBonusCasterLevel(Bonus);
        }

        public void OnEventDidTrigger(RuleCalculateAbilityParams evt) { }
    }

    [AllowedOn(typeof(BlueprintParametrizedFeature))]
    public class IncreaseCasterLevelForSpellMax : ParametrizedFeatureComponent, IInitiatorRulebookHandler<RuleCalculateAbilityParams>
    {
        public int Bonus;
        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {
            var spell = Param.Blueprint;
            if (evt.Spell != spell && evt.Spell?.Parent != spell) return;
            Bonus = Main.settings?.HighDCl == true ? 999999999 : 1;
            Log.Write($"Increase caster level of {spell.name} by {Bonus}");
            evt.AddBonusDC(Bonus);
            evt.AddBonusCasterLevel(Bonus);
        }

        public void OnEventDidTrigger(RuleCalculateAbilityParams evt) { }
    }

    // Implements Magical Knack's +2 CL (up to character level) bonus.
    //
    // Note: this is implemented as a rulebook event bonus, rather than increasing the
    // Spellbook's m_CasterLevelInternal, because that variable is used to determine
    // spells per day/spells known. Magical Knack should not affect those variables.
    //
    // This unfortunately means that some things do not properly account for the bonus/
    // The spellbook UI is fixed with a patch.
    [AllowedOn(typeof(BlueprintParametrizedFeature))]
    public class IncreaseCasterLevelUpToCharacterLevel : OwnedGameLogicComponent<UnitDescriptor>, IInitiatorRulebookHandler<RuleCalculateAbilityParams>
    {
        public int MaxBonus = 2;
        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {
            var spellbook = evt.Spellbook;
            if (spellbook == null) return;

            int bonus = GetBonus(spellbook);
            Log.Write($"Increase caster level of {evt.Spell?.name} by {bonus}");
            //evt.
            evt.AddBonusCasterLevel(bonus);
        }

        public void OnEventDidTrigger(RuleCalculateAbilityParams evt) { }

        internal int GetBonus(Spellbook spellbook)
        {
            return Math.Min(spellbook.Owner.Progression.CharacterLevel - spellbook.CasterLevel, MaxBonus);
        }

        static IncreaseCasterLevelUpToCharacterLevel()
        {
            Main.ApplyPatch(typeof(SpellBookCharacteristics_Setup_Patch), "Magical Knack showing caster level in spellbook UI");
        }
    }
    // This unfortunately means that some things do not properly account for the bonus/
    // The spellbook UI is fixed with a patch.
    [AllowedOn(typeof(BlueprintParametrizedFeature))]
    public class IncreaseCasterLevelCharacterLevel : OwnedGameLogicComponent<UnitDescriptor>, IInitiatorRulebookHandler<RuleCalculateAbilityParams>
    {
        public int MaxBonus = 2;
        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {
            var spellbook = evt.Spellbook;
            if (spellbook == null) return;

            int bonus = GetBonus(spellbook);
            Log.Write($"Increase caster level of {evt.Spell?.name} by {bonus}");
            if (bonus > 1) { evt.Spellbook.AddCasterLevel(); }
            //evt.AddBonusCasterLevel(bonus);
        }

        public void OnEventDidTrigger(RuleCalculateAbilityParams evt) { }

        internal int GetBonus(Spellbook spellbook)
        {
            return Math.Min(spellbook.Owner.Progression.CharacterLevel - spellbook.CasterLevel, MaxBonus);
        }

        static IncreaseCasterLevelCharacterLevel()
        {
            Main.ApplyPatch(typeof(SpellBookCharacteristics_Setup_Patch), "Magical Knack showing caster level in spellbook UI");
        }
    }


    // Selects any spell at `SpellLevel`, either from the provided `SpellList` or from all spells.
    //
    // If `CheckNotKnown` is set, it will also check that the `SpellcasterClass` spellbook does not
    // already contain this spell.
    public class SelectAnySpellAtLevel : CustomParamSelection
    {
        public bool CheckNotKnown;

        protected override IEnumerable<BlueprintScriptableObject> GetItems(UnitDescriptor beforeLevelUpUnit, UnitDescriptor previewUnit)
        {
            if (SpellList != null)
            {
                return SpellList.SpellsByLevel[SpellLevel].Spells;
            }

            // For traits: it's valid to take any spell, even one not from your current
            // class that you may be able to cast later.
            var spells = new List<BlueprintAbility>();
            foreach (var spell in Helpers.allSpells)
            {
                if (spell.Parent != null) continue;

                var spellLists = spell.GetComponents<SpellListComponent>();
                if (spellLists.FirstOrDefault() == null) continue;

                var level = spellLists.Min(l => l.SpellLevel);
                if (level == SpellLevel) spells.Add(spell);
            }
            return spells;
        }

        protected override IEnumerable<BlueprintScriptableObject> GetAllItems() => Helpers.allSpells;

        protected override bool CanSelect(UnitDescriptor unit, FeatureParam param)
        {
            // TODO: this doesn't seem to work.
            return !CheckNotKnown || !unit.GetSpellbook(SpellcasterClass).IsKnown(param.Blueprint as BlueprintAbility);
        }
    }


    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class SavingThrowBonusAgainstSpellSource : RuleInitiatorLogicComponent<RuleSavingThrow>
    {
        public int Value = 1;
        public SpellSource Source = SpellSource.Arcane;

        public ModifierDescriptor ModifierDescriptor = ModifierDescriptor.Trait;

        public override void OnEventAboutToTrigger(RuleSavingThrow evt)
        {
            var ability = evt.Reason?.Ability;
            if (ability != null && ability.Blueprint.Type == AbilityType.Spell &&
                ability.SpellSource == Source)
            {
                evt.AddTemporaryModifier(evt.Initiator.Stats.SaveWill.AddModifier(Value, this, ModifierDescriptor));
                evt.AddTemporaryModifier(evt.Initiator.Stats.SaveReflex.AddModifier(Value, this, ModifierDescriptor));
                evt.AddTemporaryModifier(evt.Initiator.Stats.SaveFortitude.AddModifier(Value, this, ModifierDescriptor));
            }
        }

        public override void OnEventDidTrigger(RuleSavingThrow evt) { }
    }

    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class WeaponAttackAndCombatManeuverBonus : RuleInitiatorLogicComponent<RuleCalculateAttackBonusWithoutTarget>, IInitiatorRulebookHandler<RuleCalculateCMB>
    {
        public BlueprintWeaponType WeaponType;
        public int AttackBonus;
        public ModifierDescriptor Descriptor;

        public override void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
        {
            var weaponType = evt.Weapon?.Blueprint.Type;
            if (weaponType == WeaponType)
            {
                var bonus = AttackBonus * Fact.GetRank();

                // TODO: this bonus should be a "trait" bonus. But doing it this way shows up in the UI.
                evt.AddBonus(bonus, Fact);
                //evt.AddTemporaryModifier(evt.Initiator.Stats.AdditionalAttackBonus.AddModifier(bonus, this, ModifierDescriptor.Trait));
            }
        }
        public override void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt) { }

        public void OnEventAboutToTrigger(RuleCalculateCMB evt)
        {
            var weaponType = evt.Initiator.GetThreatHand()?.Weapon?.Blueprint.Type;
            if (weaponType == WeaponType)
            {
                var bonus = AttackBonus * Fact.GetRank();
                // TODO: this bonus should be a "trait" bonus. But doing it this way shows up in the UI.
                evt.AddBonus(bonus, Fact);
                //evt.AddTemporaryModifier(evt.Initiator.Stats.AdditionalCMB.AddModifier(bonus, this, ModifierDescriptor.Trait));
            }
        }
        public void OnEventDidTrigger(RuleCalculateCMB evt) { }
    }

    [AllowedOn(typeof(BlueprintFeature))]
    public class DamageBonusIfMoraleBonus : RuleInitiatorLogicComponent<RuleCalculateWeaponStats>
    {
        public override void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            var attackBonus = evt.Initiator.Stats.AdditionalAttackBonus;
            if (attackBonus.ContainsModifier(ModifierDescriptor.Morale) &&
                attackBonus.GetDescriptorBonus(ModifierDescriptor.Morale) > 0)
            {
                evt.AddBonusDamage(1);
            }
        }

        public override void OnEventDidTrigger(RuleCalculateWeaponStats evt) { }
    }

    [AllowedOn(typeof(BlueprintFeature))]
    public class ExtraLuckBonus : RuleInitiatorLogicComponent<RuleCalculateWeaponStats>, IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>, IInitiatorRulebookHandler<RuleSkillCheck>, IInitiatorRulebookHandler<RuleSavingThrow>
    {
        void IncreaseLuckBonus(RulebookEvent evt, StatType stat)
        {
            int luck = GetLuckBonus(evt, stat);
            if (luck > 0)
            {
                var mod = Owner.Stats.GetStat(stat).AddModifier(luck + 1, this, ModifierDescriptor.Luck);
                evt.AddTemporaryModifier(mod);
            }
        }

        int GetLuckBonus(RulebookEvent evt, StatType stat)
        {
            var value = Owner.Stats.GetStat(stat);
            if (value.ContainsModifier(ModifierDescriptor.Luck))
            {
                return value.GetDescriptorBonus(ModifierDescriptor.Luck);
            }
            return 0;
        }

        public override void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            int luck = GetLuckBonus(evt, StatType.AdditionalDamage);
            if (luck > 0) evt.AddBonusDamage(1);
        }
        public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
        {
            int luck = GetLuckBonus(evt, StatType.AdditionalAttackBonus);
            if (luck > 0) evt.AddBonus(1, Fact);
        }
        public void OnEventAboutToTrigger(RuleSavingThrow evt) => IncreaseLuckBonus(evt, evt.StatType);
        public void OnEventAboutToTrigger(RuleSkillCheck evt) => IncreaseLuckBonus(evt, evt.StatType);

        public override void OnEventDidTrigger(RuleCalculateWeaponStats evt) { }
        public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt) { }
        public void OnEventDidTrigger(RuleSavingThrow evt) { }
        public void OnEventDidTrigger(RuleSkillCheck evt) { }
    }

    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class AttackBonusIfAlignmentAndHealth : RuleInitiatorLogicComponent<RuleCalculateAttackBonus>
    {
        public AlignmentComponent TargetAlignment;

        public ModifierDescriptor Descriptor;

        public int Value;

        public float HitPointPercent;

        public override void OnEventAboutToTrigger(RuleCalculateAttackBonus evt)
        {

            var hpPercent = ((float)Owner.HPLeft) / Owner.MaxHP;
            //Log.Write($"RuleCalculateAttackBonus HP {hpPercent}%, alignment {evt.Target.Descriptor.Alignment.Value}, TargetAlignment {TargetAlignment}, bonus {Value}, descriptor {Descriptor}");
            if (hpPercent < HitPointPercent &&
                evt.Target.Descriptor.Alignment.Value.HasComponent(TargetAlignment))
            {
                // TODO: this bonus should be a "trait" bonus. But doing it this way shows up in the UI.
                evt.AddBonus(Value, Fact);
                //evt.AddTemporaryModifier(evt.Initiator.Stats.AdditionalAttackBonus.AddModifier(Value, this, Descriptor));
            }
        }

        public override void OnEventDidTrigger(RuleCalculateAttackBonus evt) { }
    }

    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class BuffIfHealth : RuleInitiatorLogicComponent<RuleCalculateAttackBonus>
    {
        [JsonProperty]
        Buff appliedBuff;


        public ModifierDescriptor Descriptor;

        public int Value;

        public float HitPointPercent;
        //public override void 
        public override void OnEventAboutToTrigger(RuleCalculateAttackBonus evt)
        {
            var FrogPolymorphBuff = Traits.library.Get<BlueprintBuff>("662aa00fd6242e643b60ac8336ff39e6");
            var hpPercent = ((float)Owner.HPLeft) / Owner.MaxHP;
            //Log.Write($"RuleCalculateAttackBonus HP {hpPercent}%, alignment {evt.Target.Descriptor.Alignment.Value}, TargetAlignment {TargetAlignment}, bonus {Value}, descriptor {Descriptor}");
            if (hpPercent < HitPointPercent)
            {
                // TODO: this bonus should be a "trait" bonus. But doing it this way shows up in the UI.
                appliedBuff = Owner.AddBuff(FrogPolymorphBuff,Owner.Unit,TimeSpan.FromSeconds(12.0));
                evt.AddBonus(Value, Fact);

                //evt.AddTemporaryModifier(evt.Initiator.Stats.AdditionalAttackBonus.AddModifier(Value, this, Descriptor));
            }
        }

        public override void OnEventDidTrigger(RuleCalculateAttackBonus evt) { }
    }


    

    public class ReplaceBaseStatForStatTypeLogic : OwnedGameLogicComponent<UnitDescriptor>
    {
        public StatType StatTypeToReplaceBastStatFor;
        public StatType NewBaseStatType;
        private StatType? _oldStatType = null;

        public override void OnTurnOn()
        {
            ModifiableValue value = base.Owner.Stats.GetStat(StatTypeToReplaceBastStatFor);
            if (true)//value.GetType() == typeof(ModifiableValueSkill))
            {
                if (_oldStatType == null)
                {
                    _oldStatType = ((ModifiableValueSkill)value).BaseStat.Type;
                }

                ModifiableValue oldStat = base.Owner.Stats.GetStat((StatType)_oldStatType);
                ModifiableValue newStat = base.Owner.Stats.GetStat(NewBaseStatType);

                Traverse traverse = Traverse.Create(value);
                traverse.Field("BaseStat").SetValue(newStat);
                newStat.AddDependentValue(value);
                oldStat.RemoveDependentValue(value);
                value.UpdateValue();
            }
        }

        public override void OnTurnOff()
        {
            ModifiableValue value = base.Owner.Stats.GetStat(StatTypeToReplaceBastStatFor);
            if (true)//value.GetType() == typeof(ModifiableValueSkill) && _oldStatType != null)
            {
                ModifiableValue oldStat = base.Owner.Stats.GetStat((StatType)_oldStatType);
                ModifiableValue newStat = base.Owner.Stats.GetStat(NewBaseStatType);

                Traverse traverse = Traverse.Create(value);
                traverse.Field("BaseStat").SetValue(oldStat);
                oldStat.AddDependentValue(value);
                newStat.RemoveDependentValue(value);
                value.UpdateValue();
            }
        }
    }

// This adds support for a feat adding additional selections  (e.g. Additional Traits, Dragon Magic).
//
// The game doesn't natively support this, except via BlueprintProgression. However,
// BlueprintProgression doesn't work for things you select later, because it only adds
// the current level's features. Essentially, progressions are only designed to work for
// class features awarded at fixed levels (typically 1st level). There isn't a notion of
// progressions that are relative to the level you picked them at.
//
// So to support adding selections, we patch SelectFeature.Apply to add the follow-up features.
//
// However that wouldn't work for cases where a feat can change the progression level, as with
// Greater Eldritch Heritage.
//
// TODO: alternative design2: use IUnitGainFactHandler. I think I tried that and it didn't work,
// but don't recall why (unit not active during chargen?).
[Harmony12.HarmonyPatch(typeof(SelectFeature), "Apply", new Type[] { typeof(LevelUpState), typeof(UnitDescriptor) })]
    static class SelectFeature_Apply_Patch
    {
        internal static Dictionary<BlueprintFeature, Action<LevelUpState, UnitDescriptor>> onApplyFeature = new Dictionary<BlueprintFeature, Action<LevelUpState, UnitDescriptor>>();

        static SelectFeature_Apply_Patch() => Main.ApplyPatch(typeof(SelectFeature_Apply_Patch), "Feats that offer 2 selections, such as Additional Traits, Spell Blending, etc");

        static void Postfix(SelectFeature __instance, LevelUpState state, UnitDescriptor unit)
        {
            try
            {
                var self = __instance;
                var item = self.Item;
                if (item == null) return;

                Action<LevelUpState, UnitDescriptor> action;
                if (onApplyFeature.TryGetValue(item.Feature, out action))
                {
                    action(state, unit);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    public class ACBonusDuringSurpriseRound : RuleTargetLogicComponent<RuleCalculateAC>
    {
        public override void OnEventAboutToTrigger(RuleCalculateAC evt)
        {
            var combatState = Owner.Unit.CombatState;
            if (combatState.IsInCombat && combatState.IsWaitingInitiative)
            {
                evt.AddBonus(Owner.Stats.AC.DexterityBonus / 2, Fact);
            }
        }

        public override void OnEventDidTrigger(RuleCalculateAC evt)
        {
            throw new NotImplementedException();
        }
    }

    [AllowedOn(typeof(BlueprintUnitFact))]
    public class UndoSelection : ComponentAppliedOnceOnLevelUp
    {
        public static Lazy<BlueprintFeature> Feature = new Lazy<BlueprintFeature>(() =>
            Helpers.CreateFeature("UndoSelectionChoice", "(Go back)",
                "Select this to go back to the previous selection, allowing you to pick something else.",
                "48963ed6422b41e5ba23d1f3f0fbe7c7", null, FeatureGroup.None,
                Helpers.Create<UndoSelection>()));

        protected override void Apply(LevelUpState state)
        {
            Log.Write($"{GetType().Name}: trying to unselect");
            var selection = state.Selections.FirstOrDefault(s => s.SelectedItem?.Feature == Fact.Blueprint);
            if (selection != null)
            {
                Log.Write($"Unselecting selection {selection.Index}");
                Game.Instance.UI.CharacterBuildController.LevelUpController.UnselectFeature(selection);
            }
        }

        protected override bool RemoveAfterLevelUp => true;
    }


    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class DamageBonusAgainstFlankedTarget : RuleInitiatorLogicComponent<RuleCalculateDamage>
    {
        public int Bonus;

        public static DamageBonusAgainstFlankedTarget Create(int bonus)
        {
            var d = Helpers.Create<DamageBonusAgainstFlankedTarget>();
            d.Bonus = bonus;
            return d;
        }

        public override void OnEventAboutToTrigger(RuleCalculateDamage evt)
        {
            if (evt.Target.CombatState.IsFlanked && evt.DamageBundle.Weapon?.Blueprint.IsMelee == true)
            {
                evt.DamageBundle.WeaponDamage?.AddBonusTargetRelated(Bonus);
            }
        }

        public override void OnEventDidTrigger(RuleCalculateDamage evt) { }
    }

    public class DamageBonusIfInvisibleToTarget : RuleInitiatorLogicComponent<RuleCalculateDamage>
    {
        public int Bonus;

        public override void OnEventAboutToTrigger(RuleCalculateDamage evt)
        {
            if (evt.DamageBundle.Weapon?.Blueprint.IsMelee != true) return;

            var initiator = evt.Initiator;
            var target = evt.Target;
            // Flat-footed isn't enough, but we need to run the rule to assess the other variables
            // (such as IgnoreVisibility and IgnoreConcealment)
            var rule = Rulebook.Trigger(new RuleCheckTargetFlatFooted(initiator, target));
            if (rule.IsFlatFooted)
            {
                var targetCannotSeeUs = target.Descriptor.State.IsHelpless || // sleeping, etc
                    !target.Memory.Contains(initiator) && !rule.IgnoreVisibility || // hasn't seen us, e.g. stealth/ambush
                    UnitPartConcealment.Calculate(target, initiator) == Concealment.Total && !rule.IgnoreConcealment; // invisibility/blindness etc

                if (targetCannotSeeUs)
                {
                    evt.DamageBundle.First?.AddBonusTargetRelated(Bonus);
                }
            }
        }

        public override void OnEventDidTrigger(RuleCalculateDamage evt) { }
    }






    [Harmony12.HarmonyPatch(typeof(SpellBookCharacteristics), "Setup", new Type[0])]
    static class SpellBookCharacteristics_Setup_Patch
    {
        static void Postfix(SpellBookCharacteristics __instance)
        {
            var self = __instance;
            try
            {
                var controller = Game.Instance.UI.SpellBookController;
                var spellbook = controller.CurrentSpellbook;
                if (spellbook != null && spellbook.CasterLevel > 0)
                {
                    //spellbook.
                    int bonus = 0;
                    foreach (var feat in spellbook.Owner.Progression.Features.Enumerable)
                    {
                        foreach (var c in feat.SelectComponents<IncreaseCasterLevelUpToCharacterLevel>())
                        {
                            //bonus = Math.Max(bonus, c.GetBonus(spellbook));
                            bonus += c.GetBonus(spellbook);
                        }
                    }
                    if (bonus > 0)
                    {
                        self.CasterLevel.text = (spellbook.CasterLevel + bonus).ToString();
                        self.Concentration.text = (spellbook.GetConcentration() + bonus).ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}