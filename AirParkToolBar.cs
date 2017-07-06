//Code Adapted from https://github.com/BahamutoD/VesselMover/blob/master/VesselMoverToolbar.cs

using System;
using System.Collections;
using UnityEngine;
using KSP.UI.Screens;

namespace AirPark
{
    
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class AirParkToolbar : MonoBehaviour
    {
       
        public static bool hasAddedButton = false;
        public static bool toolbarGuiEnabled = false;

        Rect toolbarRect;
        float toolbarWidth = 280;
        float toolbarHeight = 0;
        float toolbarMargin = 6;
        float toolbarLineHeight = 20;
        float contentWidth;
        Vector2 toolbarPosition;
        Rect svRectScreenSpace;
        
        void VesselChange(Vessel v)
        {
            if (!v.isActiveVessel) return;
        }

        void Start()
        {
            toolbarPosition = new Vector2(Screen.width - toolbarWidth - 80, 50);
            toolbarRect = new Rect(toolbarPosition.x, toolbarPosition.y + 100, toolbarWidth, toolbarHeight);
            contentWidth = toolbarWidth - (2 * toolbarMargin);

            AddToolbarButton();

            GameEvents.onVesselChange.Add(VesselChange);
        }

        void OnGUI()
        {
            if (toolbarGuiEnabled) //&& AirParkPM.instance)
            {
                GUI.Window(999666, toolbarRect, ToolbarWindow, "AirPark", HighLogic.Skin.window);
            }
        }

        void ToolbarWindow(int windowID)
        {
            float line = 0;
            line += 1.25f;

            if (!FlightGlobals.ActiveVessel) { return; }

            if (AirPark.Instance)
            {
                if (!AirPark.Parked)
                {
                    if (GUI.Button(LineRect(ref line, 1.5f), "Park Vessel", HighLogic.Skin.button))
                    {
                        if (FlightGlobals.ActiveVessel && !FlightGlobals.ActiveVessel.Landed)
                            AirPark.Instance.TogglePark();
                    }

                }
                else
                {
                    if (GUI.Button(LineRect(ref line, 2), "Un-Park", HighLogic.Skin.button))
                    {
                        AirPark.Instance.TogglePark();
                    }
                }

                line += 0.2f;
                Rect spawnVesselRect = LineRect(ref line);
                svRectScreenSpace = new Rect(spawnVesselRect);
                svRectScreenSpace.x += toolbarRect.x;
                svRectScreenSpace.y += toolbarRect.y;

                if (!AirPark.autoPark)
                {
                    if (GUI.Button(spawnVesselRect, "Auto-Park OFF", HighLogic.Skin.button))
                    {
                        AirPark.Instance.ToggleAutoPark();
                    }

                }
                else
                {
                    if (GUI.Button(spawnVesselRect, "Auto-Park ON", HighLogic.Skin.button))
                    {
                        AirPark.Instance.ToggleAutoPark();
                    }

                }
            }
            else
            {
                GUIStyle centerLabelStyle = new GUIStyle(HighLogic.Skin.label) { alignment = TextAnchor.UpperCenter };
                GUI.Label(LineRect(ref line), "No AirPark Module Found", centerLabelStyle);
            }

            toolbarRect.height = (line * toolbarLineHeight) + (toolbarMargin * 2);
        }

        Rect LineRect(ref float currentLine, float heightFactor = 1)
        {
            Rect rect = new Rect(toolbarMargin, toolbarMargin + (currentLine * toolbarLineHeight), contentWidth, toolbarLineHeight * heightFactor);
            currentLine += heightFactor + 0.1f;
            return rect;
        }

        void LineLabel(string label, ref float line)
        {
            GUI.Label(LineRect(ref line), label, HighLogic.Skin.label);
        }

        void AddToolbarButton()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!hasAddedButton)
                {
                    Texture buttonTexture = GameDatabase.Instance.GetTexture("AirPark/Icon/AirPark", false);
                    ApplicationLauncher.Instance.AddModApplication(ShowToolbarGUI, HideToolbarGUI, Dummy, Dummy, Dummy, Dummy, ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                    hasAddedButton = true;
                }
            }
        }

        public void ShowToolbarGUI()
        {
            AirParkToolbar.toolbarGuiEnabled = true;
        }

        public void HideToolbarGUI()
        {
            AirParkToolbar.toolbarGuiEnabled = false;
        }

        void Dummy()
        { }

        public static bool MouseIsInRect(Rect rect)
        {
            return rect.Contains(MouseGUIPos());
        }

        public static Vector2 MouseGUIPos()
        {
            return new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 0);
        }
    }
}
