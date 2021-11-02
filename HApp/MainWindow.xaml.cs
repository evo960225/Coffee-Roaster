using BGS;
using CoffeeRoaster;
using hoshi_lib;
using hoshi_lib.View;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace HApp {
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {

        MainWindowController win;
        hoshi_lib.View.Screen screen;
        UILayout layout;
        BGSys sys = BGSys.Instance;
        CCenter center = CCenter.Instance;

        public MainWindow() {
            InitializeComponent();

            win = new MainWindowController(this);
            screen = new hoshi_lib.View.Screen(new hoshi_lib.Size(1140, 640));
            win.Screen = screen;
            layout = new UILayout();
            screen.AddElement(layout);

            CoffeeCard c = new CoffeeCard();
            c.LoadFile();


            layout.VLA1.Monitoring = center.ImmediateEffects[0][0].Parent;
            layout.VLA2.Monitoring = center.ImmediateEffects[0][1].Parent;
            layout.VLB.Monitoring = center.ImmediateEffects[1][0].Parent;
            layout.VLC.Monitoring = center.ImmediateEffects[2][0].Parent;
            layout.VLD.Monitoring = center.ImmediateEffects[3][0].Parent;
            layout.VLE.Monitoring = center.ImmediateEffects[4][0].Parent;

            layout.VRA1.Monitoring = center.SpecialEffects[0][0].Parent;
            layout.VRA2.Monitoring = center.SpecialEffects[0][1].Parent;
            layout.VRB1.Monitoring = center.SpecialEffects[1][0].Parent;
            layout.VRB2.Monitoring = center.SpecialEffects[1][1].Parent;
            layout.VRC1.Monitoring = center.SpecialEffects[2][0].Parent;
            layout.VRC2.Monitoring = center.SpecialEffects[2][1].Parent;
            layout.VRD1.Monitoring = center.SpecialEffects[3][0].Parent;
            layout.VRD2.Monitoring = center.SpecialEffects[3][1].Parent;
            layout.VRE1.Monitoring = center.SpecialEffects[4][0].Parent;
            layout.VRE2.Monitoring = center.SpecialEffects[4][1].Parent;

            layout.VR_S1.Monitoring = center.SpecialEffects[0].SquareTokenSpace.Parent;
            layout.VR_S2.Monitoring = center.SpecialEffects[1].SquareTokenSpace.Parent;
            layout.VR_S3.Monitoring = center.SpecialEffects[2].SquareTokenSpace.Parent;
            layout.VR_S4.Monitoring = center.SpecialEffects[3].SquareTokenSpace.Parent;
            layout.VR_S5.Monitoring = center.SpecialEffects[4].SquareTokenSpace.Parent;

            layout.VCup_S3.Monitoring = center.CupSpecialSpaces[0].Parent;
            layout.VCup_S4.Monitoring = center.CupSpecialSpaces[1].Parent;
            layout.VCup_S5.Monitoring = center.CupSpecialSpaces[2].Parent;

            for (int i = 0; i < 10; ++i) {
                layout.VCups.ElementAt(i).Monitoring = center.CupTestSpaces[i].Parent;
            }
            for (int i = 0; i < 5; ++i) {
                layout.VFilters.ElementAt(i).Monitoring = center.FilterSpaces[i].Parent;
            }
            layout.VTacks.Monitoring = center.TakeTokens;
            layout.VTacksTemp.Monitoring = center.TakeTempTokens;

            center.Init(CoffeeCard.Coffeese.ElementAt(Rand.Next_(CoffeeCard.Coffeese.Count() - 1)));
            layout.CoffeeView.Coffee = center.Coffee;
            CProcees p = new CProcees();

            sys.Start(p.getStartP());




        }

     
    }
}
