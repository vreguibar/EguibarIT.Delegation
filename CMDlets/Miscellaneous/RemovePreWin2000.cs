using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Remove Pre-Windows 2000 Compatible Access built-in group from the given object</para>
    /// <para type="description">Remove the built-in group Pre-Windows 2000 Compatible Access from the given object.</para>
    /// <example>This example shows how to use this CMDlet to remove permissions
    /// <code>Remove-PreWin2000 "OU=Admin,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Remove-PreWin2000 -LDAPpath "OU=Admin,DC=EguibarIT,DC=local" </code>
    /// </example>
    /// <remarks>Remove Pre-Windows 2000 Compatible Access built-in group from the given object</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Remove, "PreWin2000", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class PreWin2000 : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="inputType">STRING representing the Distinguished Name of the object (ej. OU=Admin,DC=EguibarIT,DC=local).</para>
        /// <para type="description">Distinguished Name of the object (or container) where the permissions are going to be removed.</para>
        /// </summary>
        [Parameter(
               Position = 0,
               Mandatory = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Distinguished Name of the object (or container) where the permissions are going to be removed."
            )]
        [ValidateNotNullOrEmpty]
        public string LDAPpath
        {
            get { return _ldappath; }
            set { _ldappath = value; }
        }

        private string _ldappath;

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
                ACENumber              : 1
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : Account Restrictions [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : inetOrgPerson [ClassSchema]
                IsInherited            : True

                ACENumber              : 2
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : Account Restrictions [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : user [ClassSchema]
                IsInherited            : True

                ACENumber              : 3
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : Logon Information [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : inetOrgPerson [ClassSchema]
                IsInherited            : True

                ACENumber              : 4
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : Group Membership [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : inetOrgPerson [ClassSchema]
                IsInherited            : True

                ACENumber              : 5
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : Group Membership [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : user [ClassSchema]
                IsInherited            : True

                ACENumber              : 6
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : General Information [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : inetOrgPerson [ClassSchema]
                IsInherited            : True

                ACENumber              : 7
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : General Information [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : user [ClassSchema]
                IsInherited            : True

                ACENumber              : 8
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : Remote Access Information [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : inetOrgPerson [ClassSchema]
                IsInherited            : True

                ACENumber              : 9
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ReadProperty
                AccessControlType      : Allow
                ObjectType             : Remote Access Information [Extended Rights]
                InheritanceType        : Descendents
                InheritedObjectType    : user [ClassSchema]
                IsInherited            : True

                ACENumber              : 10
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : GenericRead
                AccessControlType      : Allow
                ObjectType             : GuidNULL
                InheritanceType        : Descendents
                InheritedObjectType    : inetOrgPerson [ClassSchema]
                IsInherited            : True

                ACENumber              : 11
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : GenericRead
                AccessControlType      : Allow
                ObjectType             : GuidNULL
                InheritanceType        : Descendents
                InheritedObjectType    : group [ClassSchema]
                IsInherited            : True

                ACENumber              : 12
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : GenericRead
                AccessControlType      : Allow
                ObjectType             : GuidNULL
                InheritanceType        : Descendents
                InheritedObjectType    : user [ClassSchema]
                IsInherited            : True

                ACENumber              : 13
                IdentityReference      : BUILTIN\Pre–Windows 2000 Compatible Access
                ActiveDirectoryRightst : ListChildren
                AccessControlType      : Allow
                ObjectType             : GuidNULL
                InheritanceType        : All
                InheritedObjectType    : GuidNULL
                IsInherited            : True
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "Pre–Windows 2000 Compatible Access",
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Account Restrictions"),
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                    true
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ReadProperty,
                   AccessControlType.Allow,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Account Restrictions"),
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("user"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ReadProperty,
                   AccessControlType.Allow,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Logon Information"),
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 4
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ReadProperty,
                   AccessControlType.Allow,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Group Membership"),
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 5
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ReadProperty,
                   AccessControlType.Allow,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Group Membership"),
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("user"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 6
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ReadProperty,
                   AccessControlType.Allow,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("General Information"),
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 7
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ReadProperty,
                   AccessControlType.Allow,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("General Information"),
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("user"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 8
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ReadProperty,
                   AccessControlType.Allow,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Remote Access Information"),
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 9
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ReadProperty,
                   AccessControlType.Allow,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Remote Access Information"),
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("user"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 10
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.GenericRead,
                   AccessControlType.Allow,
                   System.Guid.Empty,
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 11
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.GenericRead,
                   AccessControlType.Allow,
                   System.Guid.Empty,
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("group"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 12
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.GenericRead,
                   AccessControlType.Allow,
                   System.Guid.Empty,
                   ActiveDirectorySecurityInheritance.Descendents,
                   EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("user"),
                   true
               ))
            {
                valueFromConstructor++;
            }

            // ACE number: 13
            if (
               EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                   _ldappath,
                   "Pre–Windows 2000 Compatible Access",
                   ActiveDirectoryRights.ListChildren,
                   AccessControlType.Allow,
                   System.Guid.Empty,
                   ActiveDirectorySecurityInheritance.All,
                   System.Guid.Empty,
                   true
               ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 13)
            {
                WriteVerbose("Remove Pre–Windows 2000 Compatible Access completed succesfully.");
            }

            if (valueFromConstructor != 13)
            {
                WriteWarning("Something went wrong when removing Pre–Windows 2000 Compatible Access");
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
    } //END class PreWin2000
} // END namespace EguibarIT.Delegation.Miscellaneous