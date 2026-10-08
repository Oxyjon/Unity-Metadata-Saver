using System;
using System.Collections;

using UnityEngine;

namespace SaveLoad
{
	public class SaveableEntity : MonoBehaviour
	{
		//Save
		public delegate void PrepareToSaveEvent(MetaData metaData);
		public event PrepareToSaveEvent prepareToSave;
		
		//Load
		public delegate void LoadObjectStateEvent(MetaData metaData);
		public event LoadObjectStateEvent loadObjectState;

		public MetaData metaData;

		private void Start()
		{
			if(metaData is { recreate: true } && metaData.guid.Equals(metaData.prefabGuid))
			{
				metaData.guid = MetaData.CreateGuid();
			}
		}
		
		//Save
		public void PrepareToSave()
		{
			prepareToSave?.Invoke(metaData);
		}
		
		//Load
		public void Load(MetaData _metaData)
		{
			metaData = _metaData;
			StartCoroutine(LoadRoutine(_metaData));
		}
		
		private IEnumerator LoadRoutine(MetaData _metaData)
		{
			yield return new WaitForEndOfFrame();
			loadObjectState?.Invoke(_metaData);
		}
	}
}