
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
    internal class CampaignTraits
    {
        public static BlueprintFeatureSelection CreateCampaignTraits()
        {
            var noFeature = Helpers.PrerequisiteNoFeature(null);
            var campaignTraits = Helpers.CreateFeatureSelection("CampaignTrait", "Campaign Trait",
                "Campaign traits are specifically tailored to relate to the Kingmaker campaign.",
                "f3c611a76bbc482c9c15219fa982fa17", null, FeatureGroup.None, noFeature);
            noFeature.Feature = campaignTraits;

            var choices = new List<BlueprintFeature>();

            string[] CampaignGuids = new string[200];
            //EmotionGuids = guids;
            string baseguid = "BB54279F30DA4802FFFF";
            int x = 0;
            for (long i = 442922691494; i < 442922691644; i++)
            {
                CampaignGuids[x] = baseguid + i.ToString();
                x++;
            }




            choices.Add(Helpers.CreateFeature("BastardTrait", "Bastard",
                "One of your parents was a member of one of the great families of Brevoy, perhaps even of the line of Rogarvia itself. Yet you have no substantive proof of your nobility, and you’ve learned that claiming nobility without evidence makes you as good as a liar. While you might own a piece of jewelry, a scrap of once-rich fabric, or an aged confession of love, none of this directly supports your claim. Thus, you’ve lived your life in the shadow of nobility, knowing that you deserve the comforts and esteem of the elite, even though the contempt of fate brings you nothing but their scorn. Whether a recent attempt to prove your heritage has brought down the wrath of a noble family’s henchmen or you merely seek to prove the worth of the blood in your veins, you’ve joined an expedition into the Stolen Lands, hoping to make a name all your own. You take a –1 penalty on all Charisma-based skill checks made when dealing with members of Brevic nobility but gain a +1 trait bonus on Will saves as a result of your stubbornness and individuality. (The penalty aspect of this trait is removed if you ever manage to establish yourself as a true noble.)",
                "d4f7e0915bd941cbac6f655927135817",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/human_bastard.png"),
                FeatureGroup.None,
                Helpers.Create<PrerequisiteFeature>(p => p.Feature = Helpers.human),
                // Other than the Prologue, there aren't many persuasion checks against members of the
                // nobility, prior to becoming a Baron. For simplicity, we simply remove the penalty after level 2.
                // (Ultimately this trait is more for RP flavor than anything.)
                Helpers.CreateAddStatBonusOnLevel(StatType.SkillPersuasion, -1, ModifierDescriptor.Penalty, 1, 2),
                Helpers.CreateAddStatBonus(StatType.SaveWill, 1, ModifierDescriptor.Trait)));

            var Outlander = Helpers.CreateFeatureSelection("OutlanderTrait", "Outlander",
                "You’ve recently come from somewhere else and are hoping to make your fortune here.\nChoose one of the following",
                "40DABEF7A6424982BC42CD39D8440029",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/outlander.png"),
                FeatureGroup.None);



            var NobleDescription = "You claim a tangential but legitimate connection to one of Brevoy’s noble families. If you aren’t human, you were likely adopted by one of Brevoy’s nobles or were instead a favored servant or even a childhood friend of a noble scion. Whatever the cause, you’ve had a comfortable life, but one far from the dignity and decadence your distant cousins know. Although you are associated with an esteemed name, your immediate family is hardly well to do, and you’ve found your name to be more of a burden to you than a boon in many social situations. You’ve recently decided to test yourself, to see if you can face the world without the aegis of a name you have little real claim or care for. An expedition into the storied Stolen Lands seems like just the test to see if you really are worth the title “noble.”";
            var NobleFamilyBorn = Helpers.CreateFeatureSelection("NobleFamilyBornTrait", "Noble born",
                NobleDescription + "\nBenefits: Select one of the following Royal families to gain its Traits",
                "ecacfcbeddfe453cafc8d60fc2fb5d45",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/noble_houses.png"),
                FeatureGroup.None);


            var Orlovski = Helpers.CreateFeatureSelection("Noble family Orlovsky Trait", "Noble family — Orlovsky",
                "Their motto is 'High Above.' \n" +
                "House Orlovsky controls northeastern Brevoy from Eagle's Watch on Mount Veshka. They try to rise above petty political maneuvers. As staunch allies of the now disappeared House Rogarvia, this has landed them in a prickly situation." +
                "\nBenefit: You have a +1 trait bonus on CMD. You select one of the following skills: Persuasion, Athletics, or Stealth. You gain a +1 trait bonus on checks with that skill, and it is always a class skill for you.",
                Helpers.MergeIds(Helpers.getStattypeGuid(StatType.AC), "9b03b7ff17394007a3fbec18bb42604b"),
                Image2Sprite.Create("Mods/EldritchArcana/sprites/house_orlovsky.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.AdditionalCMD, 1, ModifierDescriptor.Trait));


            var OrlovskiFamilyFeats = new StatType[] {

                 StatType.SkillAthletics,
                 StatType.SkillPersuasion,
                 StatType.SkillStealth,
                 //StatType.BaseAttackBonus,
                 //StatType.SneakAttack
             }.Select(skill => Traits.CreateAddStatBonus(
                $"Orlovsky{skill}Trait",
                $"{skill}",
                Orlovski.GetDescription(),
                Helpers.MergeIds(Helpers.GetSkillFocus(skill).AssetGuid, "2b01b7ff17394007a3fbec18bb42203b"),
                skill)).ToArray();

            //"0b183a3acaf5464eaad54276413fec08"


            var Lebda = Helpers.CreateFeatureSelection("Noble family Lebeda Trait", "Noble family — Lebeda",
                "Family Motto: 'Success through Grace.'\n" +
                "House Lebeda is based to the southwest of Lake Reykal in Brevoy, controlling the plains and significant portions of the lake's shipping. They are considered to be the Brevic noble family that epitomizes Rostland, having significant Taldan blood, an appreciation for fine things, and a love of sword fighting." +
                "\nBenefit: You get a +1 trait bonus on Knowledge (Arcana), and select a resource for a usable ability. You can use it at least one additional time.",
                Helpers.MergeIds(Helpers.getStattypeGuid(StatType.Intelligence), "9b03b7ff17394007a3fbec18bb42604c"),
                Image2Sprite.Create("Mods/EldritchArcana/sprites/house_lebeda.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillKnowledgeArcana, 1, ModifierDescriptor.Trait));
            //


            // var families = new List<BlueprintFeature>() { }
            //choices.Add( Helpers.CreateAddStatBonus(
            //Orlovski.SetFeatures(OrlovskiFeatures);
            Orlovski.SetFeatures(OrlovskiFamilyFeats);

            var duelingSword = Traits.library.Get<BlueprintWeaponType>("a6f7e3dc443ff114ba68b4648fd33e9f");
            var longsword = Traits.library.Get<BlueprintWeaponType>("d56c44bc9eb10204c8b386a02c7eed21");

            var layonhandsResource = Traits.library.Get<BlueprintAbilityResource>("9dedf41d995ff4446a181f143c3db98c");
            var MutagenResource = Traits.library.Get<BlueprintAbilityResource>("3b163587f010382408142fc8a97852b6");
            var JudgmentResource = Traits.library.Get<BlueprintAbilityResource>("394088e9e54ccd64698c7bd87534027f");
            var ItemBondResource = Traits.library.Get<BlueprintAbilityResource>("fbc6de6f8be4fad47b8e3ec148de98c2");
            var kiPowerResource = Traits.library.Get<BlueprintAbilityResource>("9d9c90a9a1f52d04799294bf91c80a82");
            var ArcanePoolResourse = Traits.library.Get<BlueprintAbilityResource>("effc3e386331f864e9e06d19dc218b37");
            var ImpromptuSneakAttackResource = Traits.library.Get<BlueprintAbilityResource>("78e6008db60d8f94b9196464983ad336");
            var WildShapeResource = Traits.library.Get<BlueprintAbilityResource>("ae6af4d58b70a754d868324d1a05eda4");
            var SenseiPerformanceResource = Traits.library.Get<BlueprintAbilityResource>("ac5600c9642692145b7eb4553a703c1a");

            var snowball = Traits.library.Get<BlueprintAbility>("9f10909f0be1f5141bf1c102041f93d9");
            var fireball = Traits.library.Get<BlueprintAbility>("2d81362af43aeac4387a3d4fced489c3");
            var alchemist = Traits.library.Get<BlueprintCharacterClass>("0937bec61c0dabc468428f496580c721");
            var bard = Traits.library.Get<BlueprintCharacterClass>("772c83a25e2268e448e841dcd548235f");
            var cleric = Traits.library.Get<BlueprintCharacterClass>("67819271767a9dd4fbfd4ae700befea0");
            var druid = Traits.library.Get<BlueprintCharacterClass>("610d836f3a3a9ed42a4349b62f002e96");
            var scion = Traits.library.Get<BlueprintCharacterClass>("f5b8c63b141b2f44cbb8c2d7579c34f5");
            var magus = Traits.library.Get<BlueprintCharacterClass>("45a4607686d96a1498891b3286121780");
            var paladin = Traits.library.Get<BlueprintCharacterClass>("bfa11238e7ae3544bbeb4d0b92e897ec");
            var sorcerer = Traits.library.Get<BlueprintCharacterClass>("b3a505fb61437dc4097f43c3f8f9a4cf");
            var ranger = Traits.library.Get<BlueprintCharacterClass>("cda0615668a6df14eb36ba19ee881af6");
            var wizard = Traits.library.Get<BlueprintCharacterClass>("ba34257984f4c41408ce1dc2004e342e");
            var oracle = Traits.library.Get<BlueprintCharacterClass>("ec73f4790c1d4554871b81cde0b9399b");
            var rogue = Traits.library.Get<BlueprintCharacterClass>("299aa766dee3cbf4790da4efb8c72484");
            
            var BloodlineFeyWoodlandStride = Traits.library.CopyAndAdd<BlueprintFeature>(
                "11f4072ea766a5840a46e6660894527d",
                "Noble family Garess Trait",
                Helpers.MergeIds(Helpers.getStattypeGuid(StatType.Reach), "9b03b7ff17394007a3fbec18bb42604b"));
            BloodlineFeyWoodlandStride.SetNameDescriptionIcon(
                "Noble family — Garess",
                "Familty motto: 'Strong as the Mountains'\n" +
                "House Garess is based in the western part of Brevoy, in the foothills of the Golushkin Mountains. " +
                "House Garess's crest is that of a snow-capped mountain peak in gray set against a dark blue field. There is a silvery crescent moon in the upper right corner, and there is a black hammer across the base of the peak. The Houses motto is Strong as the Mountains. " +
                "House Garess had a good relationship with the Golka dwarves until the dwarves vanished. Members of the house worked the metal that the dwarves mined. " +
                "The House has built several strongholds, Highdelve and Grayhaven, in the Golushkin Mountains. \nBenefit: Your movement speed is 5ft faster. And you have no trouble moving through nonmagical rough terrain.",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/house_garess.png")
                );
            BloodlineFeyWoodlandStride.AddComponent(Helpers.CreateAddStatBonus(StatType.Speed, 5, ModifierDescriptor.Trait));
            string[] Diffforhumans = new string[] { "Ki power as a monk", "Alchemy mutagen as a Alchemist", "Bonded item restore spell as a wizard", "Impromtu sneak attack as a acrane trickster", "Judgement ability as a inquisitor", "Arcane weapon enhancements as a magus", "Performances as a Sensei" };
            string[] DiffforhumansT = new string[] { "Ki", "Alchemy Mutagen", "Bonded Item", "Impromtu Sneak Attack", "Judgement", "Arcane Weapon Enhancements", "Sensei Performances" };
            var LebdaFeatures = new List<BlueprintFeature>() { };
            var Resources = new List<BlueprintAbilityResource> { kiPowerResource, MutagenResource, ItemBondResource, ImpromptuSneakAttackResource, JudgmentResource, ArcanePoolResourse, SenseiPerformanceResource };
            //CreateIncreaseResourceAmount for a few different resources
            x = 0;
            int y = 1;
            //Resources.randomelement();
            foreach (var stat in Resources)
            {
                x++;
                y = x < 3 ? 1 : x - 3;
                if (y == 0) y = 1;
                LebdaFeatures.Add(Helpers.CreateFeature($"Noble family {stat} Trait", $"extra {DiffforhumansT[x - 1]}",
                $"You are a resourceful family. Because of this, you " +
                $"gain {y} extra uses of {Diffforhumans[x - 1]}.",
                Helpers.MergeIds(stat.AssetGuid, "9b03b7ff17394007a3fbec18bb42604c"),
                Helpers.GetIcon(stat.AssetGuid), //
                FeatureGroup.None,
                stat.CreateIncreaseResourceAmount(y)));

            };
            Lebda.SetFeatures(LebdaFeatures);

            var hoi = new List<BlueprintFeature>() {
                //family medyved
                Helpers.CreateFeature("Noble family none Trait", "Noble family — Medvyed",
                "House Medvyed is a noble house of Brevoy that holds authority over the eastern lands that border and contain the Icerime Peaks and Gronzi Forest. They have maintained the traditions of worshiping nature, the 'Old Way'. Lord Gurev Medyed heads the Stoneclimb-based house."+
                "The people of the area raise mountain goats and sheep. They hunt in the forest and farm what little good land is on the edges of their concerns. Religion in this area tends to be more centralized on Erastil, but rumors of hidden shrines to Lamashtu do exist."+
                "The house crest is a black bear with black antlers above its head in front of a red field.Their motto is 'Endurance Overcomes All.'" +
                "\nBenefit: You can use Lay on Hands 4 times more per day, and you get a +1 trait bonus on saving throws vs compulsion effects from feys.",//bow of the true world
                Helpers.MergeIds(Helpers.getStattypeGuid(StatType.AdditionalCMB), "9b03b7ff17394007a3fbec18bb42604b"),
                Image2Sprite.Create("Mods/EldritchArcana/sprites/house_medvyed.png"),
                FeatureGroup.None,
                layonhandsResource.CreateIncreaseResourceAmount(4),Helpers.Create<SavingThrowBonusAgainstDescriptor>(s => { s.SpellDescriptor = SpellDescriptor.Compulsion; s.Value = 1; s.ModifierDescriptor = ModifierDescriptor.Trait; })),
                
                //family lodovka + atletics and snowball
                Helpers.CreateFeature("Noble family Lodovka Trait", "Noble family — Lodovka",
                "House Lodovka is a noble family of Brevoy with their headquarters on Acuben Isle on the Lake of Mists and Veils."+
                "They have traditionally been a power on the lake.Led by Lord Kozek Lodovka, both their fleet size and influence along the lake continue to increase."+
                "The fleet primarily catches fish and freshwater crabs."+
                "The house's crest includes a green crab climbing out of the water towards a gray tower/keep. Their motto is 'The Waters, Our Fields'." +
                "\nBenefit: You get a +2 trait bonus on Athletics checks. If you are a spellcaster, you know the spell Snowball.",
                Helpers.MergeIds(Helpers.getStattypeGuid(StatType.BaseAttackBonus), "9b03b7ff17394007a3fbec18bb42604b"),
                Image2Sprite.Create("Mods/EldritchArcana/sprites/house_lodovka.png"),
                FeatureGroup.None,
                Helpers.CreateAddKnownSpell(snowball,wizard,0),
                Helpers.CreateAddKnownSpell(snowball,alchemist,1),
                Helpers.CreateAddKnownSpell(snowball,bard,1),
                Helpers.CreateAddKnownSpell(snowball,cleric,1),
                Helpers.CreateAddKnownSpell(snowball,druid,1),
                Helpers.CreateAddKnownSpell(snowball,scion,1),
                Helpers.CreateAddKnownSpell(snowball,magus,1),
                Helpers.CreateAddKnownSpell(snowball,paladin,1),
                Helpers.CreateAddKnownSpell(snowball,sorcerer,1),
                Helpers.CreateAddKnownSpell(snowball,ranger,1),
                Helpers.CreateAddKnownSpell(snowball,wizard,1),
                Helpers.CreateAddKnownSpell(snowball,oracle,1),
                Helpers.CreateAddKnownSpell(snowball,rogue,1),
                //Helpers.CreateAddKnownSpell(snowball,bloodrager,1),
                //Helpers.CreateAddKnownSpell(snowball,witch,1),
                //Helpers.CreateAddKnownSpell(snowball,scald,1),
                Helpers.CreateAddStatBonus(StatType.SkillAthletics,2,ModifierDescriptor.Trait)),
                //family garess + 5 mvnt spd
                BloodlineFeyWoodlandStride,
                /*
                Helpers.CreateFeature("Noble family Garess Trait", "Noble family — Garess",
                "Familty motto: 'Strong as the Mountains'\n" +
                "House Garess is based in the western part of Brevoy, in the foothills of the Golushkin Mountains. " +
                "House Garess's crest is that of a snow-capped mountain peak in gray set against a dark blue field. There is a silvery crescent moon in the upper right corner, and there is a black hammer across the base of the peak. The Houses motto is Strong as the Mountains. "+
                "House Garess had a good relationship with the Golka dwarves until the dwarves vanished. Members of the house worked the metal that the dwarves mined. "+
                "The House has built several strongholds, Highdelve and Grayhaven, in the Golushkin Mountains. \nBenefit: Your movement speed is 5ft faster.",
                Helpers.MergeIds(Helpers.getGuids(StatType.Reach), "9b03b7ff17394007a3fbec18bb42604b"),
                Image2Sprite.Create("Mods/EldritchArcana/sprites/Icon_House_Garess.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.Speed,5,ModifierDescriptor.Trait)),
                */
                //family rogarvia
                Helpers.CreateFeature("Noble family Rogarvia Trait", "Noble family — Rogarvia",
                "Family Motto: 'With Sword and Flame.'\n" +
                "The former ruling house of Brevoy, House Rogarvia was founded by the descendants of Choral the Conqueror and Myrna Rogarvia, daughter of Nikos Surtova. Choral united Rostland and Issia into the kingdom of Brevoy after invading from Iobaria, accompanied by dragons. Most members of the House, including King Urzen Rogarvia, disappeared mysteriously in 4699 AR, in an event called the Vanishing. Their loss is not greatly mourned by the Brevic people and loyalists are calling for an investigation instead of blind allegiance to Noleski Surtova, who declared himself king."+
                "The Rogarvians were known to be ruthless rulers who did their best to hold Brevoy's disparate houses and factions together."+
                "A two-headed red dragon is the family's crest. One head of the dragon breathes fire while the other wields a sword. The house motto is 'With Sword and Flame.'" +
                "\nDrawback: You take a -4 penalty on Persuasion checks vs nobles (They hate you)." +
                "\nBenefit: You know the spell Fireball if you are a spell caster. Even if the caster normally does not get to learn the spell.",
                "B48B8234942C4FD191E99721728BF49D",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/house_rogarvia.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonusOnLevel(StatType.SkillPersuasion, -4, ModifierDescriptor.Penalty, 1, 3),
                Helpers.CreateAddKnownSpell(fireball,sorcerer,0),
                Helpers.CreateAddKnownSpell(fireball,wizard,3),
                Helpers.CreateAddKnownSpell(fireball,alchemist,3),
                Helpers.CreateAddKnownSpell(fireball,bard,3),
                Helpers.CreateAddKnownSpell(fireball,cleric,3),
                Helpers.CreateAddKnownSpell(fireball,druid,3),
                Helpers.CreateAddKnownSpell(fireball,scion,3),
                Helpers.CreateAddKnownSpell(fireball,magus,3),
                Helpers.CreateAddKnownSpell(fireball,paladin,3),
                Helpers.CreateAddKnownSpell(fireball,sorcerer,3),
                Helpers.CreateAddKnownSpell(fireball,ranger,3),
                Helpers.CreateAddKnownSpell(fireball,wizard,3),
                Helpers.CreateAddKnownSpell(fireball,oracle,3),
                Helpers.CreateAddKnownSpell(fireball,rogue,3)),

                
                /*/ family lebda
                Helpers.CreateFeature("Noble family Lebeda Trait", "Noble family — Lebeda",
                "family motto 'Success through Grace.'\n" +
                "House Lebeda is based to the southwest of Lake Reykal in Brevoy, controlling the plains and significant portions of the lake's shipping.[1] They are considered to be the Brevic noble family that epitomizes Rostland, having significant Taldan blood, an appreciation for fine things, and a love of sword fighting." +
                "\nBenefit:+1 knowledge arcana" +
                "\nBefefit:if you prepare mutagens you prepare one extra",
                Helpers.MergeIds(Helpers.getGuids(StatType.Intelligence), "9b03b7ff17394007a3fbec18bb42604c"),
                Helpers.GetIcon("79042cb55f030614ea29956177977c52"), // Great Fortitude
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillKnowledgeArcana,1,ModifierDescriptor.Trait),
                MutagenResource.CreateIncreaseResourceAmount(1)),
                */
                //family khartorov
                Helpers.CreateFeature("Noble family Khavortorov Trait", "Noble family — Khavortorov",
                "Family Motto: 'We Won't be Saddled.'\n" +
                "Khavortorov are hot-tempered family that has produced knights for many generations.\n" +
                "They are trying to better establish themselves as a great house of Brevoy now that their lieges, the Rogarvias, have disappeared. \n" +
                "Their crest is a white dragon with a helmet embedded in its chest. Many of the Khavortorov's are experts with the Aldori dueling sword.\n" +
                "Benefit: Dueling sword and longsword deal 1 extra damage and you start with one of both.",
                "44DFCE0451FC4188A06E2184EF65064B",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/house_khavortonov.png"),
                FeatureGroup.None,
                Helpers.Create<AddStartingEquipment>(a =>
                {

                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.DuelingSword, WeaponCategory.Longsword };
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = Array.Empty<BlueprintItem>();

                }),
                Helpers.Create<WeaponTypeDamageBonus>(a => { a.WeaponType = duelingSword; a.DamageBonus = 1; }),
                //Helpers.Create<WeaponConditionalDamageDice>(a => {a.CheckWielder = null })
                Helpers.Create<WeaponTypeDamageBonus>(a => { a.WeaponType = longsword; a.DamageBonus = 1; })),
                // family surtova
                Helpers.CreateFeature("Noble family Surtova Trait", "Noble family — Surtova",
                "Family Motto: 'Ours Is the Right.'\n" +
                "House Surtova is the current ruling family of Brevoy is the oldest Brevic noble family and the most influential. Their original holdings are the environs of Port Ice in northern Issia on the shores of the Lake of Mists and Veils. Their claim to the throne is linked to Nikos Surtova giving the hand of his daughter, Myrna Surtova, to Choral the Conqueror in marriage. This marriage allowed the house to keep its power as a staunch ally of House Rogarvia. In 4699 AR, during the Vanishing, House Surtova was able to use its high position to immediately claim regency until the Rogarvia's returned."+
                "The Surtovans are known as careful and cunning diplomats. Before Choral the Conqueror invaded, the Surtovans were known as pirates and raiders, and the family still has many connections with the pirates and brigands of the region, many of whom are distant relations of the Surtova clan. One of the more active pirates of the Lake of Mists and Veils, Captain Vali Dobos, is rumoured to have a close connection with the Surtova's, although he keeps his lineage hidden."+
                "Their family motto is 'Ours Is the Right,' which likely reflects their belief in a right to rulership of Brevoy since their family formerly ruled Issia as a group of crafty pirate-kings. Their crest is a gray ship in front of fields of blue on the lower half and black with silver stars on the upper half." +
                "\nBenefit: You get a +2 trait bonus on damage against flanked targets.",
                Helpers.MergeIds(Helpers.getStattypeGuid(StatType.SneakAttack), "9b03b7ff17394007a3fbec18bb42604c"),
                Image2Sprite.Create("Mods/EldritchArcana/sprites/house_surtova.png"),
                FeatureGroup.None,
                Helpers.Create<DamageBonusAgainstFlankedTarget>(a => a.Bonus = 2))
                //
            };

            hoi.Add(Orlovski);
            hoi.Add(Lebda);
            NobleFamilyBorn.SetFeatures(hoi);
            choices.Add(NobleFamilyBorn);


            var miscdes = "Nobles think about you but they don't know:\n";
            choices.Add(Helpers.CreateFeatureSelection("NobleBornTrait", "Noble Born(Human)",
                miscdes + "You claim a tangential but legitimate connection to one of Brevoy’s noble families. you’ve had a comfortable life, one you exploited untill you where send off to the be a monk and your luxury life ended.\nBenefits:you will start out with a bab penalty that will become a massive boon if you live the tale starts at -2 ends at +4",
                "a820521d923f4e569c3c69d091bf8865",
                Helpers.GetIcon("3adf9274a210b164cb68f472dc1e4544"), // Human Skilled
                FeatureGroup.None,
                PrerequisiteCharacterLevelExact.Create(10),
                Helpers.CreateAddStatBonusOnLevel(StatType.BaseAttackBonus, -2, ModifierDescriptor.Penalty, 1, 5),
                Helpers.CreateAddStatBonusOnLevel(StatType.BaseAttackBonus, -1, ModifierDescriptor.Penalty, 6, 10),
                //Helpers.CreateAddStatBonusOnLevel(StatType.AC, 1, ModifierDescriptor.Trait, 10,15),
                Helpers.CreateAddStatBonusOnLevel(StatType.BaseAttackBonus, 1, ModifierDescriptor.Trait, 11, 13),
                Helpers.CreateAddStatBonusOnLevel(StatType.BaseAttackBonus, 2, ModifierDescriptor.Trait, 14, 15),
                //Helpers.CreateAddStatBonusOnLevel(StatType.AC, 1, ModifierDescriptor.Trait, 15),
                Helpers.CreateAddStatBonusOnLevel(StatType.BaseAttackBonus, 3, ModifierDescriptor.Trait, 16, 17),
                Helpers.CreateAddStatBonusOnLevel(StatType.BaseAttackBonus, 4, ModifierDescriptor.Trait, 18)
                ));

            var SpellExpertise = Helpers.CreateFeatureSelection("OutlanderMissionary", "Outlander: Missionary",
                "You have come here to see about expanding the presence of your chosen faith after receiving visions that told you your faith is needed—what that need is, though, you’re not quite sure." +
                "\nBenefit: Pick three spells when you choose this trait. From this point on, whenever you cast these spells, you get a +1 trait bonus to caster level and DC. You also gain a +1 trait bonus to lore (Religion).",
                "6a3dfe274f45432b85361bdbb0a3009b",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/outlander.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillLoreReligion, 1, ModifierDescriptor.Trait));
            Traits.FillSpellSelection(SpellExpertise, 1, 9, Helpers.Create<IncreaseCasterLevelForSpellMax>());
            //choices.Add(SpellExpertise);
            /*var newthiny = CreateFeature("OutlanderLoreseeker", "Outlander:Loreseeker",
                "You have come here to see about expanding the presence of your chosen faith after receiving visions that told you your faith is needed—what that need is, though, you’re not quite sure.\nBenefit: Pick one spell when you choose this trait—from this point on, whenever you cast that spell, you do so at caster level max.",
                "6a3dfe274f45432b85361bdbb0a3010c",
                Helpers.GetIcon("fe9220cdc16e5f444a84d85d5fa8e3d5");*/
            var SpellExpertise2 = Helpers.CreateFeatureSelection("OutlanderLoreseeker", "Outlander: Loreseeker",
                "The secrets of ancient fallen civilizations intrigue you, particularly magical traditions. You’ve studied magic intensely, and hope to increase that knowledge by adding lost lore. You’ve come to pursue that study, and chose this place as your base because it was out of the way of bigger cities—meaning less competition to study the ancient monuments in the region, you hope!" +
                ".\nBenefit: Pick three spells when you choose this trait. From this point on, whenever you cast these spells, you get a +1 trait bonus to caster level and DC. You also gain a +1 trait bonus to Knowledge (Arcana).",
                "6a3dfe274f45432b85361bdbb0a3010c",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/outlander.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SkillKnowledgeArcana, 1, ModifierDescriptor.Trait));
            //FillSpellSelection(SpellExpertise2, 1, 9, Helpers.Create<IncreaseCasterLevelForSpellMax>(), Helpers.Create<IncreaseSpellDC>());
            var spellchoises = Traits.FillTripleSpellSelection(SpellExpertise2, 1, 9, Helpers.Create<IncreaseCasterLevelForSpellMax>());
            SpellExpertise2.SetFeatures(spellchoises);
            SelectFeature_Apply_Patch.onApplyFeature.Add(SpellExpertise2, (state, unit) =>
            {
                SpellExpertise2.AddSelection(state, unit, 1);
                SpellExpertise2.AddSelection(state, unit, 1);
            });
            SelectFeature_Apply_Patch.onApplyFeature.Add(SpellExpertise, (state, unit) =>
            {
                SpellExpertise.AddSelection(state, unit, 1);
                SpellExpertise.AddSelection(state, unit, 1);
            });
            //SpellExpertise2.
            //SpellExpertise2.SetFeatures(SpellExpertise2.Features);
            //FillSpellSelection(SpellExpertise2, 3, 3, Helpers.Create<IncreaseCasterLevelForSpell>(), Helpers.Create<IncreaseSpellDC>());
            //var ding1 = Helpers.CreateAddStatBonus(StatType.SkillKnowledgeArcana, 1, ModifierDescriptor.Trait);
            //FillSpellSelection(ding1, 1, 9, Helpers.Create<IncreaseCasterLevelForSpellMax>());
            //choices.Add(SpellExpertise2);f

            //new BlueprintComponent = Helpers.Cre

            var OutlanderFeatures = new List<BlueprintFeature>()
            {
                SpellExpertise2,
                SpellExpertise,
                Helpers.CreateFeature("OutlanderExile", "Outlander: Exile",
                "For whatever reason, you were forced to flee your homeland. Chance or fate has brought you here, and it’s here that your money ran out, leaving you stranded in this small town. You are also being pursued by enemies from your homeland, and that has made you paranoid and quick to react to danger.\nBenefit: You gain a +2 trait bonus on initiative checks.",
                "fa2c636580ee431297de8806a046054a",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/human_bastard.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.Initiative, 2, ModifierDescriptor.Trait))
            };
            //FillSpellSelection(OutlanderFeatures, 1, 9, Helpers.Create<IncreaseCasterLevelForSpellMax>());
            Outlander.SetFeatures(OutlanderFeatures);
            //Outlander.AddSelection(1,SpellExpertise,1);
            choices.Add(Outlander);

            /*
            
            /* TODO: Noble Born. This will require some adaptation to the game. *
            var nobleBorn = Helpers.CreateFeatureSelection("NobleBornTrait", "Noble Born",
                "You claim a tangential but legitimate connection to one of Brevoy’s noble families. If you aren’t human, you were likely adopted by one of Brevoy’s nobles or were instead a favored servant or even a childhood friend of a noble scion. Whatever the cause, you’ve had a comfortable life, but one far from the dignity and decadence your distant cousins know. Although you are associated with an esteemed name, your immediate family is hardly well to do, and you’ve found your name to be more of a burden to you than a boon in many social situations. You’ve recently decided to test yourself, to see if you can face the world without the aegis of a name you have little real claim or care for. An expedition into the storied Stolen Lands seems like just the test to see if you really are worth the title “noble.”",
                "a820521d923f4e569c3c69d091bf8865",
                null,
                FeatureGroup.None);
            choices.Add(nobleBorn);
            /*
            var families = new List<BlueprintFeature>();
            // TODO: Garess, Lebeda are hard to adapt to PF:K, need to invent new bonuses.
            // Idea for Garess:
            // - Feather Step SLA 1/day?
            // Lebeda:
            // - Start with extra gold? Or offer a permanent sell price bonus (perhaps 5%?)
            //
            families.Add();
            */
            var summonedBow = Traits.library.Get<BlueprintItem>("2fe00e2c0591ecd4b9abee963373c9a7");
            
            //ishomebrew
            var dice = Helpers.GetIcon("3fcc181a8b2094b4d9a636b639f0b79b");
            var OptimisticGambler =Helpers.CreateFeatureSelection("OptimisticGamblerTrait", "Optimistic Gambler",
                 "You’ve always seemed to have trouble keeping money. Worse, you always seem to have debts looming over your head. When you heard about the “Cheat the Devil and Take His Gold” gambling tournament, you felt in your gut that your luck was about to change. You’ve always been optimistic, in fact, and even though right now is one of those rare times where you don’t owe anyone any money (you just paid off a recent loan from local moneylender Lymas Smeed), you know that’ll change soon enough. Better to start amassing money now when you’re at one of those rare windfall times! You’ve set aside a gold coin for the entrance fee, and look forward to making it big—you can feel it in your bones! This time’s gonna be the big one! Your boundless optimism, even in the face of crushing situations, has always bolstered your spirit.\n" +
                 "Benefit: take a chance you will get a random benefit",
                 "c88b9398af66406cac173884df308eb8",
                 Image2Sprite.Create("Mods/EldritchArcana/sprites/optimistic_gambler.png"),
                 FeatureGroup.None);
            //list with random features
            string wwib = "What will it be?";
            string Gmbldsc = "Look at your stats and inventory or at your class ability usage stats and you will find out your bonus\n" +
                    "Or you won't. That's the thing. Sometimes you win, and sometimes you lose. But that's what you are all about.\n" +
                    "Benefit: To know what type of bonus you get in advance, right-click on the trait and scroll down." +
                    $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n " +
                    $"Note if you want a different bonus you need to restart the game. this bonus will stay the same" +
                    $"\n";
            var UndeadSummonFeature = Traits.library.Get<BlueprintFeature>("f06f246950e76864fa545c13cb334ba5");
            var TristianAngelFeature = Traits.library.Get<BlueprintFeature>("96e026d5a38b24e4b87cd7dcd831cc16");
            var RingOfEnhancedSummonsFeature = Traits.library.Get<BlueprintFeature>("2bf0c2547f455894b93083e589866030");
            var randsummom = Traits.library.CopyAndAdd<BlueprintFeature>(
                UndeadSummonFeature.AssetGuid, 
                "RandomEffectUndeadSummons",
                CampaignGuids[32]);
            
            randsummom.SetDescription(Gmbldsc + randsummom.GetName());//"undead summons"
            randsummom.SetName(wwib);
            randsummom.SetIcon(dice);

            var guidfeaturelist = new string[]{
                "201614af25697594a865355182fdb558",
                "d7d8d9691f5b8b84497c8789672fe1ba",
                "bf9b14d6f65fa944f91f5cc2b9d02fa0",
                "576933720c440aa4d8d42b0c54b77e80",
                "789c7539ba659174db702e18d7c2d330",
                "15bac762b599b7e42824c333717d79d9",
                "734a29b693e9ec346ba2951b27987e33",
                "3c0b706c526e0654b8af90ded235a089",
                "e66154511a6f9fc49a9de644bd8922db",
                "9c141c687eae35f4ba5399c11a4bdbc3",
                "9993edb6c470a6f4ab0bb8aac0b7522a",
                "aae0cb964bf516a4480d6745b71de4e7",
                "2ee2ba60850dd064e8b98bf5c2c946ba",
            };
            
            /*
            randsummom,
            RingOfEnhancedSummonsFeature,
            TristianAngelFeature,
            UndeadSummonFeature,
            */

            var OptimisticGamblerOptions = new List<BlueprintFeature>() {
                randsummom,
                
                //+3 healed
                Helpers.CreateFeature("randomeffectExtraHeal",wwib,Gmbldsc+"+3 healed by healspels"
                    ,"c88b9398af66406cac124884df308eb8",dice,FeatureGroup.None,
                    Helpers.Create<FeyFoundlingLogic>(s => { s.flatModefier = 3; })),
                //+3 healed per die
                Helpers.CreateFeature("randomeffectExtraHealdice",wwib,Gmbldsc+" +3 healed by healspels per die"
                    ,"c88b9398af66406cac124884df308ec3",dice,FeatureGroup.None,
                    Helpers.Create<FeyFoundlingLogic>(s => { s.dieModefier = 3; })),
                //get summoned bow
                Helpers.CreateFeature("randomeffectExtrabow",wwib,Gmbldsc+" You start with a very good bow +2 enhancement and speed"
                    ,"e82b9398af64406cac124884df308fb9",dice,FeatureGroup.None,
                    Helpers.Create<AddStartingEquipment>(a =>
                    {
                        a.CategoryItems = Array.Empty<WeaponCategory>();
                        a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                        a.BasicItems = new BlueprintItem[] { summonedBow };
                    })),
                //+1 on spells damage
                Helpers.CreateFeature("randomeffectExtramagicdamage",wwib,Gmbldsc+"extra damage on spells per damage die"
                    ,"c88b9398af66405cac124884df338eb8",dice,FeatureGroup.None,
                    Helpers.Create<ArcaneBloodlineArcana>(),
                    Helpers.Create<ArcaneBloodlineArcana>(),
                    Helpers.Create<ArcaneBloodlineArcana>())
            };


            for(int i = 0; i < guidfeaturelist.Length; i++)
            {
                int effectnumber = 33 + i;
                var CopiedFeat = Traits.library.CopyAndAdd<BlueprintFeature>(
                guidfeaturelist[i],
                $"RandomEffectnumber{effectnumber}",
                CampaignGuids[effectnumber]);
                CopiedFeat.SetDescription(Gmbldsc + CopiedFeat.GetName());//feature name
                CopiedFeat.SetName(wwib);
                CopiedFeat.SetIcon(dice);
                CopiedFeat.PrerequisiteFeature(any:true);
                
                
                OptimisticGamblerOptions.Add(CopiedFeat);
            }

            var bob = new StatType[] {
                StatType.AC,
                StatType.AdditionalAttackBonus,
                StatType.AdditionalCMB,
                StatType.BaseAttackBonus,
                StatType.Charisma,//(1.2.4)
                StatType.Reach,
                StatType.SneakAttack,
                StatType.Strength,//(1.2.4)
                StatType.Intelligence,
                StatType.Wisdom,//(1.2.4)
            };

            foreach (StatType stat in bob)
            {
                OptimisticGamblerOptions.Add(Helpers.CreateFeature($"randomeffect{stat}", wwib, Gmbldsc + $" +3 {stat} luck bonus"
                    , Helpers.MergeIds(Helpers.getStattypeGuid(stat), "c88b9398af66406cac173884df308eb8"),dice,FeatureGroup.None,
                    Helpers.CreateAddStatBonus(stat,3,ModifierDescriptor.Luck)));
            }


            var weapons = new WeaponCategory[] {
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
                WeaponCategory.WeaponHeavyShield,                WeaponCategory.HeavyMace  
            };
            x = 0;
            foreach (WeaponCategory weap in weapons)
            {
                
                x++;

                OptimisticGamblerOptions.Add(
                    Helpers.CreateFeature(
                        $"randomeffectExtra{weap}", wwib, Gmbldsc + $" You start with a {weap} and you have a 3 bonus on attack rolls with weapons of this type"
                        , CampaignGuids[x], dice, FeatureGroup.None,
                        Helpers.Create<WeaponCategoryAttackBonus>(b => { b.Category = weap; b.AttackBonus = 3;}),     
                        //Helpers.Create<WeaponTypeDamageBonus>(c=> { c.WeaponType = weap; })
                        Helpers.Create<AddStartingEquipment>(a =>
                        {
                            a.CategoryItems = new WeaponCategory[] { weap, weap };
                            a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                            a.BasicItems = Array.Empty<BlueprintItem>();
                        })
                    )
                 );
                
            }



            int rnd = DateTime.Now.Millisecond % OptimisticGamblerOptions.Count;
            int rnd2 = OptimisticGamblerOptions.Count-1-(DateTime.Now.Millisecond % OptimisticGamblerOptions.Count);
            //geneates a random number that is basicly a random element from the list.
            //rnd = 0;
            //rnd2 = 1;
            var xander = OptimisticGamblerOptions[rnd]; 
            var option2 = OptimisticGamblerOptions[rnd2];
            //var option3 = OptimisticGamblerOptions[DateTime.Now.Millisecond % OptimisticGamblerOptions.Count];
            OptimisticGamblerOptions = Main.settings?.CheatCustomTraits == true ? OptimisticGamblerOptions : new List<BlueprintFeature> {xander,option2 };
            //OptimisticGambler.SetFeatures(xander,option2);
            OptimisticGambler.SetFeatures(OptimisticGamblerOptions);
            OptimisticGambler.IgnorePrerequisites = true;
            choices.Add(OptimisticGambler);

            choices.Add(Helpers.CreateFeature("RostlanderTrait", "Rostlander",
                "You were raised in the south of Brevoy, a land of dense forests and rolling plains, of crystalline rivers and endless sapphire skies. You come from hearty stock and were raised with simple sensibilities of hard work winning well-deserved gains, the importance of charity and compassion, and the value of personal and familial honor. Yours is the country of the Aldori swordlords and the heroes who refused to bend before the armies of a violent conqueror. You care little for matters of politics and nobles or of deception and schemes. As you are thoroughly Brevic, the call for champions willing to expand your land’s influence into the Stolen Lands has inflamed your sense of patriotism and honor, and so you have joined an expedition to quest southward. Your hardy nature grants you a +1 trait bonus on all Fortitude saves.",
                "d99b9398af66406cac173884df308eb7",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/rostlander.png"),
                FeatureGroup.None,
                Helpers.CreateAddStatBonus(StatType.SaveFortitude, 1, ModifierDescriptor.Trait)));

            
            //var duelingSword =Traits.library.Get<BlueprintWeaponType>("a6f7e3dc443ff114ba68b4648fd33e9f");
            //var longsword =Traits.library.Get<BlueprintWeaponType>("d56c44bc9eb10204c8b386a02c7eed21");

            choices.Add(Helpers.CreateFeature("SwordScionTrait", "Sword Scion",
                "You have lived all your life in and around the city of Restov, growing up on tales of Baron Sirian Aldori and the exploits of your home city’s heroic and legendary swordlords. Perhaps one of your family members was an Aldori swordlord, you have a contact among their members, or you have dreamed since childhood of joining. Regardless, you idolize the heroes, styles, and philosophies of the Aldori and have sought to mimic their vaunted art. Before you can petition to join their ranks, however, you feel that you must test your mettle. Joining an expedition into the Stolen Lands seems like a perfect way to improve your skills and begin a legend comparable to that of Baron Aldori. You begin play with a longsword or Aldori dueling sword and gain a +1 trait bonus on all attacks and combat maneuvers made with such weapons.",
                "e16eb56b2f964321a29076226dccb29e",
                Image2Sprite.Create("Mods/EldritchArcana/sprites/sword_scion.png"),
                FeatureGroup.None,
                Helpers.Create<AddStartingEquipment>(a =>
                {
                    a.CategoryItems = new WeaponCategory[] { WeaponCategory.DuelingSword, WeaponCategory.Longsword };
                    a.RestrictedByClass = Array.Empty<BlueprintCharacterClass>();
                    a.BasicItems = Array.Empty<BlueprintItem>();
                }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = duelingSword; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; }),
                //Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = dagger; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; }),
                Helpers.Create<WeaponAttackAndCombatManeuverBonus>(a => { a.WeaponType = longsword; a.AttackBonus = 1; a.Descriptor = ModifierDescriptor.Trait; })));





            campaignTraits.SetFeatures(choices);
            return campaignTraits;
        }
    }
}
 