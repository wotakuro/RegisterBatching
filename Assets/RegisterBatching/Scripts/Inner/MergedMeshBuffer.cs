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
using System;

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
        /// バッファーのカウント
        /// </summary>
        public int Count {
            get
            {
                return combineInstanceList.Count;
            }
        }

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
        /// 指定したIndexのメッシュ数を返します
        /// </summary>
        /// <param name="idx">Index指定</param>
        /// <returns>頂点を返します</returns>
        public int GetVertexCount(int idx)
        {
            return this.combineInstanceList[idx].mesh.vertexCount;
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

        /// <summary>
        /// 任意の関数でソートします
        /// </summary>
        /// <param name="sortFunction">ソート用の関数</param>
        public void Sort(Func<Vector3, Vector3,int> sortFunction)
        {
            combineInstanceList.Sort(
                (CombineInstance a, CombineInstance b) =>
                {
                    return sortFunction( a.transform.GetColumn(3),b.transform.GetColumn(3) );
                });
        }

        /// <summary>
        /// 指定した数だけメッシュにして返します
        /// </summary>
        /// <param name="start">開始番号</param>
        /// <param name="num">終了番号</param>
        /// <returns>合体したメッシュを返します</returns>
        public Mesh GetCombineMesh(int start,int num)
        {
            CombineInstance[] combineArray = new CombineInstance[num];
            for (int i = 0; i < num; ++i)
            {
                combineArray[i] = this.combineInstanceList[ start + i ];
            }
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combineArray, true, true);
            return mesh;
        }
    }
}