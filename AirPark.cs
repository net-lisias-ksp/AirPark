using System;
using UnityEngine;

namespace AirPark
{
    public class AirPark : PartModule
    {
		[KSPField(isPersistant = true, guiActive = true, guiName = "AirParked")]
		Boolean Parked;
		[KSPField(isPersistant = true, guiActive = true, guiName = "AutoPark")]
		Boolean autoPark;
		[KSPField(isPersistant = true, guiActive = false)]
		Vector3 ParkPosition = new Vector3(0f, 0f, 0f);
		[KSPField(isPersistant = true, guiActive = false)]
		Vector3 ParkVelocity = new Vector3(0f, 0f, 0f);
        [KSPField(isPersistant = true, guiActive = false)]
        Vessel.Situations previousState;

		private static Vector3 zeroVector = new Vector3(0f, 0f, 0f);
        public bool wasSplashed;

        [KSPEvent(guiActive = true, guiName = "Toggle Park")]
        public void TogglePark()
        {
            // cannot Park in orbit or sub-orbit
            if (vessel.situation != Vessel.Situations.SUB_ORBITAL && vessel.situation != Vessel.Situations.ORBITING)
            {
                if (!Parked)
                {
                    ParkVessel();
                }
                else
                {
                   RestoreVesselState();
                }
                Parked = !Parked;
                
            }
        }

        [KSPEvent(guiActive = true, guiName = "Toggle AutoPark")] //auto park on will awake the vessel and set Parked = false if closer than 1.5 KM and inactive
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
                    ParkPosition = vessel.GetWorldPos3D();
                    part.force_activate();
                    RememberPreviousState();
                }
			}
		}

        public override void OnFixedUpdate()
        {
            #region can't Park if we're orbiting
            if (vessel.situation == Vessel.Situations.SUB_ORBITAL || vessel.situation == Vessel.Situations.ORBITING)
            {
                autoPark = false;
                Parked = false;
            }
            #endregion

            #region If we are the Inactive Vessel, See if we want to Park
            if (!vessel.isActiveVessel && autoPark)
            {
                ParkPosition = vessel.GetWorldPos3D();
                // if we're less than 1.5km from the active vessel and Parked, then wake up
                if ((vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude < 1500.0f && Parked)
                {
                    Parked = false;
                    vessel.GoOffRails();
                    vessel.situation = previousState;
                    //vessel.Landed = false;
                    //RememberPreviousState();
                    setVesselStill();
                }
                // if we're farther than 2km, auto Park if needed
                if ((vessel.GetWorldPos3D() - FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude > 2000.0f && (!Parked))
                {
                    Parked = true;
                    ParkVessel();
                }
            }
            #endregion
            
            #region if we're not Parked, and not active and flying, then go off rails
            if (!Parked & !vessel.isActiveVessel & vessel.situation == Vessel.Situations.FLYING)
            {
                vessel.GoOffRails();
            }
            #endregion
            //This is where parking seems to take place
            
            if (Parked)
            {
                ParkVessel();
            }

        }

		private void RememberPreviousState()
		{
            if (!Parked & vessel.situation != Vessel.Situations.LANDED)
			{
                previousState = vessel.situation;                           
			}	        
		}

        private void RestoreVesselState()
        {
            vessel.situation = previousState;
            //setVesselStill();
        }
		private void ParkVessel()
		{
            RememberPreviousState();
			ParkPosition = vessel.GetWorldPos3D();
			ParkVelocity = vessel.GetSrfVelocity();
            setVesselStill();
            vessel.SetPosition(ParkPosition);
            vessel.situation = Vessel.Situations.LANDED;
            vessel.Landed = true;
    			
		}
        private void setVesselStill()
        {
            vessel.SetWorldVelocity(zeroVector);
            vessel.acceleration = zeroVector;
            vessel.angularVelocity = zeroVector;
            vessel.geeForce = 0.0;	
        }
	}
}
