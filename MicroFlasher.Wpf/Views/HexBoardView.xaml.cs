using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MicroFlasher.Hex;

namespace MicroFlasher.Views {
    /// <summary>
    /// Interaction logic for HexBoardView.xaml
    /// </summary>
    public partial class HexBoardView : Grid {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(HexBoardView), new PropertyMetadata(""));
        public static readonly DependencyProperty CanClearProperty = DependencyProperty.Register("CanClear", typeof(bool), typeof(HexBoardView), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCharactersProperty = DependencyProperty.Register("ShowCharacters", typeof(bool), typeof(HexBoardView), new PropertyMetadata(true, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) {
            var board = (HexBoardView)dependencyObject;
            board.CharactersColumn.Visibility = Convert.ToBoolean(args.NewValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public HexBoardView() {
            InitializeComponent();
        }


        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public bool CanClear {
            get { return (bool)GetValue(CanClearProperty); }
            set { SetValue(CanClearProperty, value); }
        }

        public bool ShowCharacters {
            get { return (bool)GetValue(ShowCharactersProperty); }
            set { SetValue(ShowCharactersProperty, value); }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            var context = DataContext as HexBoard;
            if (context != null) {
                context.Clear();
            }
        }

        private HexBoardByte _focusedByte;
        private bool _high;

        private void DataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e) {
            var hex = KeyUtils.GetHexDigit(e.Key);
            if (!hex.HasValue && e.Key != Key.Space && e.Key != Key.Delete && e.Key != Key.Back) return;

            var direction = e.Key != Key.Back ? 1 : -1;

            var grid = sender as DataGrid;
            if (grid == null) return;
            var board = Board;
            if (board == null) return;

            var cellInfo = grid.CurrentCell;

            if (cellInfo.Item == null) return;

            var cell = cellInfo.Column.GetCellContent(cellInfo.Item);

            if (cell == null) return;
            var line = cell.DataContext as HexBoardLine;
            if (line == null) return;

            var col = cellInfo.Column;
            var byteIndex = col.DisplayIndex - 1;
            if (byteIndex < 0 || byteIndex > 15) return;

            var bt = line.Bytes[byteIndex];
            if (bt != _focusedByte) {
                _high = true;
            }
            _focusedByte = bt;

            var nextAdr = line.Address + byteIndex + direction;
            if (hex.HasValue) {
                if (_high) {
                    _focusedByte.SetHigh((byte)hex.Value);
                } else {
                    _focusedByte.SetLow((byte)hex.Value);
                    SelectByAddress(nextAdr);
                }
                _high = !_high;
            } else {
                _focusedByte.Value = null;
                SelectByAddress(nextAdr);
            }
        }

        private void SelectByAddress(int address) {
            var board = Board;
            if (board == null) return;
            var line = board.FindLine(address);
            if (line == null) return;
            var nextCol = DataGrid.Columns[(address & 0x0f) + 1];
            DataGrid.CurrentCell = new DataGridCellInfo(line, nextCol);
            DataGrid.SelectedCells.Clear();
            DataGrid.SelectedCells.Add(DataGrid.CurrentCell);
        }

        public HexBoard Board {
            get {
                return DataContext as HexBoard;
            }
        }

    }
}
