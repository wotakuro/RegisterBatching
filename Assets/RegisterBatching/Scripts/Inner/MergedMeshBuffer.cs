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
    /// <summary>
    /// 登録したメッシュデータをマージして返すバッファ領域
    /// </summary>
    class MergedMeshBuffer
    {
        /// <summary>
        /// 登録した一覧
        /// </summary>
        private List<CombineInstance> combineInstanceList = new List<CombineInstance>();

        /// <summary>
        /// データを追加します
        /// </summary>
        /// <param name="meshData">Mesh指定</param>
        /// <param name="pos">座標</param>
        /// <param name="r">回転</param>
        /// <param name="size">サイズ</param>
        public void Add(Mesh meshData, Vector3 pos, Quaternion r, Vector3 size)
        {
            Matrix4x4 m = Matrix4x4.identity;
            m.SetTRS(pos, r, size);
            this.Add(meshData, ref m);
        }

        /// <summary>
        /// データ追加
        /// </summary>
        /// <param name="meshData">Mesh指定</param>
        /// <param name="m">Matrix指定</param>
        public void Add(Mesh meshData, ref Matrix4x4 m)
        {
            var inst = new CombineInstance();
            inst.mesh = meshData;
            inst.transform = m;
            this.combineInstanceList.Add(inst);
        }

        /// <summary>
        /// 合体したメッシュを作成して返します
        /// </summary>
        /// <returns>合体したメッシュを返します</returns>
        public Mesh GetCombineMesh()
        {
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(this.combineInstanceList.ToArray(), true, true);
            return mesh;
        }
    }
}