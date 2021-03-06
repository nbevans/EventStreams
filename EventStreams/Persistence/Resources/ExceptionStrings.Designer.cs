﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventStreams.Persistence.Resources {
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
    internal class ExceptionStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EventStreams.Persistence.Resources.ExceptionStrings", typeof(ExceptionStrings).Assembly);
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
        ///   Looks up a localized string similar to The stream has advanced further than expected whilst reading the body stream; it may be invalid, malformed or corrupt..
        /// </summary>
        internal static string Body_length_indicator_mismatches_actual_body_length {
            get {
                return ResourceManager.GetString("Body_length_indicator_mismatches_actual_body_length", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The sequenced SHA-1 hash at byte position {0} is invalid; the event stream has suffered fatal data corruption..
        /// </summary>
        internal static string Data_corruption_with_current_position_only {
            get {
                return ResourceManager.GetString("Data_corruption_with_current_position_only", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The sequenced SHA-1 hashes at byte positions {0} and {1} are not contiguous; the event stream has suffered fatal data corruption..
        /// </summary>
        internal static string Data_corruption_with_previous_and_current_position {
            get {
                return ResourceManager.GetString("Data_corruption_with_previous_and_current_position", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The head and tail record length values are different; the stream may be invalid, malformed or corrupt..
        /// </summary>
        internal static string Head_and_tail_indicators_mismatch {
            get {
                return ResourceManager.GetString("Head_and_tail_indicators_mismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The stream is not positioned at the start of a record as the head indicator byte is not present..
        /// </summary>
        internal static string Head_indicator_byte_not_present {
            get {
                return ResourceManager.GetString("Head_indicator_byte_not_present", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The stream has suffered irreparable data corruption. Due to the type of corruption a repair was not possible..
        /// </summary>
        internal static string Irreparable_corruption {
            get {
                return ResourceManager.GetString("Irreparable_corruption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The seed hash was injected but the number of bytes written does not match the number of bytes injected..
        /// </summary>
        internal static string Seed_hash_injected_but_unexpected_number_of_written_bytes {
            get {
                return ResourceManager.GetString("Seed_hash_injected_but_unexpected_number_of_written_bytes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The seed hash is not of a valid length; it must be {0} bytes long..
        /// </summary>
        internal static string Seed_hash_is_invalid_length {
            get {
                return ResourceManager.GetString("Seed_hash_is_invalid_length", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested stream identified as &quot;{0}&quot; does not exist within the event store named as &quot;{1}&quot;..
        /// </summary>
        internal static string Stream_not_found {
            get {
                return ResourceManager.GetString("Stream_not_found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The stream has reached the end of the current record but a tail indicator byte is not present..
        /// </summary>
        internal static string Tail_indicator_byte_not_present {
            get {
                return ResourceManager.GetString("Tail_indicator_byte_not_present", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The buffer read from the stream does not end with a tail indicator byte; the stream appears to be invalid, malformed or corrupt..
        /// </summary>
        internal static string Tail_indicator_byte_not_present_while_backtracking {
            get {
                return ResourceManager.GetString("Tail_indicator_byte_not_present_while_backtracking", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The stream is in an abruptly truncated state starting at byte position {0}. This type of corruption is usually repairable and is typically caused by a system crash, power cut or any other situation where the program or operating system was interrupted during writing of the event stream..
        /// </summary>
        internal static string Truncation_corruption {
            get {
                return ResourceManager.GetString("Truncation_corruption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unexpected number of bytes were read from the stream. Expected {0}, but got {1}..
        /// </summary>
        internal static string Unexpected_length_returned_from_stream_read_while_backtracking {
            get {
                return ResourceManager.GetString("Unexpected_length_returned_from_stream_read_while_backtracking", resourceCulture);
            }
        }
    }
}
