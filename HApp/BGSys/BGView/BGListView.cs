using hoshi_lib;
using hoshi_lib.View;
using System.Windows.Data;
using System.Windows.Media;

namespace BGS {
    abstract public class BGListView<BgV, BgT> : MatrixControl<BgV>, IHControl
        where BgV : BGView<BgT>, new()
        where BgT : BGO {

        #region hcontrol
        HControl _borderControl = new HControl();
        public Brush BorderBrush {
            get { return _borderControl.BorderBrush; }
            set { _borderControl.BorderBrush = value; }
        }
        public double? BorderWidth {
            get { return _borderControl.BorderWidth; }
            set { _borderControl.BorderWidth = value; }
        }
        public new System.Windows.Controls.UserControl This {
            get { return _borderControl.This; }
        }
        public FontFamily Font {
            get { return _borderControl.Font; }
            set { _borderControl.Font = value; }
        }
        public Brush FontColor {
            get { return _borderControl.FontColor; }
            set { _borderControl.FontColor = value; }
        }
        public double FontSize {
            get { return _borderControl.FontSize; }
            set { _borderControl.FontSize = value; }
        }
        public string Text {
            get { return _borderControl.Text; }
            set { _borderControl.Text = value; }
        }
        #endregion
        Brush _selectableBorderColor = Brushes.YellowGreen;
        public Brush SelectableBorderColor {
            get { return _selectableBorderColor; }
            set { _selectableBorderColor = value; }
        }
        Brush _selectedBorderColor = Brushes.SteelBlue;
        public Brush SelectedBorderColor {
            get { return _selectedBorderColor; }
            set { _selectedBorderColor = value; }
        }

        public BGOList<BgT> BGObject {
            get { return Monitoring; }
        }
        public BGOList<BgT> Monitoring { get; set; }
        public bool AutoMatchElementMonitoring { get; set; }


        int _bgDataMax = -1;
        public int BGDataMax {
            get { return _bgDataMax; }
            set { _bgDataMax = value; }
        }


        public BGListView(MatrixSize size)
            : base(size, new Size(50, 50), 5, 5) {
                AutoMatchElementMonitoring = true;
            initHControl();

            AddMouseEvent(MouseButtonEvent.LeftButtonDown, (s, e) => {
                if (BGObject == null) return;
                if (BGObject.IsSelectable && !BGObject.IsSelected) {
                    BGObject.Select();
                    e.Handled = true;
                } 
            });

            //Test:BGTimer
            BGSys.Instance.MainTimer.Tick += (s, e) => {
                if (BGObject == null) {
                    BorderBrush = null;
                    this.SystemUpdate();
                    return;
                }
                if (AutoMatchElementMonitoring) {
                    int index = 0;
                    foreach (var it in BGObject) {
                        Elements[index].Monitoring = it.Parent;
                        ++index;
                    }
                    for (; index < Elements.Count; ++index) {
                        Elements[index].Monitoring = null;
                    }
                }
                if (BGObject.IsSelectable) {
                    if (BGObject.IsSelected) {
                        BorderBrush = SelectedBorderColor;
                    } else {
                        BorderBrush = SelectableBorderColor;
                    }
                } else {
                    BorderBrush = null;
                }
                this.SystemUpdate();
            };
        }
        void initHControl() {
            this.AddElement(_borderControl);
            _borderControl.BorderWidth = 2;
            _borderControl.Background = null;
            Binding height = new Binding();
            height.Source = this;
            height.Path = new System.Windows.PropertyPath(BGView<BgT>.HeightProperty);
            _borderControl.SetBinding(HControl.HeightProperty, height);
            Binding width = new Binding();
            width.Source = this;
            width.Path = new System.Windows.PropertyPath(BGView<BgT>.WidthProperty);
            _borderControl.SetBinding(HControl.WidthProperty, width);
        }
        abstract public void SystemUpdate();

    }
}
