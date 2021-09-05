using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Remove Pre-Windows 2000 Compatible Access built-in group from the given Organizational Unit</para>
    /// <para type="description">Remove the built-in group Pre-Windows 2000 Compatible Access from the given Organizational Unit.</para>
    /// <example>This example shows how to use this CMDlet to remove permissions
    /// <code>Remove-PreWin2000FromOU "OU=Admin,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Remove-PreWin2000FromOU -LDAPpath "OU=Admin,DC=EguibarIT,DC=local" </code>
    /// </example>
    /// <remarks>Remove Pre-Windows 2000 Compatible Access built-in group from the given Organizational Unit</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Remove, "PreWin2000FromOU", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class PreWin2000FromOU : PSCmdlet
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

            // Get the object
            DirectoryEntry userEntry = new DirectoryEntry(string.Format("LDAP://{0}", _ldappath));

            /*
            # First value will set/Remove the inheritance Check-Box
            #     set the value of the IsProtected parameter (1) to TRUE, the inheritance checkbox will be cleared.
            #     If we set it to FALSE, the checkbox will become checked
            # Second value will "copy" (true)  or "remove" (false) the permissions
            #     The �preserveInheritance� parameter (2) only has an effect when we uncheck the inheritance checkbox  (IsProtected = TRUE).
            #     If we set preserveInheritance to TRUE then the permissions from the parent object are copied to the object.
            #     It has the same effect as clicking �Add�.
            #     If �preserverInheritance� is set to FALSE, it has the same effect as clicking �Remove�
             */
            userEntry.ObjectSecurity.SetAccessRuleProtection(true, true);

            // Save changes to the object
            userEntry.CommitChanges();

            /*

             */

            // ACE number: 1
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ListChildren,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.All,
                    true
                );

            // ACE number: 2 ###> ReadProperty <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                    true
                );

            // ACE number: 2 ###> ListObject <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ListObject,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                    true
                );

            // ACE number: 2 ###> ReadControl <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ReadControl,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("inetOrgPerson"),
                    true
                );

            // ACE number: 3 ###> ReadProperty <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("group"),
                    true
                );

            // ACE number: 3 ###> ListObject <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ListObject,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("group"),
                    true
                );

            // ACE number: 3 ###> ReadControl <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ReadControl,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("group"),
                    true
                );

            // ACE number: 4 ###> ReadProperty <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("user"),
                    true
                );

            // ACE number: 4 ###> ListObject <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ListObject,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("user"),
                    true
                );

            // ACE number: 4 ###> ReadControl <###
            EguibarIT.Delegation.Constructors.AclCon6.AclConstructor6(
                    _ldappath,
                    "pre–windows 2000 compatible access",
                    ActiveDirectoryRights.ReadControl,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.Descendents,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("user"),
                    true
                );
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
    } //END class PreWin2000FromOU
} // END namespace EguibarIT.Delegation.Miscellaneous