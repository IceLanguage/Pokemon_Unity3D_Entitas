using PokemonBattelePokemon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class PokemonFactory{

	public static GameObject InitPokemon(int PokemonID)
	{		
		string path = new StringBuilder("PokemonPrefab/").Append(PokemonID).ToString();
		GameObject prefab = Resources.Load<GameObject>(path);
		return UnityEngine.Object.Instantiate(prefab);
	}


	public static int GetPokemonFromEncounterPokemonScriptableObject(EncounterPokemon encounterPokemons)
	{
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
}
