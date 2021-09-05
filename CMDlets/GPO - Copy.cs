using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

//using IniParser;
//using IniParser.Model;
using Microsoft.GroupPolicy;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using EguibarIT.Delegation.Other;

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
    public class CreateDeleteGPO : Cmdlet
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


        #endregion

        #region Begin()
        /// <summary>
        /// 
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }
        #endregion Begin()

        #region Process()
        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

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
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    string.Format("CN=Policies,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                    _group,
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.Vars.GuidNULL,
                    ActiveDirectorySecurityInheritance.None,
                    _removerule
                );

        }
        #endregion Process()

        #region End()
        /// <summary>
        /// 
        /// </summary>
        protected override void EndProcessing()
        {

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
    public class LinkGPO : Cmdlet
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


        #endregion

        #region Begin()
        /// <summary>
        /// 
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }
        #endregion Begin()

        #region Process()
        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

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
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("gPLink"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                );

            // ACE number: 2
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("gPLink"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                );

        }
        #endregion Process()

        #region End()
        /// <summary>
        /// 
        /// </summary>
        protected override void EndProcessing()
        {

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
    public class GPoption : Cmdlet
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


        #endregion

        #region Begin()
        /// <summary>
        /// 
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }
        #endregion Begin()

        #region Process()
        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

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
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("gPOptions"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                );

            // ACE number: 2
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("gPOptions"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                );

        }
        #endregion Process()

        #region End()
        /// <summary>
        /// 
        /// </summary>
        protected override void EndProcessing()
        {

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
    public class RSoPLogging : Cmdlet
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


        #endregion

        #region Begin()
        /// <summary>
        /// 
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }
        #endregion Begin()

        #region Process()
        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

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
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Generate Resultant Set of Policy (Logging)"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                );

        }
        #endregion Process()

        #region End()
        /// <summary>
        /// 
        /// </summary>
        protected override void EndProcessing()
        {

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
    public class RSoPPlanning : Cmdlet
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


        #endregion

        #region Begin()
        /// <summary>
        /// 
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }
        #endregion Begin()

        #region Process()
        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

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
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                    _group,
                    ActiveDirectoryRights.ExtendedRight,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Generate Resultant Set of Policy (Planning)"),
                    ActiveDirectorySecurityInheritance.All,
                    _removerule
                );

        }
        #endregion Process()

        #region End()
        /// <summary>
        /// 
        /// </summary>
        protected override void EndProcessing()
        {

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
    /// <remarks>Set the Privileged Rights into a Group Policy Objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "GpoPrivilegeRights", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class GpoPrivilegeRights : Cmdlet
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

        #endregion


        #region variables

        // DefaultNamingContext DistinguishedName
        string AdDn;

        // https://github.com/rickyah/ini-parser/wiki/First-Steps
        //Create an instance of a ini file parser
        //FileIniDataParser parser = new FileIniDataParser();
        

        // Inizialize file variable for future use
        //IniData Gpt = new IniData();
        //IniData GptTmpl = new IniData();

        //Variables hosting path to files
        string PathToGptTmpl;
        string PathToGpt;
        string SysVolPath;
        string GptTmplFile;

        // Use current user’s domain 
        Microsoft.GroupPolicy.GPDomain domain = new GPDomain();

        // Define the GPO variable
        Microsoft.GroupPolicy.Gpo Gpo;

        // Define GPO ID string
        string GpoId;

        // Registry Key var
        RegistryKey rk;

        //Vars for VersionObject
        DirectoryEntry de;
        Int64 VersionObject;
        string HexValue;
        string HexUserVN;
        string HexComputerVN;
        Int64 UserVN;
        Int64 ComputerVN;
        string NewHex;
        Int64 NewVersionObject;

        // Return value
        bool SettingOK = false;

        #endregion variables

        #region Begin()
        /// <summary>
        /// 
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            // Configure IniParser not to use spaces between separator Key=Value
            //parser.Parser.Configuration.AssigmentSpacer = "";


            // Get AD DistinguishedName
            AdDn = EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext();

            // Get the GPO
            Gpo = domain.GetGpo(_GpoToModify);

            // Get the GPO Id and Include Brackets {}
            GpoId = "{" + Gpo.Id + "}";

            // Get the SysVol path from registry
            rk = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Netlogon\\Parameters", false);
            SysVolPath = (string)rk.GetValue("SysVol");
            rk.Close();


            // Get path where the GptTmpl.inf file should be stored
            PathToGptTmpl = string.Format("{0}\\{1}\\Policies\\{2}\\Machine\\microsoft\\windows nt\\SecEdit", SysVolPath, EguibarIT.Delegation.Other.Domains.GetAdFQDN(), GpoId);

            // If the folder does not exist yet, it will be created. If the folder exists already, the line will be ignored
            Directory.CreateDirectory(PathToGptTmpl);

            // Get full path + filename
            GptTmplFile = string.Format("{0}\\GptTmpl.inf", PathToGptTmpl);

            // Get path to the Gpt.ini file. Increment to make changes.
            PathToGpt = string.Format("{0}\\{1}\\Policies\\{2}\\gpt.ini", SysVolPath, EguibarIT.Delegation.Other.Domains.GetAdFQDN(), GpoId);


            // Get content of Gpt.inf file
            // This load the INI file, reads the data contained in the file, and parses that data
            //Gpt = parser.ReadFile(PathToGpt);
            

            // Verify if the GptTmpl.inf file exists
            if (File.Exists(GptTmplFile))
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                //GptTmpl = parser.ReadFile(GptTmplFile);

                IniParser GptTmpl = new IniParser(GptTmplFile);
            }
            else
            {
                // Create the GptTmpl.Inf with basic settings

                //Add a new section and some keys
                GptTmpl.Sections.AddSection("Version");
                GptTmpl["Version"].AddKey("signature", "\"$CHICAGO$\"");
                GptTmpl["Version"].AddKey("Revision", "1");

                //Add a new section and some keys
                GptTmpl.Sections.AddSection("Unicode");
                GptTmpl["Unicode"].AddKey("Unicode", "yes");
            }


            //This line gets the SectionData from the section "Privilege Rights"
            KeyDataCollection keyCol = GptTmpl["Privilege Rights"];

            // If section does not exists, then the COUNT value is 0
            if (keyCol.Count == 0)
            {
                // Create New "Privilege Rights" section
                GptTmpl.Sections.AddSection("Privilege Rights");
            }

            //Save the GptTmpl.Inf file
            //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
            System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());



        }
        #endregion Begin()

        #region Process()
        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            //PSCmdlet.MyInvocation.BoundParameters.ContainsKey(NetworkLogon);

            /////
            ////// right "Access this computer from the network"
            /////
            if (_NetworkLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeNetworkLogonRight", _NetworkLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Access this computer from the network:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ///// right "Deny access this computer from the network"
            /////
            if (_DenyNetworkLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyNetworkLogonRight", _DenyNetworkLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny access this computer from the network:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ////// right "Allow Log On Locally"
            /////
            if (_InteractiveLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeInteractiveLogonRight", _InteractiveLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Allow Log On Locally:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ///// right "Deny Log On Locally"
            /////
            if (_DenyInteractiveLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyInteractiveLogonRight", _DenyInteractiveLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny Log On Locally:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ////// right "Allow Log On through Remote Desktop Services"
            /////
            if (_RemoteInteractiveLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeRemoteInteractiveLogonRight", _RemoteInteractiveLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Allow Log On through Remote Desktop Services:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ///// right "Deny Log On through Remote Desktop Services"
            /////
            if (_DenyRemoteInteractiveLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyRemoteInteractiveLogonRight", _DenyRemoteInteractiveLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny Log On through Remote Desktop Services:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ////// right "Log On as a Batch Job"
            /////
            if (_BatchLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeBatchLogonRight", _BatchLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Log On as a Batch Job:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ///// right "Deny Log On as a Batch Job"
            /////
            if (_DenyBatchLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyBatchLogonRight", _DenyBatchLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny Log On as a Batch Job:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ////// right "Log On as a Service"
            /////
            if (_ServiceLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeServiceLogonRight", _ServiceLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Log On as a Service:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

            /////
            ///// right "Deny Log On as a Service"
            /////
            if (_DenyServiceLogon != null)
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                GptTmpl = parser.ReadFile(GptTmplFile);

                // checked the corresponding Section and read the Key=Value pair for its modification
                SettingOK = EguibarIT.Delegation.Other.GpoPrivilegeRights.ConfigRestrictionsOnSection("Privilege Rights", "SeDenyServiceLogonRight", _DenyServiceLogon, GptTmpl);

                if (SettingOK)
                {
                    WriteVerbose("Deny Log On as a Service:  OK");
                    SettingOK = false;
                }

                //Save the GptTmpl.inf file
                //parser.WriteFile(GptTmplFile, GptTmpl); // Can't use parser to save, generates an error when GPO console try to read the file.
                System.IO.File.WriteAllText(@GptTmplFile, GptTmpl.ToString());
            }

        }

        #endregion Process()

        #region End()
        /// <summary>
        /// 
        /// </summary>
        protected override void EndProcessing()
        {
            // Modify the GPO versionNumber on the GPC
            // https://technet.microsoft.com/en-us/library/ff730972.aspx
            // https://blogs.technet.microsoft.com/grouppolicy/2008/01/08/understanding-the-domain-based-gpo-version-number-gpmc-script-included/

            // Get the GPO object
            de = new DirectoryEntry(string.Format("LDAP://CN={0},CN=Policies,CN=System,{1}", GpoId, AdDn));

            // Get the VersionObject of the DirectoryEntry (the GPO)
            VersionObject = Convert.ToInt64(de.Properties["VersionNumber"].Value.ToString());

            // Convert the value into a 8 digit HEX string
            HexValue = VersionObject.ToString("x8");

            // Top 16 bits HEX UserVersionNumber - first 4 characters (complete with zero to the left)
            // This is the UserVersion
            HexUserVN = HexValue.Substring(0, 4);

            // Lower 16 bits HEX ComputerVersionNumber - last 4 characters (complete with zero to the left)
            // This is the ComputerVersion
            HexComputerVN = HexValue.Substring(4);

            //Top 16 bits as Integer UserVersionNumber
            UserVN = Convert.ToInt64(HexUserVN, 16);

            //Lower 16 bits as Integer ComputerVersionNumber
            ComputerVN = Convert.ToInt64(HexComputerVN, 16);

            // Increment Computer Version Number by 10
            ComputerVN += 5;

            //Concatenate '0x' and 'HEX UserVersionNumber having 4 digits' and 'HEX ComputerVersionNumber having 4 digits' (0x must be added in order to indicate Hexadecimal number, otherwise fails)
            NewHex = "0x" + HexUserVN + ComputerVN.ToString("x4");

            // Convert the New Hex number to integer
            NewVersionObject = Convert.ToInt64(NewHex, 16);

            //Update the GPO VersionNumber with the new value
            de.Properties["VersionNumber"].Value = NewVersionObject.ToString();

            // Save the information on the DirectoryObject
            de.CommitChanges();

            //Close the DirectoryEntry
            de.Close();

            // Write new version value to GPT
            Gpt["General"]["Version"] = NewVersionObject.ToString();

            // Save settings on GPT file
            //parser.WriteFile(PathToGpt, Gpt); // Can't use parser to save, generates an error when GPO console try to read the file.
            System.IO.File.WriteAllText(@PathToGpt, Gpt.ToString());

        }
        #endregion End()

        /// <summary>
        /// 
        /// </summary>
        protected override void StopProcessing()
        {

        }

    } //END class GpoPrivilegeRights





} // END namespace EguibarIT.Delegation.GPC
