// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Root;
using Kingmaker.UI.Common.Animations;
using Kingmaker.UI.LevelUp;
using Kingmaker.Utility;
using UnityEngine;
using UnityModManagerNet;
using RES = EldritchArcana.Properties.Resources;

namespace EldritchArcana
{
    public class Main
    {
        [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary", new Type[0])]
        static class LibraryScriptableObject_LoadDictionary_Patch
        {
            static void Postfix(LibraryScriptableObject __instance)
            {
                var self = __instance;
                if (Main.library != null) return;
                Main.library = self;

                EnableGameLogging();

                SafeLoad(LoadCustomPortraits, "Custom Portraits in main portrait selection");

                SafeLoad(Helpers.Load, "Initialization code");

                SafeLoad(Spells.Load, "New spells");

                // Note: needs to be loaded after other spells, so it can offer them as a choice.
                SafeLoad(WishSpells.Load, "Wish spells");

                // Note: needs to run before almost everything else, so they can find the Oracle class.
                // However needs to run after spells are added, because it uses some of them.
                SafeLoad(OracleClass.Load, "Oracle class");

                //Game.Instance.Player.Inventory.Add("2fe00e2c0591ecd4b9abee963373c9a7", 1);
                // Note: spells need to be added before this, because it adds metamagics.
                //
                // It needs to run after new classes too, because SpellSpecialization needs to find
                // all class spell lists.
                SafeLoad(MagicFeats.Load, "Magic feats");

                // Note: needs to run after arcane spells (it uses some of them).
                SafeLoad(Bloodlines.Load, "Bloodlines");

                //SafeLoad(WarpriestClass.Load, "warpriest");   
                //Main.settings.DrawbackForextraTraits

                SafeLoad(DrawbackFeats.Load, "Drawback Feats");

                // Note: needs to run after things that add classes, and after bloodlines in case
                // they allow qualifying for racial prerequisites.
                SafeLoad(FavoredClassBonus.Load, "Favored class bonus, deity selection");

                //SafeLoad(ArcanistClass.Load, "Arcanist");
                //after favored class so it does not show up
                SafeLoad(Traits.Load, "Traits");

                // Note: needs to run after we create Favored Prestige Class above.
                SafeLoad(PrestigiousSpellcaster.Load, "Prestigious Spellcaster");

                // Note: needs to run after we create Favored Prestige Class above.
                //SafeLoad(WarpriestClass.Load, "Arcane Savant");

                //SafeLoad(WarpriestClass.Load, "warpriest");                
                //SafeLoad(ArcanistClass.Load, "Arcanist");

                // Note: needs to run after things that add bloodlines.
                SafeLoad(CrossbloodedSorcerer.Load, "Crossblooded Sorcerer");

                // Note: needs to run after things that add martial classes or bloodlines.
                SafeLoad(EldritchHeritage.Load, "Eldritch Heritage");

                // Note: needs to run after crossblooded and spontaneous caster classes,
                // so it can find their spellbooks.
                SafeLoad(ReplaceSpells.Load, "Spell replacement for spontaneous casters");

#if DEBUG
                // Perform extra sanity checks in debug builds.
                SafeLoad(CheckPatchingSuccess, "Check that all patches are used, and were loaded");
                SafeLoad(SaveCompatibility.CheckCompat, "Check save game compatibility");
                Log.Write("Loaded finished.");
#endif
            }
        }

        internal static LibraryScriptableObject library;

        public static bool enabled;

        public static UnityModManager.ModEntry.ModLogger logger;

        internal static Settings settings;

        static string testedGameVersion = "2.0.6";

        static PortraitLoader portraitLoader;

        static Harmony12.HarmonyInstance harmonyInstance;

        static readonly Dictionary<Type, bool> typesPatched = new Dictionary<Type, bool>();
        static readonly List<String> failedPatches = new List<String>();
        static readonly List<String> failedLoading = new List<String>();

        [System.Diagnostics.Conditional("DEBUG")]
        static void EnableGameLogging()
        {
            if (UberLogger.Logger.Enabled) return;

            // Code taken from GameStarter.Awake(). PF:K logging can be enabled with command line flags,
            // but when developing the mod it's easier to force it on.
            var dataPath = ApplicationPaths.persistentDataPath;
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            UberLogger.Logger.Enabled = true;
            var text = Path.Combine(dataPath, "GameLog.txt");
            if (File.Exists(text))
            {
                File.Copy(text, Path.Combine(dataPath, "GameLogPrev.txt"), overwrite: true);
                File.Delete(text);
            }
            UberLogger.Logger.AddLogger(new UberLoggerFile("GameLogFull.txt", dataPath));
            UberLogger.Logger.AddLogger(new UberLoggerFilter(new UberLoggerFile("GameLog.txt", dataPath), UberLogger.LogSeverity.Warning, "MatchLight"));

            UberLogger.Logger.Enabled = true;
        }

        static void LoadCustomPortraits()
        {
            if (!settings.ShowCustomPortraits) return;

            var charGen = BlueprintRoot.Instance.CharGen;
            var customPortraits = new List<BlueprintPortrait>();
            using (var md5 = MD5.Create())
            {
                foreach (var id in CustomPortraitsManager.Instance.GetExistingCustomPortraitIds())
                {
                    var portrait = UnityEngine.Object.Instantiate(charGen.CustomPortrait);
                    portrait.name = $"CustomPortrait${id}";

                    var guid = GetMd5Hash(md5, id);
                    library.AddAsset(portrait, Helpers.MergeIds("c3d4e15de2444b528c734fac8eb75ba2", guid));
                    portrait.Data = new PortraitData(id);
                    customPortraits.Add(portrait);
                }
            }

            if (customPortraits.Count > 0)
            {
                // Start a coroutine to preload the small sprite images.
                portraitLoader = new GameObject("PortraitLoader").AddComponent<PortraitLoader>();
                portraitLoader.Run(customPortraits);

                charGen.Portraits = charGen.Portraits.AddToArray(customPortraits);
            }
        }

        static string GetMd5Hash(MD5 md5, string input)
        {
            // Note: this uses MD5 to hash a string to 128-bits, it's not for any security purpose.
            var str = new StringBuilder();
            foreach (var b in md5.ComputeHash(Encoding.UTF8.GetBytes(input)))
            {
                str.Append(b.ToString("x2"));
            }
            Log.Write($"md5 hash of '${input}' is: {str}");
            return str.ToString();
        }


        class PortraitLoader : MonoBehaviour
        {
            internal void Run(List<BlueprintPortrait> portraits)
            {
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave;
                StartCoroutine(Load(portraits));
            }

            IEnumerator<object> Load(List<BlueprintPortrait> portraits)
            {
                var start = DateTime.Now;
                var watch = new Stopwatch();
                var setPortraitImage = Helpers.CreateFieldSetter<PortraitData>("m_PortraitImage");
                var manager = CustomPortraitsManager.Instance;
                var baseSmall = BlueprintRoot.Instance.CharGen.BasePortraitSmall;
                foreach (var portrait in portraits)
                {
                    // Don't block the game UI thread during load.
                    yield return null;

                    watch.Start();
                    // Only load the small portrait eagerly; let the others load if needed.
                    var data = portrait.Data;
                    var path = manager.GetSmallPortraitPath(data.CustomId);
                    // CustomPortraitsManager uses sync IO internally.
                    //
                    // I prototyped async file IO too, but it makes little difference since the
                    // entire load time is less than a second for hundreds of portraits.
                    setPortraitImage(data, manager.LoadPortrait(path, baseSmall, false));
                    watch.Stop();
                }
                Log.Write($"Loaded {portraits.Count} portraits, took: {watch.Elapsed}, time since start: {DateTime.Now - start}");
                Main.portraitLoader = null;
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        // We don't want one patch failure to take down the entire mod, so they're applied individually.
        //
        // Also, in general the return value should be ignored. If a patch fails, we still want to create
        // blueprints, otherwise the save won't load. Better to have something be non-functional.
        internal static bool ApplyPatch(Type type, String featureName)
        {
            try
            {
                if (typesPatched.ContainsKey(type)) return typesPatched[type];

                var patchInfo = Harmony12.HarmonyMethodExtensions.GetHarmonyMethods(type);
                if (patchInfo == null || patchInfo.Count() == 0)
                {
                    Log.Error($"Failed to apply patch {type}: could not find Harmony attributes");
                    failedPatches.Add(featureName);
                    typesPatched.Add(type, false);
                    return false;
                }
                var processor = new Harmony12.PatchProcessor(harmonyInstance, type, Harmony12.HarmonyMethod.Merge(patchInfo));
                var patch = processor.Patch().FirstOrDefault();
                if (patch == null)
                {
                    Log.Error($"Failed to apply patch {type}: no dynamic method generated");
                    failedPatches.Add(featureName);
                    typesPatched.Add(type, false);
                    return false;
                }
                typesPatched.Add(type, true);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to apply patch {type}: {e}");
                failedPatches.Add(featureName);
                typesPatched.Add(type, false);
                return false;
            }
        }

        static void CheckPatchingSuccess()
        {
            // Check to make sure we didn't forget to patch something.
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var infos = Harmony12.HarmonyMethodExtensions.GetHarmonyMethods(type);
                if (infos != null && infos.Count() > 0 && !typesPatched.ContainsKey(type))
                {
                    Log.Write($"Did not apply patch for {type}");
                }
            }
        }

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            logger = modEntry.Logger;
            RES.Culture = new CultureInfo("zh-CN");
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            harmonyInstance = Harmony12.HarmonyInstance.Create(modEntry.Info.Id);
            if (!ApplyPatch(typeof(LibraryScriptableObject_LoadDictionary_Patch), "All mod features"))
            {
                // If we can't patch this, nothing will work, so want the mod to turn red in UMM.
                throw Error("Failed to patch LibraryScriptableObject.LoadDictionary(), cannot load mod");
            }
            return true;
        }

        static void WriteBlueprints()
        {
            Log.Append("\n--------------------------------------------------------------------------------");
            foreach (var blueprint in library.GetAllBlueprints())
            {
                Log.Append($"var {blueprint.name} = library.Get<{blueprint.GetType().Name}>(\"{blueprint.AssetGuid}\");");
            }
            Log.Append("--------------------------------------------------------------------------------\n");
            Log.Flush();
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!enabled) return;

            var fixedWidth = new GUILayoutOption[1] { GUILayout.ExpandWidth(false) };
            if (testedGameVersion != GameVersion.GetVersion())
            {
                GUILayout.Label(string.Format(RES.testedGameVersion_warning,testedGameVersion,GameVersion.GetVersion()), fixedWidth);
            }
            if (failedPatches.Count > 0)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(RES.failedPatches_error, fixedWidth);
                foreach (var featureName in failedPatches)
                {
                    GUILayout.Label(string.Format(RES.featureName_error,featureName), fixedWidth);
                }
                GUILayout.EndVertical();
            }
            if (failedLoading.Count > 0)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(RES.failedLoading_error, fixedWidth);
                foreach (var featureName in failedLoading)
                {
                    GUILayout.Label(string.Format(RES.featureName_error,featureName), fixedWidth);
                }
                GUILayout.EndVertical();
            }

            settings.EldritchKnightFix = GUILayout.Toggle(settings.EldritchKnightFix, RES.EldritchKnightFix_info, fixedWidth);

            settings.ShowCustomPortraits = GUILayout.Toggle(settings.ShowCustomPortraits, RES.ShowCustomPortraits_info, fixedWidth);

            settings.CheatCustomTraits = GUILayout.Toggle(settings.CheatCustomTraits,
                RES.CheatCustomTraits_info, fixedWidth);

            settings.DrawbackForextraTraits = GUILayout.Toggle(settings.DrawbackForextraTraits,
                RES.DrawbackForextraTraits_info, fixedWidth);

            settings.OracleHas3SkillPoints = GUILayout.Toggle(settings.OracleHas3SkillPoints,
                RES.OracleHas3SkillPoints_info, fixedWidth);
            OracleClass.MaybeUpdateSkillPoints();

            settings.RelaxAncientLorekeeper = GUILayout.Toggle(settings.RelaxAncientLorekeeper,
                RES.RelaxAncientLorekeeper_info, fixedWidth);

            settings.RelaxTonguesCurse = GUILayout.Toggle(settings.RelaxTonguesCurse,
                RES.RelaxTonguesCurse_info, fixedWidth);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        internal static void SafeLoad(Action load, String name)
        {
            try
            {
                load();
            }
            catch (Exception e)
            {
                failedLoading.Add(name);
                Log.Error(e);
            }
        }

        internal static T SafeLoad<T>(Func<T> load, String name)
        {
            try
            {
                return load();
            }
            catch (Exception e)
            {
                failedLoading.Add(name);
                Log.Error(e);
                return default(T);
            }
        }

        internal static Exception Error(String message)
        {
            logger?.Log(message);
            return new InvalidOperationException(message);
        }
    }


    public class Settings : UnityModManager.ModSettings
    {
        public bool EldritchKnightFix = true;

        public bool CheatCustomTraits = false;

        public bool DrawbackForextraTraits = true;

        public bool ShowCustomPortraits = false;

        public bool RelaxAncientLorekeeper = false;

        public bool RelaxTonguesCurse = false;

        public bool OracleHas3SkillPoints = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }
    }
}
