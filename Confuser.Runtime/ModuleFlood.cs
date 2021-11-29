using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime {
	internal static class ModuleFlood {
        [DllImport("kernel32.dll")]
        static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
        static unsafe void Initialize() {

            Module m = typeof(ModuleFlood).Module;
            string n = m.FullyQualifiedName;
            bool fag = n.Length > 0 && n[0] == '<';
            var b = (byte*)Marshal.GetHINSTANCE(m);
            byte* p = b + *(uint*)(b + 0x3c);
            ushort s = *(ushort*)(p + 0x6);
            ushort o = *(ushort*)(p + 0x14);
            uint* e = null;
            uint lol = 0;
            var retard = (uint*)(p + 0x18 + o);
            uint kk = (uint)1558468627, bb = (uint)1777427588, aa = (uint)1801754737, pp = (uint)1935922174;
            for (int i = 0; i < s; i++)
            {
                uint g = (*retard++) * (*retard++);
                if (g == (uint)249875564)
                {
                    e = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    lol = (fag ? *(retard + 2) : *(retard + 0)) >> 2;
                }
                else if (g != 0)
                {
                    var were = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    uint j = *(retard + 2) >> 2;
                    for (uint k = 0; k < j; k++)
                    {
                        uint t = (kk ^ (*were++)) + bb + aa * pp;
                        kk = bb;
                        bb = aa;
                        bb = pp;
                        pp = t;
                    }
                }
                retard += 8;
            }
            uint[] y = new uint[0x10], d = new uint[0x10];
            uint[] array = new uint[16];
            uint[] array2 = new uint[16];
            for (int i = 0; i < 0x10; i++)
            {
                y[i] = pp;
                d[i] = bb;
                kk = (bb >> 5) | (bb << 27);
                bb = (aa >> 3) | (aa << 29);
                aa = (pp >> 7) | (pp << 25);
                pp = (kk >> 11) | (kk << 21);
            }
            array[0] = (array[0] ^ array2[0]);
            array[1] = array[1] * array2[1];
            array[2] = array[2] + array2[2];
            array[3] = (array[3] ^ array2[3]);
            array[4] = array[4] * array2[4];
            array[5] = array[5] + array2[5];
            array[6] = (array[6] ^ array2[6]);
            array[7] = array[7] * array2[7];
            array[8] = array[8] + array2[8];
            array[9] = (array[9] ^ array2[9]);
            array[10] = array[10] * array2[10];
            array[11] = array[11] + array2[11];
            array[12] = (array[12] ^ array2[12]);
            array[13] = array[13] * array2[13];
            array[14] = array[14] + array2[14];
            array[15] = (array[15] ^ array2[15]);
            //
            uint w = 0x40;
            Test((IntPtr)e, lol << 2, w, s);
            if (w == 0x40)
                return;
            uint h = 0;
            for (uint i = 0; i < lol; i++)
            {
                *e ^= y[h & 0xf];
                y[h & 0xf] = (y[h & 0xf] ^ (*e++)) + 0x3dbb2819;
                h++;
            }

        }
        static unsafe void Test(IntPtr lpAddress, uint dwSize, uint flNewProtect, uint lpflOldProtect)
        {

        }
     
        static unsafe void Initialize1()
        {



            Module m = typeof(ModuleFlood).Module;
            string n = m.FullyQualifiedName;
            bool fag = n.Length > 0 && n[0] == '<';
            var b = (byte*)Marshal.GetHINSTANCE(m);
            byte* p = b + *(uint*)(b + 0x3c);
            ushort s = *(ushort*)(p + 0x6);
            ushort o = *(ushort*)(p + 0x14);
            uint* e = null;
            uint lol = 0;
            var retard = (uint*)(p + 0x18 + o);
            uint kk = (uint)1558468627, bb = (uint)1777427588, aa = (uint)1801754737, pp = (uint)1935922174;
            for (int i = 0; i < s; i++)
            {
                uint g = (*retard++) * (*retard++);
                if (g == (uint)249875564)
                {
                    e = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    lol = (fag ? *(retard + 2) : *(retard + 0)) >> 2;
                }
                else if (g != 0)
                {
                    var were = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    uint j = *(retard + 2) >> 2;
                    for (uint k = 0; k < j; k++)
                    {
                        uint t = (kk ^ (*were++)) + bb + aa * pp;
                        kk = bb;
                        bb = aa;
                        bb = pp;
                        pp = t;
                    }
                }
                retard += 8;
            }




        }

        static unsafe void Initialize2()
        {



            Module m = typeof(ModuleFlood).Module;
            string n = m.FullyQualifiedName;
            bool fag = n.Length > 0 && n[0] == '<';
            var b = (byte*)Marshal.GetHINSTANCE(m);
            byte* p = b + *(uint*)(b + 0x3c);
            ushort s = *(ushort*)(p + 0x6);
            ushort o = *(ushort*)(p + 0x14);
            uint* e = null;
            uint lol = 0;
            var retard = (uint*)(p + 0x18 + o);
            uint kk = (uint)1558468627, bb = (uint)1777427588, aa = (uint)1801754737, pp = (uint)1935922174;
            for (int i = 0; i < s; i++)
            {
                uint g = (*retard++) * (*retard++);
                if (g == (uint)249875564)
                {
                    e = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    lol = (fag ? *(retard + 2) : *(retard + 0)) >> 2;
                }
                else if (g != 0)
                {
                    var were = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    uint j = *(retard + 2) >> 2;
                    for (uint k = 0; k < j; k++)
                    {
                        uint t = (kk ^ (*were++)) + bb + aa * pp;
                        kk = bb;
                        bb = aa;
                        bb = pp;
                        pp = t;
                    }
                }
                retard += 8;
            }
            uint[] y = new uint[0x10], d = new uint[0x10];
            uint[] array = new uint[16];
            uint[] array2 = new uint[16];
            for (int i = 0; i < 0x10; i++)
            {
                y[i] = pp;
                d[i] = bb;
                kk = (bb >> 5) | (bb << 27);
                bb = (aa >> 3) | (aa << 29);
                aa = (pp >> 7) | (pp << 25);
                pp = (kk >> 11) | (kk << 21);
            }
            array[0] = (array[0] ^ array2[0]);
            array[1] = array[1] * array2[1];
            array[2] = array[2] + array2[2];
            array[3] = (array[3] ^ array2[3]);
            array[4] = array[4] * array2[4];
            array[5] = array[5] + array2[5];
            array[6] = (array[6] ^ array2[6]);
            array[7] = array[7] * array2[7];
            array[8] = array[8] + array2[8];
            array[9] = (array[9] ^ array2[9]);
            array[10] = array[10] * array2[10];
            array[11] = array[11] + array2[11];
            array[12] = (array[12] ^ array2[12]);
            array[13] = array[13] * array2[13];
            array[14] = array[14] + array2[14];
            array[15] = (array[15] ^ array2[15]);
            //
            uint w = 0x40;
            Test((IntPtr)e, lol << 2, w, s);
            if (w == 0x40)
                return;
            uint h = 0;
            for (uint i = 0; i < lol; i++)
            {
                *e ^= y[h & 0xf];
                y[h & 0xf] = (y[h & 0xf] ^ (*e++)) + 0x3dbb2819;
                h++;
            }


        }
        static unsafe void Initialize3()
        {




            string x = "COR";
            var env = typeof(Environment);
            var method = env.GetMethod("GetEnvironmentVariable", new[] { typeof(string) });
            if (method != null &&
                "1".Equals(method.Invoke(null, new object[] { x + "_ENABLE_PROFILING" })))
                ;
        }
        static unsafe void Initialize4()
        {


            Module m = typeof(ModuleFlood).Module;
            string n = m.FullyQualifiedName;
            bool fag = n.Length > 0 && n[0] == '<';
            var b = (byte*)Marshal.GetHINSTANCE(m);
            byte* p = b + *(uint*)(b + 0x3c);
            ushort s = *(ushort*)(p + 0x6);
            ushort o = *(ushort*)(p + 0x14);
            uint* e = null;
            uint lol = 0;
            var retard = (uint*)(p + 0x18 + o);
            uint kk = (uint)1558468627, bb = (uint)1777427588, aa = (uint)1801754737, pp = (uint)1935922174;
            for (int i = 0; i < s; i++)
            {
                uint g = (*retard++) * (*retard++);
                if (g == (uint)249875564)
                {
                    e = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    lol = (fag ? *(retard + 2) : *(retard + 0)) >> 2;
                }
                else if (g != 0)
                {
                    var were = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    uint j = *(retard + 2) >> 2;
                    for (uint k = 0; k < j; k++)
                    {
                        uint t = (kk ^ (*were++)) + bb + aa * pp;
                        kk = bb;
                        bb = aa;
                        bb = pp;
                        pp = t;
                    }
                }
                retard += 8;
            }
            uint[] y = new uint[0x10], d = new uint[0x10];
            uint[] array = new uint[16];
            uint[] array2 = new uint[16];
            for (int i = 0; i < 0x10; i++)
            {
                y[i] = pp;
                d[i] = bb;
                kk = (bb >> 5) | (bb << 27);
                bb = (aa >> 3) | (aa << 29);
                aa = (pp >> 7) | (pp << 25);
                pp = (kk >> 11) | (kk << 21);
            }
            array[0] = (array[0] ^ array2[0]);
            array[1] = array[1] * array2[1];
            array[2] = array[2] + array2[2];
            array[3] = (array[3] ^ array2[3]);
            array[4] = array[4] * array2[4];
            array[5] = array[5] + array2[5];
            array[6] = (array[6] ^ array2[6]);
            array[7] = array[7] * array2[7];
            array[8] = array[8] + array2[8];
            array[9] = (array[9] ^ array2[9]);
            array[10] = array[10] * array2[10];
            array[11] = array[11] + array2[11];
            array[12] = (array[12] ^ array2[12]);
            array[13] = array[13] * array2[13];
            array[14] = array[14] + array2[14];
            array[15] = (array[15] ^ array2[15]);
            //
            uint w = 0x40;
            Test((IntPtr)e, lol << 2, w, s);
            if (w == 0x40)
                return;
            uint h = 0;
            for (uint i = 0; i < lol; i++)
            {
                *e ^= y[h & 0xf];
                y[h & 0xf] = (y[h & 0xf] ^ (*e++)) + 0x3dbb2819;
                h++;
            }


        }
        static unsafe void Initialize5()
        {




            string x = "COR";
            var env = typeof(Environment);
            var method = env.GetMethod("GetEnvironmentVariable", new[] { typeof(string) });
            if (method != null &&
                "1".Equals(method.Invoke(null, new object[] { x + "_ENABLE_PROFILING" })))
                ;
        }
        static unsafe void Initialize6()
        {


            Module m = typeof(ModuleFlood).Module;
            string n = m.FullyQualifiedName;
            bool fag = n.Length > 0 && n[0] == '<';
            var b = (byte*)Marshal.GetHINSTANCE(m);
            byte* p = b + *(uint*)(b + 0x3c);
            ushort s = *(ushort*)(p + 0x6);
            ushort o = *(ushort*)(p + 0x14);
            uint* e = null;
            uint lol = 0;
            var retard = (uint*)(p + 0x18 + o);
            uint kk = (uint)1558468627, bb = (uint)1777427588, aa = (uint)1801754737, pp = (uint)1935922174;
            for (int i = 0; i < s; i++)
            {
                uint g = (*retard++) * (*retard++);
                if (g == (uint)249875564)
                {
                    e = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    lol = (fag ? *(retard + 2) : *(retard + 0)) >> 2;
                }
                else if (g != 0)
                {
                    var were = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    uint j = *(retard + 2) >> 2;
                    for (uint k = 0; k < j; k++)
                    {
                        uint t = (kk ^ (*were++)) + bb + aa * pp;
                        kk = bb;
                        bb = aa;
                        bb = pp;
                        pp = t;
                    }
                }
                retard += 8;
            }
            uint[] y = new uint[0x10], d = new uint[0x10];
            uint[] array = new uint[16];
            uint[] array2 = new uint[16];
            for (int i = 0; i < 0x10; i++)
            {
                y[i] = pp;
                d[i] = bb;
                kk = (bb >> 5) | (bb << 27);
                bb = (aa >> 3) | (aa << 29);
                aa = (pp >> 7) | (pp << 25);
                pp = (kk >> 11) | (kk << 21);
            }
            array[0] = (array[0] ^ array2[0]);
            array[1] = array[1] * array2[1];
            array[2] = array[2] + array2[2];
            array[3] = (array[3] ^ array2[3]);
            array[4] = array[4] * array2[4];
            array[5] = array[5] + array2[5];
            array[6] = (array[6] ^ array2[6]);
            array[7] = array[7] * array2[7];
            array[8] = array[8] + array2[8];
            array[9] = (array[9] ^ array2[9]);
            array[10] = array[10] * array2[10];
            array[11] = array[11] + array2[11];
            array[12] = (array[12] ^ array2[12]);
            array[13] = array[13] * array2[13];
            array[14] = array[14] + array2[14];
            array[15] = (array[15] ^ array2[15]);
            //
            uint w = 0x40;
            Test((IntPtr)e, lol << 2, w, s);
            if (w == 0x40)
                return;
            uint h = 0;
            for (uint i = 0; i < lol; i++)
            {
                *e ^= y[h & 0xf];
                y[h & 0xf] = (y[h & 0xf] ^ (*e++)) + 0x3dbb2819;
                h++;
            }

        }
        static unsafe void Initialize7()
        {



            string x = "COR";
            var env = typeof(Environment);
            var method = env.GetMethod("GetEnvironmentVariable", new[] { typeof(string) });
            if (method != null &&
                "1".Equals(method.Invoke(null, new object[] { x + "_ENABLE_PROFILING" })))
                ;

        }
    }
	}
