using LC3Sharp.VM;
using System;

namespace LC3Sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Machine.LoadArgs(args);
            Machine.Setup();
            Machine.SetReg(RegisterType.R_PC, Flags.PC_START);

            var running = true;

            while (running)
            {
                ushort shiftAddress = 12;

                ushort nuAddress = Machine.GetReg(RegisterType.R_PC);
                ushort instr = MemoryRead(nuAddress++);
                ushort op = (UInt16)(instr >> shiftAddress);

                switch (op)
                {
                    case (ushort)OpCode.OP_ADD:
                        Machine.Add(instr);
                        break;

                    case (ushort)OpCode.OP_AND:
                        Machine.And(instr);
                        break;

                    case (ushort)OpCode.OP_NOT:
                        Machine.Not(instr);
                        break;

                    case (ushort)OpCode.OP_BR:
                        Machine.Br(instr);
                        break;

                    case (ushort)OpCode.OP_JMP:
                        Machine.Jmp(instr);
                        break;

                    case (ushort)OpCode.OP_JSR:
                        Machine.Jsr(instr);
                        break;

                    case (ushort)OpCode.OP_LD:
                        Machine.Ld(instr);
                        break;

                    case (ushort)OpCode.OP_LDI:
                        Machine.Ldi(instr);
                        break;

                    case (ushort)OpCode.OP_LDR:
                        Machine.Ldr(instr);
                        break;

                    case (ushort)OpCode.OP_LEA:
                        Machine.Lea(instr);
                        break;

                    case (ushort)OpCode.OP_ST:
                        Machine.St(instr);
                        break;

                    case (ushort)OpCode.OP_STI:
                        Machine.Sti(instr);
                        break;

                    case (ushort)OpCode.OP_STR:
                        Machine.Str(instr);
                        break;
                    case (ushort)OpCode.OP_TRAP:
                        Machine.Trap(instr);
                        break;

                    case (ushort)OpCode.OP_RES:
                    case (ushort)OpCode.OP_RTI:
                    default:
                        Machine.BadOpCode();
                        break;
                }
            }

            Machine.Shutdown();
        }

        private static UInt16 MemoryRead(UInt16 value)
        {
            return Machine.ReadMemory(value);
        }
    }
}
