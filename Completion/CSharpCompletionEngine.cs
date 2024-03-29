// 
// AlCompletionEngine.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc. (http://xamarin.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.Al.Refactoring;
using ICSharpCode.NRefactory.Al.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Al.TypeSystem;
using System.IO;

namespace ICSharpCode.NRefactory.Al.Completion
{
	public enum EditorBrowsableBehavior
	{
		Ignore,
		Normal,
		IncludeAdvanced
	}

	public class CompletionEngineCache
	{
		public List<INamespace>  namespaces;
		public ICompletionData[] importCompletion;
	}

	public class AlCompletionEngine : AlCompletionEngineBase
	{
        public static Dictionary<string, string> AsmKeyWords = null;
        public static void LoadAsmKeyWords()
        {
            AsmKeyWords = new Dictionary<string, string>();
            AsmKeyWords.Add("rax", "64 bits register");
            AsmKeyWords.Add("rbx", "64 bits register");
            AsmKeyWords.Add("rcx", "64 bits register");
            AsmKeyWords.Add("rdx", "64 bits register");
            AsmKeyWords.Add("rdi", "64 bits register");
            AsmKeyWords.Add("rsi", "64 bits register");
            AsmKeyWords.Add("rsp", "64 bits register");
            AsmKeyWords.Add("rbp", "64 bits register");
            AsmKeyWords.Add("eax", "32 bits register");
            AsmKeyWords.Add("ebx", "32 bits register");
            AsmKeyWords.Add("ecx", "32 bits register");
            AsmKeyWords.Add("edx", "32 bits register");
            AsmKeyWords.Add("ax", "16 bits register");
            AsmKeyWords.Add("bx", "16 bits register");
            AsmKeyWords.Add("cx", "16 bits register");
            AsmKeyWords.Add("dx", "16 bits register");
            AsmKeyWords.Add("ah", "8 bits register");
            AsmKeyWords.Add("al", "8 bits register");
            AsmKeyWords.Add("bh", "8 bits register");
            AsmKeyWords.Add("bl", "8 bits register");
            AsmKeyWords.Add("ch", "8 bits register");
            AsmKeyWords.Add("cl", "8 bits register");
            AsmKeyWords.Add("dh", "8 bits register");
            AsmKeyWords.Add("dl", "8 bits register");
            AsmKeyWords.Add("edi", "32 bits register");
            AsmKeyWords.Add("esi", "32 bits register");
            AsmKeyWords.Add("esp", "32 bits register");
            AsmKeyWords.Add("ebp", "32 bits register");
            AsmKeyWords.Add("di", "16 bits register");
            AsmKeyWords.Add("si", "16 bits register");

            AsmKeyWords.Add("aaa", "ASCII Adjust After Addition");
            AsmKeyWords.Add("aad", "ASCII Adjust AX Before Division");
            AsmKeyWords.Add("aam", "ASCII Adjust AX After Multiply");
            AsmKeyWords.Add("aas", "ASCII Adjust AL After Subtraction");
            AsmKeyWords.Add("adc", "Add with Carry");
            AsmKeyWords.Add("adcx", "Unsigned Integer Addition of Two Operands with Carry Flag");
            AsmKeyWords.Add("add", "Add");
            AsmKeyWords.Add("addpd", "Add Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("addps", "Add Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("addsd", "Add Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("addss", "Add Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("addsubpd", "Packed Double-FP Add/Subtract");
            AsmKeyWords.Add("addsubps", "Packed Single-FP Add/Subtract");
            AsmKeyWords.Add("adox", "Unsigned Integer Addition of Two Operands with Overflow Flag");
            AsmKeyWords.Add("aesdec", "Perform One Round of an AES Decryption Flow");
            AsmKeyWords.Add("aesdeclast", "Perform Last Round of an AES Decryption Flow");
            AsmKeyWords.Add("aesenc", "Perform One Round of an AES Encryption Flow");
            AsmKeyWords.Add("aesenclast", "Perform Last Round of an AES Encryption Flow");
            AsmKeyWords.Add("aesimc", "Perform the AES InvMixColumn Transformation");
            AsmKeyWords.Add("aeskeygenassist", "AES Round Key Generation Assist");
            AsmKeyWords.Add("and", "Logical AND");
            AsmKeyWords.Add("andn", "Logical AND NOT");
            AsmKeyWords.Add("andnpd", "Bitwise Logical AND NOT of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("andnps", "Bitwise Logical AND NOT of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("andpd", "Bitwise Logical AND of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("andps", "Bitwise Logical AND of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("arpl", "Adjust RPL Field of Segment Selector");
            AsmKeyWords.Add("bextr", "Bit Field Extract");
            AsmKeyWords.Add("blendpd", "Blend Packed Double Precision Floating-Point Values");
            AsmKeyWords.Add("blendps", "Blend Packed Single Precision Floating-Point Values");
            AsmKeyWords.Add("blendvpd", "Variable Blend Packed Double Precision Floating-Point Values");
            AsmKeyWords.Add("blendvps", "Variable Blend Packed Single Precision Floating-Point Values");
            AsmKeyWords.Add("blsi", "Extract Lowest Set Isolated Bit");
            AsmKeyWords.Add("blsmsk", "Get Mask Up to Lowest Set Bit");
            AsmKeyWords.Add("blsr", "Reset Lowest Set Bit");
            AsmKeyWords.Add("bound", "Check Array Index Against Bounds");
            AsmKeyWords.Add("bsf", "Bit Scan Forward");
            AsmKeyWords.Add("bsr", "Bit Scan Reverse");
            AsmKeyWords.Add("bswap", "Byte Swap");
            AsmKeyWords.Add("bt", "Bit Test");
            AsmKeyWords.Add("btc", "Bit Test and Complement");
            AsmKeyWords.Add("btr", "Bit Test and Reset");
            AsmKeyWords.Add("bts", "Bit Test and Set");
            AsmKeyWords.Add("bzhi", "Zero High Bits Starting with Specified Bit Position");
            AsmKeyWords.Add("call", "Call Procedure");
            AsmKeyWords.Add("cbw", "Convert Byte to Word/Convert Word to Doubleword/Convert Doubleword to Quadword");
            AsmKeyWords.Add("cdq", "Convert Word to Doubleword/Convert Doubleword to Quadword");
            AsmKeyWords.Add("cdqe", "Convert Byte to Word/Convert Word to Doubleword/Convert Doubleword to Quadword");
            AsmKeyWords.Add("clac", "Clear AC Flag in EFLAGS Register");
            AsmKeyWords.Add("clc", "Clear Carry Flag");
            AsmKeyWords.Add("cld", "Clear Direction Flag");
            AsmKeyWords.Add("clflush", "Flush Cache Line");
            AsmKeyWords.Add("cli", "Clear Interrupt Flag");
            AsmKeyWords.Add("clts", "Clear Task-Switched Flag in CR0");
            AsmKeyWords.Add("cmc", "Complement Carry Flag");
            AsmKeyWords.Add("cmovcc", "Conditional Move");
            AsmKeyWords.Add("cmp", "Compare Two Operands");
            AsmKeyWords.Add("cmppd", "Compare Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("cmpps", "Compare Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("cmps", "Compare String Operands");
            AsmKeyWords.Add("cmpsb", "Compare String Operands");
            AsmKeyWords.Add("cmpsd", "Compare Scalar Double-Precision Floating-Point Values/Compare String Operands");
            AsmKeyWords.Add("cmpsq", "Compare String Operands");
            AsmKeyWords.Add("cmpss", "Compare Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("cmpsw", "Compare String Operands");
            AsmKeyWords.Add("cmpxchg", "Compare and Exchange");
            AsmKeyWords.Add("cmpxchg16b", "Compare and Exchange Bytes");
            AsmKeyWords.Add("cmpxchg8b", "Compare and Exchange Bytes");
            AsmKeyWords.Add("comisd", "Compare Scalar Ordered Double-Precision Floating-Point Values and Set EFLAGS");
            AsmKeyWords.Add("comiss", "Compare Scalar Ordered Single-Precision Floating-Point Values and Set EFLAGS");
            AsmKeyWords.Add("cpuid", "CPU Identification");
            AsmKeyWords.Add("cqo", "Convert Word to Doubleword/Convert Doubleword to Quadword");
            AsmKeyWords.Add("crc32", "Accumulate CRC32 Value");
            AsmKeyWords.Add("cvtdq2pd", "Convert Packed Dword Integers to Packed Double-Precision FP Values");
            AsmKeyWords.Add("cvtdq2ps", "Convert Packed Dword Integers to Packed Single-Precision FP Values");
            AsmKeyWords.Add("cvtpd2dq", "Convert Packed Double-Precision FP Values to Packed Dword Integers");
            AsmKeyWords.Add("cvtpd2pi", "Convert Packed Double-Precision FP Values to Packed Dword Integers");
            AsmKeyWords.Add("cvtpd2ps", "Convert Packed Double-Precision FP Values to Packed Single-Precision FP Values");
            AsmKeyWords.Add("cvtpi2pd", "Convert Packed Dword Integers to Packed Double-Precision FP Values");
            AsmKeyWords.Add("cvtpi2ps", "Convert Packed Dword Integers to Packed Single-Precision FP Values");
            AsmKeyWords.Add("cvtps2dq", "Convert Packed Single-Precision FP Values to Packed Dword Integers");
            AsmKeyWords.Add("cvtps2pd", "Convert Packed Single-Precision FP Values to Packed Double-Precision FP Values");
            AsmKeyWords.Add("cvtps2pi", "Convert Packed Single-Precision FP Values to Packed Dword Integers");
            AsmKeyWords.Add("cvtsd2si", "Convert Scalar Double-Precision FP Value to Integer");
            AsmKeyWords.Add("cvtsd2ss", "Convert Scalar Double-Precision FP Value to Scalar Single-Precision FP Value");
            AsmKeyWords.Add("cvtsi2sd", "Convert Dword Integer to Scalar Double-Precision FP Value");
            AsmKeyWords.Add("cvtsi2ss", "Convert Dword Integer to Scalar Single-Precision FP Value");
            AsmKeyWords.Add("cvtss2sd", "Convert Scalar Single-Precision FP Value to Scalar Double-Precision FP Value");
            AsmKeyWords.Add("cvtss2si", "Convert Scalar Single-Precision FP Value to Dword Integer");
            AsmKeyWords.Add("cvttpd2dq", "Convert with Truncation Packed Double-Precision FP Values to Packed Dword Integers");
            AsmKeyWords.Add("cvttpd2pi", "Convert with Truncation Packed Double-Precision FP Values to Packed Dword Integers");
            AsmKeyWords.Add("cvttps2dq", "Convert with Truncation Packed Single-Precision FP Values to Packed Dword Integers");
            AsmKeyWords.Add("cvttps2pi", "Convert with Truncation Packed Single-Precision FP Values to Packed Dword Integers");
            AsmKeyWords.Add("cvttsd2si", "Convert with Truncation Scalar Double-Precision FP Value to Signed Integer");
            AsmKeyWords.Add("cvttss2si", "Convert with Truncation Scalar Single-Precision FP Value to Dword Integer");
            AsmKeyWords.Add("cwd", "Convert Word to Doubleword/Convert Doubleword to Quadword");
            AsmKeyWords.Add("cwde", "Convert Byte to Word/Convert Word to Doubleword/Convert Doubleword to Quadword");
            AsmKeyWords.Add("daa", "Decimal Adjust AL after Addition");
            AsmKeyWords.Add("das", "Decimal Adjust AL after Subtraction");
            AsmKeyWords.Add("dec", "Decrement by 1");
            AsmKeyWords.Add("div", "Unsigned Divide");
            AsmKeyWords.Add("divpd", "Divide Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("divps", "Divide Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("divsd", "Divide Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("divss", "Divide Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("dppd", "Dot Product of Packed Double Precision Floating-Point Values");
            AsmKeyWords.Add("dpps", "Dot Product of Packed Single Precision Floating-Point Values");
            AsmKeyWords.Add("emms", "Empty MMX Technology State");
            AsmKeyWords.Add("enter", "Make Stack Frame for Procedure Parameters");
            AsmKeyWords.Add("extractps", "Extract Packed Single Precision Floating-Point Value");
            AsmKeyWords.Add("f2xm1", "Compute 2x–1");
            AsmKeyWords.Add("fabs", "Absolute Value");
            AsmKeyWords.Add("fadd", "Add");
            AsmKeyWords.Add("faddp", "Add");
            AsmKeyWords.Add("fbld", "Load Binary Coded Decimal");
            AsmKeyWords.Add("fbstp", "Store BCD Integer and Pop");
            AsmKeyWords.Add("fchs", "Change Sign");
            AsmKeyWords.Add("fclex", "Clear Exceptions");
            AsmKeyWords.Add("fcmovcc", "Floating-Point Conditional Move");
            AsmKeyWords.Add("fcom", "Compare Floating Point Values");
            AsmKeyWords.Add("fcomi", "Compare Floating Point Values and Set EFLAGS");
            AsmKeyWords.Add("fcomip", "Compare Floating Point Values and Set EFLAGS");
            AsmKeyWords.Add("fcomp", "Compare Floating Point Values");
            AsmKeyWords.Add("fcompp", "Compare Floating Point Values");
            AsmKeyWords.Add("fcos", "Cosine");
            AsmKeyWords.Add("fdecstp", "Decrement Stack-Top Pointer");
            AsmKeyWords.Add("fdiv", "Divide");
            AsmKeyWords.Add("fdivp", "Divide");
            AsmKeyWords.Add("fdivr", "Reverse Divide");
            AsmKeyWords.Add("fdivrp", "Reverse Divide");
            AsmKeyWords.Add("ffree", "Free Floating-Point Register");
            AsmKeyWords.Add("fiadd", "Add");
            AsmKeyWords.Add("ficom", "Compare Integer");
            AsmKeyWords.Add("ficomp", "Compare Integer");
            AsmKeyWords.Add("fidiv", "Divide");
            AsmKeyWords.Add("fidivr", "Reverse Divide");
            AsmKeyWords.Add("fild", "Load Integer");
            AsmKeyWords.Add("fimul", "Multiply");
            AsmKeyWords.Add("fincstp", "Increment Stack-Top Pointer");
            AsmKeyWords.Add("finit", "Initialize Floating-Point Unit");
            AsmKeyWords.Add("fist", "Store Integer");
            AsmKeyWords.Add("fistp", "Store Integer");
            AsmKeyWords.Add("fisttp", "Store Integer with Truncation");
            AsmKeyWords.Add("fisub", "Subtract");
            AsmKeyWords.Add("fisubr", "Reverse Subtract");
            AsmKeyWords.Add("fld", "Load Floating Point Value");
            AsmKeyWords.Add("fld1", "Load Constant");
            AsmKeyWords.Add("fldcw", "Load x87 FPU Control Word");
            AsmKeyWords.Add("fldenv", "Load x87 FPU Environment");
            AsmKeyWords.Add("fldl2e", "Load Constant");
            AsmKeyWords.Add("fldl2t", "Load Constant");
            AsmKeyWords.Add("fldlg2", "Load Constant");
            AsmKeyWords.Add("fldln2", "Load Constant");
            AsmKeyWords.Add("fldpi", "Load Constant");
            AsmKeyWords.Add("fldz", "Load Constant");
            AsmKeyWords.Add("fmul", "Multiply");
            AsmKeyWords.Add("fmulp", "Multiply");
            AsmKeyWords.Add("fnclex", "Clear Exceptions");
            AsmKeyWords.Add("fninit", "Initialize Floating-Point Unit");
            AsmKeyWords.Add("fnop", "No Operation");
            AsmKeyWords.Add("fnsave", "Store x87 FPU State");
            AsmKeyWords.Add("fnstcw", "Store x87 FPU Control Word");
            AsmKeyWords.Add("fnstenv", "Store x87 FPU Environment");
            AsmKeyWords.Add("fnstsw", "Store x87 FPU Status Word");
            AsmKeyWords.Add("fpatan", "Partial Arctangent");
            AsmKeyWords.Add("fprem", "Partial Remainder");
            AsmKeyWords.Add("fprem1", "Partial Remainder");
            AsmKeyWords.Add("fptan", "Partial Tangent");
            AsmKeyWords.Add("frndint", "Round to Integer");
            AsmKeyWords.Add("frstor", "Restore x87 FPU State");
            AsmKeyWords.Add("fsave", "Store x87 FPU State");
            AsmKeyWords.Add("fscale", "Scale");
            AsmKeyWords.Add("fsin", "Sine");
            AsmKeyWords.Add("fsincos", "Sine and Cosine");
            AsmKeyWords.Add("fsqrt", "Square Root");
            AsmKeyWords.Add("fst", "Store Floating Point Value");
            AsmKeyWords.Add("fstcw", "Store x87 FPU Control Word");
            AsmKeyWords.Add("fstenv", "Store x87 FPU Environment");
            AsmKeyWords.Add("fstp", "Store Floating Point Value");
            AsmKeyWords.Add("fstsw", "Store x87 FPU Status Word");
            AsmKeyWords.Add("fsub", "Subtract");
            AsmKeyWords.Add("fsubp", "Subtract");
            AsmKeyWords.Add("fsubr", "Reverse Subtract");
            AsmKeyWords.Add("fsubrp", "Reverse Subtract");
            AsmKeyWords.Add("ftst", "TEST");
            AsmKeyWords.Add("fucom", "Unordered Compare Floating Point Values");
            AsmKeyWords.Add("fucomi", "Compare Floating Point Values and Set EFLAGS");
            AsmKeyWords.Add("fucomip", "Compare Floating Point Values and Set EFLAGS");
            AsmKeyWords.Add("fucomp", "Unordered Compare Floating Point Values");
            AsmKeyWords.Add("fucompp", "Unordered Compare Floating Point Values");
            AsmKeyWords.Add("fwait", "Wait");
            AsmKeyWords.Add("fxam", "Examine ModR/M");
            AsmKeyWords.Add("fxch", "Exchange Register Contents");
            AsmKeyWords.Add("fxrstor", "Restore x87 FPU, MMX, XMM, and MXCSR State");
            AsmKeyWords.Add("fxsave", "Save x87 FPU, MMX Technology, and SSE State");
            AsmKeyWords.Add("fxtract", "Extract Exponent and Significand");
            AsmKeyWords.Add("fyl2x", "Compute y ∗ log2x");
            AsmKeyWords.Add("fyl2xp1", "Compute y ∗ log2(x +1)");
            AsmKeyWords.Add("haddpd", "Packed Double-FP Horizontal Add");
            AsmKeyWords.Add("haddps", "Packed Single-FP Horizontal Add");
            AsmKeyWords.Add("hlt", "Halt");
            AsmKeyWords.Add("hsubpd", "Packed Double-FP Horizontal Subtract");
            AsmKeyWords.Add("hsubps", "Packed Single-FP Horizontal Subtract");
            AsmKeyWords.Add("idiv", "Signed Divide");
            AsmKeyWords.Add("imul", "Signed Multiply");
            AsmKeyWords.Add("in", "Input from Port");
            AsmKeyWords.Add("inc", "Increment by 1");
            AsmKeyWords.Add("ins", "Input from Port to String");
            AsmKeyWords.Add("insb", "Input from Port to String");
            AsmKeyWords.Add("insd", "Input from Port to String");
            AsmKeyWords.Add("insertps", "Insert Packed Single Precision Floating-Point Value");
            AsmKeyWords.Add("insw", "Input from Port to String");
            AsmKeyWords.Add("int 3", "Call to Interrupt Procedure");
            AsmKeyWords.Add("int n", "Call to Interrupt Procedure");
            AsmKeyWords.Add("into", "Call to Interrupt Procedure");
            AsmKeyWords.Add("invd", "Invalidate Internal Caches");
            AsmKeyWords.Add("invlpg", "Invalidate TLB Entries");
            AsmKeyWords.Add("invpcid", "Invalidate Process-Context Identifier");
            AsmKeyWords.Add("iret", "Interrupt Return");
            AsmKeyWords.Add("iretd", "Interrupt Return");
            AsmKeyWords.Add("jmp", "Jump");
            AsmKeyWords.Add("jcc", "Jump if Condition Is Met");
            AsmKeyWords.Add("lahf", "Load Status Flags into AH Register");
            AsmKeyWords.Add("lar", "Load Access Rights Byte");
            AsmKeyWords.Add("lddqu", "Load Unaligned Integer 128 Bits");
            AsmKeyWords.Add("ldmxcsr", "Load MXCSR Register");
            AsmKeyWords.Add("lds", "Load Far Pointer");
            AsmKeyWords.Add("lea", "Load Effective Address");
            AsmKeyWords.Add("leave", "High Level Procedure Exit");
            AsmKeyWords.Add("les", "Load Far Pointer");
            AsmKeyWords.Add("lfence", "Load Fence");
            AsmKeyWords.Add("lfs", "Load Far Pointer");
            AsmKeyWords.Add("lgdt", "Load Global/Interrupt Descriptor Table Register");
            AsmKeyWords.Add("lgs", "Load Far Pointer");
            AsmKeyWords.Add("lidt", "Load Global/Interrupt Descriptor Table Register");
            AsmKeyWords.Add("lldt", "Load Local Descriptor Table Register");
            AsmKeyWords.Add("lmsw", "Load Machine Status Word");
            AsmKeyWords.Add("lock", "Assert LOCK# Signal Prefix");
            AsmKeyWords.Add("lods", "Load String");
            AsmKeyWords.Add("lodsb", "Load String");
            AsmKeyWords.Add("lodsd", "Load String");
            AsmKeyWords.Add("lodsq", "Load String");
            AsmKeyWords.Add("lodsw", "Load String");
            AsmKeyWords.Add("loop", "Loop According to ECX Counter");
            AsmKeyWords.Add("loopcc", "Loop According to ECX Counter");
            AsmKeyWords.Add("lsl", "Load Segment Limit");
            AsmKeyWords.Add("lss", "Load Far Pointer");
            AsmKeyWords.Add("ltr", "Load Task Register");
            AsmKeyWords.Add("lzcnt", "Count the Number of Leading Zero Bits");
            AsmKeyWords.Add("maskmovdqu", "Store Selected Bytes of Double Quadword");
            AsmKeyWords.Add("maskmovq", "Store Selected Bytes of Quadword");
            AsmKeyWords.Add("maxpd", "Return Maximum Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("maxps", "Return Maximum Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("maxsd", "Return Maximum Scalar Double-Precision Floating-Point Value");
            AsmKeyWords.Add("maxss", "Return Maximum Scalar Single-Precision Floating-Point Value");
            AsmKeyWords.Add("mfence", "Memory Fence");
            AsmKeyWords.Add("minpd", "Return Minimum Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("minps", "Return Minimum Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("minsd", "Return Minimum Scalar Double-Precision Floating-Point Value");
            AsmKeyWords.Add("minss", "Return Minimum Scalar Single-Precision Floating-Point Value");
            AsmKeyWords.Add("monitor", "Set Up Monitor Address");
            AsmKeyWords.Add("mov", "Move Instruction");
            AsmKeyWords.Add("movapd", "Move Aligned Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("movaps", "Move Aligned Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("movbe", "Move Data After Swapping Bytes");
            AsmKeyWords.Add("movd", "Move Doubleword/Move Quadword");
            AsmKeyWords.Add("movddup", "Move One Double-FP and Duplicate");
            AsmKeyWords.Add("movdq2q", "Move Quadword from XMM to MMX Technology Register");
            AsmKeyWords.Add("movdqa", "Move Aligned Double Quadword");
            AsmKeyWords.Add("movdqu", "Move Unaligned Double Quadword");
            AsmKeyWords.Add("movhlps", "Move Packed Single-Precision Floating-Point Values High to Low");
            AsmKeyWords.Add("movhpd", "Move High Packed Double-Precision Floating-Point Value");
            AsmKeyWords.Add("movhps", "Move High Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("movlhps", "Move Packed Single-Precision Floating-Point Values Low to High");
            AsmKeyWords.Add("movlpd", "Move Low Packed Double-Precision Floating-Point Value");
            AsmKeyWords.Add("movlps", "Move Low Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("movmskpd", "Extract Packed Double-Precision Floating-Point Sign Mask");
            AsmKeyWords.Add("movmskps", "Extract Packed Single-Precision Floating-Point Sign Mask");
            AsmKeyWords.Add("movntdq", "Store Double Quadword Using Non-Temporal Hint");
            AsmKeyWords.Add("movntdqa", "Load Double Quadword Non-Temporal Aligned Hint");
            AsmKeyWords.Add("movnti", "Store Doubleword Using Non-Temporal Hint");
            AsmKeyWords.Add("movntpd", "Store Packed Double-Precision Floating-Point Values Using Non-Temporal Hint");
            AsmKeyWords.Add("movntps", "Store Packed Single-Precision Floating-Point Values Using Non-Temporal Hint");
            AsmKeyWords.Add("movntq", "Store of Quadword Using Non-Temporal Hint");
            AsmKeyWords.Add("movq", "Move Doubleword/Move Quadword");
            AsmKeyWords.Add("movq2dq", "Move Quadword from MMX Technology to XMM Register");
            AsmKeyWords.Add("movs", "Move Data from String to String");
            AsmKeyWords.Add("movsb", "Move Data from String to String");
            AsmKeyWords.Add("movsd", "Move Scalar Double-Precision Floating-Point Value. Move Data from String to String");
            AsmKeyWords.Add("movshdup", "Move Packed Single-FP High and Duplicate");
            AsmKeyWords.Add("movsldup", "Move Packed Single-FP Low and Duplicate");
            AsmKeyWords.Add("movsq", "Move Data from String to String");
            AsmKeyWords.Add("movss", "Move Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("movsw", "Move Data from String to String");
            AsmKeyWords.Add("movsx", "Move with Sign-Extension");
            AsmKeyWords.Add("movsxd", "Move with Sign-Extension");
            AsmKeyWords.Add("movupd", "Move Unaligned Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("movups", "Move Unaligned Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("movzx", "Move with Zero-Extend");
            AsmKeyWords.Add("mpsadbw", "Compute Multiple Packed Sums of Absolute Difference");
            AsmKeyWords.Add("mul", "Unsigned Multiply");
            AsmKeyWords.Add("mulpd", "Multiply Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("mulps", "Multiply Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("mulsd", "Multiply Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("mulss", "Multiply Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("mwait", "Monitor Wait");
            AsmKeyWords.Add("mulx", "Unsigned Multiply Without Affecting Flags");
            AsmKeyWords.Add("neg", "Two's Complement Negation");
            AsmKeyWords.Add("nop", "No Operation");
            AsmKeyWords.Add("not", "One's Complement Negation");
            AsmKeyWords.Add("or", "Logical Inclusive OR");
            AsmKeyWords.Add("orpd", "Bitwise Logical OR of Double-Precision Floating-Point Values");
            AsmKeyWords.Add("orps", "Bitwise Logical OR of Single-Precision Floating-Point Values");
            AsmKeyWords.Add("out", "Output to Port");
            AsmKeyWords.Add("outs", "Output String to Port");
            AsmKeyWords.Add("outsb", "Output String to Port");
            AsmKeyWords.Add("outsd", "Output String to Port");
            AsmKeyWords.Add("outsw", "Output String to Port");
            AsmKeyWords.Add("pabsb", "Packed Absolute Value");
            AsmKeyWords.Add("pabsd", "Packed Absolute Value");
            AsmKeyWords.Add("pabsw", "Packed Absolute Value");
            AsmKeyWords.Add("packssdw", "Pack with Signed Saturation");
            AsmKeyWords.Add("packsswb", "Pack with Signed Saturation");
            AsmKeyWords.Add("packusdw", "Pack with Unsigned Saturation");
            AsmKeyWords.Add("packuswb", "Pack with Unsigned Saturation");
            AsmKeyWords.Add("paddb", "Add Packed Integers");
            AsmKeyWords.Add("paddd", "Add Packed Integers");
            AsmKeyWords.Add("paddq", "Add Packed Quadword Integers");
            AsmKeyWords.Add("paddsb", "Add Packed Signed Integers with Signed Saturation");
            AsmKeyWords.Add("paddsw", "Add Packed Signed Integers with Signed Saturation");
            AsmKeyWords.Add("paddusb", "Add Packed Unsigned Integers with Unsigned Saturation");
            AsmKeyWords.Add("paddusw", "Add Packed Unsigned Integers with Unsigned Saturation");
            AsmKeyWords.Add("paddw", "Add Packed Integers");
            AsmKeyWords.Add("palignr", "Packed Align Right");
            AsmKeyWords.Add("pand", "Logical AND");
            AsmKeyWords.Add("pandn", "Logical AND NOT");
            AsmKeyWords.Add("pause", "Spin Loop Hint");
            AsmKeyWords.Add("pavgb", "Average Packed Integers");
            AsmKeyWords.Add("pavgw", "Average Packed Integers");
            AsmKeyWords.Add("pblendvb", "Variable Blend Packed Bytes");
            AsmKeyWords.Add("pblendw", "Blend Packed Words");
            AsmKeyWords.Add("pclmulqdq", "Carry-Less Multiplication Quadword");
            AsmKeyWords.Add("pcmpeqb", "Compare Packed Data for Equal");
            AsmKeyWords.Add("pcmpeqd", "Compare Packed Data for Equal");
            AsmKeyWords.Add("pcmpeqq", "Compare Packed Qword Data for Equal");
            AsmKeyWords.Add("pcmpeqw", "Compare Packed Data for Equal");
            AsmKeyWords.Add("pcmpestri", "Packed Compare Explicit Length Strings, Return Index");
            AsmKeyWords.Add("pcmpestrm", "Packed Compare Explicit Length Strings, Return Mask");
            AsmKeyWords.Add("pcmpgtb", "Compare Packed Signed Integers for Greater Than");
            AsmKeyWords.Add("pcmpgtd", "Compare Packed Signed Integers for Greater Than");
            AsmKeyWords.Add("pcmpgtq", "Compare Packed Data for Greater Than");
            AsmKeyWords.Add("pcmpgtw", "Compare Packed Signed Integers for Greater Than");
            AsmKeyWords.Add("pcmpistri", "Packed Compare Implicit Length Strings, Return Index");
            AsmKeyWords.Add("pcmpistrm", "Packed Compare Implicit Length Strings, Return Mask");
            AsmKeyWords.Add("pdep", "Parallel Bits Deposit");
            AsmKeyWords.Add("pext", "Parallel Bits Extract");
            AsmKeyWords.Add("pextrb", "Extract Byte/Dword/Qword");
            AsmKeyWords.Add("pextrd", "Extract Byte/Dword/Qword");
            AsmKeyWords.Add("pextrq", "Extract Byte/Dword/Qword");
            AsmKeyWords.Add("pextrw", "Extract Word");
            AsmKeyWords.Add("phaddd", "Packed Horizontal Add");
            AsmKeyWords.Add("phaddsw", "Packed Horizontal Add and Saturate");
            AsmKeyWords.Add("phaddw", "Packed Horizontal Add");
            AsmKeyWords.Add("phminposuw", "Packed Horizontal Word Minimum");
            AsmKeyWords.Add("phsubd", "Packed Horizontal Subtract");
            AsmKeyWords.Add("phsubsw", "Packed Horizontal Subtract and Saturate");
            AsmKeyWords.Add("phsubw", "Packed Horizontal Subtract");
            AsmKeyWords.Add("pinsrb", "Insert Byte/Dword/Qword");
            AsmKeyWords.Add("pinsrd", "Insert Byte/Dword/Qword");
            AsmKeyWords.Add("pinsrq", "Insert Byte/Dword/Qword");
            AsmKeyWords.Add("pinsrw", "Insert Word");
            AsmKeyWords.Add("pmaddubsw", "Multiply and Add Packed Signed and Unsigned Bytes");
            AsmKeyWords.Add("pmaddwd", "Multiply and Add Packed Integers");
            AsmKeyWords.Add("pmaxsb", "Maximum of Packed Signed Byte Integers");
            AsmKeyWords.Add("pmaxsd", "Maximum of Packed Signed Dword Integers");
            AsmKeyWords.Add("pmaxsw", "Maximum of Packed Signed Word Integers");
            AsmKeyWords.Add("pmaxub", "Maximum of Packed Unsigned Byte Integers");
            AsmKeyWords.Add("pmaxud", "Maximum of Packed Unsigned Dword Integers");
            AsmKeyWords.Add("pmaxuw", "Maximum of Packed Word Integers");
            AsmKeyWords.Add("pminsb", "Minimum of Packed Signed Byte Integers");
            AsmKeyWords.Add("pminsd", "Minimum of Packed Dword Integers");
            AsmKeyWords.Add("pminsw", "Minimum of Packed Signed Word Integers");
            AsmKeyWords.Add("pminub", "Minimum of Packed Unsigned Byte Integers");
            AsmKeyWords.Add("pminud", "Minimum of Packed Dword Integers");
            AsmKeyWords.Add("pminuw", "Minimum of Packed Word Integers");
            AsmKeyWords.Add("pmovmskb", "Move Byte Mask");
            AsmKeyWords.Add("pmovsx", "Packed Move with Sign Extend");
            AsmKeyWords.Add("pmovzx", "Packed Move with Zero Extend");
            AsmKeyWords.Add("pmuldq", "Multiply Packed Signed Dword Integers");
            AsmKeyWords.Add("pmulhrsw", "Packed Multiply High with Round and Scale");
            AsmKeyWords.Add("pmulhuw", "Multiply Packed Unsigned Integers and Store High Result");
            AsmKeyWords.Add("pmulhw", "Multiply Packed Signed Integers and Store High Result");
            AsmKeyWords.Add("pmulld", "Multiply Packed Signed Dword Integers and Store Low Result");
            AsmKeyWords.Add("pmullw", "Multiply Packed Signed Integers and Store Low Result");
            AsmKeyWords.Add("pmuludq", "Multiply Packed Unsigned Doubleword Integers");
            AsmKeyWords.Add("pop", "Pop a Value from the Stack");
            AsmKeyWords.Add("popa", "Pop All General-Purpose Registers");
            AsmKeyWords.Add("popad", "Pop All General-Purpose Registers");
            AsmKeyWords.Add("popcnt", "Return the Count of Number of Bits Set to 1");
            AsmKeyWords.Add("popf", "Pop Stack into EFLAGS Register");
            AsmKeyWords.Add("popfd", "Pop Stack into EFLAGS Register");
            AsmKeyWords.Add("popfq", "Pop Stack into EFLAGS Register");
            AsmKeyWords.Add("por", "Bitwise Logical OR");
            AsmKeyWords.Add("prefetchw", "Prefetch Data into Caches in Anticipation of a Write");
            AsmKeyWords.Add("prefetchwt1", "Prefetch Vector Data Into Caches with Intent to Write and T1 Hint");
            AsmKeyWords.Add("prefetchh", "Prefetch Data Into Caches");
            AsmKeyWords.Add("psadbw", "Compute Sum of Absolute Differences");
            AsmKeyWords.Add("pshufb", "Packed Shuffle Bytes");
            AsmKeyWords.Add("pshufd", "Shuffle Packed Doublewords");
            AsmKeyWords.Add("pshufhw", "Shuffle Packed High Words");
            AsmKeyWords.Add("pshuflw", "Shuffle Packed Low Words");
            AsmKeyWords.Add("pshufw", "Shuffle Packed Words");
            AsmKeyWords.Add("psignb", "Packed SIGN");
            AsmKeyWords.Add("psignd", "Packed SIGN");
            AsmKeyWords.Add("psignw", "Packed SIGN");
            AsmKeyWords.Add("pslld", "Shift Packed Data Left Logical");
            AsmKeyWords.Add("pslldq", "Shift Double Quadword Left Logical");
            AsmKeyWords.Add("psllq", "Shift Packed Data Left Logical");
            AsmKeyWords.Add("psllw", "Shift Packed Data Left Logical");
            AsmKeyWords.Add("psrad", "Shift Packed Data Right Arithmetic");
            AsmKeyWords.Add("psraw", "Shift Packed Data Right Arithmetic");
            AsmKeyWords.Add("psrld", "Shift Packed Data Right Logical");
            AsmKeyWords.Add("psrldq", "Shift Double Quadword Right Logical");
            AsmKeyWords.Add("psrlq", "Shift Packed Data Right Logical");
            AsmKeyWords.Add("psrlw", "Shift Packed Data Right Logical");
            AsmKeyWords.Add("psubb", "Subtract Packed Integers");
            AsmKeyWords.Add("psubd", "Subtract Packed Integers");
            AsmKeyWords.Add("psubq", "Subtract Packed Quadword Integers");
            AsmKeyWords.Add("psubsb", "Subtract Packed Signed Integers with Signed Saturation");
            AsmKeyWords.Add("psubsw", "Subtract Packed Signed Integers with Signed Saturation");
            AsmKeyWords.Add("psubusb", "Subtract Packed Unsigned Integers with Unsigned Saturation");
            AsmKeyWords.Add("psubusw", "Subtract Packed Unsigned Integers with Unsigned Saturation");
            AsmKeyWords.Add("psubw", "Subtract Packed Integers");
            AsmKeyWords.Add("ptest", "Logical Compare");
            AsmKeyWords.Add("punpckhbw", "Unpack High Data");
            AsmKeyWords.Add("punpckhdq", "Unpack High Data");
            AsmKeyWords.Add("punpckhqdq", "Unpack High Data");
            AsmKeyWords.Add("punpckhwd", "Unpack High Data");
            AsmKeyWords.Add("punpcklbw", "Unpack Low Data");
            AsmKeyWords.Add("punpckldq", "Unpack Low Data");
            AsmKeyWords.Add("punpcklqdq", "Unpack Low Data");
            AsmKeyWords.Add("punpcklwd", "Unpack Low Data");
            AsmKeyWords.Add("push", "Push Word, Doubleword or Quadword Onto the Stack");
            AsmKeyWords.Add("pusha", "Push All General-Purpose Registers");
            AsmKeyWords.Add("pushad", "Push All General-Purpose Registers");
            AsmKeyWords.Add("pushf", "Push EFLAGS Register onto the Stack");
            AsmKeyWords.Add("pushfd", "Push EFLAGS Register onto the Stack");
            AsmKeyWords.Add("pxor", "Logical Exclusive OR");
            AsmKeyWords.Add("rcl", "—Rotate");
            AsmKeyWords.Add("rcpps", "Compute Reciprocals of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("rcpss", "Compute Reciprocal of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("rcr", "—Rotate");
            AsmKeyWords.Add("rdfsbase", "Read FS/GS Segment Base");
            AsmKeyWords.Add("rdgsbase", "Read FS/GS Segment Base");
            AsmKeyWords.Add("rdmsr", "Read from Model Specific Register");
            AsmKeyWords.Add("rdpmc", "Read Performance-Monitoring Counters");
            AsmKeyWords.Add("rdrand", "Read Random Number");
            AsmKeyWords.Add("rdseed", "Read Random SEED");
            AsmKeyWords.Add("rdtsc", "Read Time-Stamp Counter");
            AsmKeyWords.Add("rdtscp", "Read Time-Stamp Counter and Processor ID");
            AsmKeyWords.Add("rep", "Repeat String Operation Prefix");
            AsmKeyWords.Add("repe", "Repeat String Operation Prefix");
            AsmKeyWords.Add("repne", "Repeat String Operation Prefix");
            AsmKeyWords.Add("repnz", "Repeat String Operation Prefix");
            AsmKeyWords.Add("repz", "Repeat String Operation Prefix");
            AsmKeyWords.Add("ret", "Return from Procedure");
            AsmKeyWords.Add("rol", "—Rotate");
            AsmKeyWords.Add("ror", "—Rotate");
            AsmKeyWords.Add("rorx", "Rotate Right Logical Without Affecting Flags");
            AsmKeyWords.Add("roundpd", "Round Packed Double Precision Floating-Point Values");
            AsmKeyWords.Add("roundps", "Round Packed Single Precision Floating-Point Values");
            AsmKeyWords.Add("roundsd", "Round Scalar Double Precision Floating-Point Values");
            AsmKeyWords.Add("roundss", "Round Scalar Single Precision Floating-Point Values");
            AsmKeyWords.Add("rsm", "Resume from System Management Mode");
            AsmKeyWords.Add("rsqrtps", "Compute Reciprocals of Square Roots of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("rsqrtss", "Compute Reciprocal of Square Root of Scalar Single-Precision Floating-Point Value");
            AsmKeyWords.Add("sahf", "Store AH into Flags");
            AsmKeyWords.Add("sal", "Shift");
            AsmKeyWords.Add("sar", "Shift");
            AsmKeyWords.Add("sarx", "Shift Without Affecting Flags");
            AsmKeyWords.Add("sbb", "Integer Subtraction with Borrow");
            AsmKeyWords.Add("scas", "Scan String");
            AsmKeyWords.Add("scasb", "Scan String");
            AsmKeyWords.Add("scasd", "Scan String");
            AsmKeyWords.Add("scasw", "Scan String");
            AsmKeyWords.Add("setcc", "Set Byte on Condition");
            AsmKeyWords.Add("sfence", "Store Fence");
            AsmKeyWords.Add("sgdt", "Store Global Descriptor Table Register");
            AsmKeyWords.Add("shl", "Shift");
            AsmKeyWords.Add("shld", "Double Precision Shift Left");
            AsmKeyWords.Add("shlx", "Shift Without Affecting Flags");
            AsmKeyWords.Add("shr", "Shift");
            AsmKeyWords.Add("shrd", "Double Precision Shift Right");
            AsmKeyWords.Add("shrx", "Shift Without Affecting Flags");
            AsmKeyWords.Add("shufpd", "Shuffle Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("shufps", "Shuffle Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("sidt", "Store Interrupt Descriptor Table Register");
            AsmKeyWords.Add("sldt", "Store Local Descriptor Table Register");
            AsmKeyWords.Add("smsw", "Store Machine Status Word");
            AsmKeyWords.Add("sqrtpd", "Compute Square Roots of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("sqrtps", "Compute Square Roots of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("sqrtsd", "Compute Square Root of Scalar Double-Precision Floating-Point Value");
            AsmKeyWords.Add("sqrtss", "Compute Square Root of Scalar Single-Precision Floating-Point Value");
            AsmKeyWords.Add("stac", "Set AC Flag in EFLAGS Register");
            AsmKeyWords.Add("stc", "Set Carry Flag");
            AsmKeyWords.Add("std", "Set Direction Flag");
            AsmKeyWords.Add("sti", "Set Interrupt Flag");
            AsmKeyWords.Add("stmxcsr", "Store MXCSR Register State");
            AsmKeyWords.Add("stos", "Store String");
            AsmKeyWords.Add("stosb", "Store String");
            AsmKeyWords.Add("stosd", "Store String");
            AsmKeyWords.Add("stosq", "Store String");
            AsmKeyWords.Add("stosw", "Store String");
            AsmKeyWords.Add("str", "Store Task Register");
            AsmKeyWords.Add("sub", "Subtract");
            AsmKeyWords.Add("subpd", "Subtract Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("subps", "Subtract Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("subsd", "Subtract Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("subss", "Subtract Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("swapgs", "Swap GS Base Register");
            AsmKeyWords.Add("syscall", "Fast System Call");
            AsmKeyWords.Add("sysenter", "Fast System Call");
            AsmKeyWords.Add("sysexit", "Fast Return from Fast System Call");
            AsmKeyWords.Add("sysret", "Return From Fast System Call");
            AsmKeyWords.Add("test", "Logical Compare");
            AsmKeyWords.Add("tzcnt", "Count the Number of Trailing Zero Bits");
            AsmKeyWords.Add("ucomisd", "Unordered Compare Scalar Double-Precision Floating-Point Values and Set EFLAGS");
            AsmKeyWords.Add("ucomiss", "Unordered Compare Scalar Single-Precision Floating-Point Values and Set EFLAGS");
            AsmKeyWords.Add("ud2", "Undefined Instruction");
            AsmKeyWords.Add("unpckhpd", "Unpack and Interleave High Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("unpckhps", "Unpack and Interleave High Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("unpcklpd", "Unpack and Interleave Low Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("unpcklps", "Unpack and Interleave Low Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vbroadcast", "Broadcast Floating-Point Data");
            AsmKeyWords.Add("vcvtph2ps", "Convert 16-bit FP Values to Single-Precision FP Values");
            AsmKeyWords.Add("vcvtps2ph", "Convert Single-Precision FP value to 16-bit FP value");
            AsmKeyWords.Add("verr", "Verify a Segment for Reading or Writing");
            AsmKeyWords.Add("verw", "Verify a Segment for Reading or Writing");
            AsmKeyWords.Add("vextractf128", "Extract Packed Floating-Point Values");
            AsmKeyWords.Add("vextracti128", "Extract packed Integer Values");
            AsmKeyWords.Add("vfmadd132pd", "Fused Multiply-Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd132ps", "Fused Multiply-Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd132sd", "Fused Multiply-Add of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd132ss", "Fused Multiply-Add of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd213pd", "Fused Multiply-Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd213ps", "Fused Multiply-Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd213sd", "Fused Multiply-Add of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd213ss", "Fused Multiply-Add of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd231pd", "Fused Multiply-Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd231ps", "Fused Multiply-Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd231sd", "Fused Multiply-Add of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmadd231ss", "Fused Multiply-Add of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmaddsub132pd", "Fused Multiply-Alternating Add/Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmaddsub132ps", "Fused Multiply-Alternating Add/Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmaddsub213pd", "Fused Multiply-Alternating Add/Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmaddsub213ps", "Fused Multiply-Alternating Add/Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmaddsub231pd", "Fused Multiply-Alternating Add/Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmaddsub231ps", "Fused Multiply-Alternating Add/Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub132pd", "Fused Multiply-Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub132ps", "Fused Multiply-Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub132sd", "Fused Multiply-Subtract of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub132ss", "Fused Multiply-Subtract of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub213pd", "Fused Multiply-Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub213ps", "Fused Multiply-Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub213sd", "Fused Multiply-Subtract of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub213ss", "Fused Multiply-Subtract of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub231pd", "Fused Multiply-Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub231ps", "Fused Multiply-Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub231sd", "Fused Multiply-Subtract of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsub231ss", "Fused Multiply-Subtract of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsubadd132pd", "Fused Multiply-Alternating Subtract/Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsubadd132ps", "Fused Multiply-Alternating Subtract/Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsubadd213pd", "Fused Multiply-Alternating Subtract/Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsubadd213ps", "Fused Multiply-Alternating Subtract/Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsubadd231pd", "Fused Multiply-Alternating Subtract/Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfmsubadd231ps", "Fused Multiply-Alternating Subtract/Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd132pd", "Fused Negative Multiply-Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd132ps", "Fused Negative Multiply-Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd132sd", "Fused Negative Multiply-Add of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd132ss", "Fused Negative Multiply-Add of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd213pd", "Fused Negative Multiply-Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd213ps", "Fused Negative Multiply-Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd213sd", "Fused Negative Multiply-Add of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd213ss", "Fused Negative Multiply-Add of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd231pd", "Fused Negative Multiply-Add of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd231ps", "Fused Negative Multiply-Add of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd231sd", "Fused Negative Multiply-Add of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmadd231ss", "Fused Negative Multiply-Add of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub132pd", "Fused Negative Multiply-Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub132ps", "Fused Negative Multiply-Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub132sd", "Fused Negative Multiply-Subtract of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub132ss", "Fused Negative Multiply-Subtract of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub213pd", "Fused Negative Multiply-Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub213ps", "Fused Negative Multiply-Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub213sd", "Fused Negative Multiply-Subtract of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub213ss", "Fused Negative Multiply-Subtract of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub231pd", "Fused Negative Multiply-Subtract of Packed Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub231ps", "Fused Negative Multiply-Subtract of Packed Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub231sd", "Fused Negative Multiply-Subtract of Scalar Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vfnmsub231ss", "Fused Negative Multiply-Subtract of Scalar Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vgatherdpd", "Gather Packed DP FP Values Using Signed Dword/Qword Indices");
            AsmKeyWords.Add("vgatherdps", "Gather Packed SP FP values Using Signed Dword/Qword Indices");
            AsmKeyWords.Add("vgatherqpd", "Gather Packed DP FP Values Using Signed Dword/Qword Indices");
            AsmKeyWords.Add("vgatherqps", "Gather Packed SP FP values Using Signed Dword/Qword Indices");
            AsmKeyWords.Add("vinsertf128", "Insert Packed Floating-Point Values");
            AsmKeyWords.Add("vinserti128", "Insert Packed Integer Values");
            AsmKeyWords.Add("vmaskmov", "Conditional SIMD Packed Loads and Stores");
            AsmKeyWords.Add("vpblendd", "Blend Packed Dwords");
            AsmKeyWords.Add("vpbroadcast", "Broadcast Integer Data");
            AsmKeyWords.Add("vperm2f128", "Permute Floating-Point Values");
            AsmKeyWords.Add("vperm2i128", "Permute Integer Values");
            AsmKeyWords.Add("vpermd", "Full Doublewords Element Permutation");
            AsmKeyWords.Add("vpermilpd", "Permute Double-Precision Floating-Point Values");
            AsmKeyWords.Add("vpermilps", "Permute Single-Precision Floating-Point Values");
            AsmKeyWords.Add("vpermpd", "Permute Double-Precision Floating-Point Elements");
            AsmKeyWords.Add("vpermps", "Permute Single-Precision Floating-Point Elements");
            AsmKeyWords.Add("vpermq", "Qwords Element Permutation");
            AsmKeyWords.Add("vpgatherdd", "Gather Packed Dword Values Using Signed Dword/Qword Indices");
            AsmKeyWords.Add("vpgatherdq", "Gather Packed Qword Values Using Signed Dword/Qword Indices");
            AsmKeyWords.Add("vpgatherqd", "Gather Packed Dword Values Using Signed Dword/Qword Indices");
            AsmKeyWords.Add("vpgatherqq", "Gather Packed Qword Values Using Signed Dword/Qword Indices");
            AsmKeyWords.Add("vpmaskmov", "Conditional SIMD Integer Packed Loads and Stores");
            AsmKeyWords.Add("vpsllvd", "Variable Bit Shift Left Logical");
            AsmKeyWords.Add("vpsllvq", "Variable Bit Shift Left Logical");
            AsmKeyWords.Add("vpsravd", "Variable Bit Shift Right Arithmetic");
            AsmKeyWords.Add("vpsrlvd", "Variable Bit Shift Right Logical");
            AsmKeyWords.Add("vpsrlvq", "Variable Bit Shift Right Logical");
            AsmKeyWords.Add("vtestpd", "Packed Bit Test");
            AsmKeyWords.Add("vtestps", "Packed Bit Test");
            AsmKeyWords.Add("vzeroall", "Zero All YMM Registers");
            AsmKeyWords.Add("vzeroupper", "Zero Upper Bits of YMM Registers");
            AsmKeyWords.Add("wait", "Wait");
            AsmKeyWords.Add("wbinvd", "Write Back and Invalidate Cache");
            AsmKeyWords.Add("wrfsbase", "Write FS/GS Segment Base");
            AsmKeyWords.Add("wrgsbase", "Write FS/GS Segment Base");
            AsmKeyWords.Add("wrmsr", "Write to Model Specific Register");
            AsmKeyWords.Add("xabort", "Transactional Abort");
            AsmKeyWords.Add("xacquire", "Hardware Lock Elision Prefix Hints");
            AsmKeyWords.Add("xadd", "Exchange and Add");
            AsmKeyWords.Add("xbegin", "Transactional Begin");
            AsmKeyWords.Add("xchg", "Exchange Register/Memory with Register");
            AsmKeyWords.Add("xend", "Transactional End");
            AsmKeyWords.Add("xgetbv", "Get Value of Extended Control Register");
            AsmKeyWords.Add("xlat", "Table Look-up Translation");
            AsmKeyWords.Add("xlatb", "Table Look-up Translation");
            AsmKeyWords.Add("xor", "Logical Exclusive OR");
            AsmKeyWords.Add("xorpd", "Bitwise Logical XOR for Double-Precision Floating-Point Values");
            AsmKeyWords.Add("xorps", "Bitwise Logical XOR for Single-Precision Floating-Point Values");
            AsmKeyWords.Add("xrelease", "Hardware Lock Elision Prefix Hints");
            AsmKeyWords.Add("xrstor", "Restore Processor Extended States");
            AsmKeyWords.Add("xrstors", "Restore Processor Extended States Supervisor");
            AsmKeyWords.Add("xsave", "Save Processor Extended States");
            AsmKeyWords.Add("xsavec", "Save Processor Extended States with Compaction");
            AsmKeyWords.Add("xsaveopt", "Save Processor Extended States Optimized");
            AsmKeyWords.Add("xsaves", "Save Processor Extended States Supervisor");
            AsmKeyWords.Add("xsetbv", "Set Extended Control Register");
            AsmKeyWords.Add("xtest", "Test If In Transactional Execution");


        }
		internal ICompletionDataFactory factory;

		#region Additional input properties

		public AlFormattingOptions FormattingPolicy { get; set; }

		public string EolMarker { get; set; }

		public string IndentString { get; set; }

		public bool AutomaticallyAddImports { get; set; }

		public bool IncludeKeywordsInCompletionList { get; set; }

		public EditorBrowsableBehavior EditorBrowsableBehavior { get; set; }

		public CompletionEngineCache CompletionEngineCache { get; set; }

		#endregion

		#region Result properties

		public bool AutoCompleteEmptyMatch;
		/// <summary>
		/// The auto complete empty match on curly bracket. (only taken into account when AutoCompleteEmptyMatch is true )
		/// </summary>
		public bool AutoCompleteEmptyMatchOnCurlyBracket = true;
		public bool AutoSelect;
		public string DefaultCompletionString;
		public bool CloseOnSquareBrackets;
		public readonly List<IMethod> PossibleDelegates = new List<IMethod>();

		#endregion

		public AlCompletionEngine(IDocument document, ICompletionContextProvider completionContextProvider, ICompletionDataFactory factory, IProjectContent content, AlTypeResolveContext ctx) : base(content, completionContextProvider, ctx)
		{
            if (AsmKeyWords == null)
                LoadAsmKeyWords();

			if (document == null) {
				throw new ArgumentNullException("document");
			}
			if (factory == null) {
				throw new ArgumentNullException("factory");
			}
			this.document = document;
			this.factory = factory;
			// Set defaults for additional input properties
			this.FormattingPolicy = FormattingOptionsFactory.CreateMono();
			this.EolMarker = Environment.NewLine;
			this.IncludeKeywordsInCompletionList = true;
			EditorBrowsableBehavior = EditorBrowsableBehavior.IncludeAdvanced;
			this.IndentString = "\t";
       
           
		}

		public bool TryGetCompletionWord(int offset, out int startPos, out int wordLength)
		{
			startPos = wordLength = 0;
			int pos = offset - 1;
			while (pos >= 0) {
				char c = document.GetCharAt(pos);
				if (!char.IsLetterOrDigit(c) && c != '_')
					break;
				pos--;
			}
			if (pos == -1)
				return false;

			pos++;
			startPos = pos;

			while (pos < document.TextLength) {
				char c = document.GetCharAt(pos);
				if (!char.IsLetterOrDigit(c) && c != '_')
					break;
				pos++;
			}
			wordLength = pos - startPos;
			return true;
		}
     /*
        static AlParser parser = new AlParser();
      static  IUnresolvedFile BaseTypeFile;
        public static void LoadBaseType(IProjectContent prj)
        {
            try
            {
                if (BaseTypeFile == null)
                {
                    var source = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "ALBASE.al");
                    var asyntaxTree = parser.Parse(source, AppDomain.CurrentDomain.BaseDirectory + "ALBASE.al");
                    asyntaxTree.Freeze();
                    BaseTypeFile = asyntaxTree.ToTypeSystem();
                }
                prj.AddOrUpdateFiles(BaseTypeFile);
            }
            catch
            {
              
                BaseTypeFile = null;
            }
        }
        public void AddBaseType()
        {
            try
            {
                if (BaseTypeFile != null && ctx.CurrentTypeDefinition != null)
                {
                    
             
                    ICSharpCode.NRefactory.TypeSystem.Implementation.DefaultResolvedTypeDefinition drtd = (ICSharpCode.NRefactory.TypeSystem.Implementation.DefaultResolvedTypeDefinition)ctx.CurrentTypeDefinition;
                    List<IUnresolvedTypeDefinition> Parts = new List<IUnresolvedTypeDefinition>();
                    Parts.AddRange(drtd.Parts.ToArray());
                    List<IUnresolvedTypeDefinition> d = BaseTypeFile.GetAllTypeDefinitions().ToList();
                    Parts.Add(d[0]);
                    ctx.CurrentTypeDefinition = new ICSharpCode.NRefactory.TypeSystem.Implementation.DefaultResolvedTypeDefinition(drtd.parentContext, Parts.ToArray());

                }
            }
            catch
            {

            }
        }

        */
		public IEnumerable<ICompletionData> GetCompletionData(int offset, bool controlSpace)
		{

            AddBaseType();
			this.AutoCompleteEmptyMatch = true;
			this.AutoSelect = true;
			this.DefaultCompletionString = null;
			SetOffset(offset);
			if (offset > 0) {
				char lastChar = document.GetCharAt(offset - 1);
				bool isComplete = false;
				var result = MagicKeyCompletion(lastChar, controlSpace, out isComplete) ?? Enumerable.Empty<ICompletionData>();
				if (!isComplete && controlSpace && char.IsWhiteSpace(lastChar)) {
					offset -= 2;
					while (offset >= 0 && char.IsWhiteSpace(document.GetCharAt(offset))) {
						offset--;
					}
					if (offset > 0) {
						var nonWsResult = MagicKeyCompletion(
							document.GetCharAt(offset),
							controlSpace,
							out isComplete
						);
						if (nonWsResult != null) {
							var text = new HashSet<string>(result.Select(r => r.CompletionText));
							result = result.Concat(nonWsResult.Where(r => !text.Contains(r.CompletionText)));
						}
					}
				}


                //List<ICompletionData> ls = result.ToList();
                //if (ls.Count > 2)
                //{


                //    ls.Add(factory.CreatePredefinedCompletionData("asm", "public T asm(string asmcode,string return_var, string return_type, params object[] args);", "Inline assembly", null));
                //    return ls.AsEnumerable();
                //}
				return result;
			}
			return Enumerable.Empty<ICompletionData>();
		}



		/// <summary>
		/// Gets the types that needs to be imported via using or full type name.
		/// </summary>
		public IEnumerable<ICompletionData> GetImportCompletionData(int offset)
		{
			var generalLookup = new MemberLookup(null, Compilation.MainAssembly);
			SetOffset(offset);

			// flatten usings
			var namespaces = new List<INamespace>();
			for (var n = ctx.CurrentUsingScope; n != null; n = n.Parent) {
				namespaces.Add(n.Namespace);
				foreach (var u in n.Usings)
					namespaces.Add(u);
			}

			foreach (var type in Compilation.GetAllTypeDefinitions ()) {
				if (!generalLookup.IsAccessible(type, false))
					continue;	
				if (namespaces.Any(n => n.FullName == type.Namespace))
					continue;
				bool useFullName = false;
				foreach (var ns in namespaces) {
					if (ns.GetTypeDefinition(type.Name, type.TypeParameterCount) != null) {
						useFullName = true;
						break;
					}
				}
				yield return factory.CreateImportCompletionData(type, useFullName, false);
			}
		}

		IEnumerable<string> GenerateNameProposals(AstType type)
		{
			if (type is PrimitiveType) {
				var pt = (PrimitiveType)type;
				switch (pt.Keyword) {
					case "object":
						yield return "o";
						yield return "obj";
						break;
					case "bool":
						yield return "b";
						yield return "pred";
						break;
					case "real":
					case "float":
					case "decimal":
						yield return "d";
						yield return "f";
						yield return "m";
                        yield return "r";
						break;
					default:
						yield return "i";
						yield return "j";
						yield return "k";
						break;
				}
				yield break;
			}
			string name;
			if (type is SimpleType) {
				name = ((SimpleType)type).Identifier;
			} else if (type is MemberType) {
				name = ((MemberType)type).MemberName;
			} else {
				yield break;
			}

			var names = WordParser.BreakWords(name);

			var possibleName = new StringBuilder();
			for (int i = 0; i < names.Count; i++) {
				possibleName.Length = 0;
				for (int j = i; j < names.Count; j++) {
					if (string.IsNullOrEmpty(names [j])) {
						continue;
					}
					if (j == i) { 
						names [j] = Char.ToLower(names [j] [0]) + names [j].Substring(1);
					}
					possibleName.Append(names [j]);
				}
				yield return possibleName.ToString();
			}
		}

		IEnumerable<ICompletionData> HandleMemberReferenceCompletion(ExpressionResult expr)
		{
			if (expr == null)
				return null;

			// do not auto select <number>. (but <number>.<number>.) (0.ToString() is valid)
			if (expr.Node is PrimitiveExpression) {
				var pexpr = (PrimitiveExpression)expr.Node;
				if (!(pexpr.Value is string || pexpr.Value is char) && !pexpr.LiteralValue.Contains('.')) {
					AutoSelect = false;
				}
			}
			var resolveResult = ResolveExpression(expr);

			if (resolveResult == null) {
				return null;
			}
			if (expr.Node is AstType) {

				// check for namespace names
				if (expr.Node.AncestorsAndSelf
					.TakeWhile(n => n is AstType)
					.Any(m => m.Role == NamespaceDeclaration.NamespaceNameRole))
					return null;

				// need to look at paren.parent because of "catch (<Type>.A" expression
				if (expr.Node.Parent != null && expr.Node.Parent.Parent is CatchClause)
					return HandleCatchClauseType(expr);
				return CreateTypeAndNamespaceCompletionData(
					location,
					resolveResult.Result,
					expr.Node,
					resolveResult.Resolver
				);
			}


			return CreateCompletionData(
				location,
				resolveResult.Result,
				expr.Node,
				resolveResult.Resolver
			);
		}

		bool IsInPreprocessorDirective()
		{
			var text = GetMemberTextToCaret().Item1;
			var miniLexer = new MiniLexer(text);
			miniLexer.Parse();
			return miniLexer.IsInPreprocessorDirective;
		}

		IEnumerable<ICompletionData> HandleObjectInitializer(SyntaxTree unit, AstNode n)
		{
			var p = n.Parent;
			while (p != null && !(p is ObjectCreateExpression)) {
				p = p.Parent;
			}
			var parent = n.Parent as ArrayInitializerExpression;
			if (parent == null)
				return null;
			if (parent.IsSingleElement)
				parent = (ArrayInitializerExpression)parent.Parent;
			if (p != null) {
				var contextList = new CompletionDataWrapper(this);
				var initializerResult = ResolveExpression(p);
				IType initializerType = null;

				if (initializerResult.Result is DynamicInvocationResolveResult) {
					var dr = (DynamicInvocationResolveResult)initializerResult.Result;
					var constructor = (dr.Target as MethodGroupResolveResult).Methods.FirstOrDefault();
					if (constructor != null)
						initializerType = constructor.DeclaringType;
				} else {
					initializerType = initializerResult != null ? initializerResult.Result.Type : null;
				}


				if (initializerType != null && initializerType.Kind != TypeKind.Unknown) {
					// check 3 cases:
					// 1) New initalizer { xpr
					// 2) Object initializer { prop = val1, field = val2, xpr
					// 3) Array initializer { new Foo (), a, xpr
					// in case 1 all object/array initializer options should be given - in the others not.

					AstNode prev = null;
					if (parent.Elements.Count > 1) {
						prev = parent.Elements.First();
						if (prev is ArrayInitializerExpression && ((ArrayInitializerExpression)prev).IsSingleElement)
							prev = ((ArrayInitializerExpression)prev).Elements.FirstOrDefault();
					}

					if (prev != null && !(prev is NamedExpression)) {
						AddContextCompletion(contextList, GetState(), n);
						// case 3)
						return contextList.Result;
					}
					var lookup = new MemberLookup(ctx.CurrentTypeDefinition, Compilation.MainAssembly);
					var list = typeof(System.Collections.IList).ToTypeReference().Resolve(Compilation);
					var list1 = typeof(System.Collections.Generic.IList<>).ToTypeReference().Resolve(Compilation);
					bool isProtectedAllowed = ctx.CurrentTypeDefinition != null && initializerType.GetDefinition() != null ? 
						ctx.CurrentTypeDefinition.IsDerivedFrom(initializerType.GetDefinition()) : 
						false;
					foreach (var m in initializerType.GetMembers (m => m.SymbolKind == SymbolKind.Field)) {
						var f = m as IField;
						if (f != null && (f.IsReadOnly || f.IsConst))
							continue;
						if (lookup.IsAccessible(m, isProtectedAllowed)) {
							var data = contextList.AddMember(m);
							if (data != null)
								data.DisplayFlags |= DisplayFlags.NamedArgument;
						}
					}

					foreach (IProperty m in initializerType.GetMembers (m => m.SymbolKind == SymbolKind.Property)) {
						if (m.CanSet && lookup.IsAccessible(m.Setter, isProtectedAllowed)  || 
							m.CanGet && lookup.IsAccessible(m.Getter, isProtectedAllowed) && m.ReturnType.GetDefinition() != null && 
							(m.ReturnType.GetDefinition().IsDerivedFrom(list.GetDefinition()) || m.ReturnType.GetDefinition().IsDerivedFrom(list1.GetDefinition()))) {
							var data = contextList.AddMember(m);
							if (data != null)
								data.DisplayFlags |= DisplayFlags.NamedArgument;
						}
					}

					if (prev != null && (prev is NamedExpression)) {
						// case 2)
						return contextList.Result;
					}

					// case 1)

					// check if the object is a list, if not only provide object initalizers
					if (initializerType.Kind != TypeKind.Array && list != null) {
						var def = initializerType.GetDefinition(); 
						if (def != null && !def.IsDerivedFrom(list.GetDefinition()) && !def.IsDerivedFrom(list1.GetDefinition()))
							return contextList.Result;
					}

					AddContextCompletion(contextList, GetState(), n);
					return contextList.Result;
				}
			}
			return null;
		}

		static readonly DateTime curDate = DateTime.Now;

		IEnumerable<ICompletionData> GenerateNumberFormatitems(bool isFloatingPoint)
		{
			yield return factory.CreateFormatItemCompletionData("D", "decimal", 123);
			yield return factory.CreateFormatItemCompletionData("D5", "decimal", 123);
			yield return factory.CreateFormatItemCompletionData("C", "currency", 123);
			yield return factory.CreateFormatItemCompletionData("C0", "currency", 123);
			yield return factory.CreateFormatItemCompletionData("E", "exponential", 1.23E4);
			yield return factory.CreateFormatItemCompletionData("E2", "exponential", 1.234);
			yield return factory.CreateFormatItemCompletionData("e2", "exponential", 1.234);
			yield return factory.CreateFormatItemCompletionData("F", "fixed-point", 123.45);
			yield return factory.CreateFormatItemCompletionData("F1", "fixed-point", 123.45);
			yield return factory.CreateFormatItemCompletionData("G", "general", 1.23E+56);
			yield return factory.CreateFormatItemCompletionData("g2", "general", 1.23E+56);
			yield return factory.CreateFormatItemCompletionData("N", "number", 12345.68);
			yield return factory.CreateFormatItemCompletionData("N1", "number", 12345.68);
			yield return factory.CreateFormatItemCompletionData("P", "percent", 12.34);
			yield return factory.CreateFormatItemCompletionData("P1", "percent", 12.34);
			yield return factory.CreateFormatItemCompletionData("R", "round-trip", 0.1230000001);
			yield return factory.CreateFormatItemCompletionData("X", "hexadecimal", 1234);
			yield return factory.CreateFormatItemCompletionData("x8", "hexadecimal", 1234);
			yield return factory.CreateFormatItemCompletionData("0000", "custom", 123);
			yield return factory.CreateFormatItemCompletionData("####", "custom", 123);
			yield return factory.CreateFormatItemCompletionData("##.###", "custom", 1.23);
			yield return factory.CreateFormatItemCompletionData("##.000", "custom", 1.23);
			yield return factory.CreateFormatItemCompletionData("## 'items'", "custom", 12);
		}

		IEnumerable<ICompletionData> GenerateDateTimeFormatitems()
		{
			yield return factory.CreateFormatItemCompletionData("D", "long date", curDate);
			yield return factory.CreateFormatItemCompletionData("d", "short date", curDate);
			yield return factory.CreateFormatItemCompletionData("F", "full date long", curDate);
			yield return factory.CreateFormatItemCompletionData("f", "full date short", curDate);
			yield return factory.CreateFormatItemCompletionData("G", "general long", curDate);
			yield return factory.CreateFormatItemCompletionData("g", "general short", curDate);
			yield return factory.CreateFormatItemCompletionData("M", "month", curDate);
			yield return factory.CreateFormatItemCompletionData("O", "ISO 8601", curDate);
			yield return factory.CreateFormatItemCompletionData("R", "RFC 1123", curDate);
			yield return factory.CreateFormatItemCompletionData("s", "sortable", curDate);
			yield return factory.CreateFormatItemCompletionData("T", "long time", curDate);
			yield return factory.CreateFormatItemCompletionData("t", "short time", curDate);
			yield return factory.CreateFormatItemCompletionData("U", "universal full", curDate);
			yield return factory.CreateFormatItemCompletionData("u", "universal sortable", curDate);
			yield return factory.CreateFormatItemCompletionData("Y", "year month", curDate);
			yield return factory.CreateFormatItemCompletionData("yy-MM-dd", "custom", curDate);
			yield return factory.CreateFormatItemCompletionData("yyyy MMMMM dd", "custom", curDate);
			yield return factory.CreateFormatItemCompletionData("yy-MMM-dd ddd", "custom", curDate);
			yield return factory.CreateFormatItemCompletionData("yyyy-M-d dddd", "custom", curDate);
			yield return factory.CreateFormatItemCompletionData("hh:mm:ss t z", "custom", curDate);
			yield return factory.CreateFormatItemCompletionData("hh:mm:ss tt zz", "custom", curDate);
			yield return factory.CreateFormatItemCompletionData("HH:mm:ss tt zz", "custom", curDate);
			yield return factory.CreateFormatItemCompletionData("HH:m:s tt zz", "custom", curDate);

		}

		[Flags]
		enum TestEnum
		{
			EnumCaseName = 0,
			Flag1 = 1,
			Flag2 = 2,
			Flags

		}

		IEnumerable<ICompletionData> GenerateEnumFormatitems()
		{
			yield return factory.CreateFormatItemCompletionData("G", "string value", TestEnum.EnumCaseName);
			yield return factory.CreateFormatItemCompletionData("F", "flags value", TestEnum.Flags);
			yield return factory.CreateFormatItemCompletionData("D", "integer value", TestEnum.Flags);
			yield return factory.CreateFormatItemCompletionData("X", "hexadecimal", TestEnum.Flags);
		}

		IEnumerable<ICompletionData> GenerateTimeSpanFormatitems()
		{
			yield return factory.CreateFormatItemCompletionData("c", "invariant", new TimeSpan(0, 1, 23, 456));
			yield return factory.CreateFormatItemCompletionData("G", "general long", new TimeSpan(0, 1, 23, 456));
			yield return factory.CreateFormatItemCompletionData("g", "general short", new TimeSpan(0, 1, 23, 456));
		}

		static Guid defaultGuid = Guid.NewGuid();

		IEnumerable<ICompletionData> GenerateGuidFormatitems()
		{
			yield return factory.CreateFormatItemCompletionData("N", "digits", defaultGuid);
			yield return factory.CreateFormatItemCompletionData("D", "hypens", defaultGuid);
			yield return factory.CreateFormatItemCompletionData("B", "braces", defaultGuid);
			yield return factory.CreateFormatItemCompletionData("P", "parentheses", defaultGuid);
		}
        List<ICompletionData> GeneratePreDefined()
        {
            List<ICompletionData> cmpl = new List<ICompletionData>();


         //   cmpl.Add( factory.CreatePredefinedCompletionData("CInt", ""));


            return cmpl;
        }

		int GetFormatItemNumber()
		{
			int number = 0;
			var o = offset - 2;
			while (o > 0) {
				char ch = document.GetCharAt(o);
				if (ch == '{')
					return number;
				if (!char.IsDigit(ch))
					break;
				number = number * 10 + ch - '0';
				o--;
			}
			return -1;
		}

		IEnumerable<ICompletionData> HandleStringFormatItems()
		{
			var formatArgument = GetFormatItemNumber();
			if (formatArgument < 0)
				return Enumerable.Empty<ICompletionData>();
			var followUp = new StringBuilder();

			var o = offset;
			while (o < document.TextLength) {
				char ch = document.GetCharAt(o);
				followUp.Append(ch); 
				o++;
				if (ch == ';')
					break;
			}
			var unit = ParseStub(followUp.ToString(), false);

			var invoke = unit.GetNodeAt<InvocationExpression>(location);

			if (invoke != null) {
				var resolveResult = ResolveExpression(new ExpressionResult(invoke, unit));
				var invokeResult = resolveResult.Result as InvocationResolveResult;
				if (invokeResult != null) {
					var arg = formatArgument + 1; // First argument is the format string
					if (arg < invoke.Arguments.Count) {
						var invokeArgument = ResolveExpression(new ExpressionResult(invoke.Arguments.ElementAt(arg), unit));
						if (invokeArgument != null) {
							var provider = GetFormatCompletionData(invokeArgument.Result.Type);
							if (provider != null)
								return provider;
							if (!invokeArgument.Result.Type.IsKnownType(KnownTypeCode.Object))
								return Enumerable.Empty<ICompletionData>();
						}
					}
				}
			}
			return HandleStringFormatItemsFallback();
		}

		IEnumerable<ICompletionData> HandleStringFormatItemsFallback()
		{
			var unit = ParseStub("a}\");", false);

			var invoke = unit.GetNodeAt<InvocationExpression>(location);

			if (invoke == null)
				return Enumerable.Empty<ICompletionData>();

			var resolveResult = ResolveExpression(new ExpressionResult(invoke, unit));
			var invokeResult = resolveResult.Result as AlInvocationResolveResult;
			if (invokeResult == null)
				return Enumerable.Empty<ICompletionData>();

			Expression fmtArgumets;
			IList<Expression> args;
			if (FormatStringHelper.TryGetFormattingParameters(invokeResult, invoke, out fmtArgumets, out args, null)) {
				return GenerateNumberFormatitems(false)
					.Concat(GenerateDateTimeFormatitems())
					.Concat(GenerateTimeSpanFormatitems())
					.Concat(GenerateEnumFormatitems())
					.Concat(GenerateGuidFormatitems());
			}
			return Enumerable.Empty<ICompletionData>();

		}

		IEnumerable<ICompletionData> GetFormatCompletionData(IType type)
		{
			if (type.Namespace != "System")
				return null;
			switch (type.Name) {
				case "Int64":
				case "UInt64":
				case "Int32":
				case "UInt32":
				case "Int16":
				case "UInt16":
				case "Byte":
				case "SByte":
					return GenerateNumberFormatitems(false);
				case "Single":
				case "Double":
				case "Decimal":
					return GenerateNumberFormatitems(true);
				case "Enum":
					return GenerateEnumFormatitems();
				case "DateTime":
					return GenerateDateTimeFormatitems();
				case "TimeSpan":
					return GenerateTimeSpanFormatitems();
				case "Guid":
					return GenerateGuidFormatitems();
			}
			return null;
		}

		IEnumerable<ICompletionData> HandleToStringFormatItems()
		{
			var unit = ParseStub("\");", false);

			var invoke = unit.GetNodeAt<InvocationExpression>(location);
			if (invoke == null)
				return Enumerable.Empty<ICompletionData>();

			var resolveResult = ResolveExpression(new ExpressionResult(invoke, unit));
			var invokeResult = resolveResult.Result as InvocationResolveResult;
			if (invokeResult == null)
				return Enumerable.Empty<ICompletionData>();
			if (invokeResult.Member.Name == "ToString")
				return GetFormatCompletionData(invokeResult.Member.DeclaringType) ?? Enumerable.Empty<ICompletionData>();
			return Enumerable.Empty<ICompletionData>();
		}

		IEnumerable<ICompletionData> MagicKeyCompletion(char completionChar, bool controlSpace, out bool isComplete)
		{
			isComplete = false;
			ExpressionResolveResult resolveResult;
			switch (completionChar) {
				// Magic key completion
				case ':':
					var text = GetMemberTextToCaret();
					var lexer = new MiniLexer(text.Item1);
					lexer.Parse();
					if (lexer.IsInSingleComment ||
						lexer.IsInChar ||
						lexer.IsInMultiLineComment ||
						lexer.IsInPreprocessorDirective) {
						return Enumerable.Empty<ICompletionData>();
					}

					if (lexer.IsInString || lexer.IsInVerbatimString)
						return HandleStringFormatItems();
					return HandleMemberReferenceCompletion(GetExpressionBeforeCursor());
				case '"':
					text = GetMemberTextToCaret();
					lexer = new MiniLexer(text.Item1);
					lexer.Parse();
					if (lexer.IsInSingleComment ||
						lexer.IsInChar ||
						lexer.IsInMultiLineComment ||
						lexer.IsInPreprocessorDirective) {
						return Enumerable.Empty<ICompletionData>();
					}

					if (lexer.IsInString || lexer.IsInVerbatimString)
						return HandleToStringFormatItems();
					return Enumerable.Empty<ICompletionData>();
				case '.':
					if (IsInsideCommentStringOrDirective() || IsInsideAsmCode()) {
						return Enumerable.Empty<ICompletionData>();
					}
					return HandleMemberReferenceCompletion(GetExpressionBeforeCursor());
				case '#':
					if (!IsInPreprocessorDirective())
						return null;
					return GetDirectiveCompletionData();
					// XML doc completion
				case '<':
					if (IsInsideDocComment()) {
						return GetXmlDocumentationCompletionData();
					}
					if (controlSpace) {
						return DefaultControlSpaceItems(ref isComplete);
					}
					return null;
				case '>':
					if (!IsInsideDocComment()) {
						if (offset > 2 && document.GetCharAt(offset - 2) == '-' && !IsInsideCommentStringOrDirective() && !IsInsideAsmCode()) {
							return HandleMemberReferenceCompletion(GetExpressionBeforeCursor());
						}
						return null;
					}
					return null;

					// Parameter completion
				case '(':
					if (IsInsideCommentStringOrDirective() && IsInsideAsmCode()) {
						return null;
					}
					var invoke = GetInvocationBeforeCursor(true);
					if (invoke == null) {
						if (controlSpace)
							return DefaultControlSpaceItems(ref isComplete, invoke);
						return null;
					}
					if (invoke.Node is TypeOfExpression) {
						return CreateTypeList();
					}
					var invocationResult = ResolveExpression(invoke);
					if (invocationResult == null) {
						return null;
					}
					var methodGroup = invocationResult.Result as MethodGroupResolveResult;
					if (methodGroup != null) {
						return CreateParameterCompletion(
							methodGroup,
							invocationResult.Resolver,
							invoke.Node,
							invoke.Unit,
							0,
							controlSpace
						);
					}

					if (controlSpace) {
						return DefaultControlSpaceItems(ref isComplete, invoke);
					}
					return null;
				case '=':
					return controlSpace ? DefaultControlSpaceItems(ref isComplete) : null;
				case ',':
					int cpos2;
					if (!GetParameterCompletionCommandOffset(out cpos2)) { 
						return null;
					}
					//	completionContext = CompletionWidget.CreateCodeCompletionContext (cpos2);
					//	int currentParameter2 = MethodParameterDataProvider.GetCurrentParameterIndex (CompletionWidget, completionContext) - 1;
					//				return CreateParameterCompletion (CreateResolver (), location, ExpressionContext.MethodBody, provider.Methods, currentParameter);	
					break;

					// Completion on space:
				case ' ':
					int tokenIndex = offset;
					string token = GetPreviousToken(ref tokenIndex, false);
					if (IsInsideCommentStringOrDirective()) {
						return null;
					}
                    if (IsInsideAsmCode())
                    {
                        return null;
                    }

					// check propose name, for context <variable name> <ctrl+space> (but only in control space context)
					//IType isAsType = null;
					var isAsExpression = GetExpressionAt(offset);
					if (controlSpace && isAsExpression != null && isAsExpression.Node is VariableDeclarationStatement && token != "new") {
						var parent = isAsExpression.Node as VariableDeclarationStatement;
						var proposeNameList = new CompletionDataWrapper(this);
						if (parent.Variables.Count != 1)
							return DefaultControlSpaceItems(ref isComplete, isAsExpression, controlSpace);

						foreach (var possibleName in GenerateNameProposals (parent.Type)) {
							if (possibleName.Length > 0) {
								proposeNameList.Result.Add(factory.CreateLiteralCompletionData(possibleName.ToString()));
							}
						}

						AutoSelect = false;
						AutoCompleteEmptyMatch = false;
						isComplete = true;
						return proposeNameList.Result;
					}
					//				int tokenIndex = offset;
					//				string token = GetPreviousToken (ref tokenIndex, false);
					//				if (result.ExpressionContext == ExpressionContext.ObjectInitializer) {
					//					resolver = CreateResolver ();
					//					ExpressionContext exactContext = new NewAlExpressionFinder (dom).FindExactContextForObjectInitializer (document, resolver.Unit, Document.FileName, resolver.CallingType);
					//					IReturnType objectInitializer = ((ExpressionContext.TypeExpressionContext)exactContext).UnresolvedType;
					//					if (objectInitializer != null && objectInitializer.ArrayDimensions == 0 && objectInitializer.PointerNestingLevel == 0 && (token == "{" || token == ","))
					//						return CreateCtrlSpaceCompletionData (completionContext, result); 
					//				}
					if (token == "=") {
						int j = tokenIndex;
						string prevToken = GetPreviousToken(ref j, false);
						if (prevToken == "=" || prevToken == "+" || prevToken == "-" || prevToken == "!") {
							token = prevToken + token;
							tokenIndex = j;
						}
					}
					switch (token) {
						case "(":
						case ",":
							int cpos;
							if (!GetParameterCompletionCommandOffset(out cpos)) { 
								break;
							}
							int currentParameter = GetCurrentParameterIndex(cpos - 1, this.offset) - 1;
							if (currentParameter < 0) {
								return null;
							}
							invoke = GetInvocationBeforeCursor(token == "(");
							if (invoke == null) {
								return null;
							}
							invocationResult = ResolveExpression(invoke);
							if (invocationResult == null) {
								return null;
							}
							methodGroup = invocationResult.Result as MethodGroupResolveResult;
							if (methodGroup != null) {
								return CreateParameterCompletion(
									methodGroup,
									invocationResult.Resolver,
									invoke.Node,
									invoke.Unit,
									currentParameter,
									controlSpace);
							}
							return null;
						case "=":
						case "==":
						case "!=":
							GetPreviousToken(ref tokenIndex, false);
							var expressionOrVariableDeclaration = GetExpressionAt(tokenIndex);
							if (expressionOrVariableDeclaration == null) {
								return null;
							}
							resolveResult = ResolveExpression(expressionOrVariableDeclaration);
							if (resolveResult == null) {
								return null;
							}
							if (resolveResult.Result.Type.Kind == TypeKind.Enum) {
								var wrapper = new CompletionDataWrapper(this);
								AddContextCompletion(
									wrapper,
									resolveResult.Resolver,
									expressionOrVariableDeclaration.Node);
								AddEnumMembers(wrapper, resolveResult.Result.Type, resolveResult.Resolver);
								AutoCompleteEmptyMatch = false;
								return wrapper.Result;
							}
							//				
							//					if (resolvedType.FullName == DomReturnType.Bool.FullName) {
							//						CompletionDataList completionList = new ProjectDomCompletionDataList ();
							//						CompletionDataCollector cdc = new CompletionDataCollector (this, dom, completionList, Document.CompilationUnit, resolver.CallingType, location);
							//						completionList.AutoCompleteEmptyMatch = false;
							//						cdc.Add ("true", "md-keyword");
							//						cdc.Add ("false", "md-keyword");
							//						resolver.AddAccessibleCodeCompletionData (result.ExpressionContext, cdc);
							//						return completionList;
							//					}
							//					if (resolvedType.ClassType == ClassType.Delegate && token == "=") {
							//						CompletionDataList completionList = new ProjectDomCompletionDataList ();
							//						string parameterDefinition = AddDelegateHandlers (completionList, resolvedType);
							//						string varName = GetPreviousMemberReferenceExpression (tokenIndex);
							//						completionList.Add (new EventCreationCompletionData (document, varName, resolvedType, null, parameterDefinition, resolver.CallingMember, resolvedType));
							//						
							//						CompletionDataCollector cdc = new CompletionDataCollector (this, dom, completionList, Document.CompilationUnit, resolver.CallingType, location);
							//						resolver.AddAccessibleCodeCompletionData (result.ExpressionContext, cdc);
							//						foreach (var data in completionList) {
							//							if (data is MemberCompletionData) 
							//								((MemberCompletionData)data).IsDelegateExpected = true;
							//						}
							//						return completionList;
							//					}
							return null;
						case "+=":
						case "-=":
							var curTokenIndex = tokenIndex;
							GetPreviousToken(ref tokenIndex, false);

							expressionOrVariableDeclaration = GetExpressionAt(tokenIndex);
							if (expressionOrVariableDeclaration == null) {
								return null;
							}

							resolveResult = ResolveExpression(expressionOrVariableDeclaration);
							if (resolveResult == null) {
								return null;
							}


							var mrr = resolveResult.Result as MemberResolveResult;
							if (mrr != null) {
								var evt = mrr.Member as IEvent;
								if (evt == null) {
									return null;
								}
								var delegateType = evt.ReturnType;
								if (delegateType.Kind != TypeKind.Delegate) {
									return null;
								}

								var wrapper = new CompletionDataWrapper(this);
								if (currentType != null) {
									//							bool includeProtected = DomType.IncludeProtected (dom, typeFromDatabase, resolver.CallingType);
									foreach (var method in ctx.CurrentTypeDefinition.Methods) {
										if (MatchDelegate(delegateType, method) /*										&& method.IsAccessibleFrom (dom, resolver.CallingType, resolver.CallingMember, includeProtected) &&*/) {
											wrapper.AddMember(method);
											//									data.SetText (data.CompletionText + ";");
										}
									}
								}
								if (token == "+=") {
									string parameterDefinition = AddDelegateHandlers(
										wrapper,
										delegateType,
										optDelegateName: GuessEventHandlerMethodName(curTokenIndex, (currentType == null) ? null : currentType.Name)
									);
								}

								return wrapper.Result;
							}
							return null;
						case ":":
							if (currentMember == null) {
								token = GetPreviousToken(ref tokenIndex, false);
								token = GetPreviousToken(ref tokenIndex, false);
								if (token == "enum")
									return HandleEnumContext();
								var wrapper = new CompletionDataWrapper(this);
								AddTypesAndNamespaces(
									wrapper,
									GetState(),
									null,
									t =>  {
										if (currentType != null && currentType.ReflectionName.Equals(t.ReflectionName))
											return null;
										var def = t.GetDefinition();
										if (def != null && t.Kind != TypeKind.Interface && (def.IsSealed ||def.IsStatic))
											return null;
										return t;
									}
								);
								return wrapper.Result;
							}
							return null;
					}

					var keywordCompletion = HandleKeywordCompletion(tokenIndex, token);
					if (keywordCompletion == null && controlSpace) {
						goto default;
					}
					return keywordCompletion;
					// Automatic completion
				default:
					if (IsInsideCommentStringOrDirective()) {
						tokenIndex = offset;
						token = GetPreviousToken(ref tokenIndex, false);
						if (IsInPreprocessorDirective() && (token.Length == 1 && char.IsLetter(completionChar) || controlSpace)) {
							while (token != null && document.GetCharAt(tokenIndex - 1) != '#') {
								token = GetPreviousToken(ref tokenIndex, false);
							}
							if (token != null)
								return HandleKeywordCompletion(tokenIndex, token);
						}
						return null;
					}
                    if (IsInsideAsmCode())
                    {
                        tokenIndex = offset;
                        token = GetPreviousToken(ref tokenIndex, false);
                        if (token.Length == 1 && char.IsLetter(token[0]))
                        {
                            var wrapper = new CompletionDataWrapper(this);


                            AddAsmKeywords(wrapper, AlCompletionEngine.AsmKeyWords);
                            return wrapper.Result;

                        }
                    }
					char prevCh = offset > 2 ? document.GetCharAt(offset - 2) : ';';
					char nextCh = offset < document.TextLength ? document.GetCharAt(offset) : ' ';
					const string allowedChars = ";,.[](){}+-*/%^?:&|~!<>=";

					if ((!Char.IsWhiteSpace(nextCh) && allowedChars.IndexOf(nextCh) < 0) || !(Char.IsWhiteSpace(prevCh) || allowedChars.IndexOf(prevCh) >= 0)) {
						if (!controlSpace)
							return null;
					}

					if (IsInLinqContext(offset)) {
						if (!controlSpace && !(char.IsLetter(completionChar) || completionChar == '_')) {
							return null;
						}
						tokenIndex = offset;
						token = GetPreviousToken(ref tokenIndex, false);
						// token last typed
						if (!char.IsWhiteSpace(completionChar) && !linqKeywords.Contains(token)) {
							token = GetPreviousToken(ref tokenIndex, false);
						}
						// token last typed

						if (linqKeywords.Contains(token)) {
							if (token == "from") {
								// after from no auto code completion.
								return null;
							}
							return DefaultControlSpaceItems(ref isComplete);
						}
						var dataList = new CompletionDataWrapper(this);
						AddKeywords(dataList, linqKeywords);
						return dataList.Result;
					}
					if (currentType != null && currentType.Kind == TypeKind.Enum) {
						if (!char.IsLetter(completionChar))
							return null;
						return HandleEnumContext();
					}
					var contextList = new CompletionDataWrapper(this);
					var identifierStart = GetExpressionAtCursor();
					if (!(char.IsLetter(completionChar) || completionChar == '_') && (!controlSpace || identifierStart == null)) {
						return controlSpace ? HandleAccessorContext() ?? DefaultControlSpaceItems(ref isComplete, identifierStart) : null;
					}

					if (identifierStart != null) {
						if (identifierStart.Node is TypeParameterDeclaration) {
							return null;
						}

						if (identifierStart.Node is MemberReferenceExpression) {
							return HandleMemberReferenceCompletion(
								new ExpressionResult(
									((MemberReferenceExpression)identifierStart.Node).Target,
									identifierStart.Unit
								)
							);
						}

						if (identifierStart.Node is Identifier) {
							if (identifierStart.Node.Parent is GotoStatement)
								return null;

							// May happen in variable names
							return controlSpace ? DefaultControlSpaceItems(ref isComplete, identifierStart) : null;
						}
						if (identifierStart.Node is VariableInitializer && location <= ((VariableInitializer)identifierStart.Node).NameToken.EndLocation) {
							return controlSpace ? HandleAccessorContext() ?? DefaultControlSpaceItems(ref isComplete, identifierStart) : null;
						}
						if (identifierStart.Node is CatchClause) {
							if (((CatchClause)identifierStart.Node).VariableNameToken.IsInside(location)) {
								return null;
							}
						}
						if (identifierStart.Node is AstType && identifierStart.Node.Parent is CatchClause) {
							return HandleCatchClauseType(identifierStart);
						}

						var pDecl = identifierStart.Node as ParameterDeclaration;
						if (pDecl != null && pDecl.Parent is LambdaExpression) {
							return null;
						}
					}


					// Do not pop up completion on identifier identifier (should be handled by keyword completion).
					tokenIndex = offset - 1;
					token = GetPreviousToken(ref tokenIndex, false);
					if (token == "class" || token == "interface" || token == "struct" || token == "enum" || token == "program") {
						// after these always follows a name
						return null;
					}
					var keywordresult = HandleKeywordCompletion(tokenIndex, token);
					if (keywordresult != null) {
						return keywordresult;
					}

					if ((!Char.IsWhiteSpace(nextCh) && allowedChars.IndexOf(nextCh) < 0) || !(Char.IsWhiteSpace(prevCh) || allowedChars.IndexOf(prevCh) >= 0)) {
						if (controlSpace)
							return DefaultControlSpaceItems(ref isComplete, identifierStart);
					}

					int prevTokenIndex = tokenIndex;
					var prevToken2 = GetPreviousToken(ref prevTokenIndex, false);
					if (prevToken2 == "delegate") {
						// after these always follows a name
						return null;
					}

					if (identifierStart == null && !string.IsNullOrEmpty(token) && !IsInsideCommentStringOrDirective() && (prevToken2 == ";" || prevToken2 == "{" || prevToken2 == "}")) {
						char last = token [token.Length - 1];
						if (char.IsLetterOrDigit(last) || last == '_' || token == ">") {
							return HandleKeywordCompletion(tokenIndex, token);
						}
					}
					if (identifierStart == null) {
						var accCtx = HandleAccessorContext();
						if (accCtx != null) {
							return accCtx;
						}
						return DefaultControlSpaceItems(ref isComplete, null, controlSpace);
					}
					AlResolver csResolver;
					AstNode n = identifierStart.Node;
					if (n.Parent is NamedArgumentExpression)
						n = n.Parent;

					if (n != null && n.Parent is AnonymousTypeCreateExpression) {
						AutoSelect = false;
					}

					// new { b$ } 
					if (n is IdentifierExpression && n.Parent is AnonymousTypeCreateExpression)
						return null;

					// Handle foreach (type name _
					if (n is IdentifierExpression) {
						var prev = n.GetPrevNode() as ForeachStatement;
						while (prev != null && prev.EmbeddedStatement is ForeachStatement)
							prev = (ForeachStatement)prev.EmbeddedStatement;
						if (prev != null && prev.InExpression.IsNull) {
							if (IncludeKeywordsInCompletionList)
								contextList.AddCustom("in");
							return contextList.Result;
						}
					}
					// Handle object/enumerable initialzer expressions: "new O () { P$"
					if (n is IdentifierExpression && n.Parent is ArrayInitializerExpression && !(n.Parent.Parent is ArrayCreateExpression)) {
						var result = HandleObjectInitializer(identifierStart.Unit, n);
						if (result != null)
							return result;
					}

					if (n != null && n.Parent is InvocationExpression ||
						n.Parent is ParenthesizedExpression && n.Parent.Parent is InvocationExpression) {
						if (n.Parent is ParenthesizedExpression)
							n = n.Parent;
						var invokeParent = (InvocationExpression)n.Parent;
						var invokeResult = ResolveExpression(
							invokeParent.Target
						);
						var mgr = invokeResult != null ? invokeResult.Result as MethodGroupResolveResult : null;
						if (mgr != null) {
							int idx = 0;
							foreach (var arg in invokeParent.Arguments) {
								if (arg == n) {
									break;
								}
								idx++;
							}

							foreach (var method in mgr.Methods) {
								if (idx < method.Parameters.Count && method.Parameters [idx].Type.Kind == TypeKind.Delegate) {
									AutoSelect = false;
									AutoCompleteEmptyMatch = false;
								}
								foreach (var p in method.Parameters) {
									contextList.AddNamedParameterVariable(p);
								}
							}
							idx++;
							foreach (var list in mgr.GetEligibleExtensionMethods (true)) {
								foreach (var method in list) {
									if (idx < method.Parameters.Count && method.Parameters [idx].Type.Kind == TypeKind.Delegate) {
										AutoSelect = false;
										AutoCompleteEmptyMatch = false;
									}
								}
							}
						}
					}

					if (n != null && n.Parent is ObjectCreateExpression) {
						var invokeResult = ResolveExpression(n.Parent);
						var mgr = invokeResult != null ? invokeResult.Result as ResolveResult : null;
						if (mgr != null) {
							foreach (var constructor in mgr.Type.GetConstructors ()) {
								foreach (var p in constructor.Parameters) {
									contextList.AddVariable(p);
								}
							}
						}
					}

					if (n is IdentifierExpression) {
						var bop = n.Parent as BinaryOperatorExpression;
						Expression evaluationExpr = null;

						if (bop != null && bop.Right == n && (bop.Operator == BinaryOperatorType.Equality || bop.Operator == BinaryOperatorType.InEquality)) {
							evaluationExpr = bop.Left;
						}
						// check for compare to enum case 
						if (evaluationExpr != null) {
							resolveResult = ResolveExpression(evaluationExpr);
							if (resolveResult != null && resolveResult.Result.Type.Kind == TypeKind.Enum) {
								var wrapper = new CompletionDataWrapper(this);
								AddContextCompletion(
									wrapper,
									resolveResult.Resolver,
									evaluationExpr
								);
								AddEnumMembers(wrapper, resolveResult.Result.Type, resolveResult.Resolver);
								AutoCompleteEmptyMatch = false;
								return wrapper.Result;
							}
						}
					}

					if (n is Identifier && n.Parent is ForeachStatement) {
						if (controlSpace) {
							return DefaultControlSpaceItems(ref isComplete);
						}
						return null;
					}

					if (n is ArrayInitializerExpression) {
						// check for new [] {...} expression -> no need to resolve the type there
						var parent = n.Parent as ArrayCreateExpression;
						if (parent != null && parent.Type.IsNull) {
							return DefaultControlSpaceItems(ref isComplete);
						}

						var initalizerResult = ResolveExpression(n.Parent);

						var concreteNode = identifierStart.Unit.GetNodeAt<IdentifierExpression>(location);
						// check if we're on the right side of an initializer expression
						if (concreteNode != null && concreteNode.Parent != null && concreteNode.Parent.Parent != null && concreteNode.Identifier != "a" && concreteNode.Parent.Parent is NamedExpression) {
							return DefaultControlSpaceItems(ref isComplete);
						}
						if (initalizerResult != null && initalizerResult.Result.Type.Kind != TypeKind.Unknown) { 

							foreach (var property in initalizerResult.Result.Type.GetProperties ()) {
								if (!property.IsPublic) {
									continue;
								}
								var data = contextList.AddMember(property);
								if (data != null)
									data.DisplayFlags |= DisplayFlags.NamedArgument;
							}
							foreach (var field in initalizerResult.Result.Type.GetFields ()) {       
								if (!field.IsPublic) {
									continue;
								}
								var data = contextList.AddMember(field);
								if (data != null)
									data.DisplayFlags |= DisplayFlags.NamedArgument;
							}
							return contextList.Result;
						}
						return DefaultControlSpaceItems(ref isComplete);
					}

					if (IsAttributeContext(n)) {
						// add attribute targets
						if (currentType == null) {
							contextList.AddCustom("assembly");
							contextList.AddCustom("module");
							contextList.AddCustom("type");
						} else {
							contextList.AddCustom("param");
							contextList.AddCustom("field");
							contextList.AddCustom("property");
							contextList.AddCustom("method");
							contextList.AddCustom("event");
						}
						contextList.AddCustom("ret");
					}
					if (n is MemberType) {
						resolveResult = ResolveExpression(
							((MemberType)n).Target
						);
						return CreateTypeAndNamespaceCompletionData(
							location,
							resolveResult.Result,
							((MemberType)n).Target,
							resolveResult.Resolver
						);
					}
					if (n != null/*					 && !(identifierStart.Item2 is TypeDeclaration)*/) {
						csResolver = new AlResolver(ctx);
						var nodes = new List<AstNode>();
						nodes.Add(n);
						if (n.Parent is ICSharpCode.NRefactory.Al.Attribute) {
							nodes.Add(n.Parent);
						}
						var astResolver = CompletionContextProvider.GetResolver(csResolver, identifierStart.Unit);
						astResolver.ApplyNavigator(new NodeListResolveVisitorNavigator(nodes));
						try {
							csResolver = astResolver.GetResolverStateBefore(n);
						} catch (Exception) {
							csResolver = GetState();
						}
						// add attribute properties.
						if (n.Parent is ICSharpCode.NRefactory.Al.Attribute) {
							var rr = ResolveExpression(n.Parent);
							if (rr != null)
								AddAttributeProperties(contextList, rr.Result);
						}
					} else {
						csResolver = GetState();
					}
					// identifier has already started with the first letter
					offset--;
					AddContextCompletion(
						contextList,
						csResolver,
						identifierStart.Node
					);
					return contextList.Result;
					//				if (stub.Parent is BlockStatement)

					//				result = FindExpression (dom, completionContext, -1);
					//				if (result == null)
					//					return null;
					//				 else if (result.ExpressionContext != ExpressionContext.IdentifierExpected) {
					//					triggerWordLength = 1;
					//					bool autoSelect = true;
					//					IType returnType = null;
					//					if ((prevCh == ',' || prevCh == '(') && GetParameterCompletionCommandOffset (out cpos)) {
					//						ctx = CompletionWidget.CreateCodeCompletionContext (cpos);
					//						NRefactoryParameterDataProvider dataProvider = ParameterCompletionCommand (ctx) as NRefactoryParameterDataProvider;
					//						if (dataProvider != null) {
					//							int i = dataProvider.GetCurrentParameterIndex (CompletionWidget, ctx) - 1;
					//							foreach (var method in dataProvider.Methods) {
					//								if (i < method.Parameters.Count) {
					//									returnType = dom.GetType (method.Parameters [i].ReturnType);
					//									autoSelect = returnType == null || returnType.ClassType != ClassType.Delegate;
					//									break;
					//								}
					//							}
					//						}
					//					}
					//					// Bug 677531 - Auto-complete doesn't always highlight generic parameter in method signature
					//					//if (result.ExpressionContext == ExpressionContext.TypeName)
					//					//	autoSelect = false;
					//					CompletionDataList dataList = CreateCtrlSpaceCompletionData (completionContext, result);
					//					AddEnumMembers (dataList, returnType);
					//					dataList.AutoSelect = autoSelect;
					//					return dataList;
					//				} else {
					//					result = FindExpression (dom, completionContext, 0);
					//					tokenIndex = offset;
					//					
					//					// check foreach case, unfortunately the expression finder is too dumb to handle full type names
					//					// should be overworked if the expression finder is replaced with a mcs ast based analyzer.
					//					var possibleForeachToken = GetPreviousToken (ref tokenIndex, false); // starting letter
					//					possibleForeachToken = GetPreviousToken (ref tokenIndex, false); // varname
					//				
					//					// read return types to '(' token
					//					possibleForeachToken = GetPreviousToken (ref tokenIndex, false); // varType
					//					if (possibleForeachToken == ">") {
					//						while (possibleForeachToken != null && possibleForeachToken != "(") {
					//							possibleForeachToken = GetPreviousToken (ref tokenIndex, false);
					//						}
					//					} else {
					//						possibleForeachToken = GetPreviousToken (ref tokenIndex, false); // (
					//						if (possibleForeachToken == ".")
					//							while (possibleForeachToken != null && possibleForeachToken != "(")
					//								possibleForeachToken = GetPreviousToken (ref tokenIndex, false);
					//					}
					//					possibleForeachToken = GetPreviousToken (ref tokenIndex, false); // foreach
					//				
					//					if (possibleForeachToken == "foreach") {
					//						result.ExpressionContext = ExpressionContext.ForeachInToken;
					//					} else {
					//						return null;
					//						//								result.ExpressionContext = ExpressionContext.IdentifierExpected;
					//					}
					//					result.Expression = "";
					//					result.Region = DomRegion.Empty;
					//				
					//					return CreateCtrlSpaceCompletionData (completionContext, result);
					//				}
					//				break;
			}
			return null;

		}

		IEnumerable<ICompletionData> HandleCatchClauseType(ExpressionResult identifierStart)
		{
			Func<IType, IType> typePred = delegate (IType type) {
				if (type.GetAllBaseTypes().Any(t => t.ReflectionName == "System.Exception"))
					return type;
				return null;
			};
			if (identifierStart.Node.Parent is CatchClause) {
				var wrapper = new CompletionDataWrapper(this);
				AddTypesAndNamespaces(
					wrapper,
					GetState(),
					identifierStart.Node,
					typePred,
					m => false
				);
				return wrapper.Result;
			}

			var resolveResult = ResolveExpression(identifierStart);
			return CreateCompletionData(
				location,
				resolveResult.Result,
				identifierStart.Node,
				resolveResult.Resolver,
				typePred
			);
		}

		string[] validEnumBaseTypes = {
			"byte",
			"sbyte",
			"shortint",
			"integer",
			"longint",
			"ushortint",
			"uinteger",
			"ulongint"
		};

		IEnumerable<ICompletionData> HandleEnumContext()
		{
			var syntaxTree = ParseStub("a", false);
			if (syntaxTree == null) {
				return null;
			}

			var curType = syntaxTree.GetNodeAt<TypeDeclaration>(location);
			if (curType == null || curType.ClassType != ClassType.Enum) {
				syntaxTree = ParseStub("a {}", false);
				var node = syntaxTree.GetNodeAt<AstType>(location);
				if (node != null) {
					var wrapper = new CompletionDataWrapper(this);
					AddKeywords(wrapper, validEnumBaseTypes);
					return wrapper.Result;
				}
			}

			var member = syntaxTree.GetNodeAt<EnumMemberDeclaration>(location);
			if (member != null && member.NameToken.EndLocation < location) {
				if (currentMember == null && currentType != null) {
					foreach (var a in currentType.Members)
						if (a.Region.Begin < location && (currentMember == null || a.Region.Begin > currentMember.Region.Begin))
							currentMember = a;
				}
				bool isComplete = false;
				return DefaultControlSpaceItems(ref isComplete);
			}

			var attribute = syntaxTree.GetNodeAt<Attribute>(location);
			if (attribute != null) {
				var contextList = new CompletionDataWrapper(this);
				var astResolver = CompletionContextProvider.GetResolver(GetState(), syntaxTree);
				var csResolver = astResolver.GetResolverStateBefore(attribute);
				AddContextCompletion(
					contextList,
					csResolver,
					attribute
				);
				return contextList.Result;
			}
			return null;
		}

		bool IsInLinqContext(int offset)
		{
			string token;
			while (null != (token = GetPreviousToken(ref offset, true)) && !IsInsideCommentStringOrDirective() && !IsInsideAsmCode()) {

				if (token == "from") {
					return !IsInsideCommentStringOrDirective(offset) && !IsInsideAsmCode(offset);
				}
				if (token == ";" || token == "{") {
					return false;
				}
			}
			return false;
		}

		IEnumerable<ICompletionData> HandleAccessorContext()
		{
			var unit = ParseStub("get; }", false);
			var node = unit.GetNodeAt(location, cn => !(cn is AlTokenNode));
			if (node is Accessor) {
				node = node.Parent;
			}
			var contextList = new CompletionDataWrapper(this);
			if (node is PropertyDeclaration || node is IndexerDeclaration) {
				if (IncludeKeywordsInCompletionList) {
					contextList.AddCustom("get");
					contextList.AddCustom("set");
					AddKeywords(contextList, accessorModifierKeywords);
				}
			} else if (node is CustomEventDeclaration) {
				if (IncludeKeywordsInCompletionList) {
					contextList.AddCustom("add");
					contextList.AddCustom("remove");
				}
			} else {
				return null;
			}

			return contextList.Result;
		}

		class IfVisitor :DepthFirstAstVisitor
		{
			TextLocation loc;
			ICompletionContextProvider completionContextProvider;
			public bool IsValid;

			public IfVisitor(TextLocation loc, ICompletionContextProvider completionContextProvider)
			{
				this.loc = loc;
				this.completionContextProvider = completionContextProvider;

				this.IsValid = true;
			}

			void Check(string argument)
			{
				// TODO: evaluate #if epressions
				if (argument.Any(c => !(char.IsLetterOrDigit(c) || c == '_')))
					return;
				IsValid &= completionContextProvider.ConditionalSymbols.Contains(argument);
			}

			Stack<PreProcessorDirective> ifStack = new Stack<PreProcessorDirective>();

			public override void VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective)
			{
				if (preProcessorDirective.Type == PreProcessorDirectiveType.If) {
					ifStack.Push(preProcessorDirective);
				} else if (preProcessorDirective.Type == PreProcessorDirectiveType.Endif) {
					if (ifStack.Count == 0)
						return;
					var ifDirective = ifStack.Pop();
					if (ifDirective.StartLocation < loc && loc < preProcessorDirective.EndLocation) {
						Check(ifDirective.Argument);
					}

				}

				base.VisitPreProcessorDirective(preProcessorDirective);
			}

			public void End()
			{
				while (ifStack.Count > 0) {
					Check(ifStack.Pop().Argument);
				}
			}
		}

		IEnumerable<ICompletionData> DefaultControlSpaceItems(ref bool isComplete, ExpressionResult xp = null, bool controlSpace = true)
		{
			var wrapper = new CompletionDataWrapper(this);
			if (offset >= document.TextLength) {
				offset = document.TextLength - 1;
			}
			while (offset > 1 && char.IsWhiteSpace(document.GetCharAt(offset))) {
				offset--;
			}
			location = document.GetLocation(offset);

			if (xp == null) {
				xp = GetExpressionAtCursor();
			}
			AstNode node;
			SyntaxTree unit;
			ExpressionResolveResult rr;
			if (xp != null) {
				node = xp.Node;
				rr = ResolveExpression(node);
				unit = xp.Unit;
			} else {
				unit = ParseStub("foo", false);
				node = unit.GetNodeAt(
					location.Line,
					location.Column + 2,
					n => n is Expression || n is AstType || n is NamespaceDeclaration || n is Attribute
				);
				rr = ResolveExpression(node);
			}
			var ifvisitor = new IfVisitor(location, CompletionContextProvider);
			unit.AcceptVisitor(ifvisitor);
			ifvisitor.End();
			if (!ifvisitor.IsValid)
				return null;
			// namespace name case
			var ns = node as NamespaceDeclaration;
			if (ns != null) {
				var last = ns.NamespaceName;
				if (last != null && location < last.EndLocation)
					return null;
			}
			if (node is Identifier && node.Parent is ForeachStatement) {
				var foreachStmt = (ForeachStatement)node.Parent;
				foreach (var possibleName in GenerateNameProposals (foreachStmt.VariableType)) {
					if (possibleName.Length > 0) {
						wrapper.Result.Add(factory.CreateLiteralCompletionData(possibleName.ToString()));
					}
				}

				AutoSelect = false;
				AutoCompleteEmptyMatch = false;
				isComplete = true;
				return wrapper.Result;
			}

			if (node is Identifier && node.Parent is ParameterDeclaration) {
				if (!controlSpace) {
					return null;
				}
				// Try Parameter name case 
				var param = node.Parent as ParameterDeclaration;
				if (param != null) {
					foreach (var possibleName in GenerateNameProposals (param.Type)) {
						if (possibleName.Length > 0) {
							wrapper.Result.Add(factory.CreateLiteralCompletionData(possibleName.ToString()));
						}
					}
					AutoSelect = false;
					AutoCompleteEmptyMatch = false;
					isComplete = true;
					return wrapper.Result;
				}
			}
			var pDecl = node as ParameterDeclaration;
			if (pDecl != null && pDecl.Parent is LambdaExpression) {
				return null;
			}
			/*						if (Unit != null && (node == null || node is TypeDeclaration)) {
				var constructor = Unit.GetNodeAt<ConstructorDeclaration>(
					location.Line,
					location.Column - 3
				);
				if (constructor != null && !constructor.ColonToken.IsNull && constructor.Initializer.IsNull) {
					wrapper.AddCustom("self");
					wrapper.AddCustom("me");
					return wrapper.Result;
				}
			}*/

			var initializer = node != null ? node.Parent as ArrayInitializerExpression : null;
			if (initializer != null) {
				var result = HandleObjectInitializer(unit, initializer);
				if (result != null)
					return result;
			}
			AlResolver csResolver = null;
			if (rr != null) {
				csResolver = rr.Resolver;
			}

			if (csResolver == null) {
				if (node != null) {
					csResolver = GetState();
					//var astResolver = new AlAstResolver (csResolver, node, xp != null ? xp.Item1 : AlUnresolvedFile);

					try {
						//csResolver = astResolver.GetResolverStateBefore (node);
						Console.WriteLine(csResolver.LocalVariables.Count());
					} catch (Exception  e) {
						Console.WriteLine("E!!!" + e);
					}

				} else {
					csResolver = GetState();
				}
			}

			if (node is Attribute) {
				// add attribute properties.
				var astResolver = CompletionContextProvider.GetResolver(csResolver, unit);
				var resolved = astResolver.Resolve(node);
				AddAttributeProperties(wrapper, resolved);
			}


			if (node == null) {
				// try lambda
				unit = ParseStub("foo) => {}", true);
				var pd = unit.GetNodeAt<ParameterDeclaration>(
					location.Line,
					location.Column
				);
				if (pd != null) {
					var astResolver = unit != null ? CompletionContextProvider.GetResolver(GetState(), unit) : null;
					var parameterType = astResolver.Resolve(pd.Type);
					// Type <name> is always a name context -> return null
					if (parameterType != null && !parameterType.IsError)
						return null;
				}
			}

			AddContextCompletion(wrapper, csResolver, node);

			return wrapper.Result;
		}

		static void AddAttributeProperties(CompletionDataWrapper wrapper, ResolveResult resolved)
		{
			if (resolved == null || resolved.Type.Kind == TypeKind.Unknown)
				return;

			foreach (var property in resolved.Type.GetProperties (p => p.Accessibility == Accessibility.Public)) {
				var data = wrapper.AddMember(property);
				if (data != null)
					data.DisplayFlags |= DisplayFlags.NamedArgument;
			}
			foreach (var field in resolved.Type.GetFields (p => p.Accessibility == Accessibility.Public)) {
				var data = wrapper.AddMember(field);
				if (data != null)
					data.DisplayFlags |= DisplayFlags.NamedArgument;
			}
			foreach (var constructor in resolved.Type.GetConstructors (p => p.Accessibility == Accessibility.Public)) {
				foreach (var p in constructor.Parameters) {
					wrapper.AddNamedParameterVariable(p);
				}
			}
		}

		void AddContextCompletion(CompletionDataWrapper wrapper, AlResolver state, AstNode node)
		{
			int i = offset - 1;
			var isInGlobalDelegate = node == null && state.CurrentTypeDefinition == null && GetPreviousToken(ref i, true) == "delegate";

			if (state != null && !(node is AstType)) {
				foreach (var variable in state.LocalVariables) {
					if (variable.Region.IsInside(location.Line, location.Column - 1)) {
						continue;
					}
					wrapper.AddVariable(variable);
				}
			}

			if (state.CurrentMember is IParameterizedMember && !(node is AstType)) {
				var param = (IParameterizedMember)state.CurrentMember;
				foreach (var p in param.Parameters) {
					wrapper.AddVariable(p);
				}
			}

			if (state.CurrentMember is IMethod) {
				var method = (IMethod)state.CurrentMember;
				foreach (var p in method.TypeParameters) {
					wrapper.AddTypeParameter(p);
				}
			}

			Func<IType, IType> typePred = null;
			if (IsAttributeContext(node)) {
				var attribute = Compilation.FindType(KnownTypeCode.Attribute);
				typePred = t => t.GetAllBaseTypeDefinitions().Any(bt => bt.Equals(attribute)) ? t : null;
			}
			if (node != null && node.Role == Roles.BaseType) {
				typePred = t => {
					var def = t.GetDefinition();
					if (def != null && t.Kind != TypeKind.Interface && (def.IsSealed || def.IsStatic))
						return null;
					return t;
				};
			}

			if (node != null && !(node is NamespaceDeclaration) || state.CurrentTypeDefinition != null || isInGlobalDelegate) {
				AddTypesAndNamespaces(wrapper, state, node, typePred);

				wrapper.Result.Add(factory.CreateLiteralCompletionData("global"));
			}

			if (!(node is AstType)) {
				if (currentMember != null || node is Expression) {
					AddKeywords(wrapper, statementStartKeywords);
					if (LanguageVersion.Major >= 5)
						AddKeywords(wrapper, new [] { "await" });
					AddKeywords(wrapper, expressionLevelKeywords);
					if (node == null || node is TypeDeclaration)
						AddKeywords(wrapper, typeLevelKeywords);
				} else if (currentType != null) {
					AddKeywords(wrapper, typeLevelKeywords);
				} else {
					if (!isInGlobalDelegate && !(node is Attribute))
						AddKeywords(wrapper, globalLevelKeywords);
				}
				var prop = currentMember as IUnresolvedProperty;
				if (prop != null && prop.Setter != null && prop.Setter.Region.IsInside(location)) {
					wrapper.AddCustom("value");
				} 
				if (currentMember is IUnresolvedEvent) {
					wrapper.AddCustom("value");
				} 

				if (IsInSwitchContext(node)) {
					if (IncludeKeywordsInCompletionList)
						wrapper.AddCustom("val"); 
				}
			} else {
				if (((AstType)node).Parent is ParameterDeclaration) {
					AddKeywords(wrapper, parameterTypePredecessorKeywords);
				}
			}

			if (node != null || state.CurrentTypeDefinition != null || isInGlobalDelegate)
				AddKeywords(wrapper, primitiveTypesKeywords);
			if (currentMember != null && (node is IdentifierExpression || node is SimpleType) && (node.Parent is ExpressionStatement || node.Parent is ForeachStatement || node.Parent is UsingStatement)) {
				if (IncludeKeywordsInCompletionList) {
					wrapper.AddCustom("var");
					wrapper.AddCustom("dynamic");
				}
			} 
			wrapper.Result.AddRange(factory.CreateCodeTemplateCompletionData());
			if (node != null && node.Role == Roles.Argument) {
				var resolved = ResolveExpression(node.Parent);
				var invokeResult = resolved != null ? resolved.Result as AlInvocationResolveResult : null;
				if (invokeResult != null) {
					int argNum = 0;
					foreach (var arg in node.Parent.Children.Where (c => c.Role == Roles.Argument)) {
						if (arg == node) {
							break;
						}
						argNum++;
					}
					var param = argNum < invokeResult.Member.Parameters.Count ? invokeResult.Member.Parameters [argNum] : null;
					if (param != null && param.Type.Kind == TypeKind.Enum) {
						AddEnumMembers(wrapper, param.Type, state);
					}
				}
			}

			if (node is Expression) {
				var root = node;
				while (root.Parent != null)
					root = root.Parent;
				var astResolver = CompletionContextProvider.GetResolver(state, root);
				foreach (var type in TypeGuessing.GetValidTypes(astResolver, (Expression)node)) {
					if (type.Kind == TypeKind.Enum) {
						AddEnumMembers(wrapper, type, state);
					} else if (type.Kind == TypeKind.Delegate) {
						AddDelegateHandlers(wrapper, type, false, true);
						AutoSelect = false;
						AutoCompleteEmptyMatch = false;
					}
				}
			}

			// Add 'this' keyword for first parameter (extension method case)
			if (node != null && node.Parent is ParameterDeclaration &&
				node.Parent.PrevSibling != null && node.Parent.PrevSibling.Role == Roles.LPar && IncludeKeywordsInCompletionList) {
				wrapper.AddCustom("self");
			}
		}

		static bool IsInSwitchContext(AstNode node)
		{
			var n = node;
			while (n != null && !(n is EntityDeclaration)) {
				if (n is SwitchStatement) {
					return true;
				}
				if (n is BlockStatement) {
					return false;
				}
				n = n.Parent;
			}
			return false;
		}

		static bool ListEquals(List<INamespace> curNamespaces, List<INamespace> oldNamespaces)
		{
			if (oldNamespaces == null || curNamespaces.Count != oldNamespaces.Count)
				return false;
			for (int i = 0; i < curNamespaces.Count; i++) {
				if (curNamespaces [i].FullName != oldNamespaces [i].FullName) {
					return false;
				}
			}
			return true;
            
		}

		void AddTypesAndNamespaces(CompletionDataWrapper wrapper, AlResolver state, AstNode node, Func<IType, IType> typePred = null, Predicate<IMember> memberPred = null, Action<ICompletionData, IType> callback = null, bool onlyAddConstructors = false)
		{
			var lookup = new MemberLookup(ctx.CurrentTypeDefinition, Compilation.MainAssembly);

			if (currentType != null) {
				for (var ct = ctx.CurrentTypeDefinition; ct != null; ct = ct.DeclaringTypeDefinition) {
					foreach (var nestedType in ct.GetNestedTypes ()) {
						if (!lookup.IsAccessible(nestedType.GetDefinition(), true))
							continue;
						if (onlyAddConstructors) {
							if (!nestedType.GetConstructors().Any(c => lookup.IsAccessible(c, true)))
								continue;
						}

						if (typePred == null) {
							if (onlyAddConstructors)
								wrapper.AddConstructors(nestedType, false, IsAttributeContext(node));
							else
								wrapper.AddType(nestedType, false, IsAttributeContext(node));
							continue;
						}

						var type = typePred(nestedType);
						if (type != null) {
							var a2 = onlyAddConstructors ? wrapper.AddConstructors(type, false, IsAttributeContext(node)) : wrapper.AddType(type, false, IsAttributeContext(node));
							if (a2 != null && callback != null) {
								callback(a2, type);
							}
						}
						continue;
					}
				}

				if (this.currentMember != null && !(node is AstType)) {
					var def = ctx.CurrentTypeDefinition;
					if (def == null && currentType != null)
						def = Compilation.MainAssembly.GetTypeDefinition(currentType.FullTypeName);
					if (def != null) {
						bool isProtectedAllowed = true;

						foreach (var member in def.GetMembers (m => currentMember.IsStatic ? m.IsStatic : true)) {
							if (member is IMethod && ((IMethod)member).FullName == "System.Object.Finalize") {
								continue;
							}
							if (member.SymbolKind == SymbolKind.Operator) {
								continue;
							}
							if (member.IsExplicitInterfaceImplementation) {
								continue;
							}
							if (!lookup.IsAccessible(member, isProtectedAllowed)) {
								continue;
							}
							if (memberPred == null || memberPred(member)) {
								wrapper.AddMember(member);
							}
						}
						var declaring = def.DeclaringTypeDefinition;
						while (declaring != null) {
							foreach (var member in declaring.GetMembers (m => m.IsStatic)) {
								if (memberPred == null || memberPred(member)) {
									wrapper.AddMember(member);
								}
							}
							declaring = declaring.DeclaringTypeDefinition;
						}
					}
				}
				if (ctx.CurrentTypeDefinition != null) {
					foreach (var p in ctx.CurrentTypeDefinition.TypeParameters) {
						wrapper.AddTypeParameter(p);
					}
				}
			}
			var scope = ctx.CurrentUsingScope;

			for (var n = scope; n != null; n = n.Parent) {
				foreach (var pair in n.UsingAliases) {
					wrapper.AddAlias(pair.Key);
				}
				foreach (var alias in n.ExternAliases) {
					wrapper.AddAlias(alias);
				}
				foreach (var u in n.Usings) {
					foreach (var type in u.Types) {
						if (!lookup.IsAccessible(type, false))
							continue;

						IType addType = typePred != null ? typePred(type) : type;

						if (onlyAddConstructors && addType != null) {
							if (!addType.GetConstructors().Any(c => lookup.IsAccessible(c, true)))
								continue;
						}

						if (addType != null) {
							var a = onlyAddConstructors ? wrapper.AddConstructors(addType, false, IsAttributeContext(node)) : wrapper.AddType(addType, false, IsAttributeContext(node));
							if (a != null && callback != null) {
								callback(a, type);
							}
						}
					}
				}

				foreach (var type in n.Namespace.Types) {
					if (!lookup.IsAccessible(type, false))
						continue;
					IType addType = typePred != null ? typePred(type) : type;
					if (onlyAddConstructors && addType != null) {
						if (!addType.GetConstructors().Any(c => lookup.IsAccessible(c, true)))
							continue;
					}

					if (addType != null) {
						var a2 = onlyAddConstructors ? wrapper.AddConstructors(addType, false, IsAttributeContext(node)) : wrapper.AddType(addType, false);
						if (a2 != null && callback != null) {
							callback(a2, type);
						}
					}
				}
			}

			for (var n = scope; n != null; n = n.Parent) {
				foreach (var curNs in n.Namespace.ChildNamespaces) {
					wrapper.AddNamespace(lookup, curNs);
				}
			}

			if (node is AstType && node.Parent is Constraint && IncludeKeywordsInCompletionList) {
				wrapper.AddCustom("new()");
			}

			if (AutomaticallyAddImports) {
				state = GetState();
				ICompletionData[] importData;

				var namespaces = new List<INamespace>();
				for (var n = ctx.CurrentUsingScope; n != null; n = n.Parent) {
					namespaces.Add(n.Namespace);
					foreach (var u in n.Usings)
						namespaces.Add(u);
				}

				if (this.CompletionEngineCache != null && ListEquals(namespaces, CompletionEngineCache.namespaces)) {
					importData = CompletionEngineCache.importCompletion;
				} else {
					// flatten usings
					var importList = new List<ICompletionData>();
					var dict = new Dictionary<string, Dictionary<string, ICompletionData>>();
					foreach (var type in Compilation.GetTopLevelTypeDefinitons ()) {
						if (!lookup.IsAccessible(type, false))
							continue;
						if (namespaces.Any(n => n.FullName == type.Namespace))
							continue;
						bool useFullName = false;
						foreach (var ns in namespaces) {
							if (ns.GetTypeDefinition(type.Name, type.TypeParameterCount) != null) {
								useFullName = true;
								break;
							}
						}

						if (onlyAddConstructors) {
							if (!type.GetConstructors().Any(c => lookup.IsAccessible(c, true)))
								continue;
						}
						var data = factory.CreateImportCompletionData(type, useFullName, onlyAddConstructors);
						Dictionary<string, ICompletionData> createdDict;
						if (!dict.TryGetValue(type.Name, out createdDict)) {
							createdDict = new Dictionary<string, ICompletionData>();
							dict.Add(type.Name, createdDict);
						}
						ICompletionData oldData;
						if (!createdDict.TryGetValue(type.Namespace, out oldData)) {
							importList.Add(data);
							createdDict.Add(type.Namespace, data);
						} else {
							oldData.AddOverload(data); 
						}
					}

					importData = importList.ToArray();
					if (CompletionEngineCache != null) {
						CompletionEngineCache.namespaces = namespaces;
						CompletionEngineCache.importCompletion = importData;
					}
				}
				foreach (var data in importData) {
					wrapper.Result.Add(data);
				}


			}

		}

		IEnumerable<ICompletionData> HandleKeywordCompletion(int wordStart, string word)
		{
			if (IsInsideCommentStringOrDirective()) {
				if (IsInPreprocessorDirective()) {
					if (word == "if" || word == "elif") {
						if (wordStart > 0 && document.GetCharAt(wordStart - 1) == '#') {
							return factory.CreatePreProcessorDefinesCompletionData();
						}
					}
				}
				return null;
			}
            if(IsInsideAsmCode())
            {

            }
			switch (word) {
				case "program":
					return null;
                case "using":
                case "include":
					if (currentType != null) {
						return null;
					}
					var wrapper = new CompletionDataWrapper(this);
					AddTypesAndNamespaces(wrapper, GetState(), null, t => null);
					return wrapper.Result;
                
             
				case "val":
					return CreateCaseCompletionData(location);
					//				case ",":
					//				case ":":
					//					if (result.ExpressionContext == ExpressionContext.InheritableType) {
					//						IType cls = NRefactoryResolver.GetTypeAtCursor (Document.CompilationUnit, Document.FileName, new TextLocation (completionContext.TriggerLine, completionContext.TriggerLineOffset));
					//						CompletionDataList completionList = new ProjectDomCompletionDataList ();
					//						List<string > namespaceList = GetUsedNamespaces ();
					//						var col = new AlTextEditorCompletion.CompletionDataCollector (this, dom, completionList, Document.CompilationUnit, null, location);
					//						bool isInterface = false;
					//						HashSet<string > baseTypeNames = new HashSet<string> ();
					//						if (cls != null) {
					//							baseTypeNames.Add (cls.Name);
					//							if (cls.ClassType == ClassType.Struct)
					//								isInterface = true;
					//						}
					//						int tokenIndex = offset;
					//	
					//						// Search base types " : [Type1, ... ,TypeN,] <Caret>"
					//						string token = null;
					//						do {
					//							token = GetPreviousToken (ref tokenIndex, false);
					//							if (string.IsNullOrEmpty (token))
					//								break;
					//							token = token.Trim ();
					//							if (Char.IsLetterOrDigit (token [0]) || token [0] == '_') {
					//								IType baseType = dom.SearchType (Document.CompilationUnit, cls, result.Region.Start, token);
					//								if (baseType != null) {
					//									if (baseType.ClassType != ClassType.Interface)
					//										isInterface = true;
					//									baseTypeNames.Add (baseType.Name);
					//								}
					//							}
					//						} while (token != ":");
					//						foreach (object o in dom.GetNamespaceContents (namespaceList, true, true)) {
					//							IType type = o as IType;
					//							if (type != null && (type.IsStatic || type.IsSealed || baseTypeNames.Contains (type.Name) || isInterface && type.ClassType != ClassType.Interface)) {
					//								continue;
					//							}
					//							if (o is Namespace && !namespaceList.Any (ns => ns.StartsWith (((Namespace)o).FullName)))
					//								continue;
					//							col.Add (o);
					//						}
					//						// Add inner classes
					//						Stack<IType > innerStack = new Stack<IType> ();
					//						innerStack.Push (cls);
					//						while (innerStack.Count > 0) {
					//							IType curType = innerStack.Pop ();
					//							if (curType == null)
					//								continue;
					//							foreach (IType innerType in curType.InnerTypes) {
					//								if (innerType != cls)
					//									// don't add the calling class as possible base type
					//									col.Add (innerType);
					//							}
					//							if (curType.DeclaringType != null)
					//								innerStack.Push (curType.DeclaringType);
					//						}
					//						return completionList;
					//					}
					//					break;
				case "is":
				case "as":
					if (currentType == null) {
						return null;
					}
					IType isAsType = null;
					var isAsExpression = GetExpressionAt(wordStart);
					if (isAsExpression != null) {
						var parent = isAsExpression.Node.Parent;
						if (parent is VariableInitializer) {
							parent = parent.Parent;
						}
						var varDecl = parent as VariableDeclarationStatement;
						if (varDecl != null) {
							ExpressionResolveResult resolved;
							if (varDecl.Type.IsVar()) {
								resolved = null;
							} else {
								resolved = ResolveExpression(parent);
							}
							if (resolved != null) {
								isAsType = resolved.Result.Type;
							}
						}
					}
					var isAsWrapper = new CompletionDataWrapper(this);
					var def = isAsType != null ? isAsType.GetDefinition() : null;
					AddTypesAndNamespaces(
						isAsWrapper,
						GetState(),
						null,
						t => t.GetDefinition() == null || def == null || t.GetDefinition().IsDerivedFrom(def) ? t : null,
						m => false);
					AddKeywords(isAsWrapper, primitiveTypesKeywords);
					return isAsWrapper.Result;
					//					{
					//						CompletionDataList completionList = new ProjectDomCompletionDataList ();
					//						ExpressionResult expressionResult = FindExpression (dom, completionContext, wordStart - document.Caret.Offset);
					//						NRefactoryResolver resolver = CreateResolver ();
					//						ResolveResult resolveResult = resolver.Resolve (expressionResult, new TextLocation (completionContext.TriggerLine, completionContext.TriggerLineOffset));
					//						if (resolveResult != null && resolveResult.ResolvedType != null) {
					//							CompletionDataCollector col = new CompletionDataCollector (this, dom, completionList, Document.CompilationUnit, resolver.CallingType, location);
					//							IType foundType = null;
					//							if (word == "as") {
					//								ExpressionContext exactContext = new NewAlExpressionFinder (dom).FindExactContextForAsCompletion (document, Document.CompilationUnit, Document.FileName, resolver.CallingType);
					//								if (exactContext is ExpressionContext.TypeExpressionContext) {
					//									foundType = resolver.SearchType (((ExpressionContext.TypeExpressionContext)exactContext).Type);
					//									AddAsCompletionData (col, foundType);
					//								}
					//							}
					//						
					//							if (foundType == null)
					//								foundType = resolver.SearchType (resolveResult.ResolvedType);
					//						
					//							if (foundType != null) {
					//								if (foundType.ClassType == ClassType.Interface)
					//									foundType = resolver.SearchType (DomReturnType.Object);
					//							
					//								foreach (IType type in dom.GetSubclasses (foundType)) {
					//									if (type.IsSpecialName || type.Name.StartsWith ("<"))
					//										continue;
					//									AddAsCompletionData (col, type);
					//								}
					//							}
					//							List<string > namespaceList = GetUsedNamespaces ();
					//							foreach (object o in dom.GetNamespaceContents (namespaceList, true, true)) {
					//								if (o is IType) {
					//									IType type = (IType)o;
					//									if (type.ClassType != ClassType.Interface || type.IsSpecialName || type.Name.StartsWith ("<"))
					//										continue;
					//	//								if (foundType != null && !dom.GetInheritanceTree (foundType).Any (x => x.FullName == type.FullName))
					//	//									continue;
					//									AddAsCompletionData (col, type);
					//									continue;
					//								}
					//								if (o is Namespace)
					//									continue;
					//								col.Add (o);
					//							}
					//							return completionList;
					//						}
					//						result.ExpressionContext = ExpressionContext.TypeName;
					//						return CreateCtrlSpaceCompletionData (completionContext, result);
					//					}
				case "override":
					// Look for modifiers, in order to find the beginning of the declaration
					int firstMod = wordStart;
					int i = wordStart;
					for (int n = 0; n < 3; n++) {
						string mod = GetPreviousToken(ref i, true);
						if (mod == "public" || mod == "protected" || mod == "private" || mod == "internal" || mod == "final") {
							firstMod = i;
						} else if (mod == "static") {
							// static methods are not overridable
							return null;
						} else {
							break;
						}
					}
					if (!IsLineEmptyUpToEol()) {
						return null;
					}
					if (currentType != null && (currentType.Kind == TypeKind.Class || currentType.Kind == TypeKind.Struct)) {
						string modifiers = document.GetText(firstMod, wordStart - firstMod);
						return GetOverrideCompletionData(currentType, modifiers);
					}
					return null;
				case "shared":
					// Look for modifiers, in order to find the beginning of the declaration
					firstMod = wordStart;
					i = wordStart;
					for (int n = 0; n < 3; n++) {
						string mod = GetPreviousToken(ref i, true);
						if (mod == "public" || mod == "protected" || mod == "private" || mod == "internal" || mod == "final") {
							firstMod = i;
						} else if (mod == "static") {
							// static methods are not overridable
							return null;
						} else {
							break;
						}
					}
					if (!IsLineEmptyUpToEol()) {
						return null;
					}
					var state = GetState();

					if (state.CurrentTypeDefinition != null && (state.CurrentTypeDefinition.Kind == TypeKind.Class || state.CurrentTypeDefinition.Kind == TypeKind.Struct)) {
						string modifiers = document.GetText(firstMod, wordStart - firstMod);
						return GetPartialCompletionData(state.CurrentTypeDefinition, modifiers);
					}
					return null;

				case "public":
				case "protected":
				case "private":
				case "internal":
				case "final":
				case "static":
					var accessorContext = HandleAccessorContext();
					if (accessorContext != null) {
						return accessorContext;
					}
					return null;
				case "new":
					int j = offset - 4;
					//				string token = GetPreviousToken (ref j, true);

					IType hintType = null;
					var expressionOrVariableDeclaration = GetNewExpressionAt(j);
					if (expressionOrVariableDeclaration == null)
						return null;
					var astResolver = CompletionContextProvider.GetResolver(GetState(), expressionOrVariableDeclaration.Node.Ancestors.FirstOrDefault(n => n is EntityDeclaration || n is SyntaxTree));
					hintType = TypeGuessing.GetValidTypes(
						astResolver,
						expressionOrVariableDeclaration.Node
					).FirstOrDefault();

					return CreateConstructorCompletionData(hintType);
				case "yield":
					var yieldDataList = new CompletionDataWrapper(this);
					DefaultCompletionString = "ret";
					if (IncludeKeywordsInCompletionList) {
						yieldDataList.AddCustom("leave");
						yieldDataList.AddCustom("ret");
					}
					return yieldDataList.Result;
				case "in":
					var inList = new CompletionDataWrapper(this);

					var expr = GetExpressionAtCursor();
					if (expr == null)
						return null;
					var rr = ResolveExpression(expr);

					AddContextCompletion(
						inList,
						rr != null ? rr.Resolver : GetState(),
						expr.Node
					);
					return inList.Result;
			}
			return null;
		}

		bool IsLineEmptyUpToEol()
		{
			var line = document.GetLineByNumber(location.Line);
			for (int j = offset; j < line.EndOffset; j++) {
				char ch = document.GetCharAt(j);
				if (!char.IsWhiteSpace(ch)) {
					return false;
				}
			}
			return true;
		}

		string GetLineIndent(int lineNr)
		{
			var line = document.GetLineByNumber(lineNr);
			for (int j = line.Offset; j < line.EndOffset; j++) {
				char ch = document.GetCharAt(j);
				if (!char.IsWhiteSpace(ch)) {
					return document.GetText(line.Offset, j - line.Offset);
				}
			}
			return "";
		}
		//		static AlAmbience amb = new AlAmbience();
		class Category : CompletionCategory
		{
			public Category(string displayText, string icon) : base(displayText, icon)
			{
			}

			public override int CompareTo(CompletionCategory other)
			{
				return 0;
			}
		}

		IEnumerable<ICompletionData> CreateConstructorCompletionData(IType hintType)
		{
			var wrapper = new CompletionDataWrapper(this);
			var state = GetState();
			Func<IType, IType> pred = null;
			Action<ICompletionData, IType> typeCallback = null;
			var inferredTypesCategory = new Category("Inferred Types", null);
			var derivedTypesCategory = new Category("Derived Types", null);

			if (hintType != null && (hintType.Kind != TypeKind.TypeParameter || IsTypeParameterInScope(hintType))) {
				if (hintType.Kind != TypeKind.Unknown) {
					var lookup = new MemberLookup(
						ctx.CurrentTypeDefinition,
						Compilation.MainAssembly
					);
					typeCallback = (data, t) => {
						//check if type is in inheritance tree.
						if (hintType.GetDefinition() != null &&
							t.GetDefinition() != null &&
							t.GetDefinition().IsDerivedFrom(hintType.GetDefinition())) {
							data.CompletionCategory = derivedTypesCategory;
						}
					};
					pred = t => {
						if (t.Kind == TypeKind.Interface && hintType.Kind != TypeKind.Array) {
							return null;
						}
						// check for valid constructors
						if (t.GetConstructors().Any()) {
							bool isProtectedAllowed = currentType != null ? 
								currentType.Resolve(ctx).GetDefinition().IsDerivedFrom(t.GetDefinition()) : false;
							if (!t.GetConstructors().Any(m => lookup.IsAccessible(m, isProtectedAllowed))) {
								return null;
							}
						}

						// check derived types
						var typeDef = t.GetDefinition();
						var hintDef = hintType.GetDefinition();
						if (typeDef != null && hintDef != null && typeDef.IsDerivedFrom(hintDef)) {
							var newType = wrapper.AddType(t, true);
							if (newType != null) {
								newType.CompletionCategory = inferredTypesCategory;
							}
						}

						// check type inference
						var typeInference = new TypeInference(Compilation);
						typeInference.Algorithm = TypeInferenceAlgorithm.ImprovedReturnAllResults;

						var inferedType = typeInference.FindTypeInBounds(new [] { t }, new [] { hintType });
						if (inferedType != SpecialType.UnknownType) {
							var newType = wrapper.AddType(inferedType, true);
							if (newType != null) {
								newType.CompletionCategory = inferredTypesCategory;
							}
							return null;
						}
						return t;
					};
					if (!(hintType.Kind == TypeKind.Interface && hintType.Kind != TypeKind.Array)) {
						var hint = wrapper.AddType(hintType, true);
						if (hint != null) {
							DefaultCompletionString = hint.DisplayText;
							hint.CompletionCategory = derivedTypesCategory;
						}
					}
					if (hintType is ParameterizedType && hintType.TypeParameterCount == 1 && hintType.FullName == "System.Collections.Generic.IEnumerable") {
						var arg = ((ParameterizedType)hintType).TypeArguments.FirstOrDefault();
						if (arg.Kind != TypeKind.TypeParameter) {
							var array = new ArrayType(ctx.Compilation, arg, 1);
							wrapper.AddType(array, true);
						}
					}
				} else {
					var hint = wrapper.AddType(hintType, true);
					if (hint != null) {
						DefaultCompletionString = hint.DisplayText;
						hint.CompletionCategory = derivedTypesCategory;
					}
				}
			} 
			AddTypesAndNamespaces(wrapper, state, null, pred, m => false, typeCallback, true);
			if (hintType == null || hintType == SpecialType.UnknownType) {
				AddKeywords(wrapper, primitiveTypesKeywords.Where(k => k != "sub"));
			}

			CloseOnSquareBrackets = true;
			AutoCompleteEmptyMatch = true;
			AutoCompleteEmptyMatchOnCurlyBracket = false;
			return wrapper.Result;
		}

		bool IsTypeParameterInScope(IType hintType)
		{
			var tp = hintType as ITypeParameter;
			var ownerName = tp.Owner.ReflectionName;
			if (currentMember != null && ownerName == currentMember.ReflectionName)
				return true;
			var ot = currentType;
			while (ot != null) {
				if (ownerName == ot.ReflectionName)
					return true;
				ot = ot.DeclaringTypeDefinition;
			}
			return false;
		}

		IEnumerable<ICompletionData> GetOverrideCompletionData(IUnresolvedTypeDefinition type, string modifiers)
		{
			var wrapper = new CompletionDataWrapper(this);
			var alreadyInserted = new List<IMember>();
			//bool addedVirtuals = false;

			int declarationBegin = offset;
			int j = declarationBegin;
			for (int i = 0; i < 3; i++) {
				switch (GetPreviousToken(ref j, true)) {
					case "public":
					case "protected":
					case "private":
					case "internal":
					case "final":
					case "override":
					case "shared":
					case "async":
						declarationBegin = j;
						break;
					case "static":
						return null; // don't add override completion for static members
				}
			}
			AddVirtuals(
				alreadyInserted,
				wrapper,
				modifiers,
				type.Resolve(ctx),
				declarationBegin
			);
			return wrapper.Result;
		}

		IEnumerable<ICompletionData> GetPartialCompletionData(ITypeDefinition type, string modifiers)
		{
			var wrapper = new CompletionDataWrapper(this);
			int declarationBegin = offset;
			int j = declarationBegin;
			for (int i = 0; i < 3; i++) {
				switch (GetPreviousToken(ref j, true)) {
					case "public":
					case "protected":
					case "private":
					case "internal":
					case "final":
					case "override":
					case "shared":
					case "async":
						declarationBegin = j;
						break;
					case "static":
						return null; // don't add override completion for static members
				}
			}

			var methods = new List<IUnresolvedMethod>();

			foreach (var part in type.Parts) {
				foreach (var method in part.Methods) {
					if (method.BodyRegion.IsEmpty) {
						if (GetImplementation(type, method) != null) {
							continue;
						}
						methods.Add(method);
					}
				}	
			}

			foreach (var method in methods) {
				wrapper.Add(factory.CreateNewPartialCompletionData(
					declarationBegin,
					method.DeclaringTypeDefinition,
					method
				)
				);
			} 

			return wrapper.Result;
		}

		IMethod GetImplementation(ITypeDefinition type, IUnresolvedMethod method)
		{
			foreach (var cur in type.Methods) {
				if (cur.Name == method.Name && cur.Parameters.Count == method.Parameters.Count && !cur.BodyRegion.IsEmpty) {
					bool equal = true;
					/*					for (int i = 0; i < cur.Parameters.Count; i++) {
						if (!cur.Parameters [i].Type.Equals (method.Parameters [i].Type)) {
							equal = false;
							break;
						}
					}*/
					if (equal) {
						return cur;
					}
				}
			}
			return null;
		}

		protected virtual void AddVirtuals(List<IMember> alreadyInserted, CompletionDataWrapper col, string modifiers, IType curType, int declarationBegin)
		{
			if (curType == null) {
				return;
			}
			foreach (var m in curType.GetMembers ().Reverse ()) {
				if (curType.Kind != TypeKind.Interface && !m.IsOverridable) {
					continue;
				}
				// filter out the "Finalize" methods, because finalizers should be done with destructors.
				if (m is IMethod && m.Name == "Finalize") {
					continue;
				}

				var data = factory.CreateNewOverrideCompletionData(
					declarationBegin,
					currentType,
					m
				);
				// check if the member is already implemented
				bool foundMember = curType.GetMembers().Any(cm => SignatureComparer.Ordinal.Equals(
					cm,
					m
				) && cm.DeclaringTypeDefinition == curType.GetDefinition()
				);
				if (foundMember) {
					continue;
				}
				if (alreadyInserted.Any(cm => SignatureComparer.Ordinal.Equals(cm, m)))
					continue;
				alreadyInserted.Add(m);
				data.CompletionCategory = col.GetCompletionCategory(m.DeclaringTypeDefinition);
				col.Add(data);
			}
		}

		void AddKeywords(CompletionDataWrapper wrapper, IEnumerable<string> keywords)
		{
			if (!IncludeKeywordsInCompletionList)
				return;
			foreach (string keyword in keywords) {
				if (wrapper.Result.Any(data => data.DisplayText == keyword))
					continue;
				wrapper.AddCustom(keyword);
			}
		}

        void AddAsmKeywords(CompletionDataWrapper wrapper, Dictionary<string,string> keywords)
        {
            if (!IncludeKeywordsInCompletionList)
                return;
            foreach (KeyValuePair<string,string> keyword in keywords)
            {
                if (wrapper.Result.Any(data => data.DisplayText == keyword.Key))
                    continue;
                wrapper.AddCustom(keyword.Key, keyword.Value);
            }
        }

		public string GuessEventHandlerMethodName(int tokenIndex, string surroundingTypeName)
		{
			var names = new List<string>();
			
			string eventName = GetPreviousToken(ref tokenIndex, false);
			string result = GetPreviousToken(ref tokenIndex, false);
			if (result != ".") {
				if (surroundingTypeName == null) {
					eventName = "Handle" + eventName;
				} else {
					names.Add(surroundingTypeName);
				}
			}
			while (result == ".") {
				result = GetPreviousToken(ref tokenIndex, false);
				if (result == "self") {
					if (names.Count == 0) {
						if (surroundingTypeName == null) {
							eventName = "Handle" + eventName;
						} else {
							names.Add(surroundingTypeName);
						}
					}
				} else if (result != null) {
					string trimmedName = result.Trim();
					if (trimmedName.Length == 0) {
						break;
					}
					names.Insert(0, trimmedName);
				}
				result = GetPreviousToken(ref tokenIndex, false);
			}
			if (!string.IsNullOrEmpty(eventName)) {
				names.Add(eventName);
			}
			result = String.Join("_", names.ToArray());
			foreach (char ch in result) {
				if (!char.IsLetterOrDigit(ch) && ch != '_') {
					result = "";
					break;
				}
			}
			return result;
		}

		bool MatchDelegate(IType delegateType, IMethod method)
		{
			if (method.SymbolKind != SymbolKind.Method)
				return false;
			var delegateMethod = delegateType.GetDelegateInvokeMethod();
			if (delegateMethod == null || delegateMethod.Parameters.Count != method.Parameters.Count) {
				return false;
			}

			for (int i = 0; i < delegateMethod.Parameters.Count; i++) {
				if (!delegateMethod.Parameters [i].Type.Equals(method.Parameters [i].Type)) {
					return false;
				}
			}
			return true;
		}

		string AddDelegateHandlers(CompletionDataWrapper completionList, IType delegateType, bool addSemicolon = true, bool addDefault = true, string optDelegateName = null)
		{
			IMethod delegateMethod = delegateType.GetDelegateInvokeMethod();
			PossibleDelegates.Add(delegateMethod);
			var thisLineIndent = GetLineIndent(location.Line);
			string delegateEndString = EolMarker + thisLineIndent + "}" + (addSemicolon ? ";" : "");
			//bool containsDelegateData = completionList.Result.Any(d => d.DisplayText.StartsWith("delegate("));
			if (addDefault && !completionList.AnonymousDelegateAdded) {
				completionList.AnonymousDelegateAdded = true;
				var oldDelegate = completionList.Result.FirstOrDefault(cd => cd.DisplayText == "delegate");
				if (oldDelegate != null)
					completionList.Result.Remove(oldDelegate);
				completionList.AddCustom(
					"delegate",
					"Creates anonymous delegate.",
					"delegate {" + EolMarker + thisLineIndent + IndentString + "|" + delegateEndString
				).DisplayFlags |= DisplayFlags.MarkedBold;
				if (LanguageVersion.Major >= 5) {
					completionList.AddCustom(
						"async delegate",
						"Creates anonymous async delegate.",
						"async delegate {" + EolMarker + thisLineIndent + IndentString + "|" + delegateEndString
					).DisplayFlags |= DisplayFlags.MarkedBold;
				}
			}
			var sb = new StringBuilder("(");
			var sbWithoutTypes = new StringBuilder("(");
			var state = GetState();
			var builder = new TypeSystemAstBuilder(state);

			for (int k = 0; k < delegateMethod.Parameters.Count; k++) {

				if (k > 0) {
					sb.Append(", ");
					sbWithoutTypes.Append(", ");
				}
				var convertedParameter = builder.ConvertParameter(delegateMethod.Parameters [k]);
				if (convertedParameter.ParameterModifier == ParameterModifier.Params)
					convertedParameter.ParameterModifier = ParameterModifier.None;
				sb.Append(convertedParameter.ToString(FormattingPolicy));
				sbWithoutTypes.Append(delegateMethod.Parameters [k].Name);
			}

			sb.Append(")");
			sbWithoutTypes.Append(")");
			var signature = sb.ToString();
			if (!completionList.HasAnonymousDelegateAdded(signature)) {
				completionList.AddAnonymousDelegateAdded(signature);

				completionList.AddCustom(
					"delegate" + signature,
					"Creates anonymous delegate.",
					"delegate" + signature + " {" + EolMarker + thisLineIndent + IndentString + "|" + delegateEndString
				).DisplayFlags |= DisplayFlags.MarkedBold;
				if (LanguageVersion.Major >= 5) {
					completionList.AddCustom(
						"async delegate" + signature,
						"Creates anonymous async delegate.",
						"async delegate" + signature + " {" + EolMarker + thisLineIndent + IndentString + "|" + delegateEndString
					).DisplayFlags |= DisplayFlags.MarkedBold;
				}
				if (!completionList.Result.Any(data => data.DisplayText == sb.ToString())) {
					completionList.AddCustom(
						signature,
						"Creates typed lambda expression.",
						signature + " => |" + (addSemicolon ? ";" : "")
					).DisplayFlags |= DisplayFlags.MarkedBold;
					if (LanguageVersion.Major >= 5) {
						completionList.AddCustom(
							"async " + signature,
							"Creates typed async lambda expression.",
							"async " + signature + " => |" + (addSemicolon ? ";" : "")
						).DisplayFlags |= DisplayFlags.MarkedBold;
					}

					if (!delegateMethod.Parameters.Any(p => p.IsOut || p.IsRef) && !completionList.Result.Any(data => data.DisplayText == sbWithoutTypes.ToString())) {
						completionList.AddCustom(
							sbWithoutTypes.ToString(),
							"Creates lambda expression.",
							sbWithoutTypes + " => |" + (addSemicolon ? ";" : "")
						).DisplayFlags |= DisplayFlags.MarkedBold;
						if (LanguageVersion.Major >= 5) {
							completionList.AddCustom(
								"async " + sbWithoutTypes,
								"Creates async lambda expression.",
								"async " + sbWithoutTypes + " => |" + (addSemicolon ? ";" : "")
							).DisplayFlags |= DisplayFlags.MarkedBold;
						}
					}
				}

			}

			string varName = optDelegateName ?? "Handle" + delegateType.Name;

			var ecd = factory.CreateEventCreationCompletionData(varName, delegateType, null, signature, currentMember, currentType);
			ecd.DisplayFlags |= DisplayFlags.MarkedBold;
			completionList.Add(ecd);

			return sb.ToString();
		}

		bool IsAccessibleFrom(IEntity member, ITypeDefinition calledType, IMember currentMember, bool includeProtected)
		{
			if (currentMember == null) {
				return member.IsStatic || member.IsPublic;
			}
			//			if (currentMember is MonoDevelop.Projects.Dom.BaseResolveResult.BaseMemberDecorator) 
			//				return member.IsPublic | member.IsProtected;
			//		if (member.IsStatic && !IsStatic)
			//			return false;
			if (member.IsPublic || calledType != null && calledType.Kind == TypeKind.Interface && !member.IsProtected) {
				return true;
			}
			if (member.DeclaringTypeDefinition != null) {
				if (member.DeclaringTypeDefinition.Kind == TypeKind.Interface) { 
					return IsAccessibleFrom(
						member.DeclaringTypeDefinition,
						calledType,
						currentMember,
						includeProtected
					);
				}

				if (member.IsProtected && !(member.DeclaringTypeDefinition.IsProtectedOrInternal && !includeProtected)) {
					return includeProtected;
				}
			}
			if (member.IsInternal || member.IsProtectedAndInternal || member.IsProtectedOrInternal) {
				//var type1 = member is ITypeDefinition ? (ITypeDefinition)member : member.DeclaringTypeDefinition;
				//var type2 = currentMember is ITypeDefinition ? (ITypeDefinition)currentMember : currentMember.DeclaringTypeDefinition;
				bool result = true;
				// easy case, projects are the same
				/*				//				if (type1.ProjectContent == type2.ProjectContent) {
				//					result = true; 
				//				} else 
				if (type1.ProjectContent != null) {
					// maybe type2 hasn't project dom set (may occur in some cases), check if the file is in the project
					//TODO !!
					//					result = type1.ProjectContent.Annotation<MonoDevelop.Projects.Project> ().GetProjectFile (type2.Region.FileName) != null;
					result = false;
				} else if (type2.ProjectContent != null) {
					//TODO!!
					//					result = type2.ProjectContent.Annotation<MonoDevelop.Projects.Project> ().GetProjectFile (type1.Region.FileName) != null;
					result = false;
				} else {
					// should never happen !
					result = true;
				}*/
				return member.IsProtectedAndInternal ? includeProtected && result : result;
			}

			if (!(currentMember is IType) && (currentMember.DeclaringTypeDefinition == null || member.DeclaringTypeDefinition == null)) {
				return false;
			}

			// inner class 
			var declaringType = currentMember.DeclaringTypeDefinition;
			while (declaringType != null) {
				if (declaringType.ReflectionName == currentMember.DeclaringType.ReflectionName) {
					return true;
				}
				declaringType = declaringType.DeclaringTypeDefinition;
			}


			return currentMember.DeclaringTypeDefinition != null && member.DeclaringTypeDefinition.FullName == currentMember.DeclaringTypeDefinition.FullName;
		}

		static bool IsAttributeContext(AstNode node)
		{
			AstNode n = node;
			while (n is AstType) {
				n = n.Parent;
			}
			return n is Attribute;
		}

		IEnumerable<ICompletionData> CreateTypeAndNamespaceCompletionData(TextLocation location, ResolveResult resolveResult, AstNode resolvedNode, AlResolver state)
		{
			if (resolveResult == null || resolveResult.IsError) {
				return null;
			}
			var exprParent = resolvedNode.GetParent<Expression>();
			var unit = exprParent != null ? exprParent.GetParent<SyntaxTree>() : null;

			var astResolver = unit != null ? CompletionContextProvider.GetResolver(state, unit) : null;
			IType hintType = exprParent != null && astResolver != null ? 
				TypeGuessing.GetValidTypes(astResolver, exprParent).FirstOrDefault() :
				null;
			var result = new CompletionDataWrapper(this);
			var lookup = new MemberLookup(
				ctx.CurrentTypeDefinition,
				Compilation.MainAssembly
			);
			if (resolveResult is NamespaceResolveResult) {
				var nr = (NamespaceResolveResult)resolveResult;
				if (!(resolvedNode.Parent is UsingDeclaration || resolvedNode.Parent != null && resolvedNode.Parent.Parent is UsingDeclaration)) {
					foreach (var cl in nr.Namespace.Types) {
						if (hintType != null && hintType.Kind != TypeKind.Array && cl.Kind == TypeKind.Interface) {
							continue;
						}
						if (!lookup.IsAccessible(cl, false))
							continue;
						result.AddType(cl, false, IsAttributeContext(resolvedNode));
					}
				}
				foreach (var ns in nr.Namespace.ChildNamespaces) {
					result.AddNamespace(lookup, ns);
				}
			} else if (resolveResult is TypeResolveResult) {
				var type = resolveResult.Type;
				foreach (var nested in type.GetNestedTypes ()) {
					if (hintType != null && hintType.Kind != TypeKind.Array && nested.Kind == TypeKind.Interface) {
						continue;
					}
					var def = nested.GetDefinition();
					if (def != null && !lookup.IsAccessible(def, false))
						continue;
					result.AddType(nested, false);
				}
			}

            return result.Result;
		
		}

		IEnumerable<ICompletionData> CreateTypeList()
		{
			foreach (var cl in Compilation.RootNamespace.Types) {
				yield return factory.CreateTypeCompletionData(cl, false, false, false);
			}

			foreach (var ns in Compilation.RootNamespace.ChildNamespaces) {
				yield return factory.CreateNamespaceCompletionData(ns);
			}
		}

		void CreateParameterForInvocation(CompletionDataWrapper result, IMethod method, AlResolver state, int parameter, HashSet<string> addedEnums, HashSet<string> addedDelegates)
		{
			if (method.Parameters.Count <= parameter) {
				return;
			}
			var resolvedType = method.Parameters [parameter].Type;
			if (resolvedType.Kind == TypeKind.Enum) {
				if (addedEnums.Contains(resolvedType.ReflectionName)) {
					return;
				}
				addedEnums.Add(resolvedType.ReflectionName);
				AddEnumMembers(result, resolvedType, state);
				return;
			}

			if (resolvedType.Kind == TypeKind.Delegate) {
				if (addedDelegates.Contains(resolvedType.ReflectionName))
					return;
				AddDelegateHandlers(result, resolvedType, false, true, "Handle" + method.Parameters [parameter].Type.Name + method.Parameters [parameter].Name);
			}
		}

		IEnumerable<ICompletionData> CreateParameterCompletion(MethodGroupResolveResult resolveResult, AlResolver state, AstNode invocation, SyntaxTree unit, int parameter, bool controlSpace)
		{
			var result = new CompletionDataWrapper(this);
			var addedEnums = new HashSet<string>();
			var addedDelegates = new HashSet<string>();

			foreach (var method in resolveResult.Methods) {
				CreateParameterForInvocation(result, method, state, parameter, addedEnums, addedDelegates);
			}
			foreach (var methods in resolveResult.GetEligibleExtensionMethods (true)) {
				foreach (var method in methods) {
					if (resolveResult.Methods.Contains(method))
						continue;
					CreateParameterForInvocation(result, new ReducedExtensionMethod(method), state, parameter, addedEnums, addedDelegates);
				}
			}

			foreach (var method in resolveResult.Methods) {
				if (parameter < method.Parameters.Count && method.Parameters [parameter].Type.Kind == TypeKind.Delegate) {
					AutoSelect = false;
					AutoCompleteEmptyMatch = false;
				}
				foreach (var p in method.Parameters) {
					result.AddNamedParameterVariable(p);
				}
			}

			if (!controlSpace) {
				if (addedEnums.Count + addedDelegates.Count == 0) {
					return Enumerable.Empty<ICompletionData>();
				}
				AutoCompleteEmptyMatch = false;
				AutoSelect = false;
			}
			AddContextCompletion(result, state, invocation);

			//			resolver.AddAccessibleCodeCompletionData (ExpressionContext.MethodBody, cdc);
			//			if (addedDelegates.Count > 0) {
			//				foreach (var data in result.Result) {
			//					if (data is MemberCompletionData) 
			//						((MemberCompletionData)data).IsDelegateExpected = true;
			//				}
			//			}
			return result.Result;
		}

		void AddEnumMembers(CompletionDataWrapper completionList, IType resolvedType, AlResolver state)
		{
			if (resolvedType.Kind != TypeKind.Enum) {
				return;
			}
			var type = completionList.AddEnumMembers(resolvedType, state);
			if (type != null)
				DefaultCompletionString = type.DisplayText;
		}

		IEnumerable<ICompletionData> CreateCompletionData(TextLocation location, ResolveResult resolveResult, AstNode resolvedNode, AlResolver state, Func<IType, IType> typePred = null)
		{
			if (resolveResult == null /*			|| resolveResult.IsError*/) {
				return null;
			}

			var lookup = new MemberLookup(
				ctx.CurrentTypeDefinition,
				Compilation.MainAssembly
			);

			if (resolveResult is NamespaceResolveResult) {
				var nr = (NamespaceResolveResult)resolveResult;
				var namespaceContents = new CompletionDataWrapper(this);

				foreach (var cl in nr.Namespace.Types) {
					if (!lookup.IsAccessible(cl, false))
						continue;
					IType addType = typePred != null ? typePred(cl) : cl;
					if (addType != null)
						namespaceContents.AddType(addType, false);
				}

				foreach (var ns in nr.Namespace.ChildNamespaces) {
					namespaceContents.AddNamespace(lookup, ns);
				}
				return namespaceContents.Result;
			}
			IType type = resolveResult.Type;

			if (type.Namespace == "System" && type.Name == "Void")
				return null;

			if (resolvedNode.Parent is PointerReferenceExpression && (type is PointerType)) {
				resolveResult = new OperatorResolveResult(((PointerType)type).ElementType, System.Linq.Expressions.ExpressionType.Extension, resolveResult);
			}

			//var typeDef = resolveResult.Type.GetDefinition();
			var result = new CompletionDataWrapper(this);
			bool includeStaticMembers = false;

			if (resolveResult is LocalResolveResult) {
				if (resolvedNode is IdentifierExpression) {
					var mrr = (LocalResolveResult)resolveResult;
					includeStaticMembers = mrr.Variable.Name == mrr.Type.Name;
				}
			}
			if (resolveResult is TypeResolveResult && type.Kind == TypeKind.Enum) {
				foreach (var field in type.GetFields ()) {
					if (!lookup.IsAccessible(field, false))
						continue;
					result.AddMember(field);
				}
				return result.Result;
			}

			bool isProtectedAllowed = lookup.IsProtectedAccessAllowed(resolveResult);
			bool skipNonStaticMembers = (resolveResult is TypeResolveResult);

			if (resolveResult is MemberResolveResult && resolvedNode is IdentifierExpression) {
				var mrr = (MemberResolveResult)resolveResult;
				includeStaticMembers = mrr.Member.Name == mrr.Type.Name;

				TypeResolveResult trr;
				if (state.IsVariableReferenceWithSameType(
					resolveResult,
					((IdentifierExpression)resolvedNode).Identifier,
					out trr
				)) {
					if (currentMember != null && mrr.Member.IsStatic ^ currentMember.IsStatic) {
						skipNonStaticMembers = true;

						if (trr.Type.Kind == TypeKind.Enum) {
							foreach (var field in trr.Type.GetFields ()) {
								if (lookup.IsAccessible(field, false))
									result.AddMember(field);
							}
							return result.Result;
						}
					}
				}
				// ADD Aliases
				var scope = ctx.CurrentUsingScope;

				for (var n = scope; n != null; n = n.Parent) {
					foreach (var pair in n.UsingAliases) {
						if (pair.Key == mrr.Member.Name) {
							foreach (var r in CreateCompletionData (location, pair.Value, resolvedNode, state)) {
								if (r is IEntityCompletionData && ((IEntityCompletionData)r).Entity is IMember) {
									result.AddMember((IMember)((IEntityCompletionData)r).Entity);
								} else {
									result.Add(r);
								}
							}
						}
					}
				}				


			}
			if (resolveResult is TypeResolveResult && (resolvedNode is IdentifierExpression || resolvedNode is MemberReferenceExpression)) {
				includeStaticMembers = true;
			}

			//			Console.WriteLine ("type:" + type +"/"+type.GetType ());
			//			Console.WriteLine ("current:" + ctx.CurrentTypeDefinition);
			//			Console.WriteLine ("IS PROT ALLOWED:" + isProtectedAllowed + " static: "+ includeStaticMembers);
			//			Console.WriteLine (resolveResult);
			//			Console.WriteLine ("node:" + resolvedNode);
			//			Console.WriteLine (currentMember !=  null ? currentMember.IsStatic : "currentMember == null");

			if (resolvedNode.Annotation<ObjectCreateExpression>() == null) {
				//tags the created expression as part of an object create expression.
				/*				
				var filteredList = new List<IMember>();
				foreach (var member in type.GetMembers ()) {
					filteredList.Add(member);
				}
				
				foreach (var member in filteredList) {
					//					Console.WriteLine ("add:" + member + "/" + member.IsStatic);
					result.AddMember(member);
				}*/
				foreach (var member in lookup.GetAccessibleMembers (resolveResult)) {
					if (member.SymbolKind == SymbolKind.Indexer || member.SymbolKind == SymbolKind.Operator || member.SymbolKind == SymbolKind.Constructor || member.SymbolKind == SymbolKind.Destructor) {
						continue;
					}
					if (resolvedNode is BaseReferenceExpression && member.IsAbstract) {
						continue;
					}
					if (member is IType) {
						if (resolveResult is TypeResolveResult || includeStaticMembers) {
							if (!lookup.IsAccessible(member, isProtectedAllowed))
								continue;
							result.AddType((IType)member, false);
							continue;
						}
					}
					bool memberIsStatic = member.IsStatic;
					if (!includeStaticMembers && memberIsStatic && !(resolveResult is TypeResolveResult)) {
						//						Console.WriteLine ("skip static member: " + member.FullName);
						continue;
					}

					var field = member as IField;
					if (field != null) {
						memberIsStatic |= field.IsConst;
					}
					if (!memberIsStatic && skipNonStaticMembers) {
						continue;
					}

					if (member is IMethod && ((IMethod)member).FullName == "System.Object.Finalize") {
						continue;
					}
					if (member.SymbolKind == SymbolKind.Operator) {
						continue;
					}

					if (member is IMember) {
						result.AddMember((IMember)member);
					}
				}
			}

			if (!(resolveResult is TypeResolveResult || includeStaticMembers)) {
				foreach (var meths in state.GetExtensionMethods (type)) {
					foreach (var m in meths) {
						if (!lookup.IsAccessible(m, isProtectedAllowed))
							continue;
						result.AddMember(new ReducedExtensionMethod(m));
					}
				}
			}

			//			IEnumerable<object> objects = resolveResult.CreateResolveResult (dom, resolver != null ? resolver.CallingMember : null);
			//			CompletionDataCollector col = new CompletionDataCollector (this, dom, result, Document.CompilationUnit, resolver != null ? resolver.CallingType : null, location);
			//			col.HideExtensionParameter = !resolveResult.StaticResolve;
			//			col.NamePrefix = expressionResult.Expression;
			//			bool showOnlyTypes = expressionResult.Contexts.Any (ctx => ctx == ExpressionContext.InheritableType || ctx == ExpressionContext.Constraints);
			//			if (objects != null) {
			//				foreach (object obj in objects) {
			//					if (expressionResult.ExpressionContext != null && expressionResult.ExpressionContext.FilterEntry (obj))
			//						continue;
			//					if (expressionResult.ExpressionContext == ExpressionContext.NamespaceNameExcepted && !(obj is Namespace))
			//						continue;
			//					if (showOnlyTypes && !(obj is IType))
			//						continue;
			//					CompletionData data = col.Add (obj);
			//					if (data != null && expressionResult.ExpressionContext == ExpressionContext.Attribute && data.CompletionText != null && data.CompletionText.EndsWith ("Attribute")) {
			//						string newText = data.CompletionText.Substring (0, data.CompletionText.Length - "Attribute".Length);
			//						data.SetText (newText);
			//					}
			//				}
			//			}

			return result.Result;
		}

		IEnumerable<ICompletionData> CreateCaseCompletionData(TextLocation location)
		{
			var unit = ParseStub("a: break;");
			if (unit == null) {
				return null;
			}
			var s = unit.GetNodeAt<SwitchStatement>(location);
			if (s == null) {
				return null;
			}

			var offset = document.GetOffset(s.Expression.StartLocation);
			var expr = GetExpressionAt(offset);
			if (expr == null) {
				return null;
			}

			var resolveResult = ResolveExpression(expr);
			if (resolveResult == null || resolveResult.Result.Type.Kind != TypeKind.Enum) { 
				return null;
			}
			var wrapper = new CompletionDataWrapper(this);
			AddEnumMembers(wrapper, resolveResult.Result.Type, resolveResult.Resolver);
			AutoCompleteEmptyMatch = false;
			return wrapper.Result;
		}

		#region Parsing methods

		ExpressionResult GetExpressionBeforeCursor()
		{
			SyntaxTree baseUnit;
			if (currentMember == null) {
				baseUnit = ParseStub("a", false);
				var type = baseUnit.GetNodeAt<MemberType>(location);
				if (type == null) {
					baseUnit = ParseStub("a;", false);
					type = baseUnit.GetNodeAt<MemberType>(location);
				}

				if (type == null) {
					baseUnit = ParseStub("A a;", false);
					type = baseUnit.GetNodeAt<MemberType>(location);
				}
				if (type != null) {
					return new ExpressionResult((AstNode)type.Target, baseUnit);
				}
			}

			baseUnit = ParseStub("ToString()", false);
			var curNode = baseUnit.GetNodeAt(location);
			// hack for local variable declaration missing ';' issue - remove that if it works.
			if (curNode is EntityDeclaration || baseUnit.GetNodeAt<Expression>(location) == null && baseUnit.GetNodeAt<MemberType>(location) == null) {
				baseUnit = ParseStub("a");
				curNode = baseUnit.GetNodeAt(location);
			}

			// Hack for handle object initializer continuation expressions
			if (curNode is EntityDeclaration || baseUnit.GetNodeAt<Expression>(location) == null && baseUnit.GetNodeAt<MemberType>(location) == null) {
				baseUnit = ParseStub("a};");
			}
			var mref = baseUnit.GetNodeAt<MemberReferenceExpression>(location); 
			if (currentMember == null && currentType == null) {
				if (mref != null) {
					return new ExpressionResult((AstNode)mref.Target, baseUnit);
				}
				return null;
			}

			//var memberLocation = currentMember != null ? currentMember.Region.Begin : currentType.Region.Begin;
			if (mref == null) {
				var type = baseUnit.GetNodeAt<MemberType>(location); 
				if (type != null) {
					return new ExpressionResult((AstNode)type.Target, baseUnit);
				}

				var pref = baseUnit.GetNodeAt<PointerReferenceExpression>(location); 
				if (pref != null) {
					return new ExpressionResult((AstNode)pref.Target, baseUnit);
				}
			}

			if (mref == null) {
				baseUnit = ParseStub("A a;", false);
				var type = baseUnit.GetNodeAt<MemberType>(location);
				if (type != null) {
					return new ExpressionResult((AstNode)type.Target, baseUnit);
				}
			}

			AstNode expr = null;
			if (mref != null) {
				expr = mref.Target;
			} else {
				Expression tref = baseUnit.GetNodeAt<TypeReferenceExpression>(location); 
				MemberType memberType = tref != null ? ((TypeReferenceExpression)tref).Type as MemberType : null;
				if (memberType == null) {
					memberType = baseUnit.GetNodeAt<MemberType>(location); 
					if (memberType != null) {
						if (memberType.Parent is ObjectCreateExpression) {
							var mt = memberType.Target.Clone();
							memberType.ReplaceWith(mt);
							expr = mt;
							goto exit;
						} else {
							tref = baseUnit.GetNodeAt<Expression>(location); 
							if (tref == null) {
								tref = new TypeReferenceExpression(memberType.Clone());
								memberType.Parent.AddChild(tref, Roles.Expression);
							}
							if (tref is ObjectCreateExpression) {
								expr = memberType.Target.Clone();
								expr.AddAnnotation(new ObjectCreateExpression());
							}
						}
					}
				}

				if (memberType == null) {
					return null;
				}
				if (expr == null) {
					expr = memberType.Target.Clone();
				}
				tref.ReplaceWith(expr);
			}
			exit:
			return new ExpressionResult((AstNode)expr, baseUnit);
		}

		ExpressionResult GetExpressionAtCursor()
		{
			//			TextLocation memberLocation;
			//			if (currentMember != null) {
			//				memberLocation = currentMember.Region.Begin;
			//			} else if (currentType != null) {
			//				memberLocation = currentType.Region.Begin;
			//			} else {
			//				memberLocation = location;
			//			}
			var baseUnit = ParseStub("a");
			var tmpUnit = baseUnit;
			AstNode expr = baseUnit.GetNodeAt(
				location,
				n => n is IdentifierExpression || n is MemberReferenceExpression
			);

			if (expr == null) {
				expr = baseUnit.GetNodeAt<AstType>(location.Line, location.Column - 1);
			}
			if (expr == null)
				expr = baseUnit.GetNodeAt<Identifier>(location.Line, location.Column - 1);
			// try insertStatement
			if (expr == null && baseUnit.GetNodeAt<EmptyStatement>(location.Line, location.Column) != null) {
				tmpUnit = baseUnit = ParseStub("a();", false);
				expr = baseUnit.GetNodeAt<InvocationExpression>(
					location.Line,
					location.Column + 1
				); 
			}

			if (expr == null) {
				baseUnit = ParseStub("()");
				expr = baseUnit.GetNodeAt<IdentifierExpression>(
					location.Line,
					location.Column - 1
				); 
				if (expr == null) {
					expr = baseUnit.GetNodeAt<MemberType>(location.Line, location.Column - 1); 
				}
			}

			if (expr == null) {
				baseUnit = ParseStub("a", false);
				expr = baseUnit.GetNodeAt(
					location,
					n => n is IdentifierExpression || n is MemberReferenceExpression || n is CatchClause
				);
			}

			// try statement 
			if (expr == null) {
				expr = tmpUnit.GetNodeAt<SwitchStatement>(
					location.Line,
					location.Column - 1
				); 
				baseUnit = tmpUnit;
			}

			if (expr == null) {
				var block = tmpUnit.GetNodeAt<BlockStatement>(location); 
				var node = block != null ? block.Statements.LastOrDefault() : null;

				var forStmt = node != null ? node.PrevSibling as ForStatement : null;
				if (forStmt != null && forStmt.EmbeddedStatement.IsNull) {
					expr = forStmt;
					var id = new IdentifierExpression("stub");
					forStmt.EmbeddedStatement = new BlockStatement() { Statements = { new ExpressionStatement(id) } };
					expr = id;
					baseUnit = tmpUnit;
				}
			}

			if (expr == null) {
				var forStmt = tmpUnit.GetNodeAt<ForeachStatement>(
					location.Line,
					location.Column - 3
				); 
				if (forStmt != null && forStmt.EmbeddedStatement.IsNull) {
					forStmt.VariableNameToken = Identifier.Create("stub");
					expr = forStmt.VariableNameToken;
					baseUnit = tmpUnit;
				}
			}
			if (expr == null) {
				expr = tmpUnit.GetNodeAt<VariableInitializer>(
					location.Line,
					location.Column - 1
				);
				baseUnit = tmpUnit;
			}

			// try parameter declaration type
			if (expr == null) {
				baseUnit = ParseStub(">", false, "{}");
				expr = baseUnit.GetNodeAt<TypeParameterDeclaration>(
					location.Line,
					location.Column - 1
				); 
			}

			// try parameter declaration method
			if (expr == null) {
				baseUnit = ParseStub("> ()", false, "{}");
				expr = baseUnit.GetNodeAt<TypeParameterDeclaration>(
					location.Line,
					location.Column - 1
				); 
			}

			// try expression in anonymous type "new { sample = x$" case
			if (expr == null) {
				baseUnit = ParseStub("a", false);
				expr = baseUnit.GetNodeAt<AnonymousTypeCreateExpression>(
					location.Line,
					location.Column
				); 
				if (expr != null) {
					expr = baseUnit.GetNodeAt<Expression>(location.Line, location.Column) ?? expr;
				} 
				if (expr == null) {
					expr = baseUnit.GetNodeAt<AstType>(location.Line, location.Column);
				} 
			}

			// try lambda 
			if (expr == null) {
				baseUnit = ParseStub("foo) => {}", false);
				expr = baseUnit.GetNodeAt<ParameterDeclaration>(
					location.Line,
					location.Column
				); 
			}

			if (expr == null)
				return null;
			return new ExpressionResult(expr, baseUnit);
		}

		ExpressionResult GetExpressionAt(int offset)
		{
			var parser = new AlParser();
			var text = GetMemberTextToCaret(); 

			int closingBrackets = 0, generatedLines = 0;
			var sb = CreateWrapper("a;", false, "", text.Item1, text.Item2, ref closingBrackets, ref generatedLines);

			var completionUnit = parser.Parse(sb.ToString());
			var offsetLocation = document.GetLocation(offset);
			var loc = new TextLocation(offsetLocation.Line - text.Item2.Line + generatedLines + 1, offsetLocation.Column);

			var expr = completionUnit.GetNodeAt(
				loc,
				n => n is Expression || n is VariableDeclarationStatement
			);
			if (expr == null)
				return null;
			return new ExpressionResult(expr, completionUnit);
		}

		ExpressionResult GetNewExpressionAt(int offset)
		{
			var parser = new AlParser();
			var text = GetMemberTextToCaret();
			int closingBrackets = 0, generatedLines = 0;
			var sb = CreateWrapper("a ();", false, "", text.Item1, text.Item2, ref closingBrackets, ref generatedLines);

			var completionUnit = parser.Parse(sb.ToString());
			var offsetLocation = document.GetLocation(offset);
			var loc = new TextLocation(offsetLocation.Line - text.Item2.Line + generatedLines + 1, offsetLocation.Column);

			var expr = completionUnit.GetNodeAt(loc, n => n is Expression);
			if (expr == null) {
				// try without ";"
				sb = CreateWrapper("a ()", false, "", text.Item1, text.Item2, ref closingBrackets, ref generatedLines);
				completionUnit = parser.Parse(sb.ToString());

				expr = completionUnit.GetNodeAt(loc, n => n is Expression);
				if (expr == null) {
					return null;
				}
			}
			return new ExpressionResult(expr, completionUnit);
		}

		#endregion

		#region Helper methods

		string GetPreviousToken(ref int i, bool allowLineChange)
		{
			char c;
			if (i <= 0) {
				return null;
			}

			do {
				c = document.GetCharAt(--i);
			} while (i > 0 && char.IsWhiteSpace(c) && (allowLineChange ? true : c != '\n'));

			if (i == 0) {
				return null;
			}

			if (!char.IsLetterOrDigit(c)) {
				return new string(c, 1);
			}

			int endOffset = i + 1;

			do {
				c = document.GetCharAt(i - 1);
				if (!(char.IsLetterOrDigit(c) || c == '_')) {
					break;
				}

				i--;
			} while (i > 0);

			return document.GetText(i, endOffset - i);
		}

		#endregion

		#region Preprocessor

		IEnumerable<ICompletionData> GetDirectiveCompletionData()
		{
			yield return factory.CreateLiteralCompletionData("if");
			yield return factory.CreateLiteralCompletionData("else");
			yield return factory.CreateLiteralCompletionData("elif");
			yield return factory.CreateLiteralCompletionData("endif");
			yield return factory.CreateLiteralCompletionData("define");
			yield return factory.CreateLiteralCompletionData("undef");
			yield return factory.CreateLiteralCompletionData("warning");
			yield return factory.CreateLiteralCompletionData("error");
			yield return factory.CreateLiteralCompletionData("pragma");
			yield return factory.CreateLiteralCompletionData("line");
			yield return factory.CreateLiteralCompletionData("line hidden");
			yield return factory.CreateLiteralCompletionData("line default");
			yield return factory.CreateLiteralCompletionData("region");
			yield return factory.CreateLiteralCompletionData("endregion");
		}

		#endregion

		#region Xml Comments

		static readonly List<string> commentTags = new List<string>(new string[] {
			"c",
			"code",
			"example",
			"exception",
			"include",
			"list",
			"listheader",
			"item",
			"term",
			"description",
			"para",
			"param",
			"paramref",
			"permission",
			"remarks",
			"returns",
			"see",
			"seealso",
			"summary",
			"value"
		}
		);

		public static IEnumerable<string> CommentTags {
			get {
				return commentTags;
			}
		}

		string GetLastClosingXmlCommentTag()
		{
			var line = document.GetLineByNumber(location.Line);

			restart:
			string lineText = document.GetText(line);
			if (!lineText.Trim().StartsWith("///", StringComparison.Ordinal))
				return null;
			int startIndex = Math.Min(location.Column - 1, lineText.Length - 1) - 1;
			while (startIndex > 0 && lineText [startIndex] != '<') {
				--startIndex;
				if (lineText [startIndex] == '/') {
					// already closed.
					startIndex = -1;
					break;
				}
			}
			if (startIndex < 0 && line.PreviousLine != null) {
				line = line.PreviousLine;
				goto restart;
			}

			if (startIndex >= 0) {
				int endIndex = startIndex;
				while (endIndex + 1 < lineText.Length && lineText [endIndex] != '>' && !char.IsWhiteSpace(lineText [endIndex])) {
					endIndex++;
				}
				string tag = endIndex - startIndex - 1 > 0 ? lineText.Substring(
					startIndex + 1,
					endIndex - startIndex - 1
				) : null;
				if (!string.IsNullOrEmpty(tag) && commentTags.IndexOf(tag) >= 0) {
					return tag;
				}
			}
			return null;
		}

		IEnumerable<ICompletionData> GetXmlDocumentationCompletionData()
		{
			var closingTag = GetLastClosingXmlCommentTag();
			if (closingTag != null) {
				yield return factory.CreateLiteralCompletionData(
					"/" + closingTag + ">"
				);
			}

			yield return factory.CreateXmlDocCompletionData(
				"c",
				"Set text in a code-like font"
			);
			yield return factory.CreateXmlDocCompletionData(
				"code",
				"Set one or more lines of source code or program output"
			);
			yield return factory.CreateXmlDocCompletionData(
				"example",
				"Indicate an example"
			);
			yield return factory.CreateXmlDocCompletionData(
				"exception",
				"Identifies the exceptions a method can throw",
				"exception cref=\"|\"></exception"
			);
			yield return factory.CreateXmlDocCompletionData(
				"include",
				"Includes comments from a external file",
				"include file=\"|\" path=\"\""
			);
			yield return factory.CreateXmlDocCompletionData(
				"inheritdoc",
				"Inherit documentation from a base class or interface",
				"inheritdoc/"
			);
			yield return factory.CreateXmlDocCompletionData(
				"list",
				"Create a list or table",
				"list type=\"|\""
			);
			yield return factory.CreateXmlDocCompletionData(
				"listheader",
				"Define the heading row"
			);
			yield return factory.CreateXmlDocCompletionData(
				"item",
				"Defines list or table item"
			);

			yield return factory.CreateXmlDocCompletionData("term", "A term to define");
			yield return factory.CreateXmlDocCompletionData(
				"description",
				"Describes a list item"
			);
			yield return factory.CreateXmlDocCompletionData(
				"para",
				"Permit structure to be added to text"
			);

			yield return factory.CreateXmlDocCompletionData(
				"param",
				"Describe a parameter for a method or constructor",
				"param name=\"|\""
			);
			yield return factory.CreateXmlDocCompletionData(
				"paramref",
				"Identify that a word is a parameter name",
				"paramref name=\"|\"/"
			);

			yield return factory.CreateXmlDocCompletionData(
				"permission",
				"Document the security accessibility of a member",
				"permission cref=\"|\""
			);
			yield return factory.CreateXmlDocCompletionData(
				"remarks",
				"Describe a type"
			);
			yield return factory.CreateXmlDocCompletionData(
				"returns",
				"Describe the return value of a method"
			);
			yield return factory.CreateXmlDocCompletionData(
				"see",
				"Specify a link",
				"see cref=\"|\"/"
			);
			yield return factory.CreateXmlDocCompletionData(
				"seealso",
				"Generate a See Also entry",
				"seealso cref=\"|\"/"
			);
			yield return factory.CreateXmlDocCompletionData(
				"summary",
				"Describe a member of a type"
			);
			yield return factory.CreateXmlDocCompletionData(
				"typeparam",
				"Describe a type parameter for a generic type or method"
			);
			yield return factory.CreateXmlDocCompletionData(
				"typeparamref",
				"Identify that a word is a type parameter name"
			);
			yield return factory.CreateXmlDocCompletionData(
				"value",
				"Describe a property"
			);

		}

		#endregion

		#region Keywords

		static string[] expressionLevelKeywords = new string [] {
			"as",
			"is",
			"else",
			"out",
			"ref",
			"null",
			"delegate",
			"default"
		};
		static string[] primitiveTypesKeywords = new string [] {
			"sub",
			"object",
			"bool",
			"byte",
			"sbyte",
			"char",
			"shortint",
			"integer",
			"longint",
			"ushortint",
			"uinteger",
			"ulongint",
			"float",
			"real",
			"decimal",
			"string",
            "date",
            "time",
			"pointer",
			"upointer",
            "guid"
		};
		static string[] statementStartKeywords = new string [] { "me", "new", "sizeof", "self", 
			"true", "false", "typeof", "checked", "unchecked", "from", "leave", "checked",
			"unchecked", "const", "persist", "do", "finally", "fixed", "for", "foreach",
			"jmp", "if", "lock", "ret", "stackalloc", "caseof", "throw", "try", "unsafe", 
			"using", "while", "yield","safe","loop","restrict","asm",
			"except"
		};
		static string[] globalLevelKeywords = new string [] {
			"program", "include", "native", "public", "internal", 
			"class", "interface", "struct", "enum", "delegate",
			"abstract", "final", "static", "unsafe", "shared"
		};
		static string[] accessorModifierKeywords = new string [] {
			"public", "internal", "protected", "private", "async"
		};
		static string[] typeLevelKeywords = new string [] {
			"public", "internal", "protected", "private", "async",
			"class", "interface", "struct", "enum", "delegate",
			"abstract", "final", "static", "unsafe", "shared",
			"const", "event", "native", "fixed", "new", 
			"operator", "explicit", "implicit", 
			"override", "readonly", "virt", "volatile"
		};
		static string[] linqKeywords = new string[] {
			"from",
			"where",
			"select",
			"group",
			"into",
			"orderby",
			"join",
			"let",
			"in",
			"on",
			"equals",
			"by",
			"ascending",
			"descending"
		};
		static string[] parameterTypePredecessorKeywords = new string[] {
			"out",
			"ref",
			"params"
		};

		#endregion

	}
}

