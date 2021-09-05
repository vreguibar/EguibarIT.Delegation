using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Remove Un-Resolvable SID from a given object</para>
    /// <para type="description">Remove Un-Resolvable SID from a given object. If a SID is displayed within the ACE, is</para>
    /// <para type="description">because a name could not be resolved. Most likely the object was deleted, and its friendly </para>
    /// <para type="description">name could not be retrived. This function will identify this unresolved SID and remove it from the ACE</para>
    /// <example>
    /// This example shows how to use this CMDlet, just displaying Unknown SID
    /// <code>Remove-UnknownSID "OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet using named parameters, just displaying Unknown SID
    /// <code>Remove-UnknownSID -LDAPpath "OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local"</code>
    ///
    /// This example shows how to use this CMDlet using named parameters and actually removing Unknown SID
    /// <code>Remove-UnknownSID -LDAPpath "OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local" -RemoveSID</code>
    /// </example>
    /// <remarks>Remove Unknown SID from an ACE</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Remove, "UnknownSID", ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveUnknownSID : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="inputType">STRING representing the Distinguished Name of the object (ej. OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local).</para>
        /// <para type="description">Distinguished Name of the object (or container) where the permissions are going to be configured.</para>
        /// </summary>
        [Parameter(
               Position = 0,
               Mandatory = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Distinguished Name of the object (or container) where the SID is going to be checked."
            )]
        [ValidateNotNullOrEmpty]
        public string LDAPpath
        {
            get { return _ldappath; }
            set { _ldappath = value; }
        }

        private string _ldappath;

        /// <summary>
        /// <para type="inputType">SWITCH parameter (true or false). If present the value becomes TRUE, and the unknown SID will be removed. If ommited, unknown SID will only display.</para>
        /// <para type="description">Switch indicator to remove the unknown SID</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Switch indicator. If present (TRUE), the unknown SID will be removed."
            )]
        public SwitchParameter RemoveSID
        {
            get { return _removesid; }
            set { _removesid = value; }
        }

        private bool _removesid;

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
            // Get the LDAP object to get the access rules from
            DirectoryEntry myObject = new DirectoryEntry(string.Format("LDAP://{0}", _ldappath));

            // Get the access rules
            AuthorizationRuleCollection rules = myObject.ObjectSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

            // Iterate through all Access Control rules
            foreach (System.Security.AccessControl.AuthorizationRule rule in rules)
            {
                ActiveDirectoryAccessRule oar = rule as ActiveDirectoryAccessRule;

                // Check if WellKnownSid. Skipping WellKnownSIDs. Continue if not.
                if (!EguibarIT.Delegation.SIDs.IsWellKnownSID(oar.IdentityReference as SecurityIdentifier))
                {
                    // Translate SID. True if exists. False if it does not exists.
                    if (!EguibarIT.Delegation.SIDs.SidExists(oar.IdentityReference.ToString()))
                    {
                        WriteVerbose(string.Format("Unresolved SID found!   {0}", oar.IdentityReference.ToString()));

                        if (_removesid)
                        {
                            try
                            {
                                // Remove unknown SID from rule
                                myObject.ObjectSecurity.RemoveAccessRule(oar);
                            }
                            catch (Exception ex1)
                            {
                                throw new ApplicationException(string.Format("An error occurred while removing access rule: '{0}'. Message is {1}", ex1, ex1.Message));
                            }
                            finally
                            {
                                WriteVerbose(string.Format("SID removed!   {0}", oar.IdentityReference.ToString()));
                            }
                        }
                        else
                        {
                            WriteWarning(string.Format("---> SID does not exists!   {0}", oar.IdentityReference.ToString()));
                        }
                    }
                }
            }

            try
            {
                // Re-apply the modified DACL to the OU
                // Now push these AccessRules to AD
                myObject.CommitChanges();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("An error occurred while commiting changes to the access rule: '{0}'. Message is {1}", ex, ex.Message));
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
    }
}