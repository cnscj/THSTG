using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace XLibEditor
{
    /// <summary>
    /// UnityEditor下模拟协程
    /// </summary>
    public class EditorCoroutine
    {
        /// <summary>
        /// 迭代列表
        /// </summary>
        private static List<IEnumerator> _itors = null;

        /// <summary>
        /// 要从迭代列表中移除的索引
        /// </summary>
        private static List<int> _removeIdxs = null;

        private static EditorApplication.CallbackFunction _updateFunc = null;

        /// <summary>
        /// Update方法是否在执行
        /// </summary>
        private static bool _isUpdateRuning = false;

        static EditorCoroutine()
        {
            _itors = new List<IEnumerator>();
            _removeIdxs = new List<int>();
            _updateFunc = _Update;
        }

        public static void Start(IEnumerator itor)
        {
            _itors.Add(itor);
            _RunUpdate(true);
        }

        private static void _RunUpdate(bool isRun)
        {
            if (isRun == _isUpdateRuning) return;

            // 运行Update
            if (isRun)
            {
                EditorApplication.update += _updateFunc;
            }
            // 停止运行Update
            else
            {
                EditorApplication.update -= _updateFunc;
            }
            _isUpdateRuning = isRun;
        }

        private static void _Update()
        {
            if (null == _itors || _itors.Count <= 0) return;
            for (int i = 0, c = _itors.Count; i < c; ++i)
            {
                if (!_itors[i].MoveNext())
                {
                    _removeIdxs.Add(i);
                }
            }

            if (_removeIdxs.Count <= 0) return;
            for (int i = _removeIdxs.Count - 1; i >= 0; --i)
            {
                _itors.RemoveAt(_removeIdxs[i]);
                _removeIdxs.RemoveAt(i);
            }

            if (_itors.Count == 0)
            {
                _RunUpdate(false);
            }
        }
    }
}
