using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Set delegation to create/delete Site objects</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can create/delete Site objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclCreateDeleteSite "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclCreateDeleteSite -Group "SL_InfraRight" </code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclCreateDeleteSite -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to create/delete Site objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclCreateDeleteSite", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class CreateDeleteSite : PSCmdlet
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
             ACE number: 1
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : CreateChild, DeleteChild
                  AccessControlType : Allow
                         ObjectType : GuidNULL
                    InheritanceType : Descendents
                InheritedObjectType : site [ClassSchema]
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : CreateChild, DeleteChild
                  AccessControlType : Allow
                         ObjectType : site [ClassSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 3
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : CreateChild, DeleteChild
                  AccessControlType : Allow
                         ObjectType : nTDSSiteSettings [ClassSchema]
                    InheritanceType : Descendents
                InheritedObjectType : site [ClassSchema]
                        IsInherited = False
             */

            // ACE number: 1 ###> CreateChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    string.Format("CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("site"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> DeleteChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    string.Format("CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("site"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> CreateChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("site"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> DeleteChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("site"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3 ###> CreateChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    string.Format("CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("nTDSSiteSettings"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("site"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3 ###> DeleteChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    string.Format("CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("nTDSSiteSettings"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("site"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 6)
            {
                WriteVerbose("Delegate Create/Delete Site object class completed succesfully.");
            }

            if (valueFromConstructor != 6)
            {
                WriteWarning("Something went wrong when delegating Create/Delete Site object class");
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
    } //END class CreateDeleteSite

    /// <summary>
    /// <para type="synopsis">Set delegation to Modify Site objects</para>
    /// <para type="description">Configures the container (configuration) to delegate the permissions to a group so it can Modify Site objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclChangeSite "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclChangeSite -Group "SL_InfraRight" </code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclChangeSite -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify Site objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclChangeSite", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ChangeSite : PSCmdlet
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
             ACE number: 1
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty, WriteProperty
                  AccessControlType : Allow
                         ObjectType : site [ClassSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("site"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("site"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 6)
            {
                WriteVerbose("Delegate change Site object class completed succesfully.");
            }

            if (valueFromConstructor != 6)
            {
                WriteWarning("Something went wrong when delegating change Site object class");
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
    } //END class ChangeSite
} // END namespace EguibarIT.Delegation.Site