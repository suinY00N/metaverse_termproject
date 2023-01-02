import whisper


def translate2text(audio):
    model = whisper.load_model("base")
    result = model.transcribe(audio)
    print(type(result["text"]), result["text"])

    return result["text"]


# print(translate2text("you.m4a"))
