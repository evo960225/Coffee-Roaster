using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {
    public class Deck<CardT> : BGOList<CardT> where CardT : BGO {

        public Deck() {

        }
        public Deck(int capacity)
            : base(capacity) {

        }

        public void Shuffle() {
            for (int i = this.Count() * 3; i > 0; --i) {
                var index = hoshi_lib.Rand.Next_(0, this.Count());
                var card = this[index];
                this.Remove(card);
                this.Add(card);
            }
        }
        public CardT Draw() {
            if (this.Count() > 0) {
                var card = this[0];
                this.Remove(card);
                return card;
            }
            return default(CardT);
        }

        /// <summary>
        /// when deck is empty, card can't be drawn more
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<CardT> Draw(int count) {
            List<CardT> list = new List<CardT>(count + 1);
            while (this.Count() > 0 && count > 0) {
                list.Add(this[0]);
                this.RemoveAt(0);
                --count;
            }
            return list;
        }
    }
}
