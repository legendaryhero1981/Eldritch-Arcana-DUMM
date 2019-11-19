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

            //var ArchaeologistCleverExplorer = Traits.library.Get<BlueprintFeature>("1322e50d2b36aba45ab5405db43c53a3");
            
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
                "a Gnomish alchemist discovers how to create a special elixir that he can imbibe in order to heighten his ability This is so potent it can be used an extra time. When consumed, the elixir causes the Alchemist’s skin to change color to match the background and causes his hands and feet to secrete a sticky residue.\n" +
                "Benfefit:you can use your mutagen an additinal 2 times per day.",
                "125cdf262e4147cda2c670db81852c69",
                Helpers.GetIcon("0d3651b2cb0d89448b112e23214e744e"),
                FeatureGroup.None,
                Helpers.Create<IncreaseResourceAmount>(i => { i.Resource = MutagenResource; i.Value = 2; }),
                gnomeReq));

            var AvidReader = Helpers.CreateFeatureSelection("AvidReaderTrait", "Avid Reader",
                "As a youth, you voraciously consumed books and scrolls provided by a member of an adventurer’s guild or a learned organization like the Pathfinder Society, and you have internalized these stories of bold adventurers." +
                "\nBenefit: Choose one Knowledge skill. You always choose to take 10 on checks with the chosen Knowledge skill, even when distracted or threatened.",
                "2e4dcdce32e159cbaf0fb3c641249cbf",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/opposition_research.png"),FeatureGroup.None );




            var AvidReaderOptions = new List<BlueprintFeature>(){

                Helpers.CreateFeature("AvidReaderArcana", "Knowledge Arcana",
                    "Because you are a magic bookworm\n" +
                    "Benefit: You can always choose to take 10 on checks with knowledge arcana, even when distracted or threatened.",
                    $"a932f3e69db44cdd33965985e37a6d2b",
                    Image2Sprite.Create("Mods/EldritchArcana/sprites/spell_perfection.png"),
                    FeatureGroup.None,
                    Helpers.Create<Take10ForSuccessLogic>(t => t.Skill = StatType.SkillKnowledgeArcana)
                  ),Helpers.CreateFeature("AvidReaderWorld", "Knowledge World",
                    "Becouse you are a bookworm.\n" +
                    "Benefit: You can always choose to take 10 on checks with knowledge world, even when distracted or threatened.",
                    $"b254f3e69db44cdd33964985e37a6d1b",
                    Image2Sprite.Create("Mods/EldritchArcana/sprites/opposition_research.png"),
                    FeatureGroup.None,
                    Helpers.Create<Take10ForSuccessLogic>(t => t.Skill = StatType.SkillKnowledgeWorld)
                  ),

            };


            AvidReader.SetFeatures(AvidReaderOptions);
            choices.Add(AvidReader);

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
 