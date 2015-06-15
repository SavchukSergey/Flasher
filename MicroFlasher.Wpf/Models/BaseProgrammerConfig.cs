using MicroFlasher.Devices;
using MicroFlasher.IO;

namespace MicroFlasher.Models {
    public abstract class BaseProgrammerConfig : BaseConfig {
        
        protected BaseProgrammerConfig(string keyPrefix)
            : base(keyPrefix) {
        }

        public abstract IProgrammer CreateProgrammer(DeviceInfo device);

        public abstract IAvrChannel CreateChannel();

    }
}
