#[macro_use] extern crate rustler;
#[macro_use] extern crate lazy_static;
use rustler::resource::ResourceArc; // NifResource

#[macro_use] extern crate serde_derive;

mod game_logic;
use game_logic::Player as Player;
use game_logic::InvectedGameState as InvectedGameState;


use rustler::{NifEnv, NifTerm, NifResult, NifEncoder};
use rustler::types::NifMapIterator;

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
        ("init_game_state", 0, init_game_state),
        ("add_player", 2, add_player),
        ("render_state", 1, render_state),
        ("update_player_desired_movement", 3, update_player_desired_movement),
        ("update_game_timestep", 2, update_game_timestep),

        ("update_state", 2, update_state), // deprecated
        ("print_player", 1, print_player),
        ("print_state", 1, print_state),
    ],
    Some(on_load)
}

pub fn on_load<'a>(env: NifEnv<'a>, _arg: NifTerm<'a>) -> bool {
    resource_struct_init!(Player, env);
    resource_struct_init!(InvectedGameState, env);
    true
}


fn init_game_state<'a>(env: NifEnv<'a>, _args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state = game_logic::init_game_state();
    Ok(ResourceArc::new(state).encode(env))
}

fn add_player<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state_arc: ResourceArc<InvectedGameState> = try!(args[0].decode());
    let state : &InvectedGameState = &state_arc;
    let player_name: String = try!(args[1].decode());

    let new_state = game_logic::add_player(state, player_name);
    Ok(ResourceArc::new(new_state).encode(env))
}

fn update_player_desired_movement<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state_arc: ResourceArc<InvectedGameState> = try!(args[0].decode());
    let state: &InvectedGameState = &state_arc;
    let player_name: String = try!(args[1].decode());
    let desired_movement: (f64, f64) = try!(args[2].decode());

    let new_state = game_logic::update_player_desired_movement(state, &player_name, desired_movement);
    Ok(ResourceArc::new(new_state).encode(env))
}

fn update_game_timestep<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state_arc: ResourceArc<InvectedGameState> = try!(args[0].decode());
    let state: &InvectedGameState = &state_arc;
    let dt: f64 = try!(args[1].decode());

    let new_state = game_logic::update_game_timestep(state, dt);
    Ok(ResourceArc::new(new_state).encode(env))
}

fn update_state<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state_arc: ResourceArc<InvectedGameState> = try!(args[0].decode());
    let state : &InvectedGameState = &state_arc;
    let movements_iter: NifMapIterator = args[1].decode()?;

    let mut movements = vec![];
    for (key, value) in movements_iter {
        let key_str = key.decode::<String>()?;
        let value_float = value.decode::<(f64, f64)>()?;
        movements.push((key_str, value_float));
    }

    let new_state = game_logic::update_state(state, &movements);
    Ok(ResourceArc::new(new_state).encode(env))
}

fn render_state<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state_arc: ResourceArc<InvectedGameState> = try!(args[0].decode());
    let state: &InvectedGameState = &state_arc;

    let state_json_str = game_logic::render_state(state);
    Ok(state_json_str.encode(env))
}



fn print_player<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let player_arc: ResourceArc<Player> = try!(args[0].decode());
    let player: &Player = &player_arc;

    let player_json_str = game_logic::print_player(player);
    Ok(player_json_str.encode(env))
}

fn print_state<'a>(env: NifEnv<'a>, args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    let state_arc: ResourceArc<InvectedGameState> = try!(args[0].decode());
    let state: &InvectedGameState = &state_arc;

    let state_json_str = game_logic::print_state(state);
    Ok(state_json_str.encode(env))
}
