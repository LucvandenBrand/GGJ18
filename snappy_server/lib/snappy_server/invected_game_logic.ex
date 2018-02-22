defmodule SnappyServer.InvectedGameLogic do
  use Rustler, otp_app: :snappy_server, crate: "invectedgamelogic"

  # When your NIF is loaded, it will override this function.
  def add(_a, _b), do: :erlang.nif_error(:nif_not_loaded)

  def print_nice_message(), do: :erlang.nif_error(:nif_not_loaded)

  @doc """
  Initializes the game state with a nice default value.
  """
  def init_game_state(), do: :erlang.nif_error(:nif_not_loaded)

  @doc """
  Adds the player identified by `player_name` to the game.
  Returns an updated state.
  """
  def add_player(state, player_name), do: :erlang.nif_error(:nif_not_loaded)

  @doc """
  Updates the direction in which the given player will move,
  based on their joystick input.
  """
  def update_player_desired_movement(state, player_name, movement_pair), do: :erlang.nif_error(:nif_not_loaded)

  @doc """
  This function moves players, performs collisions, etc.
  Run 60 times per second.
  """
  def update_game_timestep(state, deltatime), do: :erlang.nif_error(:nif_not_loaded)

  @doc """
  Returns a JSON-representation of the given state.
  """
  def render_state(state), do: :erlang.nif_error(:nif_not_loaded)

  @doc """
  debug-function that prints a single player.
  """
  def print_player(player), do: :erlang.nif_error(:nif_not_loaded)

  @doc """
  debug-function that prints the game state's internals.
  """
  def print_state(state), do: :erlang.nif_error(:nif_not_loaded)
end
