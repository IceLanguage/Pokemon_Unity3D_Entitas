using PokemonBattele;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class PokemonFactory{

	public static GameObject InitPokemon(int PokemonID)
	{		
		string path = new StringBuilder("PokemonPrefab/").Append(PokemonID).ToString();
		GameObject objectinPool = ObjectPoolController.PokemonObjectsPool[PokemonID];
		if(null == objectinPool||objectinPool.activeSelf)
		{
			GameObject prefab = Resources.Load<GameObject>(path);
			return UnityEngine.Object.Instantiate(prefab);
		}
		objectinPool.SetActive(true);
		
		return objectinPool;

	}

	public static GameObject GetPokemonBall()
	{
		GameObject pokemonBallInPool = ObjectPoolController.PokemonBallObjectsPool.New();
		if (null == pokemonBallInPool || pokemonBallInPool.activeSelf)
		{
			GameObject prefab = Resources.Load<GameObject>("PokemonPrefab/PokemonBall");
			pokemonBallInPool = UnityEngine.Object.Instantiate(prefab, BattleController.Instance.transform);
		}
		else
		{
			pokemonBallInPool.SetActive(true);
		}
		LHCoroutine.CoroutineManager.DoCoroutine(IEStorePokemonBallInPool(pokemonBallInPool));
		return pokemonBallInPool;
	}

	private static int GetPokemonFromEncounterPokemonScriptableObject(EncounterPokemon encounter)
	{
		EncounterPokemon encounterPokemons;
		//考虑特性
		Pokemon pokemon = Contexts.sharedInstance.game.playerData.scriptableObject.pokemons[0];
		BattlePokemonData pokemonData = BattlePokemonData.Context[pokemon.GetInstanceID()];
		if (AbilityManager.AbilityImpacts.ContainsKey(pokemonData.ShowAbility))
			encounterPokemons = AbilityManager.AbilityImpacts[pokemonData.ShowAbility]
				.OnEncounterPokemon(pokemonData, encounter);
		else
		{
			encounterPokemons = encounter;

		}

		int count = Math.Min(encounterPokemons.Pokemoms.Count, encounterPokemons.EncounterProbabilities.Count);
		int ProbabilitySum = 0;
		for (int i = 0; i < count; ++i)
			ProbabilitySum += encounterPokemons.EncounterProbabilities[i];
		int cur = 0;
		int index = 0;
		int Probability = RandomService.game.Int(0, ProbabilitySum);

		while (index < count && cur <= Probability)
		{
			cur += encounterPokemons.EncounterProbabilities[index];
			index++;
		}
		if (index >= count)
			return encounterPokemons.Pokemoms[count - 1];
		return encounterPokemons.Pokemoms[index - 1];
	}

	public static Pokemon BuildPokemon(int PokemonID)
	{
		Pokemon pokemom = ScriptableObject.CreateInstance<Pokemon>();
		pokemom.InitPokemon(PokemonID);
		new BattlePokemonData(pokemom);
		return pokemom;
	}
	public static Pokemon BuildPokemon(EncounterPokemon encounter)
	{
		int PokemonID = PokemonFactory.GetPokemonFromEncounterPokemonScriptableObject(encounter);
		Pokemon pokemon = PokemonFactory.BuildPokemon(PokemonID);
		//考虑特性
		Pokemon FirstPokemon = Contexts.sharedInstance.game.playerData.scriptableObject.pokemons[0];
		BattlePokemonData pokemonData = BattlePokemonData.Context[FirstPokemon.GetInstanceID()];
		if (AbilityManager.AbilityImpacts.ContainsKey(pokemonData.ShowAbility))
		    AbilityManager.AbilityImpacts[pokemonData.ShowAbility]
				.OnEncounterPokemon(pokemonData, pokemon);
		return pokemon;
	}

	/// <summary>
	/// 使用精灵球和召唤精灵时的特效
	/// </summary>
	public static void PokemonBallEffect(Vector3 pos)
	{
		if (ResourceController.Instance.PokemonShowParticle != null)
		{


			var par = ResourceController.Instance.PokemonShowParticle;
			par.transform.position = pos;

			//摄像头对准特效
			//CameraController.Instance.SetTarget(par.transform);

			par.Play();
		}
	}

	private static IEnumerator IEStorePokemonBallInPool(GameObject pokemonBall)
	{
		yield return new WaitWhile
		(
			()=>
				{
					return Contexts.sharedInstance.game.isBattleFlag;
				} 
		);
		StorePokemonBallInPool(pokemonBall);


	}

	public static void StorePokemonBallInPool(GameObject pokemonBall)
	{
		if (pokemonBall.activeSelf)
		{
			pokemonBall.SetActive(false);
			ObjectPoolController.PokemonBallObjectsPool.Store(pokemonBall);
		}
	}
}
