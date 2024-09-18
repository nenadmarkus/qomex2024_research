using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LlamaCppCom;

class LlamaCppCom
{
    public LlamaCppCom(string endpoint = "http://127.0.0.1:8000/v1/chat/completions")
    {
        this.endpoint = endpoint;
    }

    public Action<string>? OnResponseChunk;

    public void Communicate(List<Dictionary<string, string>> messages)
    {
        // prepare JSON payload
        var payload = new
        {
            messages = messages,
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
                var dataStr = line.Substring(6).Trim(); // Remove "data: " prefix

                if (dataStr == "[DONE]")
                {
                    break;
                }

                try
                {
                    using JsonDocument jsonDoc = JsonDocument.Parse(dataStr);
                    JsonElement root = jsonDoc.RootElement;

                    if (root.TryGetProperty("choices", out JsonElement choicesElement))
                    {
                        foreach (JsonElement choice in choicesElement.EnumerateArray())
                        {
                            if (choice.TryGetProperty("delta", out JsonElement deltaElement))
                            {
                                if (deltaElement.TryGetProperty("content", out JsonElement contentElement))
                                {
                                    string content = contentElement.GetString() ?? "";

                                    // Invoke the response chunk action
                                    OnResponseChunk?.Invoke(content);
                                }
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    // Handle JSON parsing errors
                    Console.WriteLine($"JSON parse error: {ex.Message}");
                    continue;
                }
            }
        }
    }

    private readonly string endpoint = "";
    private static readonly HttpClient HTTP = new() { Timeout=TimeSpan.FromMilliseconds(5000) };
}