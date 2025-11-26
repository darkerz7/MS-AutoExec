using Sharp.Shared.Enums;
using Sharp.Shared.Objects;

namespace MS_AutoExec
{
    public class ConfigJSON
    {
        public List<EventInfo> OnMapSpawn { get; set; }
        public List<EventInfo> OnMapEnd { get; set; }
        public List<EventInfo> OnRoundStartAlways { get; set; }
        public List<EventInfo> OnRoundStartWarmUp { get; set; }
        public List<EventInfo> OnRoundStartAfterWarmUp { get; set; }
        public List<EventInfo> OnRoundPrestart { get; set; }
        public List<EventInfo> OnRoundEndAlways { get; set; }
        public List<EventInfo> OnRoundEndWarmUp { get; set; }
        public List<EventInfo> OnRoundEndAfterWarmUp { get; set; }
        public List<EventInfo> OnSecondHalf { get; set; }
        public List<EventInfo> OnSecondHalfEnd { get; set; }
        public List<EventInfo> OnSecondHalfPrestart { get; set; }
        public List<EventInfo> OnFirstHalf { get; set; }
        public List<EventInfo> OnFirstHalfEnd { get; set; }
        public List<EventInfo> OnFirstHalfPrestart { get; set; }
        public List<EventInfo> OnPreHalfTime { get; set; }
        public List<EventInfo> OnPreHalfTimeEnd { get; set; }
        public List<EventInfo> OnPreHalfTimePrestart { get; set; }
        public void OnMapSpawnHandler()
        {
            StopTimers();
            foreach (EventInfo e in OnMapSpawn) e.EventInfoHandler();
        }
        public void OnMapEndHandler()
        {
            KillAllTimers(OnMapSpawn);
            KillAllTimers(OnRoundStartAlways);
            KillAllTimers(OnRoundStartWarmUp);
            KillAllTimers(OnRoundStartAfterWarmUp);
            KillAllTimers(OnSecondHalf);
            KillAllTimers(OnFirstHalf);
            KillAllTimers(OnPreHalfTime);
            KillAllTimers(OnRoundPrestart);
            KillAllTimers(OnSecondHalfPrestart);
            KillAllTimers(OnFirstHalfPrestart);
            KillAllTimers(OnPreHalfTimePrestart);

            foreach (EventInfo e in OnMapEnd) e.EventInfoHandler();
        }
        public void OnRoundStartHandler()
        {
            KillAllTimers(OnRoundEndAlways);
            KillAllTimers(OnRoundEndWarmUp);
            KillAllTimers(OnRoundEndAfterWarmUp);
            KillAllTimers(OnSecondHalfEnd);
            KillAllTimers(OnFirstHalfEnd);
            KillAllTimers(OnPreHalfTimeEnd);

            foreach (EventInfo e in OnRoundStartAlways) e.EventInfoHandler();

            var gamerules = AutoExec._modSharp!.GetGameRules();

            if (gamerules.IsWarmupPeriod) foreach (EventInfo e in OnRoundStartWarmUp) e.EventInfoHandler();
            else foreach (EventInfo e in OnRoundStartAfterWarmUp) e.EventInfoHandler();

            if (gamerules.GamePhase == GamePhase.PlayingFirstHalf) foreach (EventInfo e in OnFirstHalf) e.EventInfoHandler();
            else if (gamerules.GamePhase == GamePhase.PlayingSecondHalf) foreach (EventInfo e in OnSecondHalf) e.EventInfoHandler();

            if (AutoExec._convars!.FindConVar("mp_maxrounds") is { } cvar_maxround && cvar_maxround.GetInt32() is { } maxround && maxround > 0 && gamerules.GamePhase == GamePhase.PlayingFirstHalf && maxround / 2 == gamerules.TotalRoundsPlayed - 1) foreach (EventInfo e in OnPreHalfTime) e.EventInfoHandler();
        }
        public void OnRoundEndHandler()
        {
            KillAllTimers(OnRoundStartAlways);
            KillAllTimers(OnRoundStartWarmUp);
            KillAllTimers(OnRoundStartAfterWarmUp);
            KillAllTimers(OnSecondHalf);
            KillAllTimers(OnFirstHalf);
            KillAllTimers(OnPreHalfTime);
            KillAllTimers(OnRoundPrestart);
            KillAllTimers(OnSecondHalfPrestart);
            KillAllTimers(OnFirstHalfPrestart);
            KillAllTimers(OnPreHalfTimePrestart);

            foreach (EventInfo e in OnRoundEndAlways) e.EventInfoHandler();

            var gamerules = AutoExec._modSharp!.GetGameRules();

            if (gamerules.IsWarmupPeriod) foreach (EventInfo e in OnRoundEndWarmUp) e.EventInfoHandler();
            else foreach (EventInfo e in OnRoundEndAfterWarmUp) e.EventInfoHandler();

            if (gamerules.GamePhase == GamePhase.PlayingFirstHalf) foreach (EventInfo e in OnFirstHalfEnd) e.EventInfoHandler();
            else if (gamerules.GamePhase == GamePhase.PlayingSecondHalf) foreach (EventInfo e in OnSecondHalfEnd) e.EventInfoHandler();

            if (AutoExec._convars!.FindConVar("mp_maxrounds") is { } cvar_maxround && cvar_maxround.GetInt32() is { } maxround && maxround > 0 && gamerules.GamePhase == GamePhase.PlayingFirstHalf && maxround / 2 == gamerules.TotalRoundsPlayed - 1) foreach (EventInfo e in OnPreHalfTimeEnd) e.EventInfoHandler();
        }
        public void OnRoundPrestartHandler()
        {
            foreach (EventInfo e in OnRoundPrestart) e.EventInfoHandler();

            var gamerules = AutoExec._modSharp!.GetGameRules();

            if (gamerules.GamePhase == GamePhase.PlayingFirstHalf) foreach (EventInfo e in OnFirstHalfPrestart) e.EventInfoHandler();
            else if (gamerules.GamePhase == GamePhase.PlayingSecondHalf) foreach (EventInfo e in OnSecondHalfPrestart) e.EventInfoHandler();

            if (AutoExec._convars!.FindConVar("mp_maxrounds") is { } cvar_maxround && cvar_maxround.GetInt32() is { } maxround && maxround > 0 && gamerules.GamePhase == GamePhase.PlayingFirstHalf && maxround / 2 == gamerules.TotalRoundsPlayed - 1) foreach (EventInfo e in OnPreHalfTimePrestart) e.EventInfoHandler();
        }
        public void StopTimers()
        {
            KillAllTimers(OnMapSpawn);
            KillAllTimers(OnMapEnd);
            KillAllTimers(OnRoundStartAlways);
            KillAllTimers(OnRoundStartWarmUp);
            KillAllTimers(OnRoundStartAfterWarmUp);
            KillAllTimers(OnRoundEndAlways);
            KillAllTimers(OnRoundEndWarmUp);
            KillAllTimers(OnRoundEndAfterWarmUp);
            KillAllTimers(OnSecondHalf);
            KillAllTimers(OnSecondHalfEnd);
            KillAllTimers(OnFirstHalf);
            KillAllTimers(OnFirstHalfEnd);
            KillAllTimers(OnPreHalfTime);
            KillAllTimers(OnPreHalfTimeEnd);
            KillAllTimers(OnRoundPrestart);
            KillAllTimers(OnSecondHalfPrestart);
            KillAllTimers(OnFirstHalfPrestart);
            KillAllTimers(OnPreHalfTimePrestart);
        }
        static void KillAllTimers(List<EventInfo> ListEventInfo)
        {
            foreach (EventInfo e in ListEventInfo) e.KillTimer();
        }
        public ConfigJSON()
        {
            OnMapSpawn = [];
            OnMapEnd = [];
            OnRoundStartAlways = [];
            OnRoundStartWarmUp = [];
            OnRoundStartAfterWarmUp = [];
            OnRoundEndAlways = [];
            OnRoundEndWarmUp = [];
            OnRoundEndAfterWarmUp = [];
            OnSecondHalf = [];
            OnSecondHalfEnd = [];
            OnFirstHalf = [];
            OnFirstHalfEnd = [];
            OnPreHalfTime = [];
            OnPreHalfTimeEnd = [];
            OnRoundPrestart = [];
            OnSecondHalfPrestart = [];
            OnFirstHalfPrestart = [];
            OnPreHalfTimePrestart = [];
        }
        ~ConfigJSON()
        {
            OnMapSpawn.Clear();
            OnMapEnd.Clear();
            OnRoundStartAlways.Clear();
            OnRoundStartWarmUp.Clear();
            OnRoundStartAfterWarmUp.Clear();
            OnRoundEndAlways.Clear();
            OnRoundEndWarmUp.Clear();
            OnRoundEndAfterWarmUp.Clear();
            OnSecondHalf.Clear();
            OnSecondHalfEnd.Clear();
            OnFirstHalf.Clear();
            OnFirstHalfEnd.Clear();
            OnPreHalfTime.Clear();
            OnPreHalfTimeEnd.Clear();
            OnRoundPrestart.Clear();
            OnSecondHalfPrestart.Clear();
            OnFirstHalfPrestart.Clear();
            OnPreHalfTimePrestart.Clear();
        }
    }
}
