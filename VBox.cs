using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBOX.NET
{
    public partial class VBox
    {
        internal vboxService service;
        private String auth;

        public static readonly int STANDARD_PORT = 18083;

        public VBox(String url, int port)
            : this(url, port, "", "")
        {
        }

        public VBox(String url, int port, String login, string password)
        {
            var uri = new UriBuilder(url) 
            {
                Port = port 
            };

            this.service = new vboxService(uri.ToString());
            this.auth = this.service.IWebsessionManager_logon(login, password);
        }

        public String GetVersion()
        {
            return this.service.IVirtualBox_getVersion(auth);
        }

        public IEnumerable<Machine> GetMachines()
        {
            var machines = this.service.IVirtualBox_getMachines(auth);

            foreach (var machine in machines)
            {
                yield return new Machine(this, machine);
            }
        }
    }
}
