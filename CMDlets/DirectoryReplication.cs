using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Sets the premission for a group to Replicate the directory</para>
    /// <para type="description">Configures the configuration container to delegate the permissions to a group so it can replicate the directory</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdDirectoryReplication "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdDirectoryReplication -Group "SL_InfraRight" </code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdDirectoryReplication -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to create/delete Site objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdDirectoryReplication", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class AdDirectoryReplication : PSCmdlet
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

            #region Configuration Naming Context delegation

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Manage Replication Topology"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes All"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 4
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replication Synchronization"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 5
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Monitor active directory Replication"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number:
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes In Filtered Set"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            #endregion Configuration Naming Context delegation

            #region Schema Naming Context delegation

            // ACE number: 6
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetschemaNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Manage Replication Topology"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 7
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetschemaNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 8
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetschemaNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes All"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 9
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetschemaNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replication Synchronization"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            #endregion Schema Naming Context delegation

            #region Forest DNS Zones Naming Context delegation

            // ACE number: 10
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("DC=ForestDnsZones,{0}", EguibarIT.Delegation.Other.Domains.GetrootDomainNamingContext()),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Manage Replication Topology"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 11
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("DC=ForestDnsZones,{0}", EguibarIT.Delegation.Other.Domains.GetrootDomainNamingContext()),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 12
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("DC=ForestDnsZones,{0}", EguibarIT.Delegation.Other.Domains.GetrootDomainNamingContext()),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes All"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 13
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("DC=ForestDnsZones,{0}", EguibarIT.Delegation.Other.Domains.GetrootDomainNamingContext()),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replication Synchronization"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            #endregion Forest DNS Zones Naming Context delegation

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 13)
            {
                WriteVerbose("Delegate Directory Replication Rights completed succesfully.");
            }

            if (valueFromConstructor != 13)
            {
                WriteWarning("Something went wrong when delegating Directory Replication Rights ");
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
    } //END class AdDirectoryReplication
} // END namespace EguibarIT.Delegation.DirectoryReplication