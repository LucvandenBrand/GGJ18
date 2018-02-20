:: This batch file handles managing an Erlang node as a Windows service.
::
:: Check the :usage section for information about what this script provides

:: Set variables that describe the release
@set rel_name=snappy_server
@set rel_vsn=0.0.1
@set erts_vsn=9.1
@set erl_opts=

:: Discover the release root directory from the directory
:: of this script
@set script_dir=%~dp0
@for %%A in ("%script_dir%\..\..") do @(
  set release_root_dir=%%~fA
)
@set rel_dir=%release_root_dir%\releases\%rel_vsn%

@if "%RELEASE_MUTABLE_DIR%"=="" (
  set mutable_dir=%release_root_dir%\var
) else (
  set mutable_dir=%RELEASE_MUTABLE_DIR%
)

@if "%RELEASE_CONFIG_DIR%"=="" (
  set config_dir=%release_root_dir%
) else (
  set config_dir=%RELEASE_CONFIG_DIR%
)

@set log_dir=%mutable_dir%\log

:: Create directories which don't exist
@if not exist %mutable_dir% mkdir %mutable_dir%
@if not exist %config_dir% mkdir %config_dir%
@if not exist %log_dir% mkdir %log_dir%

@call :find_erts_dir
@call :find_sys_config
@call :find_vm_args
@call :set_boot_script_var

@set service_name=%rel_name%_%rel_vsn%
@set bindir=%erts_dir%\bin
@set progname=erl.exe
@set clean_boot_script=%release_root_dir%\bin\start_clean
@set erlsrv="%bindir%\erlsrv.exe"
@set epmd="%bindir%\epmd.exe"
@set escript="%bindir%\escript.exe"
@set werl="%bindir%\werl.exe"
@set nodetool="%release_root_dir%\bin\nodetool"
@set consolidated_dir=%rootdir%\lib\%rel_name%-%rel_vsn%\consolidated
@set hook_dir=%rel_dir%\hooks
@set command_dir=%rel_dir%\commands

:: Extract node type and name from vm.args
@for /f "usebackq tokens=1-2" %%I in (`findstr /b "\-name \-sname" "%vm_args%"`) do @(
  set node_type=%%I
  set node_name=%%J
)

:: Extract cookie from vm.args
@for /f "usebackq tokens=1-2" %%I in (`findstr /b \-setcookie "%vm_args%"`) do @(
  set cookie=%%J
)

:: Write the erl.ini file to set up paths relative to this script
@call :write_ini

:: If a start.boot file is not present, copy one from the named .boot file
@if not exist "%rel_dir%\start.boot" (
  copy "%rel_dir%\%rel_name%.boot" "%rel_dir%\start.boot" >nul
)


@set taskname="%1"
@shift /1
@set args=

@setlocal EnableDelayedExpansion
:parse_args
@set param=%1
@if "!param!"=="" @goto :args_parsed
@if "!args!"=="" (
  @set largs=%param%
) else (
  @set largs=%largs% %param%
)
@set "delim= "
@shift /1
@goto :parse_args
:args_parsed
@endlocal & @set args=%largs%

@if %taskname%=="install" @goto install
@if %taskname%=="uninstall" @goto uninstall
@if %taskname%=="start" @goto start
@if %taskname%=="foreground" @goto foreground
@if %taskname%=="console" @goto console
@if %taskname%=="stop" @goto stop
@if %taskname%=="restart" @call :stop && @goto start
@if %taskname%=="reboot" @call :stop && @goto start
@if %taskname%=="reload_config" @goto reload_config
@if %taskname%=="upgrade" @goto upgrade
@if %taskname%=="downgrade" @goto upgrade
@if %taskname%=="attach" @goto remote_console
@if %taskname%=="remote_console" @goto remote_console
@if %taskname%=="pid" @goto pid
@if %taskname%=="ping" @goto ping
@if %taskname%=="pingpeer" @goto pingpeer
@if %taskname%=="escript" @goto escript
@if %taskname%=="rpc" @goto rpc
@if %taskname%=="rpcterms" @goto rpcterms
@if %taskname%=="eval" @goto eval
@if %taskname%=="command" @goto command
@if %taskname%=="describe" @goto describe
@if %taskname%=="" @goto usage
@if exist "%command_dir%\%taskname%" (
  @"%command_dir%\%taskname%" %*
) else (
  @echo Unknown command: %taskname%
)
@goto :eof

:: Find the ERTS dir
:find_erts_dir
@if "%erts_vsn%"=="" (
  call :use_system_erts
) else (
  call :use_bundled_erts
)
@goto :eof

:: Load system ERTS info
:use_system_erts
@for /f "delims=" %%i in ('where erl') do set system_erl="%%i"
@if %system_erl%=="" (
  echo failed to locate the Erlang Runtime System!
  exit 1
)
@set system_root_dir_cmd=%system_erl% -noshell -eval "io:format(\"~s\", [filename:nativename(code:root_dir())])." -s init stop
@for /f "delims=" %%i in (`%%system_root_dir_cmd%%`) do set system_root=%%i
@set system_erts_vsn_cmd=%system_erl% -noshell -eval "Ver=erlang:system_info(version),io:format(\"~s\", [Ver])" -s init stop
@for /f "delims=" %%i in (`%%system_erts_vsn_cmd%%`) do set system_erts_vsn=%%i
@set erl=%system_erl%
@set erts_vsn=%system_erts_vsn%
@set rootdir=%system_root%
@set erts_dir=%rootdir%\erts-%erts_vsn%
@goto :eof

:: Load bundled ERTS info
:use_bundled_erts
@set erts_dir=%release_root_dir%\erts-%erts_vsn%
@set erl=%erts_dir%\bin\erl
@set rootdir=%release_root_dir%
@goto :eof

:: Find the sys.config file
:find_sys_config
@set possible_sys=%release_config_dir%\sys.config
@if exist %possible_sys% (
  set sys_config_src=%possible_sys%
) else (
  set sys_config_src=%rel_dir%\sys.config
)
@set sys_config_target=%config_dir%\sys.config
:: Replace environment variables
@powershell -command "$content = get-content %sys_config_src%; [regex]::matches($content, '\${[\w\d_-]+}') | foreach { $content = $content.Replace($_.value, [System.Environment]::GetEnvironmentVariable($_.value)) }; out-file -filepath %sys_config_target% -inputobject $content -encoding ascii" 
@set sys_config=%sys_config_target%
@goto :eof

:find_vm_args
@set possible_vmargs=%release_config_dir%\vm.args
@if exist %possible_vmargs% (
  set vmargs_src=%possible_vmargs%
) else (
  set vmargs_src=%rel_dir%\vm.args
)
@set vmargs_target=%config_dir%\vm.args
:: Replace environment variables
@powershell -command "$content = get-content %vmargs_src%; [regex]::matches($content, '\${[\w\d_-]+}') | foreach { $content = $content.Replace($_.value, [System.Environment]::GetEnvironmentVariable($_.value)) }; out-file -filepath %vmargs_target% -inputobject $content -encoding ascii" 
@set vm_args=%vmargs_target%
@goto :eof

:: set boot_script variable
:set_boot_script_var
@if exist %rel_dir%\%rel_name%.boot (
  set boot_script=%rel_dir%\%rel_name%
) else (
  set boot_script=%rel_dir%\start
)
@goto :eof

:: Write the erl.ini file
:write_ini
@set erl_ini=%erts_dir%\bin\erl.ini
@set converted_bindir=%bindir:\=\\%
@set converted_rootdir=%rootdir:\=\\%
@echo [erlang] > "%erl_ini%"
@echo Bindir=%converted_bindir% >> "%erl_ini%"
@echo Progname=%progname% >> "%erl_ini%"
@echo Rootdir=%converted_rootdir% >> "%erl_ini%"
@goto :eof

:: Display usage information
:usage
@echo Usage: %~n0 ^<task^>
@echo.
@echo Service Control
@echo =====================
@echo install                             # installs the release as a service
@echo uninstall                           # uninstalls the release service
@echo start                               # starts the service
@echo start_boot                          # not implemented
@echo foreground                          # start the release in the foreground
@echo console                             # run the release in a local shell
@echo console_clean                       # not implemented
@echo stop                                # stops the service
@echo restart                             # restarts the release service
@echo reboot                              # alias for restart on Windows
@echo reload_config                       # reload the current system's configuration from disk
@echo.
@echo Upgrades
@echo =====================
@echo upgrade ^<version^>                   # upgrade the current release service
@echo downgrade ^<version^>                 # downgrade the current release service
@echo.
@echo Utilities
@echo =====================
@echo attach                              # alias for remote_console on Windows
@echo remote_console                      # open a remote console connected to the release
@echo pid                                 # get the process id of the running release
@echo ping                                # ping the current release service, pong is returned if ok
@echo pingpeer ^<peer^>                     # check if a peer node is running, pong is returned if ok
@echo escript ^<file^>                      # execute an escript
@echo rpc ^<mod^> ^<fun^> ^[^<args..^>^]          # execute an RPC call using the given MFA
@echo rpcterms ^<mod^> ^<fun^> ^[^<expr^>^]       # execute an RPC call using the given Erlang expr for args
@echo eval ^<expr^>                         # execute the given Erlang expr on the running node
@echo command ^<mod^> ^<fun^> ^[^<args..^>^]      # execute the given module/function in a clean VM
@echo describe                            # list details about the release
@echo.
@echo Custom Commands
@echo ======================
@for %%i in (%command_dir%\*.d\*.bat) do (
  echo %%~ni
)
@goto :eof

:: Install the release as a Windows service
:: or install the specified version passed as argument
:install
@setlocal EnableDelayedExpansion
@set service_args=%erl_opts% -setcookie "!cookie!" ++ ^
       -rootdir "%rootdir%" ^
       -reldir "%release_root_dir%\releases"
@set start_erl=%erts_dir%\bin\start_erl.exe
@set description=Erlang node %node_name% in %rootdir%
@if "" == "%args%" (
  :: Install the service
  %erlsrv% add %service_name% ^
            -c "%description%" ^
            %node_type% "%node_name%" ^
            -w "%mutable_dir%" ^
            -m "%start_erl%" ^
            -args "!service_args!" ^
            -debugtype new ^
            -stopaction "init:stop()."
  endlocal
) else (
  :: relup and reldown
  endlocal
  goto upgrade
)
@goto :eof

:: Uninstall the Windows service
:uninstall
@%erlsrv% remove %service_name%
@%epmd% -kill
@goto :eof

:: Start the Windows service
:start
@call :pre_configure_hooks
@call :post_configure_hooks
@call :pre_start_hooks
@%erlsrv% start %service_name%
@call :post_start_hooks
@goto :eof

:: Stop the Windows service
:stop
@call :pre_stop_hooks
@%erlsrv% stop %service_name%
@call :post_stop_hooks
@goto :eof

:: Upgrade this release
:upgrade
@setlocal EnableDelayedExpansion
@if "" == "%args%" (
  echo Missing version argument
  echo Usage: %rel_name% %taskname% {version}
  set ERRORLEVEL=1
  exit /b %ERRORLEVEL%
)
@set source_version=%rel_vsn%
@set target_version=%args%
:: Unpack
@%escript% "%root_dir%\bin\release_utils.escript" ^
    unpack_release %rel_name% "%node_type%" "%node_name%" "!cookie!" "%target_version%"
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
:: Update env
@set rel_vsn=%target_version%
@set rel_dir=%release_root_dir\releases\%target_version%
:: TODO: init_configs
if exists "%rel_dir%\sys.config.bak" do (
  @move /Y "%rel_dir%\sys.config.bak" "%rel_dir%\sys.config"
)
if exists "%rel_dir%\vm.args.bak" do (
  @move /Y "%rel_dir%\vm.args.bak" "%rel_dir%\vm.args"
)
@copy /B /Y "%rel_dir%/sys.config" "%rel_dir%\sys.config.bak"
@copy /B /Y "%rel_dir%/vm.args" "%rel_dir%\vm.args.bak"
@copy /B /Y "%sys_config" "%rel_dir%/sys.config"
@copy /B /Y "%vm_args%" "%rel_dir%/vm.args"
:: Run pre hooks
@call :pre_configure_hooks
@call :post_configure_hooks
@call :pre_upgrade_hooks
:: Perform upgrade
@%escript% "%rootdir%/bin/release_utils.escript" ^
    install_release "%rel_name%" "%node_type%" "%node_name%" "!cookie!" "%target_version%"
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
:: Cleanup
@move /Y "%rel_dir%\sys.config.bak" "%rel_dir%\sys.config"
@move /Y "%rel_dir%\vm.args.bak" "%rel_dir%\vm.args"
:: Run post hooks
@call :post_upgrade_hooks
@endlocal
@goto :eof

:: Start a console
:console
@call :pre_configure_hooks
@call :post_configure_hooks
@call :pre_start_hooks
@start "%rel_name% console" %werl% -boot "%boot_script%" ^
       -config "%sys_config%"  ^
       -args_file "%vm_args%" ^
       -user Elixir.IEx.CLI ^
       -extra --no-halt +iex
@goto :eof

:: Start the release in the foreground
:foreground
@call :pre_configure_hooks
@call :post_configure_hooks
@call :pre_start_hooks
@set EMU=beam
@set PROGNAME=%~n0
@start "" /B "%erl%" -noshell -noinput +Bd ^
       -boot "%boot_script%" ^
       -boot_var ERTS_LIB_DIR "%erts_dir%\..\lib" ^
       -env ERL_LIBS "%release_root_dir%\lib%" ^
       -pa "%consolidated_dir%" ^
       -args_file "%vm_args%" ^
       -config "%sys_config%" ^
       %erl_opts% ^
       -extra %extra_opts%
@call :set_pid
@call :post_start_hooks

:wait_for_exit
@tasklist /FI "PID eq %pid%" | find /I "erl.exe"
@if %ERRORLEVEL% geq 1 (
  :: Process has shutdown
  @call :post_stop_hooks
) else (
  @sleep 5
  @goto :wait_for_exit
)
@goto :eof

:: Ping the running node
:ping
@setlocal EnableDelayedExpansion
@%escript% %nodetool% ping %node_type% "%node_name%" -setcookie "!cookie!"
@endlocal
@goto :eof

:: Ping a peer node
:: Ping a peer node
:pingpeer
@setlocal EnableDelayedExpansion
@if "%args"=="" (
  echo "Peer name is required!"
  exit 1
) else (
  %escript% %nodetool% ping %node_type% "%args%" -setcookie "!cookie!"
)
@endlocal
@goto :eof

:: Execute an escript
:escript
@%escript% %args%
@goto :eof

:: Execute an RPC command
:rpc
@call :ping>NUL
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
@setlocal EnableDelayedExpansion
@%escript% %nodetool% rpc %node_type% %node_name% -setcookie "!cookie!" %args%
@endlocal
@goto :eof

:: Execute an RPC command where args are expressed
:: in the form of an Erlang expression
:rpcterms
@call :ping>NUL
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
@setlocal EnableDelayedExpansion
@%escript% %nodetool% rpcterms %node_type% %node_name% -setcookie "!cookie!" %args%
@endlocal
@goto :eof

:: Eval an expression on the running node
:eval
@call :ping>NUL
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
@setlocal EnableDelayedExpansion
@%escript% %nodetool% eval %node_type% %node_name% -setcookie "!cookie!" !args!
@endlocal
@goto :eof


:: Execute a command in a clean Erlang VM
:command
@for /f "tokens=1-2*" %%a in ("%args%") do (
  @set module=%%a
  @set function=%%b
  @set command_args=%%c
)
@%erl% -boot_var ERTS_LIB_DIR "%erts_dir%\..\lib" ^
       -hidden -noshell -boot start_clean ^
       -pa "%consolidated_dir%" ^
       -s %module% %function% -s init stop
       -extra %command_args%
@goto :eof

:: Get the process id of the running node
:pid
@call :ping>NUL
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
@setlocal EnableDelayedExpansion
@%escript% %nodetool% eval %node_type% %node_name% -setcookie "!cookie!" "erlang:list_to_integer(os:getpid())."
@endlocal
@goto :eof

:set_pid
@call :ping>NUL
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
@setlocal EnableDelayedExpansion
@set get_pid_cmd=%escript% %nodetool% eval %node_type% %node_name% -setcookie "!cookie!" "erlang:list_to_integer(os:getpid()).'
@for /f "delims=" %%i in (`%%get_pid_cmd%%`) do set pid=%%i
@endlocal
@goto :eof

:: Reload the running configuration
:reload_config
@call :ping>NUL
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
@call :pre_configure_hooks
@call :post_configure_hooks
@setlocal EnableDelayedExpansion
@%escript% %nodetool% reload_config %node_type% %node_name% -setcookie "!cookie!" "%sys_config%"
@endlocal
@goto :eof

:: Describe this release
:describe
@setlocal EnableDelayedExpansion
@echo Service Information
@echo =====================
@%erlsrv% list %service_name%
@echo.
@echo Release Metadata
@echo =====================
@echo ERTS                %erts_vsn%
@echo Release Directory   %rel_dir%
@echo Config File         %sys_config%
@echo Args File           %vm_args%
@echo Name Type           %node_type%
@echo Node Name           %node_name%
@echo Node Cookie         !cookie!
@echo Extra Opts          %erl_opts%
@echo.
@echo Hooks:
@for %%i in (%hook_dir%\*.d\*.bat) do (
  echo %%i
)
@echo.
@echo Commands:
@for %%i in (%command_dir%\*.d\*.bat) do (
  echo %%i
)
@endlocal
@goto :eof

:: Remote shell to a running node
:remote_console
@call :ping
@if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
@setlocal EnableDelayedExpansion
@set id=remsh%RANDOM%-%node_name%
@start "%node_name% attach" %werl% -hidden -noshell -boot "%clean_boot_script%" ^
       -user Elixir.IEx.CLI %node_type% %id% -setcookie "!cookie!" ^
       -extra --no-halt +iex -%node_type% %id% --cookie "!cookie!" --remsh %node_name%
@endlocal
@goto :eof

:: Hook helpers
:execute_hooks
@for /r "%hook_dir%\%1.d" %%f in (*.bat) do (
  @echo Running %1 hook: %%f
  %%f
  @if %ERRORLEVEL% GEQ 1 exit %ERRORLEVEL%
)
@goto :eof

:pre_configure_hooks
@call :execute_hooks pre_configure
@goto :eof

:post_configure_hooks
@call :execute_hooks post_configure
@goto :eof

:pre_start_hooks
@call :execute_hooks pre_start
@goto :eof

:post_start_hooks
@call :execute_hooks post_start
@goto :eof

:pre_stop_hooks
@call :execute_hooks pre_stop
@goto :eof

:post_stop_hooks
@call :execute_hooks post_stop
@goto :eof

:pre_upgrade_hooks
@call :execute_hooks pre_upgrade
@goto :eof

:post_upgrade_hooks
@call :execute_hooks post_upgrade
@goto :eof
