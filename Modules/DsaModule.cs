using System;
using System.Collections.Generic;

namespace _04_dsa.Modules {
    public class DsaModule {
        private Random random = new Random();
        public int[] Dice(int count, int max) {
            return Dice(count, 1, max);
        }

        public int[] Dice(int count, int min, int max) {
            if (count < 1)
                return new int[1] { -1 };
            int[] retvals = new int[count];
            for (int i = 0; i < count; i++) {
                retvals[i] = random.Next(min, max + 1);
            }
            return retvals;
        }

    }

    public class _3_W_20_Score {
        public int W1 { get; private set; }
        public int W2 { get; private set; }
        public int W3 { get; private set; }

        public int Sternchen { get; private set; }
        public int Qualität { get; private set; }
        public Ergebnis ergebnis { get; private set; }

        public _3_W_20_Score(_3_W_20_Throw t) {
            var random = new Random();

            W1 = random.Next(1, 21);
            W2 = random.Next(1, 21);
            W3 = random.Next(1, 21);

            // prüft auf krits
            if (W1 + W2 == 2
                || W1 + W3 == 2
                || W2 + W3 == 2) {
                ergebnis = Ergebnis.krit;
                Sternchen = t.TaW * 2;
                Qualität = 0;
                return;
            }

            // prüft auf patzer
            if (W1 + W2 == 40
                || W1 + W3 == 40
                || W2 + W3 == 40) {
                ergebnis = Ergebnis.patzer;
                Sternchen = 0;
                Qualität = 0;
                return;
            }

            int tap = t.TaW - t.Modifikation;
            Sternchen = t.TaW < tap + CalculateSternchen(t) ? t.TaW : tap + CalculateSternchen(t);
            Qualität = Sternchen + CalculateQuali(t) - t.Modifikation + CalculateSternchen(t);
            if (Sternchen < 0)
                ergebnis = Ergebnis.nicht_bestanden;
            else
                ergebnis = Ergebnis.bestanden;
        }

        // I hate myself for using ternary operators
        private int CalculateSternchen(_3_W_20_Throw t) {
            int toomuch;
            toomuch = W1 > t.Wert1 ? t.Wert1 - W1 : 0;
            toomuch += W2 > t.Wert2 ? t.Wert2 - W2 : 0;
            toomuch += W3 > t.Wert3 ? t.Wert3 - W3 : 0;
            return toomuch;
        }

        // I hate myself for using ternary operators
        private int CalculateQuali(_3_W_20_Throw t) {
            int minDiff = 0;
            minDiff = W1 < t.Wert1 ? t.Wert1 - W1 : 0;
            minDiff = W2 < t.Wert2 ? minDiff > (t.Wert2 - W2) ? (t.Wert2 - W2) : minDiff : 0;
            minDiff = W3 < t.Wert3 ? minDiff > (t.Wert3 - W3) ? (t.Wert3 - W3) : minDiff : 0;
            return minDiff;
        }
    }

    public enum Ergebnis {
        nicht_bestanden,
        bestanden,
        patzer,
        krit
    };

    public enum Eigenschaften {
        MU,
        KL,
        IN,
        CH,
        FF,
        GE,
        KO,
        KK
    }

    public class _3_W_20_Throw {
        public string Talent;
        public string Eigenschaft1;
        public string Eigenschaft2;
        public string Eigenschaft3;

        public int TaW;
        public int Wert1;
        public int Wert2;
        public int Wert3;

        public int Modifikation = 0;

        public _3_W_20_Throw(string talent, string eigenschaft1, string eigenschaft2, string eigenschaft3, int taw, int wert1, int wert2, int wert3, int modifikation = 0) {
            Talent = talent;
            Eigenschaft1 = eigenschaft1;
            Eigenschaft2 = eigenschaft2;
            Eigenschaft3 = eigenschaft3;
            TaW = taw;
            Wert1 = wert1;
            Wert2 = wert2;
            Wert3 = wert3;
            Modifikation = modifikation;
        }
    }

}
