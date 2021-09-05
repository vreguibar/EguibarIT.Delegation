using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Set delegation to create/delete computer</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can create/delete computer objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclCreateDeleteComputer "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclCreateDeleteComputer -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclCreateDeleteComputer -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to create/delete computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclCreateDeleteComputer", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class CreateDeleteComputer : PSCmdlet
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
             ActiveDirectoryRightst : GenericAll
                  AccessControlType : Allow
                         ObjectType : computer [ClassSchema]
                    InheritanceType : Descendents
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : CreateChild, DeleteChild
                  AccessControlType : Allow
                         ObjectType : computer [ClassSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1
            if (
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.GenericAll,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    System.Guid.Empty,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> CreateChild <###
            if (
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> DeleteChild <###
            if (
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 3)
            {
                WriteVerbose("Delegate Create/Delete Computer object class completed succesfully.");
            }

            if (valueFromConstructor != 3)
            {
                WriteWarning("Something went wrong when delegating Create/Delete Computer object class");
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
    } //END class CreateDeleteComputer

    /// <summary>
    /// <para type="synopsis">Set delegation to rename computer</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can rename computer objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Rename-AdAclCreateDeleteComputer "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Rename-AdAclComputer -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Rename-AdAclComputer -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to rename computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Rename, "AdAclComputer", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class RenameComputer : PSCmdlet
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
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : cn
                    InheritanceType : All
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

            ACE number: 3
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : name
                    InheritanceType : All
                InheritedObjectType : computer [ClassSchema]
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
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
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
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
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
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
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
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
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
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
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
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 6)
            {
                WriteVerbose("Delegate Rename Computer object completed succesfully.");
            }

            if (valueFromConstructor != 6)
            {
                WriteWarning("Something went wrong when delegating rename Computer object.");
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
    } //END class RenameComputer

    /// <summary>
    /// <para type="synopsis">Set delegation to enable/disable computer</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can enable/disable computer objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclEnableDisableComputer "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclEnableDisableComputer -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclEnableDisableComputer -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to enable/disable computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclEnableDisableComputer", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class EnableDisableComputer : PSCmdlet
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
             ActiveDirectoryRightst : WriteProperty
                  AccessControlType : Allow
                         ObjectType : userAccountControl
                    InheritanceType : All
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty
                  AccessControlType : Allow
                         ObjectType : userAccountControl
                    InheritanceType : All
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("userAccountControl"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("userAccountControl"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate enable/disable Computer object class completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating enable/disable Computer object class");
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
    } //END class enabledisableComputer

    /// <summary>
    /// <para type="synopsis">Set delegation to change computer group membership</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can change computer object group membership.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclComputerGroupMembership "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclComputerGroupMembership -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclComputerGroupMembership -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to change computer object group membership</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclComputerGroupMembership", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ComputerGroupMembership : PSCmdlet
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
                         ObjectType : memberOf
                    InheritanceType : All
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ### WriteProperty ###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("memberOf"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
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
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("memberOf"),
                    ActiveDirectorySecurityInheritance.All,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate change Computer group membership completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating change Computer group membership");
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
    } //END class ComputerGroupMembership

    /// <summary>
    /// <para type="synopsis">Set delegation to change computer password</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can change password on computer objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclChangeComputerPassword "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclChangeComputerPassword -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclChangeComputerPassword -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to change password on computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclChangeComputerPassword", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ChangeComputerPassword : PSCmdlet
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
             ACE number: 11
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ExtendedRight
                  AccessControlType : Allow
                         ObjectType : Change Password [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Change Password"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 1)
            {
                WriteVerbose("Delegate change Computer object password completed succesfully.");
            }

            if (valueFromConstructor != 1)
            {
                WriteWarning("Something went wrong when delegating change Computer object password");
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
    } //END class ChangeComputerPassword

    /// <summary>
    /// <para type="synopsis">Set delegation to reset computer password</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can reset password on computer objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclResetComputerPassword "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclResetComputerPassword -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclResetComputerPassword -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to reset password on computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclResetComputerPassword", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ResetComputerPassword : PSCmdlet
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
             ACE number: 11
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ExtendedRight
                  AccessControlType : Allow
                         ObjectType : Reset Password [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Reset Password"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 1)
            {
                WriteVerbose("Delegate reset Computer object password completed succesfully.");
            }

            if (valueFromConstructor != 1)
            {
                WriteWarning("Something went wrong when delegating reset Computer object password");
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
    } //END class ResetComputerPassword

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Modify Computer Account Restriction Set of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to Modify Computer Account Restriction Set of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclComputerAccountRestriction "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclComputerAccountRestriction -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclComputerAccountRestriction -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify Computer Account Restriction Set of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclComputerAccountRestriction", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ComputerAccountRestriction : PSCmdlet
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
                         ObjectType : Account Restrictions [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Account Restrictions"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> WriteProperty <###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Account Restrictions"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate modify Computer object account restrictions completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating modify Computer object account restrictions");
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
    } //END class ComputerAccountRestriction

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Modify Public Information Set of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to Modify Public Information Set of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclComputerPublicInfo "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclComputerPublicInfo -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclComputerPublicInfo -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify Public Information Set of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclComputerPublicInfo", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ComputerPublicInfo : PSCmdlet
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
                         ObjectType : Public Information [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Public Information"),
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
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Public Information"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate modify Computer object public information set completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating modify Computer object public information set");
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
    } //END class ComputerPublicInfo

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Modify Personal Information Set of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to Modify Personal Information Set of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclComputerPersonalInfo "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclComputerPersonalInfo -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclComputerPersonalInfo -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify Personal Information Set of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclComputerPersonalInfo", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ComputerPersonalInfo : PSCmdlet
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
                         ObjectType : Personal Information [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Personal Information"),
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
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Personal Information"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate modify Computer object personal information set completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating modify Computer object personal information set");
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
    } //END class ComputerPersonalInfo

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Modify DNS Information Set of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to Modify DNS Information Set of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclDnsInfo "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclDnsInfo -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclDnsInfo -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify DNS Information Set of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclDnsInfo", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class DnsInfo : PSCmdlet
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
                         ObjectType : DNS Host Name Attributes [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("DNS Host Name Attributes"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> WriteProperty <###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("DNS Host Name Attributes"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate modify Computer object DNS information set completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating modify Computer object DNS information set");
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
    } //END class DnsInfo

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Modify MS TerminalServices Gateway Information Set of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to Modify MS TerminalServices Gateway Information Set of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclMsTsGatewayInfo "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclMsTsGatewayInfo -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclMsTsGatewayInfo -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify MS TerminalServices Gateway Information Set of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclMsTsGatewayInfo", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class MsTsGatewayInfo : PSCmdlet
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
                         ObjectType : MS-TS-GatewayAccess [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("MS-TS-GatewayAccess"),
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
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("MS-TS-GatewayAccess"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate modify Computer object MS-TS Gateway information set completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating modify Computer object MS-TS Gateway information set");
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
    } //END class MsTsGatewayInfo

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Modify the extended right to validate DNS Host Name of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to Modify the extended right to validate DNS Host Name of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclValidateWriteDnsHostName "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclValidateWriteDnsHostName -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclValidateWriteDnsHostName -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify the extended right to validate DNS Host Name of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclValidateWriteDnsHostName", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ValidateWriteDnsHostName : PSCmdlet
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
             ActiveDirectoryRightst : Self
                  AccessControlType : Allow
                         ObjectType : dNSHostName [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.Self,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("dNSHostName"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 1)
            {
                WriteVerbose("Delegate modify Computer object validate write DNS hostname completed succesfully.");
            }

            if (valueFromConstructor != 1)
            {
                WriteWarning("Something went wrong when delegating modify Computer object validate write DNS hostname");
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
    } //END class ValidateWriteDnsHostName

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Modify the the extended right to validate Service Principal Name of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to Modify the the extended right to validate Service Principal Name of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclValidateWriteSPN "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclValidateWriteSPN -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclValidateWriteSPN -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify the the extended right to validate Service Principal Name of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclValidateWriteSPN", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ValidateWriteSPN : PSCmdlet
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
             ActiveDirectoryRightst : Self
                  AccessControlType : Allow
                         ObjectType : servicePrincipalName [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.Self,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("servicePrincipalName"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 1)
            {
                WriteVerbose("Delegate modify Computer object validate write SPN completed succesfully.");
            }

            if (valueFromConstructor != 1)
            {
                WriteWarning("Something went wrong when delegating modify Computer object validate write SPN");
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
    } //END class ValidateWriteSPN

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to Modify BitLocker and TPM information of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to Modify BitLocker and TPM information of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclBitLockerTPM "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclBitLockerTPM -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclBitLockerTPM -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify BitLocker and TPM information of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclBitLockerTPM", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class BitLockerTPM : PSCmdlet
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
             *

            Missing ACE ObjectType
            --------------------------------------------------------
                  IdentityReference : SELF
             ActiveDirectoryRightst : WriteProperty
                  AccessControlType : Allow
                         ObjectType : msTPM-TpmInformationForComputer [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

            Missing ACE ObjectType
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty
                  AccessControlType : Allow
                         ObjectType : msTPM-TpmInformationForComputer [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

             * ACE number: 1
            --------------------------------------------------------
                  IdentityReference : SELF
             ActiveDirectoryRightst : WriteProperty
                  AccessControlType : Allow
                         ObjectType : msTPM-OwnerInformation [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty
                  AccessControlType : Allow
                         ObjectType : msTPM-OwnerInformation [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

             ACE number: 5
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : GenericAll
             AccessControlType      : Allow
             ObjectType             : GuidNULL
             InheritanceType        : Descendents
             InheritedObjectType    : msFVE-RecoveryInformation [ClassSchema]
             IsInherited            : False
             */

            // MISSING ACE number: 1 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "SELF",
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("msTPM-TpmInformationForComputer"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // MISSINGACE number: 2 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("msTPM-TpmInformationForComputer"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> WriteProperty <###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "SELF",
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("msTPM-OwnerInformation"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> ReadProperty <###
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("msTPM-OwnerInformation"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 5 ###> GenericAll <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.GenericAll,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("msFVE-RecoveryInformation"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 5)
            {
                WriteVerbose("Delegate modify Computer object Bitlocker TPM completed succesfully.");
            }

            if (valueFromConstructor != 5)
            {
                WriteWarning("Something went wrong when delegating modify Computer object Bitlocker TPM");
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
    } //END class BitLockerTPM

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

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to read Admin password of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to read Admin password of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdmPwdReadPasswordPermission "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdmPwdReadPasswordPermission -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdmPwdReadPasswordPermission -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to read Admin password of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdmPwdReadPasswordPermission", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class AdmPwdReadPasswordPermission : PSCmdlet
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
             ActiveDirectoryRightst : ReadProperty, ExtendedRight
                  AccessControlType : Allow
                         ObjectType : ms-Mcs-AdmPwd [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty
                  AccessControlType : Allow
                         ObjectType : ms-Mcs-AdmPwdExpirationTime [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("ms-Mcs-AdmPwd"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> ExtendedRight <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("ms-Mcs-AdmPwd"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
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

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 3)
            {
                WriteVerbose("Delegate modify Computer object read LAPS password completed succesfully.");
            }

            if (valueFromConstructor != 3)
            {
                WriteWarning("Something went wrong when delegating modify Computer object read LAPS password");
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
    } //END class AdmPwdReadPasswordPermission

    /// <summary>
    /// <para type="synopsis">The function will delegate the premission for a group to reset Admin password of Computer object</para>
    /// <para type="description">The function will delegate the premission for a group to reset Admin password of Computer object</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdmPwdResetPasswordPermission "SL_PcRight" "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdmPwdResetPasswordPermission -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdmPwdResetPasswordPermission -Group "SL_PcRight" -LDAPpath "OU=Desktops,OU=XXXX,OU=Sites,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to reset Admin password of Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdmPwdResetPasswordPermission", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class AdmPwdResetPasswordPermission : PSCmdlet
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
                         ObjectType : ms-Mcs-AdmPwdExpirationTime [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
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

            // ACE number: 1 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
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

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate modify Computer object reset LAPS password completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating modify Computer object reset LAPS password");
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
    } //END class AdmPwdResetPasswordPermission

    /// <summary>
    /// <para type="synopsis">The function will delegate premission for a group to Only Delete Computer objects from given container</para>
    /// <para type="description">The function will delegate premission for a group to Only Delete Computer objects from given container</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-DeleteOnlyComputer "SL_PcRight" "OU=X-Computers,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-DeleteOnlyComputer -Group "SL_PcRight" -LDAPpath "OU=X-Computers,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-DeleteOnlyComputer -Group "SL_PcRight" -LDAPpath "OU=X-Computers,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to delete only Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "DeleteOnlyComputer", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class DeleteOnlyComputer : PSCmdlet
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
             ActiveDirectoryRightst : WriteProperty
                  AccessControlType : Allow
                         ObjectType : GuidNULL
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : DeleteChild
                  AccessControlType : Allow
                         ObjectType : computer [ClassSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate modify Computer object delete only completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating modify Computer object delete only");
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
    } //END class DeleteOnlyComputer

    /// <summary>
    /// <para type="synopsis">The function will delegate premission for a group to Domain-Join Computer objects from given container</para>
    /// <para type="description">The function will delegate premission for a group to Domain-Join Computer objects from given container</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-DomainJoinComputer "SL_PcRight" "OU=X-Computers,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-DomainJoinComputer -Group "SL_PcRight" -LDAPpath "OU=X-Computers,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-DomainJoinComputer -Group "SL_PcRight" -LDAPpath "OU=X-Computers,DC=EguibarIT,DC=local" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Domain-Join Computer objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "DomainJoinComputer", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class DomainJoinComputer : PSCmdlet
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
             ActiveDirectoryRightst : CreateChild, DeleteChild
                  AccessControlType : Allow
                         ObjectType : computer [ClassSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False

             ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ExtendedRight
                  AccessControlType : Allow
                         ObjectType : Reset Password [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

             ACE number: 3
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : Account Restrictions [ExtendedRight]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

             ACE number: 4
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : Self
                  AccessControlType : Allow
                         ObjectType : dNSHostName [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False

             ACE number: 5
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : Self
                  AccessControlType : Allow
                         ObjectType : servicePrincipalName [AttributeSchema]
                    InheritanceType : Descendents
                InheritedObjectType : computer [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> CreateChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> DeleteChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Reset Password"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Account Restrictions"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Account Restrictions"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 4
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.Self,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("dNSHostName"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 5
            if (EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    _group,
                    ActiveDirectoryRights.Self,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("servicePrincipalName"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("computer"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 7)
            {
                WriteVerbose("Delegate modify Computer object Domain Join completed succesfully.");
            }

            if (valueFromConstructor != 7)
            {
                WriteWarning("Something went wrong when delegating modify Computer object Domain Join");
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
    } //END class DeleteOnlyComputer
} // END namespace EguibarIT.Delegation.Computer