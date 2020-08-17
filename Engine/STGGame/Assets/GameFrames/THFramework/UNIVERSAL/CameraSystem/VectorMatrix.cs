using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class VectorMatrix
    {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale;

        public VectorMatrix()
        {

        }

        public void Save(Transform transform)
        {
            this.localPosition = transform.localPosition;
            this.localEulerAngles = transform.localEulerAngles;
            this.localScale = transform.localScale;
        }

        public void Load(Transform transform)
        {
            transform.localPosition = this.localPosition;
            transform.localEulerAngles = this.localEulerAngles;
            transform.localScale = this.localScale;
        }

        public void Clear()
        {
            this.localPosition = Vector3.zero;
            this.localEulerAngles = Vector3.zero;
            this.localScale = Vector3.one;
        }
    }

}
