/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

// Baseado em https://github.com/ADN-DevTech/PIOTM-WallOpeningArea/blob/master/SharedParameterFunctions.cs

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace SinapiRevit
{
    class SharedParameterFunctions
    {
        public static string SharedParameterFilePath
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SharedParameters.txt");
            }
        }

        public static bool OpenOrCreateParameter(Document doc, string paramName, string paramGroup, CategorySet categorySet)
        {
            Transaction trans = new Transaction(doc);
            trans.Start("Creating shared parameter " + paramName);

            // Check on one element if the parameter already exist
            FilteredElementCollector coll = new FilteredElementCollector(doc);
            coll.OfClass(typeof(Wall));
            Element ele = coll.FirstElement();
            if (ele != null)
            {
                if (ele.GetParameters(paramName).Count > 0) return true;
            }

            // Create if not exist
            try
            {
                if (!System.IO.File.Exists(SharedParameterFilePath))
                    System.IO.File.Create(SharedParameterFilePath).Close();
            }
            catch
            {
                TaskDialog.Show("Unable to create Shared Parameter file",
                  "The plug-in could not create the required shared " +
                  "parameter file at " + SharedParameterFilePath +
                  ". Command cancelled.");
                return false;
            }

            // Open the shared parameter file
            doc.Application.SharedParametersFilename = SharedParameterFilePath;
            DefinitionFile sharedParamDefFile = doc.Application.OpenSharedParameterFile();

            // Create a category set to apply the parameter
            //CategorySet categorySet = doc.Application.Create.NewCategorySet();
            //categorySet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls));
            Binding binding = doc.Application.Create.NewInstanceBinding(categorySet);

            // Create a shared parameter group
            string groupName = paramGroup;
            DefinitionGroup sharedParamDefGroup = sharedParamDefFile.Groups.get_Item(groupName);
            if (sharedParamDefGroup == null)
                sharedParamDefGroup = sharedParamDefFile.Groups.Create(groupName);

            // Create the parameter definition for small openings
            // Check if exists, create if required
            Definition paramSmallOpeningDef = sharedParamDefGroup.Definitions.get_Item(paramName);
            if (paramSmallOpeningDef == null)
            {
                ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(paramName, ParameterType.Text);
                paramSmallOpeningDef = sharedParamDefGroup.Definitions.Create(options);
            }

            // Apply parameter for small openings to walls
            doc.ParameterBindings.Insert(paramSmallOpeningDef, binding);

            trans.Commit();

            return true;
        }
    }
}
