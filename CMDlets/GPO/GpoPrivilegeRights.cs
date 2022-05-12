using EguibarIT.Delegation.Other;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
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
}
