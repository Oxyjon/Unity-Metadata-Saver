using UnityEngine;

namespace ExampleScripts
{
	public class CubeSpawner : MonoBehaviour
	{
		[SerializeField] GameObject cubePrefab;
		[SerializeField] private Transform spawnPlane;

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				SpawnCube();
			}
		}
		
		private void SpawnCube()
		{
			//get the bounds of the plane
			Bounds bounds = spawnPlane.GetComponent<Renderer>().bounds;
			
			//get a random position within the bounds
			Vector3 randomPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
			
			//instantiate the cube at the random position
			Instantiate(cubePrefab, randomPos, Quaternion.identity);
		}
	}
}