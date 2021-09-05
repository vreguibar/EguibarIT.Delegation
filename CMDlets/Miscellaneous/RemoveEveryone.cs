using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Remove Print Operators built-in group from the given object</para>
    /// <para type="description">Remove the built-in group Print Operators from the given object.</para>
    /// <example>This example shows how to use this CMDlet to remove permissions
    /// <code>Remove-Everyone "OU=Admin,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Remove-Everyone -LDAPpath "OU=Admin,DC=EguibarIT,DC=local" </code>
    /// </example>
    /// <remarks>Remove Everyone built-in group from the given object</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Remove, "Everyone", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class Everyone : PSCmdlet
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
            ACENumber              : 2
                DistinguishedName      : OU=X-Computers,DC=EguibarIT,DC=local
                IdentityReference      : Everyone
                ActiveDirectoryRightst : ReadProperty, WriteProperty, GenericExecute
                AccessControlType      : Allow
                ObjectType             : GuidNULL
                InheritanceType        : All
                InheritedObjectType    : GuidNULL
                IsInherited            : False
             */

            // ACE number: 1
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    "Everyone",
                    ActiveDirectoryRights.ReadProperty,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.All,
                    true
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    "Everyone",
                    ActiveDirectoryRights.WriteProperty,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.All,
                    true
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 3
            if (
                EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    "Everyone",
                    ActiveDirectoryRights.GenericExecute,
                    AccessControlType.Allow,
                    System.Guid.Empty,
                    ActiveDirectorySecurityInheritance.All,
                    true
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 3)
            {
                WriteVerbose("Remove Everyone completed succesfully.");
            }

            if (valueFromConstructor != 3)
            {
                WriteWarning("Something went wrong when removing Everyone");
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
    } //END class Everyone
} // END namespace EguibarIT.Delegation.Miscellaneous