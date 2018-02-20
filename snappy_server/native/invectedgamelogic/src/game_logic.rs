extern crate serde;
extern crate serde_json;

use std::sync::Arc;
use std::collections::HashMap;
use std::collections::hash_map::Entry;

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct Player {
    position: (f64, f64),
    score: u64,
    name: String,
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
    players.insert(player_name.clone(), Player {name: player_name, score: 0, position: (0.0, 0.0) });

    InvectedGameState {game_round_time: state.game_round_time, players: Arc::new(players)}
}

pub fn update_state(state: &InvectedGameState, movements: & Vec<(String, (f64, f64))>) -> InvectedGameState {
    println!("{:?}", state);
    println!("{:?}", movements);
    let mut players = (*state.players).clone();
    for &(ref player_name, (xpos, ypos)) in movements {
        let name = player_name.clone();
        if let Entry::Occupied(mut player) = players.entry(name) {
            let score = player.get().score;
            player.insert(Player {name: player_name.clone(), score: score, position: (xpos, ypos)});
        }
    }
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
