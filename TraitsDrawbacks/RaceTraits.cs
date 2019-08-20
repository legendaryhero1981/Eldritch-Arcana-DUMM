
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
    internal class RaceTraits
    {

        public static BlueprintFeatureSelection CreateRaceTraits(BlueprintFeatureSelection adopted)
        {
            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var raceTraits = Helpers.CreateFeatureSelection("RaceTrait", "Race Trait",
                "Race traits are keyed to specific races or ethnicities, which your character must belong to in order to select the trait.",
                "6264aa9515be40cda55892da93685764", null, FeatureGroup.None,
                Helpers.PrerequisiteNoFeature(adopted), noFeature);
            noFeature.Feature = raceTraits;

            var humanReq = Helpers.PrerequisiteFeaturesFromList(Helpers.human, Helpers.halfElf, Helpers.halfOrc,
                // Note: Aasimar/Tiefling included under the assumption they have "Scion of Humanity"/"Pass for Human"
                Helpers.aasimar, Helpers.tiefling);

            var halfElfReq = Helpers.PrerequisiteFeature(Helpers.halfElf);
            var halfOrcReq = Helpers.PrerequisiteFeature(Helpers.halfOrc);
            var elfReq = Helpers.PrerequisiteFeaturesFromList(Helpers.elf, Helpers.halfElf);
            var dwarfReq = Helpers.PrerequisiteFeature(Helpers.dwarf);
            var halflingReq = Helpers.PrerequisiteFeature(Helpers.halfling);
            var gnomeReq = Helpers.PrerequisiteFeature(Helpers.gnome);
            var aasimarReq = Helpers.PrerequisiteFeature(Helpers.aasimar);
            var tieflingReq = Helpers.PrerequisiteFeature(Helpers.tiefling);

            // TODO: how do we code prerequisites so they aren't ignored by "Adopted"?
            // (only race prereq should be ignored, not others)
            //
            // Note: half-elf, half-orc can take traits from either race.
            // Also Aasimar/Tiefling are treated as having Scion of Humanity/Pass for Human in the game.
            var choices = new List<BlueprintFeature>();

            // Human:
            // - Carefully Hidden (+1 will save, +2 vs divination)
            // - Fanatic (Arcana)
            // - Historian (World and +1 bardic knowledge if Bard)
            // - Shield Bearer (+1 dmg shield bash)
            // - Superstitious (+1 save arcane spells)
            // - World Traveler (choose: persuasion, perception, or world)

            var components = new List<BlueprintComponent> { humanReq };
            components.Add(Helpers.CreateAddStatBonus(StatType.SaveWill, 1, ModifierDescriptor.Trait));
            components.Add(Helpers.Create<SavingThrowBonusAgainstSchool>(a =>
            {
                a.School = SpellSchool.Divination;
                a.Value = 2;
                a.ModifierDescriptor = ModifierDescriptor.Trait;
            }));
            choices.Add(Helpers.CreateFeature("CarefullyHiddenTrait", "Carefully Hidden (Human)",
                "Your life as a member of an unpopular ethnic group has given you an uncanny knack for avoiding detection.\nBenefit: You gain a +1 trait bonus to Will saves and a +2 trait bonus to saving throws versus divination effects.",
                "38b92d2ebb4c4cdb8e946e29f5b2f178",
                Helpers.GetIcon("175d1577bb6c9a04baf88eec99c66334"), // Iron Will
                FeatureGroup.None,
                components.ToArray()));
            choices.Add(Traits.CreateAddStatBonus("FanaticTrait", "Fanatic (Human)",
                "Your years spent in libraries reading every musty tome you could find about ancient lost civilizations has given you insight into the subjects of history and the arcane.",
                "6427e81ba399406c93b463c284a42055",
                StatType.SkillKnowledgeArcana,
                humanReq));

            var bardicKnowledge =Traits.library.Get<BlueprintFeature>("65cff8410a336654486c98fd3bacd8c5");
            components.Clear();
            components.Add(humanReq);
            components.AddRange((new StatType[] {
                StatType.SkillKnowledgeArcana,
                    StatType.SkillKnowledgeWorld,
                    StatType.SkillLoreNature,
                    StatType.SkillLoreReligion,
            }).Select((skill) => Helpers.Create<AddStatBonusIfHasFact>(a =>
            {
                a.Stat = skill;
                a.Value = 1;
                a.CheckedFact = bardicKnowledge;
                a.Descriptor = ModifierDescriptor.UntypedStackable;
            })));

            var historian = Traits.CreateAddStatBonus("HistorianTrait", "Historian (Human)",
                "Your parents were scholars of history, whether genealogists of your own family tree, sages on the subject of ancient empires, or simply hobbyists with a deep and abiding love for the past.\nBenefits: You gain a +1 trait bonus on Knowledge (history) checks and bardic knowledge checks, and Knowledge (history) is always a class skill for you.",
                "4af3871899e4440bae03d4c33d4b52fd",
                StatType.SkillKnowledgeWorld,
                components.ToArray());
            choices.Add(historian);

            components.Clear();
            components.Add(humanReq);
            components.AddRange(new String[] {
                "98a0dc03586a6d04791901c41700e516", // SpikedLightShield
                "1fd965e522502fe479fdd423cca07684", // WeaponLightShield
                "a1b85d048fb5003438f34356df938a9f", // SpikedHeavyShield
                "be9b6408e6101cb4997a8996484baf19"  // WeaponHeavyShield
            }.Select(id => Helpers.Create<WeaponTypeDamageBonus>(w => { w.DamageBonus = 1; w.WeaponType =Traits.library.Get<BlueprintWeaponType>(id); })));

            choices.Add(Helpers.CreateFeature("ShieldBearerTrait", "Shield Bearer (Human)",
                "You have survived many battles thanks to your skill with your shield.\nBenefit: When performing a shield bash, you deal 1 additional point of damage.",
                "044ebbbadfba4d58afa11bfbf38df199",
                Helpers.GetIcon("121811173a614534e8720d7550aae253"), // Shield Bash
                FeatureGroup.None,
                components.ToArray()));

            choices.Add(Helpers.CreateFeature("SuperstitiousTrait", "Superstitious (Human)",
                "You have a healthy fear of sorcerers’ speech and wizards’ words that has helped you to survive their charms.\nBenefit: You gain a +1 trait bonus on saving throws against arcane spells.",
                "f5d79e5fbb87473ca0b13ed15b742079",
                Helpers.GetIcon("2483a523984f44944a7cf157b21bf79c"), // Elven Immunities
                FeatureGroup.None,
                humanReq,
                Helpers.Create<SavingThrowBonusAgainstSpellSource>()));

            var travelerDescription = "Your family has taken the love of travel to an extreme, roaming the world extensively. You’ve seen dozens of cultures and have learned to appreciate the diversity of what the world has to offer.";
            var worldTraveler = Helpers.CreateFeatureSelection("WorldTravelerTrait", "World Traveler (Human)",
                travelerDescription + "\nBenefits: Select one of the following skills: Persuasion, Knowledge (world), or Perception. You gain a +1 trait bonus on checks with that skill, and it is always a class skill for you.",
                "ecacfcbeddfe453cafc8d60fc1db7d34",
                Helpers.GetIcon("3adf9274a210b164cb68f472dc1e4544"), // Human Skilled
                FeatureGroup.None,
                humanReq);

            var travelerFeats = new StatType[] {
                StatType.SkillPersuasion,
                StatType.SkillKnowledgeWorld,
                StatType.SkillPerception
            }.Select(skill => Traits.CreateAddStatBonus(
                $"WorldTraveler{skill}Trait",
                $"World Traveler — {UIUtility.GetStatText(skill)}",
                travelerDescription,
                Helpers.MergeIds(Helpers.GetSkillFocus(skill).AssetGuid, "9b03b7ff17394007a3fbec18aa42604b"),
                skill)).ToArray();
            worldTraveler.SetFeatures(travelerFeats);
            choices.Add(worldTraveler);

            // Elf:
            // - Dilettante Artist (persuasion)
            // - Forlorn (+1 fort save)
            // - Warrior of the Old (+2 init)
            // - Youthful Mischief (+1 ref)
            choices.Add(Traits.CreateAddStatBonus("DilettanteArtistTrait", "Dilettante Artist (Elf)",
                "Art for you is a social gateway and you use it to influence and penetrate high society.",
                "ac5a16e72ef74b4884c674dcbb61692c", StatType.SkillPersuasion, elfReq));

            BlueprintItemWeapon bite = Traits.library.CopyAndAdd<BlueprintItemWeapon>("35dfad6517f401145af54111be04d6cf", "Tusked",
                "44dfad6517f401145af54111be04d644");
            
                

            choices.Add(Helpers.CreateFeature("ForlornTrait", "Forlorn (Elf)",
                "Having lived outside of traditional elf society for much or all of your life, you know the world can be cruel, dangerous, and unforgiving of the weak.\nBenefit: You gain a +1 trait bonus on Fortitude saving throws.",
                "1511289c92ea4233b14c4f51072ea10f",
                Helpers.GetIcon("79042cb55f030614ea29956177977c52"), // Great Fortitude
                FeatureGroup.None,
                elfReq,
                Helpers.CreateAddStatBonus(StatType.SaveFortitude, 1, ModifierDescriptor.Trait)
                ));

            choices.Add(Helpers.CreateFeature("TuskedTrait", "Tusked (Half-orc)",
                "Benefit: Huge, sharp tusks bulge from your mouth, and you receive a bite attack (1d4 damage for Medium characters). If used as part of a full attack action, the bite attack is made at your full base attack bonus –5.",
                "1511289c92ea4233b14c4f51072ea09g",
                bite.Icon, // Great Fortitude
                FeatureGroup.None,
                halfOrcReq,
                Helpers.Create<AddAdditionalLimb>(x => x.Weapon = bite)
                ));

            choices.Add(Helpers.CreateFeature("WarriorOfOldTrait", "Warrior of Old (Elf)",
                "As a child, you put in long hours on combat drills, and though time has made this training a dim memory, you still have a knack for quickly responding to trouble.\nBenefit: You gain a +2 trait bonus on initiative checks.",
                "dc36a2c52abb4e6dbff549ac65a5a171",
                Helpers.GetIcon("797f25d709f559546b29e7bcb181cc74"), // Improved Initiative
                FeatureGroup.None,
                elfReq,
                Helpers.CreateAddStatBonus(StatType.Initiative, 2, ModifierDescriptor.Trait)));

            choices.Add(Helpers.CreateFeature("YouthfulMischiefTrait", "Youthful Mischeif (Elf)",
                "Though you gave up the life of a padfoot, scout, or minstrel decades before, you still know how to roll with the punches when things turn sour.\nBenefit: You gain a +1 trait bonus on Reflex saves.",
                "bfcc574d1f214455ac369fa46e07200e",
                Helpers.GetIcon("15e7da6645a7f3d41bdad7c8c4b9de1e"), // Lightning Reflexes
                FeatureGroup.None,
                elfReq,
                Helpers.CreateAddStatBonus(StatType.SaveReflex, 1, ModifierDescriptor.Trait)));

            // Half-orc:
            // - Brute (persuasion)
            // - Legacy of Sand (+1 will save)
            var brute = Traits.CreateAddStatBonus("BruteTrait", "Brute (Half-Orc)",
                "You have worked for a crime lord, either as a low-level enforcer or as a guard, and are adept at frightening away people.",
                "1ee0ce55ace74ccbb798e2fdc13181f6", StatType.SkillPersuasion, halfOrcReq);
            brute.SetIcon(Helpers.GetIcon("885f478dff2e39442a0f64ceea6339c9")); // Intimidating
            choices.Add(brute);

            var GloryOfOld = Helpers.CreateFeature("GloryOfOldTrait", "Glory of old",
                "You are part of the old Guard" +
                "\nYou belong to the elite veteran regiments of The old king and his army and are intensely loyal to him. It was you who made the last charge at the dwarven kingdom." +
                "Benefit: You receive a +1 trait bonus on saving throws against spells, spell-like abilities, and poison",
                "4283a523984f44944a7cf157b21bf7c9",
                Helpers.NiceIcons(41),
                FeatureGroup.None,
                dwarfReq,                
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Poison; s.Value = 1; s.ModifierDescriptor = ModifierDescriptor.Racial; }),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.BreathWeapon; s.Value = 1; s.ModifierDescriptor = ModifierDescriptor.Trait; }));
            components.Clear();
            components.AddRange((new SpellSchool[]
            {
                SpellSchool.Abjuration,
                SpellSchool.Conjuration,
                SpellSchool.Divination,
                SpellSchool.Enchantment,
                SpellSchool.Evocation,
                SpellSchool.Illusion,
                SpellSchool.Necromancy,
                SpellSchool.Transmutation,
                SpellSchool.Universalist
            }).Select((school) => Helpers.Create<SavingThrowBonusAgainstSchool>(a =>
            {
                a.School = school;
                a.Value = 1;
                a.ModifierDescriptor = ModifierDescriptor.Racial;
            })));

            GloryOfOld.AddComponents(components);







            choices.Add(Helpers.CreateFeature("LegacyOfSandTrait", "Legacy of Sand (Half-Orc)",
                "A large tribe of orcs adapted to life in the desert once dwelt in southeastern Katapesh. Although this tribe is long extinct, some half-orcs of Katapesh carry the traits of this tribe in their particularly large jaws, broad shoulders, and shockingly pale eyes. You often have dreams of hunts and strange ceremonies held under moonlight in the desert sands. Some ascribe these dreams to racial memory, others to visions or prophecies. These dreams have instilled in you a fierce sense of tradition.\nBenefit: You gain a +1 trait bonus on all Will saving throws.",
                "e5fb1675eb6e4ef9accef7eb3a10862a",
                Helpers.GetIcon("175d1577bb6c9a04baf88eec99c66334"), // , // Iron Will
                FeatureGroup.None,
                halfOrcReq,
                Helpers.CreateAddStatBonus(StatType.SaveWill, 1, ModifierDescriptor.Trait)));

            // Half-elf:
            // - Elven Relexes (+2 initiative)
            // - Failed Apprentice (+1 save arcane spells)
            choices.Add(Helpers.CreateFeature("ElvenReflexsTrait", "Elven Reflexes (Half-Elf)",
                "One of your parents was a member of a wild elven tribe, and you’ve inherited a portion of your elven parent’s quick reflexes.\nBenefit: You gain a +2 trait bonus on initiative checks.",
                "9975678ce2fc420da9cd6ec4fe8c8b9b",
                Helpers.GetIcon("797f25d709f559546b29e7bcb181cc74"), // Improved Initiative
                FeatureGroup.None,
                halfElfReq,
                Helpers.CreateAddStatBonus(StatType.Initiative, 2, ModifierDescriptor.Trait)));

            choices.Add(Helpers.CreateFeature("FailedAprenticeTrait", "Failed Apprentice (Half-Elf)",
                "You have a healthy fear of sorcerers’ speech and wizards’ words that has helped you to survivAs a child, your parents sent you to a distant wizard’s tower as an apprentice so that you could learn the arcane arts. Unfortunately, you had no arcane talent whatsoever, though you did learn a great deal about the workings of spells and how to resist them.\nBenefit: You gain a +1 trait bonus on saves against arcane spells.",
                "8ed66066751f43c2920055dd6358adc8",
                Helpers.GetIcon("2483a523984f44944a7cf157b21bf79c"), // Elven Immunities
                FeatureGroup.None,
                halfElfReq,
                Helpers.Create<SavingThrowBonusAgainstSpellSource>()));

            // Halfling:
            // - Freed Slave (world)
            // - Freedom Fighter (mobility)
            // - Well-Informed (persuasion)
            choices.Add(Traits.CreateAddStatBonus("FreedSlaveTrait", "Freed Slave (Halfling)",
                "You grew up as a slave and know the ins and outs of nobility better than most.",
                "d2fc5fe0c64142a79e0ebee18f14b0be", StatType.SkillKnowledgeWorld, halflingReq));
            choices.Add(Traits.CreateAddStatBonus("FreedomFighterTrait", "Freedom Fighter (Halfling)",
                "Your parents allowed escaping slaves to hide in your home, and the stories you’ve heard from them instilled into you a deep loathing of slavery, and a desire to help slaves evade capture and escape.",
                "3a4d2cd14dc446319085c865570ccc3d", StatType.SkillMobility, halflingReq));
            choices.Add(Traits.CreateAddStatBonus("WellInformedTrait", "Well-Informed (Halfling)",
                "You make it a point to know everyone and to be connected to everything around you. You frequent the best taverns, attend all of the right events, and graciously help anyone who needs it.",
                "940ced5d41594b9aa22ee22217fbd46f", StatType.SkillPersuasion, halflingReq));

            // Dwarf:
            // - Grounded (+2 mobility, +1 reflex)
            // - Militant Merchant (perception)Owner.HPLeft
            // - Ruthless (+1 confirm crits)
            // - Zest for Battle (+1 trait dmg if has morale attack bonus)
            choices.Add(Helpers.CreateFeature("GroundedTrait", "Grounded (Dwarf)",
                "You are well balanced, both physically and mentally.\nBenefit: You gain a +2 trait bonus on Mobility checks, and a +1 trait bonus on Reflex saves.",
                "9b13923527a64c3bbf8de904c5a9ef8b",
                Helpers.GetIcon("3a8d34905eae4a74892aae37df3352b9"), // Skill Focus Stealth (mobility)
                FeatureGroup.None,
                dwarfReq,
                Helpers.CreateAddStatBonus(StatType.SkillMobility, 2, ModifierDescriptor.Racial),
                Helpers.CreateAddStatBonus(StatType.SaveReflex, 1, ModifierDescriptor.Racial)));

            choices.Add(Traits.CreateAddStatBonus("MilitantMerchantTrait", "Militant Merchant (Dwarf)",
                "You know what it takes to get your goods to market and will stop at nothing to protect your products. Years of fending off thieves, cutthroats, and brigands have given you a sixth sense when it comes to danger.",
                "38226f4ad9ed4211878ef95497d01857", StatType.SkillPerception, dwarfReq));

            choices.Add(Helpers.CreateFeature("RuthlessTrait", "Ruthless (Dwarf)",
                "You never hesitate to strike a killing blow.\nBenefit: You gain a +1 trait bonus on attack rolls to confirm critical hits.",
                "58d18289cb7f4ad4a690d9502d397a3a",
                Helpers.GetIcon("f4201c85a991369408740c6888362e20"), // Improved Critical
                FeatureGroup.None,
                dwarfReq,
                Helpers.Create<CriticalConfirmationBonus>(a => { a.Bonus = 1; a.Value = 0; })));

            
            var Frostborn = Helpers.CreateFeature("FrostbornTrait", "Frostborn(Dwarf)",
                "Your where raized in the icy tundra\nBenefit:Benefit: You gain a +4 trait bonus to resist the effects of cold environments, as well as a +1 trait bonus on all saving throws against cold effects.",
                "f987f5e69db44cdd99983985e37a6c3c",
                Helpers.GetIcon("121811173a614534e8720d7550aae253"), // Weapon Specialization
                FeatureGroup.None,
                dwarfReq);
            Frostborn.AddComponent(Helpers.Create<AddDamageResistanceEnergy>(r=> { r.Type = Kingmaker.Enums.Damage.DamageEnergyType.Cold; r.Value = 4; }));
            Frostborn.AddComponent(Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Cold; s.ModifierDescriptor=ModifierDescriptor.Racial; s.Bonus = 1; }));
            choices.Add(Frostborn);

            choices.Add(Helpers.CreateFeature("ZestForBattleTrait", "Zest for Battle (Dwarf)",
                "Your greatest joy is being in the thick of battle, and smiting your enemies for a righteous or even dastardly cause.\nBenefit: Whenever you have a morale bonus to weapon attack rolls, you also receive a +1 trait bonus on weapon damage rolls.",
                "a987f5e69db44cdd98983985e37a6c2a",
                Helpers.GetIcon("31470b17e8446ae4ea0dacd6c5817d86"), // Weapon Specialization
                FeatureGroup.None,
                dwarfReq,
                Helpers.Create<DamageBonusIfMoraleBonus>()));

            // Gnome:
            // - Animal Friend (+1 will save and lore nature class skill, must have familar or animal companion)
            // - Rapscallion (+1 init, +1 thievery)
            components.Clear();
            components.Add(gnomeReq);
            components.Add(Helpers.Create<AddClassSkill>(a => a.Skill = StatType.SkillLoreNature));
            // TODO: is there a cleaner way to implement this rather than a hard coded list?
            // (Ideally: it should work if a party NPC has a familiar/animal companion too.)
            // See also: PrerequisitePet.
            components.AddRange((new String[] {
                // Animal companions
                "f6f1cdcc404f10c4493dc1e51208fd6f",
                "afb817d80b843cc4fa7b12289e6ebe3d",
                "f9ef7717531f5914a9b6ecacfad63f46",
                "f894e003d31461f48a02f5caec4e3359",
                "e992949eba096644784592dc7f51a5c7",
                "aa92fea676be33d4dafd176d699d7996",
                "2ee2ba60850dd064e8b98bf5c2c946ba",
                "6adc3aab7cde56b40aa189a797254271",
                "ece6bde3dfc76ba4791376428e70621a",
                "126712ef923ab204983d6f107629c895",
                "67a9dc42b15d0954ca4689b13e8dedea",
                // Familiars
                "1cb0b559ca2e31e4d9dc65de012fa82f",
                "791d888c3f87da042a0a4d0f5c43641c",
                "1bbca102706408b4cb97281c984be5d5",
                "f111037444d5b6243bbbeb7fc9056ed3",
                "7ba93e2b888a3bd4ba5795ae001049f8",
                "97dff21a036e80948b07097ad3df2b30",
                "952f342f26e2a27468a7826da426f3e7",
                "61aeb92c176193e48b0c9c50294ab290",
                "5551dd90b1480e84a9caf4c5fd5adf65",
                "adf124729a6e01f4aaf746abbed9901d",
                "4d48365690ea9a746a74d19c31562788",
                "689b16790354c4c4c9b0f671f68d85fc",
                "3c0b706c526e0654b8af90ded235a089",
            }).Select(id => Helpers.Create<AddStatBonusIfHasFact>(a =>
            {
                a.Stat = StatType.SaveWill;
                a.Value = 1;
                a.Descriptor = ModifierDescriptor.Trait;
                a.CheckedFact =Traits.library.Get<BlueprintFeature>(id);
            })));

            choices.Add(Helpers.CreateFeature("AnimalFriendTrait", "Animal Friend (Gnome)",
                "You’ve long been a friend to animals, and feel safer when animals are nearby.\nBenefits: You gain a +1 trait bonus on Will saving throws as long as you have an animal companion or familiar, and Lore (Nature) is always a class skill for you.",
                "91c612b225d54adaa4ce4c633501b58e",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/Icon_Gnome_Animal_Friend.png"),//Helpers.GetIcon("1670990255e4fe948a863bafd5dbda5d"), // Boon Companion
                FeatureGroup.None,
                components.ToArray()));

            choices.Add(Helpers.CreateFeature("Rapscallion", "Rapscallion (Gnome)",
                "You’ve spent your entire life thumbing your nose at the establishment and take pride in your run-ins with the law. Somehow, despite all your mischievous behavior, you’ve never been caught.\nBenefits: You gain a +1 trait bonus on Mobility checks and a +1 trait bonus on initiative checks.",
                "4f95abdcc70e4bda818be5b8860585c5",
                Helpers.GetSkillFocus(StatType.SkillMobility).Icon,
                FeatureGroup.None,
                gnomeReq,
                Helpers.CreateAddStatBonus(StatType.SkillMobility, 1, ModifierDescriptor.Trait),
                Helpers.CreateAddStatBonus(StatType.Initiative, 1, ModifierDescriptor.Trait)));

            // Aasimar:
            // - Martyr’s Blood (+1 attack if HP below half).
            // - Toxophilite (+2 crit confirm with bows)
            // - Wary (+1 perception/persuasion)

            // TODO: Enlightened Warrior

            choices.Add(Helpers.CreateFeature("MartyrsBloodTrait", "Martyr’s Blood (Aasimar)",
                "You carry the blood of a self-sacrificing celestial, and strive to live up to your potential for heroism.\nBenefit(s): As long as your current hit point total is less than half of your maximum hit points possible, you gain a +1 trait bonus on attack rolls against evil foes.",
                "729d27ad020d485f843264844f0f2155",
                Helpers.GetIcon("3ea2215150a1c8a4a9bfed9d9023903e"), // Iron Will Improved
                FeatureGroup.None,
                aasimarReq,
                Helpers.Create<AttackBonusIfAlignmentAndHealth>(a =>
                {
                    a.TargetAlignment = AlignmentComponent.Evil;
                    a.Descriptor = ModifierDescriptor.Trait;
                    a.Value = 1;
                    a.HitPointPercent = 0.5f;
                })));

            choices.Add(Helpers.CreateFeature("ToxophiliteTrait", "Toxophilite (Aasimar)",
                "You’ve inherited some of your celestial ancestor’s prowess with the bow.\nBenefit: You gain a +2 trait bonus on attack rolls made to confirm critical hits with bows.",
                "6c434f07c8984971b1d842cecdf144c6",
                Helpers.GetIcon("f4201c85a991369408740c6888362e20"), // Improved Critical
                FeatureGroup.None,
                aasimarReq,
                Helpers.Create<CriticalConfirmationBonus>(a =>
                {
                    a.Bonus = 2;
                    a.Value = 0;
                    a.CheckWeaponRangeType = true;
                    a.Type = AttackTypeAttackBonus.WeaponRangeType.RangedNormal;
                })));

            choices.Add(Helpers.CreateFeature("WaryTrait", "Wary (Aasimar)",
                "You grew up around people who were jealous of and hostile toward you. Perhaps your parents were not pleased to have a child touched by the divine—they may have berated or beaten you, or even sold you into slavery for an exorbitant price. You grew up mistrustful of others and believing your unique appearance to be a curse.\nBenefit: You gain a +1 trait bonus on Persuasion and Perception checks.",
                "7a72a0e956784cc38ea049e503189810",
                Helpers.GetIcon("86d93a5891d299d4983bdc6ef3987afd"), // Persuasive
                FeatureGroup.None,
                aasimarReq,
                Helpers.CreateAddStatBonus(StatType.SkillPersuasion, 1, ModifierDescriptor.Trait),
                Helpers.CreateAddStatBonus(StatType.SkillPerception, 1, ModifierDescriptor.Trait)));

            // Tiefling:
            // - Ever Wary (retain half dex bonus AC during surpise round)
            // - Prolong Magic (racial spell-like abilities get free extend spell)
            // - God Scorn (Demodand heritage; +1 saves vs divine spells)
            // - Shadow Stabber (+2 damage if opponent can't see you)

            choices.Add(Helpers.CreateFeature("EverWaryTrait", "Ever wary (Tiefling)",
                "Constant fear that your fiendish nature might provoke a sudden attack ensures that you never completely let down your guard.\nBenefit During the surprise round and before your first action in combat, you can apply half your Dexterity bonus (if any) to your AC. You still count as flat-footed for the purposes of attacks and effects.",
                "0400c9c99e704a1f81a769aa88044a03",
                Helpers.GetIcon("3c08d842e802c3e4eb19d15496145709"), // uncanny dodge
                FeatureGroup.None,
                tieflingReq,
                Helpers.Create<ACBonusDuringSurpriseRound>()));

            var tieflingHeritageDemodand =Traits.library.Get<BlueprintFeature>("a53d760a364cd90429e16aa1e7048d0a");
            choices.Add(Helpers.CreateFeature("GodScornTrait", "God Scorn (Demodand Tiefling)",
                "Your contempt for the gods and their sad little priests makes it easier to shake off the effects of their prayers.\nBenefit You gain a +1 trait bonus on saving throws against divine spells.",
                "db41263f6fd3450ea0a3bc45c98330f7",
                Helpers.GetIcon("2483a523984f44944a7cf157b21bf79c"), // Elven Immunities
                FeatureGroup.None,
                Helpers.PrerequisiteFeature(tieflingHeritageDemodand),
                Helpers.Create<SavingThrowBonusAgainstSpellSource>(s => s.Source = SpellSource.Divine)));

            var tieflingHeritageSelection =Traits.library.Get<BlueprintFeatureSelection>("c862fd0e4046d2d4d9702dd60474a181");
            choices.Add(Helpers.CreateFeature("ProlongMagicTrait", "Prolong Magic (Tiefling)",
                "Constant drills and preparation allow you to get more out of your innate magic.\nBenefit Whenever you use a spell - like ability gained through your tiefling heritage, it automatically acts as if affected by the Extend Spell metamagic feat.",
                "820f697f59114993a55c46044c98bf9c",
                tieflingHeritageSelection.Icon,
                FeatureGroup.None,
                tieflingReq,
                // TODO: double check that this actually works for SLAs.
                Helpers.Create<AutoMetamagic>(a => { a.Metamagic = Metamagic.Extend; a.Abilities = Traits.CollectTieflingAbilities(tieflingHeritageSelection); })));

            choices.Add(Helpers.CreateFeature("ShadowStabberTrait", "Shadow Stabber (Tiefling)",
                "An instinct for dishonorable conduct serves you well when fighting opponents who are blind, oblivious, or blundering around in the dark.\nBenefit You gain a +2 trait bonus on melee weapon damage rolls made against foes that cannot see you.",
                "b67d04e21a9147e3b8f9bd81ba36f409",
                Helpers.GetIcon("9f0187869dc23744292c0e5bb364464e"), // accomplished sneak attacker
                FeatureGroup.None,
                tieflingReq,
                Helpers.Create<DamageBonusIfInvisibleToTarget>(d => d.Bonus = 2)));

            choices.Add(UndoSelection.Feature.Value);
            raceTraits.SetFeatures(choices);
            adopted.SetFeatures(raceTraits.Features);
            adopted.AddComponent(Helpers.PrerequisiteNoFeature(raceTraits));

            return raceTraits;
        }
    }
}