using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using GlobalEnums;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("io.larissarosalene.CheatPack4", "Cheat Pack 4", "3.0.1")]
public class CheatPack4 : BaseUnityPlugin
{
    private static ConfigEntry<bool> cheatHealth;
    private static ConfigEntry<bool> cheatSilk;
    private static ConfigEntry<bool> cheatShards;
    private static ConfigEntry<bool> cheatMoney;
    private static ConfigEntry<bool> cheatDeathMoney;
    private static ConfigEntry<bool> cheatDeathSpool;
    private static ConfigEntry<bool> cheatCompass;
    private static ConfigEntry<bool> cheatMagnet;
    private static ConfigEntry<bool> cheatDeliveries;

    private static FieldInfo gmCurrentSceneObj;
    private static FieldInfo gmShadeMarker;
    private static FieldInfo gmCompassIcon;
    private static FieldInfo gmDisplayingCompass;
    private static MethodInfo iwmPositionIcon;
    private static FieldInfo iwmCompassIcon;
    private static FieldInfo cobHasMagnetTool;

    private void Awake()
    {
        cheatHealth = Config.Bind("General", "cheatHealth", false, "Don't lose health on hits");
        cheatSilk = Config.Bind(
            "General",
            "cheatSilk",
            false,
            "Don't spend silk on skills and binds"
        );
        cheatShards = Config.Bind(
            "General",
            "cheatShards",
            false,
            "Don't spend shards on donations/tool crafting"
        );
        cheatMoney = Config.Bind("General", "cheatMoney", false, "Don't spend rosaries in shops");
        cheatDeathMoney = Config.Bind(
            "General",
            "cheatDeathMoney",
            false,
            "Don't lose rosaries on death"
        );
        cheatDeathSpool = Config.Bind(
            "General",
            "cheatDeathSpool",
            false,
            "Don't break spool on death"
        );
        cheatCompass = Config.Bind("General", "cheatCompass", false, "Compass always active");
        cheatMagnet = Config.Bind("General", "cheatMagnet", false, "Magnet always active");
        cheatDeliveries = Config.Bind(
            "General",
            "cheatDeliveries",
            false,
            "Taking damage doesn't affect deliveries (also stops timer on Courier's Rasher)"
        );

        gmCurrentSceneObj = typeof(GameMap).GetField(
            "currentSceneObj",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        gmShadeMarker = typeof(GameMap).GetField(
            "shadeMarker",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        gmCompassIcon = typeof(GameMap).GetField(
            "compassIcon",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        gmDisplayingCompass = typeof(GameMap).GetField(
            "displayingCompass",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        iwmPositionIcon = typeof(InventoryWideMap).GetMethod(
            "PositionIcon",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        iwmCompassIcon = typeof(InventoryWideMap).GetField(
            "compassIcon",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        cobHasMagnetTool = typeof(CurrencyObjectBase).GetField(
            "hasMagnetTool",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        var harmony = new Harmony("io.larissarosalene.CheatPack4");
        harmony.PatchAll(typeof(CheatPack4));
        harmony.PatchAll(typeof(LiquidTranspiler));

        Logger.LogInfo("Cheat Pack 4 loaded");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerData), "TakeHealth")]
    private static void TakeHealthPrefix(PlayerData __instance, out int __state)
    {
        __state = __instance.health;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerData), "TakeHealth")]
    private static void TakeHealthPostfix(PlayerData __instance, int __state)
    {
        if (cheatHealth.Value)
        {
            __instance.health = __state;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerData), "TakeSilk")]
    private static bool TakeSilkPrefix()
    {
        return !cheatSilk.Value;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerData), "TakeGeo")]
    private static bool TakeGeoPrefix()
    {
        return !cheatMoney.Value;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerData), "TakeShards")]
    private static bool TakeShardsPrefix()
    {
        return !cheatShards.Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HeroController), "Die")]
    private static void DiePostfix(HeroController __instance, ref IEnumerator __result)
    {
        var patchedEnumerator = new PatchedDeathEnumerator()
        {
            original = __result,
            shouldCheatMoney = cheatDeathMoney.Value,
            shouldCheatSpool = cheatDeathSpool.Value,
            pd = __instance.playerData,
        };
        __result = patchedEnumerator.GetEnumerator();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameMap), "PositionCompassAndCorpse")]
    private static bool PositionCompassAndCorpsePrefix(GameMap __instance)
    {
        if (cheatCompass.Value)
        {
            __instance.UpdateCurrentScene();
            var currentSceneObj = gmCurrentSceneObj.GetValue(__instance) as GameObject;
            var shadeMarker = gmShadeMarker.GetValue(__instance) as ShadeMarkerArrow;
            var compassIcon = gmCompassIcon.GetValue(__instance) as GameObject;
            if (currentSceneObj != null)
            {
                if (!__instance.IsLostInAbyssPreMap())
                {
                    compassIcon.SetActive(value: true);
                    gmDisplayingCompass.SetValue(__instance, true);
                }
                else
                {
                    compassIcon.SetActive(value: false);
                    gmDisplayingCompass.SetValue(__instance, false);
                }
            }
            shadeMarker.SetPosition(__instance.GetCorpsePosition());
        }
        return !cheatCompass.Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(InventoryWideMap), "UpdatePositions")]
    private static void UpdatePositionsPostfix(InventoryWideMap __instance)
    {
        if (cheatCompass.Value)
        {
            var instance = GameManager.instance;
            if ((bool)instance.gameMap)
            {
                instance.gameMap.UpdateCurrentScene();
                MapZone zoneForBounds;
                Vector2 compassPositionLocalBounds = instance.gameMap.GetCompassPositionLocalBounds(
                    out zoneForBounds
                );
                iwmPositionIcon.Invoke(
                    __instance,
                    new object[]
                    {
                        iwmCompassIcon.GetValue(__instance),
                        compassPositionLocalBounds,
                        !instance.gameMap.IsLostInAbyssPreMap(),
                        zoneForBounds,
                    }
                );
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CurrencyObjectBase), "MagnetToolIsEquipped")]
    private static void MagnetToolIsEquippedPostfix(
        CurrencyObjectBase __instance,
        ref bool __result
    )
    {
        if (cheatMagnet.Value)
        {
            __result = (bool)cobHasMagnetTool.GetValue(__instance);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HeroController), "TickDeliveryItems")]
    private static bool TickDeliveryItemsPrefix()
    {
        return !cheatDeliveries.Value;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(DeliveryQuestItem),
        "TakeHitForItem",
        typeof(DeliveryQuestItem.ActiveItem),
        typeof(bool),
        typeof(int)
    )]
    private static bool TakeHitForItemPrefix()
    {
        return !cheatDeliveries.Value;
    }

    public static bool IsShardCheatEnabled()
    {
        return cheatShards.Value;
    }
}
