using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace CycBasic
{
    /// <summary>
    /// 射线
    /// </summary>
    public class CycTraceARay
    {

        public static int AskTraceARay(Body body,Point3d pt,Vector3d vec)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag[] bodyTag = { body.Tag };
            UFModl.RayHitPointInfo[] info;
            double[] origin = { pt.X, pt.Y, pt.Z };
            double[] dir = { vec.X, vec.Y, vec.Z };
            double[] mat = new double[16];
            theUFSession.Mtx4.Identity(mat);
            int res=0;
            theUFSession.Modl.TraceARay(1, bodyTag, origin, dir, mat, 0, out res, out info);
            return res;
        }



    }
}
