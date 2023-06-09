using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    private bool _mouseButtton0;
    private bool _mouseButtton1;

    private void Update()
    {
        _mouseButtton0 = _mouseButtton0 | Input.GetMouseButton(0);
        _mouseButtton1 = _mouseButtton1 | Input.GetMouseButton(1);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if(Input.GetKey(KeyCode.W))
        {
            data.direction += Vector3.forward;
        }
        if(Input.GetKey(KeyCode.S))
        {
            data.direction += Vector3.back;
        }
        if(Input.GetKey(KeyCode.A))
        {
            data.direction += Vector3.left;
        }
        if(Input.GetKey(KeyCode.D))
        {
            data.direction += Vector3.right;
        }

        if(_mouseButtton0)
        {
            data.buttons |= NetworkInputData.MOUSEBUTTON1;

            if (runner.IsClient) {
                Debug.LogError("SEND STRUCT DATA FROM CLIENT..");
                data.playerData.NpcColor = Color.blue;
                data.playerData.DictOfStrings.Set(1, "One");
                data.playerData.DictOfStrings.Set(4, "Four");

                data.playerData.items.Set(1, new Vector3(100,200,300));
                data.playerData.items.Set(2, new Vector3(300, 100, 200));

                data.playerData.Rotations.Set(1, new Vector3(100, 200, 300));
                data.playerData.Rotations.Set(3, new Vector3(100, 200, 300));



            }

        }
        if (_mouseButtton1)
        {
            data.buttons |= NetworkInputData.MOUSEBUTTON2;
        }

        _mouseButtton0 = false;
        _mouseButtton1 = false;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {  
        if(runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded%runner.Config.Simulation.DefaultPlayers)*3, 1,0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
            Debug.Log("SEVER JOIN");
        }
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if(_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    private NetworkRunner _runner;

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        }
        );
    }


    private void OnGUI()
    {
        if(_runner == null)
        {
            if(GUI.Button(new Rect(0,0,200,40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if(GUI.Button(new Rect(0,40,200,40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

}
