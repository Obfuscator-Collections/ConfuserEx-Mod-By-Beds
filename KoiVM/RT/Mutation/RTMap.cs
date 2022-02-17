using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace KoiVM.RT.Mutation
{
	[Obfuscation(Exclude = false, Feature = "+koi;-ref proxy")]
	internal class RTMap
	{
		public static readonly string VMEntry;

		public static readonly string VMRun;

		public static readonly string VMDispatcher;

		public static readonly string VMDispatcherDothrow;

		public static readonly string VMDispatcherThrow;

		public static readonly string VMDispatcherGetIP;

		public static readonly string VMDispatcherStackwalk;

		public static readonly string VMConstants;

		public static readonly Dictionary<string, string> VMConstMap;

		static RTMap()
		{
			VMEntry = "KoiVM.Runtime.VMEntry";
			VMRun = "Run";
			VMDispatcher = "KoiVM.Runtime.Execution.VMDispatcher";
			VMDispatcherDothrow = "DoThrow";
			VMDispatcherThrow = "Throw";
			VMDispatcherGetIP = "GetIP";
			VMDispatcherStackwalk = "StackWalk";
			VMConstants = "KoiVM.Runtime.Dynamic.Constants";
			VMConstMap = new Dictionary<string, string>();
			StringReader reader = new StringReader("\r\nREG_R0\t\t\t\t\t\t\t\tR0\r\nREG_R1\t\t\t\t\t\t\t\tR1\r\nREG_R2\t\t\t\t\t\t\t\tR2\r\nREG_R3\t\t\t\t\t\t\t\tR3\r\nREG_R4\t\t\t\t\t\t\t\tR4\r\nREG_R5\t\t\t\t\t\t\t\tR5\r\nREG_R6\t\t\t\t\t\t\t\tR6\r\nREG_R7\t\t\t\t\t\t\t\tR7\r\nREG_BP\t\t\t\t\t\t\t\tBP\r\nREG_SP\t\t\t\t\t\t\t\tSP\r\nREG_IP\t\t\t\t\t\t\t\tIP\r\nREG_FL\t\t\t\t\t\t\t\tFL\r\nREG_K1\t\t\t\t\t\t\t\tK1\r\nREG_K2\t\t\t\t\t\t\t\tK2\r\nREG_M1\t\t\t\t\t\t\t\tM1\r\nREG_M2\t\t\t\t\t\t\t\tM2\r\nFL_OVERFLOW\t\t\t\t\t\t\tOVERFLOW\r\nFL_CARRY\t\t\t\t\t\t\tCARRY\r\nFL_ZERO\t\t\t\t\t\t\t\tZERO\r\nFL_SIGN\t\t\t\t\t\t\t\tSIGN\r\nFL_UNSIGNED\t\t\t\t\t\t\tUNSIGNED\r\nFL_BEHAV1\t\t\t\t\t\t\tBEHAV1\r\nFL_BEHAV2\t\t\t\t\t\t\tBEHAV2\r\nFL_BEHAV3\t\t\t\t\t\t\tBEHAV3\r\nOP_NOP\t\t\t\t\t\t\t\tNOP\r\nOP_LIND_PTR\t\t\t\t\t\t\tLIND_PTR\r\nOP_LIND_OBJECT\t\t\t\t\t\tLIND_OBJECT\r\nOP_LIND_BYTE\t\t\t\t\t\tLIND_BYTE\r\nOP_LIND_WORD\t\t\t\t\t\tLIND_WORD\r\nOP_LIND_DWORD\t\t\t\t\t\tLIND_DWORD\r\nOP_LIND_QWORD\t\t\t\t\t\tLIND_QWORD\r\nOP_SIND_PTR\t\t\t\t\t\t\tSIND_PTR\r\nOP_SIND_OBJECT\t\t\t\t\t\tSIND_OBJECT\r\nOP_SIND_BYTE\t\t\t\t\t\tSIND_BYTE\r\nOP_SIND_WORD\t\t\t\t\t\tSIND_WORD\r\nOP_SIND_DWORD\t\t\t\t\t\tSIND_DWORD\r\nOP_SIND_QWORD\t\t\t\t\t\tSIND_QWORD\r\nOP_POP\t\t\t\t\t\t\t\tPOP\r\nOP_PUSHR_OBJECT\t\t\t\t\t\tPUSHR_OBJECT\r\nOP_PUSHR_BYTE\t\t\t\t\t\tPUSHR_BYTE\r\nOP_PUSHR_WORD\t\t\t\t\t\tPUSHR_WORD\r\nOP_PUSHR_DWORD\t\t\t\t\t\tPUSHR_DWORD\r\nOP_PUSHR_QWORD\t\t\t\t\t\tPUSHR_QWORD\r\nOP_PUSHI_DWORD\t\t\t\t\t\tPUSHI_DWORD\r\nOP_PUSHI_QWORD\t\t\t\t\t\tPUSHI_QWORD\r\nOP_SX_BYTE\t\t\t\t\t\t\tSX_BYTE\r\nOP_SX_WORD\t\t\t\t\t\t\tSX_WORD\r\nOP_SX_DWORD\t\t\t\t\t\t\tSX_DWORD\r\nOP_CALL\t\t\t\t\t\t\t\tCALL\r\nOP_RET\t\t\t\t\t\t\t\tRET\r\nOP_NOR_DWORD\t\t\t\t\t\tNOR_DWORD\r\nOP_NOR_QWORD\t\t\t\t\t\tNOR_QWORD\r\nOP_CMP\t\t\t\t\t\t\t\tCMP\r\nOP_CMP_DWORD\t\t\t\t\t\tCMP_DWORD\r\nOP_CMP_QWORD\t\t\t\t\t\tCMP_QWORD\r\nOP_CMP_R32\t\t\t\t\t\t\tCMP_R32\r\nOP_CMP_R64\t\t\t\t\t\t\tCMP_R64\r\nOP_JZ\t\t\t\t\t\t\t\tJZ\r\nOP_JNZ\t\t\t\t\t\t\t\tJNZ\r\nOP_JMP\t\t\t\t\t\t\t\tJMP\r\nOP_SWT\t\t\t\t\t\t\t\tSWT\r\nOP_ADD_DWORD\t\t\t\t\t\tADD_DWORD\r\nOP_ADD_QWORD\t\t\t\t\t\tADD_QWORD\r\nOP_ADD_R32\t\t\t\t\t\t\tADD_R32\r\nOP_ADD_R64\t\t\t\t\t\t\tADD_R64\r\nOP_SUB_R32\t\t\t\t\t\t\tSUB_R32\r\nOP_SUB_R64\t\t\t\t\t\t\tSUB_R64\r\nOP_MUL_DWORD\t\t\t\t\t\tMUL_DWORD\r\nOP_MUL_QWORD\t\t\t\t\t\tMUL_QWORD\r\nOP_MUL_R32\t\t\t\t\t\t\tMUL_R32\r\nOP_MUL_R64\t\t\t\t\t\t\tMUL_R64\r\nOP_DIV_DWORD\t\t\t\t\t\tDIV_DWORD\r\nOP_DIV_QWORD\t\t\t\t\t\tDIV_QWORD\r\nOP_DIV_R32\t\t\t\t\t\t\tDIV_R32\r\nOP_DIV_R64\t\t\t\t\t\t\tDIV_R64\r\nOP_REM_DWORD\t\t\t\t\t\tREM_DWORD\r\nOP_REM_QWORD\t\t\t\t\t\tREM_QWORD\r\nOP_REM_R32\t\t\t\t\t\t\tREM_R32\r\nOP_REM_R64\t\t\t\t\t\t\tREM_R64\r\nOP_SHR_DWORD\t\t\t\t\t\tSHR_DWORD\r\nOP_SHR_QWORD\t\t\t\t\t\tSHR_QWORD\r\nOP_SHL_DWORD\t\t\t\t\t\tSHL_DWORD\r\nOP_SHL_QWORD\t\t\t\t\t\tSHL_QWORD\r\nOP_FCONV_R32_R64\t\t\t\t\tFCONV_R32_R64\r\nOP_FCONV_R64_R32\t\t\t\t\tFCONV_R64_R32\r\nOP_FCONV_R32\t\t\t\t\t\tFCONV_R32\r\nOP_FCONV_R64\t\t\t\t\t\tFCONV_R64\r\nOP_ICONV_PTR\t\t\t\t\t\tICONV_PTR\r\nOP_ICONV_R64\t\t\t\t\t\tICONV_R64\r\nOP_VCALL\t\t\t\t\t\t\tVCALL\r\nOP_TRY\t\t\t\t\t\t\t\tTRY\r\nOP_LEAVE\t\t\t\t\t\t\tLEAVE\r\nVCALL_EXIT\t\t\t\t\t\t\tEXIT\r\nVCALL_BREAK\t\t\t\t\t\t\tBREAK\r\nVCALL_ECALL\t\t\t\t\t\t\tECALL\r\nVCALL_CAST\t\t\t\t\t\t\tCAST\r\nVCALL_CKFINITE\t\t\t\t\t\tCKFINITE\r\nVCALL_CKOVERFLOW\t\t\t\t\tCKOVERFLOW\r\nVCALL_RANGECHK\t\t\t\t\t\tRANGECHK\r\nVCALL_INITOBJ\t\t\t\t\t\tINITOBJ\r\nVCALL_LDFLD\t\t\t\t\t\t\tLDFLD\r\nVCALL_LDFTN\t\t\t\t\t\t\tLDFTN\r\nVCALL_TOKEN\t\t\t\t\t\t\tTOKEN\r\nVCALL_THROW\t\t\t\t\t\t\tTHROW\r\nVCALL_SIZEOF\t\t\t\t\t\tSIZEOF\r\nVCALL_STFLD\t\t\t\t\t\t\tSTFLD\r\nVCALL_BOX\t\t\t\t\t\t\tBOX\r\nVCALL_UNBOX\t\t\t\t\t\t\tUNBOX\r\nVCALL_LOCALLOC\t\t\t\t\t\tLOCALLOC\r\nHELPER_INIT\t\t\t\t\t\t\tINIT\r\nECALL_CALL\t\t\t\t\t\t\tE_CALL\r\nECALL_CALLVIRT\t\t\t\t\t\tE_CALLVIRT\r\nECALL_NEWOBJ\t\t\t\t\t\tE_NEWOBJ\r\nECALL_CALLVIRT_CONSTRAINED\t\t\tE_CALLVIRT_CONSTRAINED\r\nFLAG_INSTANCE\t\t\t\t\t\tINSTANCE\r\nEH_CATCH\t\t\t\t\t\t\tCATCH\r\nEH_FILTER\t\t\t\t\t\t\tFILTER\r\nEH_FAULT\t\t\t\t\t\t\tFAULT\r\nEH_FINALLY\t\t\t\t\t\t\tFINALLY\r\n");
			while (reader.Peek() > 0)
			{
				string line = reader.ReadLine().Trim();
				if (!string.IsNullOrEmpty(line))
				{
					string[] entry = line.Split(new char[1] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
					VMConstMap[entry[1]] = entry[0];
				}
			}
		}
	}
}
