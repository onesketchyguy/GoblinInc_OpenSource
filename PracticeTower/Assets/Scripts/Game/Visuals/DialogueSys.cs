using UnityEngine;
using LowEngine.Saving;

namespace LowEngine
{
    public static class DialogueSys
    {
        private static string[] celebrations = new string[] { "I *&!*$^# rule!", "Hell yeah!", "I'm not saying I'm awesome. I am saying that I'm the opposite of not awesome.", "I rule!", "Woo!", "Woo Hoo!" };

        private static string[] FirstNames = new string[]
        {
            "Rorot", "Brolaf", "Haushtahmp", "Harry", "Stinky", "Thudnaldo", "Rile", "Loser", "Knife-face", "Happy", "Face", "Shothead", "Awful", "Ahsome", "Awesome",
            "Mother-Lover", "BloodSoaked", "Rotten", "Yucky", "Ill founded", "Yuck", "Ick", "Gross", "Inefficient", "Blood", "Potty", "Scrott", "Stabber", "Frankendave",
            "Johnny", "Mouth", "Unit No.", "Experiment", "xXb1narieXx", "Snoop", "Lil'", "Lilo", "Stitch", "Mr.Proffesor-Doctor-Captain-Sir", "King", "Barf", "Wiggler",
            "Turd", "Spoiled", "Lemony", "Ugly"
        };
        private static string[] LastNames = new string[]
        {
            "Pricenstein", "Dreadrichards", "Greenrobinson", "Stench", "Anstench", "Cheapwick", "Creep", "Chuckles", "McStab-stab", "Stabber", "McStabby", "Head", "Baby", "Stinker",
            "The Stabbed", "The once-wronged", "Peace-ohe-shet", "McLoser", "Drencher", "Soaker", "Booger", "Buggar", "Dumby", "McEfficient", "McEfficiency", "of Awesometopia", "Lame",
            "[Your ad space here]", "The weak", "The strong", "Strong", "Jarhead", "Crotch", "Hatred", "The French", "McInefficient", "McInefficiency", "Dumb-phuck", "McFace", "00110001",
            "626", "The Second", "Junior", "Jr.", "Barfers", "Barf", "\"King\"", "On the roof", "Rotthand", "Milk", "Sucker", "Rocket"
        };

        public static string GetCelebration()
        {
            return celebrations[Random.Range(0, celebrations.Length)];
        }

        internal static string GetName()
        {
            loadNames();

            string FirstName = FirstNames[Random.Range(0, FirstNames.Length)];
            string LastName = LastNames[Random.Range(0, LastNames.Length)];

            return $"{FirstName} {LastName}";
        }

        private static void loadNames()
        {
            string[] firstNames;
            string[] lastNames;

            SaveManager.LoadNames(out firstNames, out lastNames);

            if (firstNames != null) FirstNames = firstNames;
            if (lastNames != null) LastNames = lastNames;

            SaveManager.SaveNames(FirstNames, LastNames);
        }
    }
}