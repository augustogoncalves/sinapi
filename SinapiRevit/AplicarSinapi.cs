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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SinapiRevit
{
    public partial class AplicarSinapi : Form
    {
        public AplicarSinapi()
        {
            InitializeComponent();
        }

        public delegate void AplicarSinapiEvent(int elementId, string sinapi);

        public event AplicarSinapiEvent ElementoSelecionado;

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            if (treeElementos.SelectedNode.Nodes.Count == 0)
            {
                ElementoSelecionado.Invoke(Int32.Parse((string)treeElementos.SelectedNode.Tag), txtSinapi.Text);
            }
            else
            {
                foreach (TreeNode node in treeElementos.SelectedNode.Nodes)
                {
                    ElementoSelecionado.Invoke(Int32.Parse((string)node.Tag), txtSinapi.Text);
                }
            }
        }

        public TreeNodeCollection Tree { get { return treeElementos.Nodes; } }
    }
}
