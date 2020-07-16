using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BepInEx;
using UnityEngine;
using HarmonyLib;

/*
 ** This mode is based on and inherits some functions from https://github.com/sinaioutlander/Outward-Mods
 ** I followed and learned a lot from Sinaioutlander's codes.
 ** This particular mode is simillar to "More Map Details" mode. You should check that out, it has more comments as well.
 */

namespace MapPlayerMarker
{
    [BepInPlugin(ID, NAME, VERSION)]
    public class PlayerMarker : BaseUnityPlugin
    {
        const string ID = "com.arman.mapplayermarker";
        const string NAME = "Map Player Marker";
        const string VERSION = "1.0";

        private static PlayerMarker _instance;
        private static Texture2D _customPlayerMarker;

        private int m_mapID;

        internal void Awake()
        {
            _instance = this;

            var harmony = new Harmony(ID);
            harmony.PatchAll();

            StartCoroutine(InitCoroutine());
        }

        private IEnumerator InitCoroutine()
        {
            while (MapDisplay.Instance == null)
            {
                yield return new WaitForSeconds(1.0f);
            }

        }

        [HarmonyPatch(typeof(MapDisplay), "Show", new Type[] { typeof(CharacterUI) })]
        public class MapDisplay_Show
        {
            [HarmonyPostfix]
            public static void Postfix(MapDisplay __instance, CharacterUI _owner)
            {
                var self = __instance;

                _instance.m_mapID = (int)AccessTools.GetValue(typeof(MapDisplay), self, "m_currentMapSceneID");

                if (!(bool)AccessTools.GetValue(typeof(MapDisplay), self, "m_currentAreaHasMap"))
                {
                    return;
                }

                if (MapConfigs.ContainsKey(_instance.m_mapID))
                {
                    self.CurrentMapScene.MarkerOffset = MapConfigs[_instance.m_mapID].MarkerOffset;
                    self.CurrentMapScene.Rotation = MapConfigs[_instance.m_mapID].Rotation;
                    self.CurrentMapScene.MarkerScale = MapConfigs[_instance.m_mapID].MarkerScale;
                }

                var characters = CharacterManager.Instance.Characters.Values
                    .Where(x =>
                        !x.GetComponentInChildren<MapWorldMarker>()
                        && !x.IsDead
                        && x.gameObject.activeSelf);

                foreach (Character c in characters)
                {
                    if (!c.IsAI)
                        _instance.AddWorldMarker(c.gameObject, c.Name);
                }
            }
        }

        private MapWorldMarker AddWorldMarker(GameObject go, string name)
        {
            var playerMarker = new GameObject("CustomPlayerMarker");
            playerMarker.transform.parent = go.transform;
            playerMarker.transform.position = go.transform.position;

            MapWorldMarker marker = playerMarker.AddComponent<MapWorldMarker>();
            marker.ShowCircle = true;
            marker.AlignLeft = false;
            marker.Text = name;

            return marker;
        }

        [HarmonyPatch(typeof(MapWorldMarkerDisplay), "UpdateMarkerDisplay", new Type[] {typeof(MapWorldMarker) })]
        public class Update_Player_Marker
        {
            [HarmonyPostfix]
            public static void Postfix(MapWorldMarkerDisplay __instance, MapWorldMarker _marker)
            {
                Debug.Log("Instance Player ID: " + __instance.PlayerID);
                Debug.Log("Instance name: " + __instance.name);
                Debug.Log("Instance toString: " + __instance.ToString());
                Debug.Log("Marker text: " + _marker.Text);
                Debug.Log("\n");
                if (String.Equals(_marker.gameObject.name, "CustomPlayerMarker"))
                {
                    _customPlayerMarker = LoadPNG(@"Mods\MapPlayerMarker\custom_player_marker.png");
                    __instance.Circle.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(_customPlayerMarker, new Rect(0, 0, _customPlayerMarker.width, _customPlayerMarker.height), new Vector2(0.0f, 0.0f));

                    var characters = CharacterManager.Instance.Characters.Values
                       .Where(x =>
                           !x.IsDead
                           && x.gameObject.activeSelf);

                    // TODO fix this shit
                    __instance.Circle.transform.rotation = Quaternion.Euler(0, 0, -characters.ElementAt(0).gameObject.transform.localRotation.eulerAngles.y);
                }
            }
        }

        public static Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }
            return tex;
        }

        // --- Map Config dictionary ---
        // Key: MapID (as per MapDisplay class)
        // Value: MapDependingScene settings. Only using the offset / rotation / scale values.
        public static Dictionary<int, MapConfig> MapConfigs = new Dictionary<int, MapConfig>
        {
            {
                1, // Chersonese
                new MapConfig()
                {
                    MarkerOffset = new Vector2(-531f, -543f),
                    MarkerScale = new Vector2(0.526f, 0.526f),
                    Rotation = 0f
                }
            },
            {
                3, // Hallowed Marsh
                new MapConfig()
                {
                    MarkerOffset = new Vector2(-573.0f, -515.0f),
                    MarkerScale = new Vector2(0.553f, 0.553f),
                    Rotation = 90f
                }
            },
            {
                5, // Abrassar
                new MapConfig()
                {
                    MarkerOffset = new Vector2(3f, -5f),
                    MarkerScale = new Vector2(0.534f, 0.534f),
                    Rotation = -90f
                }
            },
            {
                7, // Enmerkar Forest
                new MapConfig()
                {
                    MarkerOffset = new Vector2(-500f, -500f),
                    MarkerScale = new Vector2(0.5f, 0.5f),
                    Rotation = 0f
                }
            },
            {
                9, // Antique Plateau
                new MapConfig
                {
                    MarkerOffset = new Vector2(-504f, -505f),
                    MarkerScale = new Vector2(0.50f, 0.50f),
                    Rotation = 0f
                }
            }
        };

        public class MapConfig
        {
            public Vector2 MarkerOffset;
            public Vector2 MarkerScale;
            public float Rotation;
        }
    }
}
