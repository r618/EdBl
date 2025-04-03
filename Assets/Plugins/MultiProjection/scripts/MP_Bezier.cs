/// <summary>
/// Create by vvvision ,got help from 
/// http://vvvision.net/zblog/post/MultiProjectionUnity.html
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

 
    public class MP_Bezier : System.Object

    {

        public Vector3 p0;

        public Vector3 p1;

        public Vector3 p2;

        public Vector3 p3;

        public float ti = 0f;

        private Vector3 b0 = Vector3.zero;

        private Vector3 b1 = Vector3.zero;

        private Vector3 b2 = Vector3.zero;

        private Vector3 b3 = Vector3.zero;

        private float Ax;

        private float Ay;

        private float Az;

        private float Bx;

        private float By;

        private float Bz;

        private float Cx;

        private float Cy;

        private float Cz;



        public class BDATA
        {
            public Vector3 pos;
            public Vector3 dir;
            public Vector3 tang;
        };

        List<BDATA> vecData = new List<BDATA>();
        public int max_size = 100;
        public MP_Bezier(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, int cmax_size)
        {
            max_size = cmax_size;
            BuildBezier(v0, v1, v2, v3);
        }

        void BuildBezier(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {

            this.p0 = v0;

            this.p1 = v1;

            this.p2 = v2;

            this.p3 = v3;

            this.CheckConstant();

            for (int i = 0; i < max_size; i++)
            {
                BDATA d = new BDATA();
                float s = i * 1.0f / max_size;
                d.pos = GetPoint(s);
                vecData.Add(d);
            }

            for (int i = 0; i < vecData.Count; i++)
            {
                if (i == vecData.Count - 1)
                {
                    vecData[i].dir = vecData[i - 1].dir;
                    vecData[i].tang = vecData[i - 1].tang;
                }
                else
                {
                    Vector3 diff_pt = vecData[i].pos - vecData[i + 1].pos;
                    vecData[i].dir = diff_pt.normalized;

                    Vector3 tangent = Vector3.Cross(diff_pt, Vector3.up);
                    if (tangent.magnitude == 0) tangent = Vector3.Cross(diff_pt, Vector3.forward);
                    vecData[i].tang = tangent.normalized;
                }

            }
        }
        public MP_Bezier(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {

            BuildBezier(v0, v1, v2, v3);
        }

        private int Getindex(float s)
        {
            int i = (int)(s * vecData.Count);
            if (i < 0) i = 0;
            if (i > (vecData.Count - 1)) i = vecData.Count - 1;
            return i;
        }
        public Vector3 GetPos(float s)
        {
            int i = Getindex(s);
            return vecData[i].pos;
        }
        public Vector3 GetTang(float s)
        {
            int i = Getindex(s);
            return vecData[i].tang;
        }
        public Vector3 GetDir(float s)
        {
            int i = Getindex(s);
            return vecData[i].dir;
        }

        private Vector3 GetPoint(float t)
        {

            float t2 = t * t;

            float t3 = t * t * t;

            float x = this.Ax * t3 + this.Bx * t2 + this.Cx * t + p0.x;

            float y = this.Ay * t3 + this.By * t2 + this.Cy * t + p0.y;

            float z = this.Az * t3 + this.Bz * t2 + this.Cz * t + p0.z;

            return new Vector3(x, y, z);

        }



        private void SetConstant()

        {

            this.Cx = 3f * ((this.p0.x + this.p1.x) - this.p0.x);

            this.Bx = 3f * ((this.p3.x + this.p2.x) - (this.p0.x + this.p1.x)) - this.Cx;

            this.Ax = this.p3.x - this.p0.x - this.Cx - this.Bx;

            this.Cy = 3f * ((this.p0.y + this.p1.y) - this.p0.y);

            this.By = 3f * ((this.p3.y + this.p2.y) - (this.p0.y + this.p1.y)) - this.Cy;

            this.Ay = this.p3.y - this.p0.y - this.Cy - this.By;

            this.Cz = 3f * ((this.p0.z + this.p1.z) - this.p0.z);

            this.Bz = 3f * ((this.p3.z + this.p2.z) - (this.p0.z + this.p1.z)) - this.Cz;

            this.Az = this.p3.z - this.p0.z - this.Cz - this.Bz;

        }

        // Check if p0, p1, p2 or p3 have changed

        private void CheckConstant()

        {

            if (this.p0 != this.b0 || this.p1 != this.b1 || this.p2 != this.b2 || this.p3 != this.b3)

            {

                this.SetConstant();

                this.b0 = this.p0;

                this.b1 = this.p1;

                this.b2 = this.p2;

                this.b3 = this.p3;

            }

        }

    }