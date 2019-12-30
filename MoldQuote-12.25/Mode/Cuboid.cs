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
    /// <summary>
    /// 长方体
    /// </summary>
    public class Cuboid : IDisplayObject
    {

        public List<CycFaceData> CavityFace { get; set; }

        public Node Node { get; set; }

        public List<HoleFeature> HoleList { get; set; } = new List<HoleFeature>();

        public BodyBoundingBox Box { get; set; }

        public Body Body { get; set; }
        public Cuboid(Body body)
        {
            this.Body = body;
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
                this.Body.Blank();
            else
                this.Body.Unblank();
        }

        public void GetCuboidFeature()
        {

            List<CircleFaceStep> cif = new List<CircleFaceStep>();

            foreach (Face face in this.Body.GetFaces())
            {
                CircleFaceStep cs = AnalyzeFaceFactory.CreateCircleStep(face);
                if (cs != null)
                    cif.Add(cs);
                this.CavityFace.Add(CycFaceUtils.AskFaceData(face));
            }
            foreach (CircleFaceStep cs in cif)
            {
                HoleFeature hf = FindHole(cs, this.HoleList);
                if (hf == null)
                {
                    hf = new HoleFeature(cs);
                    this.HoleList.Add(hf);
                }
            }
            this.CavityFace = Romove(this.Box.FaceOfMaxX, this.CavityFace);
            this.CavityFace = Romove(this.Box.FaceOfMaxY, this.CavityFace);
            this.CavityFace = Romove(this.Box.FaceOfMaxZ, this.CavityFace);
            this.CavityFace = Romove(this.Box.FaceOfMinX, this.CavityFace);
            this.CavityFace = Romove(this.Box.FaceOfMinY, this.CavityFace);
            this.CavityFace = Romove(this.Box.FaceOfMinZ, this.CavityFace);
        }
        private HoleFeature FindHole(CircleFaceStep cs, List<HoleFeature> hfs)
        {
            if (hfs.Count == 0 || hfs == null)
                return null;
            foreach (HoleFeature hf in hfs)
            {
                if (hf.FindHole(cs))
                    return hf;
            }
            return null;

        }
        private List<CycFaceData> Romove(List<CycFaceData> fd, List<CycFaceData> cfd)
        {
            foreach (CycFaceData cd in fd)
            {
                cfd.Remove(cd);
            }
            return cfd;
        }


    }
}
