defmodule SnappyServer.GameServerBucket do
  require Logger

  use ExActor.GenServer, export: __MODULE__

  defstart start_link(_), do: initial_state(%{})

  defcall add_game(unity_socket), state: state do

    {:ok, new_game} = SnappyServer.GameServer.start_link(unity_socket)
    new_game_state = SnappyServer.GameServer.get(new_game)
    updated_state = Map.put(state, new_game_state.code, new_game)
    Logger.debug(inspect(updated_state))

    set_and_reply(updated_state, {:ok, new_game})
  end

  defcall add_player(game_code, {player_name, player_socket}), state: state do
    Logger.debug("Attempting to find game #{game_code} in #{inspect(state)}")
    case state do
      %{^game_code => game} ->
        # TODO actually work with game identifier
        Logger.debug("Adding player #{player_name}, #{inspect(player_socket)} \n to game #{inspect(game)}")
        reply(SnappyServer.GameServer.add_player(game, {player_name, player_socket}))
      _ ->
        reply({:error, :unexistent_game})
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

  # TODO Dont crash when game is removed, but rather trap exits and remove game from list.

end
