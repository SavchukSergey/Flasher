using System.Collections.Generic;
using System.Linq;

namespace MicroFlasher.Hex {
    public class HexBlocks {

        private readonly IList<HexBlock> _blocks = new List<HexBlock>();

        public IList<HexBlock> Blocks {
            get { return _blocks; }
        }

        public int TotalBytes {
            get {
                if (_blocks.Count == 0) return 0;
                return _blocks.Sum(b => b.Data.Length);
            }
        }
    }
}
