using System;
using UnityEngine;
using VesselModuleSaveFramework;
using KSP_Log;
#if true
namespace AirPark
{
    public class APVesselModule : VesselModuleSave
    {
        #region Fields / Globals
        public Boolean Parked;
        public static Boolean autoPark;

        //Velocity and Postion
        private Vector3 ParkPosition;
        Vector3 ParkVelocity = new Vector3(0f, 0f, 0f);
        private Vector3 zeroVector = new Vector3(0f, 0f, 0f);
        private Vector3 ParkAcceleration = new Vector3(0f, 0f, 0f);
        private Vector3 ParkAngularVelocity = new Vector3(0f, 0f, 0f);

        //Vessel State
        Vessel.Situations previousState;

        //have you ever clicked "AirParked"? Rember to keep interesting things from happening
        public bool isActive = false;

        #region Debug Fields

        public bool partDebug = true;

        public string vesselSituation;

        #endregion DebugFields


        #endregion

        #region Toggles

        public void TogglePark()
        {
            // cannot Park in orbit or sub-orbit
            if (vessel.situation != Vessel.Situations.SUB_ORBITAL && vessel.situation != Vessel.Situations.ORBITING)
            {
                if (!Parked)
                {
                    ParkPosition = GetVesselPostion();

                    //we only want to remember the initial velocity, not subseqent updates by onFixedUpdate()
                    ParkVelocity = vessel.GetSrfVelocity();
                    ParkAcceleration = vessel.acceleration;
                    ParkAngularVelocity = vessel.angularVelocity;

                    ParkVessel();
                }
                else
                {
                    RestoreVesselState();
                }
                isActive = true;
            }
        }

        //[KSPEvent(guiActive = true, guiName = "Toggle Auto UnPark")] //auto park on will awake the vessel and set Parked = false if closer than 1.5 KM and inactive
        public void ToggleAutoPark()
        {
            autoPark = !autoPark;
        }
        #endregion

        //Log Log = null;
        protected void Start()
        {
            //Log = new Log("AirPark", Log.LEVEL.INFO);
        }
        #region GameEvents
        public override void VSMStart()
        {
            ParkPosition = vessel.transform.position;
        }
#if true
        public override ConfigNode VSMSave(ConfigNode node)
        {
            //if (FlightGlobals.ActiveVessel == this.vessel)
            {
                node.AddValue("Parked", Parked);
                node.AddValue("autoPark", autoPark);
                node.AddValue("ParkPosition", ParkPosition); // Vector3

                node.AddValue("ParkVelocity", ParkVelocity); // Vector3
                node.AddValue("ParkAcceleration", ParkAcceleration); // Vector3
                node.AddValue("ParkAngularVelocity", ParkAngularVelocity); // Vector3

                node.AddValue("previousState", (int)previousState); // Vessel.Situations
                node.AddValue("isActive", isActive); // bool

                node.AddValue("partDebug", partDebug); // bool

                node.AddValue("vesselSituation", vesselSituation); // string

            }
            return node;
        }
#endif
#if true
        public override void VSMLoad(ConfigNode node)
        {
            Parked = node.SafeLoad("Parked", false);
            autoPark = node.SafeLoad("autoPark", false);
            ParkPosition = node.SafeLoad("ParkPosition", new Vector3(0, 0, 0));
            ParkVelocity = node.SafeLoad("ParkVelocity", new Vector3(0, 0, 0));
            ParkAcceleration = node.SafeLoad("ParkAcceleration", new Vector3(0, 0, 0));
            ParkAngularVelocity = node.SafeLoad("ParkAngularVelocity", new Vector3(0, 0, 0));
            // previousState
            int i = node.SafeLoad("previousState", 0);
            previousState = (Vessel.Situations)i;

            isActive = node.SafeLoad("isActive", false);
            partDebug = node.SafeLoad("partDebug", false);
            vesselSituation = node.SafeLoad("vesselSituation", "");
        }
#endif
        public void FixedUpdate()
        {
            vesselSituation = vessel.situation.ToString();

            #region can't Park if we're orbitingParkPosition
            if (vessel.situation == Vessel.Situations.SUB_ORBITAL || vessel.situation == Vessel.Situations.ORBITING && vessel.isActiveVessel)
            {
                autoPark = false;
                Parked = false;
                ScreenMessages.PostScreenMessage("Cannot Park While Sub-Orbital or Orbital", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            #endregion

            #region If we are the Inactive Vessel and AutoPark is set
            if (!vessel.isActiveVessel & autoPark)
            {
                {
                    var f = (vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude;
                }
                //ParkPosition = vessel.GetWorldPos3D();
                // if we're less than 1.5km from the active vessel and Parked, then wake up
                if (Parked && (vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude < 1500.0f & Parked)
                {
                    vessel.GoOffRails();
                    RestoreVesselState();
                }
                // if we're farther than 2km, auto Park if needed
                if (!Parked && (vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude > 2000.0f & Parked == false)
                {
                    ParkVessel();
                }
            }
            #endregion

            #region if we're not Parked, and not active and flying, then go off rails
            // I dont think it matters if we are flying when I remember previous state - gomker
            //if (!vessel.isActiveVessel & Parked==false & vessel.situation == Vessel.Situations.FLYING)
            //{
            //    vessel.GoOffRails();
            //    RestoreVesselState();
            //}
            #endregion

            //If Parked is True, Park the Vessel
            if (Parked)
            {
                ParkVessel();
            }

        }
        public override void VSMDestroy()
        {
            //instance = null;
        }

        #endregion

        #region vessel states
        private void RememberPreviousState()
        {
            if (!Parked & vessel.situation != Vessel.Situations.LANDED) //Keep from Vessel Situation from Sticking to Landed permanently
            {
                previousState = vessel.situation;
            }
        }

        private void RestoreVesselState()
        {
            if (isActive == false) { return; } //we only want to restore the state if you have parked somewhere intentionally
            vessel.situation = previousState;
            if (vessel.situation != Vessel.Situations.LANDED) { vessel.Landed = false; }
            if (Parked) { Parked = false; }

            setVesselStill();

            //Restore Velocity and Accleration
            vessel.IgnoreGForces(240);
            vessel.SetWorldVelocity(ParkVelocity);
            vessel.acceleration = ParkAcceleration;
            vessel.angularVelocity = ParkAngularVelocity;

        }

        private void ParkVessel()
        {
            RememberPreviousState();
            setVesselStill();

            vessel.situation = Vessel.Situations.LANDED;
            vessel.Landed = true;
            Parked = true;
        }

        private void setVesselStill()
        {
            vessel.IgnoreGForces(240);
            vessel.SetWorldVelocity(zeroVector);
            vessel.acceleration = zeroVector;
            vessel.angularVelocity = zeroVector;
            vessel.geeForce = 0.0;
            setVesselPosition();

        }
        #endregion

        #region Postion
        //Code Adapted from Hyperedit landing functions 
        //https://github.com/Ezriilc/HyperEdit

        public CelestialBody Body { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double alt;
        public Vector3d teleportPosition;

        public void SetAltitudeToCurrent()
        {
            var pqs = Body.pqsController;
            if (pqs == null)
            {
                Destroy(this);
                return;
            }
            var alt = pqs.GetSurfaceHeight(QuaternionD.AngleAxis(Longitude, Vector3d.down) * QuaternionD.AngleAxis(Latitude, Vector3d.forward) * Vector3d.right) - pqs.radius;
            //alt = Math.Max(alt, 0); // No need for underwater check, allow park subs
            Altitude = GetComponent<Vessel>().altitude - alt;
        }

        private Vector3d GetVesselPostion()
        {
            var pqs = vessel.mainBody.pqsController;
            if (pqs == null)
            {
                Destroy(this);
                return zeroVector;
            }

            alt = pqs.GetSurfaceHeight(vessel.mainBody.GetRelSurfaceNVector(Latitude, Longitude)) - vessel.mainBody.Radius;
            alt = Math.Max(alt, 0); // Underwater!

            teleportPosition = vessel.mainBody.GetRelSurfacePosition(Latitude, Longitude, alt + Altitude);

            return teleportPosition;
        }

        private void setVesselPosition()
        {
            vessel.IgnoreGForces(240);
            vessel.orbitDriver.pos = ParkPosition;
        }
        #endregion
    }
}

#endif