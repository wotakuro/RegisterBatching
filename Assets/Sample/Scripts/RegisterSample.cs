using UnityEngine;
using System.Collections;

using RegisterBatching;

public class RegisterSample : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
        MeshCreator creator = new MeshCreator();

        for (int i = 0; i < 40; ++i)
        {
            Vector3 pos = new Vector3(Random.value * 20, 0.0f, Random.value * 30);
            Quaternion rot = Quaternion.AngleAxis(Random.value * 360, Vector3.up);
            creator.Add(prefab, pos, rot);
        }
        creator.Generate(this.transform);
	}
	
}
