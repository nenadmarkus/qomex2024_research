namespace LlamaCppCom;

class Npc
{
    public Npc(string name, string initial_prompt, string endpoint = "http://127.0.0.1:8000/completion")
    {
        this.Engine = new LlamaCppCom(endpoint: endpoint);
        this.Prompt = initial_prompt.Trim();
        this.Name = name;
    }

    public void AddInteraction(string whowhat, string input)
    {
        this.Interaction.Add( (whowhat, input) );
    }

    public void GetResponse(Action<string> onResponseChunk)
    {
        string response = "";

        this.Engine.OnResponseChunk = (string chunk) => {
            response += chunk;
            onResponseChunk(chunk);
        };

        string interaction = "<|begin_of_text|><|start_header_id|>system<|end_header_id|>You are a helpful assistant that needs to complete the mystical interaction below (just one turn!)<|eot_id|>";
        interaction += "<|start_header_id|>user<|end_header_id|>" + this.Prompt.Trim();
        foreach (var t in this.Interaction)
        {
            interaction += "\n\n" + t.Item1 + ":\n" + t.Item2.Trim();
        }

        interaction += "\n\n" + this.Name + ":\n";
        interaction += "<|eot_id|><|start_header_id|>assistant<|end_header_id|>";

        this.Engine.Communicate(
            interaction,
            2048,
            new string[] { "<|eot_id|>" }
        );

        response = response.Trim();

        this.Interaction.Add( (this.Name, response) );

        this.TotalInteraction = interaction + response;
    }

    public string GetTotalInteraction()
    {
        return this.TotalInteraction;
    }

    private string Prompt, Name, TotalInteraction;
    private readonly List<(string, string)> Interaction = new();
    private readonly LlamaCppCom Engine = null;
}
