using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;

namespace Jking.RaspberryPi.YukiFeeder.Host.Pins
{
    public class LedPin
    {
        #region Construction

        private LedPin(GpioController controller, int pin)
        {
            _controller = controller;
            _pin = pin;
        }

        #endregion

        #region Fields

        private readonly GpioController _controller;
        private readonly int _pin;

        #endregion

        #region Methods

        public static LedPin Initialize(GpioController controller, int pin, LedPinState initialState)
        {
            LedPin result = new LedPin(controller, pin);

            controller.OpenPin(pin, PinMode.Output);
            controller.Write(pin, LedPinStateConverter.Convert(initialState));

            return result;
        }

        public void SetState(LedPinState desiredState)
        {
            _controller.Write(_pin, LedPinStateConverter.Convert(desiredState));
        }

        #endregion
    }
}
