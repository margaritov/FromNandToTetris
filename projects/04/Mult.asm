// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Mult.asm

// Multiplies R0 and R1 and stores the result in R2.
// (R0, R1, R2 refer to RAM[0], RAM[1], and RAM[2], respectively.)

//
// task 1 pseudo code
// multiplier = R1
// i=0
//result = R0
// (LOOP)
// if i=multiplier goto end
// result = result + R0

@0
D=A
@i
M=D // init i=0;

@R1
D=M
@multiplier
M=D // multiplier = R1
@0
D=A
@R2
M=D // init result=0
(LOOP)
@multiplier
D=M
@i
D=D-M
@END
D; JEQ // if i==multiplier exit loop
@R0
D=M
@R2
M=D+M
@i
M=M+1
@LOOP
0; JMP
(END)
@END
0; JMP