extern crate serde;
extern crate serde_json;

use std::sync::Arc;

#[derive(Serialize, Deserialize, Clone)]
pub struct Player {
    position: (f64, f64),
    score: u64,
    name: String,
}

#[derive(Serialize, Deserialize, Clone)]
pub struct InvectedGameState {
    players: Arc<Vec<Player>>,
    /// In seconds
    game_round_time: f64,
}


pub fn init_game_state() -> InvectedGameState {
    InvectedGameState {players: Arc::new(vec![]), game_round_time: 0.0}
}

pub fn do_print_player(player: &Player) -> String {
    serde_json::to_string(player).unwrap()
}


pub fn do_print_state(state: &InvectedGameState) -> String {
    serde_json::to_string(state).unwrap()
}

pub fn do_add_player(state: &InvectedGameState, player_name: String) -> InvectedGameState {
    let mut players = (*state.players).clone();
    players.push(Player {name: player_name, score: 0, position: (0.0, 0.0) });

    InvectedGameState {game_round_time: state.game_round_time, players: Arc::new(players)}
}
