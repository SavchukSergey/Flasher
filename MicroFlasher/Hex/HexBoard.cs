using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Atmega.Hex;
using MicroFlasher.Annotations;

namespace MicroFlasher.Hex {
    public class HexBoard : INotifyPropertyChanged {

        private readonly ObservableCollection<HexBoardLine> _lines = new ObservableCollection<HexBoardLine>();

        public HexBoard() {
            _lines.CollectionChanged += LinesOnCollectionChanged;
        }

        private void LinesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            if (args.OldItems != null) {
                foreach (HexBoardLine oldItem in args.OldItems) {
                    oldItem.DataChanged -= OnLineDataChanged;
                }
            }
            if (args.NewItems != null) {
                foreach (HexBoardLine newItem in args.NewItems) {
                    newItem.DataChanged += OnLineDataChanged;
                }
            }
        }

        private void OnLineDataChanged(object sender, EventArgs e) {
            OnDataChanged();
        }

        public HexBoardLine FindLine(int adr) {
            adr = (adr >> 4) << 4;
            return _lines.FirstOrDefault(item => item.Address == adr);
        }

        private HexBoardLine EnsureLine(int adr) {
            adr = (adr >> 4) << 4;
            var line = FindLine(adr);
            if (line == null) {
                var i = 0;
                while (i < _lines.Count && _lines[i].Address < adr)
                    i++;

                line = new HexBoardLine {
                    Address = adr
                };
                _lines.Insert(i, line);
            }
            return line;
        }

        public byte? this[int address] {
            get {
                var line = FindLine(address);
                if (line == null) return null;
                return line[address];
            }
            set {
                var line = EnsureLine(address);
                line[address] = value;
            }
        }

        public bool HasData {
            get { return Size > 0; }
        }

        public ObservableCollection<HexBoardLine> Lines { get { return _lines; } }

        public static HexBoard From(HexFile file) {
            var res = new HexBoard();
            foreach (var line in file.Lines) {
                for (int index = 0; index < line.Data.Length; index++) {
                    var bt = line.Data[index];
                    res[line.Address + index] = bt;
                }
            }
            return res;
        }

        public static HexBoard From(byte[] data, int targetStart = 0) {
            var board = new HexBoard();

            var offset = 0;

            foreach (var bt in data) {
                board[offset + targetStart] = bt;
                offset++;
            }

            return board;
        }

        public int MaxAddress {
            get {
                var res = -1;
                foreach (var line in _lines) {
                    if (line.HasData) {
                        for (var btIndex = 0; btIndex < line.Bytes.Length; btIndex++) {
                            var bt = line.Bytes[btIndex];
                            if (bt.Value.HasValue) {
                                res = Math.Max(res, line.Address + btIndex);
                            }
                        }
                    }
                }
                return res;
            }
        }

        public int Size {
            get {
                if (_lines.Count == 0) return 0;
                return _lines.Sum(line => line.Bytes.Count(b => b.Value.HasValue));
            }
        }

        public byte[] ToArray() {
            var res = new byte[MaxAddress + 1];
            foreach (var line in _lines) {
                if (line.HasData) {
                    for (var btIndex = 0; btIndex < line.Bytes.Length; btIndex++) {
                        var bt = line.Bytes[btIndex];
                        if (bt.Value.HasValue) {
                            res[line.Address + btIndex] = bt.Value.Value;
                        }
                    }
                }
            }
            return res;
        }

        public HexBlocks SplitBlocks(int pageSize) {
            pageSize = Math.Max(1, pageSize);
            var sp = new Queue<BlockInfo>(JoinBlocks(GetPages(pageSize), pageSize));
            var res = new HexBlocks();
            HexBlock block = null;
            foreach (var line in _lines) {
                for (var index = 0; index < line.Bytes.Length; index++) {
                    var bt = line.Bytes[index];
                    if (bt.Value.HasValue) {
                        var address = line.Address + index;
                        if (block != null && (address < block.Address || address > block.Address + block.Data.Length)) {
                            block = null;
                        }
                        if (block == null) {
                            var info = sp.Dequeue();
                            block = CreateBlock(info.Address, info.Length);
                            res.Blocks.Add(block);
                        }
                        block.Data[address - block.Address] = bt.Value.Value;
                    }
                }
            }

            return res;
        }

        private class BlockInfo {

            public int Address;

            public int Length;
        }

        private IEnumerable<int> GetPages(int pageSize = 1) {
            var prevKey = new int?();
            foreach (var line in _lines) {
                for (var i = 0; i < line.Bytes.Length; i++) {
                    var bt = line.Bytes[i];
                    if (bt.Value.HasValue) {
                        var address = line.Address + i;
                        var key = (address / pageSize) * pageSize;
                        if (prevKey == null || prevKey.Value != key) {
                            yield return key;
                            prevKey = key;
                        }
                    }
                }
            }
        }

        private static IEnumerable<BlockInfo> JoinBlocks(IEnumerable<int> pages, int pageSize) {
            BlockInfo prev = null;
            foreach (var page in pages) {
                BlockInfo block = null;
                if (prev != null) {
                    var prevEnd = prev.Address + prev.Length;
                    if (prevEnd == page) {
                        prev.Length += pageSize;
                        block = prev;
                    }
                }
                block = block ?? new BlockInfo {
                    Address = page,
                    Length = pageSize
                };
                if (block != prev) {
                    yield return block;
                    prev = block;
                }
            }
        }

        private static HexBlock CreateBlock(int start, int size) {
            var block = new HexBlock { Address = start, Data = new byte[size] };
            for (var i = 0; i < size; i++) {
                block.Data[i] = 0xff;
            }
            return block;
        }

        public void Clear() {
            Lines.Clear();
            Lines.Add(new HexBoardLine { Address = 0 });
            OnPropertyChanged("Size");
        }

        public HexBoard Clone() {
            var res = new HexBoard();
            foreach (var line in Lines) {
                res.Lines.Add(line.Clone());
            }
            return res;
        }

        #region Events

        public event EventHandler DataChanged;

        protected virtual void OnDataChanged() {
            var handler = DataChanged;
            if (handler != null) handler(this, new EventArgs());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void LoadFrom(HexBoard hexBoard) {
            Lines.Clear();
            foreach (var line in hexBoard.Lines) {
                Lines.Add(line.Clone());
            }
            OnPropertyChanged("Size");
        }
    }
}
