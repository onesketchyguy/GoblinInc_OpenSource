using UnityEngine;
using LowEngine.Saving;

namespace LowEngine
{
    public static class DialogueSys
    {
        private static string[] celebrations = new string[]
        {
            "I *&!*$^# rule!", "Heck yeah!",
            "I'm not saying I'm awesome. I am saying that I'm the opposite of not awesome.",
            "I rule!", "Woo!", "Woo Hoo!", "Boo ya!",
            "Sometimes I wish I could be some one else, so I could witness how awesome I am.",
            "If I'm not the greatest, then I'm as close as I'm gonna get... Yeah, I'm the greatest."
        };

        private static string[] pitty = new string[]
        {
            "Why don't they just fire me?",
            "My life is a constant stream of sorrow and misery.",
            "Today sucks.",
            "I remember a time when you could go to work, and enjoy your day... No wait that was a dream.",
            "Who would have guessed I would drop my warm blood all over my new shirt, and break my favourite mug, and run over my grandma with all in the same day?",
            "One of these days I won't hate working here. Wait that was too positive, I mean one of these days I'll be dead.",
            "What a wonderful day! Said some one else, my day has been miserable."
        };

        private static string[] startedWork = new string[]
        {
            "Back to the grind.",
            "Another day, another... Well I get paid weekly so I guess it's not always another dollar.",
            "What if I just... Didn't press a button? Oh who am I kidding, I'm gonna press all the buttons.",
            "I really hope my desk doesn't smell like THAT all day again...",
            "One of these days I'm gonna go straight to the top! Where ever that may be... Wait is there a top?",
            "What if I just... Didn't press a button? Oh who am I kidding, I'm gonna press all the buttons.",
        };

        private static string[] randomThoughts = new string[]
        {
            "Work, work, work! Is that all I'm good for? Hm... I think in the long term yes.",
            "Work, work, work.",
            "I wonder if that bird will still be there when I get home..",
            "Some times I get this feeling like we're all just part of a small simulation, developed entirely by one guy in his spare time as some sort of homage to those flash games no body plays any more.",
            "I think if I think a thought that was already thought by a thinker that thought before I thought that think my thinks may be thought too thinking to be thought in a clear sense...",
            "There was this one time... Oh no I lost my train of thought.",
            "...",
            "Some times I think about thinking.",
            "GREAAAAAAAHHH! And that's when I had simply had enough of her. Walked right out of that button shop after that.",
            "She sells sea shells by the sea shore, well that's not too hard to think, I don't know what people are on about.",
            "There's this rumor that there's an accounting office... I don't understand what they would be counting, but it sounds terrific!",
            "Huh? What? I'm awake...",
        };

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

        public static string GetPitty()
        {
            return pitty[Random.Range(0, pitty.Length)];
        }

        public static string GetWorkPhrase()
        {
            return startedWork[Random.Range(0, startedWork.Length)];
        }

        public static string GetRandomPhrase()
        {
            return randomThoughts[Random.Range(0, randomThoughts.Length)];
        }

        internal static string GetName()
        {
            LoadNames();

            string FirstName = FirstNames[Random.Range(0, FirstNames.Length)];
            string LastName = LastNames[Random.Range(0, LastNames.Length)];

            return $"{FirstName} {LastName}";
        }

        private static void LoadNames()
        {
            string[] firstNames;
            string[] lastNames;

            SaveManager.LoadNames(out firstNames, out lastNames);

            if (firstNames != null) FirstNames = firstNames;
            if (lastNames != null) LastNames = lastNames;
        }
    }
}