using UnityEngine;

public class GameLobbyUI : MonoBehaviour
{
    public void OnClickHost()
    {
        FusionBootstrap.Instance.StartHost();
    }

    public void OnClickClient()
    {
        FusionBootstrap.Instance.StartClient();
    }

    public void OnClickStartGame()
    {
        FusionBootstrap.Instance.LoadCharSelectScene();
    }
}