using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGS;
using System.Windows;

namespace CoffeeRoaster {

    public class CCenter {
        static CCenter _instance = new CCenter();
        static public CCenter Instance {
            get { return _instance; }
        }
        
        public List<ImmediateEffect> ImmediateEffects { get; set; }
        public List<SquareEffect> SpecialEffects { get; set; }
        public BGOList<CupTestSpace> CupSpecialSpaces { get; set; }
        public BGOList<CupTestSpace> CupTestSpaces { get; set; }
        public BGOList<CupTestSpace> FilterSpaces { get; set; }

        public int RoastTime { get; set; }

        //player
        public CoffeeCard Coffee { get; set; }
        public int TotalRoast {
            get { return TakeTokens.Union(TakeTempTokens).Union(Bag).OfType<IBean>().Sum((x) => x.Degree); }
        }
        public BGOList<RoastToken> TakeTokens { get; set; }
        public BGOList<RoastToken> TakeTempTokens { get; set; }
        public Deck<RoastToken> Bag { get; set; }


        BGSys sys = BGSys.Instance;
        public CCenter() {
            CProcees cp =new CProcees();
            var imef1 = sys.NewProgram("取得三合一");
            imef1.DoProgram = () => {
                TakeTokens.Add(new Flavor(FlavorType.ThreeInOne));
            };

            ImmediateEffects = new List<ImmediateEffect>(5);
            ImmediateEffects.Add(new ImmediateEffect(imef1,new BeanSpace(1), new BeanSpace(0)));
            ImmediateEffects.Add(new ImmediateEffect(cp.get2Token(), new FlavorSpace(FlavorType.Body)));
            ImmediateEffects.Add(new ImmediateEffect(cp.change2Token(),new FlavorSpace(FlavorType.ThreeInOne)));
            ImmediateEffects.Add(new ImmediateEffect(cp.Get5Back3Discard2(),new FlavorSpace(FlavorType.Aroma)));
            ImmediateEffects.Add(new ImmediateEffect(cp.discardScorePenalty(), new FlavorSpace(FlavorType.ThreeInOne)));

            SpecialEffects = new List<SquareEffect>(5);
            SpecialEffects.Add(new SquareEffect(new ThreeToken(),new FlavorSpace(FlavorType.ThreeInOne), new FlavorSpace(FlavorType.Acidity)));
            SpecialEffects.Add(new SquareEffect(new SweetnessToken(),new FlavorSpace(FlavorType.Body), new FlavorSpace(FlavorType.Acidity)));
            SpecialEffects.Add(new SquareEffect(new PriorityToken(), new FlavorSpace(FlavorType.Acidity), new FlavorSpace(FlavorType.Aroma)));
            SpecialEffects.Add(new SquareEffect(new Back2Draw3Token(), new FlavorSpace(FlavorType.ThreeInOne), new FlavorSpace(FlavorType.Aroma)));
            SpecialEffects.Add(new SquareEffect(new SievePlateToken(), new FlavorSpace(FlavorType.Body), new FlavorSpace(FlavorType.Aroma)));

            CupSpecialSpaces = new BGOList<CupTestSpace>(3);
            for (int i = 0; i < 3; ++i) {
                CupSpecialSpaces.AddAndFillEmptySpace(new CupTestSpace());
            }
            CupTestSpaces = new BGOList<CupTestSpace>(10);
            for (int i = 0; i < 10; ++i) {
                CupTestSpaces.AddAndFillEmptySpace(new CupTestSpace());
            }
            FilterSpaces = new BGOList<CupTestSpace>(5);
            for (int i = 0; i < 5; ++i) {
                FilterSpaces.AddAndFillEmptySpace(new CupTestSpace());
            }
              
            TakeTokens = new BGOList<RoastToken>();
            TakeTempTokens = new BGOList<RoastToken>();
            Bag = new Deck<RoastToken>();
        }

        public void Init(CoffeeCard coffee) {
            Coffee = coffee;
            var moistures = Coffee.BeginTokens.Count((x) => x is Moisture);
            RoastTime = 10 - (moistures + 1) / 2;
            Bag.Clear();
            Bag.AddRange(Coffee.BeginTokens);
            Bag.Shuffle();
            TakeTokens.Clear();
        }
        public void End() {
            var s = Coffee.CalculateScore(CupTestSpaces.Select((x)=>x.Token));
            MessageBox.Show(s.ToString(), "Total Score");
        }
        
    }
  
}
