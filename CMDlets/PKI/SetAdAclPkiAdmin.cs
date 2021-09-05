using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Set delegation to fully manage Certificate Authority (CA or PKI)</para>
    /// <para type="description">Configures the configuration container to delegate the permissions to a group so it can fully manage Certificate Authority (CA or PKI).</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclPkiAdmin "SL_PkiRight"</code>
    /// </example>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclPkiAdmin -Group "SL_PkiRight" </code>
    /// </example>
    /// <example>
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclPkiAdmin -Group "SL_PkiRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to fully manage Certificate Authority (CA or PKI)</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclPkiAdmin", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class PkiAdmin : PSCmdlet
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
        /// <para type="inputType">STRING representing the Distinguished Name of the object (ej. OU=Rights,OU=Admin,DC=EguibarIT,DC=local).</para>
        /// <para type="description">Distinguished Name of the OU having the Rights groups (Usually OU=Rights,OU=Admin,DC=EguibarIT,DC=local).</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Distinguished Name of the OU having the Rights groups (Usually OU=Rights,OU=Admin,DC=EguibarIT,DC=local)."
            )]
        [ValidateNotNullOrEmpty]
        public string ItRightsOuDN
        {
            get { return _itrightsoudn; }
            set { _itrightsoudn = value; }
        }

        private string _itrightsoudn;

        /// <summary>
        /// <para type="inputType">SWITCH parameter (true or false). If present the value becomes TRUE, and the access rule will be removed</para>
        /// <para type="description">Switch indicator to remove the access rule</para>
        /// </summary>
        [Parameter(
               Position = 2,
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
                    string.Format("CN=Public Key Services,CN=Services,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
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

            #region rights to modify security permissions for Pre-Windows 2000 Compatible Access group.

            // ACE number: 2
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Pre-Windows 2000 Compatible Access,CN=Builtin,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.ListChildren,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Pre-Windows 2000 Compatible Access,CN=Builtin,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 4
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Pre-Windows 2000 Compatible Access,CN=Builtin,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.GenericWrite,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            #endregion rights to modify security permissions for Pre-Windows 2000 Compatible Access group.

            #region rights to modify security permissions for Cert Publishers group.

            // ACE number: 5
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Cert Publishers,{0}", _itrightsoudn),
                    _group,
                    ActiveDirectoryRights.ListChildren,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Cert Publishers,{0}", _itrightsoudn),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 4
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Cert Publishers,{0}", _itrightsoudn),
                    _group,
                    ActiveDirectoryRights.GenericWrite,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            #endregion rights to modify security permissions for Cert Publishers group.

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 7)
            {
                WriteVerbose("Delegate PKI Administration completed succesfully.");
            }

            if (valueFromConstructor != 7)
            {
                WriteWarning("Something went wrong when delegating PKI Administration");
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
    } //END class PkiAdmin
} // END namespace EguibarIT.Delegation.PKI