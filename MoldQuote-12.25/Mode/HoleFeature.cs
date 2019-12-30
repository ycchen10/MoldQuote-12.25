using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using CycBasic;

namespace MoldQuote
{
    public class HoleFeature : IEquatable<HoleFeature>, IDisplayObject, IComparable<HoleFeature>
    {

        /// <summary>
        /// 阶梯
        /// </summary>
        public List<CircleFaceStep> StepList = new List<CircleFaceStep>();
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d Origin { get; set; }
        /// <summary>
        ///轴向
        /// </summary>
        public Vector3d Direction { get; set; }
        /// <summary>
        /// 顶边
        /// </summary>
        public Edge TopEdge { get; set; } = null;
        /// <summary>
        /// 孔高度
        /// </summary>
        public double HoleHigth { get; set; }

        public Node Node { get; set; }

        public HoleFeature()
        {

        }
        public HoleFeature(CircleFaceStep cs)
        {
            this.StepList.Add(cs);
        }

        public bool Equals(HoleFeature other)
        {
            if (this.Name == other.Name)
            {
                double angle = UMathUtils.Angle(this.Direction, other.Direction);
                if (UMathUtils.IsEqual(angle, 0))
                {
                    Matrix4 mat = new Matrix4();
                    mat.Identity();
                    mat.TransformToZAxis(this.Origin, this.Direction);
                    Point3d thisPt = this.Origin;
                    Point3d otherPt = other.Origin;
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

        public void Highlight(bool highlight)
        {
            foreach (CircleFaceStep hs in this.StepList)
            {

                hs.Highlight(highlight);

            }
        }
        /// <summary>
        /// 写入属性
        /// </summary>
        public virtual void ComputeHoleFeatureAttr()
        {
        }

        public int CompareTo(HoleFeature other)
        {
            Point3d pt1 = this.Origin;
            Point3d pt2 = other.Origin;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(pt1, this.Direction);
            mat.ApplyPos(ref pt1);
            mat.ApplyPos(ref pt2);

            if (Math.Round(pt1.X, 4) > Math.Round(pt2.X, 4))
                return -1;
            if (Math.Round(pt1.X, 4) == Math.Round(pt2.X, 4))
            {
                if (Math.Round(pt2.Y, 4) >= Math.Round(pt2.Y, 4))
                    return -1;
                else
                    return 1;
            }
            else
                return 1;


        }

        public bool FindHole(CircleFaceStep cs)
        {
            if(this.StepList.Count==0||this.StepList==null)
            {
                this.StepList.Add(cs);
                return true;
            }
            else
            {
                if (this.StepList[0].IsCylinder(cs))
                {
                    this.StepList.Add(cs);
                    return true;
                }
                   
            }
            return false;
        }
    }
}
