using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;

namespace Jking.RaspberryPi.YukiFeeder.Host.Pins
{
    public class ButtonPin
    {
        #region Construction

        private ButtonPin(GpioController controller, int pin)
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

        public static ButtonPin Initialize(GpioController controller, int pin, PinMode pinMode)
        {
            ButtonPin result = new ButtonPin(controller, pin);

            controller.OpenPin(pin, pinMode);

            return result;
        }

        public event PinChangeEventHandler OnButtonUp
        {
            add
            {
                _controller.RegisterCallbackForPinValueChangedEvent(_pin, PinEventTypes.Falling, value);
            }
            remove
            {
                _controller.UnregisterCallbackForPinValueChangedEvent(_pin, value);
            }
        }

        #endregion
    }
}
