using System;
using System.Linq;
using System.Xml.Linq;

namespace MicroFlasher.Devices {
    public static class DeviceInfoUtils {

        public static string FormatDescription(XElement xDescription) {
            if (xDescription == null) return null;
            var val = xDescription.Value;
            if (string.IsNullOrWhiteSpace(val)) return null;
            return string.Join("\r\n", val.Trim()
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray()
            );
        }
    }
}
