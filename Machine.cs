using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBOX.NET
{
    public class Machine
    {
        internal vboxService service;
        internal String auth;

        internal String Session
        {
            get
            {
                return service.IWebsessionManager_getSessionObject(auth);
            }
        }

        internal String Console
        {
            get
            {
                return service.ISession_getConsole(Session);
            }
        }

        internal Machine(VBox vbox, String auth)
        {
            this.service = vbox.service;
            this.auth = auth;
        }

        public String Name
        {
            get
            {
                return this.service.IMachine_getName(auth);
            }
        }

        public MachineState State
        {
            get
            {
                return this.service.IMachine_getState(auth);
            }
        }

        public void Start(StartMode mode)
        {
            this.Start(mode, new Dictionary<string, string>());
        }

        public void Start(StartMode mode, Dictionary<String, String> env)
        {
            String uuid = service.IMachine_getId(auth);
            String sessionType = mode.InternalValue();
            String environment = BuildEnvironment(env);

            String progress = service.IMachine_launchVMProcess(auth, Session, sessionType, environment);

            service.IProgress_waitForCompletion(progress, -1);
        }

        public void Stop()
        {
            String progress = service.IConsole_powerDown(Console);

            service.IProgress_waitForCompletion(progress, -1);
        }

        public void RestoreSnapshot(String name)
        {
            using (new MachineLock(this))
            {
                String snapshot = service.IMachine_findSnapshot(auth, name);

                String progress = service.IConsole_restoreSnapshot(Console, snapshot);

                service.IProgress_waitForCompletion(progress, -1);
            }
        }

        private String BuildEnvironment(Dictionary<String, String> env)
        {
            StringBuilder result = new StringBuilder();

            foreach (var item in env)
            {
                result.AppendFormat("{0}[={1}]\n", item.Key, item.Value);
            }

            return result.ToString();
        }
    }

    internal class MachineLock : IDisposable
    {
        private Machine machine;

        public MachineLock(Machine machine)
        {
            this.machine = machine;

            this.machine.service.IMachine_lockMachine(this.machine.auth, this.machine.Session, LockType.Write);
        }

        public void Dispose()
        {
            this.machine.service.ISession_unlockMachine(this.machine.Session);
        }
    }

    internal static class StartModeExt
    {
        internal static String InternalValue(this StartMode mode)
        {
            switch (mode)
            {
                case StartMode.QT_UI:
                    return "gui";
                case StartMode.SDL_UI:
                    return "sdl";
                case StartMode.NO_UI:
                    return "headless";
                case StartMode.DEFAULT:
                    return "";
                default:
                    return "";
            }
        }
    }

    public enum StartMode
    {
        QT_UI,
        SDL_UI,
        NO_UI,
        DEFAULT
    }
}
