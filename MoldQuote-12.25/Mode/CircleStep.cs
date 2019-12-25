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
    /// 圆环
    /// </summary>
    public class CircleStep : CircleFaceStep
    {
        public CircleStep(Face face)
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
            foreach (Edge eg in Face.GetEdges())
            {
                if (eg.SolidEdgeType == Edge.EdgeType.Circular)
                    this.ArcEdge.Add(CycEdgeUtils.GetArcData(eg, ref err));
            }
            try
            {
                this.ArcEdge = this.ArcEdge.OrderByDescending(o => o.Radius).ToList();
                this.MaxDia = this.ArcEdge[0].Radius;
                this.MinDia = this.ArcEdge[this.ArcEdge.Count - 1].Radius;
                this.StartPos = this.ArcEdge[0].Center;
                this.EndPos = this.StartPos;
                this.Matr = mat;
                this.HoleStepHigth = 0;
            }
            catch(Exception ex)
            {
                err += this.Face.Tag.ToString() + "边错误！";
                LogMgr.WriteLog(err + ex.Message);
            }
        }
    }
}
