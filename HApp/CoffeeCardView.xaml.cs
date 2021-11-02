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
    public partial class CoffeeCardView : HContainer {
        CoffeeCard _coffee;
        public CoffeeCard Coffee {
            get { return _coffee; }
            set {
                _coffee = value;
                CoffeeNameView.Text = _coffee.Name;
                int[] content= new int[7];
                content[0] = _coffee.BeginTokens.Count((x) => (x is CoffeeBean) && (x as CoffeeBean).Degree == 0);
                content[1] = _coffee.BeginTokens.Count((x) => (x is HardBean));
                content[2] = _coffee.BeginTokens.Count((x) => (x is Moisture));
                content[3] = _coffee.BeginTokens.Count((x) => (x is DefectBean));
                content[4] = _coffee.BeginTokens.Count((x) => (x is Flavor) && (x as Flavor).Type == FlavorType.Body);
                content[5] = _coffee.BeginTokens.Count((x) => (x is Flavor) && (x as Flavor).Type == FlavorType.Acidity);
                content[6] = _coffee.BeginTokens.Count((x) => (x is Flavor) && (x as Flavor).Type == FlavorType.Aroma);
                Contents = content;
                Flavors = value.GoalFlavors;
                DegreeScoreTable = value.DegreeScoreTable;
            }
        }


        int[] _contents;
        public int[] Contents {
            get { return _contents; }
            private set {
                _contents = value;
                VContent1.Text = "x" + value[0].ToString();
                VContent2.Text = "x" + value[1].ToString();
                VContent3.Text = "x" + value[2].ToString();
                VContent4.Text = "x" + value[3].ToString();
                VContent5.Text = "x" + value[4].ToString();
                VContent6.Text = "x" + value[5].ToString();
                VContent7.Text = "x" + value[6].ToString();
            }
        }

        IEnumerable<FlavorType> _flavors;
        public IEnumerable<FlavorType> Flavors {
            get { return _flavors; }
            private set { 
                _flavors = value;
                var count = _flavors.Count();
                FlavorsView.MatrixSize = new hoshi_lib.MatrixSize(count,1 );
                int index = 0;
                foreach (var it in FlavorsView.Elements) {
                    var f = new Flavor(value.ElementAt(index));
                    var b = new ImageBrush();
                    b.Stretch = Stretch.Uniform;
                    b.ImageSource = f.Image;
                    it.Background = b;
                    ++index;
                }
            }
        }

        Dictionary<int, int> _degreeScoreTable;
        public Dictionary<int, int> DegreeScoreTable {
            get { return _degreeScoreTable; }
            private set {
                _degreeScoreTable = value;
                int maxIndex = 0;
                for (int i = 0; i < 8; ++i) {
                    var it = value.ElementAt(i);
                    ScoreView.Elements[i].Text = it.Key.ToString();
                    ScoreView.Elements[i + 8].Text = "★" + it.Value.ToString();
                    ScoreView[i + 8].FontColor = Brushes.LightYellow;
                    if (it.Value > value.ElementAt(maxIndex).Value) {
                        maxIndex = i;
                    }
                }
                ScoreView.Elements[maxIndex + 8].FontColor = Brushes.GreenYellow;
            }
        }

        public CoffeeCardView() {
            InitializeComponent();

            ScoreView.Do((x) => x.Background = new SolidColorBrush(Color.FromRgb(163, 107, 30)));
            ScoreView.Do((x) => x.FontSize = 14);
            ScoreView.Do((x) => x.FontSize = 16, (x, y) => y == 0);
      
            //ScoreView.Do((x) => x.FontColor = Brushes.Yellow, (x, y) => y == 1);
            
            ScoreView.Do((x) => x.Font = new FontFamily("Adobe Gothic Std B"));
        }
    }
}
