using MicroFlasher.Devices;
using MicroFlasher.IO;

namespace MicroFlasher.Models {
    public class StubConfig : BaseProgrammerConfig {
        public StubConfig(string keyPrefix)
            : base(keyPrefix) {
        }

        public override void Save() {
        }

        public override void ReadFromConfig() {
        }

        public override string ConnectionName {
            get { return "---"; }
        }

        public override IProgrammer CreateProgrammer(DeviceInfo device) {
            return StubProgrammer.Instance;
        }

        public override IAvrChannel CreateChannel() {
            return new StubChannel();
        }
    }
}
