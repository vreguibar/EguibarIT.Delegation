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
    /// <code>Remove-PrintOperator "OU=Admin,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Remove-PrintOperator -LDAPpath "OU=Admin,DC=EguibarIT,DC=local" </code>
    /// </example>
    /// <remarks>Remove Print Operators built-in group from the given object</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Remove, "PrintOperator", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class PrintOperator : PSCmdlet
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
                DistinguishedName      : OU=NoPolicy,DC=EguibarIT,DC=local
                IdentityReference      : BUILTIN\Print Operators
                ActiveDirectoryRightst : CreateChild, DeleteChild
                AccessControlType      : Allow
                ObjectType             : printQueue [ClassSchema]
                InheritanceType        : None
                InheritedObjectType    : GuidNULL
                IsInherited            : False
             */

            // ACE number: 1 ###> CreateChild <###
            if (
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    "Print Operators",
                    ActiveDirectoryRights.CreateChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("printQueue"),
                    ActiveDirectorySecurityInheritance.All,
                    true
                ))
            {
                valueFromConstructor++;
            }

            // ACE number: 2 ###> DeleteChild <###
            if (
            EguibarIT.Delegation.Constructors.AclCon5.AclConstructor5(
                    _ldappath,
                    "Print Operators",
                    ActiveDirectoryRights.DeleteChild,
                    AccessControlType.Allow,
                    EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("printQueue"),
                    ActiveDirectorySecurityInheritance.All,
                    true
                ))
            {
                valueFromConstructor++;
            }

            //Check if everithing went OK. One point for each constructor returning TRUE
            if (valueFromConstructor == 2)
            {
                WriteVerbose("Print Operators was removed succesfully.");
            }

            if (valueFromConstructor != 2)
            {
                WriteWarning("Something happened when trying to remove Print Operators");
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
    } //END class PrintOperator
} // END namespace EguibarIT.Delegation.Miscellaneous