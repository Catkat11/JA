; Sepia transformation algorithm
;
; In initialization we are loading input parameters and calculate the processing counter
; In Main Loop there is processing each pixel in the specified range, applying sepia transformation based on rates and
; ensuring that RGB values do not exceed 255
; At the end of loop we are returning 
;
; rcx/xmm0 - pixels (input): Pointer to the array of pixels containing RGBA values
; rdx/xmm1 - sepia rates (input): Pointer to an array containing sepia intensity rates for red, green, and blue channels
; r8/xmm2 - rgb rates (input): Pointer to an array containing RGB intensity rates
; r9/xmm3 - array of 255 values (input): Pointer to an array containing 255 values
; r11  - start index (input): Start index of the pixel array to process
; r10 - end index (input): End index of the pixel array to process
; rcx/xmm0 - transformed pixels (output): Pointer to the array of transformed pixels containing RGBA values



.CODE
sepia proc			
	; Load parameters
	mov			ebx, dword ptr[rbp + 48]	; take start index from stack 
	mov			r11, rbx					; and give it to r11
	mov			ebx, dword ptr[rbp + 56]	; take end index from stack 
	mov			r10, rbx					; and give it to r10
	movdqu		xmm1, oword ptr[rdx]		; take to xmm1 sepia rates
	movdqu		xmm2, oword ptr[r8]			; take to xmm2 rgb rates
	movdqu		xmm3, oword ptr[r9]			; take to xmm4 array of 255 values	

	; Calculate counter
	sub			r10, r11					; establish counter 
	mov			rdi, r10					; and give it to rdi
	shr			edi, 2						; dividing counter by 4 because there are 4 values in each pixel

	; Prepare for multiplying by 4
	mov			rax, 4h						; prepare multiplying by 4
	mul			r11							; multiply start index by 4 (pixels size in bytes) 
	add			rcx, rax					; and add it to pixels offset table

sepiaLoop:
	cmp			edi, 0h						; if counter reaches zero, end loop
	je			endLoop						
	
	movdqu		xmm0, oword ptr[rcx]		; take to xmm0 one pixel 

	mulps		xmm0, xmm2					; multiply pixels rgb with rgb rates
	
											; before this we have in xmm0 (b g r a)
	movshdup    xmm4, xmm0					; broadcast g to b and a to r
    addps       xmm0, xmm4					; adding g with b and a with r
    movhlps     xmm4, xmm0					; broadcast a to g r a and ar to gb
    addps       xmm0, xmm4					; sums gb with ar, sum to collect is in upper bits of xmm0
	punpckldq	xmm0, xmm0					; broadcast sum to b g
	punpcklqdq	xmm0, xmm0					; broadcast sum to b g r a

	addps		xmm0, xmm1					; add sepia rates to pixels rgb  

	minps		xmm0, xmm3					; choosing minimum of rgb values and 255 values,
											; because rgb value cannot be above 255
	movdqu		oword ptr[rcx], xmm0		; save calculated pixel in pixels table
	
	add			rcx, 16						; move pointer one pixel forward, that means 16 bytes
	sub			rdi, 1						; decrease counter
	jmp			sepiaLoop					; go through loop
endLoop:
    ret
sepia endp
END