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
    desired_direction: (f64, f64),
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
        desired_direction: (0.0, 0.0),
    });

    InvectedGameState {game_round_time: state.game_round_time, players: Arc::new(players)}
}

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
                desired_direction: (xdir, ydir)
            });
        }
    }
    InvectedGameState {game_round_time: state.game_round_time, players: Arc::new(players)}
}

fn move_player(orig_player: &Player, dt: f64) -> Player {
    let mut player = orig_player.clone();

    let (mut acc_x, mut acc_y) = player.acceleration;
    let (mut vel_x, mut vel_y) = player.velocity;
    let (mut pos_x, mut pos_y) = player.position;
    pos_x += vel_x * dt;
    pos_y += vel_y * dt;
    vel_x += acc_x * dt;
    vel_y += acc_y * dt;
    acc_x += player.desired_direction.0 * dt;
    acc_y *= player.desired_direction.1 * dt;

    // player.acceleration = (acc_x, acc_y);
    // let new_player = Player {
    //     name: *player_name,
    //     score: player.score
    // }
    player.position = (pos_x, pos_y);
    player.velocity = (vel_x, vel_y);
    player.acceleration = (acc_x, acc_y);

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
