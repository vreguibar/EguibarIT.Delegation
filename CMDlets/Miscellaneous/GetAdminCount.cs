using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Management.Automation;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis"></para>
    /// <para type="description"></para>
    /// <para type="description"></para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AdAdminCount", ConfirmImpact = ConfirmImpact.Medium)]
    class GetAdAdminCountCmdlet : Cmdlet
    {

        // https://msdn.microsoft.com/en-us/library/dd878345(v=vs.85).aspx
        /// <summary>
        /// <para type="description"></para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromRemainingArguments = false,
            HelpMessage = "Full path to the configuration.ini file",
            Position = 0
        )]
        public string LDAPpath
        {
            get;
            set;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose(String.Format("Get all users within LDAPPath {0} which have AdminCount attribute set to 1", LDAPpath));
        }

        protected override void ProcessRecord()
        {

        }

        protected override void EndProcessing()
        {

        }

        protected override void StopProcessing()
        {

        }

    }
}
