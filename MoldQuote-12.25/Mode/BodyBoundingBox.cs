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
    /// 体外形面
    /// </summary>
   public class BodyBoundingBox
    {
        /// <summary>
        /// Z 最大面
        /// </summary>
        public List<CycFaceData> FaceOfMaxZ { get; set; } = new List<CycFaceData>();
        /// <summary>
        /// Z 最小面
        /// </summary>
        public List<CycFaceData> FaceOfMinZ { get; set; } = new List<CycFaceData>();
        /// <summary>
        /// X 最大面
        /// </summary>
        public List<CycFaceData> FaceOfMaxX { get; set; } = new List<CycFaceData>();
        /// <summary>
        ///  X 最小面
        /// </summary>
        public List<CycFaceData> FaceOfMinX { get; set; } = new List<CycFaceData>();
        /// <summary>
        /// Y 最大面
        /// </summary>
        public List<CycFaceData> FaceOfMaxY { get; set; } = new List<CycFaceData>();
        /// <summary>
        ///  Y 最小面
        /// </summary>
        public List<CycFaceData> FaceOfMinY { get; set; } = new List<CycFaceData>();
        /// <summary>
        /// 体
        /// </summary>
        public Body Body { get; set; }
        /// <summary>
        /// 中心dian
        /// </summary>
        public Point3d CenderPt { get; set; }
        /// <summary>
        /// 外形点
        /// </summary>
        public Point3d DisPt { get; set; }
    }
}
