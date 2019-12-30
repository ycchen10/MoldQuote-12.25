using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using CycBasic;

namespace MoldQuote
{
    public class GuideBush
    {
        public Cylinder GuideBushCy { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public double Length { get; set; }
        public Point3d StartPt { get; set; }

        public Point3d EndPt { get; set; }
        public double Dia { get; set; }

        public static GuideBush GetGuideBush(Cylinder cy)
        {
            GuideBush gb = new GuideBush();
            List<CylinderStep> cys = new List<CylinderStep>();
            foreach (CircleFaceStep cs in cy.CylinderFaceList)
            {
                if (cs is CylinderStep)
                    cys.Add(cs as CylinderStep);
            }
            if (cys.Count != 0 && AskAre(cy.FaceOfMaxZ.Face) && AskAre(cy.FaceOfMinZ.Face))
            {

                gb.Dia = cy.Dia;
                gb.StartPt = cy.FaceOfMaxZ.Point;
                gb.EndPt = cy.FaceOfMinZ.Point;
                gb.Length = Math.Round(UMathUtils.GetDis(cy.FaceOfMaxZ.Point, cy.FaceOfMinZ.Point), 3);
                gb.Name = "D" + gb.Dia.ToString() + "H" + gb.Length;
                return gb;
            }
            return null;
        }

        private static bool AskAre(Face face)
        {
            CycFaceLoop.LoopList[] loop = CycFaceLoop.AskFaceLoops(face.Tag);
            foreach (CycFaceLoop.LoopList lp in loop)
            {
                if (lp.Type == 2 && lp.EdgeList.Length == 1 && loop.Length == 2)
                {
                    Edge ed = NXObjectManager.Get(lp.EdgeList[0]) as Edge;
                    if (ed.SolidEdgeType == Edge.EdgeType.Circular)
                        return true;
                }
            }
            return false;
        }
    }
}
