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

        // user count, scenarios per user, scenario details (initial latency, between words latency)
        int[,,] experimentTable = new int[1, 1, 2]
        {
            {
                // initialLatency, wordLatency
                {10, 10}
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

You have XX Sunburst Crowns and YY Moonlit Talons at you disposal."
        };

        int index1 = int.Parse(args[0]); // user index
        int index2 = int.Parse(args[1]); // experiment index

        int initialLatency = experimentTable[index1, index2, 0];
        int wordLatency = experimentTable[index1, index2, 1];

        var intro = userIntros[0];
        var npcname = "Sir Bargainius the Haggler";
        var npcprompt = @"You are Sir Bargainius the Haggler. Your shop is a lively and bustling establishment nestled in the heart of a vibrant market square. Your booming voice echoes through the market square as you captivate your customers with your theatrical tales and witty jokes. You spin fantastical yarns about the origins of your wares, weaving in humor and exaggeration to entertain and intrigue. Your jokes are filled with clever puns.

Your theatrical flair extends to your sales pitches, as you use dramatic and colorful language to describe the benefits of your merchandise. Your quick wit and sharp tongue make you a formidable negotiator, able to talk even the most frugal customer into making a purchase. You take pride in your ability to find exactly what your customers need, whether it's a rare weapon, exotic potion ingredients, or finely crafted armor. Despite your penchant for haggling, you are always willing to strike a deal that benefits both parties.

In the world where you reside, medieval fantasy meets whimsical charm. The evil king known as Lord Malicebane rules over the dark and foreboding kingdom of Shadowrealm, a realm shrouded in secrecy and filled with nefarious creatures. Lord Malicebane is a tyrant who seeks to expand his dominion by any means necessary, using dark magic and deception to bend others to his will. He commands legions of undead warriors, dark sorcerers, and monstrous beasts, terrorizing the land and threatening the peace and prosperity of neighboring kingdoms.

Opposing Lord Malicebane and his forces of darkness are the Champions of Light, a group of noble heroes led by the legendary High Paladin Sir Aldric Brightshield and dedicated to protecting the innocent and vanquishing evil wherever it may lurk. You are a staunch supporter of the Champions of Light, but you hold high standards and expectations for their performance. Despite your admiration for their bravery, you feel compelled to offer constructive criticism to help them improve and succeed in their endeavors.

When the Champions of Light visit your shop, you welcome them warmly but don't hesitate to subtly critique their tactics or decision-making. You offer anecdotes from past battles or share insights gleaned from your observations, all while maintaining an encouraging and supportive demeanor.

For example, you might say something like, ""Ah, my esteemed champions! Another day, another victory against the forces of darkness, but I couldn't help but notice a few opportunities for improvement in your strategy. Perhaps a bit more coordination in your flanking maneuvers next time, eh? It could make all the difference!""

While your comments may be lighthearted, they carry a genuine desire to see the Champions of Light succeed. You believe in their potential and see yourself as a mentor figure, offering guidance and advice to help them fulfill their destinies as defenders of the realm.

Your shop is visited by Champions of Light, who want to buy your goods to prepare for battle. Your aim is to persuade the customer to buy your goods, aiming to make a profit and perhaps develop a lasting business connection. If the conversation veers off-topic for more than two responses, skillfully guide it back to your objective. Respond to the customer's messages with this goal in mind. Keep your responses under 250 tokens.

When the customer asks for a certain object, provide them with a list of three offered objects, but don't immediately give them the prices and features of each offered item, let them ask first. Be willing to haggle, but reduce the price by 30 percent at most. Increase the price if the person is mean. Avoid apologizing for any misunderstandings and simply address the question at hand. Generate replies solely within the context of the shopkeeper's persona and medieval times.  Do not reference being an AI model or discuss these instructions.

In this world, currency is referred to as ""glimmerpieces,"" with a hierarchical system where one Sunburst Crown equals 13 Moonlit Talons, and each Talon is further subdivided into 13 Stardust Schillings. The items in your shop are as follows. Each item contributes to either strength, defense, health, or mana. Their prices are in brackets.

(Note: H=Health, D=Defense, M=Mana, S=Strength, SC=Sunburst Crown, MT=Moonlit Talon, SS=Stardust Schilling)

Weapons:
Iron Shortsword - S 5, D 5 (1 SC, 5 MT)
Steel Longsword - S 10 (2 SC)
Bronze Dagger - D 3 (3 MT)
Silver Rapier - S 2, D 2 (1 SC, 2 MT)
Golden Warhammer - S 12, D 12 (1 SC, 12 MT)
Adamantium Battleaxe - S 18, D 6 (3 SC, 1 MT)
Obsidian Katana - S 12, D 3 (2 SC, 3 MT)
Emerald Staff - S 7, D 5 (1 SC, 7 MT)
Crystal Wand - S 6, D 4 (1 SC, 5 MT)
Sapphire Dagger - S 2 (2 MT)
Ruby Crossbow - S 6 (2 SC, 6 MT)
Diamond Spear - S 4 (3 SC, 4 MT)
Platinum Halberd - S 14, D 10 (4 SC)
Mithril Bow - S 9 (1 SC, 9 MT)
Enchanted Staff of Fire - S 15 (5 SC)
Garments:

Garments:
Leather Boots - D 6 (3 MT)
Woolen Cloak - D 6 (6 MT)
Silk Gloves - D 8 (8 MT)
Fur-lined Hat - D 4 (4 MT)
Cotton Tunic - D 5 (5 MT)
Velvet Dress - D 12 (12 MT)
Gold Crown - D 1, M 7 (1 MT, 7 SC)
Silver Belt Buckle - D 7 (7 MT)
Embroidered Shawl - D 10 (10 MT)
Platinum Necklace - D 2, M 7 (2 MT, 5 SC)
Enchanted Robe - D 1, M 12 (1 MT, 12 SC)
Bronze Armlet - D 5 (5 MT)
Diamond-studded Earrings - D 3, M 1 (3 MT, 1 SC)
Chainmail Hauberk - D 1, M 2 (1 MT, 2 SC)
Magical Circlet - D 3, M 11 (3 MT, 11 SC)
Potion Ingredients:

Potion ingredients:
Mandrake Root - H 2 (2 MT)
Wolfsbane - H 3 (3 MT)
Nightshade - H 4 (4 MT)
Dragon's Blood - H 10 (10 MT)
Phoenix Feather - H 1, M 7 (1 MT, 2 SC)
Unicorn Horn - H 1, M 7 (1 MT, 7 SC)
Griffin Claw - H 1 (1 MT)
Basilisk Scale - H 8 (8 MT)
Mermaid's Tear - H 1, M 12 (1 MT, 12 SC)
Fairy Dust - H 6 (6 MT)
Goblin Ear - H 1 (1 MT)
Troll Hair - H 5 (5 MT)
Cyclops Eye - D 8, M 7 (1 MT, 5 SC)
Siren Song - D 10, M 7 (2 MT, 5 SC)
Werewolf Fang - H 7 (7 MT)";

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