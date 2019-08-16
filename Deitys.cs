
using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Root;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using Newtonsoft.Json;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics.Components;
using static Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbilityResourceLogic;
using Kingmaker.UnitLogic.ActivatableAbilities;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

namespace EldritchArcana.Warpriest
{
    class Deitys
    {
        static LibraryScriptableObject library => Main.library;
        internal static void load()
        {
            /*
            var Deityguidlist = new string[100];
            String baseguid = "a3a5cdc9c670e6c4ca4a686d23";
            int x = 0;
            for (long i = 29999; i < 30099; i++)
            {
                Deityguidlist[x] = baseguid + i.ToString();
                x++;
            }

            var Yog = library.CopyAndAdd<BlueprintFeature>("a3a5ccc9c670e6f4ca4a686d23b89900", "Yog", Deityguidlist[0]);
            Yog.SetDescription("praise yog");
            //library.add
            foreach (var componen in Yog.ComponentsArray)
            {
                Log.Write(componen.name);
            }
            */
        }
    }
}
