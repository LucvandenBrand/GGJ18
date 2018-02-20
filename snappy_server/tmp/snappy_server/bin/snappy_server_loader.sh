#!/usr/bin/env bash

set -o posix
set -e

unset CDPATH

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd -P)"
RELEASE_ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
RELEASES_DIR="$RELEASE_ROOT_DIR/releases"
REL_NAME="${REL_NAME:-snappy_server}"
REL_VSN="${REL_VSN:-$(cut -d' ' -f2 "$RELEASES_DIR"/start_erl.data)}"
ERTS_VSN="${ERTS_VSN:-$(cut -d' ' -f1 "$RELEASES_DIR"/start_erl.data)}"

export RELEASE_ROOT_DIR
export RELEASES_DIR
export REL_NAME
export REL_VSN
export ERTS_VSN
export DEBUG_BOOT

# Set DEBUG_BOOT to output verbose debugging info during execution
if [ ! -z "$DEBUG_BOOT" ]; then
    set -x
fi

# Ensure TERM is set to something usable
# for remote shells. The only time we'll
# override this value is if it is set to "dumb"
export TERM="${TERM:-xterm}"
if [ "$TERM" = "dumb" ]; then
    TERM=xterm
fi

# Include helpers, if the current release version supports it
if [ -d "$RELEASES_DIR/$REL_VSN/libexec" ]; then
    # shellcheck source=../libexec/logger.sh
    . "$RELEASES_DIR/$REL_VSN/libexec/logger.sh"
    # shellcheck source=../libexec/erts.sh
    . "$RELEASES_DIR/$REL_VSN/libexec/erts.sh"
    OTP_VER="$(otp_vsn)"
else
    # For backwards compatibility, do not perform the OTP check
    # If the active release version was built with an older Distillery
    OTP_VER="0"
fi
export OTP_VER

# Handles graceful shutdown in OTP <20
__pid=""
_cleanup() {
    trap - HUP INT TERM QUIT EXIT USR1 USR2
    set +e
    if "$RELEASES_DIR/$REL_VSN/$REL_NAME.sh" rpc init stop >/dev/null; then
        wait $__pid
        exit $?
    else
        exit 0
    fi
}

if [ "$OTP_VER" -ge 20 ]; then
    # Use OTP break handler
    exec "$RELEASES_DIR/$REL_VSN/$REL_NAME.sh" "$@"
else
    # Use our own break handling
    case "$1" in
        foreground)
            trap '_cleanup' HUP INT TERM QUIT EXIT USR1 USR2

            "$RELEASES_DIR/$REL_VSN/$REL_NAME.sh" "$@" &
            __pid=$!

            wait $__pid
            exit $?
            ;;
        *)
            exec "$RELEASES_DIR/$REL_VSN/$REL_NAME.sh" "$@"
            ;;
    esac
fi
