#!/usr/bin/env bash

set -o posix
set -e

# Set DEBUG_BOOT to output verbose debugging info during execution
if [ ! -z "$DEBUG_BOOT" ]; then
    set -x
fi

# Name of the release
REL_NAME="${REL_NAME:-snappy_server}"
# Current version of the release
REL_VSN="${REL_VSN:-0.0.1}"
# Options passed to erl
ERL_OPTS="${ERL_OPTS:-}"
# Environment variables for run_erl
RUN_ERL_ENV="${RUN_ERL_ENV:-}"
# Current version of ERTS being used
# If this is not present/unset, it will be detected
ERTS_VSN="${ERTS_VSN:-9.1}"
export REL_NAME
export REL_VSN
export ERL_OPTS
export RUN_ERL_ENV
export ERTS_VSN

# If readlink has no -f option, or greadlink is not available,
# This function behaves like `readlink -f`
readlink_f() {
    __target_file="$1"
    cd "$(dirname "$__target_file")"
    __target_file=$(basename "$__target_file")

    # Iterate down a (possible) chain of symlinks
    while [ -L "$__target_file" ]
    do
        __target_file=$(readlink "$__target_file")
        cd "$(dirname "$__target_file")"
        __target_file=$(basename "$__target_file")
    done
    # Compute the canonicalized name by finding the physical path
    # for the directory we're in and appending the target file.
    __phys_dir=$(pwd -P)
    __result="$__phys_dir/$__target_file"
    echo "$__result"
}

# Locate the real path to this script
if uname | grep -q 'Darwin'; then
    # on OSX, best to install coreutils from homebrew or similar
    # to get greadlink
    if command -v greadlink >/dev/null 2>&1; then
        SCRIPT="${SCRIPT:-$(greadlink -f "$0")}"
    else
        SCRIPT="${SCRIPT:-$(readlink_f "$0")}"
    fi
else
    SCRIPT="${SCRIPT:-$(readlink -f "$0" )}"
fi

# Parent directory of this script
SCRIPT_DIR="$(dirname "${SCRIPT}")"
# Root directory of all releases
RELEASE_ROOT_DIR="${RELEASE_ROOT_DIR:-$(dirname "$(dirname "${SCRIPT_DIR}")")}"
# The location of builtin command scripts
RELEASE_LIBEXEC_DIR="$SCRIPT_DIR/libexec"

# shellcheck source=../libexec/logger.sh
. "$RELEASE_LIBEXEC_DIR/logger.sh"
# shellcheck source=../libexec/erts.sh
. "$RELEASE_LIBEXEC_DIR/erts.sh"
# shellcheck source=../libexec/helpers.sh
. "$RELEASE_LIBEXEC_DIR/helpers.sh"
# shellcheck source=../libexec/env.sh
. "$RELEASE_LIBEXEC_DIR/env.sh"
# shellcheck source=../libexec/config.sh
. "$RELEASE_LIBEXEC_DIR/config.sh"

# Make the release root our working directory
cd "$ROOTDIR"

# We export this so that custom tasks in Elixir
# which need to know how they were invoked can
# check this environment variable.
export DISTILLERY_TASK="$1"

# All commands are loaded from the currently active release
COMMAND_DIR="$RELEASE_LIBEXEC_DIR/commands"
COMMAND_NAME="${1:-help}"
COMMAND_PATH="$COMMAND_DIR/$COMMAND_NAME.sh"

# Only shift if there is more than 0 args, or it will exit non-zero
if [ "$#" -ne 0 ]; then
   shift
fi

# Handle aliases
case $COMMAND_NAME in
    start|start_boot)
        configure_release
        . "$COMMAND_DIR/start.sh" "$COMMAND_NAME" "$@"
        ;;
    upgrade|downgrade|install)
        configure_release
        . "$COMMAND_DIR/install.sh" "$COMMAND_NAME" "$@"
        ;;
    console|console_clean|console_boot)
        configure_release
        . "$COMMAND_DIR/console.sh" "$COMMAND_NAME" "$@"
        ;;
    *)
        # If the command exists in libexec, execute
        # Otherwise check to see if it's the name of a custom command
        if [ -f "$COMMAND_PATH" ]; then
            configure_release
            . "$COMMAND_PATH" "$@"
        else
            COMMAND_PATH="$REL_DIR/commands/$COMMAND_NAME.sh"
            if [ -f "$COMMAND_PATH" ]; then
                configure_release
                require_cookie
                . "$COMMAND_PATH" "$@"
            else
                notice "'$COMMAND_NAME' is not a valid command"
                . "$COMMAND_DIR/help.sh"
            fi
        fi
        ;;
esac
