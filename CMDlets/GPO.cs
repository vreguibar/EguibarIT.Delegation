using EguibarIT.Delegation.Other;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Set delegation to create/delete Group Policy Objects</para>
    /// <para type="description">The function will delegate the premission for a group to create/Delete Group Policy Objects objects within the Group Policy Container</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclCreateDeleteOU "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclCreateDeleteOU -Group "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclCreateDeleteOU -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on the Group Policy Container to create/delete Group Policy Objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclCreateDeleteGPO", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class CreateDeleteGPO : PSCmdlet
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
             ActiveDirectoryRightst : CreateChild
                  AccessControlType : Allow
                         ObjectType : GuidNULL
                    InheritanceType : None
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Policies,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 1)
            {
                WriteVerbose("Delegate Create/Delete GPO class completed succesfully.");
            }

            if (valueFromConstructor != 1)
            {
                WriteWarning("Something went wrong when delegating Create/Delete GPO class");
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
    } //END class CreateDeleteGPO

    /// <summary>
    /// <para type="synopsis">Set delegation to Link Group Policy Objects</para>
    /// <para type="description">The function will delegate the premission for a group to Link Group Policy Objects objects to Domain or OU</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclLinkGPO "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclLinkGPO -Group "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclLinkGPO -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on to Link Group Policy Objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclLinkGPO", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class LinkGPO : PSCmdlet
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
             ActiveDirectoryRightst : WriteProperty
                  AccessControlType : Allow
                         ObjectType : gPLink [AttributeSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : ReadProperty
                  AccessControlType : Allow
                         ObjectType : gPLink [AttributeSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("gPLink"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("gPLink"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate link GPO completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating link GPO");
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
    } //END class LinkGPO

    /// <summary>
    /// <para type="synopsis">Set delegation to gPOptions</para>
    /// <para type="description">The function will delegate the premission for a group to gPOptions</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclGPoption "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclGPoption -Group "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclGPoption -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on to gPOptions</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclGPoption", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class GPoption : PSCmdlet
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
             ActiveDirectoryRightst : ReadProperty
                  AccessControlType : Allow
                         ObjectType : gPOptions [AttributeSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False

            ACE number: 2
            --------------------------------------------------------
                  IdentityReference : XXX
             ActiveDirectoryRightst : WriteProperty
                  AccessControlType : Allow
                         ObjectType : gPOptions [AttributeSchema]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("gPOptions"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("gPOptions"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Delegate Change GpOption completed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something went wrong when delegating change GpOption");
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
    } //END class GPoption

    /// <summary>
    /// <para type="synopsis">Set delegation to Resultant Set of Policy (Logging)</para>
    /// <para type="description">The function will delegate the premission for a group to Resultant Set of Policy (Logging)</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclRSoPLogging "SL_GpoRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclRSoPLogging -Group "SL_GpoRight"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclRSoPLogging -Group "SL_GpoRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on to Resultant Set of Policy (Logging)</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclRSoPLogging", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class RSoPLogging : PSCmdlet
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
             ActiveDirectoryRightst : ExtendedRight
                  AccessControlType : Allow
                         ObjectType : Generate Resultant Set of Policy (Logging) [ExtendedRight]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Generate Resultant Set of Policy (Logging)"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 1)
            {
                WriteVerbose("Delegate RDoP Logging completed succesfully.");
            }

            if (valueFromConstructor != 1)
            {
                WriteWarning("Something went wrong when delegating RDoP logging");
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
    } //END class RSoPLogging

    /// <summary>
    /// <para type="synopsis">Set delegation to Resultant Set of Policy (Planning)</para>
    /// <para type="description">The function will delegate the premission for a group to Resultant Set of Policy (Planning)</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdAclRSoPPlanning "SL_GpoRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdAclRSoPPlanning -Group "SL_GpoRight"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdAclRSoPPlanning -Group "SL_GpoRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on to Resultant Set of Policy (Planning)</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclRSoPPlanning", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class RSoPPlanning : PSCmdlet
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
             ActiveDirectoryRightst : ExtendedRight
                  AccessControlType : Allow
                         ObjectType : Generate Resultant Set of Policy (Planning) [ExtendedRight]
                    InheritanceType : All
                InheritedObjectType : GuidNULL
                        IsInherited = False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Generate Resultant Set of Policy (Planning)"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 1)
            {
                WriteVerbose("Delegate RDoP Planning completed succesfully.");
            }

            if (valueFromConstructor != 1)
            {
                WriteWarning("Something went wrong when delegating RDoP Planning");
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
    } //END class RSoPPlanning

    /// <summary>
    /// <para type="synopsis">Set the Privileged Rights into a Group Policy Objects (MUST be executed on DomainController)</para>
    /// <para type="description">The function will modify the Privileged Rights into a Group Policy Object based on the Delegation Model with Tiers</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-GpoPrivilegeRights "Default Domain" "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-GpoPrivilegeRights -GpoToModify "Default Domain" -NetworkLogon "SL_InfraRight"</code>
    ///
    /// </example>
    /// <remarks>Set the Privileged Rights into a Group Policy Objects (MUST be executed on DomainController)</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "GpoPrivilegeRights", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class GpoPrivilegeRights : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="inputType">STRING Name of the GPO which will get the Restricted Groups modification.</para>
        /// <para type="description">Name of the GPO which will get the Restricted Groups modification.</para>
        /// </summary>
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Name of the GPO which will get the Privilege Right modification."
            )]
        [ValidateNotNullOrEmpty]
        public string GpoToModify
        {
            get { return _GpoToModify; }
            set { _GpoToModify = value; }
        }

        private string _GpoToModify;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be GRANTED the right "Access this computer from the network"</para>
        /// <para type="description">Identity (SamAccountName) to be GRANTED the right "Access this computer from the network</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be GRANTED the right 'Access this computer from the network'."
            )]
        public List<string> NetworkLogon
        {
            get { return _NetworkLogon; }
            set { _NetworkLogon = value; }
        }

        private List<string> _NetworkLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to configure the right "Deny access this computer from the network"</para>
        /// <para type="description">Identity (SamAccountName) to configure the right "Deny access this computer from the network"</para>
        /// </summary>
        [Parameter(
               Position = 2,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to configure the right 'Deny access this computer from the network'."
            )]
        public List<string> DenyNetworkLogon
        {
            get { return _DenyNetworkLogon; }
            set { _DenyNetworkLogon = value; }
        }

        private List<string> _DenyNetworkLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be GRANTED the right "Allow Log On Locally"</para>
        /// <para type="description">Identity (SamAccountName) to be GRANTED the right "Allow Log On Locally"</para>
        /// </summary>
        [Parameter(
               Position = 3,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be GRANTED the right 'Allow Log On Locally'."
            )]
        public List<string> InteractiveLogon
        {
            get { return _InteractiveLogon; }
            set { _InteractiveLogon = value; }
        }

        private List<string> _InteractiveLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to configure the right "Deny Log On Locally"</para>
        /// <para type="description">Identity (SamAccountName) to configure the right "Deny Log On Locally"</para>
        /// </summary>
        [Parameter(
               Position = 4,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to configure the right 'Deny Log On Locally'."
            )]
        public List<string> DenyInteractiveLogon
        {
            get { return _DenyInteractiveLogon; }
            set { _DenyInteractiveLogon = value; }
        }

        private List<string> _DenyInteractiveLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be GRANTED the right "Allow Log On through Remote Desktop Services"</para>
        /// <para type="description">Identity (SamAccountName) to be GRANTED the right "Allow Log On through Remote Desktop Services"</para>
        /// </summary>
        [Parameter(
               Position = 5,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be GRANTED the right 'Allow Log On through Remote Desktop Services'."
            )]
        public List<string> RemoteInteractiveLogon
        {
            get { return _RemoteInteractiveLogon; }
            set { _RemoteInteractiveLogon = value; }
        }

        private List<string> _RemoteInteractiveLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to configure the right "Deny Log On through Remote Desktop Services"</para>
        /// <para type="description">Identity (SamAccountName) to configure the right "Deny Log On through Remote Desktop Services"</para>
        /// </summary>
        [Parameter(
               Position = 6,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to configure the right 'Deny Log On through Remote Desktop Services'."
            )]
        public List<string> DenyRemoteInteractiveLogon
        {
            get { return _DenyRemoteInteractiveLogon; }
            set { _DenyRemoteInteractiveLogon = value; }
        }

        private List<string> _DenyRemoteInteractiveLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be GRANTED the right "Log On as a Batch Job"</para>
        /// <para type="description">Identity (SamAccountName) to be GRANTED the right "Log On as a Batch Job"</para>
        /// </summary>
        [Parameter(
               Position = 7,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be GRANTED the right 'Log On as a Batch Job'."
            )]
        public List<string> BatchLogon
        {
            get { return _BatchLogon; }
            set { _BatchLogon = value; }
        }

        private List<string> _BatchLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to configure the right "Deny Log On as a Batch Job"</para>
        /// <para type="description">Identity (SamAccountName) to configure the right "Deny Log On as a Batch Job"</para>
        /// </summary>
        [Parameter(
               Position = 8,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to configure the right 'Deny Log On as a Batch Job'."
            )]
        public List<string> DenyBatchLogon
        {
            get { return _DenyBatchLogon; }
            set { _DenyBatchLogon = value; }
        }

        private List<string> _DenyBatchLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be GRANTED the right "Log On as a Service"</para>
        /// <para type="description">Identity (SamAccountName) to be GRANTED the right "Log On as a Service"</para>
        /// </summary>
        [Parameter(
               Position = 9,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be GRANTED the right 'Log On as a Service'."
            )]
        public List<string> ServiceLogon
        {
            get { return _ServiceLogon; }
            set { _ServiceLogon = value; }
        }

        private List<string> _ServiceLogon;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to configure the right "Deny Log On as a Service"</para>
        /// <para type="description">Identity (SamAccountName) to configure the right "Deny Log On as a Service"</para>
        /// </summary>
        [Parameter(
               Position = 10,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to configure the right 'Deny Log On as a Service'."
            )]
        public List<string> DenyServiceLogon
        {
            get { return _DenyServiceLogon; }
            set { _DenyServiceLogon = value; }
        }

        private List<string> _DenyServiceLogon;

        #endregion Parameters definition

        #region variables

        // Return value
        private bool SettingOK = false;

        #endregion variables

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

            IniFile GptTmpl = EguibarIT.Delegation.Other.GPOs.GptTemplate(_GpoToModify);

            // Verify if the GptTmpl.inf file exists by checking existing data
            if (!GptTmpl.Sections.ContainsKey("Version"))
            {
                //Add a new section and some keys
                GptTmpl.Sections.Add(new IniSection("Version"));
                GptTmpl.Sections["Version"].KeyValuePair.Add("signature", "\"$CHICAGO$\"");
                GptTmpl.Sections["Version"].KeyValuePair.Add("Revision", "1");

                //Add a new section and some keys
                GptTmpl.Sections.Add(new IniSection("Unicode"));
                GptTmpl.Sections["Unicode"].KeyValuePair.Add("Unicode", "yes");
            }

            if (!GptTmpl.Sections.ContainsKey("Privilege Rights"))
            {
                GptTmpl.Sections.Add(new IniSection("Privilege Rights"));
            }

            //Save the GptTmpl.Inf file
            GptTmpl.WriteAllText();

            //PSCmdlet.MyInvocation.BoundParameters.ContainsKey(NetworkLogon);

            /////
            ////// right "Access this computer from the network"
            /////
            if (_NetworkLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeNetworkLogonRight", _NetworkLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Access this computer from the network:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ///// right "Deny access this computer from the network"
            /////
            if (_DenyNetworkLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyNetworkLogonRight", _DenyNetworkLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny access this computer from the network:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ////// right "Allow Log On Locally"
            /////
            if (_InteractiveLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeInteractiveLogonRight", _InteractiveLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Allow Log On Locally:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ///// right "Deny Log On Locally"
            /////
            if (_DenyInteractiveLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyInteractiveLogonRight", _DenyInteractiveLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny Log On Locally:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ////// right "Allow Log On through Remote Desktop Services"
            /////
            if (_RemoteInteractiveLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeRemoteInteractiveLogonRight", _RemoteInteractiveLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Allow Log On through Remote Desktop Services:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ///// right "Deny Log On through Remote Desktop Services"
            /////
            if (_DenyRemoteInteractiveLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyRemoteInteractiveLogonRight", _DenyRemoteInteractiveLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny Log On through Remote Desktop Services:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ////// right "Log On as a Batch Job"
            /////
            if (_BatchLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeBatchLogonRight", _BatchLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Log On as a Batch Job:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ///// right "Deny Log On as a Batch Job"
            /////
            if (_DenyBatchLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyBatchLogonRight", _DenyBatchLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny Log On as a Batch Job:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ////// right "Log On as a Service"
            /////
            if (_ServiceLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeServiceLogonRight", _ServiceLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Log On as a Service:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
            }

            /////
            ///// right "Deny Log On as a Service"
            /////
            if (_DenyServiceLogon != null)
            {
                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyServiceLogonRight", _DenyServiceLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny Log On as a Service:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                GptTmpl.WriteAllText();
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
    } //END class GpoPrivilegeRights

    /// <summary>
    /// <para type="synopsis">Set the restrictions on Local groups (MUST be executed on DomainController)</para>
    /// <para type="description">Add groups to the restricted group section of a GPO.</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-GpoRestrictedGroups "Default Domain" "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-GpoRestrictedGroups -GpoToModify "C-Baseline" -LocalAdminUsers "SL_InfraRight"</code>
    ///
    /// </example>
    /// <remarks>Set the restrictions on Local groups(MUST be executed on DomainController)</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "GpoRestrictedGroups", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class GpoRestrictedGroups : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="inputType">STRING Name of the GPO which will get the Restricted Groups modification.</para>
        /// <para type="description">Name of the GPO which will get the Restricted Groups modification.</para>
        /// </summary>
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Name of the GPO which will get the Privilege Right modification."
            )]
        [ValidateNotNullOrEmpty]
        public string GpoToModify
        {
            get { return _GpoToModify; }
            set { _GpoToModify = value; }
        }

        private string _GpoToModify;

        /// <summary>
        /// <para type="inputType">SWITCH parameter (true or false).</para>
        /// <para type="inputType">If present the value becomes TRUE, and the existing group members will remain. New users will be added.</para>
        /// <para type="inputType">Default is Not Present, so value becomes FALSE, and the existing group members will be removed, having only the users configured by this CMDlet.</para>
        /// <para type="description">Switch indicator to merge users (retain existing users). Default is not present, meaning all users in group will be removed</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Switch indicator. If present (TRUE), the access rule will be removed."
            )]
        public SwitchParameter MergeUsers
        {
            get { return _MergeUsers; }
            set { _MergeUsers = value; }
        }

        private bool _MergeUsers;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be included in the Local Administrators group."</para>
        /// <para type="description">Identity (SamAccountName) to be included in the Local Administrators group.</para>
        /// </summary>
        [Parameter(
               Position = 2,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be included in the Local Administrators group."
            )]
        public List<string> LocalAdminUsers
        {
            get { return _LocalAdminUsers; }
            set { _LocalAdminUsers = value; }
        }

        private List<string> _LocalAdminUsers;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be included in the Local Backup Operator group."</para>
        /// <para type="description">Identity (SamAccountName) to be included in the Local Backup Operator group.</para>
        /// </summary>
        [Parameter(
               Position = 3,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be included in the Local Backup Operator group."
            )]
        public List<string> LocalBackupOpUsers
        {
            get { return _LocalBackupOpUsers; }
            set { _LocalBackupOpUsers = value; }
        }

        private List<string> _LocalBackupOpUsers;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be included in the Local Event Log Readers group."</para>
        /// <para type="description">Identity (SamAccountName) to be included in the Local Event Log Readers group.</para>
        /// </summary>
        [Parameter(
               Position = 4,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be included in the Local Event Log Readers group."
            )]
        public List<string> LocalEventLogReaders
        {
            get { return _LocalEventLogReaders; }
            set { _LocalEventLogReaders = value; }
        }

        private List<string> _LocalEventLogReaders;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be included in the Local Performance Log Users group."</para>
        /// <para type="description">Identity (SamAccountName) to be included in the Local Performance Log Users group.</para>
        /// </summary>
        [Parameter(
               Position = 5,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be included in the Local Performance Log Users group."
            )]
        public List<string> LocalPerfLogUsers
        {
            get { return _LocalPerfLogUsers; }
            set { _LocalPerfLogUsers = value; }
        }

        private List<string> _LocalPerfLogUsers;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be included in the Local Performance Monitor Users group."</para>
        /// <para type="description">Identity (SamAccountName) to be included in the Local Performance Monitor Users group.</para>
        /// </summary>
        [Parameter(
               Position = 6,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be included in the Local Performance Monitor Users group."
            )]
        public List<string> LocalPerfMonitorUsers
        {
            get { return _LocalPerfMonitorUsers; }
            set { _LocalPerfMonitorUsers = value; }
        }

        private List<string> _LocalPerfMonitorUsers;

        /// <summary>
        /// <para type="inputType">STRING[] Identity (SamAccountName) to be included in the Local Remote Desktop Users group."</para>
        /// <para type="description">Identity (SamAccountName) to be included in the Local Remote Desktop Users group.</para>
        /// </summary>
        [Parameter(
               Position = 7,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Identity (SamAccountName) to be included in the Local Remote Desktop Users group."
            )]
        public List<string> LocalRemoteDesktopUsers
        {
            get { return _LocalRemoteDesktopUsers; }
            set { _LocalRemoteDesktopUsers = value; }
        }

        private List<string> _LocalRemoteDesktopUsers;

        #endregion Parameters definition

        #region variables

        // Return value
        private bool SettingOK = false;

        #endregion variables

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

            IniFile GptTmpl = EguibarIT.Delegation.Other.GPOs.GptTemplate(_GpoToModify);

            // Verify if the GptTmpl.inf file exists by checking existing data
            if (!GptTmpl.Sections.ContainsKey("Version"))
            {
                //Add a new section and some keys
                GptTmpl.Sections.Add(new IniSection("Version"));
                GptTmpl.Sections["Version"].KeyValuePair.Add("signature", "\"$CHICAGO$\"");
                GptTmpl.Sections["Version"].KeyValuePair.Add("Revision", "1");
            }

            if (!GptTmpl.Sections.ContainsKey("Unicode"))
            {
                //Add a new section and some keys
                GptTmpl.Sections.Add(new IniSection("Unicode"));
                GptTmpl.Sections["Unicode"].KeyValuePair.Add("Unicode", "yes");
            }

            if (!GptTmpl.Sections.ContainsKey("Group Membership"))
            {
                GptTmpl.Sections.Add(new IniSection("Group Membership"));
            }

            //Save the GptTmpl.Inf file
            GptTmpl.WriteAllText();

            #region
            /*
             Any group must start by * followed by SID without spaces
            use __Members for replace mode
            use , (coma) as separator without spaces
            *S-1-5-21-2850578735-692440604-3052452509-1121,*S-1-5-21-2850578735-692440604-3052452509-1117

            .\Administrators
            *S-1-5-32-544__Memberof

            .\Backup Operators
            *S-1-5-32-551__Memberof

            .\Certificate Service DCOM Access
            *S-1-5-32-574__Memberof

            .\Cryptographic Operators
            *S-1-5-32-569__Memberof

            .\Distributed COM Users
            *S-1-5-32-562__Memberof

            .\Event Log Readers
            *S-1-5-32-573__Memberof

            .\IIS_IUSRS
            *S-1-5-32-568__Memberof

            .\Network Configuration Operators
            *S-1-5-32-556__Memberof

            .\Performance Log Users
            *S-1-5-32-559__Memberof

            .\Performance Monitor Users
            *S-1-5-32-558__Memberof

            .\Power Users
            Power Users__Memberof

            .\Print Operators
            *S-1-5-32-550__Memberof

            .\Remote Desktop Users
            *S-1-5-32-555__Memberof

            .\Users
            *S-1-5-32-545__Memberof
             */
            #endregion Process()

            try
            {
                /////
                ///// Local Administrators restricted group
                /////
                ///// Local Administrators<----------*S - 1 - 5 - 32 - 544__Memberof---------- >
                /////
                if (_LocalAdminUsers != null)
                {
                    if (_MergeUsers)
                    {
                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-544__Memberof", _LocalAdminUsers, GptTmpl);
                    }
                    else
                    {
                        //Ignore existing users. Just keep the ones configured here.

                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-544__Members", _LocalAdminUsers, GptTmpl);
                    }

                    if (SettingOK)
                    {
                        WriteVerbose("Local Administrators group restrictions configured");
                        SettingOK = false;
                    }

                    //Save the GptTmpl.inf file
                    GptTmpl.WriteAllText();
                }

                /////
                ///// Local Backup Operators restricted group
                /////
                ///// Local Backup Operators  <---------- *S-1-5-32-551__Memberof ---------->
                /////
                if (_LocalBackupOpUsers != null)
                {
                    if (_MergeUsers)
                    {
                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-551__Memberof", _LocalBackupOpUsers, GptTmpl);
                    }
                    else
                    {
                        //Ignore existing users. Just keep the ones configured here.

                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-551__Members", _LocalBackupOpUsers, GptTmpl);
                    }

                    if (SettingOK)
                    {
                        WriteVerbose("Local Backup Operators group restrictions configured");
                        SettingOK = false;
                    }

                    //Save the GptTmpl.inf file
                    GptTmpl.WriteAllText();
                }

                /////
                ///// Local Event Log Readers restricted group
                /////
                ///// Local Event Log Readers  <---------- *S-1-5-32-573__Memberof ---------->
                /////
                if (_LocalEventLogReaders != null)
                {
                    if (_MergeUsers)
                    {
                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-573__Memberof", _LocalEventLogReaders, GptTmpl);
                    }
                    else
                    {
                        //Ignore existing users. Just keep the ones configured here.

                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-573__Members", _LocalEventLogReaders, GptTmpl);
                    }

                    if (SettingOK)
                    {
                        WriteVerbose("Local Event Log Readers group restrictions configured");
                        SettingOK = false;
                    }

                    //Save the GptTmpl.inf file
                    GptTmpl.WriteAllText();
                }

                /////
                ///// Local Performance Log Users restricted group
                /////
                ///// Local Performance Log Users  <---------- *S-1-5-32-559__Memberof ---------->
                /////
                if (_LocalPerfLogUsers != null)
                {
                    if (_MergeUsers)
                    {
                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-559__Memberof", _LocalPerfLogUsers, GptTmpl);
                    }
                    else
                    {
                        //Ignore existing users. Just keep the ones configured here.

                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-559__Members", _LocalPerfLogUsers, GptTmpl);
                    }

                    if (SettingOK)
                    {
                        WriteVerbose("Local Performance Log Users group restrictions configured");
                        SettingOK = false;
                    }

                    //Save the GptTmpl.inf file
                    GptTmpl.WriteAllText();
                }

                /////
                ///// Local Performance Log Users restricted group
                /////
                ///// Local Performance Monitor Users  <---------- *S-1-5-32-558__Memberof ---------->
                /////
                if (_LocalPerfMonitorUsers != null)
                {
                    if (_MergeUsers)
                    {
                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-558__Memberof", _LocalPerfMonitorUsers, GptTmpl);
                    }
                    else
                    {
                        //Ignore existing users. Just keep the ones configured here.

                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-558__Members", _LocalPerfMonitorUsers, GptTmpl);
                    }

                    if (SettingOK)
                    {
                        WriteVerbose("Local Performance Monitor Users group restrictions configured");
                        SettingOK = false;
                    }

                    //Save the GptTmpl.inf file
                    GptTmpl.WriteAllText();
                }

                /////
                ///// Local Remote Desktop Users restricted group
                /////
                ///// Local Remote Desktop Users  <---------- *S-1-5-32-555__Memberof ---------->
                /////
                if (_LocalRemoteDesktopUsers != null)
                {
                    if (_MergeUsers)
                    {
                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-555__Memberof", _LocalRemoteDesktopUsers, GptTmpl);
                    }
                    else
                    {
                        //Ignore existing users. Just keep the ones configured here.

                        // checked the corresponding Section and read the Key=Value pair for its modification
                        SettingOK = EguibarIT.Delegation.Other.GPOs.ConfigRestrictionsOnSection("Group Membership", "*S-1-5-32-555__Members", _LocalRemoteDesktopUsers, GptTmpl);
                    }

                    if (SettingOK)
                    {
                        WriteVerbose("Local Remote Desktop Users group restrictions configured");
                        SettingOK = false;
                    }

                    //Save the GptTmpl.inf file
                    GptTmpl.WriteAllText();
                }
            }
            catch
            {
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
    }//end GpoRestrictedGroups
} // END namespace EguibarIT.Delegation.GPC