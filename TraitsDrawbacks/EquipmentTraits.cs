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
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Shields;

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

            //adventurer provisions
            //armor cloth
            var HalfplateStandard = Traits.library.Get<BlueprintItemArmor>("ed6bbd7ecd050c04690fe11d4c3b3f7d");
            var HalfplateStandartPlus1 = Traits.library.Get<BlueprintItemArmor>("65de3fcad4c01ac40bc8567f67901b5b");//417
            var MithralHalfplate = Traits.library.Get<BlueprintItemArmor>("648d57a5a564f594497d28c596e691d9");
            var MithralChainshirt = Traits.library.Get<BlueprintItemArmor>("5fb6d5f9241cef241a4c878ff30d6bd4");
            var MithralBreastplate = Traits.library.Get<BlueprintItemArmor>("ac740e390d8e3e44ab99e961d660a633");
            //var ArmorRobeItem = Traits.library.Get<BlueprintItemArmor>("a15cc28e6328f024183c7e7a9707304b");//test

            var HeavyShield = Traits.library.Get<BlueprintItemShield>("f4cef3ba1a15b0f4fa7fd66b602ff32b");
            var LightShield = Traits.library.Get<BlueprintItemShield>("a85d51d0fb905f940b951eec60388bac");

            //High quality/price equipment items
            var CloakOfHeroism = Traits.library.Get<BlueprintItemEquipmentShoulders>("dd7e65ef127b930488b85a48616b4baa");//450
            var CloakWyvernItem = Traits.library.Get<BlueprintItemEquipmentShoulders>("b20919f1a98333f449956f0380fae855");//450
            var CloakOfSparklesItem = Traits.library.Get<BlueprintItemEquipmentShoulders>("5dbe5da0779cb7b47b742ac3f56f1086");//450
            var ExplorersBeltItem = Traits.library.Get<BlueprintItemEquipmentBelt>("ec34985b81b9f744488456e7009044ee");//625

            

            //bonus1 items
            var BracersOfArmor1 = Traits.library.Get<BlueprintItemEquipmentWrist>("9482c62934be44044918c3aac3730232");

            

            //usables single use
            var ScrollOfRaiseDead_Prologue = Traits.library.Get<BlueprintItem>("d09ca044d12f6034d88fca09db50946d");//0
            var ScrollOfBreathOfLife = Traits.library.Get<BlueprintItem>("5318313176485d64ca13c1f046591326");//281
            var ScrollOfInspiringRecovery = Traits.library.Get<BlueprintItem>("8f8ee8111d7ac874f8479778c5e3a8d0");//412
            var ScrollOfRemoveCurseWizard = Traits.library.Get<BlueprintItem>("f2b8655382429c14c8c888f1cf109020");//175
            var ScrollOfDetectSecretDoors = Traits.library.Get<BlueprintItem>("5e9bd8e141c622a4a8f4e4654d022f40");
            var ScrollOfProtectionFromEvil = Traits.library.Get<BlueprintItem>("96eb7a498b4db2c4a9fcfb632064b948");
            var ScrollOfStoneskin = Traits.library.Get<BlueprintItem>("a8422c98704e6a0429ebc5a56e132d95");//175
            var ScrollOfSummonNaturesAllyVd3 = Traits.library.Get<BlueprintItem>("2d7298d3e28a00146b90a49f9df0636b");//175
            var ScrollOfFireball = Traits.library.Get<BlueprintItem>("5b172c2c3e356eb43ba5a8f8008a8a5a");//93
            var PotionOfShieldOfFaith = Traits.library.Get<BlueprintItem>("bc93a78d71bef084fa155e529660ed0d");
            var HpPot = Traits.library.Get<BlueprintItem>("d52566ae8cbe8dc4dae977ef51c27d91");//12
            var UnPot = Traits.library.Get<BlueprintItem>("115c4dcc899ce9747a6e97335955092a");//12
            var HpPot2 = Traits.library.Get<BlueprintItem>("f991f3051c3b9e64fabc87891077b613");//75
            var UnPot2 = Traits.library.Get<BlueprintItem>("a9991bfea27096f46bb7b4ccf0fb7eb7");//75
            var HpPot3 = Traits.library.Get<BlueprintItem>("e76d14096063ee041bdb1d13d8172599");//187
            var UnPot3 = Traits.library.Get<BlueprintItem>("5865436f0210e3b4e8f6bb2144fdff7c");//187
            var Acid = Traits.library.Get<BlueprintItem>("4639724c4a9cc9544a2f622b66931658");//12
            var Rat = Traits.library.Get<BlueprintItem>("efa6c2ee9e630384188a50b1ce6600fe");//usefull for all ration

            //wands
            var WandOfMageArmor = Traits.library.Get<BlueprintItem>("021b4a12739c59541922e3857f3fb3a4");//175
            var WandOfCureLightWounds = Traits.library.Get<BlueprintItem>("201dcf730b7185344ba701f91dd405db");//175
            var WandOfSummonMonsterZero = Traits.library.Get<BlueprintItem>("46873fb83627a5142b994d3aeceabfde");//summon murder pony 0 gp

            //weapons
            var MasterworkLightCrossbow = Traits.library.Get<BlueprintItem>("01067a23c0cd2c54eb2d41f139f7fde7");
            var MasterworkLongsword = Traits.library.Get<BlueprintItem>("571c56d11dafbb04094cbaae659974b5");
            var MasterworkCompositeLongbow = Traits.library.Get<BlueprintItem>("4de4658c0e9b0d146b9a08ed6f030f8a");

            //blueprintloot
            var Lantarn = Traits.library.Get<BlueprintItem>("735193761aabd1d48a46051e39f0bee3");//usefull for all bluelantarn
            var GoldenStatuette = Traits.library.Get<BlueprintItem>("8adb329f4ac14694eaec87f42a8684d2");
            var Crowbar = Traits.library.Get<BlueprintItem>("0bd04e1febe11a942baf30c0a8851b36");
            var BundleofPaper = Traits.library.Get<BlueprintItem>("ace260b578df98d4c89cfc6a4e7dc4c9");
            var LargeBagOfHolding = Traits.library.Get<BlueprintItem>("6deaab0e39c6f16468c38ff8a32f57a1");//justforicon
            var SilkySkinRobeItem = Traits.library.Get<BlueprintItem>("a15cc28e6328f024183c7e7a9707304b");//justforicon
            var GoldenToad = Traits.library.Get<BlueprintItem>("4230340464dd7ba49a6469e5409993fe");//250


            int x = 0;
            string[] PackGuids = new string[12];
            for (int i = 0; i < 12; i++)
            {
                x = i + 10;
                PackGuids[i] = $"d{x}eb67b2f064321a32196226dcbc{x}6";
            }


            var WellProvisionedTrait = Helpers.CreateFeatureSelection("ProvisionedAdventurerTrait", "Well-Provisioned Adventurer",
                "You always knew you’d leave your humble beginnings behind and become an adventurer, so you scrimped and saved, buying the necessary equipment one piece at a time until you had everything you needed.Select one of the equipment packages in the section that just popped up.\n" +
                "Benefit: You start the game with a backpack full of equipment with an averidge combined worth of 900 gp",
                PackGuids[0],
                LargeBagOfHolding.Icon,
                FeatureGroup.None);

            var EquipmentPacks = new BlueprintFeature[]
            {
            Helpers.CreateFeature("ProvisionedAdventurerTraitStandard", "Team package",
                "You get some versitile items that come in handy in almost every party." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A mithral chainshirt,a wand of cure light wounds,a masterwork crossbow, two healthpotions, three rations, a lantarn, and some paper to write on.",
                PackGuids[1],
                MithralChainshirt.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { MithralChainshirt,WandOfCureLightWounds,MasterworkLightCrossbow,HpPot,HpPot2,Rat,Rat,Rat,BundleofPaper,Lantarn};
                })),
            
            
            //arcane
            Helpers.CreateFeature("ProvisionedAdventurerTraitArcane", "Arcane package",
                "The arcane adept has collected useful magical gear to ensure her success on her adventures, and she prides herself on having just the right tool for the job. This equipment package is appropriate for an arcanist, sorcerer, witch, or wizard. Some bards and summoners might also find it attractive. This equipment package can also work for a magus." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A wyvernCloack,a wand of mage armor,a masterwork crossbow,a scroll of Detect secret doors, two healthpotions and three rations,a lantarn and two pots of acid.",
                PackGuids[2],
                SilkySkinRobeItem.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] {CloakWyvernItem,WandOfMageArmor,ScrollOfDetectSecretDoors,MasterworkLightCrossbow,HpPot,HpPot2,Rat,Rat,Rat,Acid,Acid,Lantarn };
                })),
            //demonhunter
            Helpers.CreateFeature("ProvisionedAdventurerTraitWarden", "Blessed Warden Package",
                "A blessed warden is prepared to protect herself against the horrors of evil-aligned planes. This equipment package is suitable for clerics, druids, inquisitors, oracles, and paladins. The specific gear is appropriate for those visiting the chaotic evil Abyss but can be adjusted to suit other evil-aligned planes, such as swapping the scrolls of protection from chaos for scrolls of protection from law for a package designed for travel to Hell." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A mithral chainshirt,a masterwork crossbow,scroll of breath of life, a scroll of remove curse,a scroll of protection from evil, a healthpotion, three rations, and some paper to write on.",
                PackGuids[3],
                MithralChainshirt.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { MithralChainshirt,MasterworkLightCrossbow,ScrollOfBreathOfLife,ScrollOfProtectionFromEvil,ScrollOfRemoveCurseWizard,HpPot,Rat,Rat,Rat,BundleofPaper};
                })),
            //questingknight
            Helpers.CreateFeature("ProvisionedAdventurerTraitQuesting", "Questing Knight Package",
                "If not descended from nobility, the questing knight certainly looks like he fits the part. This equipment package is useful for cavaliers, fighters, and paladins." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A Halfplate armor+1,a masterwork longsword, a heavy wooden shield,three healthpotions,a javelin and three rations",
                PackGuids[4],
                HalfplateStandard.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.SpikedLightShield,WeaponCategory.Javelin };
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { HalfplateStandartPlus1,HeavyShield, MasterworkLongsword,HpPot,HpPot2,HpPot3,Rat,Rat,Rat,Lantarn};
                })),
            //planetair
            Helpers.CreateFeature("ProvisionedAdventurerTraitPlanar", "Planar Traveler Package",
                "This package equipment prepares planar travelers for the challenges of a variety of planar destinations. This package works particularly well for lightly armored combatants, such as bards and rogues." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A sparkly cape with elemental resistances,a masterwork Composite bow,a mace,a flail,an acid flask,three healthpotions,a lantarn and three rations",
                PackGuids[5],
                CloakOfSparklesItem.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.LightMace,WeaponCategory.Flail};
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] {CloakOfSparklesItem , MasterworkCompositeLongbow, HpPot,HpPot2,HpPot3,Rat,Rat,Rat,Acid,Lantarn};
                })),
            //bravo
            Helpers.CreateFeature("ProvisionedAdventurerTraitBravo", "Daring Bravo Package",
                "The daring bravo is equally adept in social situations and combat, with the right equipment to move fluidly from one to the other. Such adventurers are known as much for their flair and panache as their martial prowess, and the daring bravo’s equipment is often ornately ornamented or personalized. This equipment package is good for a bard, fighter, rogue, swashbuckler, or vigilante. Certain cavaliers, investigators, skalds, or even paladins may also find it appealing." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A cape that makes you more heroic, A mithral chainshirt,a crosbow,a rapier,a healthpotion,a lantarn and three rations",
                PackGuids[6],
                CloakOfHeroism.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.LightCrossbow,WeaponCategory.Rapier};
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { CloakOfHeroism, MithralChainshirt, HpPot,Rat,Rat,Rat,Lantarn};
                })),
            //corporeal
            Helpers.CreateFeature("ProvisionedAdventurerTraitCorporeal", "Corporeal Warrior Package",
                "Normal equipment is ill suited to fighting intangible foes, and a warrior bound for the Ethereal Plane must be prepared for incorporeal threats lurking in the ever-present mists. This equipment package is suitable for fighters and paladins." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A wand that lets you summon a Corporeal murder pony once a day,a wand of cure wounds,a wand of mage armor, A masterwork longsword , a heavycrossbow,a healthpotion,a lantarn and three rations",
                PackGuids[7],
                WandOfSummonMonsterZero.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.HeavyCrossbow};
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { WandOfSummonMonsterZero,WandOfMageArmor,WandOfCureLightWounds,MasterworkLongsword, HpPot,Rat,Rat,Rat,Lantarn};
                })),

            Helpers.CreateFeature("ProvisionedAdventurerTraitAdventurer", "Adventurer Package",
                "The adventurer package is a general package for adventurers that do not yet know in what aerea they are going to specialize." +
                "\nBenefit: You start the game with some extra stuff:" +
                " An explorers belt that helps fight of fatige,a wand of mage armor, A masterwork composite bow,a shield ,a scimitar,a healthpotion,a lantarn and three rations",
                PackGuids[8],
                ExplorersBeltItem.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.Scimitar,WeaponCategory.WeaponLightShield};
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] {ExplorersBeltItem,WandOfMageArmor,MasterworkCompositeLongbow, HpPot,Rat,Rat,Rat,Lantarn};
                })),
            
            //holywar
            Helpers.CreateFeature("ProvisionedAdventurerTraitHolywar", "Holy Warrior Package",
                "The holy warrior is prepared to use her might and zeal to take the fight to the enemy, but she understands the importance of having the right equipment to overcome the resistances of her monstrous foes. This equipment package is well-suited to a cleric, inquisitor, paladin, warpriest, or even a fighter with a religious background. It’s especially suited to characters who focus on supporting their party members and making them more effective in a fight." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A mithral brestplate, a masterwork longsword, a heavy shield, three healthpotions, a potion of shield of faith, three rations, and some paper to write on.",
                PackGuids[9],
                MithralBreastplate.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { MithralBreastplate, MasterworkLongsword,HeavyShield,HpPot,HpPot2,HpPot3,PotionOfShieldOfFaith,Rat,Rat,Rat,BundleofPaper};
                })),
            
            //holywar
            Helpers.CreateFeature("ProvisionedAdventurerTraitUnHolywar", "Unholy Warrior Package",
                "The Unholy warrior is prepared to use his might and zeal to take the fight to the enemy, but he understands the importance of having the right equipment to overcome the resistances of her monstrous foes. This equipment package is well-suited to an undead of the following classes: cleric, inquisitor, paladin, warpriest, or even a fighter with a religious background. It’s especially suited to characters who focus on supporting their party members and making them more effective in a fight." +
                "\nBenefit: You start the game with some extra stuff:" +
                " A mithral brestplate, a masterwork longsword, a heavy shield, three potions of heal undead, a potion of shield of faith, three rations, and some paper to write on.",
                PackGuids[10],
                UnPot2.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { MithralBreastplate, MasterworkLongsword,HeavyShield,UnPot,UnPot2,UnPot3,PotionOfShieldOfFaith,Rat,Rat,Rat,BundleofPaper};
                })),

            //Lore seeker package
            Helpers.CreateFeature("ProvisionedAdventurerTraitLore", "Lore Seeker Package",
                "The lore seeker has the equipment necessary to delve into ancient ruins searching for lost knowledge. As this equipment package contains little by way of armor or weapons, it is most appropriate for alchemists, bards, monks, sorcerers, and wizards." +
                "\nBenefit: You start the game with some extra stuff:" +
                "A scroll of raize dead, a scroll of fireball, a handcrossbow, a quarterstaff, three healthpotions, a potion of shield of faith, three rations, and some paper to write on.",
                PackGuids[11],
                ScrollOfRaiseDead_Prologue.Icon,
                FeatureGroup.None, Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.Quarterstaff,WeaponCategory.HandCrossbow,};
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { ScrollOfRaiseDead_Prologue,ScrollOfFireball,HpPot,HpPot2,HpPot3,PotionOfShieldOfFaith,Rat,Rat,Rat,BundleofPaper};
                })),


            };
            

            WellProvisionedTrait.SetFeatures(EquipmentPacks);

            choices.Add(WellProvisionedTrait);



            //weapons and weapon types
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
            var Falchion = Traits.library.Get<BlueprintWeaponType>("6ddc9acbbb6e40746a6a1671df1f7b47");
            var FalchionPlus1 = Traits.library.Get<BlueprintItem>("f93a7882fb01963428b091f259b15d3a");
            var ShurikenPlus1 = Traits.library.Get<BlueprintItem>("1ebf2c4026fbdbf42b319504f6eef772");
            var Shuriken = Traits.library.Get<BlueprintWeaponType>("e54eafe37766a064c98e702fdfa6328f");
            var Fauchard = Traits.library.Get<BlueprintWeaponType>("7a40899c4defec94bb9c291bde74f1a8");
            var FauchardPlus1 = Traits.library.Get<BlueprintItem>("36cadeb70d35fc84a9b69db9fb7e64f5");
            var ScythePlus1 = Traits.library.Get<BlueprintItem>("8933943621eca2d45b99d851bd9100d9");
            var Scythe = Traits.library.Get<BlueprintWeaponType>("4eacfc7e152930a45a1a16217c35011c");

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
                Helpers.CreateFeature("HairloomTraitFalchion", "Family Heirloom Falchion",
                "You grew up in a butchery. When you went of to go adventuring you got your fathers buchers knife: a falchion." +
                "\nBenefit: You get a falchion and you have experience with falchions.",
                "c28eb56b2f964321a30086226dcbc496",
                FalchionPlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { FalchionPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = Falchion; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),
                Helpers.CreateFeature("HairloomTraitShuriken", "Family Heirloom Shurikens",
                "When you grow up in a family of assasins, everyone who leaves the house gets a few shurikens." +
                "\nBenefit: You get shurikens and you have experience with shurikens.",
                "c28eb56b2f964321a32196226dcbc496",
                FalchionPlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { ShurikenPlus1,ShurikenPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = Shuriken; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),
                Helpers.CreateFeature("HairloomTraitFauchard", "Family Heirloom Fauchard",
                "Your father got a fauchard during his time as a defender, now he does not need it anymore so you can have his fauchard." +
                "\nBenefit: You get a fauchard and you have experience with fauchards.",
                "c28eb56b2f942329d32196226dcbc497",
                FauchardPlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { FauchardPlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = Fauchard; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
                ),
                Helpers.CreateFeature("HairloomTraitScythe", "Family Heirloom Scythe",
                "Woman of noble birth are forbidden from carrying any weapon bigger than a dagger." +
                "Your mother just happened to keep a scythe close at all times just in case some weeds needed to be cut short like thorns , wild grasses , Outlaws." +
                "When you went on your journey it was given to you for cutting things." +
                "\nBenefit: You get a scythe and you have experience with scythes.",
                "c28eb56b2f942329d32196226dcbd247",
                ScythePlus1.Icon,
                FeatureGroup.None,Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = Array.Empty<WeaponCategory>();
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = new BlueprintItem[] { ScythePlus1 };
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = Scythe; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })
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