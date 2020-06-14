using System;
public class Message
{
    public string type;
    public string data;

    public Message()
    {
    }

    public Message(string type, string data)
    {
        this.type = type;
        this.data = data;
    }
}
