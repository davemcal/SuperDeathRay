using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVDU
{
    class Score
    {
        public static int score = 0;
        public static int miss_combo = 0;

        public static void reset()
        {
            score = 0;
            miss_combo = 0;
        }

        public static void miss()
        {
            score += miss_combo + 1;
            miss_combo++;
        }

        public static void hit()
        {
            miss_combo = 0;
        }
    }
}
