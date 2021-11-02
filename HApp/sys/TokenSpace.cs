using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGS;

namespace CoffeeRoaster {
    public interface ISpaceParent {
        bool CheckFilled();
    }
    abstract public class TokenSpace : BGO {
        public ISpaceParent SpaceParent { get; set; }
        public bool IsFilled { get; protected set; }
        public Token Token { get; protected set; }
        abstract public bool IsAbleToPlace(Token placed);
        public bool Place(Token placed) {
            var isok = IsAbleToPlace(placed);
            
            if (isok) {
                IsFilled = true;
                Token = placed;
                if(SpaceParent!=null)SpaceParent.CheckFilled();
            }
            return isok;
        }
        public void Clear() {
            Token = null;
            IsFilled = false;
        }
    }
    public class FlavorSpace : TokenSpace {

        public FlavorType FlavorType { get; protected set; }
        public FlavorSpace(FlavorType conditionType) {
            FlavorType = conditionType;
        }

        public override bool IsAbleToPlace(Token placed) {
            if (IsFilled) return false;
            if (placed is IFlavor) {
                var p = placed as IFlavor;
                if ((p.Type & FlavorType) == FlavorType || (p.Type & FlavorType) == p.Type) {
                    return true;
                }
            }
            return false;
        }
    }
    public class BeanSpace : TokenSpace {

        int conditionDegree;
        public BeanSpace(int conditionDegree) {
            this.conditionDegree = conditionDegree;
        }

        public override bool IsAbleToPlace(Token placed) {
            if (IsFilled) return false;
            if (placed is CoffeeBean) {
                var p = placed as IBean;
                if (p.Degree == conditionDegree) {
                    return true;
                }
            }
            return false;
        }
    }

    public class CupTestSpace : TokenSpace {

        public CupTestSpace() {
            
        }

        public override bool IsAbleToPlace(Token placed) {
            if (IsFilled) return false;
            return true;
        }
    }
}
