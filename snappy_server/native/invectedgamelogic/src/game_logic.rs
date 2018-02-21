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

impl Player {
    pub fn new(player_name: String) -> Player {
        Player {
            name: player_name,
            score: 0,
            position: (0.0, 0.0),
            velocity: (0.0, 0.0),
            acceleration: (0.0, 0.0),
            desired_movement: (0.0, 0.0),
        }
    }
}

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct InvectedGameState {
    players: Arc<HashMap<String, Player>>,
    /// In seconds
    game_round_time: f64,
}

impl InvectedGameState {
    fn new() -> InvectedGameState {
        InvectedGameState {
            players: Arc::new(HashMap::new()),
            game_round_time: 0.0
        }
    }

    /// Moves the players around the board
    /// Keeps players in bounds
    /// Handles inter-player collisions
    fn move_players(self, dt: f64) -> InvectedGameState {

        let mut players = (*self.players).clone();
        for player in players.values_mut() {
            *player = move_player(player, dt);
        }

        // TODO Check and resolve inter-player collisions.

        InvectedGameState {players: Arc::new(players), .. self}
    }

    /// Checks if/who has picked up a power-up.
    /// TODO
    fn check_collision_with_powerup(self, _dt: f64) -> InvectedGameState {
        self
    }

    /// Gives player with largest area a point if interval to check score has passed again.
    /// TODO
    fn manage_scoring(self, _dt: f64) -> InvectedGameState {
        self
    }
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

    // TODO Cap these values.
    // TODO Keep player inside of game board.

    Player {acceleration, velocity, position, .. player.clone()}
}

/// Returns initial version of the game state.
pub fn init_game_state() -> InvectedGameState {
    InvectedGameState::new()
}

/// Renders the state to JSON.
pub fn render_state(state: &InvectedGameState) -> String {
    serde_json::to_string(state).unwrap()
}

/// Returns a new state with another player added.
pub fn add_player(state: &InvectedGameState, player_name: String) -> InvectedGameState {
    let mut players = (*state.players).clone();
    players.insert(player_name.clone(), Player::new(player_name));

    InvectedGameState {players: Arc::new(players), .. *state}
}

/// Updates the desired direction the player identified by `player_name` wants to move in.
/// based on their joystick input.
pub fn update_player_desired_movement(state: &InvectedGameState, player_name: &String, movement: (f64, f64)) -> InvectedGameState {
    let mut players = (*state.players).clone();
    if let Entry::Occupied(mut player_entry) = players.entry(player_name.clone()) {
        let mut new_player = player_entry.get().clone();
        new_player.desired_movement = movement;
        player_entry.insert(new_player);
    }

    InvectedGameState {players: Arc::new(players), .. *state}
}

/// Increments the game's state `dt` time to the future.
pub fn update_game_timestep(state: &InvectedGameState, dt: f64) -> InvectedGameState {
    state.clone()
        .move_players(dt)
        .check_collision_with_powerup(dt)
        .manage_scoring(dt)
}



/// Prints information of a single player. Debugging function, will probably not be used later.
pub fn print_player(player: &Player) -> String {
    serde_json::to_string(player).unwrap()
}

/// Prints information of state as a whole. Debugging function, will probably not be used later.
pub fn print_state(state: &InvectedGameState) -> String {
    serde_json::to_string(state).unwrap()
}
