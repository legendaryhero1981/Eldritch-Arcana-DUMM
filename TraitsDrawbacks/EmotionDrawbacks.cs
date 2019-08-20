using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Collections.Generic;

namespace EldritchArcana
{
    internal class EmotionDrawbacks
    {
        public static BlueprintFeatureSelection CreateEmotionDrawbacks()
        {
            //string[]  = new string[] { };
            string[] EmotionGuids = new string[200];
            //EmotionGuids = guids;
            string baseguid = "CB54279F30DA4802833F";
            int x = 0;
            for (long i = 542922691494; i < 542922691644; i++)
            {
                EmotionGuids[x] = baseguid + i.ToString();
                x++;
            }
            //int rnd = DateTime.Now.Millisecond%4;

            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var EmotionDrawbacks = Helpers.CreateFeatureSelection("EmotionDrawback", "Emotion Drawback",
                "Emotion Drawbacks put the focus on mental aspects of your character’s background.",
                EmotionGuids[0], null, FeatureGroup.None, noFeature);

            noFeature.Feature = EmotionDrawbacks;

            var choices = new List<BlueprintFeature>();
            choices.Add(Helpers.CreateFeature("AnxiousDrawback", "Anxious",
                "After suffering terribly for not being tight-lipped enough as a child, such as when you accidentally exposed your family to enemy inquisitors, you developed a habit of being overly cautious with your words." +
                "\nDrawback: You take a –2 penalty on Persuasion checks and must speak slowly due to the concentration required. Unless stated otherwise, you are assumed to not be speaking at a volume above a whisper.",
                EmotionGuids[1],
                Helpers.NiceIcons(16), // great fortitude
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillPersuasion, -2, ModifierDescriptor.Penalty)));

            //var tieflingHeritageDemodand = library.Get<BlueprintFeature>("a53d760a364cd90429e16aa1e7048d0a");
            choices.Add(Helpers.CreateFeature("AttachedDrawback", "Attached",
                "You are attached to yourself. Whenever the object of your attachment is either threatened, in danger, or in someone else’s possession, you take a –1 penalty on Will saves and a –2 penalty on saves against fear effects." +
                "\nDrawback: You take a –1 penalty on Will saves and a –2 penalty on saves against fear effects.",
                EmotionGuids[2],
                Helpers.GetIcon("2483a523984f44944a7cf157b21bf79c"), // Elven Immunities
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SaveWill, -1, ModifierDescriptor.Penalty),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Fear; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));


            choices.Add(Helpers.CreateFeature("BetrayedDrawback", "Betrayed",
                "You can roll twice and take the lower result on Sense Motive checks to get hunches. You cannot reroll this result, even if you have another ability that would normally allow you to do so." +
                "\nDrawback: You take a -3 penalty on Diplomacy checks.",
                EmotionGuids[3],
                Helpers.NiceIcons(2), // Accomplished Sneak Attacker
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.CheckDiplomacy, -3, ModifierDescriptor.Penalty)));


            choices.Add(Helpers.CreateFeature("BitterDrawback", "Bitter",
                "You have been hurt repeatedly by those you trusted, and it has become difficult for you to accept help." +
                "\nDrawback: When you receive healing from an ally’s class feature, spell, or spell-like ability, reduce the amount of that healing by 1 hit point.",
                EmotionGuids[4],
                Helpers.NiceIcons(5), // great fortitude
                FeatureGroup.None,
                Helpers.Create<FeyFoundlingLogic>(s => { s.dieModefier = 0; s.flatModefier = -1; })));

            choices.Add(Helpers.CreateFeature("CondescendingDrawback", "Condescending",
                "Raised with the assurance that only those like you are truly worthy of respect, you have an off-putting way of demonstrating that you look down on those not of your race and ethnicity or nationality." +
                "\nDrawback: You take a –5 penalty on Diplomacy and Intimidate checks.",
                EmotionGuids[5],
                Helpers.NiceIcons(10), // enchantment
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.CheckDiplomacy, -5, ModifierDescriptor.Penalty),
                Helpers.CreateAddStatBonus(StatType.CheckIntimidate, -5, ModifierDescriptor.Penalty)));

            //Effect Your base speed when frightened and fleeing increases by 5 feet, and the penalties you take from having the cowering, frightened, panicked, or shaken conditions increase by 1.If you would normally be immune to fear, you do not take these penalties but instead lose your immunity to fear(regardless of its source).
            choices.Add(Helpers.CreateFeature("CowardlyDrawback", "Cowardly",
                "You might face dangerous situations with bravado, but you are constantly afraid. And if you see a dead body you might just throw up." +
                "\nBenefit: Your movementspeed increases by 5" +
                "\nDrawback: You take a –4 penalty on saves against fear effects. And -2 to all Fortitude saves.",
                EmotionGuids[6],
                Helpers.NiceIcons(6), //invisiblilty
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SaveFortitude, -2, ModifierDescriptor.Penalty),
                Helpers.CreateAddStatBonus(StatType.Speed, 5, ModifierDescriptor.FearPenalty),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Fear; s.Value = -4; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));

            choices.Add(Helpers.CreateFeature("CrueltyDrawback", "Cruelty",
                "You were rewarded as a child for flaunting your victory over others as completely as possible, and you discovered you enjoyed the feeling of rubbing your foes’ faces in the dirt." +
                "\nBenefit: You have +2 on attack rolls against flanked targets." +
                "\nDrawback: You take a –2 penalty on attack rolls against someone that is not flanked.",
                EmotionGuids[7],
                Helpers.NiceIcons(9), // breakbone
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.BaseAttackBonus, -2, ModifierDescriptor.Penalty),
                DamageBonusAgainstFlankedTarget.Create(4)));

            choices.Add(Helpers.CreateFeature("EmptyMaskDrawback", "Empty Mask",
                "You have spent so long hiding your true identity to escape political enemies that you have lost much of your sense of self." +
                "\nDrawback: you take a –1 penalty on Will saves vs compulsion and a –2 vs people that know your identity.",
                EmotionGuids[8],
                Helpers.NiceIcons(14), // mask
                FeatureGroup.None,
                //Helpers.CreateAddStatBonus(StatType.SaveWill, -1, ModifierDescriptor.Penalty),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Compulsion; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));

            var debuff = Helpers.CreateBuff("EnvyDeBuff", "Envy",
                "You have 2 less on concentration checks.",
                EmotionGuids[9],
                Helpers.NiceIcons(1), null,
                Helpers.Create<ConcentrationBonus>(a => { a.Value = -2; a.CheckFact = true; }));

            choices.Add(Helpers.CreateFeature("EnvyDrawback", "Envy",
                "You grew up in or near an opulent, decadent culture that valued nothing more than showing up the material wealth or accomplishments of others, causing the seed of envy to be planted in your heart." +
                "\nDrawback: You have 2 less on concentration checks. if you do not posses at least 100 + 200 per level gold.",
                EmotionGuids[10],
                Helpers.NiceIcons(1), //grab
                FeatureGroup.None,
                CovetousCurseLogic.Create(debuff)));//
            //
            //int rnd = DateTime.Now.Millisecond % 64;
            var Fraud = Helpers.CreateFeatureSelection("GuiltyFraudDrawback", "Guilty Fraud",
                "You received something through trickery that you did not deserve, and your guilt for the misdeed distracts you from dangers around you." +
                "\nBenefit: Start the game dual wielding a one handed weapon." +
                "\nDrawback: You take a –2 penalty on Persuasion checks.",
                EmotionGuids[11],
                Helpers.NiceIcons(999), // great fortitude
                FeatureGroup.None,
                //WeaponCategory.LightRepeatingCrossbow                
                Helpers.CreateAddStatBonus(StatType.SkillPersuasion, -2, ModifierDescriptor.Penalty));

            //var weap = WeaponCategory.Dart;
            var hoi = new List<BlueprintFeature>() { };
            x = 11;//x is just a cheat value we use to ged guids
            //foreach (WeaponCategory weap in (WeaponCategory[])Enum.GetValues(typeof(WeaponCategory)))
            var Onehandedweapons = new WeaponCategory[] {
                WeaponCategory.Dagger,                WeaponCategory.Dart,
                WeaponCategory.DuelingSword,                WeaponCategory.ElvenCurvedBlade,
                WeaponCategory.Flail,                WeaponCategory.Greataxe,
                WeaponCategory.Javelin,                WeaponCategory.LightMace,
                WeaponCategory.Shuriken,                WeaponCategory.Sickle,
                WeaponCategory.Sling,                WeaponCategory.Kama,
                WeaponCategory.Kukri,                WeaponCategory.Starknife,
                WeaponCategory.ThrowingAxe,                WeaponCategory.LightPick,
                WeaponCategory.DwarvenWaraxe,                WeaponCategory.Trident,
                WeaponCategory.BastardSword,                WeaponCategory.Battleaxe,
                WeaponCategory.Longsword,                WeaponCategory.Nunchaku,
                WeaponCategory.Rapier,                WeaponCategory.Sai,
                WeaponCategory.Scimitar,                WeaponCategory.Shortsword,
                WeaponCategory.Club,                WeaponCategory.WeaponLightShield,
                WeaponCategory.WeaponHeavyShield,                WeaponCategory.HeavyMace,
                WeaponCategory.LightHammer,                WeaponCategory.LightPick,
            };
            foreach (WeaponCategory weap in Onehandedweapons)
            {

                x++;
                hoi.Add(Helpers.CreateFeature(
                $"Greedy{weap}Drawback",
                $"your scram reward — {weap}",
                $"{weap}", EmotionGuids[x]
                ,
                Helpers.NiceIcons(999), FeatureGroup.None,
                Helpers.Create<AddStartingEquipment>(a =>
                {

                    a.CategoryItems = new WeaponCategory[] { weap, weap };
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();

                    a.BasicItems = Array.Empty<BlueprintItem>();
                })));

                //Log.Write(x.ToString());
            }
            //Log.Write(x.ToString());
            x++;
            choices.Add(Helpers.CreateFeature("HauntedDrawback", "Haunted",
                "Something from your past—or a dark secret you presently hold—makes it difficult for you to ever be at peace, and your chronic worry that you might fall to evil influence has become a self-fulfilling prophecy." +
                "\nDrawback: You take a –2 penalty on spells with the evil descriptor.",
                EmotionGuids[x],
                Helpers.NiceIcons(39), // fatigue
                FeatureGroup.None,
                //Helpers.CreateAddStatBonus(StatType.SaveWill, -1, ModifierDescriptor.Penalty),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Evil; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));
            x++;
            choices.Add(Helpers.CreateFeature("HauntedRegretDrawback", "Haunting Regret",
                "When you were young, a relative with whom you had frequently quarreled passed away where his or her soul could not rest. Now, the unquiet spirit appears around you at inconvenient times, distracting you with regret for being unable to help." +
                "\nDrawback: You take a –2 penalty on saving throws against the distraction ability of swarms and mind-affecting effects and on concentration checks.",
                EmotionGuids[x],
                Helpers.NiceIcons(7),//fatigue//
                FeatureGroup.None,
                Helpers.Create<ConcentrationBonus>(a => a.Value = -2),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.MindAffecting; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));
            x++;
            choices.Add(Helpers.CreateFeature("ImpatientDrawback", "Impatient",
                "You love leaping into battle at the earliest opportunity, and it frustrates you to wait for others to act." +
                "\nBenefit: You take a +1 Insight bonus on Initiative." +
                "\nDrawback: You take a -2 penalty on saves against evil spells, and a -1 penalty to all attack rolls.",
                EmotionGuids[x],
                Helpers.NiceIcons(33), //rush
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.BaseAttackBonus, -1, ModifierDescriptor.Penalty),
                Helpers.CreateAddStatBonus(StatType.Initiative, 1, ModifierDescriptor.Insight),
                Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Evil; s.Value = -2; s.ModifierDescriptor = ModifierDescriptor.Penalty; })));


            Fraud.SetFeatures(hoi);
            choices.Add(Fraud);
            EmotionDrawbacks.SetFeatures(choices);
            return EmotionDrawbacks;
        }
    }
}