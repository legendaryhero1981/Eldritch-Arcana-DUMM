
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
    internal class SocialTraits
    {

        public static BlueprintFeatureSelection CreateSocialTraits(out BlueprintFeatureSelection adopted)
        {
            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var socialTraits = Helpers.CreateFeatureSelection("SocialTrait", "Social Trait",
                "Social traits focus on your character’s social class or upbringing.",
                "9e41e60c929e45bc84ded046148c07ec", null, FeatureGroup.None, noFeature);
            noFeature.Feature = socialTraits;
            var choices = new List<BlueprintFeature>();

            // This trait is finished by CreateRaceTraits.
            adopted = Helpers.CreateFeatureSelection("AdoptedTrait", "Adopted",
                "You were adopted and raised by someone not of your race, and raised in a society not your own.\nBenefit: As a result, you picked up a race trait from your adoptive parents and society, and may immediately select a race trait from your adoptive parents’ race.",
                "b4b37968273b4782b29d31c0ca215f41",
                Helpers.GetIcon("26a668c5a8c22354bac67bcd42e09a3f"), // Adaptability
                FeatureGroup.None);

            adopted.IgnorePrerequisites = true;
            adopted.Obligatory = true;
            choices.Add(adopted);

            choices.Add(Traits.CreateAddStatBonus("ChildOfTheStreetsTrait", "Child of the Streets",
                "You grew up on the streets of a large city, and as a result you have developed a knack for picking pockets and hiding small objects on your person.",
                "a181fd2561134715a04e1b05776ab7a3",
                StatType.SkillThievery));

            choices.Add(Traits.CreateAddStatBonus("FastTalkerTrait", "Fast-Talker",
                "You had a knack for getting yourself into trouble as a child, and as a result developed a silver tongue at an early age.",
                "509458a5ded54ecd9a2a4ef5388de2b7",
                StatType.SkillPersuasion));

            var LongbowWeapontype = Traits.library.Get<BlueprintWeaponType>("7a1211c05ec2c46428f41e3c0db9423f");
            var LongbowPlus1 = Traits.library.Get<BlueprintItem>("fd732e6688007e449964d8c5f2fc659d");
            var duelingSword = Traits.library.Get<BlueprintWeaponType>("a6f7e3dc443ff114ba68b4648fd33e9f");
            var DuelingSwordPlus1 = Traits.library.Get<BlueprintItem>("66106d59fd4615842af854cc9b7cbea4");
            var SaiWeapontype = Traits.library.Get<BlueprintWeaponType>("0944f411666c7594aa1398a7476ecf7d");
            var SaiPlus1 = Traits.library.Get<BlueprintItem>("096350418ea1f614bb489191398ab64b");
            var longsword = Traits.library.Get<BlueprintWeaponType>("d56c44bc9eb10204c8b386a02c7eed21");
            var ColdIronLongswordPlus1 = Traits.library.Get<BlueprintItem>("203d3fbf74304ec4aba2ed1021742e59");
            var Longspear = Traits.library.Get<BlueprintWeaponType>("fa2dd17cbde7d3f4aa918d467c30516e");
            var LongspearPlus1 = Traits.library.Get<BlueprintItem>("9479af1ecd44ceb47a1e54b7268175c3");
            var Dagger = Traits.library.Get<BlueprintWeaponType>("07cc1a7fceaee5b42b3e43da960fe76d");
            var ColdIronDaggerPlus1 = Traits.library.Get<BlueprintItem>("ebea3139c4649e7459079d424c89e561");

            var FamilyHeirloomTrait =Helpers.CreateFeatureSelection("HairloomTrait", "Family Heirloom Weapon",
                "You inherited a weapon from someone.\nBenefit: You can choose a weapon and you start the game with a +1 variant on you.\nBenefit: When using weapons of this type you have a +1 bonus on attack rolls and combat maneurvers.",
                "e16eb56b2f964321a30086226dccb39e",
                Helpers.NiceIcons(37),
                FeatureGroup.None);
            //int rnd = DateTime.Now.Millisecond % 64;
            var HeirloomWeapons = new BlueprintFeature[]
            {
                //longbow
                Helpers.CreateFeature("HairloomTraitBow", "Family Heirloom Longbow",
                "You inherited this +1 longbow from your uncle who was in the archer assassins division." +
                "\nBenefit: You can shoot better with longbows.",
                "e16eb56b2f964321a30086226dccb390",
                LongbowPlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { LongbowPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = LongbowWeapontype; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),
                //DuelingSword
                Helpers.CreateFeature("HairloomTraitDuel", "Family Heirloom DuelingSword",
                "You inherited this +1 dueling sword from your retired father." +
                "\nBenefit: You can fight better with dueling swords.",
                "e16eb56b2f964321a30086226dccb391",
                DuelingSwordPlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { DuelingSwordPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = duelingSword; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),
                //sai
                Helpers.CreateFeature("HairloomTraitSai", "Family Heirloom Sai",
                "You got this sai from your brother after he heard you were going on an adventure." +
                "\nBenefit: You can fight better with sais.",
                "e16eb56b2f964321a30086226dccb392",
                SaiPlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { SaiPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = SaiWeapontype; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),
                //Longsword
                Helpers.CreateFeature("HairloomTraitLongsword", "Family Heirloom Longsword",
                "You always saw this old iron longsword on display in your mansion and you decided to take it along in your adventure." +
                "\nBenefit: You are practiced with longswords.",
                "e16eb56b2f964321a30086226dccb393",
                ColdIronLongswordPlus1.Icon, // DuelingMastery
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { ColdIronLongswordPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = longsword; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),
                //
                Helpers.CreateFeature("HairloomTraitSpear", "Family Heirloom LongSpear",
                "You inherited this longspear from your grandfather who was a veteran in the army." +
                "\nBenefit: You get a longspear and you are experienced with it.",
                "e16eb56b2f964321a30086226dccb394",
                LongspearPlus1.Icon, 
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { LongspearPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = Longspear; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),
                Helpers.CreateFeature("HairloomTraitDagger", "Family Heirloom Dagger",
                "After catching a poacher on your land your family took care of it and you could have the poachers dagger." +
                "\nBenefit: You get a dagger and you have experience with daggers.",
                "e17eb56b2f964321a30086226dccb395",
                ColdIronDaggerPlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { ColdIronDaggerPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = Dagger; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),

            };
            FamilyHeirloomTrait.SetFeatures(HeirloomWeapons);

            choices.Add(FamilyHeirloomTrait);

            var performanceResource = Traits.library.Get<BlueprintAbilityResource>("e190ba276831b5c4fa28737e5e49e6a6");
            choices.Add(Helpers.CreateFeature("MaestroOfTheSocietyTrait", "Maestro of the Society",
                "The skills of the greatest musicians are at your fingertips, thanks to the vast treasure trove of musical knowledge in the vaults you have access to.\nBenefit: You may use bardic performance 3 additional rounds per day.",
                "847cdf262e4147cda2c670db81852c58",
                Helpers.GetIcon("0d3651b2cb0d89448b112e23214e744e"),
                FeatureGroup.None,
                Helpers.Create<IncreaseResourceAmount>(i => { i.Resource = performanceResource; i.Value = 3; })));

            var gnomeReq = Helpers.PrerequisiteFeature(Helpers.gnome);
            //var performanceResource = Traits.library.Get<BlueprintAbilityResource>("e190ba276831b5c4fa28737e5e49e6a6");
            var MutagenResource = Traits.library.Get<BlueprintAbilityResource>("3b163587f010382408142fc8a97852b6");
            choices.Add(Helpers.CreateFeature("GnomishAlchemistTrait", "Gnomish Alchemist",
                "a Gnomish alchemist discovers how to create a special elixir that he can imbibe in order to heighten his ability This is so potent it can be used an extra time. When consumed, the elixir causes the saboteur’s skin to change color to match the background and causes his hands and feet to secrete a sticky residue.\n" +
                "Benfefit:you can use your mutagen an additinal 2 times per day.",
                "125cdf262e4147cda2c670db81852c69",
                Helpers.GetIcon("0d3651b2cb0d89448b112e23214e744e"),
                FeatureGroup.None,
                Helpers.Create<IncreaseResourceAmount>(i => { i.Resource = MutagenResource; i.Value = 2; }),
                gnomeReq));


            choices.Add(Traits.CreateAddStatBonus("SuspiciousTrait", "Suspicious",
                "You discovered at an early age that someone you trusted, perhaps an older sibling or a parent, had lied to you, and lied often, about something you had taken for granted, leaving you quick to question the claims of others.",
                "2f4e86a9d42547bc85b4c829a47d054c",
                StatType.SkillPerception));

            choices.Add(UndoSelection.Feature.Value);
            socialTraits.SetFeatures(choices);
            return socialTraits;
        }
    }
}
 