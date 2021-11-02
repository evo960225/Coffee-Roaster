using BGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeRoaster {
    public class SquareEffect : BGOList<TokenSpace>, ISpaceParent {
        IEnumerable<TokenSpace> _spaces;
        public IEnumerable<TokenSpace> Spaces {
            get { return _spaces; }
        }
        public CupTestSpace SquareTokenSpace { get; protected set; }
        SquareToken _squareToken;
        public SquareToken SquareToken { get { return _squareToken; } }
        
        public SquareEffect(SquareToken token, params TokenSpace[] spaces) {
            SquareTokenSpace =new CupTestSpace();
            SquareTokenSpace.Place(token);
            var bgoc = new BGOContainer();
            bgoc.BGObject = SquareTokenSpace;

            _squareToken = token;
            _spaces = spaces;
            foreach (var it in _spaces) {
                it.SpaceParent = this;
                this.AddAndFillEmptySpace(it);
            }
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

        public bool CheckFilled() {
            bool isok = true;
            foreach (var it in _spaces) {
                if (!it.IsFilled) isok = false;
            }
            if (isok) {
                SquareToken.Get();
            }
            return isok;
        }
    }
}
