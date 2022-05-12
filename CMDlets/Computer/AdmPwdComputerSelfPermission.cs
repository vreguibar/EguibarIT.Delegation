using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">The function will delegate the right were the computer can self update admin password attributes</para>
    /// <para type="description">The function will delegate the right were the computer can self update admin password attributes</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdmPwdComputerSelfPermission "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdmPwdComputerSelfPermission -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdmPwdComputerSelfPermission -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>The function will delegate the right were the computer can self update admin password attributes</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdmPwdComputerSelfPermission", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class AdmPwdComputerSelfPermission : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="inputType">STRING representing the Distinguished Name of the object (ej. OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local).</para>
        /// <para type="description">Distinguished Name of the object (or container) where the permissions are going to be configured.</para>
        /// </summary>
        [Parameter(
               Position = 0,
               Mandatory = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Distinguished Name of the object (or container) where the permissions are going to be configured."
            )]
        [ValidateNotNullOrEmpty]
        public string LDAPpath
        {
            get { return _ldappath; }
            set { _ldappath = value; }
        }

        private string _ldappath;

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
             ACE number: 1
            --------------------------------------------------------
                  IdentityReference : SELF
             ActiveDirectoryRightst : WriteProperty
                  AccessControlType : Allow
                         ObjectType : ms-Mcs-AdmPwd [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : SELF
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : ms-Mcs-AdmPwdExpirationTime [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 2 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "SELF",
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("ms-Mcs-AdmPwdExpirationTime"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> WriteProperty <###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "SELF",
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("ms-Mcs-AdmPwdExpirationTime"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "SELF",
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("ms-Mcs-AdmPwd"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 3)
            {
                WriteVerbose("Delegate modify Computer object LAPS password update completed succesfully.");
            }

            if (valueFromConstructor != 3)
            {
                WriteWarning("Something went wrong when delegating modify Computer object LAPS password update");
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
    } //END class AdmPwdComputerSelfPermission
}