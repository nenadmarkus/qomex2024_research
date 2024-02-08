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

    static void Main(string[] args)
    {
        Test6();
    }
}