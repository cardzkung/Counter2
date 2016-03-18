using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
	public Text hpText;

	void Update(){
		hpText.text = "HP: " + currentHealth;
	}

    Renderer[] renderers;
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }
    void ToggleRenderer(bool isAlive)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isAlive;
        }
    }

    private IEnumerator Respown()
    {
        yield return new WaitForSeconds(3f);
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
        ToggleRenderer(true);
        setDefaults(false);
    }

    [SerializeField]
    private string playerLoginName;

    private int maxHP = 100;

    private int damage = 25;

    [SyncVar]
    private int currentHP;

    [SyncVar]
    private int kill;
    [SyncVar]
    private int dead;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [ClientRpc]
    public void RpcTakeDamageAndDie(string id)
    {
        Player pClass = GameManager.GetPlayer(id);
        if (isDead) return;

        currentHealth -= pClass.Damage;

        if (currentHealth <= 0)
        {
            Die(pClass.name);
            pClass.RpcRiseKill();
        }
    }

    public void Die(string whosKill)
    {
        RpcRiseDead();
        isDead = true;
        ToggleRenderer(false);
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;
        StartCoroutine(Respown());
    }

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        setDefaults(true);
    }

    public void setDefaults(bool resetKillDead)
    {
        currentHealth = maxHP;
        isDead = false;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

        if (resetKillDead)
        {
            kill = 0;
            dead = 0;
        }
    }

    public string loginName
    {
        get { return playerLoginName; }
        set { playerLoginName = value; }
    }

    public int currentHealth
    {
        get { return currentHP; }
        set { currentHP = value; }
    }

    public int Kill
    {
        get { return kill; }
        set { kill = value; }
    }

    public int Dead
    {
        get { return dead; }
        set { dead = value; }
    }

    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    [ClientRpc]
    public void RpcRiseKill()
    {
        Kill += 1;
    }

    [ClientRpc]
    public void RpcRiseDead()
    {
        Dead += 1;
    }
}
