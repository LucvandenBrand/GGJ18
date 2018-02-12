defmodule SnappyServer.InvectedGameLogic do
  use Rustler, otp_app: :snappy_server, crate: "invectedgamelogic"

  # When your NIF is loaded, it will override this function.
  def add(_a, _b), do: :erlang.nif_error(:nif_not_loaded)

  def print_nice_message(), do: :erlang.nif_error(:nif_not_loaded)

  def init_game_state(), do: :erlang.nif_error(:nif_not_loaded)

  def add_player(state, player_name), do: :erlang.nif_error(:nif_not_loaded)

  def print_player(player), do: :erlang.nif_error(:nif_not_loaded)
  def print_state(state), do: :erlang.nif_error(:nif_not_loaded)
end
