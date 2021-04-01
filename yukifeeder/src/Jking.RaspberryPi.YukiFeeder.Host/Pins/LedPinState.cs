using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;

namespace Jking.RaspberryPi.YukiFeeder.Host.Pins
{
    public enum LedPinState
    {
        On,
        Off
    }

    public static class LedPinStateConverter
    {
        public static PinValue Convert(LedPinState state)
        {
            switch (state)
            {
                case LedPinState.On:
                    return PinValue.High;
                case LedPinState.Off:
                    return PinValue.Low;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
