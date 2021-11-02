using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGS;

namespace CoffeeRoaster {
    public class CProcees {
        CCenter center = CCenter.Instance;
        BGSys sys = BGSys.Instance;
        static public PlayerAction<TokenSpace> fliterOrPutCup = BGSys.Instance.NewPlayerAction<TokenSpace>("cup test");
        public IProcess getStartP() {

            #region getToken
            Program getToken = sys.NewProgram("");
            getToken.DoProgram = () => {
                center.Bag.Shuffle();
                var tokens = center.Bag.Draw(center.RoastTime);
                center.TakeTokens.AddAndFillEmptySpace(tokens);  
            };
            
            #endregion

            #region removeMoisture
            var removeMoisture = sys.NewProgram("");
            removeMoisture.DoProgram = () => {
                foreach (var it in center.TakeTokens.ToList()) {
                    if (it is Moisture) {
                        it.RemovedFromBGOList();
                    }
                }
            };
            #endregion

            #region continue or flavor
            var continueOrFlavor = sys.NewActionBranch("");

            #endregion


            TokenSpace tackedSpace = null;
            #region useSpaceEffect - tackToken
            var selectEffect = sys.NewPlayerAction<TokenSpace>("");
            selectEffect.SingleChoiceSkip = true;
            selectEffect.ChoiceableFunc = () => {
                List<TokenSpace> spaces = new List<TokenSpace>();
                foreach (var effect in center.ImmediateEffects) {
                    foreach (var space in effect.Spaces) {
                        foreach (var token in center.TakeTokens) {
                            if (space.IsAbleToPlace(token)) {
                                spaces.Add(space);
                            }
                        }
                    }
                }
                return spaces;
            };
            selectEffect.DoProgram = (x) => {
                tackedSpace = x;
            };
            #endregion
            #region useSpaceEffect
            var useSpaceEffect = sys.NewPlayerAction<Token>("");
            useSpaceEffect.ChoiceableFunc = () => {
                List<Token> tokens = new List<Token>();
                foreach (var effect in center.ImmediateEffects) {
                    foreach (var space in effect.Spaces) {
                        foreach (var token in center.TakeTokens) {
                            if (tackedSpace.IsAbleToPlace(token)) {
                                tokens.Add(token);
                            }
                        }
                    }
                }
                return tokens;
            };
            useSpaceEffect.DoProgram = (x) => {
                if (tackedSpace.IsAbleToPlace(x)) {
                    x.RemovedFromBGOList();
                    tackedSpace.Place(x);
                    return;
                }
            };

            #endregion

            #region to flavor process
            var toFlavorProcess = sys.NewButtonAction("");
            toFlavorProcess.ActionItems = new string[] { "next" };
            toFlavorProcess.ChoiceableItemsFunc = (x) => x;
            toFlavorProcess.DoProgram = (x) => {

            };
            #endregion

            #region continue or nextRound
            var continueOrNextRound = sys.NewActionBranch("");
            #endregion

            FlavorSpace flavorEffectSpace = null;
            Flavor useEffect = null;
            #region tackFlavorEffect 
            var tackFlavorEffect = sys.NewPlayerAction<Flavor>("");
            tackFlavorEffect.ChoiceableFunc = () => {
                var tmp = center.TakeTokens.OfType<Flavor>().Where((x) => x.CheckCanUse(center.TakeTokens.OfType<IBean>()) != FlavorType.None);
                return tmp;
            };
            tackFlavorEffect.DoProgram = (x) => {
                x.RemovedFromBGOList();
                useEffect = x;
            };
            #endregion
            FlavorType type;


            #region putFlavorEffect
            var putFlavorEffect = sys.NewPlayerAction<FlavorSpace>("");
            putFlavorEffect.ChoiceableFunc = () => {
                List<FlavorSpace> fs =new List<FlavorSpace>();
                foreach(var it in center.SpecialEffects){
                    foreach(var s in it.Spaces.OfType<FlavorSpace>()){
                        if(s.IsAbleToPlace(useEffect)){
                            fs.Add(s);
                        }
                    }
                }
                return fs;
            };
            putFlavorEffect.DoProgram = (x) => {
                type = x.FlavorType;
                useEffect.Type &= type;
                x.Place(useEffect);
                flavorEffectSpace = x;
            };
            #endregion

            #region useEffectBranch
            var useEffectBranch = sys.NewActionBranch("");
            useEffectBranch.SingleChoiceSkip = true;
            #endregion

            #region dontEffectBotton
            var dontEffectBotton = sys.NewButtonAction("");
            dontEffectBotton.ActionItems = new string[] { "不使用效果，而外支付1Flavor" };
            dontEffectBotton.ChoiceableItemsFunc = (x) => {
                var tokens = center.TakeTokens.OfType<Flavor>();
                if (tokens.Count() > 0) return x;
                return null;
            };
            dontEffectBotton.DoProgram = (x) => {};
            
            #endregion

            #region doEffectBotton
            var doEffectBotton = sys.NewButtonAction("");
            doEffectBotton.ActionItems = new string[] { "使用效果" };
            doEffectBotton.ChoiceableItemsFunc = (x) => {
                return x;
            };
            doEffectBotton.DoProgram = (x) => {};
            #endregion

            #region dontUseEffect
            var dontUseEffect = sys.NewPlayerAction<Flavor>("");
            dontUseEffect.ChoiceableFunc = () => {
                var tokens= center.TakeTokens.OfType<Flavor>();
                if (tokens.Count() > 0) return tokens;
                return null;
            };
            dontUseEffect.DoProgram = (x) => {
                x.RemovedFromBGOList();
            };
            #endregion

            #region useFlavorEffect
            var useFlavorEffect = sys.NewProgram("");
            useFlavorEffect.DoProgram = () => {
                useEffect.UseEffect();
            };
            #endregion

            #region end round
            var endRound = sys.NewButtonAction("");
            endRound.ActionItems = new string[] { "next" };
            endRound.ChoiceableItemsFunc = (x) => x;
            endRound.DoProgram = (x) => {
                var tokens = center.TakeTokens;
                IEnumerable<RoastToken> newTokens = new List<RoastToken>();
                if (center.RoastTime == 10 || center.RoastTime == 14)
                    newTokens = tokens.Select((it) => it.GetDoublieIncreaseRoasting());
                else
                    newTokens = tokens.Select((it) => it.GetIncreaseRoasting());
                center.Bag.AddRange(newTokens);
                center.TakeTokens.Clear();
                center.Bag.Shuffle();
            };
            #endregion
            
            #region toRoastOrCupTest
            var toRoastOrCupTest = sys.NewActionBranch("");
            #endregion

            #region toRoast
            var toRoast = sys.NewButtonAction("");
            toRoast.ActionItems = new List<string> { "Roast" };
            toRoast.ChoiceableItemsFunc = (x) => {
                if (center.RoastTime==14) return new string[0];
                return x;
            };
            toRoast.DoProgram = (x) => { };
            #endregion

            #region toCupTest
            var toCupTest = sys.NewButtonAction("");
            toCupTest.ChoiceableItemsFunc = (x) => {
                return x;
            };
            toCupTest.ActionItems = new List<string> { "Cup Test" };
            toCupTest.DoProgram = (x) => { };
            #endregion

            #region nextRound
            var nextRound = sys.NewProgram("");
            nextRound.DoProgram = () => {
                var tokens = center.TakeTokens;
                IEnumerable<RoastToken> newTokens = new List<RoastToken>();
                
                if (center.Bag.Count != 0 || center.RoastTime != 14) {
                    ++center.RoastTime;
                    if (center.RoastTime == 10) {
                        center.Bag.Add(new Fumes());
                        center.Bag.Add(new Fumes());
                    } else if (center.RoastTime == 12) {
                        center.Bag.Add(new Fumes());
                        center.Bag.Add(new Fumes());
                        center.Bag.Add(new Fumes());
                    }
                }
                center.Bag.Shuffle();
            };
           
            #endregion

            #region fliter or put cup loop
            var fliterOrPutCupLoop = sys.NewFlowBranch("");
            #endregion

            Token putcup =null;
            #region fliter or put cup
            
            CProcees.fliterOrPutCup.ChoiceableFunc = () => {
                var t = center.Bag.Draw();
                while (t is Moisture) {
                    t = center.Bag.Draw();
                }
                center.TakeTokens.Add(t);
                putcup = t;
                List<TokenSpace> list = new List<TokenSpace>();
                var count = center.CupTestSpaces.Count((x) => x.IsFilled);
                if (count < 10) {
                    foreach (var it in center.CupTestSpaces) {
                        if (!it.IsFilled) {
                            list.Add(it);
                            break;
                        }
                    }
                } 
                

                count = center.FilterSpaces.Count((x) => x.IsFilled);
                if (center.CupSpecialSpaces.ElementAt(2).IsFilled) {
                    if (count < 5) list.Add(center.FilterSpaces[count]);
                } else if (count < 3) list.Add(center.FilterSpaces[count]);
               
                return list;
            };
            fliterOrPutCup.DoProgram = (x) => {
             
                putcup.RemovedFromBGOList();
                x.Place(putcup);
                var count = center.CupTestSpaces.Count((y) => y.IsFilled);
                if (count == 10 && center.CupSpecialSpaces.ElementAt(1).IsFilled) {
                    sys.CallPriorityNextProcess(back2Tack3());
                }
            };
            #endregion

            #region game over
            var gameover = sys.NewProgram("");
            gameover.DoProgram = () => {
                center.End();
            };
            #endregion

            getToken.NextProcess = removeMoisture;
            removeMoisture.NextProcess = continueOrFlavor;
            //
            continueOrFlavor.AddPalyerActions(selectEffect, toFlavorProcess);
            selectEffect.NextProcess = useSpaceEffect;
            useSpaceEffect.NextProcess = removeMoisture;
            toFlavorProcess.NextProcess = continueOrNextRound;
            //
            continueOrNextRound.AddPalyerActions(tackFlavorEffect, endRound);
            tackFlavorEffect.NextProcess = putFlavorEffect;
            putFlavorEffect.NextProcess = useEffectBranch;
            useEffectBranch.AddPalyerAction(doEffectBotton); doEffectBotton.NextProcess = useFlavorEffect; useFlavorEffect.NextProcess = continueOrNextRound;
            useEffectBranch.AddPalyerAction(dontEffectBotton); dontEffectBotton.NextProcess = dontUseEffect; dontUseEffect.NextProcess = continueOrNextRound;

            endRound.NextProcess = toRoastOrCupTest;
            //
            toRoastOrCupTest.AddPalyerActions(toRoast, toCupTest);
            toRoast.NextProcess = nextRound; nextRound.NextProcess = getToken;
            toCupTest.NextProcess = fliterOrPutCupLoop;
            //
            fliterOrPutCupLoop.NextProcessFunc = () => {
                var count = center.CupTestSpaces.Count((x) => x.IsFilled);
                if (count < 10 && center.Bag.Count != 0) {
                    return fliterOrPutCup;
                } else {
                    return gameover;
                }
            };
            fliterOrPutCup.NextProcess = fliterOrPutCupLoop;
            
            return getToken; 
        }

        public IProcess get2Token() {
            var p = sys.NewProgram("");
            p.DoProgram = () => {
                var tokens = CCenter.Instance.Bag.Draw(2);
                CCenter.Instance.TakeTokens.AddRange(tokens);
            };
            return p;
        }
        public IProcess change2Token() {
            var p = sys.NewMultiChoiceProcess<RoastToken>("");
            p.LoopCondition = (x) => {
                return p.ChoicedBGOs.Count() < 2;
            };
            p.ChoiceableFunc = () => {
                return CCenter.Instance.TakeTokens;
            };
            p.DoProgram = (x) => {
                foreach(var it in x){
                    it.RemovedFromBGOList();
                }
                CCenter.Instance.Bag.AddRange(x);
                CCenter.Instance.Bag.Shuffle();
                var tokens = CCenter.Instance.Bag.Draw(2);
                CCenter.Instance.TakeTokens.AddRange(tokens);
            };
            return p;
            
        }
        public IProcess Get5Back3Discard2() {
            var p = sys.NewMultiChoiceProcess<RoastToken>("");
            p.LoopCondition = (x) => {
                return p.ChoicedBGOs.Count() < 2;
            };
            p.ChoiceableFunc = () => {
                if (p.ChoicedBGOs.Count() == 0) {
                    var tokens = CCenter.Instance.Bag.Draw(5);
                    CCenter.Instance.TakeTempTokens.AddRange(tokens);
                    return CCenter.Instance.TakeTempTokens;
                }
                return p.CurrentChoiceableBGO.Except(p.ChoicedBGOs);
            };
            p.DoProgram = (x) => {
                foreach (var it in x) {
                    it.RemovedFromBGOList();
                }
            };
            

            var p2 = sys.NewMultiChoiceProcess<RoastToken>("");
            p2.LoopCondition = (x) => {
                return p2.ChoicedBGOs.Count() < 3;
            };
            p2.ChoiceableFunc = () => {
                return CCenter.Instance.TakeTempTokens;
            };
            p2.DoProgram = (x) => {
                foreach (var it in x) {
                    it.RemovedFromBGOList();
                }
                CCenter.Instance.Bag.AddRange(x);
                CCenter.Instance.Bag.Shuffle();
            };

            p.NextProcess = p2;
            return p;
        }
        public IProcess discardScorePenalty() {
            var p = sys.NewProgram("");
            p.DoProgram = () => {
                foreach (var it in CCenter.Instance.TakeTokens.ToList()) {
                    if (it is DefectBean || it is CharredBean || it is Fumes) {
                        it.RemovedFromBGOList();
                    }
                }
            };
            return p;
        }
        public IProcess back2Tack3() {
            var p = sys.NewMultiChoiceProcess<CupTestSpace>("");
            var p2 = sys.NewMultiChoiceProcess<Token>("");
            p.LoopCondition = (x)=>p.ChoicedBGOs.Count()<2;
            p.ChoiceableFunc = () => center.CupTestSpaces.Where((x)=>!p.ChoicedBGOs.Contains(x));
            p.DoProgram = (x) => {
                foreach (var it in x) {
                    var token = it.Token;
                    it.Clear();
                    center.Bag.Add(token);
                }
                center.Bag.Shuffle();
            };

            p2.LoopCondition = (x) => p2.ChoicedBGOs.Count() < 2;
            p2.ChoiceableFunc = () => {
                if (p2.ChoicedBGOs.Count() == 0) {
                    var tokens = center.Bag.Draw(3);
                    center.TakeTokens.AddRange(tokens);
                }
                return center.TakeTokens.Where((x)=>!p2.ChoicedBGOs.Contains(x));
            };
            p2.DoProgram = (x) => {
                int i = 0;
                foreach (var it in center.CupTestSpaces.Where((y)=>!y.IsFilled)) {
                    var token = x.ElementAt(i++);
                    token.RemovedFromBGOList();
                    it.Place(token);
                }
            };
            p.NextProcess = p2;
        
            return p;
        }
    }
}
