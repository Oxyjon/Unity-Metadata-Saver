using System;
using System.Collections.Generic;

using UnityEngine;

namespace SaveLoad
{
	[Serializable]
	public class MetaData
	{
		public string guid;
		public string objectName;
		public float[] position;
		public float[] rotation;
		public float[] scale;
		public bool recreate;
		public string prefabGuid;
		public string[] childrenGuids;
		public Dictionary<string, object> baseValues;


		public MetaData()
		{
			if(string.IsNullOrEmpty(guid))
			{
				guid = CreateGuid();
			}

			recreate = false;
			prefabGuid = guid;
			baseValues = new Dictionary<string, object>();
		}

		private void PrepareToSave(GameObject gameObject)
		{
			if(recreate)
			{
				if(string.IsNullOrEmpty(prefabGuid))
				{
					throw new InvalidOperationException("Prefab guid is empty");
				}

				if(prefabGuid == guid)
				{
					throw new InvalidOperationException("Prefab guid are the same");
				}
			}

			objectName = gameObject.name;
			position = SaveLoadManager.ConvertFromVector3(gameObject.transform.position);
			rotation = SaveLoadManager.ConvertFromVector3(gameObject.transform.rotation.eulerAngles);
			scale = SaveLoadManager.ConvertFromVector3(gameObject.transform.localScale);
			gameObject.GetComponent<SaveableEntity>()?.PrepareToSave();

			List<string> childrenGuidsList = new List<string>();
			foreach(Transform childTransform in gameObject.transform)
			{
				SaveableEntity saveableObject = childTransform.GetComponent<SaveableEntity>();
				if(saveableObject == null)
				{
					continue;
				}

				saveableObject.metaData.PrepareToSave(childTransform.gameObject);
				childrenGuidsList.Add(saveableObject.metaData.guid);
			}

			childrenGuids = childrenGuidsList.ToArray();
		}

		public List<MetaData> Save(GameObject gameObject)
		{
			PrepareToSave(gameObject);
			List<MetaData> savedObjects = new List<MetaData>();

			foreach(Transform childTransform in gameObject.transform)
			{
				SaveableEntity saveableObject = childTransform.GetComponent<SaveableEntity>();
				if(saveableObject == null)
				{
					continue;
				}

				savedObjects.AddRange(saveableObject.metaData.Save(childTransform.gameObject));
			}

			savedObjects.Add(this);

			return savedObjects;
		}

		public static List<MetaData> SaveObject()
		{
			List<MetaData> metaData = new List<MetaData>();

			foreach(SaveableEntity saveableObject in GameObject.FindObjectsOfType<SaveableEntity>())
			{
				metaData.AddRange(saveableObject.metaData.Save(saveableObject.gameObject));
			}

			return metaData;
		}

		public static void LoadObject(Dictionary<string, GameObject> prefabs, List<MetaData> metadata)
		{
			ClearObjects();

			Dictionary<string, GameObject> createdObjects = new Dictionary<string, GameObject>();

			foreach(MetaData data in metadata)
			{
				GameObject createdObject;
				SaveableEntity saveableEntity;


				if(data.recreate)
				{
					if(!prefabs.ContainsKey(data.prefabGuid))
					{
						throw new InvalidOperationException("Prefab with guid " + data.prefabGuid + " not found.");
					}

					createdObject = UnityEngine.Object.Instantiate(prefabs[data.prefabGuid]);
					saveableEntity = createdObject.GetComponent<SaveableEntity>();


					saveableEntity.Load(data);

					foreach(string childGuid in data.childrenGuids)
					{
						if(!createdObjects.ContainsKey(childGuid))
						{
							Debug.Log("Cannot find child with guid " + childGuid);
							continue;
						}

						createdObjects[childGuid].transform.SetParent(createdObject.transform);
					}

					createdObject.name = data.objectName;
					Vector3 position = SaveLoadManager.ConvertToVector3(data.position);
					createdObject.transform.position = new Vector3(position.x, position.y, position.z);
					Vector3 rotation = SaveLoadManager.ConvertToVector3(data.rotation);
					createdObject.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
					Vector3 scale = SaveLoadManager.ConvertToVector3(data.scale);
					createdObject.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
					createdObjects.Add(data.guid, createdObject);
				}
				else
				{
					saveableEntity = SaveLoadManager.Instance.FindSaveableEntityByGuid(data.guid);
					saveableEntity.Load(data);
					
					foreach(string childGuid in data.childrenGuids)
					{
						if(!createdObjects.ContainsKey(childGuid))
						{
							Debug.Log("Cannot find child with guid " + childGuid);
							continue;
						}

						createdObjects[childGuid].transform.SetParent(saveableEntity.transform);
					}
					
					Vector3 position = SaveLoadManager.ConvertToVector3(data.position);
					saveableEntity.transform.localPosition = new Vector3(position.x, position.y, position.z);
					Vector3 rotation = SaveLoadManager.ConvertToVector3(data.rotation);
					saveableEntity.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
					Vector3 scale = SaveLoadManager.ConvertToVector3(data.scale);
					saveableEntity.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
				}
			}
		}


		private static void ClearObjects()
		{
			foreach(SaveableEntity saveableEntity in GameObject.FindObjectsOfType<SaveableEntity>())
			{
				if(saveableEntity.metaData.recreate)
				{
					UnityEngine.Object.Destroy(saveableEntity.gameObject);
				}
			}
		}

		public static string CreateGuid()
		{
			return Guid.NewGuid().ToString();
		}
	}
}