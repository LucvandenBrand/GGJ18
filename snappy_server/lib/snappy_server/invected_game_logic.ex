defmodule SnappyServer.InvectedGameLogic do
  use Rustler, otp_app: :snappy_server, crate: "invectedgamelogic"

  # When your NIF is loaded, it will override this function.
  def add(_a, _b), do: exit(:nif_not_loaded) #:erlang.nif_error(:nif_not_loaded)
end
