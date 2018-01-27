"use strict";

var canvas;
var ctx;
var width;
var height;
var isDown = false;
var pointerX = 0;
var pointerY = 0;

function main(){
	
	canvas = document.getElementById("controlcircle");
	ctx = canvas.getContext("2d");
	width = canvas.width = screen.width;
	height = canvas.height = screen.height*.8;
	window.addEventListener("touchmove", touchMove);
	window.addEventListener("mousemove", mouseMove);
	window.addEventListener("touchstart", (e)=>{press(); touchMove(e);});
	window.addEventListener("mousedown", (e)=>{press(); mouseMove(e);});
	window.addEventListener("touchend", release);
	window.addEventListener("mouseup", release);
	window.addEventListener("touchcancel", release);
	window.addEventListener("mouseleave", release);
	
	update();
}

function touchMove(e){
	move(e.touches[0]);
}

function mouseMove(e){
	if (isDown){
		move(e);
	}
}

function press(){
	isDown = true;
}

function move(e){
	pointerX = e.pageX;
	pointerY = e.pageY;
}

function release(){
	isDown = false;
}


function update(){
	draw();
	requestAnimationFrame(update);
}


function draw(){
	ctx.clearRect(0, 0, width, height);
	ctx.strokeStyle = "#0005";
	ctx.beginPath();
	var r = Math.min(width, height)/2;
	ctx.lineWidth = r/5;
	ctx.arc(width/2, height/2, r*.7, 0, 2*Math.PI);
	ctx.stroke();
	if (isDown){
		ctx.strokeStyle = "#0005";
		ctx.lineWidth = 30 + (Math.hypot(width/2-pointerX, height/2-pointerY))/10;
		ctx.lineCap = "round";
		ctx.beginPath();
		ctx.moveTo(width/2, height/2);
		ctx.lineTo(pointerX, pointerY);
		ctx.stroke()
	}
}

window.addEventListener("load", main);
