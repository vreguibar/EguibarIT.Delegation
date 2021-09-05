using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Set delegation to create/delete Subnet objects</para>
    /// <para type="description">Configures the container (OU) to delegate the permissions to a group so it can create/delete Subnet objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclCreateDeleteSubnet "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclCreateDeleteSubnet -Group "SL_InfraRight" </code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclCreateDeleteSubnet -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to create/delete Subnet objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclCreateDeleteSubnet", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class CreateDeleteSubnet : PSCmdlet
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
                InheritedObjectType : subnet [ClassSchema]
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : CreateChild, DeleteChild
                  AccessControlType : Allow
                         ObjectType : subnet [ClassSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1 ###> CreateChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    string.Format("CN=Subnets,CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("subnet"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> DeleteChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    string.Format("CN=Subnets,CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("subnet"),
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> CreateChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Subnets,CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("subnet"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> DeleteChild <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Subnets,CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("subnet"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 4)
            {
                WriteVerbose("Delegate Create/Delete Subnet object class completed succesfully.");
            }

            if (valueFromConstructor != 4)
            {
                WriteWarning("Something went wrong when delegating Create/Delete Subnet object class");
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
    } //END class CreateDeleteSubnet

    /// <summary>
    /// <para type="synopsis">Set delegation to Modify Subnet objects</para>
    /// <para type="description">Configures the container (configuration) to delegate the permissions to a group so it can Modify Subnet objects.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclChangeSubnet "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclChangeSubnet -Group "SL_InfraRight" </code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclChangeSubnet -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to Modify Subnet objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclChangeSubnet", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class ChangeSubnet : PSCmdlet
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
                         ObjectType : subnet [ClassSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1 ###> ReadProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Subnets,CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("subnet"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 1 ###> WriteProperty <###
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Subnets,CN=Sites,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("subnet"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate change Subnet object class completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating change Subnet object class");
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
    } //END class ChangeSubnet
} // END namespace EguibarIT.Delegation.Subnet