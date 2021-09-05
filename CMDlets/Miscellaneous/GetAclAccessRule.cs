using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Show Access Rules of given object</para>
    /// <para type="description">Display all the Access Rules</para>
    /// <para type="description">for the given object</para>
    /// <example>
    /// This example shows how to use this CMDlet
    /// <code>Get-AclAccessRule "OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet filtering by identity
    /// <code>Get-AclAccessRule "OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local" "TheGood"</code>
    ///
    /// This example shows how to use this CMDlet using named parameters
    /// <code>Get-AclAccessRule -LDAPpath "OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet using named parameters filtering by identity
    /// <code>Get-AclAccessRule -LDAPpath "OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local" -SearchBy "TheGood"</code>
    /// </example>
    /// <remarks>Get the list of ACE of a given objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Get, "AclAccessRule", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(ACEarray))]
    public class AccessRule : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="inputType">STRING representing the container Distinguished name (ej. OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local).</para>
        /// <para type="description">The DistinguishedName of object to show Access Rule.</para>
        /// </summary>
        [Parameter(
               Position = 0,
               Mandatory = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "The DistinguishedName of object to show ACL."
            )]
        [ValidateNotNullOrEmpty]
        public string LDAPpath
        {
            get { return _ldappath; }
            set { _ldappath = value; }
        }

        private string _ldappath;

        /// <summary>
        /// <para type="inputType">STRING representing the Identity to search for. (ej. Administrator).</para>
        /// <para type="description">The SamAccountName of object to search for within all Access Rules.</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "The SamAccountName of object to search for within all Access Rules."
            )]
        public string SearchBy
        {
            get { return _searchby; }
            set { _searchby = value; }
        }

        private string _searchby;

        #endregion Parameters definition

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

        /// <returns>
        /// Returns the ACE of the given object
        /// </returns>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            //Create list of arrays to hold the results
            List<ACEarray> List = new List<ACEarray>();

            if (_searchby == null)
            {
                List = EguibarIT.Delegation.Other.AccessRule.AclAccessRule(_ldappath);

                WriteVerbose("\r\n               ACE (Access Control Entry)                     ");
                WriteVerbose("============================================================");
            }
            else
            {
                List = EguibarIT.Delegation.Other.AccessRule.AclAccessRule(_ldappath, _searchby);

                WriteVerbose(string.Format("\r\n        ACE (Access Control Entry)  Filtered By: {0}   ", _searchby));
                WriteVerbose("============================================================");
            }

            //WriteObject(List);
            WriteObject(List, true);

            WriteVerbose("\r\n------------------------------------------------------------");

            WriteVerbose(string.Format("Total ACEs : {0} \r\n", List.Count));
        }

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

        /// <summary>
        ///
        /// </summary>
        protected override void StopProcessing()
        {
        }
    }
}//end namespace