using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    ///     <para type="synopsis">Function to Read all Attributes GUID from Schema</para>
    ///     <para type="description">Function that reads all Attributes GUID from the Schema and returns a Hashtable object</para>
    ///         <example>
    ///             <para>This example shows how to use this CMDlet to get the Hashtable</para>
    ///             <code>Get-AttributeSchemaHashTable</code>
    ///         </example>
    ///     <remarks>Get all Attributes and its corresponding GUID from Schema, returning a hashtable</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Get, "AttributeSchemaHashTable", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(AttributeSchemaHashTable))]
    public class GetAttributeSchemaHashTable : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        ///     <para type="outputType">Hashtable.</para>
        ///     <para type="description">Hashtable containing Attributes and its corresponding GUID.</para>
        /// </summary>
        public Dictionary<string, Guid> AttributeSchemaMap
        {
            get { return attributeschemamap; }
            set { attributeschemamap = value; }
        }

        private Dictionary<string, Guid> attributeschemamap;

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
                if (EguibarIT.Delegation.Vars.AllAttributeSchemaGuids == null || EguibarIT.Delegation.Vars.AllAttributeSchemaGuids.Count == 0)
                {
                    EguibarIT.Delegation.Vars.AllAttributeSchemaGuids = EguibarIT.Delegation.GUIDs.GetAllAttributeSchemaGUID();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred when accessing global variable AllAttributeSchemaGuids: '{0}'", ex);
            }
        }

        /// <returns>
        /// Returns the Hashtable
        /// </returns>
        protected override void ProcessRecord()
        {
            try
            {
                attributeschemamap = EguibarIT.Delegation.Vars.AllAttributeSchemaGuids;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred when returning AllAttributeSchemaGuids global variable: '{0}'", ex);
            }

            WriteObject(attributeschemamap);
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
    ///     <para type="description">Hashtable containing Attributes and its corresponding GUID.</para>
    /// </summary>
    public class AttributeSchemaHashTable
    {
        /// <summary>
        ///     <para type="outputType">Hashtable.</para>
        ///     <para type="description">Hashtable containing Attributes and its corresponding GUID.</para>
        /// </summary>
        public Dictionary<string, Guid> AttributeSchemaMap;
    }
}