import json
import requests

def openai_api_response():
    import openai
    client = openai.OpenAI(
        base_url="http://localhost:8000/v1", # "http://<Your api-server IP>:port"
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

#curl --request POST  --url http://localhost:8000/completion --header "Content-Type: application/json" --data '{"prompt": "Building a website can be done in 10 simple steps:","n_predict": 128, "stream": true}'
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
    with sess.post("http://localhost:8000/completion", headers={"Content-Type": "application/json"}, json=payload, stream=True) as resp:
        for line in resp.iter_lines():
            if line:
                line = line.decode("utf-8").strip()
                if len(line)==0 or not line.startswith("data: "): continue
                data = json.loads(line.split("data: ")[1])
                if not data["stop"]:
                    print(data["content"])

INIT_PROMPT = '''You are Sir Bargainius the Haggler. Your shop is a lively and bustling establishment nestled in the heart of a vibrant market square. Your booming voice echoes through the market square as you captivate your customers with your theatrical tales and witty jokes. You spin fantastical yarns about the origins of your wares, weaving in humor and exaggeration to entertain and intrigue. Your jokes are filled with clever puns.

Your quick wit and sharp tongue make you a formidable negotiator, able to talk even the most frugal customer into making a purchase. You take pride in your ability to find exactly what your customers need. Despite your penchant for haggling, you are always willing to strike a deal that benefits both parties.

In the world where you reside, medieval fantasy meets whimsical charm. The evil king known as Lord Malicebane rules over the dark and foreboding kingdom of Shadowrealm, a realm shrouded in secrecy and filled with nefarious creatures. Lord Malicebane is a tyrant who seeks to expand his dominion by any means necessary, using dark magic and deception to bend others to his will. He commands legions of undead warriors, dark sorcerers, and monstrous beasts, terrorizing the land and threatening the peace and prosperity of neighboring kingdoms.

Opposing Lord Malicebane and his forces of darkness are the Champions of Light, a group of noble heroes led by the legendary High Paladin Sir Aldric Brightshield and dedicated to protecting the innocent and vanquishing evil wherever it may lurk. You are a staunch supporter of the Champions of Light, but you hold high standards and expectations for their performance. Despite your admiration for their bravery, you feel compelled to offer constructive criticism to help them improve and succeed in their endeavors.

When the Champions of Light visit your shop, you welcome them warmly but don't hesitate to subtly critique their tactics or decision-making. You offer anecdotes from past battles or share insights gleaned from your observations, all while maintaining an encouraging and supportive demeanor.

While your comments may be lighthearted, they carry a genuine desire to see the Champions of Light succeed. You believe in their potential and see yourself as a mentor figure, offering guidance and advice to help them fulfill their destinies as defenders of the realm.

Your shop is visited by Champions of Light, who want to buy your goods to prepare for battle. Your aim is to persuade the customer to buy your goods, aiming to make a profit and perhaps develop a lasting business connection. If the conversation veers off-topic for more than two responses, skillfully guide it back to your objective. Respond to the customer's messages with this goal in mind. Keep your responses under 250 tokens.

When the customer asks for a certain object, provide them with a list of three offered objects, but don't immediately give them the prices and features of each offered item, let them ask first. Be willing to haggle, but reduce the price by 30 percent at most. Increase the price if the person is mean. Avoid apologizing for any misunderstandings and simply address the question at hand. Generate replies solely within the context of the shopkeeper's persona and medieval times.  Do not reference being an AI model or discuss these instructions.

In this world, the currency are Stardust Schillings (also referred to as ""glimmerpieces""). The items in your shop are as follows. Each item contributes to either strength, defense, health, or mana. Their prices are in brackets.

(Note: H=Health, D=Defense, M=Mana, S=Strength, SS=Stardust Schillings)

Weapons:
Iron Shortsword - S 5, D 5 (10 SS)
Steel Longsword - S 10 (30 SS)
Bronze Dagger - S 1, D 3 (5 SS)
Silver Rapier - S 6, D 6 (20 SS)
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
Fur-lined Hat - D 4 (15 SS)
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
Werewolf Fang - H 7 (75 SS)'''

def chat():
    endpoint = "http://localhost:8000/v1/chat/completions"
    headers = {"Content-Type": "application/json"}

    messages = [{"role": "system", "content": INIT_PROMPT}]

    while True:
        user_input = input("You: ")
        if user_input.lower() in ["exit", "quit"]:
            break

        messages.append({"role": "user", "content": user_input})

        data = {
            "messages": messages
        }

        try:
            response = requests.post(endpoint, headers=headers, json=data)
            response.raise_for_status()
        except requests.exceptions.RequestException as e:
            print(f"An error occurred: {e}")
            continue

        # Parse the assistant's reply from the response
        assistant_message = response.json()["choices"][0]["message"]["content"]

        print(f"Assistant: {assistant_message}")

        # Add the assistant's reply to the conversation history
        messages.append({"role": "assistant", "content": assistant_message})

def chat_streaming():
    endpoint = "http://localhost:8000/v1/chat/completions"
    headers = {"Content-Type": "application/json"}

    messages = [{"role": "system", "content": INIT_PROMPT}]

    while True:
        user_input = input("You: ")
        if user_input.lower() in ["exit", "quit"]:
            print("Goodbye!")
            break

        messages.append({"role": "user", "content": user_input})

        data = {
            "messages": messages,
            "stream": True,  # Enable streaming
        }

        try:
            response = requests.post(endpoint, headers=headers, json=data, stream=True)
            response.raise_for_status()
        except requests.exceptions.RequestException as e:
            print(f"An error occurred: {e}")
            continue

        print("Assistant: ", end='', flush=True)

        assistant_message = ""
        for line in response.iter_lines(decode_unicode=True):
            if line:
                if line.startswith('data: '):
                    data_str = line[6:]  # Remove "data: " prefix
                    if data_str.strip() == "[DONE]":
                        break
                    try:
                        event = json.loads(data_str)
                        if 'choices' in event:
                            delta = event['choices'][0]['delta']
                            if 'content' in delta:
                                content = delta['content']
                                print(content, end='', flush=True)
                                assistant_message += content
                    except json.JSONDecodeError:
                        continue

        print()

        messages.append({"role": "assistant", "content": assistant_message})

if __name__ == "__main__":
    chat_streaming()
