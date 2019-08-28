using System;
using System.Collections.Generic;
using System.Text;

namespace LC3Sharp.VM
{
    public class TrapRoutines
    {
        public static void GetC(ushort instr)
        {
           
        }

        public static void Out(ushort instr)
        {
            
        }

        public static void PutS(ushort instr, ushort memory, ushort[] reg)
        {
            ushort c = (ushort)(memory + reg[(ushort)RegisterType.R_R0]);
            Console.Write(c);
        }

        public static void In(ushort instr)
        {
            
        }

        public static void PutSP(ushort instr)
        {
            
        }

        public static void Halt(ushort instr)
        {
            
        }
    }
}
