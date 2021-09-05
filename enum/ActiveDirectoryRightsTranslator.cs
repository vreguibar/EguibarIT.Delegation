namespace System.DirectoryServices

{

    using System;

    //https://github.com/rashiph/DecompliedDotNetLibraries/blob/master/System.DirectoryServices/System/DirectoryServices/ActiveDirectoryRightsTranslator.cs

    internal sealed class ActiveDirectoryRightsTranslator

    {

        internal static int AccessMaskFromRights(ActiveDirectoryRights adRights)

        {

            return (int)adRights;

        }



        internal static ActiveDirectoryRights RightsFromAccessMask(int accessMask)

        {

            return (ActiveDirectoryRights)accessMask;

        }

    }

}


