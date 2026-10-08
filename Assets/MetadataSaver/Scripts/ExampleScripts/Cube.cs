using SaveLoad;

using System;

using UnityEngine;

using Random = UnityEngine.Random;


namespace ExampleScripts
{
	public class Cube : MonoBehaviour, IJSaveable
	{
		public int id;

		[SerializeField] private MaterialData[] materialDatas;

		private Renderer renderer;


		private SaveableEntity saveableEntity;

		private void Start()
		{
			id = Random.Range(0, 100);

			renderer = GetComponent<Renderer>();

			saveableEntity = GetComponent<SaveableEntity>();

			saveableEntity.prepareToSave += PrepareToSaveObjectState;
			saveableEntity.loadObjectState += LoadObjectState;
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				ChangeMaterial();
			}
		}

		private void ChangeMaterial()
		{
			renderer.material = materialDatas[Random.Range(0, materialDatas.Length)].material;
		}

		public void PrepareToSaveObjectState(MetaData metaData)
		{
			metaData.baseValues["Cube.id"] = id;
			metaData.baseValues["Cube.material"] = renderer.material.name.Replace(" (Instance)", "");
		}

		public void LoadObjectState(MetaData metaData)
		{
			id = Convert.ToInt32(metaData.baseValues["Cube.id"]);

			if(metaData.baseValues.ContainsKey("Cube.material"))
			{
				string materialName = Convert.ToString(metaData.baseValues["Cube.material"]);
				MaterialData materialdata = SaveLoadManager.Instance.GetScriptableByName(materialName) as MaterialData;
				renderer.material = materialdata.material;
			}
		}
	}
}