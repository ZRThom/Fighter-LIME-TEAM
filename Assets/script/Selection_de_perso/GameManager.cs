using UnityEngine;
using System.Collections.Generic;



public class GameManagerSelect : MonoBehaviour
{   
    [System.Serializable]
    public class CharacterNameEntry
    {
        public GameObject characterPrefab;
        public string displayName;
    }
    public static GameManagerSelect Instance;

    [Header("Character Select")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject firstSelectedPrefab;
    public GameObject secondSelectedPrefab;

    [Header("Name selected")]
    public string firstSelectedName;
    public string secondSelectedName;

    [Header("character database")]
    public List<CharacterNameEntry> characterNames = new List<CharacterNameEntry>();

    [Header("Map Select")]
    public GameObject selectMapPrefab;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log($"Player1Prefab = {player1Prefab?.name}, Player2Prefab = {player2Prefab?.name}");
    }

    public void SelectPlayer(int playerNumber, GameObject prefab)
    {
        string characterName = GetCharacterDisplayName(prefab);
        if (playerNumber == 1)
        {
            firstSelectedPrefab = prefab;
            player1Prefab = prefab;
            firstSelectedName = characterName;
        }
        else if (playerNumber == 2)   
        {
            secondSelectedPrefab = prefab;
            player2Prefab = prefab;
            secondSelectedName = characterName;
        }
        Debug.Log($"Player {playerNumber} selected: {prefab.name} displayName : {characterName}");
    }

    public void SelectMap(GameObject mapPrefab)
    {
        selectMapPrefab = mapPrefab;
        Debug.Log($"Map selected : {mapPrefab.name}");
    }

    public string GetCharacterDisplayName(GameObject prefab)
    {
        if (prefab == null)
        {
            return "unknow";
        }

        foreach (var entry in characterNames)
        {
            if (entry.characterPrefab == prefab)
            {
                return entry.displayName;
            }
        }
        return prefab.name;
    }
}
