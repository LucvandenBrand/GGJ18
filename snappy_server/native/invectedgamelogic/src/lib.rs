#[macro_use] extern crate rustler;
#[macro_use] extern crate rustler_codegen;
#[macro_use] extern crate lazy_static;
use rustler::resource::ResourceArc; // NifResource
use rustler::types::elixir_struct;


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
        ("init_game_state", 0, init_game_state)
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


struct Player {
    position: (f64, f64),
    score: u64,
}

pub fn on_load<'a>(env: NifEnv<'a>, _arg: NifTerm<'a>) -> bool {
    resource_struct_init!(Player, env);
    true
}

struct InvectedGameState {
    players: Vec<Player>,
}

fn init_game_state<'a>(env: NifEnv<'a>, _args: &[NifTerm<'a>]) -> NifResult<NifTerm<'a>> {
    
    // let result = InvectedGameState { players: Vec.new![ Player {position: (0, 0), score: 1} ]};
    let result = Player { position: (0.0, 0.0), score: 1 };
    Ok(ResourceArc::new(result).encode(env))
}
