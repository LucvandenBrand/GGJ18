defmodule SnappyServerWeb.RoomChannel do
  use SnappyServerWeb, :channel

  def join("room:lobby", payload, socket) do
    if authorized?(payload) do
      IO.inspect(payload)
      room_code = payload["room_code"]
      player_name = payload["player_name"]
      IO.inspect({room_code, player_name})
      case SnappyServer.GameServerBucket.add_player(room_code, {player_name, socket}) do
        {:error, :unexistent_game} ->
          {:error, %{reason: "Unexistent Game"}}
        {:error, :player_already_exists} ->
          {:error, %{reason: "Player Already Exists"}}
        {:ok, %{room_pid: room_pid}} ->
          socket =
            socket
            |> assign(:room_code, room_code)
            |> assign(:room_pid, room_pid)
            |> assign(:player_name, player_name)
          {:ok, socket}
      end
    else
      {:error, %{reason: "unauthorized"}}
    end
  end

  # Channels can be used in a request/response fashion
  # by sending replies to requests from the client
  def handle_in("ping", payload, socket) do
    {:reply, {:ok, payload}, socket}
  end

  # It is also common to receive messages from the client and
  # broadcast to everyone in the current topic (room:lobby).
  def handle_in("shout", payload, socket) do
    broadcast socket, "shout", payload
    {:noreply, socket}
  end

  def handle_in("new_msg", %{"body" => body}, socket) do
    IO.inspect(socket.assigns)
    IO.inspect(body)
    # broadcast! socket, "new_msg", %{body: body}
    SnappyServer.GameServer.input_message(socket.assigns[:room_pid], {socket.assigns[:player_name], body})
    {:noreply, socket}
  end
  
  def handle_in("player_move", body = %{"pointer_x" => pointer_x, "pointer_y" => pointer_y}, socket) do
    IO.inspect(socket.assigns)
    IO.inspect(body)
    # broadcast! socket, "new_msg", %{body: body}
    SnappyServer.GameServer.player_move(socket.assigns[:room_pid], {socket.assigns[:player_name], %{pointer_x: pointer_x, pointer_y: pointer_y}})
    {:noreply, socket}
  end
  
  def handle_in("player_release", _body, socket) do
    IO.inspect(socket.assigns)
    # broadcast! socket, "new_msg", %{body: body}
    SnappyServer.GameServer.player_release(socket.assigns[:room_pid], socket.assigns[:player_name])
    {:noreply, socket}
  end

  def terminate(message, socket) do
    SnappyServer.GameServer.player_disconnected(socket.assigns[:room_pid], socket.assigns[:player_name]);
  end
  
  # Add authorization logic here as required.
  defp authorized?(_payload) do
    true
  end
end
