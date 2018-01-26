defmodule SnappyServer.GameServerBucket do
  require Logger

  use ExActor.GenServer, export: __MODULE__

  defstart start_link(_), do: initial_state(%{})

  defcall add_game(unity_socket), state: state do
    # TODO Generate random entry password/identifier?

    {:ok, new_game} = SnappyServer.GameServer.start_link(unity_socket)
    new_game_state = SnappyServer.GameServer.get(new_game)
    # updated_state = %{state | new_game_state.code => new_game}
    updated_state = Map.put(state, new_game_state.code, new_game)
    # updated_state = [new_game | state]
    Logger.debug(inspect(updated_state))

    # new_state(updated_state)
    set_and_reply(updated_state, {:ok, new_game})
  end

  defcast add_player(game_code, {player_name, player_socket}), state: state do
    case state do
      %{^game_code => game} ->
        # TODO actually work with game identifier
        Logger.debug("Adding player #{player_name}, #{inspect(player_socket)} \n to game #{inspect(game)}")
        {:ok, unity_listener_pid} = SnappyServer.GameServer.add_player(game, {player_name, player_socket})
        noreply
      _ ->
        noreply
        # reply({:error, :attempting_to_add_player_to_unexistent_game})
    end
  end

  defcall input_message(game_code, {player_name, input_message}), state: state do
    case state do
      %{^game_code => game} ->
        Logger.debug("Input message from #{player_name}")
        SnappyServer.GameServer.input_message(game, {player_name, input_message})

        reply(:ok)
      _ ->
        reply({:error, :attempting_to_add_player_to_unexistent_game})
    end
  end

  # defcall input_message({player_name, input_message}), state: _ do
  #   reply({:error, :attempting_to_input_message_to_unexistent_game})
  # end


  # TODO Dont crash when game is removed, but rather trap exits and remove game from list.

end
