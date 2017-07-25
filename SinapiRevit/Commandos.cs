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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace SinapiRevit
{
    public abstract class Config
    {
        public enum Unidade
        {
            Quantidade,
            Linear,
            Area,
            Volume
        }

        public struct CategoriaSuportada
        {
            public CategoriaSuportada(Type tipo, Type instancia, BuiltInCategory categoria, Unidade unidade)
            {
                Tipo = tipo;
                Instancia = instancia;
                Categoria = categoria;
                Unidade = unidade;
            }

            public Type Tipo { get; private set; }
            public Type Instancia { get; private set; }
            public BuiltInCategory Categoria { get; private set; }
            public Unidade Unidade { get; private set; }
        }

        public static CategoriaSuportada[] CategoriasSuportadas()
        {
            return new CategoriaSuportada[] {
                new CategoriaSuportada(typeof(WallType), typeof(Wall), BuiltInCategory.OST_Walls, Unidade.Area),
                new CategoriaSuportada(typeof(FamilySymbol), typeof(FamilyInstance), BuiltInCategory.OST_Doors, Unidade.Quantidade),
                new CategoriaSuportada(typeof(FamilySymbol), typeof(FamilyInstance), BuiltInCategory.OST_Windows, Unidade.Quantidade)
            };
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdAplicarParametros : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Application app = commandData.Application.Application;
            CategorySet catSet = app.Create.NewCategorySet();
            foreach (Config.CategoriaSuportada categoria in Config.CategoriasSuportadas())
                catSet.Insert(doc.Settings.Categories.get_Item(categoria.Categoria));

            SharedParameterFunctions.OpenOrCreateParameter(doc, "SINAPI", "SinapiRevit", catSet);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdAtribuirParametros : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdExtrairDados : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<ElementId> naoEspecificados = new List<ElementId>();

            Dictionary<string, double> items = new Dictionary<string, double>();
            foreach (Config.CategoriaSuportada categoria in Config.CategoriasSuportadas())
                naoEspecificados.AddRange(BuscarItems(items, doc, categoria.Instancia, categoria.Categoria, categoria.Unidade));

            return Result.Succeeded;
        }

        private List<ElementId> BuscarItems(Dictionary<string, double> items, Document doc, Type typeInstancias, BuiltInCategory categoria, Config.Unidade unidade)
        {
     

            List<ElementId> naoEspecificados = new List<ElementId>();

                FilteredElementCollector instancias = new FilteredElementCollector(doc);
                instancias.OfCategory(categoria);
                instancias.OfClass(typeInstancias);


            foreach (Element instancia in instancias)
            {
                if (instancia.GetParameters("SINAPI").Count != 1) continue;
                string sinapi = instancia.GetParameters("SINAPI")[0].AsString();
                if (string.IsNullOrWhiteSpace(sinapi))
                {
                    naoEspecificados.Add(instancia.Id);
                    continue;
                }
                if (!items.ContainsKey(sinapi)) items.Add(sinapi, 0);

                double quantidade = 0;
                switch (unidade)
                {
                    case Config.Unidade.Quantidade:
                        quantidade++;
                        break;
                    case Config.Unidade.Area:
                        quantidade += instancia.GetParameters("Area")[0].AsDouble();
                        break;
                    case Config.Unidade.Linear:
                        quantidade += instancia.GetParameters("Height")[0].AsDouble();
                        break;
                    case Config.Unidade.Volume:
                        quantidade += instancia.GetParameters("Volume")[0].AsDouble();
                        break;
                }

                items[sinapi] += quantidade;
            }
            return naoEspecificados;
        }
    }
}
