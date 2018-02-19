#[macro_use] extern crate rustler;
#[macro_use] extern crate lazy_static;
use rustler::resource::ResourceArc; // NifResource

#[macro_use] extern crate serde_derive;

mod game_logic;
use game_logic::Player as Player;
use game_logic::InvectedGameState as InvectedGameState;


use rustler::{NifEnv, NifTerm, NifResult, NifEncoder};

mod atoms {
    rustler_atoms! {
        atom ok;
        //atom error;
        //atom __true__ = "true";
        //atom __false__ = "false";
    }
}

rustler_export_nifs! {
    "Elixir.SnappyServer.InvectedGameLogic",
    [
        ("add", 2, add),
        ("print_nice_message", 0, print_nice_message),
        ("init_game_state", 0, init_game_state),
        ("add_player", 2, add_player),
        ("print_player", 1, print_player),
        ("print_state", 1, print_state),
    ],
    Some(on_load)
}

/// A testing function to see if NIF-invocation works properly
fn add<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let num1: i64 = try!(args[0].decode());
    let num2: i64 = try!(args[1].decode());

    Ok((atoms::ok(), num1 + num2).encode(env))
}

/// Another testing function
fn print_nice_message<'a>(env: NifEnv<'a>, _args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    println!("You are nice!");
    Ok(atoms::ok().encode(env))
}

pub fn on_load<'a>(env: NifEnv<'a>, _arg: NifTerm<'a>) -> bool {
    resource_struct_init!(Player, env);
    resource_struct_init!(InvectedGameState, env);
    true
}


fn init_game_state<'a>(env: NifEnv<'a>, _args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
 
    // let result = InvectedGameState { players: Vec.new![ Player {position: (0, 0), score: 1} ]};
    // let result = Player { position: (0.0, 0.0), score: 1, name: "Foobar".to_owned()};
    let state = game_logic::init_game_state();
    Ok(ResourceArc::new(state).encode(env))
}

fn add_player<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state_arc: ResourceArc<InvectedGameState> = try!(args[0].decode());
    let state : &InvectedGameState = &state_arc;
    let player_name: String = try!(args[1].decode());

    let new_state = game_logic::do_add_player(state, player_name);
    // let mut players = (*state.players).clone();
    // players.push(Player {name: player_name, score: 0, position: (0.0, 0.0) });

    // let new_state = InvectedGameState {game_round_time: state.game_round_time, players: Arc::new(players)};
    Ok(ResourceArc::new(new_state).encode(env))
}

fn print_player<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let player_arc: ResourceArc<Player> = try!(args[0].decode());
    let player: &Player = &player_arc;

    let player_json_str = game_logic::do_print_player(player);

    Ok(player_json_str.encode(env))
}

fn print_state<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state_arc: ResourceArc<InvectedGameState> = try!(args[0].decode());
    let state: &InvectedGameState = &state_arc;

    let state_json_str = game_logic::do_print_state(state);
    Ok(state_json_str.encode(env))
}
