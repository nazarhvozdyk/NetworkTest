using Unity.Netcode;
using UnityEngine;

public class NetworkButtons : MonoBehaviour
{
    private void OnGUI()
    {
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsServer)
            return;

        var UIRect = new Rect(25, 25, 200, 600);
        GUILayout.BeginArea(UIRect);

        if (GUILayout.Button("Host")) OnStartHostButtonDown();
        if (GUILayout.Button("Client")) OnStartClientButtonDown();

        GUILayout.EndArea();
    }

    private void OnStartHostButtonDown() => NetworkManager.Singleton.StartHost();
    private void OnStartClientButtonDown() => NetworkManager.Singleton.StartClient();
}
