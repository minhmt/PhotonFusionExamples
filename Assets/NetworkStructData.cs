using Fusion;
using UnityEngine;

public struct NetworkStructPlayerData : INetworkStruct
{

    public NetworkId NetworkIdField;
    public Color NpcColor;
    public NetworkBehaviourId NetworkBehaviourIdField;
    public PlayerRef PlayerRefField;
    // Network Collections must be NOT be declared as auto-implemented properties,
    // and with only a default getter (the IL Weaver will replace the getter).
    [Networked, Capacity(16)]
    public NetworkDictionary<int, NetworkString<_4>> DictOfStrings => default;

    [Networked, Capacity(16)]
    public NetworkDictionary<int,Vector3> items => default;
    [Networked, Capacity(16)]
    public NetworkArray<Vector3> Rotations => default;

}
