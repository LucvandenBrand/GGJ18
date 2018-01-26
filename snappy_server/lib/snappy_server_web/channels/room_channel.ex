defmodule SnappyServerWeb.RoomChannel do
  use SnappyServerWeb, :channel

  def join("room:lobby", payload, socket) do
    if authorized?(payload) do
      room_code = payload["room_code"]
      player_name = payload["player_name"]
      SnappyServer.GameServerBucket.add_player(room_code, {player_name, socket})
      # socket =
      #   socket
      #   |> assign(:room_code, room_code)
      #   |> assign(:player_name, player_name)
      {:ok, socket}
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

  def handle_in("new_msg", %{"body" => body, "room_code" => room_code, "player_name" => player_name}, socket) do
    # IO.inspect(socket.assigns)
    # broadcast! socket, "new_msg", %{body: body}
    SnappyServer.GameServerBucket.input_message(room_code, {player_name, body})
    {:noreply, socket}
  end
  # Add authorization logic here as required.
  # TODO add username to player list.
  defp authorized?(_payload) do
    true
  end
end
