/*
The MIT License (MIT)

Copyright (c) 2016 Yusuke Kurokawa

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RegisterBatching
{
    public class ColliderCreator
    {
        /// <summary>
        /// Node本体
        /// </summary>
        private struct CollisionNode
        {
            public Collider collider;
            public Vector3 pos;
            public Vector3 scale;
            public Quaternion rotation;
        }

        /// <summary>
        /// prefabをどう配置したのかを覚えておきます
        /// </summary>
        private struct PrefabPositioningInfo
        {
            public GameObject prefab;
            public Vector3 pos;
            public Quaternion rotation;
            public Vector3 size;
        }

        /// <summary>
        /// Prefab情報
        /// </summary>
        private Dictionary<GameObject, List<CollisionNode>> prefabInfo = new Dictionary<GameObject, List<CollisionNode>>();

        /// <summary>
        /// prefabの配置情報
        /// </summary>
        private List<PrefabPositioningInfo> prefabPositionList = new List<PrefabPositioningInfo>();

        /// <summary>
        /// コリジョンの登録
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        public void Add(GameObject prefab, Vector3 pos, Quaternion rotation,Vector3 size)
        {

            PrefabPositioningInfo positioningInfo = new PrefabPositioningInfo();
            positioningInfo.prefab = prefab;
            positioningInfo.pos = pos;
            positioningInfo.rotation = rotation;
            positioningInfo.size = size;
            prefabPositionList.Add(positioningInfo);
        }

        /// <summary>
        /// Node情報一覧を取得します
        /// </summary>
        /// <param name="prefab">Prefabの指定</param>
        /// <returns>Node情報を返します</returns>
        private List<CollisionNode> GetNodeListInfo(GameObject prefab)
        {
            if (this.prefabInfo == null) { this.prefabInfo = new Dictionary<GameObject, List<CollisionNode>>(); }
            List<CollisionNode> list = null;
            if (!this.prefabInfo.TryGetValue(prefab, out list))
            {
                list = this.ConstructList(prefab);
                this.prefabInfo.Add(prefab, list);
            }
            return list;
        }

        /// <summary>
        /// PrefabからColliderのオブジェクトを生成します
        /// </summary>
        /// <param name="prefab"> prefabの指定</param>
        /// <returns></returns>
        private List<CollisionNode> ConstructList(GameObject prefab)
        {
            var cList = prefab.GetComponentsInChildren<Collider>();

            if (cList == null) { return null; }
            List<CollisionNode> result = new List<CollisionNode>();
            foreach (var c in cList)
            {
                CollisionNode node;
                node.collider = c;
                node.pos = c.transform.position;
                node.scale = c.transform.lossyScale;
                node.rotation = c.transform.rotation;
                result.Add(node);
            }
            return result;
        }

        /// <summary>
        /// Generate処理を行います
        /// </summary>
        /// <param name="p"></param>
        public void Generate(Transform p)
        {
            GameObject obj = new GameObject("identity");
            obj.transform.parent = p;
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;

            foreach (var positioning in this.prefabPositionList)
            {
                var prefabColliderInfo = this.GetNodeListInfo(positioning.prefab);
                if (prefabColliderInfo == null) { continue; }

                foreach (var info in prefabColliderInfo)
                {
                    if (info.collider.isTrigger) { continue; }
                    // sphere collider
                    if (info.collider.GetType() == typeof(SphereCollider))
                    {
                        var origin = info.collider as SphereCollider;
                        var sph = obj.AddComponent<SphereCollider>();
                        sph.radius = origin.radius * GetMaxColumn( Vector3.Scale( info.scale , positioning.size) );
                        sph.center = positioning.pos + positioning.rotation * Vector3.Scale(info.pos, positioning.size) +
                             positioning.rotation * info.rotation * origin.center;
                    }
                    else if (info.collider.GetType() == typeof(BoxCollider))
                    {

                    }
                    else if (info.collider.GetType() == typeof(CapsuleCollider))
                    {

                    }
                }
            }
        }

        private float GetMaxColumn(Vector3 vec)
        {
            return Mathf.Max(vec.x, vec.y, vec.z);
        }

    }
}