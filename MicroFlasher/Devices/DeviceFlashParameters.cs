using System.Xml.Linq;

namespace MicroFlasher.Devices {
    public class DeviceFlashParameters {

        public int Size { get; set; }

        public int PageSize { get; set; }

        public static DeviceFlashParameters From(XElement xFlash) {
            var xSize = xFlash.Attribute("size");
            var xPage = xFlash.Attribute("page");
            var size = xSize != null ? int.Parse(xSize.Value) : 0;
            return new DeviceFlashParameters {
                Size = size,
                PageSize = xPage != null ? int.Parse(xPage.Value) : 1
            };
        }
    }
}
