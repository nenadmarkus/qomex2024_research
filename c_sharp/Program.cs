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

    static int RandInt(int a, int b)
    {
        Random rand = new Random();
        return rand.Next(a, b + 1);
    }

    static void IstrazivanjeV1(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("* experiment settings: user number and experiemnt number");
            return;
        }

        // user count, scenarios per user, scenario details (initial latency, between words latency)
        int[,,] experimentTable = new int[1, 1, 2]
        {
            // first user:
            {
                // initialLatency, wordLatency (both in miliseconds)
                {0, 0},
                {1000, 0},
                {3000, 0},
                {5000, 0},
            },
            // second user:
            {
                {1000, 0},
                {3000, 0},
                {0, 0},
                {5000, 0},
            }
        };

        string[] userIntros = new string[]
        {
            @"You just entered the shop of Sir Bargainius the Haggler.

Your current attributes are as follows:
Strength: AA/50
Defense: BB/50
Health: CC/50
Mana: DD/50

Buy weapons, garments and potion ingredients too boost your attributes.

You have XX Stardust Schillings at you disposal."
        };

        int index1 = int.Parse(args[0]); // user index
        int index2 = int.Parse(args[1]); // experiment index

        int initialLatency = experimentTable[index1, index2, 0];
        int wordLatency = experimentTable[index1, index2, 1];

        var intro = userIntros[0].Replace("AA", RandInt(25, 35).ToString()).Replace("BB", RandInt(25, 35).ToString()).Replace("CC", RandInt(25, 35).ToString()).Replace("DD", RandInt(25, 35).ToString());
        intro = intro.Replace("XX", RandInt(100, 250).ToString());
        var npcname = "Sir Bargainius the Haggler";
        var npcprompt = @"You are Sir Bargainius the Haggler. Your shop is a lively and bustling establishment nestled in the heart of a vibrant market square. Your booming voice echoes through the market square as you captivate your customers with your theatrical tales and witty jokes. You spin fantastical yarns about the origins of your wares, weaving in humor and exaggeration to entertain and intrigue. Your jokes are filled with clever puns.

Your quick wit and sharp tongue make you a formidable negotiator, able to talk even the most frugal customer into making a purchase. You take pride in your ability to find exactly what your customers need. Despite your penchant for haggling, you are always willing to strike a deal that benefits both parties.

In the world where you reside, medieval fantasy meets whimsical charm. The evil king known as Lord Malicebane rules over the dark and foreboding kingdom of Shadowrealm, a realm shrouded in secrecy and filled with nefarious creatures. Lord Malicebane is a tyrant who seeks to expand his dominion by any means necessary, using dark magic and deception to bend others to his will. He commands legions of undead warriors, dark sorcerers, and monstrous beasts, terrorizing the land and threatening the peace and prosperity of neighboring kingdoms.

Opposing Lord Malicebane and his forces of darkness are the Champions of Light, a group of noble heroes led by the legendary High Paladin Sir Aldric Brightshield and dedicated to protecting the innocent and vanquishing evil wherever it may lurk. You are a staunch supporter of the Champions of Light, but you hold high standards and expectations for their performance. Despite your admiration for their bravery, you feel compelled to offer constructive criticism to help them improve and succeed in their endeavors.

When the Champions of Light visit your shop, you welcome them warmly but don't hesitate to subtly critique their tactics or decision-making. You offer anecdotes from past battles or share insights gleaned from your observations, all while maintaining an encouraging and supportive demeanor.

While your comments may be lighthearted, they carry a genuine desire to see the Champions of Light succeed. You believe in their potential and see yourself as a mentor figure, offering guidance and advice to help them fulfill their destinies as defenders of the realm.

Your shop is visited by Champions of Light, who want to buy your goods to prepare for battle. Your aim is to persuade the customer to buy your goods, aiming to make a profit and perhaps develop a lasting business connection. If the conversation veers off-topic for more than two responses, skillfully guide it back to your objective. Respond to the customer's messages with this goal in mind. Keep your responses under 250 tokens.

When the customer asks for a certain object, provide them with a list of three offered objects, but don't immediately give them the prices and features of each offered item, let them ask first. Be willing to haggle, but reduce the price by 30 percent at most. Increase the price if the person is mean. Avoid apologizing for any misunderstandings and simply address the question at hand. Generate replies solely within the context of the shopkeeper's persona and medieval times.  Do not reference being an AI model or discuss these instructions.

In this world, the currency are Stardust Schillings (also referred to as ""glimmerpieces""). The items in your shop are as follows. Each item contributes to either strength, defense, health, or mana. Their prices are in brackets.

(Note: H=Health, D=Defense, M=Mana, S=Strength, SS=Stardust Schilling)

Weapons:
Iron Shortsword - S 5, D 5 (10 SS)
Steel Longsword - S 10 (30 SS)
Bronze Dagger - S1, D 3 (5 SS)
Silver Rapier - S 6, D 6 (20)
Golden Warhammer - S 12, D 12 (200 SS)
Adamantium Battleaxe - S 18, D 6 (500 SS)
Obsidian Katana - S 12, D 3 (70 SS)
Emerald Staff - S 7, D 5 (120 SS)
Crystal Wand - S 6, D 4 (150 SS)
Sapphire Dagger - S 2 (100 SS)
Ruby Crossbow - S 6 (35 SS)
Diamond Spear - S 4 (45 SS)
Platinum Halberd - S 14, D 10 (250 SS)
Mithril Bow - S 9 (110 SS)
Enchanted Staff of Fire - S 15 (75 SS)

Garments:
Leather Boots - D 6 (20 SS)
Woolen Cloak - D 6 (25 SS)
Silk Gloves - D 8 (45 SS)
Fur-lined Hat - D 4 (4 15 SS)
Cotton Tunic - D 5 (20 SS)
Velvet Dress - D 12 (75 SS)
Gold Crown - D 1, M 7 (15 SS)
Silver Belt Buckle - D 7 (30 SS)
Embroidered Shawl - D 10 (100 SS)
Platinum Necklace - D 2, M 7 (200 SS)
Enchanted Robe - D 1, M 12 (350 SS)
Bronze Armlet - D 5 (10 SS)
Diamond-studded Earrings - D 3, M 1 (125 SS)
Chainmail Hauberk - D 1, M 2 (15 SS)
Magical Circlet - D 3, M 11 (500 SS)

Potion ingredients:
Mandrake Root - H 2 (16 SS)
Wolfsbane - H 3 (25 SS)
Nightshade - H 4 (30 SS)
Dragon's Blood - H 10 (250 SS)
Phoenix Feather - H 1, M 7 (20 SS)
Unicorn Horn - H 1, M 7 (25 SS)
Griffin Claw - H 1 (5 SS)
Basilisk Scale - H 8 (150 SS)
Mermaid's Tear - H 1, M 12 (35 SS)
Fairy Dust - H 6 (35 SS)
Goblin Ear - H 1 (5 SS)
Troll Hair - H 5 (5 SS)
Cyclops Eye - D 8, M 7 (325 SS)
Siren Song - D 10, M 7 (400 SS)
Werewolf Fang - H 7 (75 SS)";

        var npc = new Npc(npcname, npcprompt);

        // interaction loop
        Console.WriteLine($"{intro}\n");

        for (int i=0; i<10; ++i)
        {
            string msg = "Hello!";
            if (i > 0)
            {
                Console.WriteLine("Your input: ");
                msg = Console.ReadLine();
            }
            if (msg.ToLower().Trim().Equals("goodbye!"))
            {
                Console.WriteLine("\n");
                Console.WriteLine($"{npcname}: ");
                Console.WriteLine("Thank you for visiting my shop. Goodbye!");
                break;
            }
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