using Jking.RaspberryPi.YukiFeeder.Host.Dispenser;
using Jking.RaspberryPi.YukiFeeder.Host.Pins;
using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Jking.RaspberryPi.YukiFeeder.Host
{
    // Command to copy to raspberry pi
    // scp C:\Users\Jerry\Documents\Github\raspberry-pi\yukifeeder\src\Jking.RaspberryPi.YukiFeeder.Host\bin\LinuxRelease\netcoreapp3.1\linux-arm\* pi@192.168.1.51:/home/pi/Documents
    class Program
    {
        #region Fields

        private static int _errorCount = 0;

        private const int MAX_ERROR_COUNT = 10;

        #endregion

        #region Methods

        static async Task Main(string[] args)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        static async Task InitializeAsync()
        {
            try
            {
                using (GpioController controller = new GpioController())
                {
                    TreatDispenser dispenser = new TreatDispenser(controller);

                    await using (WebSocketConnection connection = await WebSocketConnection.BeginListen(async s =>
                    {
                        await dispenser.DispenseAsync().ConfigureAwait(false);
                    }))
                    {
                        await Task.Delay(-1);
                    }
                }
            }
            catch
            {
                // TODO: Log this to a file?
                _errorCount++;

                if (_errorCount < MAX_ERROR_COUNT)
                    await InitializeAsync().ConfigureAwait(false);
            }
        }

        #endregion
    }
}
