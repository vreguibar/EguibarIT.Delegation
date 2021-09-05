/*

 // Create empty new IniFile object
 IniFile newIni = new IniFile();

 //Read Ini file
 newIni.ReadFile(@"C:\Temp\File.inf");

 //Create new IniFile object from INI file
 IniFile newIni = new IniFile(@"C:\Temp\File.inf");

 //Enumerate all sections and its corresponding Key/Values
 foreach(var section in newIni.Sections)
     {
         Console.WriteLine("Section: {0}", section.Value.SectionName);

         foreach(var setting in section.Value.KeyValuePair)
         {
             Console.WriteLine("Key: {0} Value: {1}", setting.Key, setting.Value);
         }
     }

 //Check section exists
 if (newIni.Sections.ContainsKey("Asasa"))
 {
     Console.WriteLine("Section Name: {0}", newIni.Sections["Version"].SectionName);
 }
 else
 {
     Console.WriteLine("Section does not exist");
 }

 //Get existing Section
 Console.WriteLine(newIni.Sections["Version"].SectionName);

 // Gets the KeyValuePair with name key in the specified section.
 Console.WriteLine(newIni["Version", "signature"]);
 // OR
 Console.WriteLine(newIni.Sections["Version"].KeyValuePair["signature"]);

 //Add a new section
 IniSection section = new IniSection("Delegation Model");
 newIni.Sections.Add(section);
 // OR
 newIni.Sections.Add(new IniSection("Delegation Model"));

 //Add a new Key/Value pair within a given section
 newIni.Sections["Delegation Model"].KeyValuePair.Add("T0Admin","TheGood");

 //Change value of a existing key
 newIni.Sections["Delegation Model"].KeyValuePair["T0Admin"] = "TheGood";

 //Save file
 newIni.SaveFile(@"C:\Temp\File.inf");

 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EguibarIT.Delegation.Other
{
    /// <summary>
    ///
    /// </summary>
    public class IniFile
    {
        /// <summary>
        /// The full path and name of the ini file.
        /// Example:
        /// "C:\folder\file.ini"
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// A dictionary of sections within the INI file.
        /// </summary>
        public IniSections Sections { get; set; }

        /// <summary>
        /// A dictionary of KeyValuePair not contained within a specific section.
        /// </summary>
        public IniKeyValuePair KeyValuePair { get; set; }

        /// <summary>
        /// Creates a new, empty, IniFile object.
        /// </summary>
        public IniFile()
        {
            Sections = new IniSections();
            KeyValuePair = new IniKeyValuePair();
            FilePath = "";
        }

        /// <summary>
        /// Creates a new IniFile object from the given INI file.
        /// </summary>
        /// <param name="filePath"></param>
        public IniFile(string filePath) : this()
        {
            ReadFile(filePath);
        }//end IniFile

        /// <summary>
        /// Populate IniFile object from the INI file
        /// </summary>
        /// <param name="filePath"></param>
        public void ReadFile(string filePath)
        {
            string[] iniLines = File.ReadAllLines(filePath, Encoding.UTF8);

            IniKeyValuePair currentSection = KeyValuePair;

            foreach (string line in iniLines)
            {
                //Check if line is empty. Continue if false
                if (line != "")
                {
                    // Identify if section
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        //remove section's delimiters to get just the name
                        IniSection section = new IniSection(line.Substring(1, line.Length - 2).Trim());

                        currentSection = section.KeyValuePair;
                        Sections.Add(section);
                    }//end if section
                    else
                    {
                        //Check if line is comment
                        if (line.StartsWith(";") || line.StartsWith("#"))
                        {
                            // assuming comments start with the semicolon or #
                            // do nothing
                        }
                        else
                        {
                            string key = null;
                            string value = null;

                            //Split line into key/value
                            string[] keyPair = line.Split(new char[] { '=' }, 2);

                            //keys are stored as uppercase
                            key = keyPair[0].Trim();

                            if (keyPair.Length > 1)
                            {
                                value = keyPair[1].Trim();
                            }

                            currentSection.Add(key, value);
                        }//end else comment
                    }//end else section
                }//end if line empty
            }//end foreach
        }

        /// <summary>
        /// Attempts to save the INI file record to its associated physical file.
        /// </summary>
        public void SaveFile()
        {
            if (String.IsNullOrEmpty(FilePath))
                throw new InvalidOperationException("This INI record has no associated file.");
            SaveFile(FilePath);
        }

        /// <summary>
        /// Attempts to save the INI file record to the specified physical
        /// file. It is created if it does not exist.
        /// </summary>
        /// <param name="filePath">
        /// The full path to the file which will be created or overwritten.
        /// </param>
        public bool SaveFile(string filePath)
        {
            bool result = true;
            try
            {
                //file is not appended to. Default encoding is UTF8
                System.IO.TextWriter tw = new System.IO.StreamWriter(filePath, false, Encoding.UTF8);

                // Check that parameters without heading do exist
                if (this.KeyValuePair.Count > 0)
                {
                    //the first written parameters are the ones without a heading
                    foreach (KeyValuePair<string, string> parameter in this.KeyValuePair)
                    {
                        if (parameter.Value == null)
                        {
                            tw.WriteLine(parameter.Key);
                        }
                        else
                        {
                            tw.WriteLine("{0}={1}", parameter.Key, parameter.Value);
                        }
                    }
                }

                //there may be multiple sections, so loop through all of them
                foreach (IniSection section in this.Sections.Values)
                {
                    tw.WriteLine("[{0}]", section.SectionName);

                    //most INI conventions state that keys are case insensitive, and this
                    //will convert them to uppercase for clarity.
                    foreach (KeyValuePair<string, string> parameter in section.KeyValuePair)
                    {
                        if (parameter.Value == null)
                        {
                            tw.WriteLine(parameter.Key);
                        }
                        else
                        {
                            tw.WriteLine("{0}={1}", parameter.Key, parameter.Value);
                        }
                    }
                    tw.WriteLine();
                }
                tw.Close();
            }
            catch (Exception)
            {
                result = false;
            }

            return (result);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool WriteAllText()
        {
            bool result = false;

            if (this.FilePath != null)
            {
                WriteAllText(this.FilePath);
                result = true;
            }
            else
            {
                result = false;
                throw new InvalidOperationException("there is no path to save file.");
            }
            return result;
        }

        /// <summary>
        /// Can't use IniFile.Save to save, generates an error when GPO console try to read the file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool WriteAllText(string filePath)
        {
            bool result = true;
            string OutText = "";

            try
            {
                // Check that parameters without heading do exist
                if (this.KeyValuePair.Count > 0)
                {
                    //the first written parameters are the ones without a heading
                    foreach (KeyValuePair<string, string> parameter in this.KeyValuePair)
                    {
                        if (parameter.Value == null)
                        {
                            OutText += parameter.Key;
                        }
                        else
                        {
                            OutText += string.Format("{0}={1}", parameter.Key, parameter.Value);
                        }
                        //New line
                        OutText += Environment.NewLine; //newline to represent new pair
                    }//end foreach
                }//end if

                //there may be multiple sections, so loop through all of them
                foreach (IniSection section in this.Sections.Values)
                {
                    OutText += string.Format("[{0}]", section.SectionName);
                    //New line
                    //OutText += "\n"; //newline to represent new pair
                    OutText += Environment.NewLine; //newline to represent new pair

                    //most INI conventions state that keys are case insensitive, and this
                    //will convert them to uppercase for clarity.
                    foreach (KeyValuePair<string, string> parameter in section.KeyValuePair)
                    {
                        if (parameter.Value == null)
                        {
                            OutText += parameter.Key;
                        }
                        else
                        {
                            OutText += string.Format("{0}={1}", parameter.Key, parameter.Value);
                        }

                        //New line
                        OutText += Environment.NewLine; //newline to represent new pair
                    }//end foreach

                    //New line
                    OutText += Environment.NewLine; //newline to represent new pair
                }//end foreach

                // Finally, save the file
                System.IO.File.WriteAllText(filePath, OutText);
            }
            catch (Exception)
            {
                result = false;
            }

            return (result);
        }

        /// <summary>
        /// Gets or sets the setting with name key in the specified section.
        /// If it does not exist setting it will create it and getting it will return null.
        /// </summary>
        /// <param name="sectionName">name of section to place key in.</param>
        /// <param name="key">the unique key name linked to this setting</param>
        /// <returns>value associated with key or null if it does not exist</returns>
        public string this[string sectionName, string key]
        {
            get
            {
                if (Sections.ContainsKey(sectionName) &&
                    Sections[sectionName].KeyValuePair.ContainsKey(key))
                    return Sections[sectionName].KeyValuePair[key];
                else
                    return null;
            }
            set
            {
                if (Sections.ContainsKey(sectionName))
                {
                    IniKeyValuePair p = Sections[sectionName].KeyValuePair;
                    if (p.ContainsKey(key))
                        p[key] = value;
                    else
                        p.Add(key, value);
                }
                else
                {
                    IniSection section = new IniSection(sectionName);
                    section.KeyValuePair.Add(key, value);
                    Sections.Add(section);
                }
            }
        }

        /// <summary>
        /// Gets or sets the setting with name key in the INI files uncategorized paramaters.
        /// </summary>
        /// <param name="key">the unique key name linked to this setting</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (KeyValuePair.ContainsKey(key))
                    return KeyValuePair[key];
                else
                    return null;
            }
            set
            {
                if (KeyValuePair.ContainsKey(key))
                    KeyValuePair[key] = value;
                else
                    KeyValuePair.Add(key, value);
            }
        }
    }//end class

    //-------------Classes that are used with the INI file--------------//

    //just a section with a name and KeyValuePair

    /// <summary>
    ///
    /// </summary>
    public class IniSection
    {
        /// <summary>
        ///
        /// </summary>
        public IniKeyValuePair KeyValuePair { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sectionName"></param>
        public IniSection(string sectionName)
        {
            SectionName = sectionName;
            KeyValuePair = new IniKeyValuePair();
        }
    }

    //customized dictionary with uppercase key

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class IniSections : Dictionary<string, IniSection>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="section"></param>
        public void Add(IniSection section)
        {
            base.Add(section.SectionName, section);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="section"></param>
        new public void Add(string name, IniSection section)
        {
            if (section.SectionName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentOutOfRangeException("Name must be same as section name");
            else
                Add(section);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new public bool ContainsKey(string key)
        {
            return base.ContainsKey(key);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new public IniSection this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }
    }

    //basically a string dictionary with uppercase keys
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class IniKeyValuePair : Dictionary<string, string>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        new public void Add(string key, string value)
        {
            base.Add(key, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new public bool ContainsKey(string key)
        {
            return base.ContainsKey(key);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new public string this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }
    }
}