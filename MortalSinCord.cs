using MelonLoader;
using UnityEngine;
using DiscordRPC;
using Il2Cpp;
using Il2CppPlayerLogic;

[assembly: MelonInfo(typeof(MortalSinCord.MortalSinCord), "MortalSinCord", "1.0.0", "MustBeLeaving")]
[assembly: MelonGame("Doronik Games", "Mortal Sin")]

namespace MortalSinCord {
    public class MortalSinCord : MelonMod {
        public const string AppID = "1104295261024567406";
        public DiscordRpcClient? client;
        public string? scene;
        public DateTime time = DateTime.UtcNow;

        public override void OnInitializeMelon() {
            client = new DiscordRpcClient(AppID);
            client.Initialize();

            time = DateTime.UtcNow;
        }

        public override void OnFixedUpdate() {
            UpdateActivity();
        }

        private static string GetPlayerHealthMessage(Health h) {
            double perc = (double)h.CurrentHealth / h.MaxHealth;

            if (perc >= 0.8) return "Alive and well";
            if (perc >= 0.6) return "Struggling";
            if (perc >= 0.3) return "Holding on";
            if (perc >= 0.1) return "Barely holding on";

            return "There is no light";
        }
        
        private static string GetPlayerLocation(GameSession gs) {
            return 
            (gs.currentLevelProgressKey.LevelType switch {
                EndLevel.LevelType.Dungeon => "Exploring the Dungeon",
                EndLevel.LevelType.Cave => "Letching the Cave",
                EndLevel.LevelType.Forest => "Investigating the Forest",
                EndLevel.LevelType.MawOfSin => "Engulfed by The Maw",
                EndLevel.LevelType.TrialOfResolve => "Being tested in the Trial",
                _ => "Preparing in the Graveyard"
            }) 
            + " (" +
            gs.currentLevelProgressKey.AreaLetter switch {
                LevelProgressKey.AreaLetterType.A => "A",
                LevelProgressKey.AreaLetterType.B => "B",
                LevelProgressKey.AreaLetterType.C => "C",
                LevelProgressKey.AreaLetterType.D => "D",
                _ => "A"
            } + "-" + gs.currentLevelProgressKey.AreaLevel + ")";
        }
        private void UpdateActivity() {
            if (client == null) {
                MelonLogger.Error($"NullClient!");
                return;
            }

            GameObject pl_obj = GameObject.Find("Player");
            if (pl_obj == null) {
                client.SetPresence(new RichPresence() {
                    Details = "In the void",
                    State = "Exploring the Main Menu",
                    Assets = new Assets() {
                        LargeImageKey = "ms_header",
                        LargeImageText = "Mortal Sin",
                    }
                });

                time = DateTime.UtcNow;

                return;
            }

            Player pl = pl_obj.GetComponent<Player>();
            if (pl.isDead) {
                client.SetPresence(new RichPresence() {
                    Details = "In the abyss",
                    State = "Exploring mortality",
                    Assets = new Assets() {
                        LargeImageKey = "ms_header",
                        LargeImageText = "Mortal Sin",
                    }
                });

                time = DateTime.UtcNow;

                return;
            }

            GameSession gs = GameObject.Find("GameSession").GetComponent<GameSession>();
            
            client.SetPresence(new RichPresence() {
                Details = GetPlayerLocation(gs),
                State = GetPlayerHealthMessage(pl.health),
                Timestamps = new Timestamps() {
                    Start = time
                },
                Assets = new Assets() {
                    LargeImageKey = "ms_header",
                    LargeImageText = "Mortal Sin",
                    SmallImageKey = Global.PlayerClass.name switch {
                        "Struggler" => "sword",
                        "Mage" => "mace",
                        "Monk" => "unarmed",
                        "Berserker" => "manslayer",
                        "Reaper" => "sickle",
                        "Duelist" => "katanas",
                        "Hunter" => "spear",
                        "Vampire" => "unarmed",
                        "Martyr" => "sinblade",
                        "Gladiator" => "axe",
                        "Warlock" => "hellfire",
                        "Stalker" => "cleaver",
                        "Hoarder" => "cleavers",
                        _ => throw new NotImplementedException("(New Class) Please create a github issue!!!")
                    },
                    SmallImageText = Global.PlayerClass.name,
                }
            });;
        }
    }
}