using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MicroFlasher.STKv1;

namespace MicroFlasher.Devices {
    public class DeviceInfo {

        private DeviceFlashParameters _flash = new DeviceFlashParameters();
        private DeviceEepromParameters _eeprom = new DeviceEepromParameters();
        private DeviceBits _lockBits = new DeviceBits();
        private DeviceBits _fuseBits = new DeviceBits();

        public string Name { get; set; }

        public DeviceFlashParameters Flash {
            get { return _flash; }
        }

        public DeviceEepromParameters Eeprom {
            get { return _eeprom; }
        }

        public DeviceBits LockBits {
            get { return _lockBits; }
        }

        public DeviceBits FuseBits {
            get { return _fuseBits; }
        }

        public int RamSize { get; set; }

        public AvrSignature Signature { get; set; }

        public StkDeviceCode StkCode { get; set; }

        public static DeviceInfo From(XElement node) {
            var xName = node.Attribute("name");

            var xFlash = node.Element("flash");
            var xEeprom = node.Element("eeprom");
            var xRam = node.Attribute("ram");
            var xSignature = node.Attribute("signature");
            var xLockBits = node.Element("lockBits");
            var xFuseBits = node.Element("fuseBits");
            var xStkCode = node.Attribute("stkCode");
            return new DeviceInfo {
                Name = xName != null ? xName.Value : "unknown",
                _flash = xFlash != null ? DeviceFlashParameters.From(xFlash) : new DeviceFlashParameters(),
                _eeprom = xEeprom != null ? DeviceEepromParameters.From(xEeprom) : new DeviceEepromParameters(),
                RamSize = xRam != null ? int.Parse(xRam.Value) : 0,
                Signature = xSignature != null ? AvrSignature.Parse(xSignature.Value) : new AvrSignature(),
                _lockBits = xLockBits != null ? DeviceBits.Parse(xLockBits) : new DeviceBits(),
                _fuseBits = xFuseBits != null ? DeviceBits.Parse(xFuseBits) : new DeviceBits(),
                StkCode = xStkCode != null ? ParseStkCode(xStkCode.Value) : StkDeviceCode.None,
            };
        }

        private static StkDeviceCode ParseStkCode(string val) {
            StkDeviceCode res;
            if (Enum.TryParse(val, true, out res)) return res;
            return (StkDeviceCode) ParseInt(val);
        }

        private static int ParseInt(string val) {
            if (val.StartsWith("0x")) return int.Parse(val.Substring(2), NumberStyles.HexNumber);
            return int.Parse(val);
        }

        public static IList<DeviceInfo> List() {
            var xDoc = XDocument.Load("devices.xml");
            return xDoc.Root.Elements().Select(From).OrderBy(item => item.Name).ToList();
        }

        public override string ToString() {
            return Name;
        }
    }

}
