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
    public class Bolt:IDisplayObject
    {
        public Cylinder BoltCy { get; set; }
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

        public Node Node { get; set; }
       

        public static Bolt GetBolt(Cylinder cy)
        {
            Bolt bt = new Bolt();
            List<CylinderStep> cs = new List<CylinderStep>();
            foreach (CircleFaceStep cf in cy.CylinderFaceList)
            {
                if (cf is CylinderStep)
                    cs.Add(cf as CylinderStep);
            }
            if (cs.Count == 2)
            {
                bt.Dia = cy.Dia;
                if (AskSexangle(cy.FaceOfMaxZ.Face))
                {
                    bt.StartPt = cy.FaceOfMaxZ.Point;
                    bt.EndPt = cy.FaceOfMinZ.Point;
                    bt.Length = Math.Round(UMathUtils.GetDis(bt.StartPt, bt.EndPt), 0);
                }
                if (AskSexangle(cy.FaceOfMinZ.Face))
                {
                    bt.StartPt = cy.FaceOfMinZ.Point;
                    bt.EndPt = cy.FaceOfMaxZ.Point;
                    bt.Length = Math.Round(UMathUtils.GetDis(bt.StartPt, bt.EndPt), 0);
                }
                bt.Name = "M" + bt.Dia + "X" + bt.Length;
            }
            return null;
        }

        private static bool AskSexangle(Face face)
        {
            CycFaceLoop.LoopList[] loop = CycFaceLoop.AskFaceLoops(face.Tag);
            foreach (CycFaceLoop.LoopList lp in loop)
            {
                if (lp.Type == 2 && lp.EdgeList.Length == 6 && loop.Length == 2)
                {
                    return true;
                }
            }
            return false;
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
                this.BoltCy.CylinderBody.Blank();
            else
                this.BoltCy.CylinderBody.Unblank();
        }
    }
}
