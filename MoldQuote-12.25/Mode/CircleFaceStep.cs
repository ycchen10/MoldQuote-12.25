using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using CycBasic;
using NXOpen.BlockStyler;

namespace MoldQuote
{
    public abstract class CircleFaceStep : IComparable<CircleFaceStep>, IDisplayObject
    {
        /// <summary>
        /// 起点
        /// </summary>
        public Point3d StartPos { get; set; }
        /// <summary>
        /// 终点
        /// </summary>
        public Point3d EndPos { get; set; }
        /// <summary>
        /// 最大半径
        /// </summary>
        public double MaxDia { get; set; }
        /// <summary>
        /// 最小半径
        /// </summary>
        public double MinDia { get; set; }

        public Face Face { get; set; }
        /// <summary>
        /// 面数据
        /// </summary>
        public CycFaceData FaceData { get; set; }
        /// <summary>
        /// 圆弧边
        /// </summary>
        public List<ArcEdgeData> ArcEdge { get; set; } = new List<ArcEdgeData>();
        /// <summary>
        /// 高度
        /// </summary>
        public double HoleStepHigth { get; set; }
        /// <summary>
        /// 矩阵
        /// </summary>
        public Matrix4 Matr { get; set; }

        public Node Node { get; set; }

        /// <summary>
        /// 判断是否是一个孔
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsTheSameHole(CircleFaceStep other)
        {
            double angle = UMathUtils.Angle(this.FaceData.Dir, other.FaceData.Dir);
            if (UMathUtils.IsEqual(angle, 0) == false && UMathUtils.IsEqual(angle, Math.PI) == false)
            {
                return false;
            }
            Vector3d vec1 = new Vector3d();
            Vector3d vec2 = new Vector3d();
            if (UMathUtils.IsEqual(this.FaceData.Point, other.FaceData.Point))
            {
                vec1 = this.FaceData.Dir;
                vec2 = new Vector3d(-vec1.X, -vec1.Y, -vec1.Z);
            }
            else
            {
                vec1 = UMathUtils.GetVector(this.FaceData.Point, other.FaceData.Point);
                vec2 = UMathUtils.GetVector(other.FaceData.Point, this.FaceData.Point);
            }
            angle = UMathUtils.Angle(this.FaceData.Dir, vec1);
            if (UMathUtils.IsEqual(angle, 0) == false && UMathUtils.IsEqual(angle, Math.PI) == false)
            {
                return false;
            }
            if (CycTraceARay.AskTraceARay(this.Face.GetBody(), this.FaceData.Point, vec1) > 1 && CycTraceARay.AskTraceARay(other.Face.GetBody(), other.FaceData.Point, vec2) > 1)

            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 判断是否是一个圆台
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsCylinder(CircleFaceStep other)
        {
            double angle = UMathUtils.Angle(this.FaceData.Dir, other.FaceData.Dir);
            if (UMathUtils.IsEqual(angle, 0) == false && UMathUtils.IsEqual(angle, Math.PI) == false)
            {
                return false;
            }
            Vector3d vec1 = new Vector3d();
            Vector3d vec2 = new Vector3d();
            if (UMathUtils.IsEqual(this.FaceData.Point, other.FaceData.Point))
            {
                vec1 = this.FaceData.Dir;
                vec2 = new Vector3d(-vec1.X, -vec1.Y, -vec1.Z);
            }
            else
            {
                vec1 = UMathUtils.GetVector(this.FaceData.Point, other.FaceData.Point);
                vec2 = UMathUtils.GetVector(other.FaceData.Point, this.FaceData.Point);
            }
            angle = UMathUtils.Angle(this.FaceData.Dir, vec1);
            if (UMathUtils.IsEqual(angle, 0) == false && UMathUtils.IsEqual(angle, Math.PI) == false)
            {
                return false;
            }
            return true;
        }
        public int CompareTo(CircleFaceStep other)
        {
            try
            {
                Point3d thisPt = new Point3d();
                Point3d otherPt = new Point3d();
                if (UMathUtils.IsEqual(this.StartPos, this.EndPos))
                    thisPt = this.StartPos;
                else
                    thisPt = UMathUtils.GetMiddle(this.StartPos, this.EndPos);
                if (UMathUtils.IsEqual(other.StartPos, other.EndPos))
                    otherPt = other.StartPos;
                else
                    otherPt = UMathUtils.GetMiddle(other.StartPos, other.EndPos);
                Matrix4 mat = new Matrix4();
                Vector3d vec = new Vector3d();
                mat.Identity();
                if (this.MaxDia == this.MinDia)
                {
                    if (this.MaxDia >= other.MaxDia)
                    {
                        if (UMathUtils.IsEqual(this.StartPos, other.StartPos))
                        {
                            vec = UMathUtils.GetVector(other.EndPos, this.EndPos);
                            mat.TransformToZAxis(this.EndPos, vec);
                        }
                        else
                        {
                            vec = UMathUtils.GetVector(other.StartPos, this.StartPos);
                            mat.TransformToZAxis(this.StartPos, vec);
                        }
                    }
                    else
                    {
                        if (UMathUtils.IsEqual(this.StartPos, other.StartPos))
                        {
                            vec = UMathUtils.GetVector(this.EndPos, other.EndPos);
                            mat.TransformToZAxis(other.EndPos, vec);
                        }
                        else
                        {
                            vec = UMathUtils.GetVector(this.StartPos, other.StartPos);
                            mat.TransformToZAxis(other.StartPos, vec);
                        }
                    }
                }
                else
                    mat = this.Matr;
                mat.ApplyPos(ref thisPt);
                mat.ApplyPos(ref otherPt);

                if (Math.Round(thisPt.Z, 3) > Math.Round(otherPt.Z, 3))
                    return -1;
                else if (Math.Round(thisPt.Z, 3) == Math.Round(otherPt.Z, 3))
                {
                    if (this.MaxDia >= other.MaxDia)
                        return -1;
                    else
                        return 1;
                }
                else
                    return 1;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog(ex.Message);
            }
            return 1;

        }
        /// <summary>
        /// 高亮显示
        /// </summary>
        /// <param name="highlight"></param>
        public void Highlight(bool highlight)
        {
            if (highlight)
                this.Face.Highlight();
            else
                this.Face.Unhighlight();
        }
        /// <summary>
        /// 计算属性
        /// </summary>
        /// <param name="Direction">方向</param>
        public abstract void ComputeHoleStepAttr(Vector3d dir);

        public override string ToString()
        {
            return this.MaxDia.ToString() + "," + this.MinDia.ToString() + "," + this.HoleStepHigth.ToString() + ",";
        }
        /// <summary>
        /// 判断是否是孔
        /// </summary>
        /// <param name="faceData"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool GetHole()
        {
            NXOpen.UF.UFSession ufsession = NXOpen.UF.UFSession.GetUFSession();
            double[] pot = { this.FaceData.Point.X, this.FaceData.Point.Y, this.FaceData.Point.Z };
            int out_status;
            ufsession.Modl.AskPointContainment(pot, this.Face.GetBody().Tag, out out_status);
            if (out_status != 2)
            {
                return false;
            }
            return true;
        }
    }
}
