using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player[] players { get => _currentPlayers.ToArray(); }
    private static List<Player> _currentPlayers = new List<Player>();

    private void Awake()
    {
        _currentPlayers.Add(this);
    }
}
