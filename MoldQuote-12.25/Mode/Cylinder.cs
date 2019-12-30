using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using CycBasic;
using NXOpen.BlockStyler;
using System.Collections;

namespace MoldQuote
{
    /// <summary>
    /// 圆柱体
    /// </summary>
    public class Cylinder : IDisplayObject, IEquatable<Cylinder>
    {
        /// <summary>
        /// 圆柱面
        /// </summary>
        public List<CircleFaceStep> CylinderFaceList { get; set; } = new List<CircleFaceStep>();
        /// <summary>
        /// Z 最大面
        /// </summary>
        public CycFaceData FaceOfMaxZ { get; set; }
        /// <summary>
        /// Z 最小面
        /// </summary>
        public CycFaceData FaceOfMinZ { get; set; }
        /// <summary>
        /// 最大半径
        /// </summary>
        public double MaxDis { get; set; }
        /// <summary>
        /// 最小半径
        /// </summary>
        public double MinDis { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public double CylinderHigth { get; set; }
        /// <summary>
        /// 向量
        /// </summary>
        public Vector3d Direction { get; set; }
        /// <summary>
        /// 起始点
        /// </summary>
        public Point3d StartPt { get; set; }
        /// <summary>
        /// 终止点
        /// </summary>
        public Point3d EndPt { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        public Body CylinderBody { get; set; }
        /// <summary>
        /// 最小圆柱直径
        /// </summary>
        public double Dia { get; set; }
        public Node Node { get; set; }

        public Cylinder(Body body)
        {
            this.CylinderBody = body;
            ComputeCylinderAttr();
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
                this.CylinderBody.Blank();
            else
                this.CylinderBody.Unblank();
        }

        public bool Equals(Cylinder other)
        {
            if (this.Name == other.Name)
            {
                double angle = UMathUtils.Angle(this.Direction, other.Direction);
                if (UMathUtils.IsEqual(angle, 0))
                {
                    Matrix4 mat = new Matrix4();
                    mat.Identity();
                    mat.TransformToZAxis(this.StartPt, this.Direction);
                    Point3d thisPt = this.StartPt;
                    Point3d otherPt = other.StartPt;
                    mat.ApplyPos(ref thisPt);
                    mat.ApplyPos(ref otherPt);
                    if (UMathUtils.IsEqual(thisPt.Z, otherPt.Z))
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// 计算属性
        /// </summary>
        public void ComputeCylinderAttr()
        {
            foreach (Face fa in this.CylinderBody.GetFaces())
            {
                CircleFaceStep cs = AnalyzeFaceFactory.CreateCylinder(fa);
                if (cs != null)
                {
                    this.CylinderFaceList.Add(cs);

                }
            }
            if (this.CylinderFaceList.Count >= 2)
                this.CylinderFaceList.Sort();
            this.Direction = this.CylinderFaceList[0].Matr.GetZAxis();
            foreach (CircleFaceStep cfs in this.CylinderFaceList)
            {
                cfs.ComputeHoleStepAttr(this.Direction);
            }
            this.CylinderFaceList.Sort(delegate (CircleFaceStep a, CircleFaceStep b)
            {
                Point3d pt1 = UMathUtils.GetMiddle(a.StartPos, a.EndPos);
                Point3d pt2 = UMathUtils.GetMiddle(b.StartPos, b.EndPos);
                a.Matr.ApplyPos(ref pt1);
                a.Matr.ApplyPos(ref pt2);
                return pt2.Z.CompareTo(pt1.Z);
            });
            double max = 0;
            double min = 9999;
            double dia = 9999;
            List<CylinderStep> csp = new List<CylinderStep>();
            foreach (CircleFaceStep cfs in this.CylinderFaceList)
            {
                this.Name += cfs.ToString();
                if (max < cfs.MaxDia)
                    max = cfs.MaxDia;
                if(min>cfs.MinDia)
                {
                    min = cfs.MinDia;
                }
                if (cfs is CylinderStep)
                    csp.Add(cfs as CylinderStep);            
            }
            foreach(CylinderStep cy in csp)
            {
                if (dia > cy.MaxDia * 2)
                    dia = cy.MaxDia * 2;
            }
            this.StartPt = this.CylinderFaceList[0].StartPos;
            this.EndPt = this.CylinderFaceList[this.CylinderFaceList.Count - 1].StartPos;
            this.CylinderHigth = Math.Round(UMathUtils.GetDis(this.EndPt, this.StartPt));
            this.MaxDis = max;
            this.MinDis = min;
        }
    }
}
