using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class DeckBuildingController : MonoBehaviour
{
	public GameObject collectionPrefab;
	public GameObject collectionGrid;
	public Text deckListText;
	
	DeckListData data = new DeckListData();
	CardData[] allCards;

	string filename = "Decklist";

	void Start()
    {
		allCards = Resources.LoadAll("Cards", typeof(CardData)).Cast<CardData>().ToArray();

		populateDeckCollection();
		updateDeckList();
	}

	void populateDeckCollection()
	{
		foreach (CardData card in allCards)
		{
			GameObject collectionObject = Instantiate(collectionPrefab, new Vector3(0, 0, 0), Quaternion.identity);
			CollectionCardController collectionController = collectionObject.GetComponent<CollectionCardController>();

			collectionObject.transform.SetParent(collectionGrid.transform);
			collectionController.Data = card;
			collectionController.RegisterCountChanged((card) => updateDeckList());
		}
	}

	void updateDeckList()
    {
		deckListText.text = "";
		List<CardData> cardsList = new List<CardData>();

		foreach(Transform child in collectionGrid.transform)
        {
			CollectionCardController collectionCardController = child.GetComponent<CollectionCardController>();

			if(collectionCardController != null && collectionCardController.Count != 0)
            {
				string cardDescription = collectionCardController.Count + "/4 " + collectionCardController.Data.CardTitle + "\n";
				deckListText.text += cardDescription;

				for(int i = 0; i < collectionCardController.Count; i++)
                {
					cardsList.Add(collectionCardController.Data);
                }
            }
        }

		data.Cards = cardsList;
    }

	string GetFilename()
    {
		return Application.persistentDataPath + "/" + filename + ".json";
	}

	public void OnFilenameChange(string newFilename)
    {
		filename = newFilename;
    }

	public void OnGameStart()
    {
		PlayerChoices.DeckList = data;
		SceneManager.LoadSceneAsync("Level");
    }

    public void OnDeckSave()
	{
		string dataString = JsonUtility.ToJson(data);
		System.IO.File.WriteAllText(GetFilename(), dataString);
	}

	public void OnDeckLoad()
    {
		if (File.Exists(GetFilename()))
		{
			string jsonFile = System.IO.File.ReadAllText(GetFilename());
			data = JsonUtility.FromJson<DeckListData>(jsonFile);

			foreach (Transform child in collectionGrid.transform)
			{
				CollectionCardController collectionCardController = child.GetComponent<CollectionCardController>();

				if (collectionCardController != null)
					if (data.CardsCount.ContainsKey(collectionCardController.Data))
						collectionCardController.Count = data.CardsCount[collectionCardController.Data];
					else
						collectionCardController.Count = 0;
			}
		}

		updateDeckList();
	}
}
