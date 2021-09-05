
namespace System.DirectoryServices

{

    using System;

    //https://github.com/rashiph/DecompliedDotNetLibraries/blob/master/System.DirectoryServices/System/DirectoryServices/ActiveDirectorySecurityInheritance.cs

    public enum ActiveDirectorySecurityInheritance

    {

        None,

        All,

        Descendents,

        SelfAndChildren,

        Children

    }

}
