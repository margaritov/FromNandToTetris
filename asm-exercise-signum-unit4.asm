// Program Signum.asm
// Computes: if R0>0 
//			R1=1
//			else R1=0

@R0
D=M
@8
D;JGT
@0
D=A
@10
0;JMP
@1
D=A
@R1
M=D