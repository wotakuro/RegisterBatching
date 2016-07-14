# RegisterBatching
Unity上で BG（背景）用のオブジェクトを配置する際に、同じオブジェクトを沢山描画する場合でも、GameObjectが大量に増えてしまうケースがあるかと思います。
また、UnityEditor上で配置せず、csv/json、ランタイム生成等を行った場合は staticバッチングが効かずにもどかしい感じがするかと思います。

このプロジェクトでは、同じオブジェクトを沢山配置する際に事前に一つのMeshに固めてしまってGameObjectの描画ならびに描画負荷を減らしてしまおうというものです。

同じPrefabを描画する際に以下のような形で書くことで、何かいい感じに一つのMeshに対応するようです

     MeshCreator creator = new MeshCreator();
     for (int i = 0; i < 40; ++i)
     {
     Vector3 pos = new Vector3(Random.value * 20, 0.0f, Random.value * 30);
     Quaternion rot = Quaternion.AngleAxis(Random.value * 360, Vector3.up);
     creator.Add(prefab, pos, rot);
    }
    creator.Generate(this.transform);

今後追加予定：障害物として配置するためにCollider配置にも対応予定。出来る限り少ないGameObject数で配置できるようにする予定。
　回転等を考慮する必要のない球体だと有利になるように組む予定
