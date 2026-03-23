using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GlobalPressurePlate2D : MonoBehaviour
{
    [SerializeField] private LayerMask pressMask;
    [SerializeField] private int channelID = 0;

    // Static Dictionary: This is the "Shared Memory" for the whole game
    private static readonly Dictionary<int, bool> _channelStates = new Dictionary<int, bool>();
    private static readonly Dictionary<int, int> _activeCounts = new Dictionary<int, int>();

    public static bool GetChannelState(int id)
    {
        return _channelStates.ContainsKey(id) && _channelStates[id];
    }

    private readonly HashSet<Collider2D> _localPressing = new HashSet<Collider2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & pressMask) == 0) return;

        if (_localPressing.Add(other))
        {
            if (!_activeCounts.ContainsKey(channelID)) _activeCounts[channelID] = 0;
            _activeCounts[channelID]++;
            _channelStates[channelID] = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_localPressing.Remove(other))
        {
            _activeCounts[channelID]--;
            if (_activeCounts[channelID] <= 0)
            {
                _activeCounts[channelID] = 0;
                _channelStates[channelID] = false;
            }
        }
    }

    private void OnDestroy()
    {
        _activeCounts.Clear();
        _channelStates.Clear();
    }
}