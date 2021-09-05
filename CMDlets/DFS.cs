using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Set delegation to fully manage Distributed File System (DFS)</para>
    /// <para type="description">Configures the configuration container to delegate the permissions to a group so it can fully manage Distributed File System (DFS).</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclFullControlDFS "SL_DfsRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclFullControlDFS -Group "SL_DfsRight" </code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclFullControlDFS -Group "SL_DfsRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to fully manage Distributed File System (DFS)</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclFullControlDFS", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class FullControlDFS : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="inputType">STRING representing the group SamAccountName.</para>
        /// <para type="description">Identity of the group getting the delegation.</para>
        /// </summary>
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Identity of the group getting the delegation."
            )]
        [ValidateNotNullOrEmpty]
        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        private string _group;

        /// <summary>
        /// <para type="inputType">SWITCH parameter (true or false). If present the value becomes TRUE, and the access rule will be removed</para>
        /// <para type="description">Switch indicator to remove the access rule</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Switch indicator. If present (TRUE), the access rule will be removed."
            )]
        public SwitchParameter RemoveRule
        {
            get { return _removerule; }
            set { _removerule = value; }
        }

        private bool _removerule;

        #endregion Parameters definition

        #region Begin()

        /// <summary>
        ///
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (this.MyInvocation.BoundParameters.ContainsKey("Verbose"))
            {
                WriteVerbose("|=> ************************************************************************ <=|");
                WriteVerbose(DateTime.Today.ToShortDateString());
                WriteVerbose(string.Format("  Starting: {0}", this.MyInvocation.MyCommand));

                string paramVerbose;
                paramVerbose = "Parameters:\n";
                paramVerbose += string.Format("{0,-12}{1,-30}{2,-30}\n", null, " Key", " Value");
                paramVerbose += string.Format("{0,-12}{1,-30}{2,-30}\n", null, "----------", "----------");

                // display PSBoundparameters formatted nicely for Verbose output
                // var is iDictionary
                var pb = this.MyInvocation.BoundParameters; // | Format - Table - AutoSize | Out - String).TrimEnd()

                foreach (var item in pb)
                {
                    paramVerbose += string.Format("{0,-12}{1,-30}{2,-30}\n", null, item.Key, item.Value);
                }
                WriteVerbose(string.Format("{0}\n", paramVerbose));
            }
        }

        #endregion Begin()

        #region Process()

        /// <summary>
        ///
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            // Integer to catch contructor results
            int valueFromConstructor = 0;

            /*

             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=DFSR-GlobalSettings,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.GenericAll,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Dfs-Configuration,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.GenericAll,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate DFS Full Control completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating DFS Full Control");
            }
        }

        #endregion Process()

        #region End()

        /// <summary>
        ///
        /// </summary>
        protected override void EndProcessing()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("Verbose"))
            {
                string paramVerboseEnd;
                paramVerboseEnd = string.Format("\n         Function {0} finished.\n", this.MyInvocation.MyCommand);
                paramVerboseEnd += "-------------------------------------------------------------------------------\n\n";

                WriteVerbose(paramVerboseEnd);
            }
        }

        #endregion End()

        /// <summary>
        ///
        /// </summary>
        protected override void StopProcessing()
        {
        }
    } //END class FullControlDFS
} // END namespace EguibarIT.Delegation.DFS