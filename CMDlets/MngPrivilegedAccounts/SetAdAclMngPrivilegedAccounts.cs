using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Managed Privileged Accounts</para>
    /// <para type="description">The function will delegate the premission for a group to Managed Privileged Accounts</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclMngPrivilegedAccounts "SL_PUM"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclMngPrivilegedAccounts -Group "SL_PUM"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclMngPrivilegedAccounts -Group "SL_PUM" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on AdminSDHolder container to Manage Privileged Accounts objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclMngPrivilegedAccounts", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class MngPrivilegedAccounts : PSCmdlet
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
             <#
    dsacls "CN=AdminSDHolder,CN=System,DC=EguibarIT,DC=local" /G "EguibarIT\SL_PUM":RPWP;member

    dsacls "CN=AdminSDHolder,CN=System,DC=EguibarIT,DC=local" /G "EguibarIT\SL_PUM":CA;"Reset Password"
    dsacls "CN=AdminSDHolder,CN=System,DC=EguibarIT,DC=local" /G "EguibarIT\SL_PUM":RPWP;lockoutTime
    dsacls "CN=AdminSDHolder,CN=System,DC=EguibarIT,DC=local" /G "EguibarIT\SL_PUM":RPWP;pwdLastSet

    dsacls "CN=AdminSDHolder,CN=System,DC=EguibarIT,DC=local" /G "EguibarIT\SL_PUM":RPWP;userAccountControl
    dsacls "CN=AdminSDHolder,CN=System,DC=EguibarIT,DC=local" /G "EguibarIT\SL_PUM":CA;"Change Password"
    dsacls "CN=AdminSDHolder,CN=System,DC=EguibarIT,DC=local" /G "EguibarIT\SL_PUM":RPWP;lockoutTime
    #>
             */

            /*
             ACE number: 1
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : member [AttributeSchema]
                    InheritanceType : None
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : lockoutTime [AttributeSchema]
                    InheritanceType : None
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 3
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : userAccountControl [AttributeSchema]
                    InheritanceType : None
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 4
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : pwdLastSet [AttributeSchema]
                    InheritanceType : None
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 5
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ExtendedRight
                  AccessControlType : Allow
                         ObjectType : Reset Password [ExtendedRight]
                    InheritanceType : None
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 6
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ExtendedRight
                  AccessControlType : Allow
                         ObjectType : Change Password [ExtendedRight]
                    InheritanceType : None
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("member"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("member"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("lockoutTime"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("lockoutTime"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("userAccountControl"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("userAccountControl"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 4 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("pwdLastSet"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 4 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("pwdLastSet"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 5 ###> ExtendedRight <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Reset Password"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 6 ###> ExtendedRight <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=AdminSDHolder,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Change Password"),
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 10)
            {
                WriteVerbose("Delegate Management of Privileged Accounts Rights completed succesfully.");
            }

            if (valueFromConstructor != 10)
            {
                WriteWarning("Something went wrong when delegating Management of Privileged Accounts Rights");
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
    } //END class MngPrivilegedAccounts
} // END namespace EguibarIT.Delegation.PrivilegedAccounts