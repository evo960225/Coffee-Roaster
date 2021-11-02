using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace CoffeeRoaster {
    public class CoffeeCard {
        static public IEnumerable<CoffeeCard> Coffeese { get; protected set; }

        public string Name { get; protected set; }
        public IEnumerable<RoastToken> BeginTokens { get; protected set; }
        public Dictionary<int, int> DegreeScoreTable { get; protected set; }
        public IEnumerable<FlavorType> GoalFlavors { get; protected set; }
        public int PlayLevel { get; protected set; }
        public int DegreeLevel { get; protected set; }

        public CoffeeCard() {
            
        }

        public int CalculateScore(IEnumerable<Token> tokens) {
            int score = 0;
            var degree = tokens.OfType<IBean>().Sum((x) => x.Degree);
            if (DegreeScoreTable.ContainsKey(degree)) {
                score += DegreeScoreTable[degree];
            }
           

            var flavors = tokens.OfType<IFlavor>().Select((x)=>x.Type).ToList();
            int add = 1;
            foreach (var it in GoalFlavors) {
                if (flavors.Contains(it)) {
                    flavors.Remove(it);
                    score += add;
                    ++add;
                }
            }
            if (flavors.Count == 0) {
                score -= 3;
            }
            var skill = tokens.OfType<CoffeeBean>();
            if (skill.Count() > 0) {
                var sk = skill.GroupBy((x) => x.Degree).Select((x) => x.Count()).Max() - 2;
                if (sk > 5) sk = 5;
                score += sk;
            }
            var t = tokens.OfType<IScorePenalty>().Sum((x) => x.ScorePenalty);
            score -= t;
            if (tokens.Count() != 10) {
                score -= 5;
            }
            return score;
        }

        public void LoadFile() {
            List<CoffeeCard> listCard = new List<CoffeeCard>();

            StreamReader fs = new StreamReader("../../coffee.json");
            var jstr  = fs.ReadToEnd();
            JArray arr = JArray.Parse(jstr);
            foreach (var it in arr) {
                var coffee =new CoffeeCard();
                var _beginTokens =new List<RoastToken>();
               
                coffee.Name = it["name"].Value<string>();
                coffee.DegreeLevel = it["deepDegree"].Value<int>();
                coffee.PlayLevel = it["level"].Value<int>();
                coffee.BeginTokens =_beginTokens;
                var tokens = ((JArray)it["content"]).Values<int>().ToArray();

                for (int i = 0; i < tokens[0]; ++i) {
                    _beginTokens.Add(new CoffeeBean());
                }
                for (int i = 0; i < tokens[1]; ++i) {
                    _beginTokens.Add(new HardBean());
                }
                for (int i = 0; i < tokens[2]; ++i) {
                    _beginTokens.Add(new Moisture());
                }
                for (int i = 0; i < tokens[3]; ++i) {
                    _beginTokens.Add(new DefectBean());
                }
                for (int i = 0; i < tokens[4]; ++i) {
                    _beginTokens.Add(new Flavor(FlavorType.Body));
                }
                for (int i = 0; i < tokens[5]; ++i) {
                    _beginTokens.Add(new Flavor(FlavorType.Acidity));
                }
                for (int i = 0; i < tokens[6]; ++i) {
                    _beginTokens.Add(new Flavor(FlavorType.Aroma));
                }

                coffee.DegreeScoreTable = new Dictionary<int,int>();
                var degreeScore = (JArray)it["goalRoast"];
                foreach (var val in degreeScore) {
                    var r = val["roast"].Value<int>();
                    var s = val["score"].Value<int>();
                    coffee.DegreeScoreTable.Add(r, s);
                }

                var goalF = new List<FlavorType>();
                coffee.GoalFlavors = goalF;
                var flavors = (JArray)it["goalFlavors"];
                foreach (var val in flavors) {
                    var v = (FlavorType)Enum.Parse(typeof(FlavorType), val.Value<string>());
                    goalF.Add(v);
                }
               

                listCard.Add(coffee);
            }

            Coffeese = listCard;
        }
    }


}
