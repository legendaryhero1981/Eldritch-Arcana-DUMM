using System;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes;
using System.Collections.Generic;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Items;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Enums;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Blueprints.Classes.Spells;

namespace EldritchArcana
{
    internal class EquipmentTraits
    {
        public static BlueprintFeatureSelection CreateEquipmentTraits()
        {
            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var EquipmentTraits = Helpers.CreateFeatureSelection("EquipmentTrait", "Equipment Trait",
                "Many adventurers come to rely on certain gear to the extent that the equipment and the adventurer each become something more when the other is present. The symbiosis between adventurers and their gear is varied and complex. Below are several traits and feats that help characters make the most of their equipment or use their equipment to make the most of their skills.",
                "2c32f22c222c45bc84ded746148c07ee", null, FeatureGroup.None, noFeature);
            noFeature.Feature = EquipmentTraits;
            var choices = new List<BlueprintFeature>();


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
            var ScimitarPlus1 = Traits.library.Get<BlueprintItem>("7bef1327d4cb31249823d9fcb24af332");
            var Scimitar = Traits.library.Get<BlueprintWeaponType>("d9fbec4637d71bd4ebc977628de3daf3");

            var FamilyHeirloomTrait = Helpers.CreateFeatureSelection("HairloomTrait", "Family Heirloom Weapon",
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
                Helpers.CreateFeature("HairloomTraitScimitar", "Family Heirloom Scimitar",
                "Your family got rich after finding a treasure on a small island on the sea. they found a skeleton with a pirate hat and a scimitar. You could have the scimitar for your adventure." +
                "\nBenefit: You get a scimitar and you have experience with scimitars.",
                "e18eb56b2f964321a30086226dcbb396",
                ScimitarPlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { ScimitarPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = Scimitar; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),

            };
            FamilyHeirloomTrait.SetFeatures(HeirloomWeapons);

            choices.Add(FamilyHeirloomTrait);

            choices.Add(Helpers.CreateFeature("IronLiverTrait", "Iron Liver",
                "Due to a lucky constitution or frequent exposure, your body is resistant to poison, including alcohol and drugs." +
                "\nBenefit: You gain a +2 trait bonus on Fortitude saves against poison and drugs, and a +4 trait bonus on Fortitude saves to avoid the effects of alcohol.",
                "ccc555c0789d43a2b6cfad26aeda3410",
                Helpers.GetIcon("2483a523984f44944a7cf157b21bf79c"), // Elven Immunities
                FeatureGroup.None,
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(a =>
                {
                    a.SpellDescriptor = SpellDescriptor.Poison;
                    a.Value = 2;
                    a.ModifierDescriptor = ModifierDescriptor.Trait;
                })));




            /*

            Rough and Ready
            Benefit: When you use a tool of your trade (requiring at least 1 rank in the appropriate Craft or Profession skill) as a weapon, you do not take the improvised weapon penalty and instead receive a +1 trait bonus on your attack. This trait is commonly used with shovels, picks, blacksmith hammers, and other sturdy tools — lutes and brooms make terribly fragile weapons. 

            Well-Provisioned Adventurer
            This is an Equipment trait, a category of trait (like Combat, Magic, or Social). Alternatively, a PC can purchase an equipment package for 1,000 gp, or she might receive an equipment package as a reward from a wealthy NPC in exchange for a valuable service.
            This trait allows a PC to select one of the following equipment packages instead of spending starting gold.


            */

            choices.Add(UndoSelection.Feature.Value);
            EquipmentTraits.SetFeatures(choices);
            return EquipmentTraits;
        }
    }
}