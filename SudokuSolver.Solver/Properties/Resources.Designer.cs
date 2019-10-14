﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SudokuSolver.Solver.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SudokuSolver.Solver.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Array must be 9x9..
        /// </summary>
        internal static string GridDimensionExceptionMessage {
            get {
                return ResourceManager.GetString("GridDimensionExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Column contains more than one of a single value..
        /// </summary>
        internal static string StateValidationColumnDegeneracyException {
            get {
                return ResourceManager.GetString("StateValidationColumnDegeneracyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Row contains more than one of a single value..
        /// </summary>
        internal static string StateValidationRowDegeneracyException {
            get {
                return ResourceManager.GetString("StateValidationRowDegeneracyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Square contains more than one of a single value..
        /// </summary>
        internal static string StateValidationSquareDegeneracyException {
            get {
                return ResourceManager.GetString("StateValidationSquareDegeneracyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsolved cell with no possibilities.
        /// </summary>
        internal static string UnsolvableCellException {
            get {
                return ResourceManager.GetString("UnsolvableCellException", resourceCulture);
            }
        }
    }
}