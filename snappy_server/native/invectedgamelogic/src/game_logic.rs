extern crate serde;
extern crate serde_json;

use std::sync::Arc;
use std::collections::HashMap;
use std::collections::hash_map::Entry;

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct Player {
    name: String,
    score: u64,

    position: (f64, f64),
    velocity: (f64, f64),
    acceleration: (f64, f64),
    desired_movement: (f64, f64),
}

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct InvectedGameState {
    players: Arc<HashMap<String, Player>>,
    /// In seconds
    game_round_time: f64,
}

/// Returns initial version of the game state.
pub fn init_game_state() -> InvectedGameState {
    InvectedGameState {players: Arc::new(HashMap::new()), game_round_time: 0.0}
}

/// Renders the state to JSON.
pub fn render_state(state: &InvectedGameState) -> String {
    serde_json::to_string(state).unwrap()
}

fn new_player(player_name: String) -> Player {
    Player {
        name: player_name,
        score: 0,
        position: (0.0, 0.0),
        velocity: (0.0, 0.0),
        acceleration: (0.0, 0.0),
        desired_movement: (0.0, 0.0),
    }
}

/// Returns a new state with another player added.
pub fn add_player(state: &InvectedGameState, player_name: String) -> InvectedGameState {
    let mut players = (*state.players).clone();
    players.insert(player_name.clone(), new_player(player_name));

    InvectedGameState {players: Arc::new(players), .. *state}
}

// Deprecated
pub fn update_state(state: &InvectedGameState, movements: & Vec<(String, (f64, f64))>) -> InvectedGameState {
    println!("{:?}", state);
    println!("{:?}", movements);
    let mut players = (*state.players).clone();
    for &(ref player_name, (xdir, ydir)) in movements {
        let name = player_name.clone();
        if let Entry::Occupied(mut player) = players.entry(name) {
            let score = player.get().score;
            let velocity = player.get().velocity;
            let acceleration = player.get().acceleration;
            let position = player.get().position;
            player.insert(Player {
                name: player_name.clone(),
                score: score,

                position: position,
                velocity: velocity,
                acceleration: acceleration,
                desired_movement: (xdir, ydir)
            });
        }
    }
    InvectedGameState {game_round_time: state.game_round_time, players: Arc::new(players)}
}

pub fn update_player_desired_movement(state: &InvectedGameState, player_name: &String, movement: (f64, f64)) -> InvectedGameState {
    let mut players = (*state.players).clone();
    if let Entry::Occupied(mut player_entry) = players.entry(player_name.clone()) {
        let mut new_player = player_entry.get().clone();
        new_player.desired_movement = movement;
        player_entry.insert(new_player);
    }

    InvectedGameState {players: Arc::new(players), .. *state}
}

pub fn update_game_timestep(state: &InvectedGameState, dt: f64) -> InvectedGameState {
    move_players(state, dt)
}

fn move_player(player: &Player, dt: f64) -> Player {
    let &Player {
        acceleration: (acc_x, acc_y),
        velocity: (vel_x, vel_y),
        position: (pos_x, pos_y),
        desired_movement: (dir_x, dir_y),
        ..
    } = player;

    let position = (pos_x + vel_x * dt, pos_y + vel_y * dt);
    let velocity = (vel_x + acc_x * dt, vel_y + acc_y * dt);
    let acceleration = (acc_x + dir_x, acc_y + dir_y);

    // TODO cap these values.

    Player {acceleration, velocity, position, .. player.clone()}
}

fn move_players(state: &InvectedGameState, dt: f64) -> InvectedGameState {
    let players: HashMap<String, Player> = (*state.players).values().map(|player| {
        let player_name = player.name.clone();
        (player_name, move_player(player, dt))
    }).collect();

    InvectedGameState {players: Arc::new(players), .. *state}
}


/// Prints information of a single player. Debugging function, will probably not be used later.
pub fn print_player(player: &Player) -> String {
    serde_json::to_string(player).unwrap()
}

/// Prints information of state as a whole. Debugging function, will probably not be used later.
pub fn print_state(state: &InvectedGameState) -> String {
    serde_json::to_string(state).unwrap()
}
