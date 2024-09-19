namespace LlamaCppCom;

class Npc
{
    public Npc(string name, string initial_prompt, string endpoint = "http://127.0.0.1:8000/v1/chat/completions")
    {
        this.Engine = new LlamaCppCom(endpoint: endpoint);
        this.Messages.Add(("system", initial_prompt.Trim()));
    }

    public void AddInteraction(string whowhat, string input)
    {
        this.Messages.Add(("user", input.Trim()));
    }

    public void GetResponse(Action<string> onResponseChunk)
    {
        string response = "";

        this.Engine.OnResponseChunk = (string chunk) => {
            response += chunk;
            onResponseChunk(chunk);
        };

        this.Engine.Communicate(this.Messages);

        response = response.Trim();

        this.Messages.Add(("assistant", response.Trim()));
    }

    public string GetTotalInteraction()
    {
        return "... NOT IMPLEMENTED ...";
    }

    private readonly List<(string role, string content)> Messages = new();
    private readonly LlamaCppCom Engine;
}
