// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using JetBrains.Annotations;

using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Root.Strings.GameLog;
using Kingmaker.Controllers.Combat;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UI.Log;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Utility;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

using static Kingmaker.UI.GenericSlot.EquipSlotBase;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

using RES = EldritchArcana.Properties.Resources;
using static EldritchArcana.Common;

namespace EldritchArcana
{
    static class Spells
    {
        internal const string hypnoticPatternId = "bd623ae7179a4d19a40b977ffca1b83f";
        internal const string MassResurrectionId = "bd623ae7179a4d19a40b977ffca1b84g";
        internal const string SummonWildhuntId = "vr623ae7179a4d19a40b977ffca1b52f";
        internal const string SummonGoblinSixId = "C001615230714F29BAEC3E2A0F40C3BF";
        internal const string SummonSquirrelId = "2E8716AC8C09468ABDA8BDB7BE8E5971";
        static LibraryScriptableObject library => Main.library;

        internal static AbilitySpawnFx commonTransmutationFx;

        internal static void Load()
        {
            var angelicAspect = library.Get<BlueprintAbility>("75a10d5a635986641bfbcceceec87217");
            commonTransmutationFx = angelicAspect.GetComponent<AbilitySpawnFx>();

            Main.SafeLoad(FixElementalFormSpellcasting, RES.FixElementalSpells_info);

            Main.SafeLoad(DismissSpell.Load, RES.DismissSpells_info);

            Main.SafeLoad(FireSpells.Load, RES.FireSpells_info);
            Main.SafeLoad(FlySpells.Load, RES.FlySpells_info);
            Main.SafeLoad(TimeStop.Load, RES.TimeStopSpells_info);
            Main.SafeLoad(KnockAndDetectSecrets.Load, RES.KnockAndDetectSecretsSpells_info);
            Main.SafeLoad(LoadTrueResurrection, RES.TrueResurrectionSpells_info);
            Main.SafeLoad(LoadSummonWildhunt, RES.SummonWildhuntSpells_info);

            Main.SafeLoad(ExperimentalSpells.LoadSpritualWeapon, RES.SpritualWeaponSpells_info);
            Main.SafeLoad(ExperimentalSpells.LoadEmergencyForceSphere, RES.EmergencyForceSphereSpells_info);

            Main.SafeLoad(LoadGreaterMagicWeapon, RES.GreaterMagicWeaponSpells_info);
            Main.SafeLoad(LoadWeaponOfAwe, RES.WeaponOfAweSpells_info);

            Main.SafeLoad(LoadHypnoticPattern, RES.HypnoticPatternSpells_info);

            // TODO: divine spell: Blessing of Fervor, Atonement (w/ option to change to your alignment?)
            // TODO: blood money would be interesting.
            // Also lots of fun utility spells:
            //
            // - Web Bolt: ranged touch single target web
            // - Floating Disk: A temporary group carry boost
            // - Darting Duplicate: A single copy mirror image (enemies can ignore with will save though)
            // - Jump: target gets a bonus to Acrobatics
            // - Warding Weapon: caster does not provoke AoO when casting
            // - Communal Mount: increase map movement speed
            // - Touch of Idiocy
            // - Twilight Haze: A group stealth boost
            //
            // Interesting ones:
            // - Alarm / Bed of Iron: probably not difficult, if I can find the code that handles resting.
            // - Spectral Hand: not too bad if it just increases touch range
            //   (ignoring that the hand should technically be targetable).
            //   Worst case it could be a temporary large thievery boost.
            // - Charm person: maybe can work similar to confuse/daze, needs investigation.
            //
            // - Battering Blast (force): https://www.d20pfsrd.com/magic/all-spells/b/battering-blast/
            // - Forceful Strike: https://www.d20pfsrd.com/magic/all-spells/f/forceful-strike
            // - Fire seeds: https://www.d20pfsrd.com/magic/all-spells/f/fire-seeds/

            // TODO: Teleport/Shadow Walk/Greater Teleport?
            //
            // The biggest issue with these spells is they can break campaign scripts,
            // which rely on triggering events at certain locations. So to avoid that,
            // instead of an actual teleport, they could be a large movement speed bonus
            // on the world map (and possibly the normal map, but not too high to
            // break trigger scripts).
            //
            // Basically: cast teleport, exit to world map, and you can now move without time passing
            // or encumbrance. Nothing is revealed during this travel, so you can only go along routes
            // you've been previously (that removes the mishap chance).
            //
            // Shadow walk would be Teleport without the distance limit, and you can bring a full party
            // once you get the spell.
            //
            // Perhaps Greater Teleport allows you to see things as you travel. (Conceptually combine
            // scrying the route with movement.)
            //
            // Note: Teleport has a limit of how many people can be brought along.
            // You'd need to be 18th level to transport a full party of 6 (ignoring pets),
            // or you need 2 arcane casters. Since PF:K has bigger party size, we can improve
            // the scaling to 1 person/2 levels (instead of 1 per 3). That would mean the full party
            // can be transported at level 12, same as PnP.
        }

        static void FixElementalFormSpellcasting()
        {
            var spellIds = new string[] {
                "690c90a82bf2e58449c6b541cb8ea004", // elemental body i, ii, iii, iv
                "6d437be73b459594ab103acdcae5b9e2",
                "459e6d5aab080a14499e13b407eb3b85",
                "376db0590f3ca4945a8b6dc16ed14975"
            };
            foreach (var spellId in spellIds)
            {
                var baseSpell = library.Get<BlueprintAbility>(spellId);
                foreach (var spell in baseSpell.Variants)
                {
                    var buff = spell.GetComponent<AbilityEffectRunAction>().Actions.Actions
                            .OfType<ContextActionApplyBuff>().First().Buff;
                    buff.AddComponent(AddMechanicsFeature.MechanicsFeatureType.NaturalSpell.CreateAddMechanics());
                }
            }
        }

        static void LoadWeaponOfAwe()
        {
            var shaken = library.Get<BlueprintBuff>("25ec6cb6ab1845c48a95f9c20b034220");

            var enchantment = Helpers.Create<BlueprintWeaponEnchantment>();
            enchantment.name = "WeaponOfAweEnchantment";
            Helpers.SetLocalizedStringField(enchantment, "m_EnchantName", RES.WeaponOfAweSpells_info);
            Helpers.SetLocalizedStringField(enchantment, "m_Description", RES.WeaponofAweDescription_info);
            Helpers.SetLocalizedStringField(enchantment, "m_Prefix", "");
            Helpers.SetLocalizedStringField(enchantment, "m_Suffix", "");
            library.AddAsset(enchantment, "21985d11a0f941a2b359c48b5d8a32da");
            enchantment.SetComponents(Helpers.Create<WeaponOfAweLogic>(w => w.Buff = shaken));

            var paladinWeaponBond = library.Get<BlueprintAbility>("7ff088ab58c69854b82ea95c2b0e35b4");
            var spell = Helpers.CreateAbility("WeaponOfAwe", RES.WeaponOfAweSpells_info,
                RES.WeaponOfAweAbilityDescription_info,
                "9c98a1de91a54ba583b9f4880d505766",
                paladinWeaponBond.Icon,
                AbilityType.Spell, CommandType.Standard, AbilityRange.Close,
                Helpers.minutesPerLevelDuration, "",
                Helpers.CreateContextRankConfig(),
                Helpers.CreateSpellComponent(SpellSchool.Transmutation),
                Helpers.CreateRunActions(Helpers.Create<ContextActionEnchantWornItem>(c =>
                {
                    c.Enchantment = enchantment;
                    c.Slot = SlotType.PrimaryHand;
                    c.DurationValue = Helpers.CreateContextDuration(rate: DurationRate.Minutes);
                })));

            //var Shambler = Helpers.CreateAbility("hoi", "shambler", "summons 1d4+2 shambling mounts", "c88b9398af66406aaa111888df308eb8", dice, AbilityType.Spell, CommandType.Standard, AbilityRange.Medium, "long", null,
            //    );

            spell.CanTargetSelf = true;
            spell.NeedEquipWeapons = true;
            // Note: the paladin animation is neat, but it's very long.
            //spell.Animation = paladinWeaponBond.Animation;
            var arcaneWeaponSwitchAbility = library.Get<BlueprintAbility>("3c89dfc82c2a3f646808ea250eb91b91");
            spell.Animation = arcaneWeaponSwitchAbility.Animation;
            spell.AvailableMetamagic = Metamagic.Quicken | Metamagic.Heighten | Metamagic.Extend;
            spell.CanTargetFriends = true;

            spell.AddToSpellList(Helpers.clericSpellList, 2);
            spell.AddToSpellList(Helpers.inquisitorSpellList, 2);
            spell.AddToSpellList(Helpers.paladinSpellList, 2);
            Helpers.AddSpellAndScroll(spell, "5739bf41893fddf4f98f8bd6a86b0a52"); // disrupting wepaon scroll
        }

        static void LoadSummonWildhunt()
        {
            var GoblinSummonSix = library.Get<BlueprintAbility>("1909400d0731ae049ac62edade91c1f7");//TestSummonAbility.
            //var summonMonsterIXd4plus1 = library.Get<BlueprintAbility>("4988b2e622c6f2d4b897894e3be13f09");
            //var SummonMonsterIXBase = library.Get<BlueprintAbility>("52b5df2a97df18242aec67610616ded0");
            //var SummonSpecial = library.Get<BlueprintAbility>("563876dbfa98696409e60fc635ea9d77");//summon murder pony
            //var SummonSpecial = library.Get<BlueprintAbility>("c2e5b967c47a81c4aa355ef213ec5634");//RaySpellLanternKingStar.
            var SummonSpecial = library.Get<BlueprintAbility>("06a171f8ce3ad71418ffc516ded07a6b");//squirrelhorde
            //var SummonSpecial = library.Get<BlueprintAbility>("26c8d5dc21025564baaeaee51ede05c1");//Switchdoublecompainions
            //var kalike = library.Get<BlueprintAbility>("5cb0f13fc0eef464993b2e082f186032");//SwitchTo_Kalikke_Ability.
            //var kanerah = library.Get<BlueprintAbility>("fb96d35da88acb1498dc51a934f6c4d5");//SwitchTo_Kanerah_Ability.
            var camo = library.Get<BlueprintAbility>("b26a123a009d4a141ac9c19355913285");//camoflage
            var GobIcon = Image2Sprite.Create("Mods/EldritchArcana/sprites/Summon_goblins.png");
            //TestSummonAbility.1909400d0731ae049ac62edade91c1f7//summon 6 goblins


            //var spell = library.CopyAndAdd(summonMonsterIXd4plus1, "Wildhunt", SummonWildhuntId);
            var GoblinSpell = library.CopyAndAdd(GoblinSummonSix, "GoblinSummonSix", SummonGoblinSixId);//
            GoblinSpell.AddComponent(Helpers.CreateSpellComponent(SpellSchool.Conjuration));
            Lazy<BlueprintItem> shortSword = new Lazy<BlueprintItem>(() => library.Get<BlueprintItem>("f717b39c351b8b44388c471d4d272f4e"));
            GoblinSpell.MaterialComponent.Item = shortSword.Value;
            GoblinSpell.MaterialComponent.Count = 1;
            var SquirrelSpell = library.CopyAndAdd(SummonSpecial, "Squirrelhorde", SummonSquirrelId);
            SquirrelSpell.SetNameDescription(RES.SquirrelSpellName_info,
                RES.SquirrelSpellDescription_info);
            GoblinSpell.SetNameDescription(RES.GoblinSpellName_info,
                RES.GoblinSpellDescription_info);
            GoblinSpell.SetIcon(GobIcon);
            //var spell2 = library.CopyAndAdd(kalike, "kalikesw", "5cc0f13fc0eef464993b2e082f186033");
            //var spell3 = library.CopyAndAdd(kanerah, "kanerahsw", "5db0f13fc0eef464993b2e082f186034");
            //Helpers.
            //spell
            //spell.ReplaceComponent<ISummonPoolHandler>(summonWildhunt.GetComponent<ISummonPoolHandler>());
            SquirrelSpell.AddToSpellList(Helpers.druidSpellList, 3);
            SquirrelSpell.AddToSpellList(Helpers.inquisitorSpellList, 3);
            SquirrelSpell.AddToSpellList(Helpers.wizardSpellList, 3);
            SquirrelSpell.AddToSpellList(Helpers.bardSpellList, 3);

            GoblinSpell.AddToSpellList(Helpers.magusSpellList, 2);
            GoblinSpell.AddToSpellList(Helpers.wizardSpellList, 2);
            GoblinSpell.AddToSpellList(Helpers.bardSpellList, 2);
            GoblinSpell.AddToSpellList(Helpers.druidSpellList, 2);
        }

        static void LoadTrueResurrection()
        {
            var resurrection = library.Get<BlueprintAbility>("80a1a388ee938aa4e90d427ce9a7a3e9");
            var raiseDead = library.Get<BlueprintAbility>("a0fc99f0933d01643b2b8fe570caa4c5");

            var resurrectionBuff = library.Get<BlueprintBuff>("12f2f2cf326dfd743b2cce5b14e99b3c");//resurrectionBuff
            var fireball = library.Get<BlueprintAbility>("2d81362af43aeac4387a3d4fced489c3");
            var spell = library.CopyAndAdd(resurrection, "MassResurrection", MassResurrectionId);

            //var 00084298d39172b4e954b8eca5575dd9
            spell.SetNameDescription(RES.TrueResurrectionSpells_info,
                RES.ResurrectionSpellDescription_info);

            spell.ActionType = CommandType.Free;

            //spell.HasAreaEffect();

            spell.ReplaceComponent<AbilityTargetsAround>(Helpers.CreateAbilityTargetsAround(25.Feet(), TargetType.Ally, includeDead: true));

            //spell.ReplaceComponent<AbilityDeliverClashingRocks>()
            spell.ReplaceComponent<AbilityAoERadius>(fireball.GetComponent<AbilityAoERadius>());
            spell.ReplaceComponent<SpellComponent>(raiseDead.GetComponent<SpellComponent>());
            spell.MaterialComponent.Count = 1;

            //spell.AddToSpellList(Helpers.spel, 3);
            spell.AddToSpellList(Helpers.healingDomainSpellList, 8);
            spell.AddToSpellList(Helpers.druidSpellList, 9);
            /*
            Helpers.AddSpellAndScroll(spell, "84cd707a7ae9f934389ed6bbf51b023a"); // scroll rainbow pattern*/

        }

        static void LoadHypnoticPattern()
        {
            var rainbowPattern = library.Get<BlueprintAbility>("4b8265132f9c8174f87ce7fa6d0fe47b");
            var rainbowPatternBuff = library.Get<BlueprintBuff>("6477ae917b0ec7a4ca76bc9f36b023ac");

            var spell = library.CopyAndAdd(rainbowPattern, "HypnoticPattern", hypnoticPatternId);
            spell.SetNameDescription(RES.HypnoticPatternSpells_info,
                RES.HypnoticSpellDescription_info);
            spell.LocalizedDuration = Helpers.CreateString($"{spell.name}.Duration", RES.HypnoticDurationLocalized_info);

            var buff = library.CopyAndAdd(rainbowPatternBuff, $"{spell.name}Buff", "d5a5ac267e21484a9332d96f3be3452d");
            buff.SetNameDescription(spell.Name, spell.Description);

            // duration is 2 rounds after concentration expires
            buff.AddComponent(SpellConcentrationLogic.Create(Helpers.CreateContextDuration(2)));

            var constructType = library.Get<BlueprintFeature>("fd389783027d63343b4a5634bd81645f");
            var undeadType = library.Get<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33");
            var bloodlineUndeadArcana = library.Get<BlueprintFeature>("1a5e7191279e7cd479b17a6ca438498c");

            spell.SetComponents(
                SpellSchool.Illusion.CreateSpellComponent(),
                Helpers.CreateAbilityTargetsAround(10.Feet(), TargetType.Enemy),
                rainbowPattern.GetComponent<SpellDescriptorComponent>(),
                // Adjust HD affected: 2d4 + caster level (max 10).
                Helpers.CreateCalculateSharedValue(
                    DiceType.D4.CreateContextDiceValue(2, AbilityRankType.StatBonus.CreateContextValue())),
                // Caster level max 10.
                Helpers.CreateContextRankConfig(type: AbilityRankType.StatBonus, max: 10),
                rainbowPattern.GetComponent<AbilitySpawnFx>(),
                Helpers.CreateRunActions(Helpers.CreateConditional(
                    Helpers.CreateConditionsCheckerOr(
                        // Can't apply if:
                        // - it's unconcious,
                        // - a construct,
                        // - undead (unless caster has undead bloodline arcnaa),
                        // - or insufficient HD remaining.
                        Helpers.Create<ContextConditionIsUnconscious>(),
                        constructType.CreateConditionHasFact(),
                        Helpers.CreateAndLogic(false, undeadType.CreateConditionHasFact(), bloodlineUndeadArcana.CreateConditionCasterHasFact(not: true)),
                        Helpers.Create<ContextConditionHitDice>(c => { c.AddSharedValue = true; c.Not = true; })
                    ),
                    null,
                    ifFalse: new GameAction[] {
                        SavingThrowType.Will.CreateActionSavingThrow(
                            // Will save faled: apply buff (permanent, buff uses concentration+2).
                            Helpers.CreateConditionalSaved(null, failed:
                                buff.CreateApplyBuff(Helpers.CreateContextDuration(0), fromSpell: true, permanent: true))),
                        // Regardless of will save, subtract these HD.
                        SharedValueChangeType.SubHD.CreateActionChangeSharedValue()
                    })));

            spell.AddToSpellList(Helpers.wizardSpellList, 2);
            spell.AddToSpellList(Helpers.bardSpellList, 2);
            Helpers.AddSpellAndScroll(spell, "84cd707a7ae9f934389ed6bbf51b023a"); // scroll rainbow pattern
        }

        static void LoadGreaterMagicWeapon()
        {
            var enchantments = new String[] {
                "d704f90f54f813043a525f304f6c0050",
                "9e9bab3020ec5f64499e007880b37e52",
                "d072b841ba0668846adeb007f623bd6c",
                "6a6a0901d799ceb49b33d4851ff72132",
                "746ee366e50611146821d61e391edf16",
            }.Select(library.Get<BlueprintWeaponEnchantment>).ToArray();

            var name = "GreaterMagicWeapon";
            for (int i = 0; i < enchantments.Length; i++)
            {
                var enchant = library.CopyAndAdd(enchantments[i], $"{name}Bonus{i + 1}", enchantments[i].AssetGuid,
                    "01a963207ccb484897f3de00344cad55");
                enchant.SetComponents(Helpers.Create<GreaterMagicWeaponBonusLogic>(g => g.EnhancementBonus = i + 1));
                enchantments[i] = enchant;
            }

            var enchantItem = Helpers.Create<ContextActionGreaterMagicWeapon>();
            enchantItem.Enchantments = enchantments;
            enchantItem.DurationValue = Helpers.CreateContextDuration(rate: DurationRate.Hours);

            var arcaneWeaponSwitchAbility = library.Get<BlueprintAbility>("3c89dfc82c2a3f646808ea250eb91b91");
            var spell = Helpers.CreateAbility(name, RES.GreaterMagicWeaponSpells_info,
                RES.GreaterMagicWeaponAbilityDescription_info,
                "6e513ce66905424eb441755cd264fbfa",
                arcaneWeaponSwitchAbility.Icon,
                AbilityType.Spell, CommandType.Standard, AbilityRange.Close,
                Helpers.hourPerLevelDuration, "",
                Helpers.CreateContextRankConfig(),
                Helpers.CreateSpellDescriptor(SpellDescriptor.None),
                Helpers.CreateSpellComponent(SpellSchool.Transmutation),
                Helpers.CreateRunActions(enchantItem)
            );
            spell.CanTargetSelf = true;
            spell.NeedEquipWeapons = true;
            spell.Animation = arcaneWeaponSwitchAbility.Animation;
            spell.AvailableMetamagic = Metamagic.Quicken | Metamagic.Heighten | Metamagic.Extend;
            spell.CanTargetFriends = true;

            spell.AddToSpellList(Helpers.wizardSpellList, 3);
            spell.AddToSpellList(Helpers.magusSpellList, 3);
            spell.AddToSpellList(Helpers.paladinSpellList, 3);
            spell.AddToSpellList(Helpers.inquisitorSpellList, 3);
            spell.AddToSpellList(Helpers.clericSpellList, 4);
            Helpers.AddSpellAndScroll(spell, "5739bf41893fddf4f98f8bd6a86b0a52"); // scroll disrupting weapon
        }
    }

    public class SpellConcentrationLogic : BuffLogic, IUnitCommandActHandler, IInitiatorRulebookHandler<RuleCastSpell>, ITargetRulebookHandler<RuleDealDamage>
    {
        [CanBeNull]
        public ContextDurationValue Duration;

        [JsonProperty]
        bool concentrating = true;

        public static SpellConcentrationLogic Create(ContextDurationValue duration = null)
        {
            var s = Helpers.Create<SpellConcentrationLogic>();
            s.Duration = duration;
            return s;
        }

        // Track concentration for a buff on ourselves, or a buff applied to others.
        public new UnitEntityData GetSubscribingUnit() => Buff.Context.MaybeCaster;

        public void HandleUnitCommandDidAct(UnitCommand command)
        {
            if (command.Executor != GetSubscribingUnit()) return;
            // Note: ignoring UnitInteractWithObject because things like opening doors should be a move action.
            if (command.Type == CommandType.Standard && !(command is UnitInteractWithObject))
            {
                RemoveBuff();
            }
        }

        public void OnEventAboutToTrigger(RuleCastSpell evt)
        {
            if (evt.Spell.Blueprint.Type == AbilityType.Spell) RemoveBuff();
        }

        public void OnEventAboutToTrigger(RuleDealDamage evt) { }

        public void OnEventDidTrigger(RuleCastSpell evt) { }

        public void OnEventDidTrigger(RuleDealDamage evt)
        {
            var spell = Buff.Context.SourceAbilityContext?.Ability;
            if (spell != null && !Rulebook.Trigger(new RuleCheckConcentration(evt.Target, spell, evt)).Success)
            {
                RemoveBuff();
            }
        }

        void RemoveBuff()
        {
            if (!concentrating) return;
            concentrating = false;
            var duration = Duration != null ? Duration.Calculate(Buff.Context).Seconds : new TimeSpan();
            var remaining = duration.TotalMilliseconds == 0 ? "now"
                : "in " + BlueprintRoot.Instance.Calendar.GetCompactPeriodString(duration);
            Helpers.GameLog.AddLogEntry(
                $"Not concentrating on {Buff.Name}, ends {remaining}.",
                GameLogStrings.Instance.InitiativeRoll.Color, LogChannel.Combat);
            Buff.RemoveAfterDelay(duration);
        }
    }

    // Adapted from ContextActionEnchantWornItem, adjusted so the bonus won't stack.
    public class ContextActionGreaterMagicWeapon : ContextAction
    {
        public BlueprintItemEnchantment[] Enchantments;

        public ContextDurationValue DurationValue;

        public override string GetCaption() => $"GreaterMagicWeapon enchant (for {DurationValue})";

        public override void RunAction()
        {
            var caster = Context.MaybeCaster;
            if (caster == null) return;

            var unit = Target.Unit;
            if (unit == null) return;

            var weapon = unit.GetThreatHand()?.Weapon;
            if (weapon == null) return;

            foreach (var enhance in Enchantments)
            {
                var fact = weapon.Enchantments.GetFact(enhance);
                if (fact != null && fact.IsTemporary)
                {
                    weapon.RemoveEnchantment(fact);
                }
            }

            // Calculate that weapon's existing enhancement bonus.
            var rule = Context.TriggerRule(new RuleCalculateWeaponStats(unit, weapon));

            // Greater Magic Weapon:
            // +1 enhancement per 4 caster levels.
            var casterLevel = Context.Params.CasterLevel;
            var bonus = Math.Min(Math.Max(1, casterLevel / 4), 5);

            var delta = bonus - rule.Enhancement;
            Log.Write($"{GetType().Name} existing bonus {rule.Enhancement} (partial enhancement {rule.EnhancementTotal}), target bonus: {bonus}");
            if (delta > 0)
            {
                var enchant = Enchantments[delta - 1];
                var rounds = DurationValue.Calculate(Context);
                Log.Write($"Add enchant {enchant} to {weapon} for {rounds} rounds.");
                weapon.AddEnchantment(enchant, Context, rounds);
            }
        }
    }

    // Adapted from WeaponEnhancementBonus, adjusted so it won't penetrate DR higher than magical (in theory).
    public class GreaterMagicWeaponBonusLogic : WeaponEnchantmentLogic, IInitiatorRulebookHandler<RuleCalculateWeaponStats>, IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>
    {
        public int EnhancementBonus;

        public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            if (evt.Weapon != Owner) return;
            evt.AddBonusDamage(EnhancementBonus);
            evt.Enhancement += 1;
        }

        public void OnEventDidTrigger(RuleCalculateWeaponStats evt) { }

        public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
        {
            if (evt.Weapon != Owner) return;
            evt.AddBonus(EnhancementBonus, Fact);
        }

        public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt) { }
    }

    public class WeaponOfAweLogic : WeaponEnchantmentLogic, IInitiatorRulebookHandler<RuleCalculateWeaponStats>, IInitiatorRulebookHandler<RuleAttackWithWeapon>
    {
        public int Bonus = 2;
        public BlueprintBuff Buff;

        public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            if (evt.Weapon != Owner) return;
            evt.AddTemporaryModifier(evt.Initiator.Stats.AdditionalDamage.AddModifier(Bonus, this, ModifierDescriptor.Sacred));
        }

        public void OnEventAboutToTrigger(RuleAttackWithWeapon evt) { }

        public void OnEventDidTrigger(RuleCalculateWeaponStats evt) { }

        public void OnEventDidTrigger(RuleAttackWithWeapon evt)
        {
            if (evt.Weapon == Owner && evt.AttackRoll.IsCriticalConfirmed && !evt.AttackRoll.FortificationNegatesCriticalHit)
            {
                evt.Target.Descriptor.AddBuff(Buff, evt.Initiator, 1.Rounds().Seconds);
            }
        }
    }

    public class ContextActionRangedTouchAttack : ContextAction
    {
        public BlueprintItemWeapon Weapon;

        public ActionList OnHit, OnMiss;

        internal static ContextActionRangedTouchAttack Create(GameAction[] onHit, GameAction[] onMiss = null)
        {
            var r = Helpers.Create<ContextActionRangedTouchAttack>();
            r.Weapon = Main.library.Get<BlueprintItemWeapon>("f6ef95b1f7bb52b408a5b345a330ffe8");
            r.OnHit = Helpers.CreateActionList(onHit);
            r.OnMiss = Helpers.CreateActionList(onMiss);
            return r;
        }

        public override string GetCaption() => $"Ranged touch attack";

        public override void RunAction()
        {
            try
            {
                var weapon = Weapon.CreateEntity<ItemEntityWeapon>();
                var context = AbilityContext;
                var attackRoll = context.AttackRoll ?? new RuleAttackRoll(context.MaybeCaster, Target.Unit, weapon, 0);
                attackRoll = context.TriggerRule(attackRoll);
                if (context.ForceAlwaysHit) attackRoll.SetFake(AttackResult.Hit);
                Log.Write($"Ranged touch attack on {Target.Unit}, hit? {attackRoll.IsHit}");
                if (attackRoll.IsHit)
                {
                    OnHit.Run();
                }
                else
                {
                    OnMiss.Run();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }


    // Like AbilityTargetsAround, but actually uses either the main target point,
    // or the caster as the only target.
    //
    // Intended for delayed spells, so they can display the targeting area, even though the
    // initial spell (the delay buff/item) won't actually target them.
    public class FakeTargetsAround : AbilitySelectTarget, IAbilityAoERadiusProvider
    {
        public Feet AoERadius, SpreadSpeed;

        public TargetType Targets;

        public bool TargetCaster;

        public static FakeTargetsAround Create(Feet radius, TargetType targetType = TargetType.Any,
            Feet spreadSpeed = default(Feet), bool toCaster = false)
        {
            var f = Helpers.Create<FakeTargetsAround>();
            f.AoERadius = radius;
            f.Targets = targetType;
            f.SpreadSpeed = spreadSpeed;
            f.TargetCaster = toCaster;
            return f;
        }

        public override IEnumerable<TargetWrapper> Select(AbilityExecutionContext context, TargetWrapper anchor)
        {
            Log.Write($"FakeTargetsAround: anchor at {anchor}");
            return new TargetWrapper[] { TargetCaster ? context.Caster : anchor };
        }

        public override Feet GetSpreadSpeed() => SpreadSpeed;
        Feet IAbilityAoERadiusProvider.AoERadius => AoERadius;
        TargetType IAbilityAoERadiusProvider.Targets => Targets;
    }
}
