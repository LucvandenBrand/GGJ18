defmodule SnappyServer.GameServer do
  require Logger

  @moduledoc """
  This contains the Game messaging. Both the lobby system as well as the actual game communication system.
  """

  defmodule State do
    @enforce_keys [:unity_listener, :unity_socket]
    defstruct players: %{}, unity_listener: nil, unity_socket: nil, code: "AAAA", color: "000000", voronoi_color_index: 0
  end

  def voronoi_color(voronoi_color_index)
  def voronoi_color(0),do: "09FF00"
  def voronoi_color(1),do: "FF0000"
  def voronoi_color(2),do: "0017FF"
  def voronoi_color(3),do: "E26B2CFF"
  def voronoi_color(4),do: "A5A5A5"
  def voronoi_color(5),do: "FFFF00"
  def voronoi_color(6),do: "9500CA"
  def voronoi_color(7),do: "FF9898"
  def voronoi_color(8),do: "474747"
  def voronoi_color(9),do: "FF00B9"
  def voronoi_color(10),do: "67942A"
  def voronoi_color(11),do: "814E18"
  def voronoi_color(12),do: "000000"
  def voronoi_color(13),do: "00FFF4"
  def voronoi_color(14),do: "FFFFFF"
  def voronoi_color(n) when n > 14, do: voronoi_color(rem(n, 14))

  use ExActor.GenServer

  defstart start_link(unity_socket) do
    # TODO Code generation

    {:ok, unity_listener_pid} = Task.start_link(fn ->
      SnappyServer.TCPServer.serve(unity_socket, self())
    end)
    state = %State{unity_listener: unity_listener_pid, unity_socket: unity_socket, code: generate_code()}

    :erlang.send_after(1, self(), :first_tick!)
    initial_state(state)
  end

  defp generate_code do
	alphabet = String.codepoints "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
	Enum.random(alphabet) <>
	Enum.random(alphabet) <>
	Enum.random(alphabet) <>
	Enum.random(alphabet)
#     Ecto.UUID.generate()
#     |> String.split_at(4)
#     |> elem(0)
#     |> String.upcase
  end

  @doc "Called during lobby creation."
  defcall add_player({player_name, player_socket}), state: state do
    # Logger.debug("Attempting to add player #{player_name} to state #{inspect(state)}")
    if Map.has_key?(state.players, player_name) do
      # reply({:error, :player_already_exists})
      updated_state = %State{state | players: Map.put(state.players, player_name, %{state.players[player_name] | player_socket: player_socket})}

      set_and_reply(updated_state, {:ok, %{unity_listener: updated_state.unity_listener, room_pid: self(), voronoi_color: updated_state.players[player_name].voronoi_color}})
    else
      voronoi_color = voronoi_color(state.voronoi_color_index)
      updated_state = %State{state | players: Map.put(state.players, player_name, %{player_socket: player_socket, voronoi_color: voronoi_color}), voronoi_color_index: state.voronoi_color_index + 1}

      send_to_unity(updated_state, %{type: "player_added", player_name: player_name})
      updated_state
      set_and_reply(updated_state, {:ok, %{unity_listener: updated_state.unity_listener, room_pid: self(), voronoi_color: voronoi_color}})
    end
  end

  defcast input_message({player_name, input_message}), state: state do
    # Logger.debug("Input Message: #{player_name} #{inspect(input_message)}")
    send_to_unity(state, %{type: "input_message", message: input_message, player_name: player_name})
    noreply
  end

  defcast player_move({player_name, movement_map = %{pointer_x: pointer_x, pointer_y: pointer_y}}), state: state do
    Logger.debug("Player Move Message: #{player_name} #{inspect(movement_map)}")
    send_to_unity(state, %{type: "player_move", player_name: player_name, pointer_x: pointer_x, pointer_y: pointer_y})
    noreply
  end

  defcast player_release(player_name), state: state do
    send_to_unity(state, %{type: "player_release", player_name: player_name})
    noreply
  end

  defcast player_disconnected(player_name), state: state do
    send_to_unity(state, %{type: "player_disconnected", player_name: player_name})
    noreply
  end


  @doc "Called when game is finished?"
  defcast stop, do: stop_server(:normal)

  @doc "Debugging"
  defcall get, state: state, do: reply(state)
  
  defp send_to_unity(state, message) do
    SnappyServer.TCPServer.write_message(state.unity_socket, Poison.encode!(message))
  end

  defhandleinfo :first_tick!, state: state do
    Logger.debug("Unity GameServer is live!")
    send_to_unity(state, %{type: "room_code", room_code: state.code})

    new_state(state)
  end
end
