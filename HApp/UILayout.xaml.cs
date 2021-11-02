using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using hoshi_lib.View;

namespace CoffeeRoaster {
    /// <summary>
    /// HUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class UILayout : HContainer {

        public IEnumerable<TokenSpaceView> VCups {
            get {
                return new TokenSpaceView[] {
                    VCup1, VCup2, VCup3, VCup4, VCup5,VCup6,VCup7,VCup8,VCup9,VCup10
                };
            }
        }
        public IEnumerable<TokenSpaceView> VFilters {
            get { 
                return new TokenSpaceView[] { VFilter1, VFilter2, VFilter3, VFilter4, VFilter5 };
            }
        }

        public UILayout() {
            InitializeComponent();
            BGS.BGSys.Instance.MainTimer.Tick += MainTimer_Tick;
        }

        void MainTimer_Tick(object sender, EventArgs e) {
            var center = CCenter.Instance;
            RoundTimeView.Text = "Roast Time: " + center.RoastTime;
            this.CurrentConcentrationView.Text = "Total Roast: " + center.TotalRoast;
        }
    }
}
