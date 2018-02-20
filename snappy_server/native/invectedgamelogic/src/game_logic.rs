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

/// Returns a new state with another player added.
pub fn add_player(state: &InvectedGameState, player_name: String) -> InvectedGameState {
    let mut players = (*state.players).clone();
    players.insert(player_name.clone(), Player {
        name: player_name,
        score: 0,
        position: (0.0, 0.0),
        velocity: (0.0, 0.0),
        acceleration: (0.0, 0.0),
        desired_movement: (0.0, 0.0),
    });

    InvectedGameState {game_round_time: state.game_round_time, players: Arc::new(players)}
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

    let mut new_state = state.clone();
    new_state.players = Arc::new(players);
    new_state
}

pub fn update_game_timestep(state: &InvectedGameState, dt: f64) -> InvectedGameState {
    move_players(state, dt)
}

fn move_player(orig_player: &Player, dt: f64) -> Player {
    let mut player = orig_player.clone();

    let (acc_x, acc_y) = player.acceleration;
    let (vel_x, vel_y) = player.velocity;
    let (pos_x, pos_y) = player.position;
    let (dir_x, dir_y) = player.desired_movement;

    player.position = (pos_x + vel_x * dt, pos_y + vel_y * dt);
    player.velocity = (vel_x + acc_x * dt, vel_y + acc_y * dt);
    player.acceleration = (acc_x + dir_x, acc_y + dir_y);

    // TODO cap these values.

    player
}

fn move_players(state: &InvectedGameState, dt: f64) -> InvectedGameState {
    let players: HashMap<String, Player> = (*state.players).values().map(|player| {
        let player_name = player.name.clone();
        (player_name, move_player(player, dt))
    }).collect();

    InvectedGameState {game_round_time: state.game_round_time, players: Arc::new(players)}
}


/// Prints information of a single player. Debugging function, will probably not be used later.
pub fn print_player(player: &Player) -> String {
    serde_json::to_string(player).unwrap()
}

/// Prints information of state as a whole. Debugging function, will probably not be used later.
pub fn print_state(state: &InvectedGameState) -> String {
    serde_json::to_string(state).unwrap()
}
