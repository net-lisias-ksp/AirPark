using System;
using System.Text;
using UnityEngine;

namespace AirPark
{
    //public class AirParkVM : VesselModule
    //{
    //    public static AirParkVM instance;
    //    public static Vessel vessel {get; private set;}

    //    #region Fields / Globals
    //    //[KSPField(isPersistant = true, guiActive = true, guiName = "AirParked")]
    //    public static Boolean Parked;
    //    //[KSPField(isPersistant = true, guiActive = true, guiName = "Auto UnPark")]
    //    public static Boolean autoPark;

    //    //Velocity and Postion
    //    [KSPField(isPersistant = true, guiActive = false)]
    //    //private Vector3 ParkPosition = new Vector3(0f, 0f, 0f);
    //    public static Vector3 ParkPosition;

    //    //[KSPField(isPersistant = true, guiActive = false)]
    //    public static Vector3 ParkVelocity = new Vector3(0f, 0f, 0f);
    //    public static Vector3 zeroVector = new Vector3(0f, 0f, 0f);
    //    //[KSPField(isPersistant = true, guiActive = false)]
    //    public static Vector3 ParkAcceleration = new Vector3(0f, 0f, 0f);
    //    //[KSPField(isPersistant = true, guiActive = false)]
    //    public static Vector3 ParkAngularVelocity = new Vector3(0f, 0f, 0f);

    //    //Vessel State
    //    //[KSPField(isPersistant = true, guiActive = true)] //flip to false guiactive on release
    //    public static Vessel.Situations previousState;
    //    //[KSPField(isPersistant = true, guiActive = false)]

    //    //have you ever clicked "AirParked"? Rember to keep interesting things from happening
    //    public static bool isActive = false;

    //    #region Debug Fields

    //    [KSPField(isPersistant = true)]
    //    public bool partDebug = true;

    //    [KSPField(guiActive = true, isPersistant = false, guiName = "Current Situation")]
    //    public string vesselSituation;

    //    #endregion DebugFields

    //    #endregion

    //    #region Toggles

    //    public static void TogglePark()
    //    {
    //        // cannot Park in orbit or sub-orbit
    //        if (vessel.situation != Vessel.Situations.SUB_ORBITAL && vessel.situation != Vessel.Situations.ORBITING)
    //        {
    //            if (!Parked)
    //            {
    //                ParkPosition = GetVesselPostion();

    //                //we only want to remember the initial velocity, not subseqent updates by onFixedUpdate()
    //                ParkVelocity = vessel.GetSrfVelocity();
    //                ParkAcceleration = vessel.acceleration;
    //                ParkAngularVelocity = vessel.angularVelocity;

    //                ParkVessel();
    //            }
    //            else
    //            {
    //                RestoreVesselState();
    //            }
    //            isActive = true;
    //        }
    //    }

    //    //[KSPEvent(guiActive = true, guiName = "Toggle Auto UnPark")] //auto park on will awake the vessel and set Parked = false if closer than 1.5 KM and inactive
    //    public void ToggleAutoPark()
    //    {
    //        autoPark = !autoPark;
    //    }
    //    #endregion

    //    #region GameEvents

    //   public void Start()
    //    {
    //        vessel = GetComponent<Vessel>();

    //        if (instance)
    //        {
    //            Destroy(instance);
    //            instance = this;
    //        }
    //        else
    //        {
    //            instance = this;
    //        }
    //    }

    //    //public override void OnSave(ConfigNode node)
    //    //{
    //    //    base.OnSave(node);
    //    //    if (vessel != null)
    //    //    {

    //    //        ParkPosition = GetVesselPostion();
    //    //    }

    //    //}

    //    public void FixedUpdate()
    //    {
    //        //Set debug values
    //        if (vessel == null) { return; }
    //        vesselSituation = vessel.situation.ToString();

    //        #region can't Park if we're orbitingParkPosition
    //        if (vessel.situation == Vessel.Situations.SUB_ORBITAL || vessel.situation == Vessel.Situations.ORBITING)
    //        {
    //            autoPark = false;
    //            Parked = false;
    //        }
    //        #endregion

    //        #region If we are the Inactive Vessel and AutoPark is set
    //        if (!vessel.isActiveVessel & autoPark)
    //        {
    //            //ParkPosition = vessel.GetWorldPos3D();
    //            // if we're less than 1.5km from the active vessel and Parked, then wake up
    //            if ((vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude < 1500.0f & Parked)
    //            {
    //                vessel.GoOffRails();
    //                RestoreVesselState();
    //            }
    //            // if we're farther than 2km, auto Park if needed
    //            if ((vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude > 2000.0f & Parked == false)
    //            {
    //                ParkVessel();
    //            }
    //        }
    //        #endregion

    //        #region if we're not Parked, and not active and flying, then go off rails
    //        // I dont think it matters if we are flying when I remember previous state - gomker
    //        //if (!vessel.isActiveVessel & Parked==false & vessel.situation == Vessel.Situations.FLYING)
    //        //{
    //        //    vessel.GoOffRails();
    //        //    RestoreVesselState();
    //        //}
    //        #endregion

    //        //If Parked is True, Park the Vessel
    //        if (Parked)
    //        {
    //            ParkVessel();
    //        }

    //    }
    //    #endregion

    //    #region vessel states
    //    public static void RememberPreviousState()
    //    {
    //        if (!Parked & vessel.situation != Vessel.Situations.LANDED) //Keep from Vessel Situation from Sticking to Landed permanently
    //        {
    //            previousState = vessel.situation;
    //        }
    //    }

    //    public static void RestoreVesselState()
    //    {
    //        if (isActive == false) { return; } //we only want to restore the state if you have parked somewhere intentionally
    //        vessel.situation = previousState;
    //        if (vessel.situation != Vessel.Situations.LANDED) { vessel.Landed = false; }
    //        if (Parked) { Parked = false; }

    //        setVesselStill();

    //        //Restore Velocity and Accleration
    //        vessel.SetWorldVelocity(ParkVelocity);
    //        vessel.acceleration = ParkAcceleration;
    //        vessel.angularVelocity = ParkAngularVelocity;

    //    }

    //    public static void ParkVessel()
    //    {
    //        RememberPreviousState();
    //        setVesselStill();

    //        vessel.situation = Vessel.Situations.LANDED;
    //        vessel.Landed = true;
    //        Parked = true;
    //    }

    //    public static void setVesselStill()
    //    {
    //        vessel.SetWorldVelocity(zeroVector);
    //        vessel.acceleration = zeroVector;
    //        vessel.angularVelocity = zeroVector;
    //        vessel.geeForce = 0.0;
    //        setVesselPosition();

    //    }
    //    #endregion

    //    #region Postion
    //    //Code Adapted from Hyperedit landing functions 
    //    //https://github.com/Ezriilc/HyperEdit

    //    public static CelestialBody Body { get; set; }
    //    public static double Latitude { get; set; }
    //    public static double Longitude { get; set; }
    //    public static double Altitude { get; set; }
    //    public static double alt;
    //    public static Vector3d teleportPosition;

    //    public void SetAltitudeToCurrent()
    //    {
    //        var pqs = Body.pqsController;
    //        if (pqs == null)
    //        {
    //            Destroy(this);
    //            return;
    //        }
    //        var alt = pqs.GetSurfaceHeight(QuaternionD.AngleAxis(Longitude, Vector3d.down) * QuaternionD.AngleAxis(Latitude, Vector3d.forward) * Vector3d.right) - pqs.radius;
    //        //alt = Math.Max(alt, 0); // No need for underwater check, allow park subs
    //        Altitude = GetComponent<Vessel>().altitude - alt;
    //    }

    //    public static Vector3d GetVesselPostion()
    //    {
    //        var pqs = vessel.mainBody.pqsController;
    //        if (pqs == null)
    //        {
    //            //Destroy(this);
    //            return zeroVector;
    //        }

    //        alt = pqs.GetSurfaceHeight(vessel.mainBody.GetRelSurfaceNVector(Latitude, Longitude)) - vessel.mainBody.Radius;
    //        alt = Math.Max(alt, 0); // Underwater!

    //        teleportPosition = vessel.mainBody.GetRelSurfacePosition(Latitude, Longitude, alt + Altitude);

    //        return teleportPosition;
    //    }

    //    public static void setVesselPosition()
    //    {
    //        vessel.orbitDriver.pos = ParkPosition;
    //    }
    //    #endregion
    //}

}

