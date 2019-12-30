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
    public class AnalyzeBodyFactory
    {
        public Cylinder CreateCylinder(Body body, BodyBoundingBox box)
        {
            if (box.FaceOfMaxZ.Count == 1 && box.FaceOfMinZ.Count == 1
                && (box.FaceOfMaxX.Count == 0 || box.FaceOfMaxX == null)
                 && (box.FaceOfMinX.Count == 0 || box.FaceOfMinX == null)
                  && (box.FaceOfMaxY.Count == 0 || box.FaceOfMaxY == null)
                   && (box.FaceOfMinY.Count == 0 || box.FaceOfMinY == null)
                )
            {
                if (AskFacePeripheralIsArc(box.FaceOfMaxZ[0].Face) && AskFacePeripheralIsArc(box.FaceOfMinZ[0].Face))
                {
                    Cylinder cy = new Cylinder(body);
                    cy.FaceOfMaxZ = box.FaceOfMaxZ[0];
                    cy.FaceOfMinZ = box.FaceOfMinZ[0];

                    return cy;
                }

            }

            return null;
        }
        public Cuboid CreateCuboid(Body body, BodyBoundingBox box)
        {
            if (box.FaceOfMaxZ.Count >= 1 && box.FaceOfMinZ.Count >= 1
                && box.FaceOfMaxX.Count >= 1 && box.FaceOfMinX.Count >= 1
                  && box.FaceOfMaxY.Count >= 1 && box.FaceOfMinY.Count >= 1
                )

            {
                Cuboid cu = new Cuboid(body);
                cu.Box = box;
                return cu;
            }
            return null;

        }
        public BodyBoundingBox GetBoundingBoxFace(Body body)
        {
            BodyBoundingBox box = new BodyBoundingBox();
            NXObject[] obj = { body };
            Matrix4 mat = new Matrix4();
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            mat.Identity();
            mat.TransformToCsys(wcs, ref mat);
            CycBoundingBoxUtils.GetBoundingBoxInLocal(obj, null, mat, ref centerPt, ref disPt);
            box.CenderPt = centerPt;
            box.DisPt = disPt;
            box.Body = body;
            List<CycFaceData> cf = new List<CycFaceData>();
            foreach (Face fe in body.GetFaces())
            {
                CycFaceData faceData = CycFaceUtils.AskFaceData(fe);
                cf.Add(faceData);
                //double angleX = UMathUtils.Angle(faceData.Dir, mat.GetXAxis());
                //double angleY = UMathUtils.Angle(faceData.Dir, mat.GetYAxis());
                //double angleZ = UMathUtils.Angle(faceData.Dir, mat.GetZAxis());
                //Point3d pt1 = faceData.Point;
                //mat.ApplyPos(ref pt1);
                //if (UMathUtils.IsEqual(angleZ, 0))
                //{
                //    if (UMathUtils.IsEqual(faceData.Point.Z, centerPt.Z + disPt.Z))
                //        box.FaceOfMaxZ.Add(faceData);
                //}
                //if (UMathUtils.IsEqual(angleZ, Math.PI))
                //{
                //    if (UMathUtils.IsEqual(faceData.Point.Z, centerPt.Z - disPt.Z))
                //        box.FaceOfMinZ.Add(faceData);
                //}
                //if (UMathUtils.IsEqual(angleX, 0))
                //{
                //    if (UMathUtils.IsEqual(faceData.Point.X, centerPt.X + disPt.X))
                //        box.FaceOfMaxX.Add(faceData);
                //}
                //if (UMathUtils.IsEqual(angleX, Math.PI))
                //{
                //    if (UMathUtils.IsEqual(faceData.Point.X, centerPt.X - disPt.X))
                //        box.FaceOfMinX.Add(faceData);
                //}
                //if (UMathUtils.IsEqual(angleY, 0))
                //{
                //    if (UMathUtils.IsEqual(faceData.Point.Y, centerPt.Y + disPt.Y))
                //        box.FaceOfMaxY.Add(faceData);
                //}
                //if (UMathUtils.IsEqual(angleY, Math.PI))
                //{
                //    if (UMathUtils.IsEqual(faceData.Point.Y, centerPt.Y - disPt.Y))
                //        box.FaceOfMinY.Add(faceData);
                //}
            }
            box.FaceOfMaxX = cf.Where(e => UMathUtils.IsEqual(UMathUtils.Angle(e.Dir, mat.GetXAxis()), 0)&& UMathUtils.IsEqual(e.Point.X, centerPt.X + disPt.X)).ToList();
            box.FaceOfMinX = cf.Where(e => UMathUtils.IsEqual(UMathUtils.Angle(e.Dir, mat.GetXAxis()), Math.PI) && UMathUtils.IsEqual(e.Point.X, centerPt.X - disPt.X)).ToList();

            box.FaceOfMaxY = cf.Where(e => UMathUtils.IsEqual(UMathUtils.Angle(e.Dir, mat.GetYAxis()), 0) && UMathUtils.IsEqual(e.Point.Y, centerPt.Y + disPt.Y)).ToList();
            box.FaceOfMinY = cf.Where(e => UMathUtils.IsEqual(UMathUtils.Angle(e.Dir, mat.GetYAxis()), Math.PI) && UMathUtils.IsEqual(e.Point.Y, centerPt.Y - disPt.Y)).ToList();

            box.FaceOfMaxZ = cf.Where(e => UMathUtils.IsEqual(UMathUtils.Angle(e.Dir, mat.GetZAxis()), 0) && UMathUtils.IsEqual(e.Point.Z, centerPt.Z + disPt.Z)).ToList();
            box.FaceOfMinZ = cf.Where(e => UMathUtils.IsEqual(UMathUtils.Angle(e.Dir, mat.GetZAxis()), Math.PI) && UMathUtils.IsEqual(e.Point.Z, centerPt.Z - disPt.Z)).ToList();



            return box;

        }
        /// <summary>
        /// 分析面最边缘线是否是圆弧
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public bool AskFacePeripheralIsArc(Face face)
        {
            CycFaceLoop.LoopList[] loop = CycFaceLoop.AskFaceLoops(face.Tag);
            foreach (CycFaceLoop.LoopList lp in loop)
            {
                if (lp.Type == 1 && lp.EdgeList.Length == 1)
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
