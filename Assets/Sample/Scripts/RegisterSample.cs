using UnityEngine;
using System.Collections;

using RegisterBatching;

public class RegisterSample : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
        MeshCreator creator = new MeshCreator();

        for (int i = 0; i < 10; ++i)
        {
            Vector3 pos = new Vector3( i * 4.0f, 0.0f, 10.0f );
            Quaternion rot = Quaternion.AngleAxis(Random.value * 360, Vector3.up);
            creator.Add(prefab, pos, rot,Vector3.one + Vector3.up * i * 0.2f);
        }
        creator.Generate(this.transform, 150, SortFunc);
	}

    private static int SortFunc(Vector3 a, Vector3 b)
    {
        if (a.y < b.y) { return 1; }
        if (a.y > b.y) { return -1; }
        if (a.x < b.x) { return 1; }
        if (a.x > b.x) { return -1; }
        return 0;
    }
}
