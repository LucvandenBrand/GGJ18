defmodule SnappyServer.GameServerBucket do
  require Logger

  use ExActor.GenServer, export: __MODULE__

  defstart start_link(_), do: initial_state([])

  defcall add_game(unity_socket), state: state do
    # TODO Generate random entry password/identifier?

    {:ok, new_game} = SnappyServer.GameServer.start_link(unity_socket)
    updated_state = [new_game | state]
    Logger.debug(inspect updated_state)

    # new_state(updated_state)
    set_and_reply(updated_state, {:ok, new_game})
  end

  defcast add_player({player_name, player_socket}), state: [last_game | rest_game] do
    # TODO actually work with game identifier
    Logger.debug("Adding player #{player_name}, #{inspect(player_socket)} \n to game #{inspect(last_game)}")
    {:ok, unity_listener_pid} = SnappyServer.GameServer.add_player(last_game, {player_name, player_socket})

    # reply({:ok, unity_listener_pid})
    noreply
  end

  defcast add_player({player_name, player_socket}), state: [] do
    reply({:error, :attempting_to_add_player_to_unexistent_game})
  end

  defcall input_message({player_name, input_message}), state: [last_game | rest_game] do
    Logger.debug("Input message from #{player_name}")
    SnappyServer.GameServer.input_message(last_game, {player_name, input_message})
    reply(:ok)
  end

  defcall input_message({player_name, input_message}), state: [] do
    reply({:error, :attempting_to_input_message_to_unexistent_game})
  end


  # TODO Dont crash when game is removed, but rather trap exits and remove game from list.

end
