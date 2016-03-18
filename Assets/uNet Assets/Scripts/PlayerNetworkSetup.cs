using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {

    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsController;
    public Camera fpsCamera;
    public AudioListener audioListener;

	public PlayerShooting playerShooting;

    public override void OnStartLocalPlayer()
    {
		playerShooting.enabled = true;
        fpsController.enabled = true;
        fpsCamera.enabled = true;
        audioListener.enabled = true;
        GameManager.LocalPlayer = GetComponent<Player>();
        base.OnStartLocalPlayer();
    }

    Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        GetComponent<Player>().Setup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netID, _player);
    }

    void OnDisable()
    {
        GameManager.UnRegisterPlayer(transform.name);
    }

    void ToggleRenderer(bool isAlive)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isAlive;
        }
    }
    void ToggleControls(bool isAlive)
    {
        fpsController.enabled = isAlive;
        fpsCamera.cullingMask = ~fpsCamera.cullingMask;
    }
    void Respawn()
    {
        ToggleRenderer(true);
        if (isLocalPlayer)
            ToggleControls(true);
    }
}
