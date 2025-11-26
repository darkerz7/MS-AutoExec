using Microsoft.Extensions.Logging;
using Sharp.Shared.Enums;
using Sharp.Shared.Types;

namespace MS_AutoExec
{
    public class EventInfo
    {
        Guid? Timer;
        public bool ShowInConsole { get; set; }
        public bool EntryInLog { get; set; }
        public float Delay { get; set; }
        public List<string> Commands { get; set; }
        public EventInfo()
        {
            ShowInConsole = false;
            EntryInLog = false;
            Timer = null;
            Delay = 0.0f;
            Commands = [];
        }
        ~EventInfo()
        {
            KillTimer();
        }
        public void EventInfoHandler()
        {
            if (Delay <= 0.0f) ExecCommands();
            else CreateTimer();
        }
        void ExecCommands()
        {
            if (IsValid())
            {
                foreach (string sCommand in Commands)
                {
                    AutoExec._modSharp!.ServerCommand(sCommand);
                    if (ShowInConsole) AutoExec.PrintToConsole($"Exec: {sCommand}");
                    if (EntryInLog) AutoExec._logger!.LogInformation($"Exec: {sCommand}");
                }
            }
        }
        void CreateTimer()
        {
            KillTimer();
            Timer = AutoExec._modSharp!.PushTimer(() =>
            {
                ExecCommands();
                KillTimer();
            }, Delay, GameTimerFlags.StopOnMapEnd);
        }
        public void KillTimer()
        {
            if (Timer != null)
            {
                AutoExec._modSharp!.StopTimer((Guid)Timer);
                Timer = null;
            }
        }
        bool IsValid()
        {
            if (Commands != null && Commands.Count > 0) return true;
            return false;
        }
    }
}
