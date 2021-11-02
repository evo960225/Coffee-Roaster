using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGS;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Effects;

namespace CoffeeRoaster {
    public class TokenSpaceView : BGView<TokenSpace> {
       CCenter center = CCenter.Instance;
       Rectangle rect = new Rectangle();
        Ellipse circle = new Ellipse();
        public TokenSpaceView() {

            rect.Width = circle.Width = 64;
            rect.Height = circle.Height = 64;
            circle.Fill = Brushes.SandyBrown;
            circle.Opacity = 0;
            var shadow = new DropShadowEffect();
            shadow.BlurRadius = 3;
            shadow.ShadowDepth = 3;
            shadow.Opacity = 0.7;
            shadow.Color = Color.FromRgb(148, 132, 100);
            rect.Effect = circle.Effect = shadow;
            this.AddElement(circle);
            this.AddElement(rect);
            Panel.SetZIndex(circle, -10);
        }
        
        public override void SystemUpdate() {
           
            if (BGObject != null) {
             
                var token = BGObject.Token;
                if (token == null) {
                    rect.Opacity = circle.Opacity = 0;
                    circle.Fill = Brushes.SandyBrown;
                    Text = null;
                    return;
                }
                var b = new ImageBrush();
                b.Stretch = Stretch.Uniform;
                b.ImageSource = token.Image;
              
                if (BGObject is CupTestSpace) {
                    rect.Fill = b;
                    rect.Opacity = 1;
                    circle.Opacity = 0;
                } else {
                    circle.Fill = b;
                    circle.Opacity = 1;
                    rect.Opacity = 0;
                }
            } else {
                rect.Opacity = circle.Opacity = 0;
                circle.Fill = Brushes.SandyBrown;
                Text = null;
            }
        }
 

    }

    public class TokenView : BGView<RoastToken> {
        CCenter center = CCenter.Instance;
        Ellipse circle = new Ellipse();
        public TokenView() {

            circle.Width = 64;
            circle.Height = 64;
            circle.Opacity = 0;
              var shadow = new DropShadowEffect();
              shadow.ShadowDepth = 3;
              shadow.BlurRadius = 2;
            shadow.Color = Color.FromRgb(192, 182, 162);
            circle.Effect = shadow;
            this.AddElement(circle);
            Panel.SetZIndex(circle, -10);
        }

        public override void SystemUpdate() {
            if (BGObject != null) {
                var b = new ImageBrush();
                b.Stretch = Stretch.Uniform;
                b.ImageSource = BGObject.Image;
                circle.Fill = b;
                circle.Opacity = 1;
            } else {
                circle.Opacity = 0;
                Text = null;
            }
        }
    }
}
