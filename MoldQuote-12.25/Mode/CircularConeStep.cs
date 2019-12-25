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
    /// 圆锥面
    /// </summary>
    public class CircularConeStep : CircleFaceStep
    {
       
        public CircularConeStep(Face face)
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
            foreach (Edge eg in this.Face.GetEdges())
            {
                if (eg.SolidEdgeType == Edge.EdgeType.Circular)
                    this.ArcEdge.Add(CycEdgeUtils.GetArcData(eg, ref err));
            }
            if (this.ArcEdge.Count == 1)
            {
                this.StartPos = this.ArcEdge[0].Center;
                this.EndPos = UMathUtils.GetSymmetry(this.StartPos, this.FaceData.Point);
                mat.TransformToZAxis(this.StartPos, UMathUtils.GetVector(this.EndPos, this.StartPos));
                this.MaxDia = Math.Round(this.ArcEdge[0].Radius, 3);
                this.MinDia = 0;
                this.HoleStepHigth = UMathUtils.GetDis(this.StartPos, this.EndPos);
            }
            else if (this.ArcEdge.Count == 2)
            {
                this.ArcEdge = this.ArcEdge.OrderByDescending(o => o.Radius).ToList();
                this.StartPos = this.ArcEdge[0].Center;
                this.EndPos = this.ArcEdge[1].Center;
                this.MaxDia = Math.Round(this.ArcEdge[0].Radius, 3);
                this.MinDia = Math.Round(this.ArcEdge[1].Radius, 3);
                mat.TransformToZAxis(this.StartPos, UMathUtils.GetVector(this.EndPos, this.StartPos));
                this.HoleStepHigth = UMathUtils.GetDis(this.StartPos, this.EndPos);
            }
            else
            {
                err += this.Face.Tag.ToString() + "边错误！";
                LogMgr.WriteLog(err);
            }
        }
    }
}
