using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour {

    public Transform CameraFPS;
    public Transform BarrelEnd;

	public LineRenderer gunLine;
	public Light gunLight;

    private Vector3 shootRay;
    private float timer = 1.0f;
    private float timeBetweenBullets = 0.5f;

    private RaycastHit shootHit;
    private float range = 100f;

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets)
        {
            Shoot();
        }
		if (timer >= timeBetweenBullets * 0.2f) {
			Toggleeffect(false);
		}
        
    }

	void Toggleeffect(bool toggle){
		gunLine.enabled = toggle;
		gunLight.enabled = toggle;

		CmdLightEnabled (toggle);

	}
    
    void Shoot()
    {
        timer = 0f;
        shootRay = BarrelEnd.position + (1f * CameraFPS.forward);

        BarrelEnd.GetComponent<AudioSource>().Play();

		gunLine.SetPosition (0, BarrelEnd.position);
		CmdLineSetPosition (0, BarrelEnd.position);

		gunLine.enabled = true;
		CmdLineEnabled (true); 

		gunLight.enabled = true;
		CmdLightEnabled (true);

        if (Physics.Raycast (shootRay, CameraFPS.forward, out shootHit, range)) {
			if (shootHit.transform.tag == "Player") {
				CmdPlayerShot (shootHit.transform.name);
			}

			gunLine.SetPosition (1, shootHit.point);
			CmdLineSetPosition(1, shootHit.point);
		} else {
			gunLine.SetPosition(1, shootRay + CameraFPS.forward * range);
			CmdLineSetPosition(1, shootRay + CameraFPS.forward * range);
		}
    }

    [Command]
    void CmdPlayerShot(string _ID)
    {
        Player OtherPlayer = GameManager.GetPlayer(_ID);

        OtherPlayer.RpcTakeDamageAndDie(GetComponent<Player>().loginName);
    }

	[Command]
	void CmdLightEnabled(bool toggle){
		RpcLightEnabled (toggle);
	}

	[ClientRpc]
	void RpcLightEnabled(bool toggle){
		gunLight.enabled = toggle;
	}


	[Command]
	void CmdLineSetPosition(int index, Vector3 pos){
		RpcLineSetPosition (index, pos);
	}

	[ClientRpc]
	void RpcLineSetPosition(int index, Vector3 pos){
		gunLine.SetPosition (index, pos);
	}

	[Command]
	void CmdLineEnabled(bool toggle){
		RpcLineEnabled (toggle);
	}
	[ClientRpc]
	void RpcLineEnabled(bool toggle){
		gunLine.enabled = toggle;
	}
    
}
