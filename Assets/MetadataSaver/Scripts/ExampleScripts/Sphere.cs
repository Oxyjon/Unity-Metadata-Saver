using SaveLoad;

using System;

using UnityEngine;

namespace ExampleScripts
{
	public class Sphere : MonoBehaviour, IJSaveable
	{
		//wasd movement
		public float speed = 10f;

		private SaveableEntity saveableEntity;

		private void Start()
		{
			saveableEntity = GetComponent<SaveableEntity>();

			saveableEntity.prepareToSave += PrepareToSaveObjectState;
			saveableEntity.loadObjectState += LoadObjectState;
		}

		private void Update()
		{
			//get input
			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");

			//move
			transform.Translate(x * speed * Time.deltaTime, 0, z * speed * Time.deltaTime);


		}

		public void PrepareToSaveObjectState(MetaData metaData)
		{
			metaData.baseValues["Sphere.speed"] = speed;
			
		}

		public void LoadObjectState(MetaData metaData)
		{
			speed = Convert.ToInt32(metaData.baseValues["Sphere.speed"]);
		}
	}
}