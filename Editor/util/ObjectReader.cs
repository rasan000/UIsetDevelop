
using System;
using System.IO;
using UnityEngine;

namespace UIset.util
{

    /// <summary>
    /// read object and return result
    /// </summary>
    class ObjectReader
    {
        // <summary>
        /// search mesh renderer in children
        /// </summary>
        /// <param name="parent">transform parent</param>
        /// <param name="meshName">Mesh name</param>
        /// <returns></returns>
        public MeshRenderer FindMeshRendererInChildren(Transform parent, string meshName)
        {
            MeshRenderer result = null;
            foreach (Transform child in parent)
            {
                if (child.name == meshName)
                {
                    result = child.GetComponent<MeshRenderer>();
                    if (result != null)
                    {
                        return result;
                    }
                }

                result = FindMeshRendererInChildren(child, meshName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }


        /// <summary>
        /// オブジェクトを名前で検索します。
        /// 指定したオブジェクトの子オブジェクト、孫オブジェクトを含めて検索します。
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject FindGameObjectByName(GameObject parent, string name)
        {
            Transform parentTransform = parent.transform;

            // 子オブジェクトの中から名前が一致するオブジェクトを検索します。
            foreach (Transform childTransform in parentTransform)
            {
                if (childTransform.name == name)
                {
                    return childTransform.gameObject;
                }

                // 孫オブジェクトも含めて検索します。
                GameObject found = FindGameObjectByName(childTransform.gameObject, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

    }
}