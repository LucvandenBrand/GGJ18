# This file is responsible for configuring your application
# and its dependencies with the aid of the Mix.Config module.
#
# This configuration file is loaded before any dependency and
# is restricted to this project.
use Mix.Config

# General application configuration
config :snappy_server,
  ecto_repos: [SnappyServer.Repo],
  server: true

# Configures the endpoint
config :snappy_server, SnappyServerWeb.Endpoint,
  url: [host: "localhost"],
  secret_key_base: "xgPYclcyVJIUhQbF310Ubh6L36e2zgaPR/6YM0x5in0Gh7yvgowbpU1csE3s4t+n",
  render_errors: [view: SnappyServerWeb.ErrorView, accepts: ~w(html json)],
  pubsub: [name: SnappyServer.PubSub,
           adapter: Phoenix.PubSub.PG2]

# Configures Elixir's Logger
config :logger, :console,
  format: "$time $metadata[$level] $message\n",
  metadata: [:request_id]

# Import environment specific config. This must remain at the bottom
# of this file so it overrides the configuration defined above.
import_config "#{Mix.env}.exs"
