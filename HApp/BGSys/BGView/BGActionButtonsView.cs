using hoshi_lib.View;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace BGS {

    public class BGActionButtonView : BGView<ActionButton> {

        public BGActionButtonView() {
            Background = new SolidColorBrush(Color.FromRgb(232, 232, 232));
            FontColor = Brushes.Brown; //new SolidColorBrush(Color.FromRgb(255, 255, 255));
            FontSize = 14;
        }

        public override void SystemUpdate() {
            if (BGObject == null) return;
            this.Text = BGObject.Text;
        }
       
    }

    public class BGActionButtonsView : StackControl {

        public IEnumerable<BGView<ActionButton>> Buttons {
            get { return buttons; }
        }
        public List<BGView<ActionButton>> buttons = new List<BGView<ActionButton>>(6);
         
        public BGActionButtonsView() {
            Background = null;
            this.Orientation = System.Windows.Controls.Orientation.Horizontal;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Size = new hoshi_lib.Size(500, 50);

            BGSys.Instance.MainTimer.Tick += (s, e) => this.Update();
        }

        int preTimes = -1;
        public void Update() {
            var sys = BGSys.Instance;
            var times = sys.ListenMsgTimes;

            if (preTimes == times) return;

            this.Children.Clear();
            buttons.Clear();

            foreach (var it in BGSys.Instance.SelectItems) {
                var button = createButton(it.Parent);
                buttons.Add(button);
                this.Children.Add(button);
            }
            preTimes = times;
        }
        BGView<ActionButton> createButton(BGOContainer item) {
            BGActionButtonView button = new BGActionButtonView();
            
            button.Monitoring = item;
            button.Margin = new System.Windows.Thickness(5, 0, 5, 0);
            var width = 40 + button.BGObject.Text.Count() * button.FontSize / 1.5;
            button.Size = new hoshi_lib.Size(width, 40);

            return button;
        }
       
    }

}
