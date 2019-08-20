# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
## [1.2.1]

### Added
-More Icons Made by Nolanoth for spells traits and feats in the mod.
-Frostborn dwarf trait.

### Changed
-Undeadfeature Has now also a prerequisite option Spellfocus or Spellfocus-Necromancy.
-Summon six goblins now has a spell school and required material components.
-Bulky battelborn Has been moved to custom traits.

### Fixed
-A lot of descriptions see list:https://imgur.com/gallery/fr7rV2A 
Credits to Silverite for bringing them to my attention and pointing out the correct descriptions. 


## [1.2.0]

### Added
-Icons Made by Nolanoth for spells traits and feats in the mod.

### Changed
-Drawback spell vulnerability now adds vulnerability to a fitting school. 
-The undead feature now requires a Urgortha as a deity

## [1.1.9]

### Added
-Optimistic gambler trait(extra possibilities)
-Witches curse drawback trait
-Undead drawback trait.

### Changed
-Drawback spell vulnerability now also adds an elemental vulnerability to each spell school. 
-Replaced spiky shied and heavy shield with light weapons light hammer.

### Fixed
-Optimistic gambler some weapons not applying damage bonus.

### Removed
-Spiked shield and heavy spiked shield from random options and from guilty fraud

## [1.1.8]

### Added
-Optimistic gambler trait(extra possibilitys)

### Changed
-Drawback spell vulnerability is -4 from -3
-Drawback peg leg is now -2 on Initiative and -10 movement speed.
-Only One drawback feat may be chosen per character

### Fixed
-Lebeda resource description wizzard -> wizard

### Removed
-Arcanist

## [1.1.7]

### Added
-Family heirloom extra options(dagger)
-Tusked trait
-Glory of old trait
-drawbackfeats(peg leg, Spell Vulnerability)
-Optimistic gambler trait(gain a random bonus only 6 now more will be added)

### Changed
-Drawback feat frail is now -3 health at level 1 and -1 health per hit die
-Drawback Rogarvia is now -4 on persuasion vs nobles
-Drawback Warded against nature to be more like pnp.

### Fixed
-Lebeda resource descriptions not being understandable for some people

## [1.1.6]

### Added
-Family heirloom traits with 5 options.
-Gnomish alchemist trait.
-Custom trait Bloodline havoc for bloodragers that want to pick blood havoc

## [1.1.5]

### Added
-Crossblooded sorcerer can choose a bloodline mutation.

### Fixed
-Prestigious spellcaster is now being added correctly again.

## [1.1.4]

### Added
-3 extra drawbacks
-A drawback feat(take a drawback at level 1 to gain an extra feat)
-an extra regional trait

### Changed
-Outlander can now select 3 spells properly if you choose missionary or lore seeker.
-Trait Bulky battleborn now gives +2 hp per level each level thereafter up to level 64(for compatibility with mods that level beyond 20)

### Removed
-Arcanist Acces(unless you already have levels in the class) it is too buggy
and there is another modder already working on arcanist and he is much further. https://github.com/SnowyJune973/Arcanist/blob/3d190c49c3d0934fde9dbf5fec450ad157b5130b/Patches/Arcanist_Consume_Patch.cs

## [1.1.3]
### Fixed
-Bloodline Progression(not all yet)
-A class from another mod generating a duplicate id

### Changed
-Apparently introduced a bug that makes Prestigious spellcaster not being added correctly.

### Implemented
- The base for arcanist(perhaps early access version in 1.1.4) It's playable now but if  I change a Guid or the way they work your game is corrupted so don't pick on your main save


## [1.1.2]
### Added
-a new drawback

### Fixed
-Trait bunded hand description 
-Drawback coward not applying movement speed
-Bloodlines not being able to be selected on dragon disciple

## [1.1.1]

### Added
- Drawbacks + trait on character creation and on additional traits
- (1/3) of all drawbacks on roll20

### Changed
- A skip option for traits.

### Fixed
- new druids won't get meteor swarm(apparently druids don't learn this natural disaster)
- magi bards won't get emergency force sphere.

## [1.1.0]

### Added
-level 3 Summon swarm:
-level 2 Summon Sextuple band of goblins

### Fixed
-Outlander trait description

## [1.0.9]

### Added
-level 9 true resurrection:

### Fixed
-Figured out some bloodline features where being added to bloodrager bloodlines(From the call of the wild).
-Now, this mod checks if a bloodline contains the name "rager" and if it does it ignores it.

## [1.0.8]

### Added
-level 4 abjuration protection spell for wizards, magus, bard, Protectiondomain: Emergency force sphere:

### Changed
- experimenting with outlander traits:
- Meteor swarm available for druid and fire domain and magus as well.(they are not added retroactively though so if you have a caster that already has level 9 spells you have to add it with a bag of tricks)
- ComponentAppliedOnceOnLevelUp.appliedRank public 

### Working on:
- baseline compatibility with other mods that add classes.
- spiritual weapon

## [1.0.7]

### Changed
- Noble Born traits:
- updating and adding family descriptions and making the bonuses more like the intended bonuses
- Lovdevka now had + 2 athletics and knows snowball if you are ANY caster(except for casters added by other mods)
- Rogarvia has - 10 persuasion and knows fireball if you are ANY caster(except for casters added by other mods)

## [1.0.6]

### Added
 - flat bonus increase magical flat knack in custom traits.
 - house Khavortorov(sword scion but damage instead of attack bonus)
 - house Rogarvia(-10 persuasion vs nobles and you know fireball as a sorcerer spell)

### Changed
- Noble Born traits:
- updating and adding family descriptions and making the bonuses more like the intended bonuses
- Garess now has the + 5 movement speed
- Lovdevka now had + 2 atletics and knows snowball if wizzard
- Medyved 5->4 lay on hand extra changes

### Removed
- Noble born level 1 prerequisite(someone can find out they were a secret child from a noble family and find out at a later age)


## [1.0.5]

### Changed
- river rat bonus. to +1 dmg(dagger only) + 1 athletics and + 1 dagger and athletics is a class skill
- trait family outlander is now in a tree with outlander as the parent for a cleaner selection
- wish(arcane) is disabled by default unless cheats are enabled(once picked will stay in the spellbook)

## [1.0.4]

### Added
 - opting in to cheat traits also changes the cost of wish temporary hitpoints to x-50 (so negative 50) 

### Changed
- wish to be universalist. and level 9 and to have access to stat changes.

### Implemented
- bloodline fey id put in a separate code block so laughing touch can be added in future.

## [1.0.3]

### Added
 - Option to opt in to cheat traits

### Changed
- Moved mystic prophecy and metamagic expert to cheat traits

## [1.0.2]

### Added
- campaintrait family outlander
- Regionaltrait river rat.
## [1.0.1]

### Added
- Normal versions of the overpowered traits so you can choose both.


## [1.0.0]

### Added
- Nobleborn trait this is a trait i made up to test adding traits
- bulky dwarf = each levelup + 10 hp extra
- Mystic prophecy :give +2 int,wis,cha + 10 arcana at level 10(can only take at level 1)
- noble born start at a base attack bonus penalty -2 it grows gradualy to +4 over the course of your leveling up to 20(can only take at level 1)

### changed
- Metamagic traits to be overpowerd.(i was just modding the game for myself and did not expect eldrich arcana not to be updated anymore)

## [0.9.8]

### Removed
- Crossblooded sorcerer prerequisites becouse it caused a conflict and made the entire class unplayable.

### Changed
- Wish spell loading so it generates ids untill it finds one that is not duplicate and duplicate  Ids are dumped in miracle preloading.
  This couses it to throw an error but that might be fixed some other time.

### Fixed
- Lifemystery oracle can load miracle again
- Crossblooded sorcerer(without prerequisites)

## [0.9.7]

### Fixed
- interaction between spell trait selection and a new gui feature from pathfinder kingmaker. 

## [0.9.6]
 Joostjasper has taken over Updating the mod.
## [0.9.5]

### Added
- Arcane Bloodline now gets Metamagic Adept as part of its 3rd level power, and
  with Arcane Apotheosis (at level 20), metamagic won't increase casting time
  (this matches PnP bloodline).

### Changed
- Oracles now get 3+int skill points, similar to Druids (4+int is correct in
  PnP, but the game has condensed skills, so it should be 3 to match other
  classes). This fix can be disabled in settings, and is not retroactive for
  existing Oracle's skill ranks (unless they respec).

### Fixed
- Spells added by this mod can now be used in specialist Wizard slots.
- Eldritch Scions can now pick Extra Arcana or the new Magus arcanas.
- Magical Knack caster level bonus now shows up in the Spellbook UI text.
- Tongues curse now allows animal companions to be controlled if you can talk
  to the corresponding NPC.
- Setting metamagic cost to 0 in Bag of Tricks no longer prevents metamagic rods
  from being initialized.
- Spell Perfection no longer requires a full round action to apply one metamagic
  (for spontaneous casters).
- Delayed Blast Fireball can now be used with Selective Spell.


## [0.9.4]

### Fixed
- Update mod to work against game version 1.2.3 (other fixes planned for 0.9.4
  will be released as 0.9.5, to get this out ASAP).


## [0.9.3]

### Fixed
- Fix serious regression from the 0.9.2 attempted Respec mod fix; this allowed
  multiple trait selections via multiclassing.
- Life Link is now removed on rest, and no longer requires 2 resources per link.

## [0.9.2]

### Added
- New setting to disable the Tongues curse penalty, as it may catch players by
  surprise that they can't control some of their party (similar to PnP, the
  curse prevents communication with party members in combat, unless they speak
  your language. They can learn your language with 1 rank in Knowledge: World.)

### Fixed
- Possible fix for Respec mod issue with Traits/Favored class Bonus
  (note: Respec mod does not currently work on 1.2.0n, so unable to verify fix).
- Enable metamagic for many of the new spells that were missing it
  (Wall of Fire, Delayed Blast Fireball, Fly/Overland Flight, etc).
- Elemental Spell now works correctly with elemental damage immunities.
  (Previously it would sometimes check your immunity against the old element.)
- Fey Foundling now works with AOE healing effects (such as Channel Energy).
- Fly and Overland Flight buffs now correctly suppress their variants to prevent
  stacking multiple copies of the same buff.
- Carefully Hidden now gives the correct +1 will save instead of reflex.
- Life Link no longer plays visual/sound effects for fully healed targets.
- Clarify description of Meteor Swarm (implemented as +4 DC against the primary
  target if hit, rather than -4 to save).
- Tiefling racial spell-like abilities no longer show up in spell selections
  (such as Magical Lineage).
- Add try+catch guards to patches that were missing them; this should increase
  stability. Improve logging for patches that fail to apply (patch errors will
  now log in UnityModManager.log in release builds).
- Possible fix for CharBSelectorLayer_FillData_Patch exception on PF:K 1.1.6
  (the patch is used for Ancient Lorekeeper race prerequisite, and can be
  disabled in settings).

## [0.9.1] - 2019-01-22
### Added
- Reckless (Combat Trait)

### Fixed
- Fix prerequisites for sorcerer bloodline bonus feats, for ones that are used by
  archetypes: Fey (Sylvan), Celestial (Empyreal), and Arcane (Sage). (These could
  not be selected due to an and/or bug.)
- Portrait Loader can now load from portrait directories with non-integer names.
- Some trait bonuses (such as Fate's Favored) now show up in the combat log.
- Clarify Ancient Lorekeeper archetype description to mention that it replaces
  mystery (e.g. Time) class skills with its own.

## [0.9.0] - 2019-01-21
### Added
- Initial Release, see README for complete feature list.


future inplements;
spiderhawk
https://www.d20pfsrd.com/classes/base-classes/magus/archetypes/paizo-fans-united/spiderhawk
alchemical affinity
https://www.d20pfsrd.com/classes/core-classes/wizard/arcane-discoveries/arcane-discoveries-paizo/alchemical-affinity/
moment of prescience.
https://www.d20pfsrd.com/magic/all-spells/m/moment-of-prescience/
d