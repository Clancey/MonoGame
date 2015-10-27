// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using GameController;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Xna.Framework.Input
{
    static partial class GamePad
    {
        static GamePad()
        {
            foreach (var controller in GCController.Controllers)
            {
//                controller.ControllerPausedHandler
            }
        }

        private static GamePadCapabilities PlatformGetCapabilities(int index)
        {
            var ind = (GCControllerPlayerIndex)index;
            var controller = GCController.Controllers?.FirstOrDefault(c => c.PlayerIndex == ind && c.AttachedToDevice);

            return controller == null ?  new GamePadCapabilities { IsConnected = false } : GetCapabilities(controller);
        }

        private static GamePadState PlatformGetState(int index, GamePadDeadZone deadZoneMode)
        {

            var ind = (GCControllerPlayerIndex)index;
            var controller = GCController.Controllers?.FirstOrDefault(c => c.PlayerIndex == ind);
        
            if (controller == null)
                return new GamePadState();

//              if(controller.MicroGamepad.ButtonA.IsPressed)
             

            var buttons = new List<Buttons>();
            if (controller.MicroGamepad?.ButtonA.IsPressed == true)
                buttons.Add(Buttons.A);
            var state = new GamePadState(
                new GamePadThumbSticks(),
                new GamePadTriggers(),
                new GamePadButtons(buttons.ToArray()),
                new GamePadDPad());
            state.IsConnected = true;
            return state;
        }

        private static bool PlatformSetVibration(int index, float leftMotor, float rightMotor)
        {
            return false;
        }

        static GamePadCapabilities GetCapabilities(GCController controller)
        {
            //All iOS controllers have these basics
            var capabilities = new GamePadCapabilities()
            {
                IsConnected = true,
                HasAButton = true,
                HasXButton = true,
                HasDPadDownButton = true,
                HasDPadLeftButton = true,
                HasDPadRightButton = true,
                HasDPadUpButton = true,
            };
            if (controller.ExtendedGamepad != null)
            {

            }

            if (controller.Gamepad != null)
            {

            }
            return capabilities;
        }

        static void SetUpController(GCController controller)
        {
            controller.ControllerPausedHandler = Pause;
        }

        static void Pause(GCController controller)
        {

        }
    }
}