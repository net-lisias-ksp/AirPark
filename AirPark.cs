using System;
using UnityEngine;

namespace AirPark
{
        public class AirPark : PartModule
        {
		[KSPField(isPersistant = true, guiActive = true, guiName = "AirParked")]
		Boolean Parked;
		[KSPField(isPersistant = true, guiActive = true, guiName = "Auto UnPark")]
		Boolean autoPark;
		
        //Velocity and Postion
        [KSPField(isPersistant = true, guiActive = false)]
        //private Vector3 ParkPosition = new Vector3(0f, 0f, 0f);
        private Vector3 ParkPosition;
		
        [KSPField(isPersistant = true, guiActive = false)]
		Vector3 ParkVelocity = new Vector3(0f, 0f, 0f);
        private static Vector3 zeroVector = new Vector3(0f, 0f, 0f);
        [KSPField(isPersistant = true, guiActive = false)]
        private Vector3 ParkAcceleration = new Vector3(0f, 0f, 0f);
        [KSPField(isPersistant = true, guiActive = false)]
        private Vector3 ParkAngularVelocity = new Vector3(0f, 0f, 0f);

        //Vessel State
        [KSPField(isPersistant = true, guiActive = true)] //flip to false guiactive on release
        Vessel.Situations previousState;
        [KSPField(isPersistant = true, guiActive = false)]

        //have you ever clicked "AirParked"? Rember to keep interesting things from happening
        public bool isActive = false; 

        #region Debug Fields

            [KSPField(isPersistant = true)]
            public bool partDebug = true;

            [KSPField(guiActive = true, isPersistant = false, guiName = "Current Situation")]
            public string vesselSituation;

        #endregion DebugFields    

       [KSPEvent(guiActive = true, guiName = "Toggle Park")]
        public void TogglePark()
        {
            // cannot Park in orbit or sub-orbit
            if (vessel.situation != Vessel.Situations.SUB_ORBITAL && vessel.situation != Vessel.Situations.ORBITING)
            {
                if (!Parked)
                {
                    //ParkPosition = vessel.GetWorldPos3D();
                    ParkPosition = vessel.transform.position;                    

                    //we only want to remember the initial velocity, not subseqent updates by onFixedUpdate()
                    //ParkVelocity = vessel.GetSrfVelocity(); 
                    //ParkAcceleration = vessel.acceleration;
                    //ParkAngularVelocity = vessel.angularVelocity;   

                    ParkVessel();
                }
                else
                {
                   RestoreVesselState();
                }
                isActive = true;
            }
        }

        [KSPEvent(guiActive = true, guiName = "Toggle Auto UnPark")] //auto park on will awake the vessel and set Parked = false if closer than 1.5 KM and inactive
        public void ToggleAutoPark()
        {
            autoPark = !autoPark;
        }

		public override void OnStart(StartState state)
		{
			if (state != StartState.Editor)
			{
                if (vessel != null)
                {                    
                    part.force_activate();
                    //RememberPreviousState();
                    ParkPosition = vessel.transform.position;
                }
			}
		}
        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            if (vessel != null ){ 
                //ParkPosition = vessel.GetWorldPos3D(); 
                ParkPosition = vessel.transform.position;
                //ParkVelocity = vessel.GetSrfVelocity(); 
                //ParkAcceleration = vessel.acceleration;
                //ParkAngularVelocity = vessel.angularVelocity;   
            }
            
        }
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (vessel != null)
            {
                //ParkPosition = vessel.GetWorldPos3D(); 
                //ParkPosition = vessel.transform.position;
                //ParkVelocity = vessel.GetSrfVelocity(); 
                //ParkAcceleration = vessel.acceleration;
                //ParkAngularVelocity = vessel.angularVelocity;   
            }
        }

        public void FixedUpdate()
        {
            //Set debug values
            vesselSituation = vessel.situation.ToString();

            #region can't Park if we're orbitingParkPosition
            if (vessel.situation == Vessel.Situations.SUB_ORBITAL || vessel.situation == Vessel.Situations.ORBITING)
            {
                autoPark = false;
                Parked = false;
            }
            #endregion

            #region If we are the Inactive Vessel and AutoPark is set
            if (!vessel.isActiveVessel & autoPark)
            {
                //ParkPosition = vessel.GetWorldPos3D();
                // if we're less than 1.5km from the active vessel and Parked, then wake up
                if ((vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude < 1500.0f & Parked)
                {
                    vessel.GoOffRails();
                    RestoreVesselState();
                }
                // if we're farther than 2km, auto Park if needed
                if ((vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude > 2000.0f & Parked==false)
                {
                   ParkVessel();
                }
            }
            #endregion
            
            #region if we're not Parked, and not active and flying, then go off rails
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
            vessel.SetPosition(ParkPosition);

            //Restore Velocity and Accleration
            //vessel.SetWorldVelocity(ParkVelocity);
            //vessel.acceleration = ParkAcceleration;
            //vessel.angularVelocity = ParkAngularVelocity;

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
          
            //if (vessel.situation == Vessel.Situations.LANDED) { return; } // should keep from moving slightly on every frame update
            vessel.SetWorldVelocity(zeroVector);
            vessel.acceleration = zeroVector;
            vessel.angularVelocity = zeroVector;
            vessel.geeForce = 0.0;

            if (ParkPosition != zeroVector) { vessel.SetPosition(ParkPosition); }
        }
	}
}
