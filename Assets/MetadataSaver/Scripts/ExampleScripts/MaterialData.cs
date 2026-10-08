using UnityEngine;

namespace ExampleScripts
{
	[CreateAssetMenu(fileName = "ExampleMaterial", menuName = "Cube/Material", order = 0)]
	public class MaterialData : ScriptableObject
	{
		public Material material;
	}
}