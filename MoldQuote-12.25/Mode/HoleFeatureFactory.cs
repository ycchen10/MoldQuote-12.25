using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using CycBasic;

namespace MoldQuote
{
    public class HoleFeatureFactory
    {
        /// <summary>
        /// 获取孔特征
        /// </summary>
        /// <param name="hf"></param>
        /// <returns></returns>
        public static HoleFeature GteHoleFeature(HoleFeature hf)
        {

            List<CylinderStep> cyls = new List<CylinderStep>();
            List<CircleStep> circle = new List<CircleStep>();
            List<CircularConeStep> cone = new List<CircularConeStep>();
            foreach (CircleFaceStep cf in hf.StepList)
            {
                if (cf is CylinderStep)
                    cyls.Add(cf as CylinderStep);
                if (cf is CircleStep)
                    circle.Add(cf as CircleStep);
                if (cf is CircularConeStep)
                    cone.Add(cf as CircularConeStep);
            }
            int ray1;
            int ray2;
            Vector3d vec1 = UMathUtils.GetVector(hf.StepList[0].StartPos, hf.StepList[0].EndPos);
            Vector3d vec2 = UMathUtils.GetVector(hf.StepList[0].EndPos, hf.StepList[0].StartPos);
            ray1 = CycTraceARay.AskTraceARay(hf.StepList[0].Face.GetBody(), hf.StepList[0].StartPos, vec1);
            ray2 = CycTraceARay.AskTraceARay(hf.StepList[0].Face.GetBody(), hf.StepList[0].StartPos, vec2);
            if (ray2 == 0 && ray1 == 0) //通孔
            {
                if (cyls.Count == 1)
                    return new OnlyThroughHoleFeature(cyls, cone, hf.StepList);
                else
                    return new StepThroughHoleFeature(cyls, cone, circle, hf.StepList);
            }
            else //盲孔
            {
                bool cir = false;
                bool co = false;
                foreach (CircleStep cs in circle)
                {
                    if (cs.ArcEdge.Count == 1)
                        cir = true;
                }
                foreach (CircularConeStep cs in cone)
                {
                    if (cs.ArcEdge.Count == 1)
                        co = true;
                }
                if (cir)
                    return new StepHoleFeature(cyls, cone, circle, hf.StepList);
                if (circle.Count == 0 || circle == null)
                    return new OnlyBlindHoleFeature(cyls, cone, hf.StepList);
                if (co)
                    return new StepBlindHoleFeature(cyls, cone, circle, hf.StepList);
            }
            return null;
        }
    }
}
