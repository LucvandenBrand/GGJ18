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

function main(){
	
	var canvas = document.getElementById("controlcircle");
	var ctx = canvas.getContext("2d");
	var width = canvas.width = screen.width;
	var height = canvas.height = screen.height*.8;
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
		
		channel.push("player_move", {pointer_x: pointerX, pointer_y: pointerY});
	}

	function release(){
		isDown = false;
		channel.push("player_release", {});
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
		ctx.lineWidth = r/3;
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
	
	update();
}



socket.connect()

// Now that you are connected, you can join channels with a topic:



// chatInput.addEventListener("keypress", event => {
//     if(event.keyCode === 13){
//         console.log(chatInput.value, "TEST")
//         channel.push("new_msg", {body: chatInput.value})
//         chatInput.value = ""
//     }
// });


joinButton.addEventListener("click", event => {
    channel           = socket.channel("room:lobby", {room_code: roomCodeInput.value, player_name: playerNameInput.value})

//     let messagesContainer = document.querySelector("#messages")


//     channel.on("new_msg", payload => {
//         let messageItem = document.createElement("li");
//         messageItem.innerText = `[${Date()}] ${payload.body}`
//         messagesContainer.appendChild(messageItem)
//     })

    event.preventDefault();

    channel.join()
        .receive("error", resp => { console.log("Unable to join", resp) })
        .receive("ok", resp => {
            console.log("Joined successfully", resp)

//             document.querySelector("[data-origin='player-name']").innerHTML = playerNameInput.value;
//             document.querySelector("[data-origin='room-code']").innerHTML = roomCodeInput.value;
            document.querySelector("#game-joining-section").style = "display: none";
            document.querySelector("#game-playing-section").style = "display: block";
			
			main();
        });

})

export default socket
