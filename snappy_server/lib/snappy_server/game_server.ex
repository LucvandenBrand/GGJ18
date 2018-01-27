defmodule SnappyServer.GameServer do
  require Logger

  @moduledoc """
  This contains the Game messaging. Both the lobby system as well as the actual game communication system.
  """

  defmodule State do
    @enforce_keys [:unity_listener, :unity_socket]
    defstruct players: %{}, unity_listener: nil, unity_socket: nil, code: "AAAA"
  end

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
    Integer.to_string(:rand.uniform(65535), 32)
  end

  @doc "Called during lobby creation."
  defcall add_player({player_name, player_socket}), state: state do
    # Logger.debug("Attempting to add player #{player_name} to state #{inspect(state)}")
    if Map.has_key?(state.players, player_name) do
      reply({:error, :player_already_exists})
    else
      updated_state = %State{state | players: Map.put(state.players, player_name, player_socket)}
      # Logger.debug(inspect(updated_state))

      send_to_unity(updated_state, %{type: "player_added", player_name: player_name})

      set_and_reply(updated_state, {:ok, %{unity_listener: updated_state.unity_listener, room_pid: self()}})
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

  defcast player_release({player_name}), state: state do
    send_to_unity(state, %{type: "player_release", player_name: player_name})
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
