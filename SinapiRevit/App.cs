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
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace SinapiRevit
{
    class Aplicativo : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            a.CreateRibbonTab("SinapiRevit");
            RibbonPanel painel = a.CreateRibbonPanel("SinapiRevit", "Integração");

            PushButtonData aplicarParametro = new PushButtonData("APLICAR_PARAMETRO", "Setup",
                System.Reflection.Assembly.GetExecutingAssembly().Location, typeof(CmdAplicarParametros).FullName);
            painel.AddItem(aplicarParametro);

            PushButtonData atribuirParametro = new PushButtonData("ATRIBUIR_PARAMETRO", "Atribuir",
                System.Reflection.Assembly.GetExecutingAssembly().Location, typeof(CmdAtribuirParametros).FullName);
            painel.AddItem(atribuirParametro);

            PushButtonData exportarDados = new PushButtonData("EXPORTAR_DADOS", "Exporar quantitativo",
                System.Reflection.Assembly.GetExecutingAssembly().Location, typeof(CmdExtrairDados).FullName);
            painel.AddItem(exportarDados);            

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
