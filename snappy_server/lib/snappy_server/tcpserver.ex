
defmodule SnappyServer.TCPServer do
  require Logger

  def accept(port) do
    {:ok, socket} = :gen_tcp.listen(port,
    [:binary, packet: 2, active: false, reuseaddr: true])
    Logger.info "Accepting connections on port #{port}"
    loop_acceptor(socket)
  end

  # Wait for next connection and hand it off to its own process
  # And then recurse
  defp loop_acceptor(port_listener) do
    {:ok, client} = :gen_tcp.accept(port_listener)
    # {:ok, game_server} = SnappyServer.GameServer.start_link(client)
    # {:ok, pid} = Task.Supervisor.start_child(SnappyServer.TCPServer.SocketTaskSupervisor,
    #   fn ->
    #     serve(client, game_server)
    #   end
    # )
    {:ok, unity_listener_pid} = SnappyServer.GameServerBucket.add_game(client)
    :ok = :gen_tcp.controlling_process(client, unity_listener_pid)

    loop_acceptor(port_listener)
  end

  def serve(socket, game_server) do
    echo(socket, game_server)

    serve(socket, game_server)
  end

  # Reads single message, then returns this message unaltered to socket.
  defp echo(socket, game_server) do
    input  = read_message(socket)
    output = input
    write_message(socket, output)
    write_message(socket, "bazbazbaz")
  end

  # Reads single message from socket
  defp read_message(socket) do
    {:ok, data} = :gen_tcp.recv(socket, 0)
    Logger.debug("Received input: #{inspect(data)}")
    data
  end

  # Writes single message to socket
  def write_message(socket, data) do
    IO.inspect(socket)
    IO.inspect(data)
    Logger.debug("Sending output: #{data}")
    :gen_tcp.send(socket, data)
  end
end
