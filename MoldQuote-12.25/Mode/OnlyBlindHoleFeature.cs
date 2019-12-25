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
    /// 单一盲孔
    /// </summary>
    public class OnlyBlindHoleFeature : HoleFeature
    {
        public List<CylinderStep> CylinderHoleList { get; set; } = new List<CylinderStep>();
        public List<CircularConeStep> ConeList { get; set; } = new List<CircularConeStep>();

        public OnlyBlindHoleFeature(List<CylinderStep> cyl, List<CircularConeStep> con, List<CircleFaceStep> cf)
        {
            this.CylinderHoleList = cyl;
            this.ConeList = con;
            this.StepList = cf;
            ComputeHoleFeatureAttr();
        }
        public override void ComputeHoleFeatureAttr()
        {
            string err = "";
            Matrix4 mat = this.StepList[0].Matr;
            for(int i=1;i<this.StepList.Count;i++ )
            {
                this.StepList[0].ComputeHoleStepAttr(mat.GetZAxis());
            }
            try
            {
                this.StepList.Sort(delegate (CircleFaceStep a, CircleFaceStep b)
                {
                    Point3d pt1 = UMathUtils.GetMiddle(a.StartPos, a.EndPos);
                    Point3d pt2 = UMathUtils.GetMiddle(b.StartPos, b.EndPos);
                    mat.ApplyPos(ref pt1);
                    mat.ApplyPos(ref pt2);
                    return pt2.Z.CompareTo(pt1.Z);
                });
                this.Origin = this.StepList[0].StartPos;
                this.TopEdge = this.StepList[0].ArcEdge[0].Edge;
                this.Direction = mat.GetZAxis();
                foreach (CircleFaceStep cs in this.StepList)
                {
                    this.Name += cs.ToString();
                    this.HoleHigth += cs.HoleStepHigth;
                }
            }
            catch(Exception ex)
            {
                LogMgr.WriteLog("MoldQuote.OnlyThroughHoleFeature"+err + ex.Message);
            }
            
        }


    }
}
