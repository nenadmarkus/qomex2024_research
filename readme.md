

* Compile:

```
git clone https://github.com/ggerganov/llama.cpp
cd llama.cpp
mkdir build
cd build
cmake .. -DCMAKE_APPLE_SILICON_PROCESSOR=arm64 
make -j 8
```

* Download the model from <https://huggingface.co/QuantFactory/Meta-Llama-3-8B-GGUF> -- not that this is the base model, not instruction tuned!

* Run server:

```
./llama.cpp/build/bin/server --model mistral-7b-instruct-v0.1.Q2_K.gguf --port 8000 --host 0.0.0.0
```