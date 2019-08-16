
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
    internal class MagicTraits
    {
        public static BlueprintFeatureSelection CreateMagicTraits()
        {
            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var magicTraits = Helpers.CreateFeatureSelection("MagicTrait", "Magic Trait",
                "Magic traits focus on any magical events or training your character may have had in their past.",
                "d89181c607e4431084f9d97532c5c554", null, FeatureGroup.None, noFeature);
            noFeature.Feature = magicTraits;

            var choices = new List<BlueprintFeature>();
            choices.Add(Traits.CreateAddStatBonus("ClassicallySchooledTrait", "Classically Schooled",
                "Your greatest interests as a child did not lie with current events or the mundane—you have always felt out of place, as if you were born in the wrong era. You take to philosophical discussions of the Great Beyond and of historical events with ease.",
                "788098518aa9436782397fa318c64c69",
                StatType.SkillKnowledgeArcana));

            choices.Add(Traits.CreateAddStatBonus("DangerouslyCuriousTrait", "Dangerously Curious",
                "You have always been intrigued by magic, possibly because you were the child of a magician or priest. You often snuck into your parent’s laboratory or shrine to tinker with spell components and magic devices, and frequently caused quite a bit of damage and headaches for your parent as a result.",
                "0c72c573cc404b42916dc7265ea6f59a",
                StatType.SkillUseMagicDevice));


            var WildShapeResource = Traits.library.Get<BlueprintAbilityResource>("ae6af4d58b70a754d868324d1a05eda4");


            choices.Add(Helpers.CreateFeature("BeastOfSocietyTrait", "Beast Of Society",
                "A master druid revealed to you greater secrets of concentration when changing your form into that of an animal." +
                "\nBenefit: You can shapeshift more often.",
                "e34889a2dd7e4e9ebfdfa76bfb8f4445",
                WildShapeResource.Icon, 
                FeatureGroup.None,
                WildShapeResource.CreateIncreaseResourceAmount(4)));

            choices.Add(Helpers.CreateFeature("FocusedMindTrait", "Focused Mind",
                "Your childhood was dominated either by lessons of some sort (whether musical, academic, or other) or by a horrible home life that encouraged your ability to block out distractions and focus on the immediate task at hand.\nBenefit: You gain a +2 trait bonus on concentration checks.",
                "e34889a2dd7e4e9ebfdfa76bfb8f5556",
                Helpers.GetIcon("06964d468fde1dc4aa71a92ea04d930d"), // Combat Casting
                FeatureGroup.None,
                Helpers.Create<ConcentrationBonus>(a => a.Value = 2)));

            var giftedAdept = Helpers.CreateFeatureSelection("GiftedAdeptTrait", "Gifted Adept",
                "Your interest in magic was inspired by witnessing a spell being cast in a particularly dramatic method, perhaps even one that affected you physically or spiritually. This early exposure to magic has made it easier for you to work similar magic on your own.\nBenefit: Pick one spell when you choose this trait—from this point on, whenever you cast that spell, its effects manifest at +1 caster level.",
                "5eb0b8050ed5466986846cffca0b35b6",
                Helpers.GetIcon("fe9220cdc16e5f444a84d85d5fa8e3d5"), // Spell Specialization Progression
                FeatureGroup.None);
            Traits.FillSpellSelection(giftedAdept, 1, 9, Helpers.Create<IncreaseCasterLevelForSpell>());
            choices.Add(giftedAdept);

            choices.Add(Helpers.CreateFeature("MagicalKnackTrait", "Magical Knack",
                "You were raised, either wholly or in part, by a magical creature, either after it found you abandoned in the woods or because your parents often left you in the care of a magical minion. This constant exposure to magic has made its mysteries easy for you to understand, even when you turn your mind to other devotions and tasks.\nBenefit: Pick a class when you gain this trait—your caster level in that class gains a +2 trait bonus as long as this bonus doesn’t raise your caster level above your current Hit Dice.",
                "8fd15d5aa003497aa7f976530d21e430",
                Helpers.GetIcon("16fa59cc9a72a6043b566b49184f53fe"), // Spell Focus
                FeatureGroup.None,
                //Helpers.Create<IncreaseCasterLevel>(),
                Helpers.Create<IncreaseCasterLevelUpToCharacterLevel>()));

            var magicalLineage = Helpers.CreateFeatureSelection("MagicalLineageTrait", "Magical Lineage",
                "One of your parents was a gifted spellcaster who not only used metamagic often, but also developed many magical items and perhaps even a new spell or two—and you have inherited a fragment of this greatness.\nBenefit: Pick one spell when you choose this trait. When you apply metamagic feats to this spell that add at least 1 level to the spell, treat its actual level as 1 lower for determining the spell’s final adjusted level.",
                "1785787fb62a4c529104ba53d0de99af",
                Helpers.GetIcon("ee7dc126939e4d9438357fbd5980d459"), // Spell Penetration
                FeatureGroup.None);
            Traits.FillSpellSelection(magicalLineage, 1, 9, Helpers.Create<ReduceMetamagicCostForSpell>(r => r.Reduction = 1));
            choices.Add(magicalLineage);

            choices.Add(UndoSelection.Feature.Value);
            magicTraits.SetFeatures(choices);
            return magicTraits;

        }
    }
}