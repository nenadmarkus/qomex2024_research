* Compile:

```
git clone https://github.com/ggerganov/llama.cpp
cd llama.cpp
git checkout b2740
mkdir build
cd build
cmake .. -DCMAKE_APPLE_SILICON_PROCESSOR=arm64 
make -j 8
```

* Download the model from <https://huggingface.co/QuantFactory/Meta-Llama-3-8B-Instruct-GGUF>

* Run server:

```
./llama.cpp/build/bin/server --model Meta-Llama-3-8B-Instruct.Q3_K_L.gguf --port 8000 --host 0.0.0.0
```