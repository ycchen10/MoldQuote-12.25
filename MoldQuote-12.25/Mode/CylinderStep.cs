using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using CycBasic;

namespace MoldQuote
{
    /// <summary>
    /// 圆柱孔
    /// </summary>
    public class CylinderStep : CircleFaceStep
    {
      
        public CylinderStep(Face face)
        {
            this.Face = face;
            this.FaceData = CycFaceUtils.AskFaceData(face);
            
            ComputeHoleStepAttr(this.FaceData.Dir);
        }

        public override void ComputeHoleStepAttr(Vector3d dir)
        {
            string err = "";
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(this.FaceData.Point, dir);
            this.Matr = mat;
            foreach (Edge eg in Face.GetEdges())
            {
                if (eg.SolidEdgeType == Edge.EdgeType.Circular)
                    this.ArcEdge.Add(CycEdgeUtils.GetArcData(eg, ref err));
            }
            this.ArcEdge.Sort(delegate (ArcEdgeData a, ArcEdgeData b)
            {
                Point3d pt1 = a.Center;
                Point3d pt2 = b.Center;
                mat.ApplyPos(ref pt1);
                mat.ApplyPos(ref pt2);
                return pt2.Z.CompareTo(pt1.Z);
            });
            try
            {
                this.MaxDia = Math.Round(this.ArcEdge[0].Radius, 3);
                this.MinDia = this.MaxDia;
                this.StartPos = this.ArcEdge[0].Center;
                this.EndPos = this.ArcEdge[this.ArcEdge.Count - 1].Center;
                this.HoleStepHigth = Math.Round(UMathUtils.GetDis(this.StartPos, this.EndPos));
            }
            catch (Exception ex)
            {
                err += this.Face.Tag.ToString() + "错误！";
                LogMgr.WriteLog(err + ex.Message);
            }
        }
    }
}
