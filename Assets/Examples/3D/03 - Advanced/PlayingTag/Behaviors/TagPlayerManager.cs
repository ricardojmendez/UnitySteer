using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TagPlayerManager : MonoBehaviour 
{
	public static TagPlayerManager Instance
	{
		get { return SingletonPerLevel<TagPlayerManager>.Instance; }
	}


	[SerializeField]
	TagPlayer _prefab;

	[SerializeField]
	int _playersToInitialize = 30;

	[SerializeField]
	float _pauseBetweenGames = 5f;

	[SerializeField]
	float _gameTime = 25f;

	[SerializeField]
	float _preyHeadStart = 2f;


	public float NextGameStart { get; private set; }
	public float NextGameEnd { get; private set; }

	public bool IsGameTime 
	{
		get { return Time.time >= NextGameStart && Time.time < NextGameEnd; }
	}
	public bool IsGamePlaying { get; private set; }

	public List<TagPlayer> Players = new List<TagPlayer>();


	public void CapturedPrey(TagPlayer player)
	{
		StopGame();
		Debug.Log(string.Format("{0} {1} captured prey!", Time.time, player));
		player.Grow();
		var quarry = player.ForPursuit.Quarry.GetComponent<TagPlayer>();
		quarry.Die();
		Destroy(player.ForPursuit.Quarry.gameObject);
	}


	void Start() 
	{
		SetUpNextGameTime();
		for (int i = 0; i < _playersToInitialize; i++)
		{
			Instantiate(_prefab);
		}
	}
		
	void Update() 
	{
		if (!IsGamePlaying && IsGameTime)
		{
			StartGame();
		}
		else if (IsGamePlaying && !IsGameTime)
		{
			StopGame();
		}
	}

	void SetUpNextGameTime() 
	{
		NextGameStart = Time.time + _pauseBetweenGames;
		NextGameEnd = NextGameStart + _gameTime;
		IsGamePlaying = false;
	}

	void StartGame()
	{
		Debug.Log(string.Format("{0} Starting game", Time.time));
		StartCoroutine(StartGameEnumerator());
	}

	IEnumerator StartGameEnumerator()
	{
		IsGamePlaying = true;
		var prey = Players.OrderBy(x => Random.value).First();
		prey.State = TagPlayer.PlayerState.Prey;
		prey.name = "Prey";
		yield return new WaitForSeconds(_preyHeadStart);
		foreach(var attacker in Players.Where(x => x != prey && IsGamePlaying))
		{
			attacker.State = TagPlayer.PlayerState.Pursuer;
			attacker.ForPursuit.Quarry = prey.Vehicle;
			attacker.name = string.Format("Attacker [{0}]", attacker.GetInstanceID());
			yield return null;
			if (!IsGamePlaying)
			{
				yield break;
			}
		}
	}


	void StopGame()
	{
		Debug.Log(string.Format("{0} Stopping game", Time.time));
		foreach(var player in Players)
		{
			player.State = TagPlayer.PlayerState.Neutral;
		}
		SetUpNextGameTime();
	}
}
