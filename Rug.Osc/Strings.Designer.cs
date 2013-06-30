﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18047
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rug.Osc {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Rug.Osc.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Address string may not be null or empty.
        /// </summary>
        internal static string Address_NullOrEmpty {
            get {
                return ResourceManager.GetString("Address_NullOrEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported arguemnt type &apos;{0}&apos;.
        /// </summary>
        internal static string Arguments_UnsupportedType {
            get {
                return ResourceManager.GetString("Arguments_UnsupportedType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid container address &apos;{0}&apos;.
        /// </summary>
        internal static string Container_IsValidContainerAddress {
            get {
                return ResourceManager.GetString("Container_IsValidContainerAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot match 2 address patterns.
        /// </summary>
        internal static string OscAddress_CannotMatch2AddressPatterns {
            get {
                return ResourceManager.GetString("OscAddress_CannotMatch2AddressPatterns", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot match non literal address parts.
        /// </summary>
        internal static string OscAddress_CannotMatchNonLiteral {
            get {
                return ResourceManager.GetString("OscAddress_CannotMatchNonLiteral", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The address &apos;{0}&apos; is not a valid osc address.
        /// </summary>
        internal static string OscAddress_NotAValidOscAddress {
            get {
                return ResourceManager.GetString("OscAddress_NotAValidOscAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown address part &apos;{0}&apos;.
        /// </summary>
        internal static string OscAddress_UnknownAddressPart {
            get {
                return ResourceManager.GetString("OscAddress_UnknownAddressPart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected end of message while parsing argument &apos;{0}&apos;.
        /// </summary>
        internal static string Parser_ArgumentUnexpectedEndOfMessage {
            get {
                return ResourceManager.GetString("Parser_ArgumentUnexpectedEndOfMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The packet length is not the correct size.
        /// </summary>
        internal static string Parser_InvalidSegmentLength {
            get {
                return ResourceManager.GetString("Parser_InvalidSegmentLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Address was empty.
        /// </summary>
        internal static string Parser_MissingAddressEmpty {
            get {
                return ResourceManager.GetString("Parser_MissingAddressEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Address terminator could not be found.
        /// </summary>
        internal static string Parser_MissingAddressTerminator {
            get {
                return ResourceManager.GetString("Parser_MissingAddressTerminator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Terminator could not be found while parsing argument &apos;{0}&apos;.
        /// </summary>
        internal static string Parser_MissingArgumentTerminator {
            get {
                return ResourceManager.GetString("Parser_MissingArgumentTerminator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No comma found.
        /// </summary>
        internal static string Parser_MissingComma {
            get {
                return ResourceManager.GetString("Parser_MissingComma", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type tag terminator could not be found.
        /// </summary>
        internal static string Parser_MissingTypeTag {
            get {
                return ResourceManager.GetString("Parser_MissingTypeTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected end of message.
        /// </summary>
        internal static string Parser_UnexpectedEndOfMessage {
            get {
                return ResourceManager.GetString("Parser_UnexpectedEndOfMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown argument type &apos;{0}&apos; on argument &apos;{1}&apos;.
        /// </summary>
        internal static string Parser_UnknownArgumentType {
            get {
                return ResourceManager.GetString("Parser_UnknownArgumentType", resourceCulture);
            }
        }
    }
}
