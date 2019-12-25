using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using CycBasic;
using NXOpen.Utilities;

namespace MoldQuote
{
    public class AnalyzeFaceFactory
    {
        /// <summary>
        /// 获取孔面
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static CircleFaceStep CreateHoleStep(Face face)
        {
            if (face.SolidFaceType == Face.FaceType.Cylindrical) //圆柱
            {
                CylinderStep cs = new CylinderStep(face);
                bool hole = true;
                if (UMathUtils.IsEqual(cs.ArcEdge[0].Angle, Math.PI / 2) || UMathUtils.IsEqual(cs.ArcEdge[cs.ArcEdge.Count - 1].Angle, Math.PI / 2))
                    hole = false;
                if (cs.GetHole() && hole)
                    return cs;
            }
            if (face.SolidFaceType == Face.FaceType.Conical)
            {
                CircularConeStep ch = new CircularConeStep(face);
                bool hole = true;
                if (UMathUtils.IsEqual(ch.ArcEdge[0].Angle, Math.PI / 2) || UMathUtils.IsEqual(ch.ArcEdge[ch.ArcEdge.Count - 1].Angle, Math.PI / 2))
                    hole = false;
                if (ch.GetHole() || hole)
                    return ch;
            }
            if (face.SolidFaceType == Face.FaceType.Planar)
            {
                if (face.GetEdges().Length <= 2)
                {
                    bool isCycle = true;
                    foreach (Edge edge in face.GetEdges())
                    {
                        if (edge.SolidEdgeType != Edge.EdgeType.Circular)
                        {
                            isCycle = false;
                            break;
                        }

                    }
                    if (isCycle)
                    {
                        return new CircleStep(face);
                    }
                }
                else if (face.GetEdges().Length == 4)
                {
                    string err = "";
                    List<ArcEdgeData> data = new List<ArcEdgeData>();
                    foreach (Edge edge in face.GetEdges())
                    {
                        if (edge.SolidEdgeType == Edge.EdgeType.Circular)
                            data.Add(CycEdgeUtils.GetArcData(edge, ref err));
                    }
                    if (data.Count == 2)
                    {
                        if (UMathUtils.IsEqual(data[0].Center, data[1].Center))
                            return new CircleStep(face);
                    }
                    if (data.Count == 1)
                    {
                        return new CircleStep(face);
                    }
                }
                else if(face.GetEdges().Length==6)
                {
                    string err = "";
                    List<ArcEdgeData> data = new List<ArcEdgeData>();
                    foreach (Edge edge in face.GetEdges())
                    {
                        if (edge.SolidEdgeType == Edge.EdgeType.Circular)
                            data.Add(CycEdgeUtils.GetArcData(edge, ref err));
                    }
                    if(data.Count==4)
                    {
                        bool isCycle = true;
                        for(int i=1;i<4;i++)
                        {
                            if (UMathUtils.IsEqual(data[0].Center, data[i].Center))
                                isCycle = false;
                        }
                        if(isCycle)
                            return new CircleStep(face);
                    }
                }

            }
            return null;
        }
        /// <summary>
        /// 获取圆柱面
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static CircleFaceStep CreateCylinder(Face face)
        {
            if (face.SolidFaceType == Face.FaceType.Cylindrical) //圆柱
            {
                CylinderStep cs = new CylinderStep(face);
                bool hole = true;
                if (face.GetEdges().Length != 2)
                    hole = false;
                if (cs.GetHole() && hole)
                    return cs;
            }
            if (face.SolidFaceType == Face.FaceType.Conical)
            {
                CircularConeStep ch = new CircularConeStep(face);
                bool hole = true;
                if (face.GetEdges().Length != 2)
                    hole = false;
                if (ch.GetHole() || hole)
                    return ch;
            }
            if (face.SolidFaceType == Face.FaceType.Planar)
            {
                CycFaceLoop.LoopList[] loops = CycFaceLoop.AskFaceLoops(face.Tag);
                string err = "";
                foreach(CycFaceLoop.LoopList lt in loops)
                {
                    if(lt.Type==1&&lt.EdgeList.Length==1)
                    {
                        Edge eg = NXObjectManager.Get(lt.EdgeList[0]) as Edge;
                        ArcEdgeData ad = CycEdgeUtils.GetArcData(eg,ref err);
                        if (ad != null && ad.IsWholeCircle)
                            return new CircleStep(face);
                    }
                   
                }


            }
            return null;
        }
    }
}
