
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
    internal class FaithTraits
    {
        public static BlueprintFeatureSelection CreateFaithTraits()
        {
            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var faithTraits = Helpers.CreateFeatureSelection("FaithTrait", "Faith Trait",
                "Faith traits focus on the character's religious and philosophical leanings.",
                "21d0fe2d88e44e5cbfb28becadf86110", null, FeatureGroup.None, noFeature);
            noFeature.Feature = faithTraits;

            var choices = new List<BlueprintFeature>();
            choices.Add(Helpers.CreateFeature("BirthmarkTrait", "Birthmark",
                "You were born with a strange birthmark that looks very similar to the holy symbol of the god you chose to worship later in life.\nBenefits: This birthmark increases your devotion to your god. You gain a +2 trait bonus on all saving throws against charm and compulsion effects.",
                "ebf720b1589d43a2b6cfad26aeda34f9",
                Helpers.GetIcon("2483a523984f44944a7cf157b21bf79c"), // Elven Immunities
                FeatureGroup.None,
                Helpers.Create<SavingThrowBonusAgainstSchool>(a =>
                {
                    a.School = SpellSchool.Enchantment;
                    a.Value = 2;
                    a.ModifierDescriptor = ModifierDescriptor.Trait;
                })));

            

            choices.Add(Traits.CreateAddStatBonus("ChildOfTheTempleTrait", "Child of the Temple",
                "You have long served at a temple in a city, where you picked up on many of the nobility’s customs in addition to spending much time in the temple libraries studying your faith.",
                "cb79816f17d84a51b173ef74aa325561",
                StatType.SkillLoreReligion));


            choices.Add(Traits.CreateAddStatBonus("DevoteeOfTheGreenTrait", "Devotee of the Green",
                "Your faith in the natural world or one of the gods of nature makes it easy for you to pick up on related concepts.",
                "6b8e68de9fc04139af0f1127d2a33984",
                StatType.SkillLoreNature));

            choices.Add(Traits.CreateAddStatBonus("EaseOfFaithTrait", "Ease of Faith",
                "Your mentor, the person who invested your faith in you from an early age, took steps to ensure you understood that what powers your divine magic is no different from that which powers the magic of other religions. This philosophy makes it easier for you to interact with others who may not share your views.",
                "300d727a858d4992a3e01c8165a4c25f",
                StatType.SkillPersuasion));

            var channelEnergyResource = Traits.library.Get<BlueprintAbilityResource>("5e2bba3e07c37be42909a12945c27de7");
            var channelEnergyEmpyrealResource = Traits.library.Get<BlueprintAbilityResource>("f9af9354fb8a79649a6e512569387dc5");
            var channelEnergyHospitalerResource = Traits.library.Get<BlueprintAbilityResource>("b0e0c7716ab27c64fb4b131c9845c596");
            choices.Add(Helpers.CreateFeature("ExaltedOfTheSocietyTrait", "Exalted of the Society",
                "The vaults of the great city contain many secrets of the divine powers of the gods, and you have studied your god extensively.\nBenefit: You may channel energy 1 additional time per day.",
                "3bb1b077ad0845b59663c0e1b343011a",
                Helpers.GetIcon("cd9f19775bd9d3343a31a065e93f0c47"), // Extra Channel
                FeatureGroup.None,
                channelEnergyResource.CreateIncreaseResourceAmount(1),
                channelEnergyEmpyrealResource.CreateIncreaseResourceAmount(1),
                channelEnergyHospitalerResource.CreateIncreaseResourceAmount(1),
                LifeMystery.channelResource.CreateIncreaseResourceAmount(1)));

            choices.Add(Helpers.CreateFeature("FatesFavoredTrait", "Fate's Favored",
                "Whenever you are under the effect of a luck bonus of any kind, that bonus increases by 1.",
                "0c5dcccc21e148cdaf0fb3c643249bfb",
                Helpers.GetIcon("9a7e3cd1323dfe347a6dcce357844769"), // blessing luck & resolve
                FeatureGroup.None,
                Helpers.Create<ExtraLuckBonus>()));

            var WisFlesh = Helpers.CreateFeatureSelection("WisdomintheFleshTrait", "Wisdom in the Flesh",
                "Your hours of meditation on inner perfection and the nature of strength and speed allow you to focus your thoughts to achieve things your body might not normally be able to do on its own.\n" +
                "Benefit: choose a stat normaly decided by strength charisma or dexterety and use wisom instead.",
                "1d4dcccc21e148cdaf0fb3c643249cbf",
                Helpers.NiceIcons(43), // blessing luck & resolve
                FeatureGroup.None,
                Helpers.Create<ExtraLuckBonus>());

            var WisFleshOptions = new BlueprintFeature[6];
            var icons = new int[] {0,1,24,2,25,6 };
            var OldStats = new StatType[] {
                StatType.Dexterity,
                StatType.Dexterity,
                StatType.Charisma,
                StatType.Charisma,
                StatType.Strength,
                //StatType.Intelligence,
                StatType.Dexterity,
            };
            var Stats = new StatType[] {
                StatType.SkillMobility,
                StatType.SkillThievery,
                StatType.SkillUseMagicDevice,
                StatType.CheckIntimidate,
                StatType.SkillAthletics,
                //StatType.SkillKnowledgeWorld,
                StatType.SkillStealth,
            };
            for (int i = 0; i < 6; i++) {                
                WisFleshOptions[i]= Helpers.CreateFeature($"EmpathicDiplomatTrait{Stats[i]}", $"Use Wisdom for calculating {Stats[i]}",
                    "Your hours of meditation on inner perfection and the nature of strength and speed allow you to focus your thoughts to achieve things your body might not normally be able to do on its own. \n" +
                    $"Benefit:You modify your {Stats[i]} using your Wisdom modifier. insted of your {OldStats[i]}",
                    $"a98{i}f{i}e69db44cdd889{i}3985e37a6d2b",
                    Helpers.NiceIcons(i),
                    FeatureGroup.None,                   
                    Helpers.Create<ReplaceBaseStatForStatTypeLogic>(x =>
                    {
                        x.StatTypeToReplaceBastStatFor = Stats[i];
                        x.NewBaseStatType = StatType.Wisdom;
                    })                    
                    );                
            }
            WisFlesh.SetFeatures(WisFleshOptions);
            choices.Add(WisFlesh);


            choices.Add(Helpers.CreateFeature("IndomitableFaithTrait", "Indomitable Faith",
                "You were born in a region where your faith was not popular, but you still have never abandoned it. Your constant struggle to maintain your own faith has bolstered your drive.\nBenefit: You gain a +1 trait bonus on Will saves.",
                "e50acadad65b4028884dd4a74f14e727",
                Helpers.GetIcon("175d1577bb6c9a04baf88eec99c66334"), // Iron Will
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SaveWill, 1, ModifierDescriptor.Trait)));

            choices.Add(Traits.CreateAddStatBonus("ScholarOfTheGreatBeyondTrait", "Scholar of the Great Beyond",
                "Your greatest interests as a child did not lie with current events or the mundane—you have always felt out of place, as if you were born in the wrong era. You take to philosophical discussions of the Great Beyond and of historical events with ease.",
                "0896fea4f7ca4635aa4e5338a673610d",
                StatType.SkillKnowledgeWorld));

            // TODO: Stalwart of the Society

            faithTraits.SetFeatures(choices);
            return faithTraits;
        }
    }
}