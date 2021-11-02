using BGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CoffeeRoaster {
    abstract public class SquareToken : Token {
        abstract public string Name { get; }
        public override BitmapImage Image {
            get {
                return new BitmapImage(new Uri("../../Image/" + this.Name + ".png", UriKind.RelativeOrAbsolute));
            }
        }
        abstract protected IProcess Effect {get;}
        public SquareToken() {
        }
        public void Get(){
            BGSys.Instance.CallPriorityNextProcess(Effect);
        }
        abstract public void UseEffect();
    }

    public class ThreeToken : SquareToken,IBean {
        public override string Name {
            get { return "Three"; }
        }
        protected override IProcess Effect {
            get {
                var p = BGSys.Instance.NewProgram("");
                p.DoProgram = () => {
                    foreach (var it in CCenter.Instance.SpecialEffects) {
                        if (it.SquareToken == this) {
                            it.SquareTokenSpace.Clear();
                        }
                    }
                    CCenter.Instance.CupTestSpaces.ElementAt(0).Place(this);
                };
                return p;
            }
        }

        public override void UseEffect() {
            throw new NotImplementedException();
        }

        public int Degree {
            get { return 3; }
        }
    }
    public class SweetnessToken : SquareToken,IFlavor {
        public override string Name {
            get { return "Sweetness"; }
        }
        protected override IProcess Effect {
            get {
                var p = BGSys.Instance.NewProgram("");
                p.DoProgram = () => {
                    foreach (var it in CCenter.Instance.SpecialEffects) {
                        if (it.SquareToken == this) {
                            it.SquareTokenSpace.Clear();
                        }
                    }
                    CCenter.Instance.CupTestSpaces.ElementAt(1).Place(this);
                };
                return p;
            }
        }

        public override void UseEffect() {
            throw new NotImplementedException();
        }

        public FlavorType Type {
            get { return FlavorType.Sweetness; }
        }
    }
    public class PriorityToken : SquareToken {
        public override string Name {
            get { return "Priority"; }
        }
        protected override IProcess Effect {
            get {
                var p = BGSys.Instance.NewProgram("");
                var pe = BGSys.Instance.NewPlayerAction<Token>("");
                var pe2 = BGSys.Instance.NewPlayerAction<TokenSpace>("");
                p.DoProgram = () => {
                    foreach (var it in CCenter.Instance.SpecialEffects) {
                        if (it.SquareToken == this) {
                            it.SquareTokenSpace.Clear();
                        }
                    }
                    CCenter.Instance.CupSpecialSpaces.ElementAt(0).Place(this);

                    BGSys.Instance.RegisterReplaceProcess("cup test", ()=> {
                        var count = CCenter.Instance.CupTestSpaces.Count((x)=>x.IsFilled);
                        return count == 3 || count == 4;
                    }, pe);
                };

                var center = CCenter.Instance;
                pe.ChoiceableFunc = () => {
                    for (int i = 0; i < 2; ++i) {
                        var t = center.Bag.Draw();
                        while (t is Moisture) {
                            t = center.Bag.Draw();
                        }
                        center.TakeTokens.Add(t);
                    }
                    return center.TakeTokens;
                };
                Token putcup = null;
                pe.DoProgram = (x) => {
                    putcup = x;
                    putcup.RemovedFromBGOList();
                };
                pe2.ChoiceableFunc = () => {
                    var count = CCenter.Instance.CupTestSpaces.Count((x)=>x.IsFilled);
                    return new TokenSpace[] { center.CupTestSpaces.ElementAt(count) };
                };
                pe2.DoProgram = (x) => {
                    x.Place(putcup);
                    var list = center.TakeTokens.ToArray();
                    center.TakeTokens.Clear();
                    center.Bag.AddRange(list);
                    center.Bag.Shuffle();
                };
                pe.NextProcess = pe2;
                pe2.NextProcess = CProcees.fliterOrPutCup;
                return p;
            }
        }

        public override void UseEffect() {
            throw new NotImplementedException();
        }
    }
    public class Back2Draw3Token : SquareToken {
        public override string Name {
            get { return "Back2Draw3"; }
        }
        protected override IProcess Effect {
            get {
                var p = BGSys.Instance.NewProgram("");
                p.DoProgram = () => {
                    foreach (var it in CCenter.Instance.SpecialEffects) {
                        if (it.SquareToken == this) {
                            it.SquareTokenSpace.Clear();
                        }
                    }
                    CCenter.Instance.CupSpecialSpaces.ElementAt(1).Place(this);
                };
                return p;
            }
        }

        public override void UseEffect() {
            throw new NotImplementedException();
        }
    }
    public class SievePlateToken : SquareToken {
        public override string Name {
            get { return "Sieve Plate"; }
        }
        protected override IProcess Effect {
            get {
                var p = BGSys.Instance.NewProgram("");
                p.DoProgram = () => {
                    foreach (var it in CCenter.Instance.SpecialEffects) {
                        if (it.SquareToken == this) {
                            it.SquareTokenSpace.Clear();
                        }
                    }
                    CCenter.Instance.CupSpecialSpaces.ElementAt(2).Place(this);
                };
                return p;
            }
        }

        public override void UseEffect() {
            throw new NotImplementedException();
        }
    }
}
