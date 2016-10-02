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

        //bool showMoveHelp = false;
        float helpHeight;

        void Start()
        {
            toolbarPosition = new Vector2(Screen.width - toolbarWidth - 60, 39);
            toolbarRect = new Rect(toolbarPosition.x, toolbarPosition.y + 100, toolbarWidth, toolbarHeight);
            contentWidth = toolbarWidth - (2 * toolbarMargin);

            AddToolbarButton();
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

            if (!AirPark.Parked)
            {
                if (GUI.Button(LineRect(ref line, 1.5f), "Park Vessel", HighLogic.Skin.button))
                {
                    //VesselMove.instance.StartMove(FlightGlobals.ActiveVessel, true);
                    if (FlightGlobals.ActiveVessel && !FlightGlobals.ActiveVessel.Landed) //{ AirParkVM.ParkPosition = AirParkVM.GetVesselPostion(); }
                    AirPark.Instance.TogglePark();
                }

            }
            else
            {
                if (GUI.Button(LineRect(ref line, 2), "Un-Park", HighLogic.Skin.button))
                {
                    //if (AirParkVM.Parked) { AirParkVM.RestoreVesselState(); }
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
                    //AirParkPM.autoPark = !AirParkPM.autoPark;
                    AirPark.Instance.ToggleAutoPark();
                }

            }
            else
            {
                if (GUI.Button(spawnVesselRect, "Auto-Park ON", HighLogic.Skin.button))
                {
                    //AirParkPM.autoPark = !AirParkPM.autoPark;
                    AirPark.Instance.ToggleAutoPark();
                }

            }


            toolbarRect.height = (line * toolbarLineHeight) + (toolbarMargin * 2);
        }

        Rect LineRect(ref float currentLine, float heightFactor = 1)
        {
            Rect rect = new Rect(toolbarMargin, toolbarMargin + (currentLine * toolbarLineHeight), contentWidth, toolbarLineHeight * heightFactor);
            currentLine += heightFactor + 0.1f;
            return rect;
        }

        void MoveHelp(int windowID)
        {
            float line = 0;
            line += 1.25f;
            LineLabel("Movement: " + GameSettings.PITCH_DOWN.primary.ToString() + " " +
                GameSettings.PITCH_UP.primary.ToString() + " " +
                GameSettings.YAW_LEFT.primary.ToString() + " " +
                GameSettings.YAW_RIGHT.primary.ToString(), ref line);
            LineLabel("Roll: " + GameSettings.ROLL_LEFT.primary.ToString() + " " +
                GameSettings.ROLL_RIGHT.primary.ToString(), ref line);
            LineLabel("Pitch: " + GameSettings.TRANSLATE_DOWN.primary.ToString() + " " +
                GameSettings.TRANSLATE_UP.primary.ToString(), ref line);
            LineLabel("Yaw: " + GameSettings.TRANSLATE_LEFT.primary.ToString() + " " +
                GameSettings.TRANSLATE_RIGHT.primary.ToString(), ref line);
            LineLabel("Auto rotate rocket: " + GameSettings.TRANSLATE_BACK.primary.ToString(), ref line);
            LineLabel("Auto rotate plane: " + GameSettings.TRANSLATE_FWD.primary.ToString(), ref line);
            LineLabel("Change movement speed: Tab", ref line);
            //line++;
            //LineLabel("Hotkey: Alt + P", ref line);

            helpHeight = (line * toolbarLineHeight) + (toolbarMargin * 2);
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
