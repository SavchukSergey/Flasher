using System.Reflection;

namespace MicroFlasher.Models {
    public class AboutModel {

        public string Version {
            get { return Assembly.GetName().Version.ToString(); }
        }

        public string Copyright {
            get {
                var attr = Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
                return attr != null ? attr.Copyright : null;
            }
        }

        public string Product {
            get {
                var attr = Assembly.GetCustomAttribute<AssemblyProductAttribute>();
                return attr != null ? attr.Product : null;
            }
        }

        private static Assembly Assembly {
            get { return Assembly.GetExecutingAssembly(); }
        }
    }
}
