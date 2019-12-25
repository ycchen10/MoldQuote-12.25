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
    /// 平底盲孔
    /// </summary>
    class StepHoleFeature:HoleFeature
    {
        public List<CylinderStep> CylinderHoleList { get; private set; } = new List<CylinderStep>();
        public List<CircularConeStep> ConeList { get; private set; } = new List<CircularConeStep>();
        public List<CircleStep> CircleList { get; private set; } = new List<CircleStep>();

        public StepHoleFeature(List<CylinderStep> cyl, List<CircularConeStep> con, List<CircleStep> cs, List<CircleFaceStep> cf)
        {
            this.CylinderHoleList = cyl;
            this.ConeList = con;
            this.StepList = cf;
            this.CircleList = cs;
            ComputeHoleFeatureAttr();
        }
        public override void ComputeHoleFeatureAttr()
        {
            Matrix4 mat = this.CircleList[this.CircleList.Count - 1].Matr;
            try
            {

                foreach (CircleFaceStep cs in StepList)
                {
                    cs.ComputeHoleStepAttr(mat.GetZAxis());
                }
                this.StepList.Sort();
                foreach (CircleFaceStep cs in StepList)
                {
                    this.Name += cs.ToString();
                    this.HoleHigth += cs.HoleStepHigth;
                }
                this.Direction = mat.GetZAxis();
                this.Origin = this.StepList[0].StartPos;
                this.TopEdge = this.StepList[0].ArcEdge[0].Edge;
            }
            catch(Exception ex)
            {
                LogMgr.WriteLog("MoldQuote.StepHoleFeature" + ex.Message);
            }
        }
    }
}
