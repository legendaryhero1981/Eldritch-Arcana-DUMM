
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
using Kingmaker.UnitLogic.Buffs.Blueprints;
using UnityEngine;
using System.IO;
using System.Collections;
using Kingmaker.Enums.Damage;

namespace EldritchArcana
{
    internal class PhysiqueDrawbacks
    {
        public static BlueprintFeatureSelection CreatePhysiqueDrawbacks()
        {
            string[] PhysiqueGuids = new string[100];
            string baseguid = "CB54279F30DA4802833F";
            int x = 0;
            for (long i = 432922691494; i < 432922691544; i++)
            {
                PhysiqueGuids[x] = baseguid + i.ToString();
                x++;
            }

            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var PhysiqueDrawbacks = Helpers.CreateFeatureSelection("PhysiqueDrawback", "Physique Drawback",
                "Physique Drawbacks puts the focus on bodily cripling aspects of your character’s background.",
                PhysiqueGuids[0], null, FeatureGroup.None, noFeature);

            noFeature.Feature = PhysiqueDrawbacks;
            var components = new List<BlueprintComponent> { };
            //components.Add(Helpers.Create<ArmorClassBonusAgainstAlignment>(s => { s.alignment= AlignmentComponent.Neutral; s.Value = -2; s.Descriptor = ModifierDescriptor.FearPenalty; }));            
            components.Add(Helpers.Create<ACBonusAgainstWeaponCategory>(w => { w.Category = WeaponCategory.Bite; w.ArmorClassBonus = -2; w.Descriptor = ModifierDescriptor.Penalty; }));
            components.Add(Helpers.Create<ACBonusAgainstWeaponCategory>(w => { w.Category = WeaponCategory.Claw; w.ArmorClassBonus = -2; w.Descriptor = ModifierDescriptor.Penalty; }));
            components.AddRange((new String[] {
                // Animal companions
                "f6f1cdcc404f10c4493dc1e51208fd6f",                "afb817d80b843cc4fa7b12289e6ebe3d",
                "f9ef7717531f5914a9b6ecacfad63f46",                "f894e003d31461f48a02f5caec4e3359",
                "e992949eba096644784592dc7f51a5c7",                "aa92fea676be33d4dafd176d699d7996",
                "2ee2ba60850dd064e8b98bf5c2c946ba",                "6adc3aab7cde56b40aa189a797254271",
                "ece6bde3dfc76ba4791376428e70621a",                "126712ef923ab204983d6f107629c895",
                "67a9dc42b15d0954ca4689b13e8dedea",                // Familiars
                "1cb0b559ca2e31e4d9dc65de012fa82f",                "791d888c3f87da042a0a4d0f5c43641c",
                "1bbca102706408b4cb97281c984be5d5",                "f111037444d5b6243bbbeb7fc9056ed3",
                "7ba93e2b888a3bd4ba5795ae001049f8",                "97dff21a036e80948b07097ad3df2b30",
                "952f342f26e2a27468a7826da426f3e7",                "61aeb92c176193e48b0c9c50294ab290",
                "5551dd90b1480e84a9caf4c5fd5adf65",                "adf124729a6e01f4aaf746abbed9901d",
                "4d48365690ea9a746a74d19c31562788",                "689b16790354c4c4c9b0f671f68d85fc",
                "3c0b706c526e0654b8af90ded235a089",
            }).Select(id => Helpers.Create<AddStatBonusIfHasFact>(a =>
            {
                a.Stat = StatType.AC;
                a.Value = -2;
                a.Descriptor = ModifierDescriptor.Penalty;
                a.CheckedFact = Traits.library.Get<BlueprintFeature>(id);
            })));

            var choices = new List<BlueprintFeature>();
            choices.Add(Helpers.CreateFeature("NatureWardDrawback", "Warded Against Nature",
                "You look scary, Animals see you as a predator and do not willingly approach within 30 feet of you." +
                "\nBenefit: Chance of wild animals not engaging you is greater." +//not actualy implemented but great for rp and you have an entire party 
                "\nDrawback: You take a -2 penalty on AC against animal bites and claw attack, and for each animal you have you lose 2 AC.",//implemented
                PhysiqueGuids[1],
                Helpers.GetIcon("1670990255e4fe948a863bafd5dbda5d"), // Boon Companion
                FeatureGroup.None,
                components.ToArray()));

            
            
            var burningHands = Traits.library.Get<BlueprintAbility>("4783c3709a74a794dbe7c8e7e0b1b038");
            choices.Add(Helpers.CreateFeature("BurnedDrawback", "Burned",
                "You were badly burned once by volcanic ash, torch-wielding mobs, or some fiery accident, and the scars pain you terribly you whenever you are too near to fire." +
                "\nDrawback: You take a –2 penalty on saves against fire effects.",
                PhysiqueGuids[2],
                burningHands.Icon,
                FeatureGroup.None,
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Fire; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));


            //var burningHands = Traits.library.Get<BlueprintAbility>("4783c3709a74a794dbe7c8e7e0b1b038");
            choices.Add(Helpers.CreateFeature("EntomophobeDrawback", "Entomophobe",
                "A harrowing experience with insects when you were young instilled in you a deep-seated fear of vermin of all description, especially when they swarm together." +
                "\nDrawback: You take a –2 penalty on attacks against vermin, and you take a –2 penalty on saving throws against the nauseated condition of a swarm’s distraction ability.",
                PhysiqueGuids[3],
                Helpers.NiceIcons(8),//spider web//
                FeatureGroup.None,
                Helpers.Create<SwarmAoeVulnerability>(),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Poison; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));


            //Family Ties is a little silly

            choices.Add(Helpers.CreateFeature("OrphanDrawback", "Family Died",
                "Your family is extremely important to you, and you feel disheartened becouse they died." +
                "\nDrawback: You take a -2 penalty on Will saves.",
                PhysiqueGuids[4],
                Helpers.NiceIcons(7), // Accomplished Sneak Attacker
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SaveWill, -2, ModifierDescriptor.Penalty)));


            choices.Add(Helpers.CreateFeature("FeyTakenDrawback", "Fey-taken",
                "As a child, you were whisked away by mischievous fey for a time. When you returned, you were ever after considered odd and distant. You long to return there, and find the mortal world dull and at times revolting, so you do not eat as you should and do not question strange visions." +
                "\nDrawback: You take a –2 penalty on saving throws against disease, illusions, and poison of all kinds, as well as against the spells, spell-like abilities, and supernatural abilities of fey.",
                PhysiqueGuids[5],
                Helpers.GetIcon("2483a523984f44944a7cf157b21bf79c"), // Elven Immunities
                FeatureGroup.None,
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Poison; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; }),
                Helpers.Create<SavingThrowBonusAgainstSchool>(a => { a.School = SpellSchool.Illusion; a.Value = -2; a.ModifierDescriptor = ModifierDescriptor.Penalty; }),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Disease; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));

            //Foul Brand
            //As a child, you were whisked away by mischievous fey for a time. When you returned, you were ever after considered odd and distant. You long to return there, and find the mortal world dull and at times revolting, so you do not eat as you should and do not question strange visions.
            var FoulBrand = Helpers.CreateFeatureSelection("FoulBrandDrawback", "Foul Brand",
                "You have the symbol of an evil deity burned into your flesh. The place where the symbol is decides what the penalty is.",
                PhysiqueGuids[6],
                burningHands.Icon,
                FeatureGroup.None);


            var BrandedFeatures = new List<BlueprintFeature>()
            {

                Helpers.CreateFeature("LegDrawback", "Leg",
                "The symbol is on your leg." +
                "\nDrawback: Your movement speed is 5ft slower.",
                PhysiqueGuids[7],
                Helpers.NiceIcons(5), // Accomplished Sneak Attacker
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.Speed, -5, ModifierDescriptor.Penalty))
                ,
                Helpers.CreateFeature("FaceDrawback", "Face",
                "The symbol is on your face." +
                "\nDrawback: You take a -2 penalty on Persuasion.",
                PhysiqueGuids[8],
                Helpers.NiceIcons(3), // fear
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillPersuasion, -2, ModifierDescriptor.Penalty))
                ,
                Helpers.CreateFeature("HandsDrawback", "Hands",
                "The symbol is on your hands." +
                "\nDrawback: You take a -2 penalty on Trickery.",
                PhysiqueGuids[9],
                Helpers.NiceIcons(13), // Accomplished Sneak Attacker
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillThievery, -2, ModifierDescriptor.Penalty))

            };


            FoulBrand.SetFeatures(BrandedFeatures);
            choices.Add(FoulBrand);

            var Hedonisticdebuff = Helpers.CreateBuff("HedonisticDeBuff", "HedonisticDebuff",
                "You have feel like you are fatigued.",
                PhysiqueGuids[10],
                Helpers.NiceIcons(7), null,
                Helpers.CreateAddStatBonus(StatType.Strength, -2, ModifierDescriptor.Penalty),
                Helpers.CreateAddStatBonus(StatType.Dexterity, -2, ModifierDescriptor.Penalty));

            choices.Add(Helpers.CreateFeature("HedonisticDrawback", "Hedonistic",
                "You are a creature of pleasure and comfort." +
                "\nDrawback: You take a -2 penalty on Strength and Dexterity if you do not possess at least 100 + 200 per level gold.",
                PhysiqueGuids[11],
                Helpers.NiceIcons(10), // needs sloth icon
                FeatureGroup.None,
                CovetousCurseLogic.Create(Hedonisticdebuff)));

            choices.Add(Helpers.CreateFeature("HelplessDrawback", "Helpless",
                "You once stood helpless as great harm befell a loved one, and that paralysis sometimes returns when an ally is in a dire position." +
                "\nDrawback: You take a -2 penalty on saving throws vs paralysis and petrification.",
                PhysiqueGuids[12],
                Helpers.NiceIcons(3),//spider web//
                FeatureGroup.None,
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Paralysis; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; }),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Petrified; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));

            var UndeadImmunities = Traits.library.Get<BlueprintFeature>("8a75eb16bfff86949a4ddcb3dd2f83ae");
            var UndeadType = Traits.library.Get<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33");
            var Undeadcurse = Traits.library.CopyAndAdd<BlueprintFeature>(
                UndeadType.AssetGuid,
                "UndeadCurseDrawback",
                PhysiqueGuids[13]);

            //UndeadImmunities.SetDescription("undead immunitys");
            //Sprite frailsprite = Image2Sprite.Create("images_sprites/frail.png");
            //Urgathoa
            var UrgathoaFeature = Traits.library.Get<BlueprintFeature>("812f6c07148088e41a9ac94b56ac2fc8");
            var SpellFocus = Traits.library.Get<BlueprintFeature>("16fa59cc9a72a6043b566b49184f53fe");
            var SpellFocusNecromancy = Traits.library.Get<BlueprintFeature>("8791da25011fd1844ad61a3fea6ece54");
            //var AsmodeusFeature = Traits.library.Get<BlueprintFeature>("a3a5ccc9c670e6f4ca4a686d23b89900");
            
            //Zon - Kuthon.aae911217c5105244bbfddca6a58d77c
            //NorgorberFeature.805b6bdc8c96f4749afc687a003f9628
            //8791da25011fd1844ad61a3fea6ece54


            var CurseOptions = Helpers.CreateFeatureSelection("CurseOptions", "You Were Cursed[HB]",
                "You were cursed. Select a Curse.",
                PhysiqueGuids[14],
                Helpers.NiceIcons(10),
                FeatureGroup.None);
            //Undeadcurse.SetName("you were cursed to be an undead");

            var ElementalWeaknesListFeature = (new DamageEnergyType[] {
                DamageEnergyType.Fire,
                DamageEnergyType.Holy,
                DamageEnergyType.Divine,
            }).Select((element) =>
            Helpers.Create<AddEnergyVulnerability>(a => { a.Type = element; }));



            var RangedWeaponsDebuff = (new WeaponCategory[] {
                WeaponCategory.LightCrossbow,
                WeaponCategory.HeavyCrossbow,
                WeaponCategory.Javelin,
                WeaponCategory.KineticBlast,
                WeaponCategory.Longbow,
                WeaponCategory.Shortbow,
                WeaponCategory.Ray,
                WeaponCategory.Dart,
                WeaponCategory.Shuriken,
                WeaponCategory.ThrowingAxe,
            }).Select((WeapCat) =>
            Helpers.Create<WeaponCategoryAttackBonus>(a => { a.Category = WeapCat; a.AttackBonus = -2; }));

            var RangedWeaponsBuff = (new WeaponCategory[] {
                WeaponCategory.LightCrossbow,
                WeaponCategory.HeavyCrossbow,
                WeaponCategory.Javelin,
                WeaponCategory.KineticBlast,
                WeaponCategory.Longbow,
                WeaponCategory.Shortbow,
                WeaponCategory.Ray,
                WeaponCategory.Dart,
                WeaponCategory.Shuriken,
                WeaponCategory.ThrowingAxe,
            }).Select((WeapCat) =>
            Helpers.Create<WeaponCategoryAttackBonus>(a => { a.Category = WeapCat; a.AttackBonus = 1; }));



            UndeadType.SetNameDescriptionIcon("Undead Curse", "This creature is Changed by Urgothoa to be an undead.\nCreature is vulnerable to Fire,Holy And divine attacks.", Helpers.NiceIcons(44));
            Undeadcurse.SetNameDescriptionIcon("Undead Curse(inccorrect version)", "This version just changes con and cha the new version also changes necrotic and healing to function correctly this version still exists for save compatibility and its not a big deal.", Helpers.NiceIcons(44));
            //Undeadcurse.AddComponent(Helpers.PrerequisiteFeature(UrgathoaFeature));
            var lijstjelief = new List<BlueprintFeature> { SpellFocus,SpellFocusNecromancy , UrgathoaFeature};
            UndeadType.AddComponent(Helpers.PrerequisiteFeaturesFromList(lijstjelief,any:false));
            UndeadType.AddComponents(ElementalWeaknesListFeature);
            //Undeadcurse.SetFeatures(new List<BlueprintFeature> { UndeadImmunities});
            //Undeadcurse.SetNameDescription("","You where cursed to be an undead");
            foreach(BlueprintComponent bob in UndeadImmunities.GetComponents<BlueprintComponent>())
            {
                Undeadcurse.AddComponent(bob);
            }

            var FrogPolymorphBuff = Traits.library.Get<BlueprintBuff>("662aa00fd6242e643b60ac8336ff39e6");

            var CurseFeatures = new List<BlueprintFeature>()
            {
                //UndeadImmunities,
                //Undeadcurse,
                UndeadType,
                /*
                Helpers.CreateFeature("LycantropyDrawback", "Lycantropy",
                "lycantropy.description" +
                "\nDrawback:???",
                PhysiqueGuids[15],
                Helpers.NiceIcons(3), // fear
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillPersuasion, -2, ModifierDescriptor.Penalty))
                ,*/
                Helpers.CreateFeature("PolymorphDrawback",  "Witch's Curse",
                "You were cursed by a witch to become a frog. You did not like this and you killed the witch."+
                "The curse is almost broken, but if you are in a moment of weakness, you transform back into a frog."+
                "Drawback: If you attack someone when you are below 40 % health, you are transformed into a frog for 2 rounds.",
                PhysiqueGuids[16],
                Helpers.NiceIcons(45),
                FeatureGroup.None,
                Helpers.Create<BuffIfHealth>(a =>
                {                   
                    a.Descriptor = ModifierDescriptor.Penalty;
                    a.Value = -2;
                    a.HitPointPercent = 0.5f;
                }))

            };
            CurseOptions.SetFeatures(CurseFeatures);
            choices.Add(CurseOptions);



            choices.Add(Helpers.CreateFeature("AsthmaticDrawback", "Asthmatic",
                "Asthma, because out of all the things you could be bad at... you suck at breathing." +
                "\nDrawback: You suffer a -2 penalty against any effect that will cause you to be fatigued or exhausted, and any effect with cloud, dust, fog, or smoke in its name. You hold your breath for only half the normal duration. Additionally, sleeping in light or heavier armor fatigues you.",
                PhysiqueGuids[17],
                Helpers.NiceIcons(7),
                FeatureGroup.None,
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Fatigue; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; }),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Bomb; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));

            var Badvision = Helpers.CreateFeatureSelection("BadvisionDrawback", "Bad Vision",
                "You cannot see as well as others." +
                "\nDrawback: You take a –1 penalty on Perception checks. and you gain an other penalty depending on the type of bad vision you have. select from the options",
                PhysiqueGuids[18],
                Helpers.NiceIcons(46),
                FeatureGroup.None,        
                Helpers.CreateAddStatBonus(StatType.SkillPerception, -1, ModifierDescriptor.Crippled));

            var BadvisionFeatures = new List<BlueprintFeature>()
            {

                Helpers.CreateFeature("BadvisionDrawbackNear", "Nearsighted",
                "You cannot see well far in the distance." +
                "\nDrawback: All ranged attacks you make suffer an -2 penalty.",
                PhysiqueGuids[19],
                Helpers.NiceIcons(46), 
                FeatureGroup.None
                )
                ,
                Helpers.CreateFeature("BadvisionDrawbackFar", "Farsighted",
                "You cannot see well close to you." +
                "\nDrawback:Bane: All melee attacks you make suffer a -1 attack penalty.",
                PhysiqueGuids[20],
                Helpers.NiceIcons(46),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.AdditionalAttackBonus, -1, ModifierDescriptor.Penalty))
                ,
                Helpers.CreateFeature("BadvisionDrawbackBookwurm", "Bookworm[hb]",
                "You have spent way to much time reading up close with bad light and it ruined your eyesight. you can however fill in the blanks in your vision by recognizing paterns you have learned from books." +
                "\nBenefit: You use your inteligence modefier instead of wisdom on perception checks" +
                "\nDrawback: You take an additional -1 on perception checks.",
                PhysiqueGuids[21],
                Helpers.NiceIcons(46),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillPerception, -1, ModifierDescriptor.Penalty),
                Helpers.Create<ReplaceBaseStatForStatTypeLogic>(v =>
                {
                    v.StatTypeToReplaceBastStatFor = StatType.SkillPerception;
                    v.NewBaseStatType = StatType.Intelligence;
                }))

            };
            BadvisionFeatures[0].AddComponents(RangedWeaponsDebuff);
            BadvisionFeatures[1].AddComponents(RangedWeaponsBuff);

            Badvision.SetFeatures(BadvisionFeatures);
            choices.Add(Badvision);




            choices.Add(Helpers.CreateFeature("MisbegottenDrawback", "Misbegotten",
                "Whether due to the influence of malign magic, disease, or the scorn of the gods, you were born with a troublesome deformity that interferes with your movement." +
                "\nDrawback: You take a –2 penalty on all Dexterity-based skill checks.",
                PhysiqueGuids[22],
                Helpers.NiceIcons(29),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillStealth, -2, ModifierDescriptor.Crippled),
                Helpers.CreateAddStatBonus(StatType.SkillMobility, -2, ModifierDescriptor.Crippled),
                Helpers.CreateAddStatBonus(StatType.SkillThievery, -2, ModifierDescriptor.Crippled)));


            //choices.Add(
            //var CultistsVillage_Cultists = Traits.library.Get<BlueprintFaction>("0dd3f77814cc7bf4e9cfb1c96f2a4b4e");
            var LamashtusCurse = Traits.library.Get<BlueprintFeature>("ef3c653365c4a0a46b0d43a44f930186");
            var Occult = Helpers.CreateFeature("OccultBargainDrawback", "Occult Bargain",
                            "You draw magical power from a source who insists that its identity remains secret." +
                            "\nDrawback: You take a –1 penalty on concentration checks. and you have - 2 on saves and ac vs people that worship lamashtus",
                            PhysiqueGuids[23],
                            Helpers.NiceIcons(47),
                            FeatureGroup.None,
                            Helpers.Create<ConcentrationBonus>(a => { a.Value = -1; a.CheckFact = true; }),
                            Helpers.Create<ACBonusAgainstFactOwner>(t => { t.CheckedFact = LamashtusCurse;t.Bonus = -2; }));
            //occult.pre
            choices.Add(Occult);

            var feyfeature = Traits.library.Get<BlueprintFeature>("018af8005220ac94a9a4f47b3e9c2b4e");//FeyType.
            choices.Add(Helpers.CreateFeature("SpookedDrawback", "Spooked",
                "You had a traumatic experience with a spirit at a young age that colors your reactions to such creatures even to this day." +
                "\nDrawback: You take a –4 penalty on attackrolls vs fey creatures. and a -2 on rolls vs fear",
                PhysiqueGuids[24],
                Helpers.NiceIcons(39),
                FeatureGroup.None,
                Helpers.Create<AttackBonusAgainstFactOwner>(a=> { a.Bonus = -4; a.CheckedFact = feyfeature; }),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(f => { f.Bonus = -2; f.SpellDescriptor = SpellDescriptor.Fear; })));

            /*
            foreach (var choice in choices)
            {
                Log.Write(choice.Name);
                Log.Write(choice.Description);
            }*/


            PhysiqueDrawbacks.SetFeatures(choices);
            return PhysiqueDrawbacks;
        }
    }
}