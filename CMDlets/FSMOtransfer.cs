using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    ///     <para type="synopsis">Delegate the management rights of FMSO roles</para>
    ///     <para type="description">Delegate the right to transfer any of the 5 FMSO roles to a given group</para>
    ///     <example>
    ///         <code>
    ///             Set-AdAclFMSOtransfer -Group "SL_FSMOadmin" -FSMOroles 'Schema', 'Infrastructure', 'DomainNaming', 'RID', 'PDC'
    ///         </code>
    ///     </example>
    ///     <example>
    ///         <code>
    ///             Set-AdAclFMSOtransfer -Group "SL_FSMOadmin" -FSMOroles 'Schema', 'Infrastructure', 'DomainNaming', 'RID', 'PDC' -RemoveRule
    ///         </code>
    ///     </example>
    ///     <remarks>Delegate the management rights of FMSO roles</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclFSMOtransfer", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class FSMOtransfer : PSCmdlet
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
        /// <para type="inputType">STRING ARRAY representing the each FSMO roles.</para>
        /// <para type="description">Flexible Single Master Operations (FSMO) Roles to delegate.</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Flexible Single Master Operations (FSMO) Roles to delegate."
            )]
        [ValidateNotNullOrEmpty]
        [ValidateSet("Schema", "Infrastructure", "DomainNaming", "RID", "PDC")]
        public string[] FSMOroles
        {
            get { return _FSMOroles; }
            set { _FSMOroles = value; }
        }
        private string[] _FSMOroles;


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


        #endregion

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

            foreach(string role in FSMOroles)
            {
                switch (role)
                {
                    case "Schema":
                        ////////////////////////////////////////////////
                        // Change Schema Master - Forest Role
                        /*
                              ACENumber              : 1
                                    DistinguishedName: CN=Schema,CN=Configuration,DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : ExtendedRight
                              AccessControlType      : Allow
                              ObjectType             : Change Schema Master[Extended Rights]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False

                              ACENumber              : 2
                                    DistinguishedName: CN=Schema,CN=Configuration,DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : WriteProperty
                              AccessControlType      : Allow
                              ObjectType             : fSMORoleOwner [AttributeSchema]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False
                        */

                        // ACENumber 1
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               EguibarIT.Delegation.Other.Domains.GetschemaNamingContext(),
                               _group,
                               ActiveDirectoryRights.ExtendedRight,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Change Schema Master"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Change Schema Master Extended Right\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Change Schema Master Extended Right\" .");
                        }

                        // ACENumber 2
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               EguibarIT.Delegation.Other.Domains.GetschemaNamingContext(),
                               _group,
                               ActiveDirectoryRights.WriteProperty,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("fSMORoleOwner"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Write Property fSMORoleOwner\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Write Property fSMORoleOwner\" .");
                        }
                        break;

                    case "DomainNaming":
                        ////////////////////////////////////////////////
                        // Change Domain Naming Master  - Forest Role
                        /*
                              ACENumber              : 1
                                    DistinguishedName: CN=Partitions,CN=Configuration,DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : ExtendedRight
                              AccessControlType      : Allow
                              ObjectType             : Change Domain Master[Extended Rights]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False

                              ACENumber              : 2
                                    DistinguishedName: CN=Partitions,CN=Configuration,DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : WriteProperty
                              AccessControlType      : Allow
                              ObjectType             : fSMORoleOwner [AttributeSchema]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False
                        */

                        // ACENumber 1
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               string.Format("CN=Partitions,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                               _group,
                               ActiveDirectoryRights.ExtendedRight,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Change Domain Master"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Change Domain Master Extended Right\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Change Domain Master Extended Right\" .");
                        }

                        // ACENumber 2
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               string.Format("CN=Partitions,{0}", EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext()),
                               _group,
                               ActiveDirectoryRights.WriteProperty,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("fSMORoleOwner"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Write Property fSMORoleOwner\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Write Property fSMORoleOwner\" .");
                        }
                        break;

                    case "Infrastructure":
                        ////////////////////////////////////////////////
                        // Infrastructure Master  - Domain Role
                        /*
                              ACENumber              : 1
                                    DistinguishedName: CN=Infrastructure,DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : ExtendedRight
                              AccessControlType      : Allow
                              ObjectType             : Change Infrastructure Master[Extended Rights]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False

                              ACENumber              : 2
                                    DistinguishedName: CN=Infrastructure,DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : WriteProperty
                              AccessControlType      : Allow
                              ObjectType             : fSMORoleOwner [AttributeSchema]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False
                        */

                        // ACENumber 1
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               string.Format("CN=Infrastructure,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                               _group,
                               ActiveDirectoryRights.ExtendedRight,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Change Infrastructure Master"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Change Infrastructure Master Extended Right\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Change Infrastructure Master Extended Right\" .");
                        }

                        // ACENumber 2
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               string.Format("CN=Infrastructure,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                               _group,
                               ActiveDirectoryRights.WriteProperty,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("fSMORoleOwner"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Write Property fSMORoleOwner\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Write Property fSMORoleOwner\" .");
                        }
                        break;

                    case "RID":
                        ////////////////////////////////////////////////
                        // Relative Identyfier (RID) Master  - Domain Role
                        /*
                              ACENumber              : 1
                                    DistinguishedName: CN=RID Manager$,CN=System,DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : ExtendedRight
                              AccessControlType      : Allow
                              ObjectType             : Change Rid Master[Extended Rights]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False

                              ACENumber              : 2
                                    DistinguishedName: CN=RID Manager$,CN=System,DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : WriteProperty
                              AccessControlType      : Allow
                              ObjectType             : fSMORoleOwner [AttributeSchema]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False
                        */

                        // ACENumber 1
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               string.Format("CN=RID Manager$,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                               _group,
                               ActiveDirectoryRights.ExtendedRight,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Change Rid Master"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Change Rid Master Extended Right\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Change Rid Master Extended Right\" .");
                        }

                        // ACENumber 2
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               string.Format("CN=RID Manager$,CN=System,{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()),
                               _group,
                               ActiveDirectoryRights.WriteProperty,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("fSMORoleOwner"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Write Property fSMORoleOwner\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Write Property fSMORoleOwner\" .");
                        }
                        break;

                    case "PDC":
                        ////////////////////////////////////////////////
                        // PDCe Master  - Domain Role
                        /*
                              ACENumber              : 1
                                    DistinguishedName: DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : ExtendedRight
                              AccessControlType      : Allow
                              ObjectType             : Change PDC[Extended Rights]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False

                              ACENumber              : 2
                                    DistinguishedName: DC=EguibarIT,DC=local
                              IdentityReference      : EguibarIT\SL_FSMOtransferRight
                              ActiveDirectoryRightst : WriteProperty
                              AccessControlType      : Allow
                              ObjectType             : fSMORoleOwner [AttributeSchema]
                                      InheritanceType: None
                              InheritedObjectType    : GuidNULL
                              IsInherited            : False
                        */

                        // ACENumber 1
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                               _group,
                               ActiveDirectoryRights.ExtendedRight,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Change PDC"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Change PDC Extended Right\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Change PDC Extended Right\" .");
                        }

                        // ACENumber 2
                        if (
                           EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                               EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext(),
                               _group,
                               ActiveDirectoryRights.WriteProperty,
                               AccessControlType.Allow,
                               EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("fSMORoleOwner"),
                               _removerule
                        ))
                        {
                            WriteVerbose("Delegate \"Write Property fSMORoleOwner\" completed succesfully");
                        }
                        else
                        {
                            WriteWarning("Something went wrong when delegating \"Write Property fSMORoleOwner\" .");
                        }
                        break;
                } //end switch

                if (_removerule)
                {
                    WriteVerbose(String.Format("The right to transfer {1} role was revoked from {0}.", _group, role));
                }
                else
                {
                    WriteVerbose(String.Format("{0} now has the right to transfer {1} role.", _group, role));
                }
            } //end foreach
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

    } //END class FSMOtransfer
} // END namespace EguibarIT.Delegation
