// sum.asm
@0 // sum = 0
D=A
@sum
M=D

@0 // i = 0
D=A
@i
M=D

@R0 // n = R0
D=M
@n
M=D

(LOOP)
@n
D=M
@i
D=D-M
@STOP
D; JLE
@i
M=M+1
D=M
@sum
M=D+M
@LOOP
0; JMP
(STOP)
@sum
D=M
@R1
M=D
(END)
@END
0; JMP