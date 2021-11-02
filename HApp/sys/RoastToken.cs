using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGS;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
namespace CoffeeRoaster {
    abstract public class Token : BGO {
        abstract public BitmapImage Image { get; }
    }
    abstract public class RoastToken : Token {
        abstract public RoastToken GetIncreaseRoasting();
        abstract public RoastToken GetDoublieIncreaseRoasting();
    }
    public interface IBean {
        int Degree { get; }
    }
    public interface IScorePenalty {
        int ScorePenalty { get; }
    }
    public interface IFlavor {
        FlavorType Type { get; }
    }

    public class CoffeeBean : RoastToken, IBean {
        int _degree = 0;
        public int Degree {
            get { return _degree; }
        }
        BitmapImage _image;
        public override BitmapImage Image {
            get {
                if (_image == null) {
                    _image = new BitmapImage(new Uri("../../Image/" + Degree + ".png", UriKind.RelativeOrAbsolute));
                }
                return _image;
            }
        }

        public CoffeeBean(int degree = 0) {
            _degree = degree;
        }

        public override RoastToken GetIncreaseRoasting() {
            if (_degree + 1 <= 4) {
                return new CoffeeBean(_degree + 1);
            } else {
                return new CharredBean();
            }
        }
        public override RoastToken GetDoublieIncreaseRoasting() {
            if (_degree + 2 <= 4) {
                return new CoffeeBean(_degree + 2);
            } else {
                return new CharredBean();
            }
        }
    }

    public class HardBean : RoastToken, IScorePenalty, IBean {
        BitmapImage _image;
        public override BitmapImage Image {
            get {
                if (_image == null) {
                    _image = new BitmapImage(new Uri("../../Image/Hard.png", UriKind.RelativeOrAbsolute));
                }
                return _image;
            }
        }
        public int Degree {
            get { return 0; }
        }
        public int ScorePenalty {
            get { return 1; }
        }
        public override RoastToken GetIncreaseRoasting() {
            return new CoffeeBean();
        }
        public override RoastToken GetDoublieIncreaseRoasting() {
            return new CoffeeBean();
        }

       
    }

    public class CharredBean : RoastToken, IScorePenalty {
        BitmapImage _image;
        public override BitmapImage Image {
            get {
                if (_image == null) {
                    _image = new BitmapImage(new Uri("../../Image/Charred.png", UriKind.RelativeOrAbsolute));
                }
                return _image;
            }
        }
        public int ScorePenalty {
            get { return 1; }
        }
        public override RoastToken GetIncreaseRoasting() {
            return this;
        }
        public override RoastToken GetDoublieIncreaseRoasting() {
            return this;
        }
    }

    public class Moisture : RoastToken {
        BitmapImage _image;
        public override BitmapImage Image {
            get {
                if (_image == null) {
                    _image = new BitmapImage(new Uri("../../Image/Moisture.png", UriKind.RelativeOrAbsolute));
                }
                return _image;
            }
        }
        public override RoastToken GetIncreaseRoasting() {
            return null;
        }
        public override RoastToken GetDoublieIncreaseRoasting() {
            return null;
        }
    }

    public class Fumes : RoastToken, IScorePenalty {
        BitmapImage _image;
        public override BitmapImage Image {
            get {
                if (_image == null) {
                    _image = new BitmapImage(new Uri("../../Image/Fumes.png", UriKind.RelativeOrAbsolute));
                }
                return _image;
            }
        }
        public int ScorePenalty {
            get { return 1; }
        }
        public override RoastToken GetIncreaseRoasting() {
            return this;
        }
        public override RoastToken GetDoublieIncreaseRoasting() {
            return this;
        }
    }
    
    public class DefectBean : RoastToken, IScorePenalty {
        BitmapImage _image;
        public override BitmapImage Image {
            get {
                if (_image == null) {
                    _image = new BitmapImage(new Uri("../../Image/DefectBean.png", UriKind.RelativeOrAbsolute));
                }
                return _image;
            }
        }
        public int ScorePenalty {
            get { return 2; }
        }
        public override RoastToken GetIncreaseRoasting() {
            return this;
        }
        public override RoastToken GetDoublieIncreaseRoasting() {
            return this;
        }
    }

    public enum FlavorType {
        None = 0,
        Body = 1,
        Acidity = 2,
        Aroma = 4,
        Sweetness = 8,
        ThreeInOne = 7
    }

    public class Flavor : RoastToken, IFlavor {
        BitmapImage _image;
        public override BitmapImage Image {
            get {
                if (_image == null) {
                    if (Type != FlavorType.ThreeInOne) {
                        _image = new BitmapImage(new Uri("../../Image/" + Type.ToString() + ".png", UriKind.RelativeOrAbsolute));
                    } else {
                        _image = new BitmapImage(new Uri("../../Image/Three in One.png", UriKind.RelativeOrAbsolute));
                    }
                }
                return _image;
            }
        }
        public FlavorType Type { get; set; }
        
        public Flavor(FlavorType type) {
            initEffect();
            Type = type;
        }


        public override RoastToken GetIncreaseRoasting() {
            return this;
        }
        public override RoastToken GetDoublieIncreaseRoasting() {
            return this;
        }

        MultiChoiceAction<CoffeeBean> bodyeffect = BGSys.Instance.NewMultiChoiceProcess<CoffeeBean>("");
        MultiChoiceAction<RoastToken> acidityffect = BGSys.Instance.NewMultiChoiceProcess<RoastToken>("");
        PlayerAction<CoffeeBean> aromaeffect = BGSys.Instance.NewPlayerAction<CoffeeBean>("");
        public void UseEffect() {
            BGSys sys = BGSys.Instance;
            CCenter center = CCenter.Instance;
          
         
            if (this.Type == FlavorType.ThreeInOne) {
                var b = BGSys.Instance.NewActionBranch("");
                var p1 = BGSys.Instance.NewButtonAction("");
                var p2 = BGSys.Instance.NewButtonAction("");
                var p3 = BGSys.Instance.NewButtonAction("");
                
                p1.ActionItems = new string[] { "Body" };
                p2.ActionItems = new string[] { "Acidity" };
                p3.ActionItems = new string[] { "Aroma" };
                var type = CheckCanUse(center.TakeTokens.OfType<IBean>());
                p1.ChoiceableItemsFunc = (x) => {
                    if ((type & FlavorType.Body) == FlavorType.Body) return x;
                    return null;
                };
                p2.ChoiceableItemsFunc = (x) => {
                    if ((type & FlavorType.Acidity) == FlavorType.Acidity) return x;
                    return null;
                };
                p3.ChoiceableItemsFunc = (x) => {
                    if ((type & FlavorType.Aroma) == FlavorType.Aroma) return x;
                    return null;
                };
                p1.DoProgram = (x) => {
                };
                p2.DoProgram = (x) => { };
                p3.DoProgram = (x) => { };

                b.AddPalyerActions(p1, p2, p3);
                p1.NextProcess = bodyeffect;
                p2.NextProcess = acidityffect;
                p3.NextProcess = aromaeffect;
                sys.CallPriorityNextProcess(b);
            }else if ((this.Type & FlavorType.Body) == FlavorType.Body) {
                sys.CallPriorityNextProcess(bodyeffect);
            } else if ((this.Type & FlavorType.Acidity) == FlavorType.Acidity) {
                sys.CallPriorityNextProcess(acidityffect);
            } else if ((this.Type & FlavorType.Aroma) == FlavorType.Aroma) {
                sys.CallPriorityNextProcess(aromaeffect);
            }
        }
        void initEffect() {
            CCenter center = CCenter.Instance;
            bodyeffect.LoopCondition = (x) => {
                return bodyeffect.ChoicedBGOs.Count() < 2;
            };
            bodyeffect.ChoiceableFunc = () => {
                var curde = bodyeffect.ChoicedBGOs.Sum((x) => x.Degree);
                var beans = center.TakeTokens.OfType<CoffeeBean>().Where((x) => {
                    if (x.Degree > 0 && x.Degree < 4) {
                        if (curde + x.Degree <= 4) {
                            return true;
                        }
                    }
                    return false;
                });
                return beans;
            };
            bodyeffect.DoProgram = (x) => {
                x.ElementAt(0).RemovedFromBGOList();
                x.ElementAt(1).RemovedFromBGOList();
                var de = x.ElementAt(0).Degree + x.ElementAt(1).Degree;
                var bean = new CoffeeBean(de);
                center.Bag.Add(bean);
                center.Bag.Shuffle();
            };

            acidityffect.LoopCondition = (x) => {
                return acidityffect.ChoicedBGOs.Count() < 2;
            };
            acidityffect.ChoiceableFunc = () => {
                var beans = center.TakeTokens.OfType<IBean>();
                return beans.OfType<RoastToken>();
            };
            acidityffect.DoProgram = (x) => {
                x.ElementAt(0).RemovedFromBGOList();
                x.ElementAt(1).RemovedFromBGOList();
                center.Bag.AddRange(x);
                center.Bag.Shuffle();
            };

            CoffeeBean removed = null;
            aromaeffect.ChoiceableFunc = () => {
                var beans = center.TakeTokens.OfType<CoffeeBean>().Where((x) => x.Degree > 1);
                return beans;
            };
            aromaeffect.DoProgram = (x) => {
                removed = x;
                x.RemovedFromBGOList();
            };

            var sys = BGSys.Instance;
            var aromaeffect2 = sys.NewMultiChoiceProcess<ActionButton>("");
            IEnumerable<ActionButton> buttons = null;
            int count = 0;
            aromaeffect2.LoopCondition = (x) => {
                foreach (var it in sys.SelectItems) {
                    it.Unselectable();
                }
                ++count;
                return count != 2;
            };
            aromaeffect2.ChoiceableFunc = () => {
                var cude = removed.Degree;
                if (count == 0) {
                    if (cude >= 2) sys.AddActionButtons(new string[] { "1" });
                    if (cude >= 3) sys.AddActionButtons(new string[] { "2" });
                    if (cude == 4) sys.AddActionButtons(new string[] { "3" });
                    buttons = sys.SelectItems;
                } else {
                    sys.ClearActionButton();

                    if (removed.Degree - int.Parse(aromaeffect2.ChoicedBGOs.ElementAt(0).Text) == 1) sys.AddActionButton("1");
                    if (removed.Degree - int.Parse(aromaeffect2.ChoicedBGOs.ElementAt(0).Text) == 2) sys.AddActionButton("2");
                    if (removed.Degree - int.Parse(aromaeffect2.ChoicedBGOs.ElementAt(0).Text) == 3) sys.AddActionButton("3");
                }
                return sys.SelectItems;
            };
            aromaeffect2.DoProgram = (x) => {

                foreach (var it in x) {
                    it.RemovedFromBGOList();
                    CoffeeBean b = new CoffeeBean(int.Parse(it.Text));
                    center.Bag.Add(b);
                }
                sys.ClearActionButton();
                center.Bag.Shuffle();
            };

            aromaeffect.NextProcess = aromaeffect2;
        }

        int hard = 0, zero = 0, one = 0, two = 0, three = 0, four = 0;
        public FlavorType CheckCanUse(IEnumerable<IBean> beans) {
            countNumber(beans);
            FlavorType type = FlavorType.None;
            if ((this.Type & FlavorType.Body) == FlavorType.Body) {
                if (one >= 2 && two >= 2) {
                    type |= FlavorType.Body;
                }
                if (one >= 1 && two >= 1) {
                    type |= FlavorType.Body;
                }
                if (one >= 1 && three >= 1) {
                    type |= FlavorType.Body;
                }
            }
            if ((this.Type & FlavorType.Acidity) == FlavorType.Acidity) {
                if (hard + zero+ one + two + three + four >= 2) {
                    type |= FlavorType.Acidity;
                }
            }
            if ((this.Type & FlavorType.Aroma) == FlavorType.Aroma) {
                if (two + three + four >= 1) {
                    type |= FlavorType.Aroma;
                }
            }
            return type;
        }
        void countNumber(IEnumerable<IBean> beans) {
            hard = 0;
            zero = 0;
            one = 0;
            two = 0;
            three = 0;
            four = 0;
            foreach (var it in beans) {
                if (it.Degree == 0) {
                    if (it is HardBean) ++hard;
                    else ++zero;
                } else if (it.Degree == 1) {
                    ++one;
                } else if (it.Degree == 2) {
                    ++two;
                } else if (it.Degree == 3) {
                    ++three;
                } else if (it.Degree == 4) {
                    ++four;
                }
            }
        }
    }

   
}

