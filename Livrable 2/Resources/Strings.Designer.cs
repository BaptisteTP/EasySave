﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EasySave2._0.Resources
{
    using System;


    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings()
        {
        }

        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EasySave2._0.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Recherche une chaîne localisée semblable à OperationList.
        /// </summary>
        public static string OperationList
        {
            get
            {
                return ResourceManager.GetString("OperationList", resourceCulture);
            }
        }

        /// <summary>
        ///   Recherche une chaîne localisée semblable à RealTimeLogFolder.
        /// </summary>
        public static string RealTimeLogFolder
        {
            get
            {
                return ResourceManager.GetString("RealTimeLogFolder", resourceCulture);
            }
        }

        /// <summary>
        ///   Recherche une chaîne localisée semblable à RealTimeLogFolder.
        /// </summary>
        public static string DailyLogFolder
        {
            get
            {
                return ResourceManager.GetString("DailyLogFolder", resourceCulture);
            }
        }

        /// <summary>
        ///   Recherche une chaîne localisée semblable à RealTimeLogFolder.
        /// </summary>
        public static string ValidSave
        {
            get
            {
                return ResourceManager.GetString("ValidSave", resourceCulture);
            }
        }

        /// <summary>
        ///   Recherche une chaîne localisée semblable à RealTimeLogFolder.
        /// </summary>
        public static string WelcomeMessage
        {
            get
            {
                return ResourceManager.GetString("WelcomeMessage", resourceCulture);
            }
        }
    }
}
