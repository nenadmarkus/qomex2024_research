using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LlamaCppCom;

class LlamaCppCom
{
    public LlamaCppCom(string endpoint = "http://127.0.0.1:8000/completion")
    {
        this.endpoint = endpoint;
    }

    public Action<string>? OnResponseChunk;

    public void Communicate(string prompt, int n_predict, string[] stop_seqs)
    {
        // prepare JSON payload
        var payload = new
        {
            prompt = prompt,
            n_predict = n_predict,
            stop = stop_seqs,
            stream = true
        };

        // prepare HTTP request
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        };

        // send the request with the correct completion option
        // (this ensures we can stream the response)
        using var response = HTTP.Send(request, HttpCompletionOption.ResponseHeadersRead);

        // Ensure we got a successful response
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("* chatbot server error: response.StatusCode: " + response.StatusCode);
        }

        // stream the response
        using var stream = response.Content.ReadAsStream();
        using var reader = new System.IO.StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("data: "))
            {
                var data = JsonSerializer.Deserialize<dynamic>(line.Substring(6));
                if (data is not null && data.GetProperty("stop").GetBoolean() is false)
                {
                    this.OnResponseChunk?.Invoke(
                        data.GetProperty("content").GetString()
                    );
                }
            }
        }
    }

    private readonly string endpoint = "";
    private static readonly HttpClient HTTP = new() { Timeout=TimeSpan.FromMilliseconds(5000) };
}