using UnityEngine;
using System.Collections;
namespace XLua.Cast
{

    public class IEnumerator : XLua.Cast.Any<System.Collections.IEnumerator>
    {
        public IEnumerator(System.Collections.IEnumerator i) : base(i)
        {
        }
    }
}


