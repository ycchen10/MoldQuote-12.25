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
    /// <summary>
    /// 单一的通孔
    /// </summary>
    public class OnlyThroughHoleFeature : HoleFeature
    {
        public List<CylinderStep> CylinderHoleList { get; private set; } = new List<CylinderStep>();
        public List<CircularConeStep> ConeList { get; private set; } = new List<CircularConeStep>();

        public OnlyThroughHoleFeature(List<CylinderStep> cyl, List<CircularConeStep> con, List<CircleFaceStep> cf)
        {
            this.CylinderHoleList = cyl;
            this.ConeList = con;
            this.StepList = cf;
            ComputeHoleFeatureAttr();
        }
        public override void ComputeHoleFeatureAttr()
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            foreach (CircularConeStep ccs in ConeList)
            {
                if (ccs.ArcEdge.Count == 1)
                    mat = ccs.Matr;
            }
            double max = 0;
            try
            {

                foreach (CircleFaceStep cs in this.StepList)
                {
                    if (max < cs.MaxDia)
                    {
                        max = cs.MaxDia;
                    }
                    cs.ComputeHoleStepAttr(mat.GetZAxis());
                }
                this.StepList.Sort();
                this.Direction = mat.GetZAxis();
                this.Origin = this.StepList[0].StartPos;
                this.HoleHigth = Math.Round(UMathUtils.GetDis(this.StepList[0].StartPos, this.StepList[this.StepList.Count - 1].StartPos), 3);
                this.Name = max.ToString() + this.HoleHigth.ToString();
                this.TopEdge = this.StepList[0].ArcEdge[0].Edge;
            }
            catch(Exception ex)
            {
                LogMgr.WriteLog("MoldQuote.OnlyThroughHoleFeature" + ex.Message);
            }

        }

    }
}
