using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoldQuote
{
    /// <summary>
    /// 台阶盲孔
    /// </summary>
   public class StepBlindHoleFeature:HoleFeature
    {
        public List<CylinderStep> CylinderHoleList { get; private set; } = new List<CylinderStep>();
        public List<CircularConeStep> ConeList { get; private set; } = new List<CircularConeStep>();
        public List<CircleStep> CircleList { get; private set; } = new List<CircleStep>();

        public StepBlindHoleFeature(List<CylinderStep> cyl, List<CircularConeStep> con,List<CircleStep> cs, List<CircleFaceStep> cf)
        {
            this.CylinderHoleList = cyl;
            this.ConeList = con;
            this.StepList = cf;
            this.CircleList = cs;
            ComputeHoleFeatureAttr();
        }
        public override void ComputeHoleFeatureAttr()
        {
            this.StepList.Sort();
            foreach (CircleFaceStep cs in this.StepList)
            {
                cs.ComputeHoleStepAttr(this.StepList[0].Matr.GetZAxis());
            }
            this.Origin = this.StepList[0].StartPos;
            this.TopEdge = this.StepList[0].ArcEdge[0].Edge;
            this.Direction = this.StepList[0].Matr.GetZAxis();
            foreach (CircleFaceStep cs in this.StepList)
            {
                this.Name += cs.ToString();
                this.HoleHigth += cs.HoleStepHigth;
            }
        }
    }
}
