defmodule SnappyServerWeb.PageController do
  use SnappyServerWeb, :controller

  def index(conn, _params) do
    render conn, "index.html"
  end
end
