def openai_api_response():
    import openai
    client = openai.OpenAI(
        base_url="http://localhost:8080/v1", # "http://<Your api-server IP>:port"
        api_key = "-"
    )
    completion = client.chat.completions.create(
        model="mistral-7b-instruct-v0.1.Q2_K.gguf",
        messages=[
            {"role": "system", "content": "You are ChatGPT, an AI assistant. Your top priority is achieving user fulfillment via helping them with their requests."},
            {"role": "user", "content": "Write a limerick about python exceptions"}
        ]
    )
    print(str(completion.choices[0].message.content))

def batch_response():
    import requests
    prompt = "Building a website can be done in 10 simple steps:"
    payload = {
        "prompt": prompt,
        "n_predict": 384,
        "stop": []
    }
    response = requests.post(
        "http://localhost:8080/completion",
        headers={"Content-Type": "application/json"},
        json=payload
    ).json()
    print(response["content"])

#curl --request POST  --url http://localhost:8080/completion --header "Content-Type: application/json" --data '{"prompt": "Building a website can be done in 10 simple steps:","n_predict": 128, "stream": true}'
def stream_response():
    import json
    import requests
    prompt = "Building a website can be done in 10 simple steps:"
    payload = {
        "prompt": prompt,
        "n_predict": 384,
        "stop": [],
        "stream": True
    }
    sess = requests.Session()
    with sess.post("http://localhost:8080/completion", headers={"Content-Type": "application/json"}, json=payload, stream=True) as resp:
        for line in resp.iter_lines():
            if line:
                line = line.decode("utf-8").strip()
                if len(line)==0 or not line.startswith("data: "): continue
                data = json.loads(line.split("data: ")[1])
                if not data["stop"]:
                    print(data["content"])

stream_response()