using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Set delegation to Rename OU</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can Rename organizational unit objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Rename-AdAclOU "SL_ADRight" "OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Rename-AdAclOU -Group "SL_ADRight" -LDAPpath "OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Rename-AdAclOU -Group "SL_ADRight" -LDAPpath "OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Rename OU objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Rename, "AdAclOU", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class RenameOU : PSCmdlet
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
        /// <para type="inputType">STRING representing the Distinguished Name of the object (ej. OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local).</para>
        /// <para type="description">Distinguished Name of the object (or container) where the permissions are going to be configured.</para>
        /// </summary>
        [Parameter(
               Position = 1,
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
             ACE number: 1
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : distinguishedName
                    InheritanceType : All
                InheritedObjectType : organizationalUnit [ClassSchema]
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : cn
                    InheritanceType : All
                InheritedObjectType : organizationalUnit [ClassSchema]
                        IsInherited = False

            ACE number: 3
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : name
                    InheritanceType : All
                InheritedObjectType : organizationalUnit [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ### WriteProperty ###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("distinguishedName"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("organizationalUnit"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ### ReadProperty ###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("distinguishedName"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("organizationalUnit"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ### WriteProperty ###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("cn"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("organizationalUnit"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ### ReadProperty ###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("cn"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("organizationalUnit"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3 ### WriteProperty ###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("name"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("organizationalUnit"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3 ### ReadProperty ###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("name"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("organizationalUnit"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 6)
            {
                WriteVerbose("Delegate rename OU object class completed succesfully.");
            }

            if (valueFromConstructor != 6)
            {
                WriteWarning("Something went wrong when delegating rename OU object class");
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
    } //END class RenameOU
}
