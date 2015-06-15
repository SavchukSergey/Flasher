using System.Xml.Linq;

namespace MicroFlasher.Devices {
    public class DeviceEepromParameters {

        public int Size { get; set; }

        public int PageSize { get; set; }

        public static DeviceEepromParameters From(XElement xEeprom) {
            var xSize = xEeprom.Attribute("size");
            var xPage = xEeprom.Attribute("page");
            var size = xSize != null ? int.Parse(xSize.Value) : 0;
            return new DeviceEepromParameters {
                Size = size,
                PageSize = xPage != null ? int.Parse(xPage.Value) : size,
            };
        }
    }
}
