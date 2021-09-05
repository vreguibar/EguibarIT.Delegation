using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    ///     <para type="synopsis">Function to Read all Extended Rights GUID from Schema</para>
    ///     <para type="description">Function that reads all Extended Rights GUID from the Schema and returns a Hashtable object</para>
    ///         <example>
    ///             <para>This example shows how to use this CMDlet to get the Hashtable</para>
    ///             <code>Get-ExtendedRightHashTable</code>
    ///         </example>
    ///     <remarks>Get all Extended Rights and its corresponding GUID from Schema, returning a hashtable</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Get, "ExtendedRightHashTable", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(ExtendedRightHashTable))]
    public class GetExtendedRightHashTable : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        ///     <para type="outputType">Hashtable.</para>
        ///     <para type="description">Hashtable containing Extended Rights and its corresponding GUID.</para>
        /// </summary>
        public Dictionary<string, Guid> ExtendedRightsMap
        {
            get { return extendedrightsmap; }
            set { extendedrightsmap = value; }
        }

        private Dictionary<string, Guid> extendedrightsmap;

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

            try
            {
                if (EguibarIT.Delegation.Vars.AllExtendedRightsGuids == null || EguibarIT.Delegation.Vars.AllExtendedRightsGuids.Count == 0)
                {
                    EguibarIT.Delegation.Vars.AllExtendedRightsGuids = EguibarIT.Delegation.GUIDs.GetAllExtendedRightsGUID();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred when accessing global variable AllExtendedRightsGuids: '{0}'", ex);
            }
        }

        /// <returns>
        /// Returns the Hashtable
        /// </returns>
        protected override void ProcessRecord()
        {
            try
            {
                extendedrightsmap = EguibarIT.Delegation.Vars.AllExtendedRightsGuids;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred when returning AllExtendedRightsGuids global variable: '{0}'", ex);
            }
            WriteObject(extendedrightsmap);
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

    /// <summary>
    ///     <para type="outputType">Hashtable.</para>
    ///     <para type="description">Hashtable containing Extended Rights and its corresponding GUID.</para>
    /// </summary>
    public class ExtendedRightHashTable
    {
        /// <summary>
        ///     <para type="outputType">Hashtable.</para>
        ///     <para type="description">Hashtable containing Extended Rights and its corresponding GUID.</para>
        /// </summary>
        public Dictionary<string, Guid> extendedrightsmap;
    }
}