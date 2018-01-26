defmodule SnappyServer.GameServer do
  require Logger

  @moduledoc """
  This contains the Game messaging. Both the lobby system as well as the actual game communication system.
  """

  defmodule State do
    @enforce_keys [:unity_socket]
    defstruct players: %{}, unity_socket: nil
  end

  use ExActor.GenServer

  defstart start_link(unity_socket) do
    :erlang.send_after(1, self(), :first_tick!)
    initial_state(%State{unity_socket: unity_socket})
  end

  @doc "Called during lobby creation."
  defcast add_player(player_name, player_socket), state: state do
    put_in(state, [:players, player_name], player_socket)
  end

  @doc "Called when game is finished?"
  defcast stop, do: stop_server(:normal)

  @doc "Debugging"
  defcall get, state: state, do: reply(state)

  defcast send_to_unity(message) do
  end


  defhandleinfo :first_tick!, state: state do
    Logger.debug("Unity GameServer is live!")

    new_state(state)
  end
end
