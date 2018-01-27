// NOTE: The contents of this file will only be executed if
// you uncomment its entry in "assets/js/app.js".

// To use Phoenix channels, the first step is to import Socket
// and connect at the socket path in "lib/web/endpoint.ex":
import {Socket} from "phoenix"

let socket = new Socket("/socket", {params: {token: window.userToken}})

// When you connect, you'll often need to authenticate the client.
// For example, imagine you have an authentication plug, `MyAuth`,
// which authenticates the session and assigns a `:current_user`.
// If the current user exists you can assign the user's token in
// the connection for use in the layout.
//
// In your "lib/web/router.ex":
//
//     pipeline :browser do
//       ...
//       plug MyAuth
//       plug :put_user_token
//     end
//
//     defp put_user_token(conn, _) do
//       if current_user = conn.assigns[:current_user] do
//         token = Phoenix.Token.sign(conn, "user socket", current_user.id)
//         assign(conn, :user_token, token)
//       else
//         conn
//       end
//     end
//
// Now you need to pass this token to JavaScript. You can do so
// inside a script tag in "lib/web/templates/layout/app.html.eex":
//
//     <script>window.userToken = "<%= assigns[:user_token] %>";</script>
//
// You will need to verify the user token in the "connect/2" function
// in "lib/web/channels/user_socket.ex":
//
//     def connect(%{"token" => token}, socket) do
//       # max_age: 1209600 is equivalent to two weeks in seconds
//       case Phoenix.Token.verify(socket, "user socket", token, max_age: 1209600) do
//         {:ok, user_id} ->
//           {:ok, assign(socket, :user, user_id)}
//         {:error, reason} ->
//           :error
//       end
//     end
//
// Finally, pass the token on connect as below. Or remove it
// from connect if you don't care about authentication.

"use strict";


let roomCodeInput         = document.querySelector("#room-code")
let playerNameInput         = document.querySelector("#player-name")
// let chatInput         = document.querySelector("#chat-input")
let joinButton        = document.querySelector("#join-button")
var channel;

var normalizedX;
var normalizedY;
var oldNormalizedX;
var oldNormalizedY;

function joystickloop(){
    if(normalizedX != oldNormalizedX || normalizedY != oldNormalizedY) {
		    channel.push("player_move", {pointer_x: normalizedX, pointer_y: normalizedY});
        oldNormalizedX = normalizedX;
        oldNormalizedY = normalizedY;
    }
    window.requestAnimationFrame(joystickloop);
}
window.requestAnimationFrame(joystickloop);
// window.setInterval(joystickloop, 1000/60);


function main(){
	  
	  var canvas = document.getElementById("controlcircle");
	  var ctx = canvas.getContext("2d");
	  var width = canvas.width = window.innerWidth;
	  var height = canvas.height = window.innerHeight*.8;
	  var circleradius = Math.min(width, height)/2 * 0.7;
	  var isDown = false;
	  var pointerX = 0;
	  var pointerY = 0;
	  window.addEventListener("touchmove", touchMove);
	  window.addEventListener("mousemove", mouseMove);
	  window.addEventListener("touchstart", (e)=>{press(); touchMove(e);});
	  window.addEventListener("mousedown", (e)=>{press(); mouseMove(e);});
	  window.addEventListener("touchend", release);
	  window.addEventListener("mouseup", release);
	  window.addEventListener("touchcancel", release);
	  window.addEventListener("mouseleave", release);

	  

	  function touchMove(e){
		    e.preventDefault();
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
		    // pointerX = e.layerX;
		    // pointerY = e.layerY;
        pointerX = e.pageX;
        pointerY = e.pageY;
		    //console.log({pointer_x: (pointerX - width/2) / circleradius, pointer_y: (pointerY - height/2) / circleradius});
		    normalizedX = (pointerX - width/2) / circleradius;
		    normalizedY = (pointerY - height/2) / circleradius;
        // alert('' + pointerX + ', ' + width + ', ' + circleradius + ', ' + normalizedX + ', ' + normalizedY)
		    
		    // channel.push("player_move", {pointer_x: normalizedX, pointer_y: normalizedY});
	  }

	  function release(){
		    isDown = false;
        normalizedX = 0;
        normalizedY = 0;
		    // channel.push("player_release", {});
	  }


	  function update(){
		    draw();
		    requestAnimationFrame(update);
	  }


	  function draw(){
		    ctx.clearRect(0, 0, width, height);
		    ctx.strokeStyle = "#0005";
		    ctx.beginPath();
        // 		var r = Math.min(width, height)/2;
		    ctx.lineWidth = circleradius/2;
		    ctx.arc(width/2, height/2, circleradius, 0, 2*Math.PI);
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
        draw_name();
        draw_room_code();

	  }

    function draw_name(){
        ctx.textAlign = "start";
        ctx.textBaseline = "top";
        ctx.font = "12px Arial";
        ctx.fillText("Player:", 30, 0);
        ctx.font = "28px Arial";
        ctx.fillText(playerNameInput.value, 30, 20);
    }

    function draw_room_code(){
        ctx.textAlign = "end";
        ctx.textBaseline = "top";
        ctx.font = "12px Arial";
        ctx.fillText("Room:", screen.width - 30, 0);
        ctx.font = "28px Arial";
        ctx.fillText(roomCodeInput.value, screen.width - 30, 20);
    }
	  
	  update();
}



socket.connect();

$(document).ready(function(){
    $(document).on('keyup', "input[data-always-uppercase=true]", function () {
        $(this).val(function (_, val) {
            return val.toUpperCase();
        });
    });

    joinButton.addEventListener("click", event => {
        channel           = socket.channel("room:lobby", {room_code: roomCodeInput.value, player_name: playerNameInput.value});

        //     let messagesContainer = document.querySelector("#messages")


        //     channel.on("new_msg", payload => {
        //         let messageItem = document.createElement("li");
        //         messageItem.innerText = `[${Date()}] ${payload.body}`
        //         messagesContainer.appendChild(messageItem)
        //     })

        event.preventDefault();

        channel.join()
            .receive("error", resp => { console.log("Unable to join", resp);
                                        alert("Unable to join game: " + resp.reason);
                                      })
            .receive("ok", resp => {
                console.log("Joined successfully", resp);

                //             document.querySelector("[data-origin='player-name']").innerHTML = playerNameInput.value;
                //             document.querySelector("[data-origin='room-code']").innerHTML = roomCodeInput.value;
                // document.querySelector("#game-joining-section").style = "display: none";
                // document.querySelector("#game-playing-section").style = "display: block";
                $("#game-joining-section").hide();
                $("#game-playing-section").show();
			          
			          main();
            });
    });
})

export default socket;
