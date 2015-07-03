using System.Text;

namespace MicroFlasher.Monitor {

    public class SerialMonitorMessage {

        public readonly StringBuilder Content = new StringBuilder();

        public bool Empty {
            get { return Content.Length == 0; }
        }

        public int Length { get { return Content.Length; } }
    }
}
