
using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Controllers.Combat;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
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
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Class.LevelUp.Actions;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Parts;

namespace EldritchArcana
{
    internal class RegionalTraits
    {
        public static BlueprintFeatureSelection CreateRegionalTraits()
        {
            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var regionalTraits = Helpers.CreateFeatureSelection("RegionalTrait", "Regional Trait",
                "Regional traits are keyed to specific regions, be they large (such as a nation or geographic region) or small (such as a city or a specific mountain). In order to select a regional trait, your PC must have spent at least a year living in that region. At 1st level, you can only select one regional trait (typically the one tied to your character’s place of birth or homeland), despite the number of regions you might wish to write into your character’s background.",
                "6158dd4ad2544c27bc3a9b48c2e8a2ca", null, FeatureGroup.None, noFeature);
            noFeature.Feature = regionalTraits;

            // TODO: more regional traits.

            // Note: use the generic feat names/text to let players RP this as they choose.
            var choices = new List<BlueprintFeature>();


            var signatureSpell = Helpers.CreateFeatureSelection("SignatureSpellTrait", "Signature Spell",
                "You have learned a mystical secret that empowers your spellcasting.\nBenefit: Pick one spell when you choose this trait—from this point on, whenever you cast that spell, you do so at +1 caster level.",
                "7a3dfe274f45432b85361bdbb0a3009b",
                Helpers.GetIcon("fe9220cdc16e5f444a84d85d5fa8e3d5"), // Spell Specialization Progression
                FeatureGroup.None,
                Helpers.Create<IncreaseCasterLevelForSpell>());
            Traits.FillSpellSelection(signatureSpell, 1, 9, Helpers.Create<IncreaseCasterLevelForSpell>());
            choices.Add(signatureSpell);

            var metamagicApprentice = Helpers.CreateFeatureSelection("MetamagicApprenticeTrait", "Metamagic Master",
                "Your ability to alter your spell of choice is greater than expected.\nBenefit: Select one spell of 3rd level or below; when you use the chosen spell with a metamagic feat, it uses one spell slot one level lower than it normally would.\nstarting level is still minimun",
                "00844f940e434033ab826e5ff5930012",
                Helpers.GetIcon("ee7dc126939e4d9438357fbd5980d459"), // Spell Penetration
                FeatureGroup.None);
            Traits.FillSpellSelection(metamagicApprentice, 1, 3, Helpers.Create<ReduceMetamagicCostForSpell>(r => { r.Reduction = 1; r.MaxSpellLevel = 3; }));
            choices.Add(metamagicApprentice);


            choices.Add(Helpers.CreateFeature("BlightedTrait", "Blighted Physiology",
                "Exposure to corruption has altered your body causing you to sprout horrific growths beneath your skin." +
                "\nBenefit: You gain a +1 natural armor bonus to AC, but your body does not work as a normal creature’s would. Anytime you receive magical healing you heal 1 hp less per die.",
                "c50bdfaad65b4028884dd4a74f14e792",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/anatomist.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.AC, 1, ModifierDescriptor.NaturalArmor),
                Helpers.Create<FeyFoundlingLogic>(s => { s.dieModefier = -1; s.flatModefier = 0; })));

            choices.Add(Helpers.CreateFeature("WanderlustTrait", "Wanderlust",
                "Your childhood was brightened by the new places you constantly saw as you traveled with your parents, who were merchants. Still excited by travel, you gain great energy when traveling overland." +
                "\nBenefit: Treat your base land speed as 10 feet higher when determining your overland speed.",
                "d40bdfaad65b4028884dd4a74f14e793",
                Helpers.NiceIcons(0),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.Speed, 10, ModifierDescriptor.Insight)));




            var dagger = Traits.library.Get<BlueprintWeaponType>("07cc1a7fceaee5b42b3e43da960fe76d");

            var riverrat = Traits.CreateAddStatBonus("DaggerboyTrait", "River Rat (Marsh or River)",
                "You learned to swim right after you learned to walk. When you were a youth, a gang of river pirates put you to work swimming in night-time rivers. And canals with a dagger between your teeth so you could sever the anchor ropes of merchant vessels. \n Benefit: You gain a +1 trait bonus on damage rolls with a dagger and a +1 trait bonus on Swim(atletics is class skill) checks. and you start with a dagger.",
                "e16eb56b2f964321a29976226dccb39f",
                StatType.SkillAthletics // strongman

                );
            //riverrat.Icon = Helpers.NiceIcons(38);
            /*
            var riverratextra = Helpers.CreateFeature("AtleticsTrait", "Swimmer",
                "Your swimming made you athletic",
                "EB6BC4BF90B1433C80878C9D0C81AAED", Helpers.GetSkillFocus(StatType.SkillAthletics).Icon,
                FeatureGroup.None,
                Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.Dagger, WeaponCategory.Dagger };
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = Array.Empty<BlueprintItem>();
                }),
                //Helpers.CreateAddStatBonus(StatType.SkillAthletics,1,ModifierDescriptor.Trait),
                //,
                Helpers.Create<WeaponTypeDamageBonus>(a => { a.WeaponType = dagger; a.DamageBonus = 1; })
                //Helpers.Create<WeaponCategoryAttackBonus>(a => { a.Category = WeaponCategory.Dagger; a.AttackBonus = 1; })
                );
                */
            
            riverrat.AddComponent(Helpers.Create<WeaponTypeDamageBonus>(a => { a.WeaponType = dagger; a.DamageBonus = 1; }));
            riverrat.AddComponent(Helpers.Create<AddStartingEquipment>(a =>
            {
                a.CategoryItems = new WeaponCategory[] { WeaponCategory.Dagger, WeaponCategory.Dagger };
                a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                a.BasicItems = Array.Empty<BlueprintItem>();
            }));
            choices.Add(riverrat);
            //WeaponCategoryAttackBonus

            choices.Add(Helpers.CreateFeature("EmpathicDiplomatTrait", "Empathic Diplomat",
                "You have long followed the path of common sense and empathic insight when using diplomacy. \n" +
                "Benefit:You modify your Diplomacy checks using your Wisdom modifier, not your Charisma modifier.",
                "a987f5e69db44cdd88983985e37a6d2b",
                Helpers.NiceIcons(999), // Weapon Specialization
                FeatureGroup.None,
                //dwarfReq,
                Helpers.Create<ReplaceBaseStatForStatTypeLogic>(x =>
                {
                    x.StatTypeToReplaceBastStatFor = StatType.SkillPersuasion;
                    x.NewBaseStatType = StatType.Wisdom;
                })));

            var BruisingInt = Traits.CreateAddStatBonus("BruisingIntellectTrait", "Bruising Intellect",
               "Your sharp intellect and rapier-like wit bruise egos. \n" +
                "Benefits: Intimidate is always a class skill for you, and you may use your Intelligence modifier when making Intimidate checks instead of your Charisma modifier.",
                "b222b5e69db44cdd88983985e37a6d2f",
                StatType.SkillPersuasion
                );

            BruisingInt.AddComponent(Helpers.Create<ReplaceBaseStatForStatTypeLogic>(x =>
            {
                x.StatTypeToReplaceBastStatFor = StatType.SkillPersuasion;
                x.NewBaseStatType = StatType.Intelligence;
            }));

            choices.Add(BruisingInt);

            choices.Add(UndoSelection.Feature.Value);
            regionalTraits.SetFeatures(choices);
            return regionalTraits;
        }
    }
}