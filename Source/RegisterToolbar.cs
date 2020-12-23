using UnityEngine;
using ToolbarControl_NS;
//using KSP_Log;

namespace AirPark
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {

#if false
         internal static Log Log = null;
       void Awake()
        {
            if (Log == null)
#if DEBUG
                Log = new Log("LogNotes", Log.LEVEL.INFO);
#else
          Log = new Log("LogNotes", Log.LEVEL.ERROR);
#endif

        }
#endif

        void Start()
        {
            ToolbarControl.RegisterMod(AirParkToolbar.MODID, AirParkToolbar.MODNAME);
        }

    }
}
