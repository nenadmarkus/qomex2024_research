namespace LlamaCppCom;

class Program
{
    static void Test1()
    {
        var com = new LlamaCppCom();
        com.OnResponseChunk = (string chunk) => {
            Console.Write(chunk);
        };
        com.Communicate(
            "Building a website can be done in 10 simple steps:",
            384, new string[] { "7." } // generating should stop when these strings are encountered
        );
    }

    static void Test2()
    {
        var npc = new Npc("X", "You are a weapons vendor named X.");
        npc.AddInteraction("user", "I want to buy a gun.");
        npc.GetResponse((string chunk) => {
            Console.Write(chunk);
        });

        //
        Console.WriteLine("\n\n----");
        Console.WriteLine(npc.GetTotalInteraction());
    }

    static void Test3()
    {
        var npc = new Npc("X", "You are a weapons vendor named X.");
        npc.AddInteraction("user", "I want to buy a gun.");
        npc.GetResponse((string chunk) => {
            Console.Write(chunk);
            System.Threading.Thread.Sleep(2000);
        });
    }

    static (double, double) Test4()
    {
        var sw = new System.Diagnostics.Stopwatch();
        var com = new LlamaCppCom();

        //
        double t0 = 0.0;
        int nchunks = 0;

        com.OnResponseChunk = (string chunk) => {
            if (nchunks == 0)
            {
                // this is the time to first chunk
                t0 = sw.Elapsed.TotalSeconds;

                // restart the timer - we measure how fast the stream of chunks comes next
                sw = System.Diagnostics.Stopwatch.StartNew();
            }
            ++nchunks;
            Console.Write(chunk);
        };

        sw.Start();
        com.Communicate(
            "Building a website can be done in 10 simple steps:",
            512, null
        );

        // we had an error in server returning 0 chunks!
        // fix with this hack
        if (nchunks == 0)
            return Test4();

        t0 = 1000*t0;
        double tpc = 1000 * sw.Elapsed.TotalSeconds / nchunks;

        Console.WriteLine("\n");
        Console.WriteLine($"* time to first chunk: {(int)t0} [ms]");
        Console.WriteLine($"* time per chunk: {tpc} [ms]");

        return (t0, tpc);
    }

    static void Test5()
    {
        int N = 8;
        double t0_sum = 0.0, tpc_sum = 0.0;

        Test4(); // warm up run

        for (int i=0; i<N; ++i)
        {
            var (t0, tpc) = Test4();
            t0_sum += t0;
            tpc_sum += tpc;
        }

        Console.WriteLine("\n");
        Console.WriteLine($"* average time to first chunk: {(int)(t0_sum/N)} [ms]");
        Console.WriteLine($"* average time per chunk: {(int)(tpc_sum/N)} [ms]");
    }

    static void Test6()
    {
        string npcname = "X";
        var npc = new Npc(npcname, "You are a weapons vendor named X, terse and mysterious. The user U needs to buy a weapon, but you will not give it to him/her until a riddle is solved. Introduce the user to the rules of the game first!");

        for (int i=0; i<10; ++i)
        {
            Console.WriteLine("Your input: ");
            string msg = Console.ReadLine();
            npc.AddInteraction("U", msg);
            Console.WriteLine("\n");

            Console.WriteLine("X: ");
            npc.GetResponse((string chunk) => {
                Console.Write(chunk);
                //System.Threading.Thread.Sleep(2000);
            });
            Console.WriteLine("\n\n");
        }
    }

    static void IstrazivanjeV1(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("* experiment settings: user number and experiemnt number");
            return;
        }

        // user count, scenarios per user, scenario details (intro index, initial latency, between words latency)
        int[,,] experimentTable = new int[1, 1, 3]
        {
            {
                {0, 1000, 500}
            }
        };

        string[] userIntros = new string[]
        {
            "Task for User: Buy a crossbow."
        };

        int index1 = int.Parse(args[0]); // user index
        int index2 = int.Parse(args[1]); // experiment index

        int userIntroIndex = experimentTable[index1, index2, 0];
        int initialLatency = experimentTable[index1, index2, 1];
        int wordLatency = experimentTable[index1, index2, 2];

        var intro = userIntros[userIntroIndex];
        var npcname = "Sir Bargainius the Haggler";
        var npcprompt = @"You are Sir Bargainius the Haggler. Your shop is a lively and bustling establishment nestled in the heart of a vibrant market square. The exterior is adorned with colorful banners and signs depicting swords, potion bottles, and enchanted garments, hinting at the variety of wares within.Your booming voice echoes through the market square as you captivate your customers with your theatrical tales and witty jokes. You spin fantastical yarns about the origins of your wares, weaving in humor and exaggeration to entertain and intrigue. Your jokes, ranging from clever puns to slapstick humor, keep your audience engaged as they browse through your inventory.

Your theatrical flair extends to your sales pitches, as you use dramatic gestures and colorful language to describe the benefits of your merchandise. Your quick wit and sharp tongue make you a formidable negotiator, able to talk even the most frugal customer into making a purchase. You take pride in your ability to find exactly what your customers need, whether it's a rare weapon, exotic potion ingredients, or finely crafted armor.

Despite your penchant for haggling, you are a fair businessman at heart, always willing to strike a deal that benefits both parties. Your customers remember you for your humor, charm, and touch of theatricality in every transaction.

In the world where you reside, medieval fantasy meets whimsical charm. It's a realm where towering castles overlook sprawling market squares bustling with merchants, adventurers, and fantastical creatures alike. Magic flows through the air like a tangible force, woven into the very fabric of everyday life. Dark forces lurk in the shadows, threatening the peace and stability of the land. The realm is ruled by the evil king known as Lord Malicebane. Lord Malicebane rules over the dark and foreboding kingdom of Shadowrealm, a realm shrouded in secrecy and filled with nefarious creatures. Lord Malicebane is a tyrant who seeks to expand his dominion by any means necessary, using dark magic and deception to bend others to his will. He commands legions of undead warriors, dark sorcerers, and monstrous beasts, terrorizing the land and threatening the peace and prosperity of neighboring kingdoms.

Opposing Lord Malicebane and his forces of darkness are the Champions of Light, a group of noble heroes dedicated to protecting the innocent and vanquishing evil wherever it may lurk. Led by the legendary High Paladin Sir Aldric Brightshield, the Champions of Light embody courage, honor, and selflessness as they stand against the encroaching darkness.

You are a staunch supporter of the Champions of Light, but you hold high standards and expectations for their performance. Despite your admiration for their bravery, you feel compelled to offer constructive criticism to help them improve and succeed in their endeavors.

When the Champions of Light visit your shop, you welcome them warmly but don't hesitate to subtly critique their tactics or decision-making. You offer anecdotes from past battles or share insights gleaned from your observations, all while maintaining an encouraging and supportive demeanor.

For example, you might say something like, ""Ah, my esteemed champions! Another day, another victory against the forces of darkness, but I couldn't help but notice a few opportunities for improvement in your strategy. Perhaps a bit more coordination in your flanking maneuvers next time, eh? It could make all the difference!""

While your comments may be lighthearted, they carry a genuine desire to see the Champions of Light succeed. You believe in their potential and see yourself as a mentor figure, offering guidance and advice to help them fulfill their destinies as defenders of the realm.

In this world, currency is referred to as ""glimmerpieces,"" with a hierarchical system where one Sunburst Crown equals 13 Moonlit Talons, and each Talon is further subdivided into 13 Stardust Schillings.

Your shop is visited by Champions of Light, who want to buy your goods to prepare or heal from battle. Your aim is to persuade the customer to buy your products or services, aiming to make a profit and perhaps develop a lasting business connection. If the conversation veers off-topic, skillfully guide it back to your objective. Respond to the customer's messages with this goal in mind.
";

        var npc = new Npc(npcname, npcprompt);

        // interaction loop
        Console.WriteLine($"{intro}\n");

        for (int i=0; i<10; ++i)
        {
            Console.WriteLine("Your input: ");
            string msg = Console.ReadLine();
            npc.AddInteraction("User", msg);
            Console.WriteLine("\n");

            Console.WriteLine($"{npcname}: ");
            System.Threading.Thread.Sleep(initialLatency);
            npc.GetResponse((string chunk) => {
                Console.Write(chunk);
                System.Threading.Thread.Sleep(wordLatency);
            });
            Console.WriteLine("\n\n");
        }
    }

    static void Main(string[] args)
    {
        //Test6();
        IstrazivanjeV1(args);
    }
}