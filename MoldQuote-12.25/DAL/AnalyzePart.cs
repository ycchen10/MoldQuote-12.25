using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using CycBasic;
using System.Collections.Concurrent;

namespace MoldQuote
{
    public class AnalyzePart
    {
        private Part m_workPart;

        private static AnalyzePart m_instance;


        private CoordinateSystem m_wcs;

        private AnalyzePart()
        {
            m_workPart = Session.GetSession().Parts.Work;
            m_wcs = m_workPart.WCS.CoordinateSystem;
        }

        public static AnalyzePart Instance()
        {
            if (m_instance == null)
                m_instance = new AnalyzePart();
            return m_instance;
        }
        /// <summary>
        /// 长方体
        /// </summary>
        public List<Cuboid> CuboidList { get; set; } = new List<Cuboid>();
        /// <summary>
        /// 圆柱体
        /// </summary>
        public List<Cylinder> CylinderList { get; set; } = new List<Cylinder>();

        public void Analyze(Part part)
        {

            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(m_wcs, ref mat);

            foreach (Body body in part.Bodies)
            {
                BodyBoundingBox box = new BodyBoundingBox();
                AnalyzeBodyFactory.GetBoundingBoxFace(body, ref box);
                Cylinder cy = AnalyzeBodyFactory.CreateCylinder(body, box);
                Cuboid cu = AnalyzeBodyFactory.CreateCuboid(body, box);
                if (cy != null)
                {
                    double angly = UMathUtils.Angle(cy.Direction, mat.GetZAxis());
                    if ((UMathUtils.IsEqual(angly, 0) || UMathUtils.IsEqual(angly, Math.PI)))
                        this.CylinderList.Add(cy);
                }

                if (cu != null)
                    this.CuboidList.Add(cu);

            }
        }

        public void SetWcs(Body aPlate, Body bPlate)
        {
            BodyBoundingBox boxA = new BodyBoundingBox();
            BodyBoundingBox boxB = new BodyBoundingBox();
            Matrix4 mat = new Matrix4();
            mat.Identity();
            AnalyzeBodyFactory.GetBoundingBoxFace(aPlate, ref boxA);
            AnalyzeBodyFactory.GetBoundingBoxFace(bPlate, ref boxB);
            Point3d pt1 = UMathUtils.GetMiddle(boxA.CenderPt, boxA.CenderPt);
            Vector3d vec = UMathUtils.GetVector(boxB.CenderPt, boxA.CenderPt);
            mat.TransformToZAxis(pt1, vec);
            Matrix4 invers = mat.GetInversMatrix();

            if (UMathUtils.IsEqual(pt1.X, 0) && UMathUtils.IsEqual(pt1.Y, 0) && UMathUtils.IsEqual(pt1.Z, 0))
            {
                CartesianCoordinateSystem wcs = CycBoundingBoxUtils.CreateCoordinateSystem(mat, invers);
                m_workPart.WCS.SetCoordinateSystem(wcs);
            }
        }
        public IDisplayObject FindDisplayObject(Node node)
        {
            foreach (Cuboid io in CuboidList)
            {
                if (io.Node != null)
                {
                    if (io.Node.Equals(node))
                        return io;
                }

            }
            foreach (Cylinder io in CylinderList)
            {
                if (io.Node != null)
                {
                    if (io.Node.Equals(node))
                        return io;
                }

            }
            return null;
        }



    }
}
