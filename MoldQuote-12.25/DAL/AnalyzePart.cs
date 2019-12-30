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
            AnalyzeBodyFactory bodyFactory = new AnalyzeBodyFactory();
            BodyBoundingBox box = new BodyBoundingBox();
            foreach (Body body in part.Bodies)
            {
                box = bodyFactory.GetBoundingBoxFace(body);
                Cylinder cy = bodyFactory.CreateCylinder(body, box);
                Cuboid cu = bodyFactory.CreateCuboid(body, box);
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

            Matrix4 mat = new Matrix4();
            mat.Identity();
            AnalyzeBodyFactory bodyFactory = new AnalyzeBodyFactory();
            BodyBoundingBox boxA = bodyFactory.GetBoundingBoxFace(aPlate);
            BodyBoundingBox boxB = bodyFactory.GetBoundingBoxFace(bPlate);

            Point3d pt1 = UMathUtils.GetMiddle(boxA.CenderPt, boxA.CenderPt);
            Vector3d vec = UMathUtils.GetVector(boxB.CenderPt, boxA.CenderPt);
            mat.TransformToZAxis(pt1, vec);
            Point3d pt3 = new Point3d(boxA.CenderPt.X, boxA.CenderPt.Y, boxA.CenderPt.Z);
            mat.ApplyPos(ref pt3);
            Vector3d vecX = new Vector3d();
            Vector3d vecY = new Vector3d();
            if (Math.Round(boxA.DisPt.X, 3) > Math.Round(boxA.DisPt.Y, 3))
            {
                vecX = UMathUtils.GetVector(pt3, new Point3d(pt3.X + boxA.DisPt.X, pt3.Y, pt3.Z));
                vecY = UMathUtils.GetVector(pt3, new Point3d(pt3.X, pt3.Y + boxA.DisPt.Y, pt3.Z));
            }
            else
            {
                vecY = UMathUtils.GetVector(pt3, new Point3d(pt3.X + boxA.DisPt.X, pt3.Y, pt3.Z));
                vecX = UMathUtils.GetVector(pt3, new Point3d(pt3.X, pt3.Y + boxA.DisPt.Y, pt3.Z));
            }
            mat.TransformToZAxis(pt1, vecX, vecY);
            Matrix4 invers = mat.GetInversMatrix();

            CartesianCoordinateSystem wcs = CycBoundingBoxUtils.CreateCoordinateSystem(mat, invers);
            m_workPart.WCS.SetCoordinateSystem(wcs);

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
