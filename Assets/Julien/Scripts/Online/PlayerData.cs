using System;

[Serializable]
public struct PlayerData
{
    public string Username { get; private set; }
    public int ChoosenColor { get; private set; }

    public PlayerData(string username, int choosenColor)
    {
        Username = username;
        ChoosenColor = choosenColor;
    }
}