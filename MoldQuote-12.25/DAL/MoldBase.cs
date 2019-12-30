﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using CycBasic;

namespace MoldQuote
{
    public class MoldBase
    {
        /// <summary>
        /// A板
        /// </summary>
        public Cuboid APlate { get; set; }
        /// <summary>
        /// B板
        /// </summary>
        public Cuboid BPlate { get; set; }
        /// <summary>
        /// 上模板
        /// </summary>
        public List<Cuboid> UpperPlates { get; set; } = new List<Cuboid>();
        /// <summary>
        /// 下模板
        /// </summary>
        public List<Cuboid> LowerPlates { get; set; } = new List<Cuboid>();
        /// <summary>
        /// 上垫脚
        /// </summary>
        public List<Cuboid> UpperSpacer { get; set; } = new List<Cuboid>();
        /// <summary>
        /// 下垫脚
        /// </summary>
        public List<Cuboid> LowerSpacer { get; set; } = new List<Cuboid>();
        /// <summary>
        /// 上顶针板
        /// </summary>
        public List<Cuboid> UpperEiectorPlates { get; set; } = new List<Cuboid>();

        /// <summary>
        /// 下顶针板
        /// </summary>
        public List<Cuboid> LowerEiectorPlates { get; set; } = new List<Cuboid>();
        /// <summary>
        /// 导柱
        /// </summary>
        public List<Cylinder> GuidePin { get; set; } = new List<Cylinder>();
        /// <summary>
        /// 导套
        /// </summary>
        public List<GuideBush> GuideBushList { get; set; } = new List<GuideBush>();
        /// <summary>
        /// 螺栓
        /// </summary>
        public List<Bolt> BoltList { get; set; } = new List<Bolt>();
        /// <summary>
        /// 模板分类
        /// </summary>
        /// <param name="cuboids"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void CuboidClassify(List<Cuboid> cuboids, Body a, Body b)
        {
            this.APlate = cuboids.Find(x => x.Body == a);
            this.BPlate = cuboids.Find(x => x.Body == b);
            double max = Math.Round(this.APlate.CenderPt.Z + this.APlate.DisPt.Z, 3);

            while (GetLoopThrugh(cuboids, ref max))
            {
            }          
            this.UpperPlates = this.UpperPlates.OrderBy(o => o.CenderPt.Z).ToList(); ;
            
            this.UpperSpacer = GetSpacer(this.UpperPlates);
           
            if (this.UpperSpacer.Count > 0)
            {
                double spacerMax = Math.Round(UpperSpacer[0].CenderPt.Z + UpperSpacer[0].DisPt.Z, 3);
                double spacerMin = Math.Round(UpperSpacer[0].CenderPt.Z - UpperSpacer[0].DisPt.Z, 3);
                foreach (Cuboid cu in cuboids)
                {
                    double cuMax = Math.Round(cu.CenderPt.Z + cu.DisPt.Z, 3);
                    double cuMin = Math.Round(cu.CenderPt.Z - cu.DisPt.Z, 3);
                   
                    if (spacerMin < cuMin && spacerMax > cuMax)
                    {
                        this.UpperEiectorPlates.Add(cu);
                    }
                }
            }

            double min = Math.Round(this.BPlate.CenderPt.Z - this.BPlate.DisPt.Z, 3);
            foreach (Cuboid cu in cuboids)
            {
                if (Math.Round(cu.CenderPt.Z + cu.DisPt.Z, 3) == min)
                {
                    this.LowerPlates.Add(cu);
                    max = Math.Round(cu.CenderPt.Z - cu.DisPt.Z, 3);
                }
            }
            this.LowerPlates = this.LowerPlates.OrderByDescending(o => o.CenderPt.Z).ToList(); ;
            this.LowerSpacer = GetSpacer(this.LowerPlates);
            if (this.LowerSpacer.Count > 0)
            {
                foreach (Cuboid cu in cuboids)
                {
                    double cuMax = Math.Round(cu.CenderPt.Z + cu.DisPt.Z, 3);
                    double cuMin = Math.Round(cu.CenderPt.Z - cu.DisPt.Z, 3);
                    double spacerMax = Math.Round(LowerSpacer[0].CenderPt.Z + cu.DisPt.Z, 3);
                    double spacerMin = Math.Round(LowerSpacer[0].CenderPt.Z - cu.DisPt.Z, 3);
                    if (spacerMin < cuMin && spacerMax > cuMax)
                    {
                        this.UpperEiectorPlates.Add(cu);
                    }
                }
            }


        }
        /// <summary>
        /// 圆柱分类
        /// </summary>
        /// <param name="cyls"></param>
        public void CylinderClassify(List<Cylinder> cyls)
        {
            CoordinateSystem wcs = Session.GetSession().Parts.Work.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(wcs, ref mat);
            foreach (Cylinder cy in cyls)
            {
                Bolt bt = Bolt.GetBolt(cy);
                if (bt != null)
                {
                    Point3d pt = bt.EndPt;
                    mat.ApplyPos(ref pt);
                    if (Math.Round(pt.Z, 3) < Math.Round(this.APlate.CenderPt.Z + this.APlate.DisPt.Z, 3) ||
                       Math.Round(pt.Z, 3) < Math.Round(this.BPlate.CenderPt.Z - this.BPlate.DisPt.Z, 3))
                    {
                        this.BoltList.Add(bt);

                    }
                    continue;
                }
                GuideBush gb = GuideBush.GetGuideBush(cy);
                if (gb != null)
                {
                    Point3d pt = bt.EndPt;
                    mat.ApplyPos(ref pt);
                    if (Math.Round(pt.Z, 3) > Math.Round(this.LowerSpacer[0].CenderPt.Z + this.LowerSpacer[0].DisPt.Z, 3) &&
                       Math.Round(pt.Z, 3) < Math.Round(this.BPlate.CenderPt.Z + this.BPlate.DisPt.Z, 3))
                    {
                        this.GuideBushList.Add(gb);

                    }
                    continue;
                }
                if (CycTraceARay.AskTraceARay(this.APlate.Body, cy.StartPt, cy.Direction) == 0 &&
                    CycTraceARay.AskTraceARay(this.BPlate.Body, cy.StartPt, cy.Direction) == 0)
                {
                    this.GuidePin.Add(cy);
                }
                bool isok = true;
                for (int i = 0; i < this.UpperEiectorPlates.Count; i++)
                {
                    if (CycTraceARay.AskTraceARay(this.UpperEiectorPlates[i].Body, cy.StartPt, cy.Direction) != 0)
                    {
                        isok = false;
                    }
                }
                if (isok)
                {
                    this.GuidePin.Add(cy);
                    continue;
                }

                for (int i = 0; i < this.LowerEiectorPlates.Count; i++)
                {
                    if (CycTraceARay.AskTraceARay(this.LowerEiectorPlates[i].Body, cy.StartPt, cy.Direction) != 0)
                    {
                        isok = false;
                    }
                }
                if (isok)
                {
                    this.GuidePin.Add(cy);
                    continue;
                }
            }
        }
        private List<Cuboid> GetSpacer(List<Cuboid> cuboids)
        {
            List<Cuboid> sp = new List<Cuboid>();
            foreach (Cuboid cu in cuboids)
            {
                if (!UMathUtils.IsEqual(cu.CenderPt.X, 0) || !UMathUtils.IsEqual(cu.CenderPt.Y, 0))
                {                   
                    sp.Add(cu);
                }

            }
            return sp;
        }

        private bool GetLoopThrugh( List<Cuboid> cd, ref double max)
        {
            bool isok = false;
            double temp = max;
            foreach (Cuboid cu in cd)
            {
                if (Math.Round(cu.CenderPt.Z - cu.DisPt.Z, 3) == max)
                {
                    temp = Math.Round(cu.CenderPt.Z + cu.DisPt.Z, 3);
                    this.UpperPlates.Add(cu);
                    isok = true;
                }
            }
            max = temp;
            return isok;

        }


    }
}
