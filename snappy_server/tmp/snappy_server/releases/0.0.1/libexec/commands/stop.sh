#!/usr/bin/env bash

set -o posix

## Stops a daemon started via `start`

set -e

require_cookie
require_live_node
run_hooks pre_stop

# Wait for the node to completely stop...
PID="$(get_pid)"
if ! nodetool "stop"; then
    exit 1
fi

# Wait until the pid is gone
while kill -s 0 "$PID" 2>/dev/null;
do
    sleep 1
done

# Finally, run all post_stop hooks
run_hooks post_stop
