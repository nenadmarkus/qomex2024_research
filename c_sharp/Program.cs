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

    static void Main(string[] args)
    {
        Test3();
    }
}