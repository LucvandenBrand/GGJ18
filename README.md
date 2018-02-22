# GGJ18
A game developed for the [Global Game Jam](http://globalgamejam.org) 2018!

## :nut_and_bolt: Build and Run
This is a Unity project, so simply open the project from Unity and build it.
The project is setup under [Unity 2017.3.0 Windows](https://unity3d.com/get-unity/download/archive), other versions might work,
but **are not officially supported!**. Linux users might try [this](https://forum.unity.com/threads/unity-on-linux-release-notes-and-known-issues.350256/page-2) build.

## :briefcase: Branch Workflow
### Development
The development branch (or branches related to it) contains all active work.

### Master
The master branch contains all work that is production-ready. Before anything can be merged into master, it must be approved by a pull-request!

## :video_game: Unity Workflow
Within the `Scenes` folder, there is space for an ignored `KlootScenes` folder.
This folder is for you to mess around with on your own, 
the folder is never commited such that you do not need to worry about merge conflicts.

**Keep in mind: merge conflicts are truly avoided by proper communication!**

## Building Elixir Release

To ensure the server can be run on systems without Elixir itself, a release can be built using the following command:

```
MIX_ENV=prod mix release --executable
```

The release can then be found in
```
_build/prod/rel/snappy_server/bin/
```

And run with:

```
_build/prod/rel/snappy_server/bin/snappy_server foreground
```

## Installing and running Elixir for the server code:

The server code is contained in te `snappy_server` folder.

To install Elixir, I recommend using the `asdf version manager`.

https://github.com/asdf-vm/asdf

You'll need Erlang, Elixir and nodejs.

```
asdf plugin-add erlang
asdf plugin-add elixir
asdf plugin-add nodejs

# If problems during installing Erlang, install these packages first: https://github.com/asdf-vm/asdf-erlang#before-asdf-install
asdf install erlang 20.1
asdf global erlang 20.1

asdf install elixir 1.5.2
asdf global elixir 1.5.2

asdf install nodejs 9.4.0
asdf global nodejs 9.4.0
```


Then, go to the `snappy_server` folder and run `mix deps.get` to install the Elixir dependencies.

Then, go to the `snappy_server/assets` folder, and install the nodejs dependencies using `npm install`.

Then, you can start the server from `snapp_server` by typing `mix phx.server`.

While running this in your terminal, start the game in Unity. It will attempt to connect at (for now) port 8002.

The web-view for on phones can (for now) be seen at port 4000.
