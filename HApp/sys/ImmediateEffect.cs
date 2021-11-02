using BGS;
using System.Collections.Generic;

namespace CoffeeRoaster {
    public class ImmediateEffect : BGOList<TokenSpace>, ISpaceParent {
        public IEnumerable<TokenSpace> Spaces {
            get { return _spaces; }
        }
        IEnumerable<TokenSpace> _spaces;
        IProcess effect;

        public ImmediateEffect(IProcess effect, params TokenSpace[] spaces) {
            this.effect = effect;
            _spaces = spaces;
            foreach (var it in _spaces) {
                it.SpaceParent = this;
                this.AddAndFillEmptySpace(it);
            }
        }

        public bool CheckFilled() {
            bool isok = true;
            foreach (var it in _spaces) {
                if (!it.IsFilled) isok = false;
            }
            if (isok) {
                BGSys.Instance.CallPriorityNextProcess(effect);
            }
            return isok;
        }

        public bool IsAbleToPlace(RoastToken token) {
            foreach (var it in _spaces) {
                if (it.IsAbleToPlace(token)) {
                    return true;
                }
            }
            return false;
        }
        public bool Place(RoastToken token) {
            foreach (var it in _spaces) {
                if (it.Place(token)) {
                    return true;
                }
            }
            return false;
        }
    }
}
