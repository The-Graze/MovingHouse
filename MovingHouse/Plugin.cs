using BepInEx;
using BepInEx.Configuration;
using GorillaNetworking;
using OVR.OpenVR;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using Utilla;

namespace MovingHouse
{
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public ConfigEntry<string> Home;
        public string ChosenHome;
        SpawnManager SpawnManager;
        Transform[] _HomeLocations;
        Dictionary<string, Transform> _HomeLocationsDict;
        public static volatile Plugin p;
        GorillaGeoHideShowTrigger[] geos;
        void Awake()
        {
            Home = Config.Bind("Settings", "StartHome", "basement", "Accepted Values: 'stump','mountain', 'skyjungle, 'basement' and 'beach'");
            ChosenHome = Home.Value;
            p = this;
        }
        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }
        void OnGameInitialized(object sender, EventArgs e)
        {
            SpawnManager = FindObjectOfType<SpawnManager>();
            _HomeLocations = SpawnManager.ChildrenXfs();
            _HomeLocationsDict = new Dictionary<string, Transform>
            {
                { "stump", null },
                { "mountain", _HomeLocations[5] },
                { "skyjungle", _HomeLocations[9] },
                { "basement", _HomeLocations[11] },
                { "beach", _HomeLocations[13] }
            };
            _HomeLocations[13].position = new Vector3(28, 11, 2f);
            _HomeLocations[9].position = new Vector3(-75, 163, -100);
            _HomeLocations[5].position = new Vector3(-26, 18, -96);
            geos = FindObjectsOfType<GorillaGeoHideShowTrigger>();
            if (ChosenHome != "stump")
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().transform.localPosition = Vector3.zero;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().transform.parent.position = _HomeLocationsDict[ChosenHome].position;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().transform.localPosition = Vector3.zero;
                GorillaLocomotion.Player.Instance.InitializeValues();
                //new GameObject("DEBUG-SpawnPoints");
               // DEBUGshowSpwans();
            }
            foreach (Camera c in Camera.allCameras)
            {
                c.clearFlags = CameraClearFlags.Skybox;
            }
        }

        void DEBUGshowSpwans()
        {
            foreach(Transform t in _HomeLocations)
            {
                GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
                a.transform.SetParent(GameObject.Find("DEBUG-SpawnPoints").transform);
                a.transform.position = t.position;
                a.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                a.name = t.name;
                Destroy(a.GetComponent<Collider>());
                a.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            }
        }
        public void Update()
        {
            if (PhotonNetworkController.Instance.currentState == PhotonNetworkController.ConnectionState.DeterminingPingsAndPlayerCount || PhotonNetworkController.Instance.currentState == PhotonNetworkController.ConnectionState.Initialization)
            {
                if (ChosenHome == "mountain")
                {
                    geos[9].OnBoxTriggered();
                }
                if (ChosenHome == "skyjungle")
                {
                    geos[3].OnBoxTriggered();
                    geos[10].OnBoxTriggered();
                }
                if (ChosenHome == "basement")
                {
                    geos[3].OnBoxTriggered();
                    geos[19].OnBoxTriggered();
                }
                if (ChosenHome == "beach")
                {
                    geos[16].OnBoxTriggered();
                    GameObject.Find("Level").transform.GetChild(14).gameObject.SetActive(true);
                }
            }
        }
    }
}
