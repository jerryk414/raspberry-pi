using Jking.RaspberryPi.YukiFeeder.Host.Pins;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jking.RaspberryPi.YukiFeeder.Host.Dispenser
{
    class TreatDispenser
    {
        #region Construction

        public TreatDispenser(GpioController controller)
        {
            _ledPin = LedPin.Initialize(controller, 18, LedPinState.Off);
            _buttonPin = ButtonPin.Initialize(controller, 23, PinMode.InputPullDown);

            _buttonPin.OnButtonUp += async (s, e) =>
            {
                await DispenseAsync().ConfigureAwait(false);
            };
        }

        #endregion

        #region Fields

        private readonly LedPin _ledPin;
        private readonly ButtonPin _buttonPin;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private static TimeSpan _dispenseThreshold = TimeSpan.FromMilliseconds(100);

        #endregion

        #region Methods

        public async Task DispenseAsync()
        {
            if (await _semaphore.WaitAsync(_dispenseThreshold).ConfigureAwait(false))
            {
                try
                {
                    _ledPin.SetState(LedPinState.On);

                    await Task.Delay(1000).ConfigureAwait(false);

                    _ledPin.SetState(LedPinState.Off);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        #endregion
    }
}
