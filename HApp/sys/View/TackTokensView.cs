using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGS;
using System.Windows.Media;

namespace CoffeeRoaster {
    public class TackTokensView : BGListView<TokenView, RoastToken> {

        public TackTokensView():base(new hoshi_lib.MatrixSize(6,3)) {
            this.ElementsSize = new hoshi_lib.Size(66, 66);
            Background = new SolidColorBrush(Color.FromRgb(252, 252, 142));
        }

        public override void SystemUpdate() {

        }
    }
}
