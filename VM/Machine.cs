using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
/// <summary>
/// Implementation of the LC3 VM
/// see this link
/// https://justinmeiners.github.io/lc3-vm/#1:1
/// </summary>
namespace LC3Sharp.VM
{
    public class Machine
    {
        private static UInt16[] memory = new UInt16[UInt16.MaxValue];
        private static UInt16[] reg = new UInt16[(int)RegisterType.R_COUNT];

        #region Register operations
        public static UInt16 GetReg(RegisterType registerType)
        {
            throw new NotImplementedException();
        }
        public static void SetReg(RegisterType registerType, Flags flagValue)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Machine Operations
        public static void LoadArgs(string[] args)
        {
            throw new NotFiniteNumberException();
        }
        public static void Shutdown()
        {
            throw new NotImplementedException();
        }
        public static void Setup()
        {
            throw new NotFiniteNumberException();
        }
        public static ushort SignExtend(ushort x, int bit_count)
        {
           
            if (((x >> (bit_count - 1)) & 1) == 1)
            {
                x |=(ushort) (0xFFFF << bit_count);
            }
            return x;
        }
        public static void  UpdateFlags(ushort r)
        {
            if (reg[r] == 0)
            {
                reg[(int)RegisterType.R_COND] = (ushort)ConditionFlags.FL_ZRO;
            }
            else if (reg[r] >> 15 == 1) /* a 1 in the left-most bit indicates negative */
            {
                reg[(int)RegisterType.R_COND] = (ushort)ConditionFlags.FL_NEG;
            }
            else
            {
                reg[(int)RegisterType.R_COND] = (ushort)ConditionFlags.FL_POS;
            }
        }
        public static ushort ReadMemory(int address)
        {
            return memory[address];
        }
        public static void WriteMemory(int address, ushort value)
        {
            memory[address] = value;
        }
        // TODO: Add code to load the file to the memory
        public static void ReadImageFile(FileStream file)
        {

            ushort orign;
            using (BinaryReader br = new BinaryReader(file))
            {
                orign = br.ReadUInt16();
                orign = Swap16(orign);

                ushort max_read = (ushort)(ushort.MaxValue - orign);
                while(br.BaseStream.Position != br.BaseStream.Length)
                {
                    if (orign == max_read)
                        throw new Exception("Trying to write to an unknow location");
                    var cell = br.ReadUInt16();
                    cell = Swap16(orign);
                    memory[++orign] = cell;
                }

            }
        }

        public static int ReadImage(string FileName)
        {
            FileStream fs = File.OpenRead(FileName);
            ReadImageFile(fs);
            return 1;
        }

        public static ushort Swap16(ushort x)
        {
            return(ushort)( (x << 8) | (x >> 8));
        }
        #endregion
        #region OpCodes

        public static void Add(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort r1 = (ushort)(instr >> 6 & 0x7);
            ushort imm_flag = (ushort)(instr >> 5 & 0x1);
            if (imm_flag == 1) {
                ushort imm5 = SignExtend((ushort)(instr & 0x1F), 5);
                reg[r0] = (ushort)(reg[r1] + imm5);
            }
            else
            {
                ushort r2 = (ushort)(instr & 0x7);
                reg[r0] = (ushort)(reg[r1] + reg[r2]);
            }
            UpdateFlags(r0);
        }
        public static void And(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort r1 = (ushort)(instr >> 6 & 0x7);
            ushort imm_flag = (ushort)(instr >> 5 & 0x1);
            if (imm_flag == 1)
            {
                ushort imm5 = SignExtend((ushort)(instr & 0x1F), 5);
                reg[r0] = (ushort)(reg[r1] & imm5);
            }
            else
            {
                ushort r2 = (ushort)(instr & 0x7);
                reg[r0] = (ushort)(reg[r1] & reg[r2]);
            }
            UpdateFlags(r0);
        }
        public static void Not(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort r1 = (ushort)(instr >> 6 & 0x7);
            reg[r0] = (ushort)~reg[r1];
            UpdateFlags(r0);
        }
        // Branch
        public static void Br(ushort instr)
        {
            ushort pc_offset = SignExtend((ushort)(instr & 0x1ff), 9);
            ushort cond_flag = (ushort)(instr >> 9 & 0x7);
            if((cond_flag & reg[(ushort)RegisterType.R_COND]) == 1)
            {
                reg[(ushort)RegisterType.R_PC] += pc_offset;
            }
        }
        public static void Jmp(ushort instr)
        {
            ushort r1 = (ushort)(instr >> 6 & 0x7);
            reg[(ushort)RegisterType.R_PC] = (ushort)reg[r1];
        }
        // Jump Register
        public static void Jsr(ushort instr)
        {
            ushort r1 = (ushort)(instr >> 6 & 0x7);
            ushort long_pc_offset = SignExtend((ushort)(instr & 0x7ff), 11);
            ushort long_flag = (ushort)((instr >> 11) & 1);

            reg[(ushort)RegisterType.R_R7] = reg[(ushort)RegisterType.R_PC];
            if(long_flag == 1)
            {
                reg[(ushort)RegisterType.R_PC] += long_pc_offset;
            }
            else
            {
                reg[(ushort)RegisterType.R_PC] = reg[r1];
            }
        }
        // Load
        public static void Ld(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort pc_offset = SignExtend((ushort)(instr & 0x1ff), 9);
            reg[r0] = ReadMemory(reg[(ushort)RegisterType.R_PC] + pc_offset);
            UpdateFlags(r0);
        }
        public static void BadOpCode()
        {
            throw new NotImplementedException();
        }
        // Load Register
        public static void Ldr(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort r1 = (ushort)(instr >> 6 & 0x7);
            ushort offset = SignExtend((ushort)(instr & 0x3f), 6);
            reg[r0] = ReadMemory(reg[r1] + offset);
            UpdateFlags(r0);
        }
        // load indirect
        public static void Ldi(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort pc_offset = SignExtend((ushort)(instr & 0x1ff),9);
            reg[r0] = ReadMemory(reg[(ushort)RegisterType.R_PC] + pc_offset);
            UpdateFlags(r0);
        }
        // Load Effective Address
        public static void Lea(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort pc_offset = SignExtend((ushort)(instr & 0x1ff), 9);
            reg[r0] = (ushort)(reg[(ushort)RegisterType.R_PC] + pc_offset);
            UpdateFlags(r0);
        }
        // Store
        public static void St(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort pc_offset = SignExtend((ushort)(instr & 0x1ff), 9);
            WriteMemory(reg[(ushort)RegisterType.R_PC] + pc_offset, reg[r0]);

        }
        // Store indirect
        public static void Sti(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort pc_offset = SignExtend((ushort)(instr & 0x1ff), 9);
            WriteMemory(ReadMemory(reg[(ushort)RegisterType.R_PC] + pc_offset), reg[r0]);

        }
        // Store Register
        public static void Str(ushort instr)
        {
            ushort r0 = (ushort)(instr >> 9 & 0x7);
            ushort r1 = (ushort)(instr >> 6 & 0x7);
            ushort offset = SignExtend((ushort)(instr & 0x3f), 6);
            WriteMemory(reg[r1] + offset, reg[r0]);
        }
        // Trap routine
        // TODO: Need to be implemented later 
        public static void Trap(ushort instr)
        {
            ushort memory = 0;
            switch (instr & 0xFF)
            {
                case (ushort)TrapCode.TRAP_GETC:                    
                    TrapRoutines.GetC(instr);
                    break;
                case (ushort)TrapCode.TRAP_OUT:
                    TrapRoutines.Out(instr);
                    break;
                case (ushort)TrapCode.TRAP_PUTS:                   
                    TrapRoutines.PutS(instr, memory, reg);
                    break;
                case (ushort)TrapCode.TRAP_IN:
                    TrapRoutines.In(instr);
                    break;
                case (ushort)TrapCode.TRAP_PUTSP:
                    TrapRoutines.PutSP(instr);
                    break;
                case (ushort)TrapCode.TRAP_HALT:
                    TrapRoutines.Halt(instr);
                    break;
            }
        }
        #endregion
    }

    public enum RegisterType{
        R_R0 = 0,
        R_R1,
        R_R2,
        R_R3,
        R_R4,
        R_R5,
        R_R6,
        R_R7,
        R_PC, /* program counter */
        R_COND,
        R_COUNT

    }

    public enum OpCode {
        OP_BR = 0, /* branch */
        OP_ADD,    /* add  */
        OP_LD,     /* load */
        OP_ST,     /* store */
        OP_JSR,    /* jump register */
        OP_AND,    /* bitwise and */
        OP_LDR,    /* load register */
        OP_STR,    /* store register */
        OP_RTI,    /* unused */
        OP_NOT,    /* bitwise not */
        OP_LDI,    /* load indirect */
        OP_STI,    /* store indirect */
        OP_JMP,    /* jump */
        OP_RES,    /* reserved (unused) */
        OP_LEA,    /* load effective address */
        OP_TRAP    /* execute trap */
    }
    public enum ConditionFlags
    {
        FL_POS = 1 << 0, /* P */
        FL_ZRO = 1 << 1, /* Z */
        FL_NEG = 1 << 2, /* N */
    }
    public enum Flags
    {
        PC_START = 0x3000
    }
    public enum TrapCode
    {
        TRAP_GETC = 0x20,  /* get character from keyboard, not echoed onto the terminal */
        TRAP_OUT = 0x21,   /* output a character */
        TRAP_PUTS = 0x22,  /* output a word string */
        TRAP_IN = 0x23,    /* get character from keyboard, echoed onto the terminal */
        TRAP_PUTSP = 0x24, /* output a byte string */
        TRAP_HALT = 0x25   /* halt the program */
    }
    public enum memoryMappedRegisters
    {
        MR_KBSR = 0xFE00, /* keyboard status */
        MR_KBDR = 0xFE02  /* keyboard data */
    }
}
