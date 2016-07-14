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
    public class MeshCreator
    {

        /// <summary>
        /// Node本体
        /// </summary>
        private struct DrawNode
        {
            public Mesh mesh;
            public Material material;
            public Matrix4x4 localMatrix;
        }

        /// <summary>
        /// Prefab情報
        /// </summary>
        private Dictionary<GameObject, List<DrawNode>> prefabInfo = new Dictionary<GameObject, List<DrawNode>>();
        /// <summary>
        /// Material別 Rendererの設定
        /// </summary>
        private Dictionary<Material, MergedMeshBuffer> rendererSet = new Dictionary<Material, MergedMeshBuffer>();


        /// <summary>
        /// 描画予定の登録
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        public void Add(GameObject prefab, Vector3 pos, Quaternion rotation)
        {
            var nodeList = this.GetNodeListInfo(prefab);
            if (nodeList == null) { return; }
            Matrix4x4 m = Matrix4x4.identity;
            foreach (var node in nodeList)
            {
                m = Matrix4x4.TRS(pos, rotation, Vector3.one);
                m *= node.localMatrix;
                var buffer = this.GetBuffer(node.material);
                buffer.Add(node.mesh, ref m);
            }
        }

        /// <summary>
        /// メッシュ用のバッファーを返します
        /// </summary>
        /// <param name="material">Materialの指定</param>
        /// <returns>バッファーを返します</returns>
        private MergedMeshBuffer GetBuffer(Material material)
        {
            MergedMeshBuffer buffer = null;
            if (!this.rendererSet.TryGetValue(material, out buffer))
            {
                buffer = new MergedMeshBuffer();
                this.rendererSet.Add(material, buffer);
            }
            return buffer;
        }

        /// <summary>
        /// Node情報一覧を取得します
        /// </summary>
        /// <param name="prefab">Prefabの指定</param>
        /// <returns>Node情報を返します</returns>
        private List<DrawNode> GetNodeListInfo(GameObject prefab)
        {
            if (this.prefabInfo == null) { this.prefabInfo = new Dictionary<GameObject, List<DrawNode>>(); }
            List<DrawNode> list = null;
            if (!this.prefabInfo.TryGetValue(prefab, out list))
            {
                list = this.ConstructList(prefab);
                this.prefabInfo.Add(prefab, list);
            }
            return list;
        }

        /// <summary>
        /// Prefabから描画用のオブジェクトを生成します
        /// </summary>
        /// <param name="prefab"> prefabの指定</param>
        /// <returns></returns>
        private List<DrawNode> ConstructList(GameObject prefab)
        {
            var rList = prefab.GetComponentsInChildren<MeshRenderer>();

            if (rList == null) { return null; }
            List<DrawNode> result = new List<DrawNode>();
            foreach (var r in rList)
            {
                var filter = r.gameObject.GetComponent<MeshFilter>();
                if (filter == null) { continue; }
                DrawNode node;
                node.localMatrix = filter.transform.localToWorldMatrix;
                node.mesh = filter.sharedMesh;
                node.material = r.sharedMaterial;
                result.Add(node);
            }
            return result;
        }

        /// <summary>
        /// Generate処理を行います
        /// </summary>
        /// <param name="p">親のTransformを指定</param>
        public void Generate(Transform p)
        {
            if (this.rendererSet != null)
            {
                foreach (var kv in this.rendererSet)
                {
                    if (kv.Key == null || kv.Value == null) { continue; }
                    var obj = new GameObject(kv.Key.name);
                    obj.transform.parent = p;
                    obj.transform.position = Vector3.zero;
                    obj.transform.rotation = Quaternion.identity;

                    var filter = obj.AddComponent<MeshFilter>();
                    var renderer = obj.AddComponent<MeshRenderer>();

                    renderer.material = kv.Key;
                    filter.mesh = kv.Value.GetCombineMesh();
                }
                this.rendererSet = null;
                this.prefabInfo = null;
            }
        }
    }
}