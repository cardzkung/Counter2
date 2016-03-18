using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

	public Text hpLocal;

	void Update(){
		hpLocal.text = "HP: " + GetLocalPlayer.currentHealth;
	}

    private const string PLAYER_ID_PREFIX = "Player";

    public static Dictionary<string, Player> players = new Dictionary<string, Player>();
    public static Player LocalPlayer;

    public static void RegisterPlayer(string _netID, Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        _player.transform.name = _playerID;
        _player.loginName = _playerID;
        players.Add(_playerID, _player);
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static Player GetPlayer(string _playerID)
    {
        return players[_playerID];
    }
    public static List<Player> GetPlayers()
    {
        List<Player> player = new List<Player>();
        foreach (KeyValuePair<string, Player> pare in players)
        {
            player.Add(pare.Value);
        }
        return player;
    }

    public static Player GetLocalPlayer
    {
        get { return LocalPlayer; }
    }
}
