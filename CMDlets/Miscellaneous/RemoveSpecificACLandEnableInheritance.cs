using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">The function will Remove Specific/Non-Inherited ACL and enable inheritance on an object</para>
    /// <para type="description">The function will Remove Specific/Non-Inherited ACL and enable inheritance on an object</para>
    /// <example>This example shows how to use this CMDlet to Remove Specific/Non-Inherited ACL and enable inheritance of an object
    /// <code>Set-AdInheritance "OU=Admin,DC=EguibarIT,DC=local" $true $true</code>
    ///
    /// This example shows how to use this CMDlet to Set/Clear Inheritance of an object using named parameters
    /// <code>Remove-SpecificACLandEnableInheritance -LDAPpath "OU=Admin,DC=EguibarIT,DC=local" -RemoveInheritance $true -RemovePermissions $true</code>
    /// </example>
    /// <remarks>The function will Set/Clear Inheritance of an object</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Remove, "SpecificACLandEnableInheritance", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class SpecificACLandEnableInheritance : PSCmdlet
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
               HelpMessage = "Distinguished Name of the object (or container) where the permissions are going to changed."
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
            try
            {
                // Get the object
                DirectoryEntry userEntry = new DirectoryEntry(string.Format("LDAP://{0}", _ldappath));

                //System.Security.AccessControl.AuthorizationRuleCollection Rules = userEntry.ObjectSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                ActiveDirectorySecurity Rules = userEntry.ObjectSecurity;

                AuthorizationRuleCollection arc = Rules.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

                //foreach (System.Security.AccessControl.AuthorizationRule rule in Rules)
                foreach (ActiveDirectoryAccessRule rule in arc)
                {
                    userEntry.ObjectSecurity.RemoveAccessRuleSpecific(rule);
                }
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
                userEntry.ObjectSecurity.SetAccessRuleProtection(false, false);

                // Save changes to the object
                userEntry.CommitChanges();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("An error occurred while fixing inheritance: '{0}'. Message is {1}", ex, ex.Message));
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
    } //END class SpecificACLandEnableInheritance
} // END namespace EguibarIT.Delegation.Miscellaneous