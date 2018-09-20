// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.


	// init SCREEN_MAX_ADDRESS = SCREEN + 8192
	@8192
	D=A
	@SCREEN
	D=D+A
	@SCREEN_MAX_ADDRESS
	M=D;

(START)
	//init fillColor  0: White,  -1: Black
	@fillColor
	M=0
	M=M-1	
	
	// init goToStart
	@goToStart
	M=0
	
(NO_KEY_PRESSED)
	@KBD
	D=M
	@NO_KEY_PRESSED
	D; JEQ

(FILL)
	// i = SCREEN address
	@SCREEN
	D=A
	@i
	M=D
	
	(LOOP)
	// if i==SCREEN_MAX_ADDRESS EXIT LOOP
	@SCREEN_MAX_ADDRESS
	D=M
	@i
	D=D-M
	@FILL_DONE
	D;JLE
	
	@fillColor
	D=M
	
	@i
	A=M //pointer arithmetic A=i
	M=D
	@i
	M=M+1
	@LOOP
	0; JMP
	
(FILL_DONE)
	@fillColor
	D=M
	@START
	D; JEQ
	
(AWAIT_RELEASE)
	@KBD
	D=M
	@AWAIT_RELEASE
	D; JNE

	@fillColor
	M=0	
	@FILL
	0; JMP