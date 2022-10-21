using System.Collections.Generic;
using UnityEngine;

public partial class SoundController
{
    [Header("[REGISTER SOUND]")]
    public SoundInfor HomeBg;
    public SoundInfor InGameBg;
    public SoundInfor UIClick;

    public SoundInfor get_item;
    public SoundInfor get_pokemon;
    public SoundInfor get_boom;
    public SoundInfor pokemon_levelup;
    public SoundInfor pokemon_leveldown;
    public SoundInfor throw_item;
    public SoundInfor throw_key;
    public SoundInfor attack_boss_endgame;
    public SoundInfor attack_boss_blast;
    public SoundInfor tap;
    public SoundInfor attack_special_endgame;
    public SoundInfor attack_special_impact;
    public SoundInfor open_chest;
    public SoundInfor gem_collect;
    public SoundInfor update_wall;
    public SoundInfor firework;
    public SoundInfor player_drop_ground;
    public SoundInfor unlock_pokemon_win;
    public SoundInfor boy_get_hit;
    public SoundInfor girl_get_hit;
    public SoundInfor ball_impact;
    public SoundInfor footstep;
    public SoundInfor gameWin;
    public SoundInfor gameLose;


    public List<SoundInfor> lstPokemonAttack;
}