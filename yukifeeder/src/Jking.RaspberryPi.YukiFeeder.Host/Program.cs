using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Jking.RaspberryPi.YukiFeeder.Host
{
    class Program
    {
        // Command to copy to raspberry pi
        // scp C:\Users\Jerry\Source\Repos\Jking.RaspberryPi.Console\Jking.RaspberryPi.Host\bin\Debug\netcoreapp3.1\linux-arm\* pi@192.168.1.51:/home/pi/Documents
        static async Task Main(string[] args)
        {
            using (GpioController controller = new GpioController())
            {
                controller.OpenPin(_ledPin, PinMode.Output);
                controller.Write(_ledPin, PinValue.High);

                controller.OpenPin(_buttonPin, PinMode.InputPullDown);

                controller.RegisterCallbackForPinValueChangedEvent(_buttonPin, PinEventTypes.Falling, async (s, e) =>
                {
                    await DispenseTreat(controller).ConfigureAwait(false);
                });

                await using (WebSocketConnection connection = await WebSocketConnection.BeginListen(async s =>
                {
                    await DispenseTreat(controller).ConfigureAwait(false);
                }))
                {
                    await Task.Delay(-1);
                }
            }
        }

        private static async Task DispenseTreat(GpioController controller)
        {
            if (!_treatBeingDispensed)
            {
                _treatBeingDispensed = true;

                controller.Write(_ledPin, PinValue.Low);

                await Task.Delay(1000).ConfigureAwait(false);

                controller.Write(_ledPin, PinValue.High);

                _treatBeingDispensed = false;
            }
        }

        private static bool _treatBeingDispensed;
        private static int _ledPin = 18;
        private static int _buttonPin = 23;
    }
}
